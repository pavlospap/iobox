using IOBox.Workers.ArchiveExpired.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IOBox.Workers.ArchiveExpired;

internal static class Services
{
    public static IServiceCollection AddArchiveExpiredWorker(
        this IServiceCollection services,
        IConfigurationSection ioSection,
        string ioName)
    {
        return services.AddWorker<
            IArchiveExpiredWorker,
            ArchiveExpiredWorker,
            ArchiveExpiredOptions,
            ArchiveExpiredOptionsValidator>(
            ioSection, ioName, ArchiveExpiredOptions.Section);
    }
}
