using EasyNetQ;

using IOBox.Persistence;

namespace IOBox.SqlServer.Demo.Services;

internal class MessageListener(
    IBus bus,
    IDbStore dbStore) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await bus.SendReceive.ReceiveAsync<NewMemberWelcomeMessage>(
            nameof(NewMemberWelcomeMessage),
            async message => await dbStore.AddNewMessageAsync(
                ioName: "Inbox01",
                messageId: message.MessageId,
                payload: JsonSerializer.Serialize(message),
                contextInfo: $"Queue: {nameof(NewMemberWelcomeMessage)}",
                connection: null,
                transaction: null,
                cancellationToken: stoppingToken),
            cancellationToken: stoppingToken);

        await bus.SendReceive.ReceiveAsync<NewMemberBonusMessage>(
            nameof(NewMemberBonusMessage),
            async message => await dbStore.AddNewMessageAsync(
                ioName: "Inbox02",
                messageId: message.MessageId,
                payload: JsonSerializer.Serialize(message),
                contextInfo: $"Queue: {nameof(NewMemberBonusMessage)}",
                connection: null,
                transaction: null,
                cancellationToken: stoppingToken),
            cancellationToken: stoppingToken);
    }
}
