namespace Server.Persistence;

public class UserDb
{
    public Guid Id { get; set; }
    
    public string UserName { get; set; }
    
    public string B64 { get; set; }
    
    public string SKID { get; set; }

    public virtual ICollection<PackageDb> Packages { get; set; } = new List<PackageDb>();

}