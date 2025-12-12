namespace IOBox.Persistence;

/// <summary>
/// Represents a contract for managing messages in a database. All the methods
/// are intended to be invoked only by the library infrastructure — not by 
/// library consumers.
/// </summary>
public interface IDbStoreInternal
{
    /// <summary>
    /// Retrieves messages that are ready to be processed. This method is intended 
    /// to be invoked only by the library infrastructure — not by library consumers.
    /// </summary>
    /// <param name="ioName">The inbox/outbox name to get the related configuration.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A collection of messages ready for processing.</returns>
    Task<IEnumerable<Message>> GetMessagesToProcessAsync(
        string ioName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves messages that are eligible to be retried after a failure. This 
    /// method is intended to be invoked only by the library infrastructure — not 
    /// by library consumers.
    /// </summary>
    /// <param name="ioName">The inbox/outbox name to get the related configuration.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A collection of messages eligible for retry.</returns>
    Task<IEnumerable<Message>> GetMessagesToRetryAsync(
        string ioName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Unlocks messages that were previously locked for processing but were not 
    /// completed.This method is intended to be invoked only by the library 
    /// infrastructure — not by library consumers.
    /// </summary>
    /// <param name="ioName">The inbox/outbox name to get the related configuration.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UnlockMessagesAsync(
        string ioName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks new messages as expired based on the TTL specified on configuration. 
    /// This method is intended to be invoked only by the library infrastructure — not 
    /// by library consumers.
    /// </summary>
    /// <param name="ioName">The inbox/outbox name to get the related configuration.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task MarkNewMessagesAsExpiredAsync(
        string ioName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks failed messages as expired based on the TTL specified on configuration. 
    /// This method is intended to be invoked only by the library infrastructure — not 
    /// by library consumers.
    /// </summary>
    /// <param name="ioName">The inbox/outbox name to get the related configuration.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task MarkFailedMessagesAsExpiredAsync(
        string ioName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Archives processed messages based on the TTL specified on configuration. 
    /// This method is intended to be invoked only by the library infrastructure — not 
    /// by library consumers.
    /// </summary>
    /// <param name="ioName">The inbox/outbox name to get the related configuration.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ArchiveProcessedMessagesAsync(
        string ioName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Archives expired messages based on the TTL specified on configuration. 
    /// This method is intended to be invoked only by the library infrastructure — not 
    /// by library consumers.
    /// </summary>
    /// <param name="ioName">The inbox/outbox name to get the related configuration.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ArchiveExpiredMessagesAsync(
        string ioName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Permanently deletes processed messages based on the TTL specified on 
    /// configuration. This method is intended to be invoked only by the library 
    /// infrastructure — not by library consumers.
    /// </summary>
    /// <param name="ioName">The inbox/outbox name to get the related configuration.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteProcessedMessagesAsync(
        string ioName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Permanently deletes expired messages based on the TTL specified on 
    /// configuration. This method is intended to be invoked only by the library 
    /// infrastructure — not by library consumers.
    /// </summary>
    /// <param name="ioName">The inbox/outbox name to get the related configuration.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteExpiredMessagesAsync(
        string ioName,
        CancellationToken cancellationToken = default);
}
