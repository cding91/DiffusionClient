namespace DiffusionClient.Common;

/// <summary>
/// Represents the result of an API request
/// </summary>
/// <typeparam name="T">Type of the data contained in the result</typeparam>
public class Result<T>
{
    /// <summary>
    /// Data contained in the result
    /// </summary>
    public required T Data { get; init; }
    
    /// <summary>
    /// Request ID of the result
    /// </summary>
    public required string RequestId { get; init; }
    
    /// <summary>
    /// Parameterless constructor
    /// </summary>
    public Result() {}
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="data"><see cref="Data"/></param>
    /// <param name="requestId"><see cref="RequestId"/></param>
    public Result(T data, string requestId)
    {
        Data = data;
        RequestId = requestId;
    }
}