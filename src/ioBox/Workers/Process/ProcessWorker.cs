using IOBox.Queues;
using IOBox.TaskExecution;
using IOBox.Workers.Process.Options;

using Microsoft.Extensions.Options;

namespace IOBox.Workers.Process;

internal class ProcessWorker(
    string ioName,
    IOptionsMonitor<ProcessOptions> optionsMonitor,
    IMessageQueueFactory messageQueueFactory,
    IMessageProcessor messageProcessor,
    ITaskExecutionWrapper taskExecutionWrapper) : IProcessWorker
{
    public Task ExecuteAsync(CancellationToken stoppingToken)
    {
        async Task processAsync(CancellationToken cancellationToken)
        {
            var options = optionsMonitor.Get(ioName);

            var messages = messageQueueFactory.GetOrCreate(ioName)
                .DequeueBatch(options.BatchSize);

            if (messages.Count > 0)
            {
                await messageProcessor.ProcessMessagesAsync(
                    ioName, messages, cancellationToken);
            }
        }

        return taskExecutionWrapper.WrapTaskAsync(
            ioName,
            processAsync,
            optionsMonitor,
            stoppingToken);
    }
}
