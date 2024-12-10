using System.Net;
using System.Text.Json;
using DiffusionClient;
using DiffusionClient.Queue;
using DiffusionService;
using RichardSzalay.MockHttp;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<DiffusionWorker>();

var apiKey = builder.Configuration["FalAi:ApiKey"];

var dummyEnqueueResponse = new QueueResponse
{
    RequestId = "dummy_request_id",
    Status = QueueStatus.IN_QUEUE,
};
var dummyInQueueResponse = new QueueResponse
{
    RequestId = "dummy_request_id",
    Status = QueueStatus.IN_PROGRESS
};
var dummyCompleteResponse = new QueueResponse
{
    RequestId = "dummy_request_id",
    Status = QueueStatus.COMPLETED
};

var startTIme = DateTimeOffset.Now;
var mockHandler = new MockHttpMessageHandler();
mockHandler.When(HttpMethod.Post, "https://queue.fal.run/*/requests")
    .Respond(HttpStatusCode.OK, "application/json", JsonSerializer.Serialize(dummyEnqueueResponse));
mockHandler.When(HttpMethod.Get, "https://queue.fal.run/*")
    .Respond(() =>
    {
        if (DateTimeOffset.Now - startTIme < TimeSpan.FromSeconds(5))
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(dummyInQueueResponse))
            });
        }
        else
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(dummyCompleteResponse))
            });
        }
    });

builder.Services.AddHttpClient("DummyClient", c =>
    {
        c.BaseAddress = new Uri("https://queue.fal.run/");
        c.DefaultRequestHeaders.Add("Accept", "application/json");
        c.DefaultRequestHeaders.Add("Authorization", $"Key {apiKey}");
    })
    .ConfigurePrimaryHttpMessageHandler(() => mockHandler);

builder.Services.AddHttpClient("DevClient", c =>
{
    c.BaseAddress = new Uri("https://queue.fal.run/");
    c.DefaultRequestHeaders.Add("Accept", "application/json");
    c.DefaultRequestHeaders.Add("Authorization", $"Key {apiKey}");
});

builder.Services.AddTransient<Client>(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    // var httpClient = httpClientFactory.CreateClient("DummyClient");
    var httpClient = httpClientFactory.CreateClient("DevClient");
    return new Client(httpClient);
});

var app = builder.Build();
app.Run();