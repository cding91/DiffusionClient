using DiffusionClient.FalAI.Entities;

namespace DiffusionClient.FalAI;

/// <summary>
/// Input for the Fast SDXL model
/// </summary>
public record FastSdxlInput
{
    /// <summary>
    /// An id bound to a request, can be used with response to identify the request itself. Default value: ""
    /// </summary>
    public string? RequestId { get; init; } = string.Empty;
    
    /// <summary>
    /// The prompt to use for generating the image. Be as descriptive as possible for best results.
    /// </summary>
    public string Prompt { get; init; } = string.Empty;
    
    /// <summary>
    /// The negative prompt to use. Use it to address details that you don't want in the image.
    /// This could be colors, objects, scenery and even the small details (e.g. moustache, blurry, low resolution). Default value: ""
    /// </summary>
    public string? NegativePrompt { get; init; }
    
    /// <summary>
    /// The size of the generated image. Default value: square_hd
    /// </summary>
    public ImageSize? ImageSize { get; init; }
    
    /// <summary>
    /// The number of inference steps to perform. Default value: 25
    /// </summary>
    public int? NumInferenceSteps { get; init; }
    
    /// <summary>
    /// The same seed and the same prompt given to the same version of Stable Diffusion will output the same image every time.
    /// </summary>
    public int? Seed { get; init; }
    
    /// <summary>
    /// The CFG (Classifier Free Guidance) scale is a measure of how close you want the model to stick to your prompt when looking for a related image to show you. Default value: 7.5
    /// </summary>
    public double? GuidanceScale { get; init; }
    
    /// <summary>
    /// If set to true, the function will wait for the image to be generated and uploaded before returning the response.
    /// This will increase the latency of the function but it allows you to get the image directly in the response without going through the CDN.
    /// </summary>
    public bool? SyncMode { get; init; }
    
    /// <summary>
    /// The number of images to generate. Default value: 1
    /// </summary>
    public int? NumImages { get; init; }
    
    /// <summary>
    /// The list of LoRA weights to use. Default value: ``
    /// </summary>
    public IList<LoraWeight>? Loras { get; init; }
    
    /// <summary>
    /// The list of embeddings to use. Default value: ``
    /// </summary>
    public IList<Embedding>? Embeddings { get; init; }
    
    /// <summary>
    /// If set to true, the safety checker will be enabled. Default value: true
    /// </summary>
    public bool? EnableSafetyChecker { get; init; }
    
    /// <summary>
    /// The version of the safety checker to use. v1 is the default CompVis safety checker. v2 uses a custom ViT model. Default value: "v1"
    /// </summary>
    public SafetyCheckerVersion? SafetyCheckerVersion { get; init; }
    
    /// <summary>
    /// If set to true, the prompt will be expanded with additional prompts.
    /// </summary>
    public string? ExpandPrompt { get; init; }
    
    /// <summary>
    /// The format of the generated image. Default value: "jpeg"
    /// </summary>
    public Format? Format { get; init; }
}