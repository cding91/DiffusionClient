using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OneOf;

namespace DiffusionClient.Storage;

/// <summary>
/// A client for uploading files to a storage service so that files in the input can be replaced with URLs.
/// </summary>
public class StorageClient
{
    /// <summary>
    /// HTTP client for uploading files.
    /// </summary>
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Serializer settings for JSON.
    /// </summary>
    private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
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
    /// Constructor for the storage client.
    /// </summary>
    /// <param name="httpClient">DI of HTTP client</param>
    public StorageClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Uploads a file to the storage service and return a URL to the file.
    /// </summary>
    /// <param name="file">File to be uploaded</param>
    /// <returns>URL to the file</returns>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request fails.</exception>
    /// <exception cref="JsonException">Thrown when the response cannot be deserialized.</exception>
    public async Task<string> Upload(byte[] file)
    {
        var baseUrl = "https://rest.alpha.fal.ai";
        var targetUrl = $"{baseUrl}/storage/upload/initiate?storage_type=fal-cdn-v3";

        var iniUploadRequest = new InitialUploadRequest();
        var iniUploadResponse = await InitiateUpload(iniUploadRequest);

        using var uploadContent = new ByteArrayContent(file);
        uploadContent.Headers.ContentType = new MediaTypeHeaderValue(iniUploadRequest.ContentType);
        var finalUploadResponse = await _httpClient.PutAsync(iniUploadResponse.UploadUrl, uploadContent);
        finalUploadResponse.EnsureSuccessStatusCode();

        return iniUploadResponse.FileUrl;
    }

    /// <summary>
    /// Request the storage service to initiate the upload process to obtain the URL to upload the file and URL to the file.
    /// </summary>
    /// <param name="request">Initial upload request to the storage service</param>
    /// <returns><see cref="InitialUploadResponse"/><see cref="InitialUploadResponse"/> which contains URLs</returns>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request fails.</exception>
    /// <exception cref="JsonException">Thrown when the response cannot be deserialized.</exception>
    public async Task<InitialUploadResponse> InitiateUpload(InitialUploadRequest request)
    {
        var baseUrl = "https://rest.alpha.fal.ai";
        var targetUrl = $"{baseUrl}/storage/upload/initiate?storage_type=fal-cdn-v3";

        var response = await _httpClient.PostAsync(targetUrl, new StringContent(JsonConvert.SerializeObject(request, _jsonSerializerSettings),
            Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var iniUploadResponse = JsonConvert.DeserializeObject<InitialUploadResponse>(jsonResponse, _jsonSerializerSettings)
                                ?? throw new JsonException("Failed to deserialize the initial upload response");

        return iniUploadResponse;
    }

    /// <summary>
    /// Transforms the input by replacing files with URLs.
    /// </summary>
    /// <param name="input">Input to transform with URLs</param>
    /// <typeparam name="TInput">Type of input</typeparam>
    /// <returns>New input with file replaced by URLs</returns>
    public async Task<TInput> TransformInput<TInput>(TInput input)
    {
        if (input == null)
        {
            return await Task.FromResult(input);
        }

        var properties = typeof(TInput).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType == typeof(FileResource));
        foreach (var p in properties)
        {
            var value = p.GetValue(input) as FileResource;
            
            if (value?.Value is byte[] file)
            {
                var url = await Upload(file);
                p.SetValue(input, new FileResource(url));
            }
        }

        return await Task.FromResult(input);
    }
}