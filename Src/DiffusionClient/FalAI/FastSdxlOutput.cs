using DiffusionClient.FalAI.Entities;

namespace DiffusionClient.FalAI;

public record FastSdxlOutput
{
    /// <summary>
    /// The generated image files info.
    /// </summary>
    public required IList<Image> Images { get; init; }
    
    // public required Timings Timings { get; init; }
    
    /// <summary>
    /// Seed of the generated Image.
    /// It will be the same value of the one passed in the input or the randomly generated that was used in case none was passed.
    /// </summary>
    public required long Seed { get; init; }
    
    /// <summary>
    /// Whether the generated images contain NSFW concepts.
    /// </summary>
    public required IList<bool> HasNsfwConcepts { get; init; }
    
    /// <summary>
    /// The prompt used for generating the image.
    /// </summary>
    public required string Prompt { get; init; }
}