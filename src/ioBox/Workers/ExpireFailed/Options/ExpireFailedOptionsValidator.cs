using IOBox.TaskExecution.Options;

using Microsoft.Extensions.Options;

namespace IOBox.Workers.ExpireFailed.Options;

internal class ExpireFailedOptionsValidator : TaskExecutionOptionsValidator<ExpireFailedOptions>
{
    public override ValidateOptionsResult Validate(string? name, ExpireFailedOptions options)
    {
        var result = base.Validate(name, options);

        if (result.Failed)
        {
            return result;
        }

        if (options.Ttl < 0)
        {
            return ValidateOptionsResult.Fail(
                $"{name} - {nameof(ExpireFailedOptions)}." +
                $"{nameof(options.Ttl)} must be greater than or equal to 0.");
        }

        return ValidateOptionsResult.Success;
    }
}
