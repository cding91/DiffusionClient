namespace DiffusionClient.Common;

public interface IPollable
{
    /// <summary>
    /// Timeout in milliseconds. If the request is not completed within the timeout, the request will be cancelled.
    /// Null means no timeout.
    /// </summary>
    int? Timeout { get; }

    /// <summary>
    /// The interval (in milliseconds) at which to poll for updates.
    /// </summary>
    int PollingInterval { get; }
}