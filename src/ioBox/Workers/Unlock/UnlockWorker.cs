using IOBox.Persistence;
using IOBox.TaskExecution;
using IOBox.Workers.Unlock.Options;

using Microsoft.Extensions.Options;

namespace IOBox.Workers.Unlock;

internal class UnlockWorker(
    string ioName,
    IDbStoreInternal dbStoreInternal,
    IOptionsMonitor<UnlockOptions> optionsMonitor,
    ITaskExecutionWrapper taskExecutionWrapper) : IUnlockWorker
{
    public Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return taskExecutionWrapper.WrapTaskAsync(
            ioName,
            cancellationToken =>
                dbStoreInternal.UnlockMessagesAsync(ioName, cancellationToken),
            optionsMonitor,
            stoppingToken);
    }
}
