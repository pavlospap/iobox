using IOBox.TaskExecution.Options;

using Microsoft.Extensions.Options;

namespace IOBox.Workers.ExpireNew.Options;

internal class ExpireNewOptionsValidator : TaskExecutionOptionsValidator<ExpireNewOptions>
{
    public override ValidateOptionsResult Validate(string? name, ExpireNewOptions options)
    {
        var result = base.Validate(name, options);

        if (result.Failed)
        {
            return result;
        }

        if (options.Ttl < 0)
        {
            return ValidateOptionsResult.Fail(
                $"{name} - {nameof(ExpireNewOptions)}." +
                $"{nameof(options.Ttl)} must be greater than or equal to 0.");
        }

        return ValidateOptionsResult.Success;
    }
}
