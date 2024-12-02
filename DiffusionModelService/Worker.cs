using DiffusionClient;
using DiffusionClient.Queue;

namespace DiffusionModelService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly HttpClient _httpClient;

    public Worker(IHttpClientFactory httpClientFactory, ILogger<Worker> logger)
    {
        _httpClient = httpClientFactory.CreateClient("DiffusionHttpClient");
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var client = new Client(_httpClient);
        
        var option = new QueueSubscribeOptions(input: new Dictionary<string, object>
        {
            { "prompt", "A cat" }
        });
        
        var response = await client.Subscribe<object>("fal-ai/fast-sdxl", option);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}