using IOBox.TaskExecution.Options;

using Microsoft.Extensions.Options;

namespace IOBox.Workers.ArchiveExpired.Options;

internal class ArchiveExpiredOptionsValidator :
    TaskExecutionOptionsValidator<ArchiveExpiredOptions>
{
    public override ValidateOptionsResult Validate(
        string? name, ArchiveExpiredOptions options)
    {
        var result = base.Validate(name, options);

        if (result.Failed)
        {
            return result;
        }

        if (options.Ttl < 0)
        {
            return ValidateOptionsResult.Fail(
                $"{name} - {nameof(ArchiveExpiredOptions)}." +
                $"{nameof(options.Ttl)} must be greater than or equal to 0.");
        }

        return ValidateOptionsResult.Success;
    }
}
