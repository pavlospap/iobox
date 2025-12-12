using IOBox.Persistence;
using IOBox.TaskExecution;
using IOBox.Workers.ArchiveExpired.Options;

using Microsoft.Extensions.Options;

namespace IOBox.Workers.ArchiveExpired;

internal class ArchiveExpiredWorker(
    string ioName,
    IDbStoreInternal dbStoreInternal,
    IOptionsMonitor<ArchiveExpiredOptions> optionsMonitor,
    ITaskExecutionWrapper taskExecutionWrapper) : IArchiveExpiredWorker
{
    public Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return taskExecutionWrapper.WrapTaskAsync(
            ioName,
            cancellationToken =>
                dbStoreInternal.ArchiveExpiredMessagesAsync(ioName, cancellationToken),
            optionsMonitor,
            stoppingToken);
    }
}
