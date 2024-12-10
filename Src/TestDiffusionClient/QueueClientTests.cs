using System.Net;
using System.Text;
using System.Text.Json;
using DiffusionClient.Common;
using DiffusionClient.Queue;
using FluentAssertions;
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
            Status = QueueStatus.COMPLETED,
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

        actualResponse.Should().BeEquivalentTo(expectedResponse);
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
                if (status == QueueStatus.COMPLETED)
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
                            JsonSerializer.Serialize(new QueueResponse("requestId", QueueStatus.IN_QUEUE)),
                            Encoding.UTF8, "application/json"),
                    }
                    : new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(
                            JsonSerializer.Serialize(new QueueResponse("requestId", QueueStatus.COMPLETED)),
                            Encoding.UTF8, "application/json"),
                    };
            });
        
        await _queueClient.SubscribeToStatus("endpointId", options);

        onCompleteCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Result_ShouldReturnResult()
    {
        var expectedResult = new Result<Dictionary<string, string>>
        {
            RequestId = "requestId",
            Data = new Dictionary<string, string>()
            {
                {"url", "https://example.com/requestId/result"},
            }
        };
        var expectedJsonResponse =
            new StringContent(JsonSerializer.Serialize(expectedResult), Encoding.UTF8, "application/json");
        
        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = expectedJsonResponse,
            });
        
        var actualResult = await _queueClient.Result<Dictionary<string, string>>("endpointId", "requestId");

        actualResult.Should().BeEquivalentTo(expectedResult);
    }
}