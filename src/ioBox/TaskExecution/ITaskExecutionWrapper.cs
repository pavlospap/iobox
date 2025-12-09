using IOBox.TaskExecution.Options;

using Microsoft.Extensions.Options;

namespace IOBox.TaskExecution;

internal interface ITaskExecutionWrapper
{
    Task WrapTaskAsync<TOptions>(
        string ioName,
        Func<CancellationToken, Task> task,
        IOptionsMonitor<TOptions> optionsMonitor,
        CancellationToken stoppingToken) where TOptions : ITaskExecutionOptions;
}
