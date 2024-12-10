namespace DiffusionClient.FalAI.Entities;

/// <summary>
/// The version of the safety checker to use. v1 is the default CompVis safety checker. v2 uses a custom ViT model. Default value: "v1"
/// </summary>
public enum SafetyCheckerVersion
{
    V1,
    V2
}