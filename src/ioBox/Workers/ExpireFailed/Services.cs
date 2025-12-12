using IOBox.Workers.ExpireFailed.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IOBox.Workers.ExpireFailed;

internal static class Services
{
    public static IServiceCollection AddExpireFailedWorker(
        this IServiceCollection services,
        IConfigurationSection ioSection,
        string ioName)
    {
        return services.AddWorker<
            IExpireFailedWorker,
            ExpireFailedWorker,
            ExpireFailedOptions,
            ExpireFailedOptionsValidator>(
            ioSection, ioName, ExpireFailedOptions.Section);
    }
}
