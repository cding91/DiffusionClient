using DiffusionClient.Queue;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace TestDiffusionClient;

/// <summary>
/// Test fixture for <see cref="QueueClient"/>
/// </summary>
public class QueueClientTestFixture : IDisposable
{
    /// <summary>
    /// Service provider for DI
    /// </summary>
    internal IServiceProvider ServiceProvider { get; }
    
    /// <summary>
    /// HTTP message handler mock
    /// </summary>
    /// <remarks>
    /// Add implementations to this mock to test HTTP requests in unit tests
    /// </remarks>
    internal Mock<HttpMessageHandler> HttpMessageHandlerMock { get; }

    /// <summary>
    /// Whether the object has been disposed
    /// </summary>
    private bool _disposed = false;

    /// <summary>
    /// Constructor to set up DI
    /// </summary>
    public QueueClientTestFixture()
    {
        HttpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddHttpClient<QueueClient>("ForQueueClient")
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri("https://queue.fal.run/");
            })
            .ConfigurePrimaryHttpMessageHandler(() => HttpMessageHandlerMock.Object);
        
        // serviceCollection.AddTransient<QueueClient>();

        ServiceProvider = serviceCollection.BuildServiceProvider();
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            if (ServiceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        _disposed = true;
    }
}