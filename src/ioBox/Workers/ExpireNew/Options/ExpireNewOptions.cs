using IOBox.TaskExecution.Options;

namespace IOBox.Workers.ExpireNew.Options;

/// <summary>
/// Represents configuration options related to the expiration of new messages.
/// </summary>
public class ExpireNewOptions : ITaskExecutionOptions
{
    internal const string Section = "ExpireNew";

    /// <summary>
    /// Indicates whether this worker is enabled and should perform its task.
    /// The worker is always scheduled to run at the specified <see cref="Delay"/>, 
    /// but it will only execute its logic if this setting is <c>true</c>.
    /// Default value is <c>true</c>.
    /// </summary>
    public bool Enabled { get; set; } = true;

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
    /// The time-to-live (TTL), in milliseconds, for new messages. After this 
    /// period, new messages will be marked as expired.
    /// Must be greater than or equal to <c>0</c>.
    /// If set to <c>0</c>, no automatic expiration of new messages will occur.
    /// Default value is <c>3,600,000 ms (1 hour)</c>.
    /// </summary>
    public int Ttl { get; set; } = 3_600_000;
}
