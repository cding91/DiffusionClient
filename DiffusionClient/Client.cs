using System.Text;
using System.Text.Json;
using DiffusionClient.Common;
using DiffusionClient.Queue;
using DiffusionClient.Response;

namespace DiffusionClient;

public class Client
{
    private readonly HttpClient _httpClient;
    private readonly QueueClient _queueClient;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="httpClient">DI of HTTP client</param>
    public Client(HttpClient httpClient) {
        _httpClient = httpClient;
        _queueClient = new QueueClient(httpClient);
    }

    public async Task<T> Subscribe<T>(string endpointId, QueueSubmitOptions submitOptions, QueueSubscribeOptions subscribeOptions)
    {
        var queueResponse = await _queueClient.Submit(endpointId, submitOptions);
        
        if (queueResponse == null)
        {
            return default;
        }
        
        if (subscribeOptions.OnQueue != null)
        {
            subscribeOptions.OnQueue(queueResponse.RequestId);
        }
        
        Console.WriteLine(queueResponse.RequestId);

        return default;
    }
    
}

