namespace DiffusionClient.Storage;

/// <summary>
/// Initial upload request for the storage service.
/// </summary>
public record InitialUploadRequest
{
    /// <summary>
    /// File name. Default is the current date and time.bin.
    /// </summary>
    public string FileName { get; init; } = $"{DateTimeOffset.Now}.bin";
    
    /// <summary>
    /// Content type. Default is "application/octet-stream".
    /// </summary>
    public string ContentType { get; init; } = "application/octet-stream";
};