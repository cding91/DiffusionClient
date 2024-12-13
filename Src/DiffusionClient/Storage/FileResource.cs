namespace DiffusionClient.Storage;
using OneOf;

/// <summary>
/// Represents a file resource that can be either a URL or a byte array.
/// </summary>
public class FileResource: OneOfBase<string, byte[]>
{
    /// <summary>
    /// Constructor using a URL
    /// </summary>
    /// <param name="url"></param>
    public FileResource(string url) : base(url) { }
    
    /// <summary>
    /// Constructor using a byte array
    /// </summary>
    /// <param name="file"></param>
    public FileResource(byte[] file) : base(file) { }
}