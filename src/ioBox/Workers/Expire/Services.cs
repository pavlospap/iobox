using IOBox.Workers.Expire.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IOBox.Workers.Expire;

internal static class Services
{
    public static IServiceCollection AddExpireWorker(
        this IServiceCollection services,
        IConfigurationSection ioSection,
        string ioName)
    {
        return services.AddWorker<
            IExpireWorker,
            ExpireWorker,
            ExpireOptions,
            ExpireOptionsValidator>(
            ioSection, ioName, ExpireOptions.Section);
    }
}
