using DiffusionClient.Queue;

namespace DiffusionClient.Common;

public interface IQueueable
{
    /// <summary>
    /// The ID of the request
    /// </summary>
    public string? RequestId { get; }
    
    /// <summary>
    /// Callback function called when a request is enqueued
    /// </summary>
    public Action<string>? OnEnqueue { get; }
    
    /// <summary>
    /// Callback function called when a request queue status is updated
    /// </summary>
    public Action<QueueStatus>? OnQueueUpdate { get; }
}