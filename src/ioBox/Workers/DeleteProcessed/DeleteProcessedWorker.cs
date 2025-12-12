using IOBox.Persistence;
using IOBox.TaskExecution;
using IOBox.Workers.DeleteProcessed.Options;

using Microsoft.Extensions.Options;

namespace IOBox.Workers.DeleteProcessed;

internal class DeleteProcessedWorker(
    string ioName,
    IDbStoreInternal dbStoreInternal,
    IOptionsMonitor<DeleteProcessedOptions> optionsMonitor,
    ITaskExecutionWrapper taskExecutionWrapper) : IDeleteProcessedWorker
{
    public Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return taskExecutionWrapper.WrapTaskAsync(
            ioName,
            cancellationToken =>
                dbStoreInternal.DeleteProcessedMessagesAsync(ioName, cancellationToken),
            optionsMonitor,
            stoppingToken);
    }
}
