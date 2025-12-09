using IOBox.Persistence;
using IOBox.TaskExecution;
using IOBox.Workers.Archive.Options;

using Microsoft.Extensions.Options;

namespace IOBox.Workers.Archive;

internal class ArchiveWorker(
    string ioName,
    IDbStoreInternal dbStoreInternal,
    IOptionsMonitor<ArchiveOptions> optionsMonitor,
    ITaskExecutionWrapper taskExecutionWrapper) : IArchiveWorker
{
    public Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return taskExecutionWrapper.WrapTaskAsync(
            ioName,
            cancellationToken =>
                dbStoreInternal.ArchiveMessagesAsync(ioName, cancellationToken),
            optionsMonitor,
            stoppingToken);
    }
}
