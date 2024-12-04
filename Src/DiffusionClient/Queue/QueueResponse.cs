using System.Diagnostics.CodeAnalysis;

namespace DiffusionClient.Queue;

/// <summary>
/// Response for the queue
/// </summary>
public record QueueResponse
{
    /// <summary>
    /// Request ID
    /// </summary>
    public required string RequestId { get; init; }
    
    /// <summary>
    /// <see cref="QueueStatus"/>
    /// </summary>
    public required QueueStatus Status { get; init; }
    
    /// <summary>
    /// URL to the response
    /// </summary>
    /// <remarks>
    /// Available only when the request is in progress of being processed.
    /// </remarks>
    public string? ResponseUrl { get; init; }
    
    /// <summary>
    /// Position in the queue
    /// </summary>
    /// <remarks>
    /// Available only when the request is in the queue not started yet.
    /// </remarks>
    public int? QueuePosition { get; init; }
    
    /// <summary>
    /// Parameterless constructor
    /// </summary>
    public QueueResponse() {}
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="requestId">ID of the request</param>
    /// <param name="status">Status of the request represented by <see cref="QueueStatus"/></param>
    [SetsRequiredMembers]
    public QueueResponse(string requestId, QueueStatus status)
    {
        RequestId = requestId;
        Status = status;
    }
}