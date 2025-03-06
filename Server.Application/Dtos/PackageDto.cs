namespace Server.Application.Dtos;

public class PackageDto
{
    public Guid Id { get; set; }
    
    public bool IsDelivered { get; set; }
    
    public Guid SenderId { get; set; }
    
    public Guid RecipientId { get; set; }
    
    public DateTime CreationDate { get; set; }
    
    public DateTime? CompletedTime { get; set; }
}