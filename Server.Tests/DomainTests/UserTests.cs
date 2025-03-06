using FluentAssertions;
using Server.Domain.Entities;
using Server.Domain.ValueObjects;

namespace Server.Tests.DomainTests;

public class UserTests
{
    [Theory]
    [InlineData("Artem", "sadf", "12345678901")]
    public void Create_User_Should_Successfull(string userName, string b64, string SKID)
    {
        var test = () =>
        {
            var user = new User(userName, new Certificate(b64,SKID));
            return user;
        };
        test.Should().NotThrow();
        var user = test();
        user.UserName.Should().NotBeNull();
        user.Certificate.SKID.Should().NotBeNull();
        user.Certificate.B64.Should().NotBeNull();
    }
    
    [Theory]
    [InlineData("Artem", "", "12345678901" )]
    [InlineData("", "Mardakhaev", "12345678901")]
    [InlineData("Artem", "Mardakhaev", "")]
    [InlineData(null, "Mardakhaev", "12345678901")]
    [InlineData("Artem", null, "12345678901")]
    [InlineData("Artem", "Mardakhaev", null)]
    [InlineData("1234567890123456789012345678901", "Mardakhaev", "12345678901")]
    public void Create_User_Should_Fail(string userName, string b64, string skid)
    {
        var test = () =>
        {
            var user = new User(userName, new Certificate(b64, skid));
            return user;
        };
        test.Should().Throw<Exception>();

    }
}