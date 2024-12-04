using System.Net.Http.Headers;
using DiffusionClient.Queue;
using DiffusionModelService;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();

builder.Services.AddHttpClient("DiffusionHttpClient", client =>
{
    client.BaseAddress = new Uri("https://queue.fal.run/"); // Set the base address (must end with trailing slash)
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Key",
        "16d92018-6826-47f8-9e62-dc2519c0b64a:794d77160258fce006fa8bed9d7cfdc2"); // Set the API key
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

var host = builder.Build();
host.Run();

