namespace Server.Domain.Entities;

public class Package
{
    public Guid Id { get; set; }
    
    public bool IsDelivered { get; set; }
    
    public User Sender { get; set; }
    
    public User Recipient { get; set; }
    
    public string FilePath { get; set; }
    
    public DateTime CreationDate { get; set; }
    
    public DateTime? CompletedTime { get; set; }

    public Package(Guid id, bool isDelivered, User sender, User recipient, string filePath, DateTime creationDate, DateTime? completedTime)
    {
        Id = id;
        IsDelivered = isDelivered;
        Sender = sender;
        Recipient = recipient;
        FilePath = filePath;
        CreationDate = creationDate;
        CompletedTime = completedTime;
    }

    public Package(User sender, User recipient)
    {
        Id = Guid.NewGuid();
        IsDelivered = false;
        Sender = sender;
        Recipient = recipient;
        FilePath = $"./files/file_{Id}_{sender.Id}_{recipient.Id}.zip";
    }
}