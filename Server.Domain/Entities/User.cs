using Server.Domain.ValueObjects;

namespace Server.Domain.Entities;

public class User
{
    private const int USER_NAME_LENGTH = 30;
    
    public Guid Id { get; private set; }
    
    public string UserName { get; private set; }
    
    public Certificate Certificate { get; private set; }

    public List<Package> Packages { get; private set; } = new List<Package>();
    
    public User(Guid id, string userName, Certificate certificate, List<Package> packages)
    {
        Id = id;
        UserName = SetUserName(userName);
        Certificate = certificate;
        Packages = packages;
    }

    public User(string userName, Certificate certificate)
    {
        Id = Guid.NewGuid();
        UserName = SetUserName(userName);
        Certificate = certificate;
    }

    private string SetUserName(string userName)
    {
        
        if (!string.IsNullOrWhiteSpace(userName) && userName.Length <= USER_NAME_LENGTH)
        {
            return userName;
        }
        else
        {
            throw new ArgumentOutOfRangeException(null,$"Invalid user name length. Should be less then {USER_NAME_LENGTH} and it should not be empty");
        }
    }
}