using System.Threading.Channels;
using DiffusionClient.FalAI;
using DiffusionService;
using DiffusionService.Extensions;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<DiffusionWorker>();

var apiKey = builder.Configuration["FalAI:ApiKey"] ?? throw new Exception("ApiKey is null");

builder.Services.AddDummyClient();
// builder.Services.AddFalAiClient(apiKey);

var chIn = Channel.CreateUnbounded<FastSdxlInput>();
builder.Services.AddSingleton(chIn);
builder.Services.AddSingleton(chIn.Reader);
builder.Services.AddSingleton(chIn.Writer);

var chOut = Channel.CreateUnbounded<FastSdxlOutput>();
builder.Services.AddSingleton(chOut.Reader);
builder.Services.AddSingleton(chOut.Writer);

var app = builder.Build();

chIn.Writer.TryWrite(new FastSdxlInput { Prompt = "A goddess with a monkey pet"});

_ = Task.Run(async () =>
{
    await foreach (var output in chOut.Reader.ReadAllAsync())
    {
        Console.WriteLine($"{DateTimeOffset.Now}: Received new output, {output}");
    }
});

app.Run();