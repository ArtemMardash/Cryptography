namespace Server.Persistence;

public class PackageDb
{
    public Guid Id { get; set; }
    
    public bool IsDelivered { get; set; }
    
    public virtual UserDb Sender { get; set; }
    
    public Guid SenderId { get; set; }
    
    public Guid RecipientId { get; set; }
    
    public string FilePath { get; set; }
    
    public DateTime CreationDate { get; set; }
    
    public DateTime? CompletedTime { get; set; }
}