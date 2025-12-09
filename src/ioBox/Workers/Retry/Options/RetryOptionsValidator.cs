using IOBox.TaskExecution.Options;

using Microsoft.Extensions.Options;

namespace IOBox.Workers.Retry.Options;

internal class RetryOptionsValidator : TaskExecutionOptionsValidator<RetryOptions>
{
    public override ValidateOptionsResult Validate(string? name, RetryOptions options)
    {
        var result = base.Validate(name, options);

        if (result.Failed)
        {
            return result;
        }

        if (options.Limit < 0)
        {
            return ValidateOptionsResult.Fail(
                $"{name} - {nameof(RetryOptions)}." +
                $"{nameof(options.Limit)} must be greater than or equal to 0.");
        }

        return ValidateOptionsResult.Success;
    }
}
