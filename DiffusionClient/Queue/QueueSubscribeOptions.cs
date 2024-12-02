using DiffusionClient.Common;
using DiffusionClient.Response;

namespace DiffusionClient.Queue;

public class QueueSubscribeOptions : ISubscribable, IPollable, IQueueable
{
    /// <summary>
    /// The ID of the request
    /// </summary>
    public string? RequestId { get; init; }
    
    /// <summary>
    /// Callback function called when a request is enqueued
    /// </summary>
    public Action<string>? OnQueue { get; init; }
    
    /// <summary>
    /// Callback function called when a request queue status is updated
    /// </summary>
    public Action<QueueStatus>? OnQueueUpdate { get; init; }
    
    /// <summary>
    /// Subscription mode
    /// </summary>
    public SubscribeMode Mode { get; init; } = SubscribeMode.Polling;

    /// <summary>
    /// Cancellation token to abort the subscription
    /// </summary>
    public CancellationToken? AbortSignal { get; init; }

    /// <summary>
    /// Whether to include logs for the request in the response
    /// </summary>
    public bool Logs { get; init; } = false;

    /// <summary>
    /// URL to send a webhook notification to when the request is completed.
    /// </summary>
    public string? WebHookUrl { get; init; }

    /// <summary>
    /// Timeout in milliseconds. If the request is not completed within the timeout, the request will be cancelled.
    /// Null means no timeout.
    /// </summary>
    public int? Timeout { get; init; }
    
    /// <summary>
    /// The interval (in milliseconds) at which to poll for updates.
    /// </summary>
    public int PollingInterval { get; init; } = 500;
}