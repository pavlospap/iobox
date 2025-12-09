using IOBox.Workers.Unlock.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IOBox.Workers.Unlock;

internal static class Services
{
    public static IServiceCollection AddUnlockWorker(
        this IServiceCollection services,
        IConfigurationSection ioSection,
        string ioName)
    {
        return services.AddWorker<
            IUnlockWorker,
            UnlockWorker,
            UnlockOptions,
            UnlockOptionsValidator>(
            ioSection, ioName, UnlockOptions.Section);
    }
}
