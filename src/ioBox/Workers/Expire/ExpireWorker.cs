using IOBox.Persistence;
using IOBox.TaskExecution;
using IOBox.Workers.Expire.Options;

using Microsoft.Extensions.Options;

namespace IOBox.Workers.Expire;

internal class ExpireWorker(
    string ioName,
    IDbStoreInternal dbStoreInternal,
    IOptionsMonitor<ExpireOptions> optionsMonitor,
    ITaskExecutionWrapper taskExecutionWrapper) : IExpireWorker
{
    public Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return taskExecutionWrapper.WrapTaskAsync(
            ioName,
            cancellationToken =>
                dbStoreInternal.MarkMessagesAsExpiredAsync(ioName, cancellationToken),
            optionsMonitor,
            stoppingToken);
    }
}
