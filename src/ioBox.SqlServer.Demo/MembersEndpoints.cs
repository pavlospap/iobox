using System.Transactions;

using IOBox.Persistence;
using IOBox.SqlServer.Demo.Persistence;

using IDbContext = IOBox.SqlServer.Demo.Persistence.IDbContext;

namespace IOBox.SqlServer.Demo;

internal static class MembersEndpoints
{
    public static WebApplication AddMembersEndpoints(this WebApplication app)
    {
        app.MapPost("members", async (
            IDbStore dbStore,
            IDbContext dbContext,
            IMembersRepository repository,
            NewMemberInsertDto member,
            CancellationToken cancellationToken = default) =>
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            using var connection = dbContext.CreateConnection();

            var memberId = await repository.AddNewMemberAsync(
                member.Email,
                cancellationToken);

            var welcomeMessageId = Guid.NewGuid().ToString();

            await dbStore.AddNewMessageAsync(
                ioName: "Outbox01",
                messageId: welcomeMessageId,
                payload: JsonSerializer.Serialize(new NewMemberWelcomeMessage(
                    welcomeMessageId,
                    memberId,
                    member.Email)),
                contextInfo: "POST: /members",
                connection: null,
                transaction: null,
                cancellationToken: cancellationToken);

            var bonusMessageId = Guid.NewGuid().ToString();

            await dbStore.AddNewMessageAsync(
                ioName: "Outbox02",
                messageId: bonusMessageId,
                payload: JsonSerializer.Serialize(new NewMemberBonusMessage(
                    bonusMessageId,
                    memberId,
                    member.Email)),
                contextInfo: "POST: /members",
                connection: null,
                transaction: null,
                cancellationToken: cancellationToken);

            scope.Complete();

            return Results.Ok();
        });

        return app;
    }
}
