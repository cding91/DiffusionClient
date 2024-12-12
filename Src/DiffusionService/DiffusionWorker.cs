using System.Threading.Channels;
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
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly ChannelReader<FastSdxlInput> _inputChannelReader;
    private readonly ChannelWriter<FastSdxlOutput> _outputChannelWriter;

    /// <summary>
    /// Constructor for <see cref="DiffusionWorker"/>.
    /// </summary>
    /// <param name="client">DI of <see cref="Client"/> for diffusion models</param>
    /// <param name="logger">DI of <see cref="ILogger"/> for the worker class</param>
    /// <param name="appLifetime"><see cref="IHostApplicationLifetime"/> to control the application lifetime</param>
    /// <param name="inputChannelReader">Reader for input channel</param>
    /// <param name="outputChannelWriter">Writer for output channel</param>
    public DiffusionWorker(Client client, ILogger<DiffusionWorker> logger, IHostApplicationLifetime appLifetime, ChannelReader<FastSdxlInput> inputChannelReader, ChannelWriter<FastSdxlOutput> outputChannelWriter)
    {
        _client = client;
        _logger = logger;
        _appLifetime = appLifetime;
        _inputChannelReader = inputChannelReader;
        _outputChannelWriter = outputChannelWriter;
        _logger.LogInformation("Diffusion Worker created");
    }

    /// <summary>
    /// Worker execution method
    /// </summary>
    /// <param name="stoppingToken"><see cref="CancellationToken"/> for graceful termination</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var item in _inputChannelReader.ReadAllAsync(stoppingToken))
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            var output = await _client.Subscribe<FastSdxlInput, FastSdxlOutput>("fal-ai/fast-sdxl",
                new ClientOptions<FastSdxlInput>
                {
                    Input = item,
                    OnEnqueue = (requestId) => _logger.LogInformation("Enqueued: {time}, {requestId}", DateTimeOffset.Now, requestId),
                    OnQueueUpdate = (status) => _logger.LogInformation("Queue update: {time}, {status}", DateTimeOffset.Now, status)
                });
            
            await _outputChannelWriter.WriteAsync(output.Data, stoppingToken);
            _logger.LogInformation("Result: {time}, {response}", DateTimeOffset.Now, output);
        }

        _logger.LogInformation("Worker stopped at: {time}", DateTimeOffset.Now);
    }
}