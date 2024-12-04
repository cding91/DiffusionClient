using System.Net;
using System.Text;
using System.Text.Json;
using DiffusionClient.Common;
using DiffusionClient.Queue;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;
using Xunit;
using Assert = Xunit.Assert;

namespace TestDiffusionClient;

public class QueueClientTests : IClassFixture<QueueClientTestFixture>
{
    private readonly QueueClient _queueClient;
    
    private readonly Mock<HttpMessageHandler> _handlerMock;

    /// <summary>
    /// Constructor to set up DI
    /// </summary>
    /// <param name="fixture"><see cref="QueueClientTestFixture"/></param>
    public QueueClientTests(QueueClientTestFixture fixture)
    {
        _queueClient = fixture.ServiceProvider.GetRequiredService<QueueClient>();
        _handlerMock = fixture.HttpMessageHandlerMock;
    }
    
    [Fact]
    public async Task Status_ShouldReturnQueueStatus()
    {
        var expectedResponse = new QueueResponse
        {
            RequestId = "requestId",
            Status = QueueStatus.Completed,
        };
        var expectedJsonResponse =
            new StringContent(JsonSerializer.Serialize(expectedResponse), Encoding.UTF8, "application/json");
        
        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>()
            )
            .Returns(async () =>
            {
                await Task.Delay(2000);
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = expectedJsonResponse,
                };
            });

        var actualResponse = await _queueClient.Status("endpointId", "requestId");

        Assert.Equal(actualResponse, expectedResponse);
    }

    [Fact]
    public async Task Poll_ShouldCallOnUpdateAndOnComplete()
    {
        var onCompleteCalled = false;

        var options = new QueueSubscribeOptions
        {
            RequestId = "requestId",
            OnQueueUpdate = (status) =>
            {
                if (status == QueueStatus.Completed)
                {
                    onCompleteCalled = true;
                }
            },
            PollingInterval = 1000,
        };

        var timeToComplete = 10000;
        var waitTime = 0.0;
        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(() =>
            {
                waitTime += options.PollingInterval;
                return (waitTime < timeToComplete)
                    ? new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(
                            JsonSerializer.Serialize(new QueueResponse("requestId", QueueStatus.InQueue)),
                            Encoding.UTF8, "application/json"),
                    }
                    : new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(
                            JsonSerializer.Serialize(new QueueResponse("requestId", QueueStatus.Completed)),
                            Encoding.UTF8, "application/json"),
                    };
            });
        
        await _queueClient.SubscribeToStatus("endpointId", options);

        Assert.True(onCompleteCalled);
    }

    [Fact]
    public async Task Result_ShouldReturnResult()
    {

    }
}