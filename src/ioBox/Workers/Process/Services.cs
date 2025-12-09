using IOBox.Workers.Process.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IOBox.Workers.Process;

internal static class Services
{
    public static IServiceCollection AddProcessWorker(
        this IServiceCollection services,
        IConfigurationSection ioSection,
        string ioName)
    {
        return services.AddWorker<
            IProcessWorker,
            ProcessWorker,
            ProcessOptions,
            ProcessOptionsValidator>(
            ioSection, ioName, ProcessOptions.Section);
    }
}
