using DiffusionClient.Queue;

namespace DiffusionClient.Common;

public interface ISubscribable
{
    /// <summary>
    /// Subscription mode
    /// </summary>
    SubscribeMode Mode { get; }

    /// <summary>
    /// Cancellation token to abort the subscription
    /// </summary>
    public CancellationToken? AbortSignal { get; init; }
    
    /// <summary>
    /// Whether to include logs for the request in the response
    /// </summary>
    bool Logs { get; }
    
    /// <summary>
    /// URL to send a webhook notification to when the request is completed.
    /// </summary>
    public string? WebHookUrl { get; }
}