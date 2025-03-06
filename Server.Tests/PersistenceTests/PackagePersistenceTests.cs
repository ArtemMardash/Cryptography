using FluentAssertions;
using Server.Domain.Entities;
using Server.Domain.ValueObjects;
using Server.Persistence;

namespace Server.Tests.PersistenceTests;

public class PackagePersistenceTests
{
    [Fact(DisplayName = "Get inbound packages should success, when user get any packages")]
    public async Task Get_Inbound_Packages_Should_SuccessAsync()
    {
        //Arrange
        var context = Helper.GetContext<ServerContext>("Server");
        var userRepository = new UserRepository(context);
        var packageRepository = new PackageRepository(context);
        
        var senderId = Guid.Parse("b1c8ed74-d05e-4e40-a2c0-31a84a36df55");
        var recipientId = Guid.Parse("ed6e3a08-130a-4f2c-9263-215adc338d19");
        var packageId = Guid.Parse("eccc0dc7-0aa2-4f87-9415-ea4f095a2bff");
        
        var sender = new User(senderId,"Artem123456789", new Certificate("1234567890123477", "0987654321123477"), new List<Package>());
        var recipient = new User(recipientId,"Artem12345677", new Certificate("777777777", "8888888887"), new List<Package>());
        await userRepository.CreateUserAsync(recipient, CancellationToken.None);
        await userRepository.CreateUserAsync(sender, CancellationToken.None);
        
        var package = new Package(id: packageId,
            isDelivered: false, 
            sender: sender,
            recipient: recipient,
            filePath:  $"./files/file_{packageId}_{sender.Id}_{recipient.Id}.zip",
            creationDate: DateTime.Parse("03/23/2025"), 
            completedTime: null);
        
        sender.Packages.Add(package);
        
        await userRepository.UpdateSenderAsync(sender, CancellationToken.None);
        await context.SaveChangesAsync(CancellationToken.None);

        //Act
        var packages = await packageRepository.GetInboundPackagesAsync(recipientId, CancellationToken.None);

        //Assert
        packages.Count.Should().Be(1);
        //packages[0].Should().Be(package);
        await Verify(packages);
    }

    [Fact]
    public async Task Get_Outbound_Packages_Should_SuccessAsync()
    {
        //Arrange
        var context = Helper.GetContext<ServerContext>("Server");
        var userRepository = new UserRepository(context);
        var packageRepository = new PackageRepository(context);
        
        var senderId = Guid.Parse("69a7e662-a38b-4bb9-a7b6-7a2a70a5704e");
        var recipientId = Guid.Parse("770d187e-9ce6-48fa-823e-bceae8976782");
        var packageId = Guid.Parse("b996efb9-b461-4d5a-937e-3e9fb0582723");
        
        var sender = new User(senderId,"Artem123456", new Certificate("123456789012347", "098765432112347"), new List<Package>());
        var recipient = new User(recipientId,"Artem1234567", new Certificate("77777777", "888888888"), new List<Package>());
        await userRepository.CreateUserAsync(sender, CancellationToken.None);
        await userRepository.CreateUserAsync(recipient, CancellationToken.None);
       
        var package = new Package(id: packageId,
            isDelivered: false, 
            sender: sender,
            recipient: recipient,
            filePath:  $"./files/file_{packageId}_{sender.Id}_{recipient.Id}.zip",
            creationDate: DateTime.Parse("02/23/2025"), 
            completedTime: null);
        
        sender.Packages.Add(package);
        
        await userRepository.UpdateSenderAsync(sender, CancellationToken.None);
        await context.SaveChangesAsync(CancellationToken.None);

        //Act
        var packages = await packageRepository.GetOutboundPackagesAsync(senderId, CancellationToken.None);

        //Assert
        packages.Count.Should().Be(1);
        //packages[0].Should().Be(package);
        await Verify(packages);
    }

    [Fact]
    public async Task Create_Package_Should_SuccessAsync()
    {
        //Arrange
        var context = Helper.GetContext<ServerContext>("Server");
        var userRepository = new UserRepository(context);
        var packageRepository = new PackageRepository(context);
        
        var senderId = Guid.Parse("8e2b4d05-b2a8-4958-acfc-14a71f561fcc");
        var recipientId = Guid.Parse("6b0d6675-7858-4e93-894e-d58013ad8d38");
        var packageId = Guid.Parse("fdb1650d-e42f-4969-ad93-a6b99bdcee61");
        
        var sender = new User(senderId,"Artem1234569191", new Certificate("1234567890123479191", "0987654321123479191"), new List<Package>());
        var recipient = new User(recipientId,"Artem12345678181", new Certificate("777777778181", "8888888888181"), new List<Package>());
        await userRepository.CreateUserAsync(sender, CancellationToken.None);
        await userRepository.CreateUserAsync(recipient, CancellationToken.None);
        
        var package = new Package(id: packageId,
            isDelivered: false,
            sender: sender,
            recipient: recipient,
            filePath:  $"./files/file_{packageId}_{sender.Id}_{recipient.Id}.zip",
            creationDate: DateTime.Parse("03/23/2025"), 
            completedTime: null);

        await context.SaveChangesAsync(CancellationToken.None);
        //Act
        var idToCheck=await packageRepository.CreatePackageAsync(package, CancellationToken.None);
        
        //Assert
        package.Id.Should().Be(idToCheck);
        await Verify(package);
    }
    
    [Fact]
    public async Task Get_Package_By_Id_Should_SuccessAsync()
    {
        //Arrange
        var context = Helper.GetContext<ServerContext>("Server");
        var userRepository = new UserRepository(context);
        var packageRepository = new PackageRepository(context);
        
        var senderId = Guid.Parse("d08220f1-6751-481a-88ac-cfb7c6f80e3b");
        var recipientId = Guid.Parse("fdfdf220-d1cd-418e-82eb-655a71701f83");
        var packageId = Guid.Parse("88e8bea7-a7d1-4687-9244-ad94db9cc956");
        
        var sender = new User(senderId,"Artem1234569292", new Certificate("1234567890123479292", "0987654321123479292"), new List<Package>());
        var recipient = new User(recipientId,"Artem12345678282", new Certificate("777777778282", "8888888888282"), new List<Package>());
        await userRepository.CreateUserAsync(sender, CancellationToken.None);
        await userRepository.CreateUserAsync(recipient, CancellationToken.None);
        
        var package = new Package(id: packageId,
            isDelivered: false,
            sender: sender,
            recipient: recipient,
            filePath:  $"./files/file_{packageId}_{sender.Id}_{recipient.Id}.zip",
            creationDate: DateTime.Parse("03/23/2025"), 
            completedTime: null);
        
        await packageRepository.CreatePackageAsync(package, CancellationToken.None);
        await context.SaveChangesAsync(CancellationToken.None);
        
        //Act
        var result = await packageRepository.GetPackageByIdAsync(packageId, CancellationToken.None);
        
        //Assert
        result.Should().NotBeNull();
        await Verify(result);
    }
}