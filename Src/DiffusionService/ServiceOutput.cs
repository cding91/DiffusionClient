namespace DiffusionService;

/// <summary>
/// Output for the diffusion model. Must be compatible with the model API.
/// </summary>
public record ServiceOutput
{
    public string ImageUrl { get; init; } = string.Empty;
}