using Microsoft.Extensions.Configuration;

namespace IOBox;

internal static class Configuration
{
    public const string InboxesSection = "IOBox:Inboxes";

    public const string OutboxesSection = "IOBox:Outboxes";

    public const string WorkersSection = "Workers";

    public static IEnumerable<IConfigurationSection> GetAllInboxesAndOutboxes(
        this IConfiguration configuration)
    {
        var inboxes = configuration
            .GetSection(InboxesSection)
            .GetChildren();

        var outboxes = configuration
            .GetSection(OutboxesSection)
            .GetChildren();

        return inboxes.Concat(outboxes);
    }
}
