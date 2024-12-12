using DiffusionClient.Common;
using Newtonsoft.Json;

namespace DiffusionClient.Queue;

public interface IQueueClient
{
    /// <summary>
    /// Submit a request to the queue
    /// </summary>
    /// <param name="endpointId">Endpoint ID</param>
    /// <param name="options"><see cref="QueueSubmitOptions{TInput}"/></param>
    /// <returns><see cref="QueueResponse"/> returned by the request</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the HTTP method type is invalid.</exception>
    /// <exception cref="System.Text.Json.JsonException">Thrown when the response cannot be deserialized.</exception>
    Task<QueueResponse> Submit<TInput>(string endpointId, QueueSubmitOptions<TInput> options);

    /// <summary>
    /// Subscribe to the status of the request
    /// </summary>
    /// <param name="endpointId">Endpoint ID</param>
    /// <param name="options">Options for status subscription</param>
    /// <returns><see cref="QueueResponse"/> with status set as completed</returns>
    /// <exception cref="NotImplementedException">Thrown when streaming mode or logs are requested.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the request is cancelled due to timeout or external signal.</exception>
    Task<QueueResponse> SubscribeToStatus(string endpointId, QueueSubscribeOptions options);

    /// <summary>
    /// Retrieve the result of the request from the queue
    /// </summary>
    /// <param name="endpointId">Endpoint ID</param>
    /// <param name="requestId">ID of the request</param>
    /// <returns>Result of the request</returns>
    /// <exception cref="JsonException">Thrown when the response cannot be deserialized.</exception>
    Task<Result<TOutput>> Result<TOutput>(string endpointId, string requestId);
}