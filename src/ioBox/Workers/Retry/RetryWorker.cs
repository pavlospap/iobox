using IOBox.Persistence;
using IOBox.Queues;
using IOBox.TaskExecution;
using IOBox.Workers.Retry.Options;

using Microsoft.Extensions.Options;

namespace IOBox.Workers.Retry;

internal class RetryWorker(
    string ioName,
    IDbStoreInternal dbStoreInternal,
    IOptionsMonitor<RetryOptions> optionsMonitor,
    IMessageQueueFactory messageQueueFactory,
    ITaskExecutionWrapper taskExecutionWrapper) : IRetryWorker
{
    public Task ExecuteAsync(CancellationToken stoppingToken)
    {
        async Task retryAsync(CancellationToken cancellationToken)
        {
            var messages = await dbStoreInternal.GetMessagesToRetryAsync(
                ioName, cancellationToken);

            messageQueueFactory.GetOrCreate(ioName).EnqueueBatch(messages);
        }

        return taskExecutionWrapper.WrapTaskAsync(
            ioName,
            retryAsync,
            optionsMonitor,
            stoppingToken);
    }
}
