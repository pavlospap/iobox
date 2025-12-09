using IOBox.Workers;

using Microsoft.Extensions.Hosting;

namespace IOBox;

internal class MessageBackgroundService(IEnumerable<IWorker> workers) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var tasks = workers.Select(w => w.ExecuteAsync(stoppingToken));

        await Task.WhenAll(tasks);
    }
}
