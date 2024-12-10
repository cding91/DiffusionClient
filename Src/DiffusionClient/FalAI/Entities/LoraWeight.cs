namespace DiffusionClient.FalAI.Entities;

public record LoraWeight
{
    /// <summary>
    /// URL or the path to the LoRA weights. Or HF model name.
    /// </summary>
    public required string Path { get; init; }
    
    /// <summary>
    /// The scale of the LoRA weight. This is used to scale the LoRA weight before merging it with the base model. Default value: 1
    /// </summary>
    public double? Scale { get; init; }
    
    /// <summary>
    /// If set to true, the embedding will be forced to be used.
    /// </summary>
    public double? Force { get; init; }
};