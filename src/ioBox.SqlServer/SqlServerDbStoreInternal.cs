using System.Data;

using Dapper;

using IOBox.Persistence;
using IOBox.Persistence.Options;
using IOBox.Workers.Archive.Options;
using IOBox.Workers.Delete.Options;
using IOBox.Workers.ExpireFailed.Options;
using IOBox.Workers.ExpireNew.Options;
using IOBox.Workers.Poll.Options;
using IOBox.Workers.Retry.Options;
using IOBox.Workers.Unlock.Options;

using Microsoft.Extensions.Options;

namespace IOBox.SqlServer;

internal class SqlServerDbStoreInternal(
    IDbContext dbContext,
    IOptionsMonitor<DbOptions> dbOptionsMonitor,
    IOptionsMonitor<PollOptions> pollOptionsMonitor,
    IOptionsMonitor<RetryOptions> retryPollOptionsMonitor,
    IOptionsMonitor<UnlockOptions> unlockOptionsMonitor,
    IOptionsMonitor<ExpireNewOptions> expireNewOptionsMonitor,
    IOptionsMonitor<ExpireFailedOptions> expireFailedOptionsMonitor,
    IOptionsMonitor<ArchiveOptions> archiveOptionsMonitor,
    IOptionsMonitor<DeleteOptions> deleteOptionsMonitor) : IDbStoreInternal
{
    public async Task<IEnumerable<Message>> GetMessagesToProcessAsync(
        string ioName,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ioName, nameof(ioName));

        var sql = $@"
            WITH NewMessages AS ( 
                SELECT TOP (@BatchSize) * 
                FROM {dbOptionsMonitor.Get(ioName).FullTableName} WITH (ROWLOCK, UPDLOCK, READPAST) 
                WHERE Status = {MessageStatus.New} 
                ORDER BY ReceivedAt 
            ) 
            UPDATE NewMessages 
            SET 
                Status = {MessageStatus.Locked}, 
                LockedAt = SYSUTCDATETIME() 
            OUTPUT 
                INSERTED.Id, 
                INSERTED.MessageId, 
                INSERTED.Payload, 
                INSERTED.ContextInfo;";

        using var connection = dbContext.CreateConnection(ioName);

        connection.Open();

        using var transaction = connection.BeginTransaction(
            IsolationLevel.ReadCommitted);

        var command = new CommandDefinition(
            sql,
            new { pollOptionsMonitor.Get(ioName).BatchSize },
            transaction,
            cancellationToken: cancellationToken);

        var messages = await connection.QueryAsync<Message>(command);

        transaction.Commit();

        return messages;
    }

    public async Task<IEnumerable<Message>> GetMessagesToRetryAsync(
        string ioName,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ioName, nameof(ioName));

        var sql = $@"
            WITH FailedMessages AS ( 
                SELECT TOP (@BatchSize) * 
                FROM {dbOptionsMonitor.Get(ioName).FullTableName} WITH (ROWLOCK, UPDLOCK, READPAST) 
                WHERE Status = {MessageStatus.Failed} AND 
                      Retries < @Limit 
                ORDER BY FailedAt 
            ) 
            UPDATE FailedMessages 
            SET 
                Status = {MessageStatus.Locked}, 
                LockedAt = SYSUTCDATETIME(), 
                Retries = Retries + 1 
            OUTPUT 
                INSERTED.Id, 
                INSERTED.MessageId,
                INSERTED.Payload,
                INSERTED.ContextInfo;";

        using var connection = dbContext.CreateConnection(ioName);

        connection.Open();

        using var transaction = connection.BeginTransaction(
            IsolationLevel.ReadCommitted);

        var retryPollOptions = retryPollOptionsMonitor.Get(ioName);

        var command = new CommandDefinition(
            sql,
            new
            {
                retryPollOptions.BatchSize,
                retryPollOptions.Limit
            },
            transaction,
            cancellationToken: cancellationToken);

        var messages = await connection.QueryAsync<Message>(command);

        transaction.Commit();

        return messages;
    }

    public async Task UnlockMessagesAsync(
        string ioName,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ioName, nameof(ioName));

        var sql = $@"
            WITH LockedMessages AS ( 
                SELECT TOP (@BatchSize) * 
                FROM {dbOptionsMonitor.Get(ioName).FullTableName} WITH (ROWLOCK, UPDLOCK, READPAST) 
                WHERE Status = {MessageStatus.Locked} AND 
                      LockedAt <= DATEADD(MILLISECOND, -@Timeout, SYSUTCDATETIME()) 
                ORDER BY LockedAt 
            ) 
            UPDATE LockedMessages 
            SET 
                Status = {MessageStatus.Failed}, 
                FailedAt = SYSUTCDATETIME();";

        using var connection = dbContext.CreateConnection(ioName);

        connection.Open();

        using var transaction = connection.BeginTransaction(
            IsolationLevel.ReadCommitted);

        var command = new CommandDefinition(
            sql,
            new
            {
                retryPollOptionsMonitor.Get(ioName).BatchSize,
                unlockOptionsMonitor.Get(ioName).Timeout
            },
            transaction,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        transaction.Commit();
    }

    public async Task MarkNewMessagesAsExpiredAsync(
        string ioName,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ioName, nameof(ioName));

        var sql = $@"
            WITH MessagesToExpire AS ( 
                SELECT TOP (@BatchSize) * 
                FROM {dbOptionsMonitor.Get(ioName).FullTableName} WITH (ROWLOCK, UPDLOCK, READPAST) 
                WHERE Status = {MessageStatus.New} AND 
                      ReceivedAt <= DATEADD(MILLISECOND, -@Ttl, SYSUTCDATETIME())
                ORDER BY ReceivedAt
            ) 
            UPDATE MessagesToExpire 
            SET 
                Status = {MessageStatus.Expired}, 
                ExpiredAt = SYSUTCDATETIME();";

        using var connection = dbContext.CreateConnection(ioName);

        connection.Open();

        using var transaction = connection.BeginTransaction(
            IsolationLevel.ReadCommitted);

        var expireNewOptions = expireNewOptionsMonitor.Get(ioName);

        var command = new CommandDefinition(
            sql,
            new
            {
                expireNewOptions.BatchSize,
                expireNewOptions.Ttl
            },
            transaction,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        transaction.Commit();
    }

    public async Task MarkFailedMessagesAsExpiredAsync(
        string ioName,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ioName, nameof(ioName));

        var sql = $@"
            WITH MessagesToExpire AS ( 
                SELECT TOP (@BatchSize) * 
                FROM {dbOptionsMonitor.Get(ioName).FullTableName} WITH (ROWLOCK, UPDLOCK, READPAST) 
                WHERE Status = {MessageStatus.Failed} AND 
                      FailedAt <= DATEADD(MILLISECOND, -@Ttl, SYSUTCDATETIME())
                ORDER BY FailedAt
            ) 
            UPDATE MessagesToExpire 
            SET 
                Status = {MessageStatus.Expired}, 
                ExpiredAt = SYSUTCDATETIME();";

        using var connection = dbContext.CreateConnection(ioName);

        connection.Open();

        using var transaction = connection.BeginTransaction(
            IsolationLevel.ReadCommitted);

        var expireFailedOptions = expireFailedOptionsMonitor.Get(ioName);

        var command = new CommandDefinition(
            sql,
            new
            {
                expireFailedOptions.BatchSize,
                expireFailedOptions.Ttl
            },
            transaction,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        transaction.Commit();
    }

    public async Task ArchiveMessagesAsync(
        string ioName,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ioName, nameof(ioName));

        var dbOptions = dbOptionsMonitor.Get(ioName);

        var table = dbOptions.FullTableName;

        var sql = $@"
            DECLARE @MessagesToArchive TABLE (Id int);

            INSERT INTO @MessagesToArchive (Id)
            SELECT TOP (@BatchSize) Id 
            FROM {table} WITH (ROWLOCK, UPDLOCK, READPAST) 
            WHERE 
                (Status = {MessageStatus.Processed} AND 
                 ProcessedAt <= DATEADD(MILLISECOND, -@ProcessedMessageTtl, SYSUTCDATETIME())) OR
                (Status = {MessageStatus.Expired} AND 
                 ExpiredAt <= DATEADD(MILLISECOND, -@ExpiredMessageTtl, SYSUTCDATETIME()))
            ORDER BY 
                CASE 
                    WHEN Status = {MessageStatus.Processed} THEN ProcessedAt
                    WHEN Status = {MessageStatus.Expired} THEN ExpiredAt
                END;
            
            INSERT INTO {dbOptions.ArchiveFullTableName} (
                MessageId,
                Payload,
                ContextInfo,
                Status,
                Retries,
                Error,
                ReceivedAt,
                LockedAt,
                ProcessedAt,
                FailedAt,
                ExpiredAt)
            SELECT
                MessageId,
                Payload,
                ContextInfo,
                Status,
                Retries,
                Error,
                ReceivedAt,
                LockedAt,
                ProcessedAt,
                FailedAt,
                ExpiredAt
            FROM {table}
            WHERE Id IN (SELECT Id FROM @MessagesToArchive);

            DELETE FROM {table}
            WHERE Id IN (SELECT Id FROM @MessagesToArchive);";

        using var connection = dbContext.CreateConnection(ioName);

        connection.Open();

        using var transaction = connection.BeginTransaction(
            IsolationLevel.ReadCommitted);

        var archiveOptions = archiveOptionsMonitor.Get(ioName);

        var command = new CommandDefinition(
            sql,
            new
            {
                archiveOptions.BatchSize,
                archiveOptions.ProcessedMessageTtl,
                archiveOptions.ExpiredMessageTtl
            },
            transaction,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        transaction.Commit();
    }

    public async Task DeleteMessagesAsync(
        string ioName,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ioName, nameof(ioName));

        var sql = $@"
            WITH MessagesToDelete AS ( 
                SELECT TOP (@BatchSize) * 
                FROM {dbOptionsMonitor.Get(ioName).FullTableName} WITH (ROWLOCK, UPDLOCK, READPAST) 
                WHERE 
                    (Status = {MessageStatus.Processed} AND 
                     ProcessedAt <= DATEADD(MILLISECOND, -@ProcessedMessageTtl, SYSUTCDATETIME())) OR
                    (Status = {MessageStatus.Expired} AND 
                     ExpiredAt <= DATEADD(MILLISECOND, -@ExpiredMessageTtl, SYSUTCDATETIME()))
                ORDER BY 
                    CASE 
                        WHEN Status = {MessageStatus.Processed} THEN ProcessedAt
                        WHEN Status = {MessageStatus.Expired} THEN ExpiredAt
                    END
            ) 
            DELETE FROM MessagesToDelete;";

        using var connection = dbContext.CreateConnection(ioName);

        connection.Open();

        using var transaction = connection.BeginTransaction(
            IsolationLevel.ReadCommitted);

        var deleteOptions = deleteOptionsMonitor.Get(ioName);

        var command = new CommandDefinition(
            sql,
            new
            {
                deleteOptions.BatchSize,
                deleteOptions.ProcessedMessageTtl,
                deleteOptions.ExpiredMessageTtl
            },
            transaction,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);

        transaction.Commit();
    }
}
