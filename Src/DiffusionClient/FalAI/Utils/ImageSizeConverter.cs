using DiffusionClient.FalAI.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace DiffusionClient.FalAI.Utils;

/// <summary>
/// Custom JSON converter for <see cref="ImageSize"/> which a union type of <see cref="CustomImageSize"/> and <see cref="PresetImageSize"/>
/// </summary>
public class ImageSizeConverter: JsonConverter<ImageSize>
{
    /// <summary>
    /// Settings for JSON serialization.
    /// It uses <see cref="SnakeCaseNamingStrategy"/> for naming convention.
    /// </summary>
    private readonly JsonSerializerSettings _settings = new JsonSerializerSettings
    {
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy
            {
                OverrideSpecifiedNames = false,
                ProcessDictionaryKeys = true,
            }
        }
    };
    
    /// <summary>
    /// Write JSON representation of <see cref="ImageSize"/>
    /// </summary>
    /// <inheritdoc />
    /// <exception cref="JsonSerializationException">
    /// Throw exception is <see cref="value"/> is not one of <see cref="CustomImageSize"/> and <see cref="PresetImageSize"/>
    /// </exception>
    public override void WriteJson(JsonWriter writer, ImageSize? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }
        
        if (value.IsT0) // CustomImageSize
        {
            // serializer.Serialize(writer, value.AsT0);
            var json = JsonConvert.SerializeObject(value.AsT0, _settings);
            writer.WriteRawValue(json);
            return;
        }
        
        if (value.IsT1) // PresetImageSize
        {
            // serializer.Serialize(writer, value.AsT1);
            var json = JsonConvert.SerializeObject(value.AsT1, _settings);
            writer.WriteRawValue(json);
            return;
        }
        
        throw new JsonSerializationException("Unexpected value type");
    }

    /// <summary>
    /// Read JSON representation of <see cref="ImageSize"/>
    /// </summary>
    /// <inheritdoc />
    /// <returns>
    /// <see cref="ImageSize"/> or null
    /// </returns>
    /// <exception cref="JsonSerializationException">
    /// Throw exception is token type is not <see cref="JTokenType.Object"/> or <see cref="JTokenType.String"/>
    /// </exception>
    public override ImageSize? ReadJson(JsonReader reader, Type objectType, ImageSize? existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        var token = JToken.Load(reader);

        switch (token.Type)
        {
            case JTokenType.Object:
            {
                // var customImageSize = token.ToObject<CustomImageSize>(serializer);
                var customImageSize = JsonConvert.DeserializeObject<CustomImageSize>(token.ToString(), _settings);
                if (customImageSize == null)
                {
                    throw new JsonSerializationException("Unexpected null value for ImageSize");
                }
                
                return new ImageSize(customImageSize);
            }
            case JTokenType.String:
            {
                // var presetImageSize = token.ToObject<PresetImageSize>(serializer);
                var presetImageSize = JsonConvert.DeserializeObject<PresetImageSize>(token.ToString(), _settings);
                return new ImageSize(presetImageSize);
            }
            default:
                throw new JsonSerializationException("Unexpected token type");
        }
    }
}