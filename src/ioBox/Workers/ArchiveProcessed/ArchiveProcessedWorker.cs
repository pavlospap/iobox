using IOBox.Persistence;
using IOBox.TaskExecution;
using IOBox.Workers.ArchiveProcessed.Options;

using Microsoft.Extensions.Options;

namespace IOBox.Workers.ArchiveProcessed;

internal class ArchiveProcessedWorker(
    string ioName,
    IDbStoreInternal dbStoreInternal,
    IOptionsMonitor<ArchiveProcessedOptions> optionsMonitor,
    ITaskExecutionWrapper taskExecutionWrapper) : IArchiveProcessedWorker
{
    public Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return taskExecutionWrapper.WrapTaskAsync(
            ioName,
            cancellationToken =>
                dbStoreInternal.ArchiveProcessedMessagesAsync(ioName, cancellationToken),
            optionsMonitor,
            stoppingToken);
    }
}
