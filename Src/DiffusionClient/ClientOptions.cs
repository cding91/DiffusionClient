using DiffusionClient.Common;
using DiffusionClient.Queue;

namespace DiffusionClient;

/// <summary>
/// Options for the client
/// </summary>
/// <typeparam name="TInput">Type of API input</typeparam>
public class ClientOptions<TInput> : IEnqueueable<TInput>, ISubscribable, IPollable, IQueueable
{
    /// <summary>
    /// Input for the request
    /// </summary>
    public TInput? Input { get; init; }

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

    /// <summary>
    /// The ID of the request
    /// </summary>
    /// <remarks>
    /// This is the ID of the request that is returned when the request is submitted.
    /// Initialized to an empty string but will be set when the request is submitted.
    /// </remarks>
    public string RequestId { get; set; } = string.Empty;

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
    /// Whether to include logs for the request in the response
    /// </summary>
    public bool Logs { get; init; } = false;

    /// <summary>
    /// Timeout in milliseconds. If the request is not completed within the timeout, the request will be cancelled.
    /// Null means no timeout.
    /// </summary>
    public int? Timeout { get; init; }

    /// <summary>
    /// The interval (in milliseconds) at which to poll for updates.
    /// </summary>
    public int PollingInterval { get; init; } = 500;

    /// <summary>
    /// Creates a new instance of <see cref="QueueSubmitOptions{TInput}"/> using the properties of this class.
    /// </summary>
    /// <returns>A new instance of <see cref="QueueSubmitOptions{TInput}"/></returns>
    public QueueSubmitOptions<TInput> ToQueueSubmitOptions()
    {
        return new QueueSubmitOptions<TInput>
        {
            Input = Input,
            Method = Method,
            AbortSignal = AbortSignal,
            WebHookUrl = WebHookUrl,
            Priority = Priority
        };
    }

    /// <summary>
    /// Creates a new instance of <see cref="QueueSubscribeOptions"/> using the properties of this class.
    /// </summary>
    /// <returns>A new instance of <see cref="QueueSubscribeOptions"/></returns>
    public QueueSubscribeOptions ToQueueSubscribeOptions()
    {
        return new QueueSubscribeOptions
        {
            RequestId = RequestId,
            OnQueue = OnQueue,
            OnQueueUpdate = OnQueueUpdate,
            Mode = Mode,
            AbortSignal = AbortSignal,
            Logs = Logs,
            WebHookUrl = WebHookUrl,
            Timeout = Timeout,
            PollingInterval = PollingInterval
        };
    }
}