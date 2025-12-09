using System.Data;

using Dapper;

namespace IOBox.SqlServer.Demo.Persistence;

internal class MembersRepository(IDbContext dbContext) : IMembersRepository
{
    public async Task<int> AddNewMemberAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var sql = @"
            INSERT INTO App.Members 
                (Email, HasBeenWelcomed, HasBeenGivenBonus) 
            VALUES 
                (@email, 0, 0);";

        var command = new CommandDefinition(
            sql,
            new { email },
            cancellationToken: cancellationToken);

        using var conn = dbContext.CreateConnection();

        return await conn.ExecuteAsync(command);
    }

    public async Task UpdateMemberAsWelcomedAsync(
        int id,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        var sql = @"
            UPDATE App.Members 
            SET HasBeenWelcomed = 1
            WHERE Id = @id";

        var command = new CommandDefinition(
            sql,
            new { id },
            transaction,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);
    }

    public async Task UpdateMemberAsReceivedBonusAsync(
        int id,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        var sql = @"
            UPDATE App.Members 
            SET HasBeenGivenBonus = 1
            WHERE Id = @id";

        var command = new CommandDefinition(
            sql,
            new { id },
            transaction,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);
    }
}
