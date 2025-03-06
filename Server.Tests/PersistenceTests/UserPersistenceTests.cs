using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Server.Domain.Entities;
using Server.Domain.ValueObjects;
using Server.Infrastructure.Services;
using Server.Persistence;

namespace Server.Tests.PersistenceTests;

public class UserPersistenceTests
{
    
    [Theory]
    [InlineData("Artem123", "12345678901123", "12345678901123")]
    public async Task Create_User_Should_SuccessfullAsync(string userName, string b64, string skid)
    {
        //Arrange
        var context = Helper.GetContext<ServerContext>("Server");
        var repository = new UserRepository(context);
        var user = new User(userName, new Certificate(b64, skid));
        //Act
        var id = await repository.CreateUserAsync(user, CancellationToken.None);

        //Assert
        id.Should().NotBeEmpty();
        context.ChangeTracker
            .Entries<UserDb>()
            .Any(e => e.Entity.Id == id && e.State == EntityState.Added)
            .Should()
            .BeTrue();
    }

    [Theory]
    [InlineData("Artem", "09876543211234567", "12345678901", "User with that data already exists")]
    [InlineData("Artem1", "13579", "123456789011", "User with that data already exists")]
    [InlineData("Artem11", "098765432112345671", "24680", "User with that data already exists")]
    public async Task Create_User_Should_FailAsync(string userName, string b64, string skid, string errorMessage)
    {
        //Arange
        var context = Helper.GetContext<ServerContext>("Server");
        var repository = new UserRepository(context);
        var existedUser = new User("Artem", new Certificate("13579", "24680"));
        var existedUserToDb = new UserDb
        {
            B64 = existedUser.Certificate.B64,
            Id = existedUser.Id,
            SKID = existedUser.Certificate.SKID,
            UserName = existedUser.UserName
        };
        await context.Users.AddAsync(existedUserToDb, CancellationToken.None);
        await context.SaveChangesAsync(CancellationToken.None);
        var user = new User(userName, new Certificate(b64, skid));

        //Act
        var test = async () => await repository.CreateUserAsync(user, CancellationToken.None);

        //Assert
        await test.Should().ThrowAsync<InvalidOperationException>().WithMessage(errorMessage);
    }

    [Theory]
    [InlineData("Artem9", "12345678909", "12345678909", "09876543219")]
    public async Task Change_Certificate_Should_SuccessfullAsync(string userName, string newB64, string skid,
        string currentB64)
    {
        //Arrange
        var context = Helper.GetContext<ServerContext>("Server");
        var repository = new UserRepository(context);
        var user = new User(userName, new Certificate(currentB64, skid));
        var id = await repository.CreateUserAsync(user, CancellationToken.None);
        await context.SaveChangesAsync(CancellationToken.None);
        //Act
        await repository.EditCertificateAsync(id, newB64, CancellationToken.None);
        await context.SaveChangesAsync(CancellationToken.None);
        //Assert
        var updatedUser = await repository.GetUserByIdAsync(id, CancellationToken.None);
        updatedUser.Should().NotBeNull();
        updatedUser.Certificate.B64.Should().Be(newB64);
        await Verify(updatedUser);
    }

    [Theory]
    [InlineData("Artem12345", "", "123456789012345", "098765432112345")]
    public async Task Change_Certificate_Should_FailAsync(string userName, string newB64, string skid,
        string currentB64)
    {
        //Arrange
        var context = Helper.GetContext<ServerContext>("Server");
        var repository = new UserRepository(context);
        var user = new User(userName, new Certificate(currentB64, skid));
        //Act
        var id = await repository.CreateUserAsync(user, CancellationToken.None);
        await context.SaveChangesAsync(CancellationToken.None);
        var test = async () => await repository.EditCertificateAsync(id, newB64, CancellationToken.None);

        //Assert
        await test.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact(DisplayName = "Get User By Id Should Success, when user with id exists")]
    public async Task Get_User_By_Id_Should_SuccessAsync()
    {
        //Arrange
        var context = Helper.GetContext<ServerContext>("Server");
        var repository = new UserRepository(context);
        var existedUser = new User("Artem1234", new Certificate("12345678901234", "09876543211234"));
        var id = await repository.CreateUserAsync(existedUser, CancellationToken.None);
        await context.SaveChangesAsync(CancellationToken.None);

        //Act
        var user = await repository.GetUserByIdAsync(id, CancellationToken.None);

        //Assert
        user.Should().NotBeNull();
        await Verify(user);
    }

    [Fact]
    public async Task Get_User_By_Id_Should_FailAsync()
    {
        //Arrange
        var id = Guid.NewGuid();
        var context = Helper.GetContext<ServerContext>("Server");
        var repository = new UserRepository(context);

        //Act
        var test = async () => await repository.GetUserByIdAsync(id, CancellationToken.None);

        //Assert
        await test.Should().ThrowAsync<ArgumentNullException>();
    }
    
    [Fact]
    public async Task Zip_Create_Should_Success()
    {
        //Arrange
        var file1 = File.ReadAllBytes("./signature.txt");
        var file2 = File.ReadAllBytes("./encrypted.txt");
        var zipService = new ZipService();
        //Act
        var result = await zipService.CompressAsync(new Dictionary<string, byte[]>
        {
            ["signature.txt"] = file1,
            ["encrupted.txt"] = file2
        }, CancellationToken.None);
        //Assert
        result.Should().NotBeNull();
        result.Length.Should().BeGreaterThan(0);
    }
    
    [Fact]
    public async Task Zip_Decompress_Should_Success()
    {
        //Arrange
        var file1 = File.ReadAllBytes("./compressed.zip");
        var zipService = new ZipService();
        //Act
        var result = await zipService.DeCompressAsync(file1, CancellationToken.None);
        //Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2);
    }

}