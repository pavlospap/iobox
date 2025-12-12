using IOBox.Workers.DeleteExpired.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IOBox.Workers.DeleteExpired;

internal static class Services
{
    public static IServiceCollection AddDeleteExpiredWorker(
        this IServiceCollection services,
        IConfigurationSection ioSection,
        string ioName)
    {
        return services.AddWorker<
            IDeleteExpiredWorker,
            DeleteExpiredWorker,
            DeleteExpiredOptions,
            DeleteExpiredOptionsValidator>(
            ioSection, ioName, DeleteExpiredOptions.Section);
    }
}
