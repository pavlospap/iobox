using IOBox.Workers.DeleteProcessed.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IOBox.Workers.DeleteProcessed;

internal static class Services
{
    public static IServiceCollection AddDeleteProcessedWorker(
        this IServiceCollection services,
        IConfigurationSection ioSection,
        string ioName)
    {
        return services.AddWorker<
            IDeleteProcessedWorker,
            DeleteProcessedWorker,
            DeleteProcessedOptions,
            DeleteProcessedOptionsValidator>(
            ioSection, ioName, DeleteProcessedOptions.Section);
    }
}
