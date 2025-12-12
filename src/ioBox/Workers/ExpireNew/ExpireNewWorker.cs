using IOBox.Persistence;
using IOBox.TaskExecution;
using IOBox.Workers.ExpireNew.Options;

using Microsoft.Extensions.Options;

namespace IOBox.Workers.ExpireNew;

internal class ExpireNewWorker(
    string ioName,
    IDbStoreInternal dbStoreInternal,
    IOptionsMonitor<ExpireNewOptions> optionsMonitor,
    ITaskExecutionWrapper taskExecutionWrapper) : IExpireNewWorker
{
    public Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return taskExecutionWrapper.WrapTaskAsync(
            ioName,
            cancellationToken =>
                dbStoreInternal.MarkNewMessagesAsExpiredAsync(ioName, cancellationToken),
            optionsMonitor,
            stoppingToken);
    }
}
