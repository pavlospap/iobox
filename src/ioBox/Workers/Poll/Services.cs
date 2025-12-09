using IOBox.Workers.Poll.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IOBox.Workers.Poll;

internal static class Services
{
    public static IServiceCollection AddPollWorker(
        this IServiceCollection services,
        IConfigurationSection ioSection,
        string ioName)
    {
        return services.AddWorker<
            IPollWorker,
            PollWorker,
            PollOptions,
            PollOptionsValidator>(
            ioSection, ioName, PollOptions.Section);
    }
}
