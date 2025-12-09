using IOBox.Workers.Retry.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IOBox.Workers.Retry;

internal static class Services
{
    public static IServiceCollection AddRetryWorker(
        this IServiceCollection services,
        IConfigurationSection ioSection,
        string ioName)
    {
        return services.AddWorker<
            IRetryWorker,
            RetryWorker,
            RetryOptions,
            RetryOptionsValidator>(
            ioSection, ioName, RetryOptions.Section);
    }
}
