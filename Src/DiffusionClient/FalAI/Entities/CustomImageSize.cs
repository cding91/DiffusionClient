namespace DiffusionClient.FalAI.Entities;

/// <summary>
/// Custom image size
/// </summary>
public record CustomImageSize
{
    /// <summary>
    /// The width of the generated image. Default value: `512`
    /// </summary>
    public int? Width { get; init; } = 512;

    /// <summary>
    /// The height of the generated image. Default value: `512`
    /// </summary>
    public int? Height { get; init; } = 512;
}