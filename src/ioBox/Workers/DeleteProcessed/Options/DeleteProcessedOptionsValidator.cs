using IOBox.TaskExecution.Options;

using Microsoft.Extensions.Options;

namespace IOBox.Workers.DeleteProcessed.Options;

internal class DeleteProcessedOptionsValidator :
    TaskExecutionOptionsValidator<DeleteProcessedOptions>
{
    public override ValidateOptionsResult Validate(
        string? name, DeleteProcessedOptions options)
    {
        var result = base.Validate(name, options);

        if (result.Failed)
        {
            return result;
        }

        if (options.Ttl < 0)
        {
            return ValidateOptionsResult.Fail(
                $"{name} - {nameof(DeleteProcessedOptions)}." +
                $"{nameof(options.Ttl)} must be greater than or equal to 0.");
        }

        return ValidateOptionsResult.Success;
    }
}
