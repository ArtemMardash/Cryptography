using FluentAssertions;
using NSubstitute;
using Server.Application.Dtos;
using Server.Application.Interfaces;
using Server.Application.RequestHandlers;
using Server.Domain.Entities;
using Server.Domain.ValueObjects;
using Server.Persistence;

namespace Server.Tests.ApplicationTests;

public class CreateUserRequestHandlerTest
{
    [Fact]
    public async Task Create_User_Request_Handler_Should_Success()
    {
        //Arrange
        var dto = new CreateUserDto
        {
            B64 = "12345678901111",
            SKID = "098765432111111",
            UserName = "Artem1111"
        };
        var context = Helper.GetContext<ServerContext>("users");
        var repository = new UserRepository(context);
        var unitOfWork = new UnitOfWork(context);
        var createUserRequestHandler = new CreateUserRequestHandler(repository, unitOfWork);
        
        //Act
        var result = await createUserRequestHandler.Handle(dto, CancellationToken.None);
        var user = await repository.GetUserByIdAsync(result, CancellationToken.None);

        //Assert
        result.Should().NotBeEmpty();
        await Verifier.Verify(user);
    }
    
    [Theory]
    [InlineData("123456789011", "09876543211", "Artem")] //Check that UserName is the same
    [InlineData("12345678901", "0987654321", "M")]     //Check that SKID is the same
    [InlineData("1234567890", "098765432111", "Artem11")]   //Check that B64 is the same
    public async Task Create_User_Request_Handler_Should_Fail(string b64, string skid, string userName)
    {
        //Arange
        var dto = new CreateUserDto
        {
            B64 = b64,
            SKID = skid,
            UserName = userName
        };
        var context = Helper.GetContext<ServerContext>("users");
        var repository = new UserRepository(context);
        var unitOfWork = new UnitOfWork(context);
        var createUserRequestHandler = new CreateUserRequestHandler(repository, unitOfWork);
        await context.Users.AddAsync(new UserDb
        {
            Id = Guid.NewGuid(),
            B64 = "1234567890",
            SKID = "0987654321",
            UserName = "Artem"
        });
        await unitOfWork.SaveChangesAsync(CancellationToken.None);
        
        //Act
        var test =async ()=> await createUserRequestHandler.Handle(dto, CancellationToken.None);

        //Assert
        await test.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Update_Certificate_Request_Handler_Should_Success()
    {
        //Arange
        var context = Helper.GetContext<ServerContext>("users");
        var repository = new UserRepository(context);
        var unitOfWork = new UnitOfWork(context);
        var user1 = new User("arte", new Certificate("12345678", "09876543"));
        var id = await repository.CreateUserAsync(user1, CancellationToken.None);
        await context.SaveChangesAsync(CancellationToken.None);
        var changeCertificateRh = new ChangeCertificateRequestHandler(repository, unitOfWork);
        
        //Act
        await changeCertificateRh.Handle(new ChangeCertificateDto { Id = id, newB64 = "1357908642" },
            CancellationToken.None);
        var user = await repository.GetUserByIdAsync(id, CancellationToken.None);
        
        //Assert
        user.Certificate.B64.Should().Be("1357908642");
        await Verifier.Verify(user);
    }


    public async Task Get_Sent_Packages_Request_Handler_Should_Success()
    {
        //Arange
        var context = Helper.GetContext<ServerContext>("users");
        var userRepository = new UserRepository(context);
        var packageRepository = new PackageRepository(context);
        var user = new User("arte", new Certificate("12345678", "09876543"));
        var id = await userRepository.CreateUserAsync(user, CancellationToken.None);
        await context.SaveChangesAsync();
        
        //Act
        var sentPackagesRequestHandler = new GetOutboundPackagesRequestHandler(packageRepository);
        var result = await sentPackagesRequestHandler.Handle(new GetOutboundPackagesDto
        {
            UserId = id
        }, CancellationToken.None);
    }
    
}