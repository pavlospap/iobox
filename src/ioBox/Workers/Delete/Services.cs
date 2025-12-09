using IOBox.Workers.Delete.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IOBox.Workers.Delete;

internal static class Services
{
    public static IServiceCollection AddDeleteWorker(
        this IServiceCollection services,
        IConfigurationSection ioSection,
        string ioName)
    {
        return services.AddWorker<
            IDeleteWorker,
            DeleteWorker,
            DeleteOptions,
            DeleteOptionsValidator>(
            ioSection, ioName, DeleteOptions.Section);
    }
}
