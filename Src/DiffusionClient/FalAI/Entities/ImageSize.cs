using DiffusionClient.FalAI.Utils;
using Newtonsoft.Json;
using OneOf;

namespace DiffusionClient.FalAI.Entities;

/// <summary>
/// Union type for image size
/// </summary>
[JsonConverter(typeof(ImageSizeConverter))]
public class ImageSize: OneOfBase<CustomImageSize, PresetImageSize>
{
    public ImageSize(CustomImageSize customImageSize) : base(customImageSize) { }
    public ImageSize(PresetImageSize presetImageSize) : base(presetImageSize) { }
}