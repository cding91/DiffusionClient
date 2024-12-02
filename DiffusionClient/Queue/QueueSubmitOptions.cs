using DiffusionClient.Common;

namespace DiffusionClient.Queue;

/// <summary>
/// Submit options
/// </summary>
public class QueueSubmitOptions: IEnqueueable
{
    /// <summary>
    /// Input for the request
    /// </summary>
    public Input? Input { get; init; }

    /// <summary>
    /// HTTP method for the request
    /// </summary>
    public HttpMethodType Method { get; init; } = HttpMethodType.Post;
    
    /// <summary>
    /// Abort signal for the request
    /// </summary>
    public CancellationToken? AbortSignal { get; init; }
    
    /// <summary>
    /// The webhook URL to send the response to when the request is completed.
    /// </summary>
    public string? WebHookUrl { get; init; }

    /// <summary>
    /// Priority of the request. Default is <see cref="QueuePriority.Normal"/>.
    /// </summary>
    public QueuePriority? Priority { get; init; } = QueuePriority.Normal;
}