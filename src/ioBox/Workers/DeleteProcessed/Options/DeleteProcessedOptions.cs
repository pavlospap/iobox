using IOBox.TaskExecution.Options;

namespace IOBox.Workers.DeleteProcessed.Options;

/// <summary>
/// Represents configuration options related to the deleting of processed messages.
/// </summary>
public class DeleteProcessedOptions : ITaskExecutionOptions
{
    internal const string Section = "DeleteProcessed";

    /// <summary>
    /// Indicates whether this worker is enabled and should perform its task.
    /// The worker is always scheduled to run at the specified <see cref="Delay"/>, 
    /// but it will only execute its logic if this setting is <c>true</c>.
    /// Default value is <c>false</c>.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// The delay, in milliseconds, between each execution cycle.
    /// Must be greater than <c>0</c>.
    /// Default value is <c>10,000 ms (10 sec)</c>.
    /// </summary>
    public int Delay { get; set; } = 10_000;

    /// <summary>
    /// The timeout, in milliseconds, for a single execution cycle.
    /// Must be greater than or equal to <c>0</c>.
    /// If set to <c>0</c>, no timeout will be applied.
    /// Default value is <c>10,000 ms (10 sec)</c>.
    /// </summary>
    public int Timeout { get; set; } = 10_000;

    /// <summary>
    /// The batch size, i.e., the number of messages to process in a single 
    /// execution cycle.
    /// Must be greater than <c>0</c>.
    /// Default value is <c>1000</c>.
    /// </summary>
    public int BatchSize { get; set; } = 1000;

    /// <summary>
    /// The time-to-live (TTL), in milliseconds, for processed messages. After 
    /// this period, processed messages will be deleted.
    /// Must be greater than or equal to <c>0</c>.
    /// If set to <c>0</c>, no automatic deletion of processed messages will occur.
    /// Default value is <c>1,800,000 ms (30 min)</c>.
    /// </summary>
    public int Ttl { get; set; } = 1_800_000;
}
