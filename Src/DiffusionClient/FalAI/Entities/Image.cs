namespace DiffusionClient.FalAI.Entities;

/// <summary>
/// Image entity in the FalAI API output
/// </summary>
public record Image
{
    /// <summary>
    /// URL of the generated image
    /// </summary>
    public required string Url { get; init; }
    
    /// <summary>
    /// Width of the generated image
    /// </summary>
    public int Width { get; init; }
    
    /// <summary>
    /// Height of the generated image
    /// </summary>
    public int Height { get; init; }
    
    /// <summary>
    /// Type of the image. Default value: "image/jpeg"
    /// </summary>
    public string? ContentType { get; init; }
};