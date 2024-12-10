namespace DiffusionService;

/// <summary>
/// Input for the diffusion model. Must be compatible with the model API.
/// </summary>
public record ServiceInput
{
    public string Prompt { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
}