using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace IOBox.Workers;

internal static class Services
{
    public static IServiceCollection AddWorker
        <TWorker, TWorkerImpl, TOptions, TOptionsValidator>(
        this IServiceCollection services,
        IConfigurationSection ioSection,
        string ioName,
        string workerSectionName)
        where TWorker : class, IWorker
        where TWorkerImpl : TWorker
        where TOptions : class
        where TOptionsValidator : class, IValidateOptions<TOptions>
    {
        var workerSection = ioSection.GetSection(workerSectionName);

        services
            .AddKeyedSingleton<TWorker>(ioName, (sp, key) =>
                ActivatorUtilities.CreateInstance<TWorkerImpl>(sp, key))
            .AddSingleton<IWorker>(sp => sp.GetKeyedService<TWorker>(ioName)!)
            .AddOptions<TOptions>(ioName)
            .Bind(workerSection)
            .ValidateOnStart();

        services.TryAddSingleton<IValidateOptions<TOptions>, TOptionsValidator>();

        return services;
    }
}
