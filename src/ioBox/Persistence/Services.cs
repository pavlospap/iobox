using IOBox.Persistence.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace IOBox.Persistence;

internal static class Services
{
    public static IServiceCollection AddDbOptions(
        this IServiceCollection services,
        IConfigurationSection ioSection,
        string ioName)
    {
        var dbSection = ioSection.GetSection(DbOptions.Section);

        services
            .AddOptions<DbOptions>(ioName)
            .Bind(dbSection)
            .ValidateOnStart();

        services.TryAddSingleton<IValidateOptions<DbOptions>, DbOptionsValidator>();

        return services;
    }
}
