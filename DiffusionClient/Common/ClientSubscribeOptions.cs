using DiffusionClient.Queue;
using DiffusionClient.Response;

namespace DiffusionClient.Common;

/// <summary>
/// Client subscribe options
/// </summary>
public class ClientSubscribeOptions
{
    public required QueueSubscribeOptions SubscribeOptions { get; init; }
    public required QueueSubmitOptions SubmitOptions { get; init; }
}