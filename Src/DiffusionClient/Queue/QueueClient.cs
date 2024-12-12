using System.Text;
using DiffusionClient.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DiffusionClient.Queue;

/// <summary>
/// Queue client for handling requests regards to the job queue
/// </summary>
public class QueueClient : IQueueClient
{
    /// <summary>
    /// HTTP client for making requests
    /// </summary>
    private readonly HttpClient _httpClient;
    
    /// <summary>
    /// JSON serializer settings
    /// </summary>
    private JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
    {
        NullValueHandling = NullValueHandling.Ignore,
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy
            {
                OverrideSpecifiedNames = false,
                ProcessDictionaryKeys = true,
            }
        },
    };

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="httpClient">HTTP client to making requests</param>
    public QueueClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Submit a request to the queue
    /// </summary>
    /// <param name="endpointId">Endpoint ID</param>
    /// <param name="options"><see cref="QueueSubmitOptions{TInput}"/></param>
    /// <returns><see cref="QueueResponse"/> returned by the request</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the HTTP method type is invalid.</exception>
    /// <exception cref="System.Text.Json.JsonException">Thrown when the response cannot be deserialized.</exception>
    public async Task<QueueResponse> Submit<TInput>(string endpointId, QueueSubmitOptions<TInput> options)
    {
        // Serialize the content
        var jsonRequest = JsonConvert.SerializeObject(options.Input, _jsonSerializerSettings);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        // var content = new StringContent(JsonSerializer.Serialize(options.Input), Encoding.UTF8, "application/json");

        // TODO: Verify if the official API has this. Does not seems to be needed.
        // Send the request
        // using HttpResponseMessage response = options.Method switch
        // {
        //     HttpMethodType.Post => await _httpClient.PostAsync($"{endpointId}/", content,
        //         options.AbortSignal ?? CancellationToken.None),
        //     HttpMethodType.Get => await _httpClient.GetAsync($"{endpointId}/",
        //         options.AbortSignal ?? CancellationToken.None),
        //     HttpMethodType.Put => await _httpClient.PutAsync($"{endpointId}/", content,
        //         options.AbortSignal ?? CancellationToken.None),
        //     HttpMethodType.Delete => await _httpClient.DeleteAsync($"{endpointId}/",
        //         options.AbortSignal ?? CancellationToken.None),
        //     _ => throw new ArgumentOutOfRangeException(nameof(options), options.Method, "Invalid HTTP method type")
        // };
        using HttpResponseMessage response = await _httpClient.PostAsync($"{endpointId}/", content);
        response.EnsureSuccessStatusCode();

        // Deserialize the response
        var jsonResponse = await response.Content.ReadAsStringAsync();
        var queueResponse = JsonConvert.DeserializeObject<QueueResponse>(jsonResponse, _jsonSerializerSettings);        
        // var queueResponse = JsonSerializer.Deserialize<QueueResponse>(json);

        if (queueResponse == null)
        {
            throw new JsonException("Failed to deserialize the response");
        }

        return queueResponse;
    }


    /// <summary>
    /// Check the status of the request
    /// </summary>
    /// <param name="endpointId">End point ID</param>
    /// <param name="requestId">Request ID</param>
    /// <returns><see cref="QueueResponse"/></returns>
    /// <exception cref="JsonException">Thrown when the response cannot be deserialized.</exception>
    /// <remarks>
    /// Right now, this method follows Fal AI's API. It may change in the future.
    /// </remarks>
    public async Task<QueueResponse> Status(string endpointId, string requestId)
    {
        using var message = await _httpClient.GetAsync($"/{endpointId}/requests/{requestId}/status/");
        message.EnsureSuccessStatusCode();

        var json = await message.Content.ReadAsStringAsync();
        var response = JsonConvert.DeserializeObject<QueueResponse>(json, _jsonSerializerSettings);
        // var response = JsonSerializer.Deserialize<QueueResponse>(json);

        if (response == null)
        {
            throw new JsonException("Failed to deserialize the response");
        }

        return response;
    }

    /// <summary>
    /// Subscribe to the status of the request
    /// </summary>
    /// <param name="endpointId">Endpoint ID</param>
    /// <param name="options">Options for status subscription</param>
    /// <returns><see cref="QueueResponse"/> with status set as completed</returns>
    /// <exception cref="NotImplementedException">Thrown when streaming mode or logs are requested.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the request is cancelled due to timeout or external signal.</exception>
    public async Task<QueueResponse> SubscribeToStatus(string endpointId, QueueSubscribeOptions options)
    {
        if (options.Mode == SubscribeMode.Streaming)
        {
            throw new NotImplementedException("Streaming mode is not implemented yet");
        }

        if (options.Logs)
        {
            throw new NotImplementedException("Logs is not implemented yet");
        }

        // Terminate the polling if (1) the external token is cancelled or (2) the timeout is reached
        using var source = CancellationTokenSource.CreateLinkedTokenSource(
            options.AbortSignal ?? CancellationToken.None,
            new CancellationTokenSource(options.Timeout ?? Timeout.Infinite).Token);

        return await Poll(endpointId, options.RequestId, options.OnQueueUpdate, options.PollingInterval,
            source.Token);
    }

    /// <summary>
    /// Retrieve the result of the request from the queue
    /// </summary>
    /// <param name="endpointId">Endpoint ID</param>
    /// <param name="requestId">ID of the request</param>
    /// <returns>Result of the request</returns>
    /// <exception cref="JsonException">Thrown when the response cannot be deserialized.</exception>
    public async Task<Result<TOutput>> Result<TOutput>(string endpointId, string requestId)
    {
        using var message = await _httpClient.GetAsync($"{endpointId}/requests/{requestId}/");
        message.EnsureSuccessStatusCode();
        
        var json = await message.Content.ReadAsStringAsync();
        var output = JsonConvert.DeserializeObject<TOutput>(json, _jsonSerializerSettings);
        // var response = JsonSerializer.Deserialize<Result<TOutput>>(json);
        
        if (output == null)
        {
            throw new JsonException("Failed to deserialize the response");
        }
        
        var result = new Result<TOutput>
        {
            RequestId = requestId,
            Data = output
        };
        
        return result;
    }

    /// <summary>
    /// Poll the status of the request
    /// </summary>
    /// <param name="endpointId">Endpoint ID</param>
    /// <param name="requestId">Request ID</param>
    /// <param name="onUpdate">Callback when there is update</param>
    /// <param name="pollInterval">Milliseconds to wait before next poll</param>
    /// <param name="cancellationToken">External cancellation token</param>
    /// <exception cref="OperationCanceledException">Thrown when the request is cancelled due to timeout or external signal.</exception>
    internal async Task<QueueResponse> Poll(string endpointId, string requestId, Action<QueueStatus>? onUpdate,
        int pollInterval,
        CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var statusResponse = await Status(endpointId, requestId);

            onUpdate?.Invoke(statusResponse.Status);

            if (statusResponse.Status == QueueStatus.COMPLETED)
            {
                return statusResponse;
            }

            await Task.Delay(pollInterval, cancellationToken);
        }

        throw new OperationCanceledException("The request is cancelled due to timeout or external signal");
    }
}