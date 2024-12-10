using DiffusionClient;
using DiffusionClient.FalAI;

namespace DiffusionService;

/// <summary>
/// Worker class for the Diffusion service.
/// </summary>
public class DiffusionWorker : BackgroundService
{
    private readonly Client _client;
    private readonly ILogger<DiffusionWorker> _logger;
    private readonly CancellationTokenSource _cancellationTokenSource;

    /// <summary>
    /// Constructor for <see cref="DiffusionWorker"/>.
    /// </summary>
    /// <param name="client">DI of <see cref="Client"/> for diffusion models</param>
    /// <param name="logger">DI of <see cref="ILogger"/> for the worker class</param>
    public DiffusionWorker(Client client, ILogger<DiffusionWorker> logger)
    {
        _client = client;
        _logger = logger;
        _cancellationTokenSource = new CancellationTokenSource();
        _logger.LogInformation("Diffusion Worker created");
    }

    /// <summary>
    /// Worker execution method
    /// </summary>
    /// <param name="stoppingToken"><see cref="CancellationToken"/> for graceful termination</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken, _cancellationTokenSource.Token);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            var finalResponse = await _client.Subscribe<FastSdxlInput, FastSdxlOutput>("fal-ai/fast-sdxl",
                new ClientOptions<FastSdxlInput>
                {
                    Input = new FastSdxlInput
                    {
                        Prompt = "An air condition",
                    },
                    OnEnqueue = (requestId) => _logger.LogInformation("Enqueued: {time}, {requestId}", DateTimeOffset.Now, requestId),
                    OnQueueUpdate = (status) => _logger.LogInformation("Queue update: {time}, {status}", DateTimeOffset.Now, status)
                });
            
            _logger.LogInformation("Final response: {time}, {response}", DateTimeOffset.Now, finalResponse);
            _cancellationTokenSource.Cancel();

            await Task.Delay(1000, stoppingToken);
        }
    }
}