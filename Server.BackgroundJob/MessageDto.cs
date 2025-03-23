using Newtonsoft.Json;

namespace Server.BackgroundJob;

public class MessageDto
{
    public Guid SenderId { get; set; }
    
    public Guid RecipientId { get; set; }
    
    public byte[] Files { get; set; }
}