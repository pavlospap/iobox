using IOBox.Persistence;
using IOBox.Queues;
using IOBox.TaskExecution;
using IOBox.Workers.Archive;
using IOBox.Workers.Delete;
using IOBox.Workers.ExpireFailed;
using IOBox.Workers.ExpireNew;
using IOBox.Workers.Poll;
using IOBox.Workers.Process;
using IOBox.Workers.Retry;
using IOBox.Workers.Unlock;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IOBox;

/// <summary>
/// Provides extension methods for registering ioBox services and configurations 
/// related to inbox/outbox message processing.
/// </summary>
public static class Services
{
    /// <summary>
    /// Registers all services and background workers required for ioBox inbox/outbox
    /// processing, including polling, processing, retrying, expiring, unlocking, archiving
    /// and deleting messages.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="services"/> or <paramref name="configuration"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if no inboxes or outboxes are defined, or if names are missing or duplicated.
    /// </exception>
    public static IServiceCollection AddIOBox(
        this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));

        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        ValidateNames(configuration);

        var sections = configuration.GetAllInboxesAndOutboxes();

        foreach (var section in sections)
        {
            var ioName = section.GetValue<string>("Name")!;

            var workersSection = section.GetSection(
                Configuration.WorkersSection);

            services
                .AddPollWorker(workersSection, ioName)
                .AddRetryWorker(workersSection, ioName)
                .AddProcessWorker(workersSection, ioName)
                .AddUnlockWorker(workersSection, ioName)
                .AddExpireNewWorker(workersSection, ioName)
                .AddExpireFailedWorker(workersSection, ioName)
                .AddArchiveWorker(workersSection, ioName)
                .AddDeleteWorker(workersSection, ioName)
                .AddKeyedSingleton<IMessageQueue, InMemoryMessageQueue>(ioName)
                .AddDbOptions(section, ioName);
        }

        return services
            .AddHostedService<MessageBackgroundService>()
            .AddSingleton<IMessageQueueFactory, MessageQueueFactory>()
            .AddSingleton<ITaskExecutionWrapper, TaskExecutionWrapper>();
    }

    private static void ValidateNames(IConfiguration configuration)
    {
        var names = configuration.GetAllInboxesAndOutboxes()
            .Select(s => s.GetValue<string>("Name"));

        if (!names.Any())
        {
            throw new ArgumentException(
                "Configuration must define at least one Inbox or Outbox under " +
                $"'{Configuration.InboxesSection}' or " +
                $"'{Configuration.OutboxesSection}'.");
        }

        if (names.Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException(
                "Each Inbox or Outbox configuration must have a non-empty 'Name' value.");
        }

        var hasDuplicateNames = names
            .GroupBy(n => n)
            .Any(g => g.Count() > 1);

        if (hasDuplicateNames)
        {
            throw new ArgumentException(
                "Duplicate Inbox or Outbox names detected. " +
                "Each configuration must have a unique 'Name' value.");
        }
    }
}
