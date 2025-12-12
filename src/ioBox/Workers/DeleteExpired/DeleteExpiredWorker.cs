using IOBox.Persistence;
using IOBox.TaskExecution;
using IOBox.Workers.DeleteExpired.Options;

using Microsoft.Extensions.Options;

namespace IOBox.Workers.DeleteExpired;

internal class DeleteExpiredWorker(
    string ioName,
    IDbStoreInternal dbStoreInternal,
    IOptionsMonitor<DeleteExpiredOptions> optionsMonitor,
    ITaskExecutionWrapper taskExecutionWrapper) : IDeleteExpiredWorker
{
    public Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return taskExecutionWrapper.WrapTaskAsync(
            ioName,
            cancellationToken =>
                dbStoreInternal.DeleteExpiredMessagesAsync(ioName, cancellationToken),
            optionsMonitor,
            stoppingToken);
    }
}
