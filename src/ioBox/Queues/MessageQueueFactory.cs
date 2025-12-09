using Microsoft.Extensions.DependencyInjection;

namespace IOBox.Queues;

internal class MessageQueueFactory(IServiceProvider serviceProvider) :
    IMessageQueueFactory
{
    public IMessageQueue GetOrCreate(string ioName) =>
        serviceProvider.GetKeyedService<IMessageQueue>(ioName)!;
}
