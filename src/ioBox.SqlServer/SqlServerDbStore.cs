using System.Data;

using Dapper;

using IOBox.Persistence;
using IOBox.Persistence.Options;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IOBox.SqlServer;

internal class SqlServerDbStore(
    IDbContext dbContext,
    ILogger<SqlServerDbStore> logger,
    IOptionsMonitor<DbOptions> dbOptionsMonitor) : IDbStore
{
    private const short UniqueIndexViolation = 2601;

    public Task AddNewMessageAsync(
        string ioName,
        string messageId,
        string payload,
        string? contextInfo = null,
        IDbConnection? connection = null,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ioName, nameof(ioName));

        ArgumentException.ThrowIfNullOrWhiteSpace(messageId, nameof(messageId));

        ArgumentException.ThrowIfNullOrWhiteSpace(payload, nameof(payload));

        if (transaction is not null && connection is null)
        {
            throw new ArgumentException(
                "A transaction was provided without a corresponding connection.",
                nameof(transaction));
        }

        var sql = $@"
            INSERT INTO {dbOptionsMonitor.Get(ioName).FullTableName} (
                MessageId, 
                Payload, 
                ContextInfo, 
                Status, 
                ReceivedAt, 
                Retries) 
            VALUES (
                @messageId, 
                @payload, 
                @contextInfo, 
                {MessageStatus.New}, 
                SYSUTCDATETIME(), 
                0);";

        var command = new CommandDefinition(
            sql,
            new { messageId, payload, contextInfo },
            transaction,
            cancellationToken: cancellationToken);

        try
        {
            return ExecuteAsync(ioName, command, connection);
        }
        catch (SqlException ex) when (ex.Number == UniqueIndexViolation)
        {
            if (logger.IsEnabled(LogLevel.Error))
            {
                logger.LogError(
                    ex,
                    "Message with Id: {messageId} already exists in '{ioName}'",
                    messageId,
                    ioName);
            }

            throw;
        }
    }

    public Task MarkMessageAsProcessedAsync(
        string ioName,
        int id,
        IDbConnection? connection = null,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ioName, nameof(ioName));

        if (transaction is not null && connection is null)
        {
            throw new ArgumentException(
                "A transaction was provided without a corresponding connection.",
                nameof(transaction));
        }

        var sql = $@"
            UPDATE {dbOptionsMonitor.Get(ioName).FullTableName} 
            SET Status = {MessageStatus.Processed}, 
                ProcessedAt = SYSUTCDATETIME()
            WHERE Id = @id;";

        var command = new CommandDefinition(
            sql,
            new { id },
            transaction,
            cancellationToken: cancellationToken);

        return ExecuteAsync(ioName, command, connection);
    }

    public Task MarkMessageAsFailedAsync(
        string ioName,
        int id,
        string? error = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ioName, nameof(ioName));

        var sql = $@"
            UPDATE {dbOptionsMonitor.Get(ioName).FullTableName} 
            SET Status = {MessageStatus.Failed},
                Error = @error,
                FailedAt = SYSUTCDATETIME()
            WHERE Id = @id;";

        var command = new CommandDefinition(
            sql,
            new { id, error },
            cancellationToken: cancellationToken);

        return ExecuteAsync(ioName, command);
    }

    private Task ExecuteAsync(
        string ioName, CommandDefinition command, IDbConnection? connection = null)
    {
        if (connection != null)
        {
            return connection.ExecuteAsync(command);
        }

        return CreateConnectionAndExecuteAsync();

        async Task CreateConnectionAndExecuteAsync()
        {
            using var connection = dbContext.CreateConnection(ioName);

            await connection.ExecuteAsync(command);
        }
    }
}
