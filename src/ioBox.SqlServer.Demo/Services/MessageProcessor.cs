using EasyNetQ;

using IOBox.Persistence;
using IOBox.SqlServer.Demo.Persistence;

using IDbContext = IOBox.SqlServer.Demo.Persistence.IDbContext;

namespace IOBox.SqlServer.Demo.Services;

internal class MessageProcessor(
   IBus bus,
   IDbStore dbStore,
   IDbContext dbContext,
   IMembersRepository repository) : IMessageProcessor
{
    public async Task ProcessMessagesAsync(
        string ioName,
        List<Message> messages,
        CancellationToken cancellationToken = default)
    {
        foreach (var message in messages)
        {
            try
            {
                using var connection = dbContext.CreateConnection();

                connection.Open();

                using var transaction = connection.BeginTransaction();

                switch (ioName)
                {
                    case "Inbox01":
                        {
                            var msg = JsonSerializer.Deserialize<NewMemberWelcomeMessage>(
                                message.Payload);

                            await repository.UpdateMemberAsWelcomedAsync(
                                msg!.MemberId,
                                connection,
                                transaction,
                                cancellationToken);

                            break;
                        }
                    case "Inbox02":
                        {
                            var msg = JsonSerializer.Deserialize<NewMemberBonusMessage>(
                                message.Payload);

                            await repository.UpdateMemberAsReceivedBonusAsync(
                                msg!.MemberId,
                                connection,
                                transaction,
                                cancellationToken);

                            break;
                        }
                    case "Outbox01":
                        {
                            var msg = JsonSerializer.Deserialize<NewMemberWelcomeMessage>(
                                message.Payload);

                            await bus.SendReceive.SendAsync(
                                nameof(NewMemberWelcomeMessage), msg, cancellationToken);

                            break;
                        }
                    case "Outbox02":
                        {
                            var msg = JsonSerializer.Deserialize<NewMemberBonusMessage>(
                                message.Payload);

                            await bus.SendReceive.SendAsync(
                                nameof(NewMemberBonusMessage), msg, cancellationToken);

                            break;
                        }

                    default:
                        throw new NotSupportedException(
                            $"Message processing for {ioName} is not supported.");
                }

                await dbStore.MarkMessageAsProcessedAsync(
                    ioName,
                    message.Id,
                    connection,
                    transaction,
                    cancellationToken);

                transaction.Commit();
            }
            catch (Exception ex)
            {
                await dbStore.MarkMessageAsFailedAsync(
                    ioName,
                    message.Id,
                    ex.Message,
                    cancellationToken: cancellationToken);
            }
        }
    }
}
