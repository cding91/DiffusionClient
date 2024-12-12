using System.Net;
using DiffusionClient;
using DiffusionClient.FalAI;
using DiffusionClient.FalAI.Entities;
using DiffusionClient.Queue;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RichardSzalay.MockHttp;

namespace DiffusionService.Extensions;

/// <summary>
/// Extension methods for configuring the services to use a dummy client for testing purposes.
/// </summary>
public static class DummyClientExtensions
{
    /// <summary>
    /// Configures the services to use a dummy client for testing purposes.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/> to add dummy client to</param>
    public static void AddDummyClient(this IServiceCollection services)
    {
        var jsonSerializerSettings = new JsonSerializerSettings
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
        
        const string appId = "fal-ai/fast-sdxl";
        const string requestId = "dummy_request_id";
        string? prompt = null;
        
        var dummyEnqueueResponse = new QueueResponse
        {
            RequestId = requestId,
            Status = QueueStatus.IN_QUEUE,
        };
        var dummyInQueueResponse = new QueueResponse
        {
            RequestId = requestId,
            Status = QueueStatus.IN_PROGRESS
        };
        var dummyCompleteResponse = new QueueResponse
        {
            RequestId = requestId,
            Status = QueueStatus.COMPLETED
        };
        var dummyOutput = new FastSdxlOutput
        {
            Images = new List<Image>
            {
                new Image
                {
                    Url = "https://dummy.url",
                    Width = 100,
                    Height = 100,
                    ContentType = "image/jpeg"
                }
            },
            Seed = 0,
            HasNsfwConcepts = [false],
            Prompt = prompt,
        };
        
        var startTIme = DateTimeOffset.Now;
        var mockHandler = new MockHttpMessageHandler();
        mockHandler.When(HttpMethod.Post, $"https://queue.fal.run/{appId}/")
            .Respond((request) =>
            {
                var content = request.Content.ReadAsStringAsync().Result;
                var input = JsonConvert.DeserializeObject<FastSdxlInput>(content, jsonSerializerSettings);
                prompt = input.Prompt;
                
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(dummyEnqueueResponse, jsonSerializerSettings))
                });
            });
        mockHandler.When(HttpMethod.Get, $"https://queue.fal.run/{appId}/requests/{requestId}/status/")
            .Respond(() =>
            {
                if (DateTimeOffset.Now - startTIme < TimeSpan.FromSeconds(5))
                {
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(dummyInQueueResponse, jsonSerializerSettings))
                    });
                }
                else
                {
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(dummyCompleteResponse, jsonSerializerSettings)),
                    });
                }
            });
        mockHandler.When(HttpMethod.Get, $"https://queue.fal.run/{appId}/requests/{requestId}/")
            .Respond(() =>
            {
                var updatedOutput = dummyOutput with
                {
                    Prompt = prompt
                };
                
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(updatedOutput, jsonSerializerSettings))
                });
            });
        
        services.AddHttpClient("DummyClient", c =>
            {
                c.BaseAddress = new Uri("https://queue.fal.run/");
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                c.DefaultRequestHeaders.Add("Authorization", $"Key dummy_api_key");
            })
            .ConfigurePrimaryHttpMessageHandler(() => mockHandler);
        
        services.AddTransient<Client>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient("DummyClient");
            return new Client(httpClient);
        });
    }
}