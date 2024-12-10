using Newtonsoft.Json;

namespace DiffusionClient.Queue;

/// <summary>
/// Queue status
/// </summary>
public enum QueueStatus
{
    IN_QUEUE,
    
    IN_PROGRESS,
    
    COMPLETED,
}