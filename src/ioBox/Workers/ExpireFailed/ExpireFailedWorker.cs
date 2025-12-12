using IOBox.Persistence;
using IOBox.TaskExecution;
using IOBox.Workers.ExpireFailed.Options;

using Microsoft.Extensions.Options;

namespace IOBox.Workers.ExpireFailed;

internal class ExpireFailedWorker(
    string ioName,
    IDbStoreInternal dbStoreInternal,
    IOptionsMonitor<ExpireFailedOptions> optionsMonitor,
    ITaskExecutionWrapper taskExecutionWrapper) : IExpireFailedWorker
{
    public Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return taskExecutionWrapper.WrapTaskAsync(
            ioName,
            cancellationToken =>
                dbStoreInternal.MarkFailedMessagesAsExpiredAsync(ioName, cancellationToken),
            optionsMonitor,
            stoppingToken);
    }
}
