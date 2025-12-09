using IOBox.Persistence;
using IOBox.Queues;
using IOBox.TaskExecution;
using IOBox.Workers.Poll.Options;

using Microsoft.Extensions.Options;

namespace IOBox.Workers.Poll;

internal class PollWorker(
    string ioName,
    IDbStoreInternal dbStoreInternal,
    IOptionsMonitor<PollOptions> optionsMonitor,
    IMessageQueueFactory messageQueueFactory,
    ITaskExecutionWrapper taskExecutionWrapper) : IPollWorker
{
    public Task ExecuteAsync(CancellationToken stoppingToken)
    {
        async Task pollAsync(CancellationToken cancellationToken)
        {
            var messages = await dbStoreInternal.GetMessagesToProcessAsync(
                ioName, cancellationToken);

            messageQueueFactory.GetOrCreate(ioName).EnqueueBatch(messages);
        }

        return taskExecutionWrapper.WrapTaskAsync(
            ioName,
            pollAsync,
            optionsMonitor,
            stoppingToken);
    }
}
