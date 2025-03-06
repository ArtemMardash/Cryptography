namespace Server.Domain.ValueObjects;

public class Certificate
{
    public string B64 { get; private set; }
    
    public string SKID { get; private set; }

    public Certificate(string b64, string skid)
    {
        B64 = SetString(b64, nameof(B64));
        SKID = SetString(skid, nameof(SKID));
    }

    private string SetString(string input, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentOutOfRangeException(null,$"Invalid {fieldName}. Should not be empty");
        }

        return input;
    }
}