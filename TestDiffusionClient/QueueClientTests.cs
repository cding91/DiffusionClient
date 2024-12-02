using System.Net;
using System.Text;
using System.Text.Json;
using DiffusionClient;
using DiffusionClient.Queue;
using DiffusionClient.Response;
using Moq;
using Moq.Protected;
using Xunit;
using Assert = Xunit.Assert;

namespace TestDiffusionClient;

public class QueueClientTests
{
    [Fact]
    public async Task Status_ShouldReturnQueueStatus()
    {
        var expectedResponse = new QueueResponse("requestId", QueueStatus.Completed);
        var expectedJsonResponse = new StringContent(JsonSerializer.Serialize(expectedResponse), Encoding.UTF8, "application/json");
        
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        handlerMock
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
        
        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://queue.fal.run/"),
        };
        
        var queueService = new QueueClient(httpClient);
        
        var status = await queueService.Status("endpointId", "requestId");
        
        Assert.Equal(QueueStatus.Completed, status);
    }

    [Fact]
    public async Task Poll_ShouldCallOnUpdateAndOnComplete()
    {
        var timeToComplete = 2000;
        var waitTime = 0.0;
        var pollInterval = 1000;
        
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(() =>
            {
                waitTime += pollInterval;
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

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://queue.fal.run/"),
        };
        var queueService = new QueueClient(httpClient);
        
        var onCompleteCalled = false;

        await queueService.Poll("endpointId", "requestId", null, () => onCompleteCalled = true, pollInterval,
            new CancellationToken());
        
        Assert.True(onCompleteCalled);
    }
}