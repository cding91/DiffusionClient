namespace DiffusionClient.Storage;

/// <summary>
/// Response from the initial upload request to the storage service.
/// </summary>
public record InitialUploadResponse
{
    /// <summary>
    /// URL to the file.
    /// </summary>
    public string FileUrl { get; init; } = string.Empty;

    /// <summary>
    /// URL to upload the file.
    /// </summary>
    public string UploadUrl { get; init; } = string.Empty;
}