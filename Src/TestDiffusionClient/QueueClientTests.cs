using System.Net;
using System.Text;
using DiffusionClient.Common;
using DiffusionClient.Queue;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xunit;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TestDiffusionClient;

public class QueueClientTests : IClassFixture<QueueClientTestFixture>
{
    private readonly QueueClient _queueClient;
    private readonly Mock<HttpMessageHandler> _handlerMock;

    private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
    {
        NullValueHandling = NullValueHandling.Ignore,
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy
            {
                OverrideSpecifiedNames = false,
                ProcessDictionaryKeys = true,
            }
        }
    };

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
            new StringContent(JsonConvert.SerializeObject(expectedResponse, _jsonSerializerSettings), Encoding.UTF8,
                "application/json");

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
                            JsonConvert.SerializeObject(new QueueResponse("requestId", QueueStatus.IN_QUEUE), _jsonSerializerSettings),
                            Encoding.UTF8, "application/json"),
                    }
                    : new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(
                            JsonConvert.SerializeObject(new QueueResponse("requestId", QueueStatus.COMPLETED), _jsonSerializerSettings),
                            Encoding.UTF8, "application/json"),
                    };
            });

        await _queueClient.SubscribeToStatus("endpointId", options);

        onCompleteCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Result_ShouldReturnResult()
    {
        var expectedResult = new Result<Dictionary<string, object>>
        {
            RequestId = "requestId",
            Data = new Dictionary<string, object>()
            {
                { "url", "https://example.com/requestId/result" },
            }
        };
        var expectedJsonResponse =
            new StringContent(JsonConvert.SerializeObject(expectedResult, _jsonSerializerSettings), Encoding.UTF8,
                "application/json");

        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(expectedResult.Data, _jsonSerializerSettings), Encoding.UTF8,
                    "application/json")
            });

        var actualResult = await _queueClient.Result<Dictionary<string, object>>("endpointId", "requestId");

        actualResult.Should().BeEquivalentTo(expectedResult);
    }
}