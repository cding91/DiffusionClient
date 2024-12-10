using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace DiffusionClient.FalAI.Entities;

/// <summary>
/// Preset image size used by FalAI
/// </summary>
public enum PresetImageSize
{
    /// <summary>
    /// Square image with high definition
    /// </summary>
    SQUARE_HD,
    
    /// <summary>
    /// Square image
    /// </summary>
    SQUARE,
    
    /// <summary>
    /// Portrait image with 4:3 aspect ratio
    /// </summary>
    PORTRAIT_4_3,
    
    /// <summary>
    /// Portrait image with 16:9 aspect ratio
    /// </summary>
    PORTRAIT_16_9,
    
    /// <summary>
    /// Portrait image with 9:16 aspect ratio
    /// </summary>
    LANDSCAPE_4_3,
    
    /// <summary>
    /// Landscape image with 16:9 aspect ratio
    /// </summary>
    LANDSCAPE_16_9,
}