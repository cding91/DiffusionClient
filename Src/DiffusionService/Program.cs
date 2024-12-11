using System.Net;
using System.Text.Json;
using DiffusionClient;
using DiffusionClient.Queue;
using DiffusionService;
using RichardSzalay.MockHttp;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<DiffusionWorker>();

var apiKey = builder.Configuration["FalAI:ApiKey"];

builder.Services.AddDummyClient();
// builder.Services.AddFalAiClient(apiKey);

var app = builder.Build();
app.Run();