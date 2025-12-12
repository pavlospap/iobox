using IOBox.TaskExecution.Options;

using Microsoft.Extensions.Options;

namespace IOBox.Workers.ArchiveProcessed.Options;

internal class ArchiveProcessedOptionsValidator :
    TaskExecutionOptionsValidator<ArchiveProcessedOptions>
{
    public override ValidateOptionsResult Validate(
        string? name, ArchiveProcessedOptions options)
    {
        var result = base.Validate(name, options);

        if (result.Failed)
        {
            return result;
        }

        if (options.Ttl < 0)
        {
            return ValidateOptionsResult.Fail(
                $"{name} - {nameof(ArchiveProcessedOptions)}." +
                $"{nameof(options.Ttl)} must be greater than or equal to 0.");
        }

        return ValidateOptionsResult.Success;
    }
}
