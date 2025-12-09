using IOBox.Persistence;
using IOBox.TaskExecution;
using IOBox.Workers.Delete.Options;

using Microsoft.Extensions.Options;

namespace IOBox.Workers.Delete;

internal class DeleteWorker(
    string ioName,
    IDbStoreInternal dbStoreInternal,
    IOptionsMonitor<DeleteOptions> optionsMonitor,
    ITaskExecutionWrapper taskExecutionWrapper) : IDeleteWorker
{
    public Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return taskExecutionWrapper.WrapTaskAsync(
            ioName,
            cancellationToken =>
                dbStoreInternal.DeleteMessagesAsync(ioName, cancellationToken),
            optionsMonitor,
            stoppingToken);
    }
}
