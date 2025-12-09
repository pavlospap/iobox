using System.Data;

namespace IOBox.Persistence;

/// <summary>
/// Represents a contract for storing and managing messages in a database. All 
/// the methods are intended to be invoked by library consumers.
/// </summary>
public interface IDbStore
{
    /// <summary>
    /// Adds a new message to the db store. This method is intended to be invoked by 
    /// library consumers.
    /// </summary>
    /// <param name="ioName">The inbox/outbox name to get the related configuration.</param>
    /// <param name="messageId">The unique identifier of the message used for deduplication.</param>
    /// <param name="payload">The serialized message payload.</param>
    /// <param name="contextInfo">Optional context information related to the message.</param>
    /// <param name="connection">
    /// An optional database connection to use for the operation. The caller is 
    /// responsible for managing its lifecycle.
    /// </param>
    /// <param name="transaction">
    /// An optional database transaction to use for the operation. If provided, it 
    /// must be associated with the specified <paramref name="connection"/>. The 
    /// caller is responsible for managing its lifecycle.
    /// </param> 
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddNewMessageAsync(
        string ioName,
        string messageId,
        string payload,
        string? contextInfo = null,
        IDbConnection? connection = null,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks the specified message as successfully processed. This method is 
    /// intended to be invoked by library consumers.
    /// </summary>
    /// <param name="ioName">The inbox/outbox name to get the related configuration.</param>
    /// <param name="id">The internal identifier of the message in the db store.</param>
    /// <param name="connection">
    /// An optional database connection to use for the operation. The caller is 
    /// responsible for managing its lifecycle.
    /// </param>
    /// <param name="transaction">
    /// An optional database transaction to use for the operation. If provided, it 
    /// must be associated with the specified <paramref name="connection"/>. The 
    /// caller is responsible for managing its lifecycle.
    /// </param> 
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task MarkMessageAsProcessedAsync(
        string ioName,
        int id,
        IDbConnection? connection = null,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks the specified message as failed to process. This method is intended 
    /// to be invoked by library consumers.
    /// </summary>
    /// <param name="ioName">The inbox/outbox name to get the related configuration.</param>
    /// <param name="id">The internal identifier of the message in the db store.</param>
    /// <param name="error">An optional error message describing the failure.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task MarkMessageAsFailedAsync(
        string ioName,
        int id,
        string? error = null,
        CancellationToken cancellationToken = default);
}
