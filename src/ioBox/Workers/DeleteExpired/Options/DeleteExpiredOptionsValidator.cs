using IOBox.TaskExecution.Options;

using Microsoft.Extensions.Options;

namespace IOBox.Workers.DeleteExpired.Options;

internal class DeleteExpiredOptionsValidator :
    TaskExecutionOptionsValidator<DeleteExpiredOptions>
{
    public override ValidateOptionsResult Validate(
        string? name, DeleteExpiredOptions options)
    {
        var result = base.Validate(name, options);

        if (result.Failed)
        {
            return result;
        }

        if (options.Ttl < 0)
        {
            return ValidateOptionsResult.Fail(
                $"{name} - {nameof(DeleteExpiredOptions)}." +
                $"{nameof(options.Ttl)} must be greater than or equal to 0.");
        }

        return ValidateOptionsResult.Success;
    }
}
