using DiffusionClient.Queue;

namespace DiffusionClient.Common;

public interface IEnqueueable
{
    /// <summary>
    /// Input for the request
    /// </summary>
    public Input? Input { get; }

    /// <summary>
    /// HTTP method for the request
    /// </summary>
    public HttpMethodType Method { get; }

    /// <summary>
    /// Abort signal for the request
    /// </summary>
    public CancellationToken? AbortSignal { get; }

    /// <summary>
    /// The webhook URL to send the response to when the request is completed.
    /// </summary>
    public string? WebHookUrl { get; }

    /// <summary>
    /// Priority of the request. Default is <see cref="QueuePriority.Normal"/>.
    /// </summary>
    public QueuePriority? Priority { get; }
}