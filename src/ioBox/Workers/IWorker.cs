namespace IOBox.Workers;

internal interface IWorker
{
    Task ExecuteAsync(CancellationToken stoppingToken);
}
