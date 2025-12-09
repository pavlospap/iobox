using System.Data;

namespace IOBox.SqlServer.Demo.Persistence;

internal interface IMembersRepository
{
    Task<int> AddNewMemberAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task UpdateMemberAsWelcomedAsync(
        int id,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default);

    Task UpdateMemberAsReceivedBonusAsync(
        int id,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default);
}
