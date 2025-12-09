using IOBox.Workers.Archive.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IOBox.Workers.Archive;

internal static class Services
{
    public static IServiceCollection AddArchiveWorker(
        this IServiceCollection services,
        IConfigurationSection ioSection,
        string ioName)
    {
        return services.AddWorker<
            IArchiveWorker,
            ArchiveWorker,
            ArchiveOptions,
            ArchiveOptionsValidator>(
            ioSection, ioName, ArchiveOptions.Section);
    }
}
