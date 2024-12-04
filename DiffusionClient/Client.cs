using DiffusionClient.Common;
using DiffusionClient.Queue;

namespace DiffusionClient;

/// <summary>
/// Client for interacting with the diffusion model API
/// </summary>
public class Client
{
    private readonly QueueClient _queueClient;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="httpClient">DI of HTTP client</param>
    public Client(HttpClient httpClient)
    {
        _queueClient = new QueueClient(httpClient);
    }

    /// <summary>
    /// Submit a request to the queue and subscribe to the status until it is completed
    /// </summary>
    /// <param name="endpointId">Endpoint ID</param>
    /// <param name="options">Client subscribe options represented as <see cref="ClientOptions{TInput}"/></param>
    /// <returns>Result of the request</returns>
    public async Task<Result<TOutput>> Subscribe<TInput, TOutput>(string endpointId, ClientOptions<TInput> options)
    {
        var enqueueResponse = await _queueClient.Submit(endpointId, options.ToQueueSubmitOptions());

        options.RequestId = enqueueResponse.RequestId; // Set the request ID to the response ID
        options.OnQueue?.Invoke(enqueueResponse.RequestId);

        var completeResponse = await _queueClient.SubscribeToStatus(endpointId, options.ToQueueSubscribeOptions());

        var result = await _queueClient.Result<TOutput>(endpointId, completeResponse.RequestId);

        return result;
    }
}