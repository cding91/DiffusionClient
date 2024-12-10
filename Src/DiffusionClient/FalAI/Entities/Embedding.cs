namespace DiffusionClient.FalAI.Entities;

public record Embedding
{
    /// <summary>
    /// URL or the path to the embedding weights.
    /// </summary>
    public required string Path { get; init; }
    
    /// <summary>
    /// The list of tokens to use for the embedding. Default value: `<s0>,<s1>`
    /// </summary>
    public IList<string>? Tokens { get; init; }
};