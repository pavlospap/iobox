using IOBox.Workers.ArchiveProcessed.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IOBox.Workers.ArchiveProcessed;

internal static class Services
{
    public static IServiceCollection AddArchiveProcessedWorker(
        this IServiceCollection services,
        IConfigurationSection ioSection,
        string ioName)
    {
        return services.AddWorker<
            IArchiveProcessedWorker,
            ArchiveProcessedWorker,
            ArchiveProcessedOptions,
            ArchiveProcessedOptionsValidator>(
            ioSection, ioName, ArchiveProcessedOptions.Section);
    }
}
