using DiffusionClient;

namespace DiffusionService;

/// <summary>
/// Extension methods for configuring the services to use a diffusion client.
/// </summary>
public static class DiffusionClientExtensions
{
    public static void AddFalAiClient(this IServiceCollection services, string apiKey)
    {
       services.AddHttpClient("DevClient", c =>
        {
            c.BaseAddress = new Uri("https://queue.fal.run/");
            c.DefaultRequestHeaders.Add("Accept", "application/json");
            c.DefaultRequestHeaders.Add("Authorization", $"Key {apiKey}");
        });
        services.AddTransient<Client>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient("DevClient");
            return new Client(httpClient);
        });
    }
}