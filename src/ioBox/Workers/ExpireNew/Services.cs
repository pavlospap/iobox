using IOBox.Workers.ExpireNew.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IOBox.Workers.ExpireNew;

internal static class Services
{
    public static IServiceCollection AddExpireNewWorker(
        this IServiceCollection services,
        IConfigurationSection ioSection,
        string ioName)
    {
        return services.AddWorker<
            IExpireNewWorker,
            ExpireNewWorker,
            ExpireNewOptions,
            ExpireNewOptionsValidator>(
            ioSection, ioName, ExpireNewOptions.Section);
    }
}
