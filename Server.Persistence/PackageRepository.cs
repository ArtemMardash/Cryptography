using Microsoft.EntityFrameworkCore;
using Server.Application.Interfaces;
using Server.Domain.Entities;
using Server.Domain.ValueObjects;

namespace Server.Persistence;

public class PackageRepository : IPackageRepository
{
    private readonly ServerContext _serverContext;

    public PackageRepository(ServerContext serverContext)
    {
        _serverContext = serverContext;
    }

    /// <summary>
    /// Method to get packages that were sent to user(recipient)
    /// </summary>
    public async Task<List<Package>> GetInboundPackagesAsync(Guid userId, CancellationToken cancellationToken)
    {
        var recipientDb = await _serverContext.Users.FindAsync(userId, cancellationToken);
        if (recipientDb == null)
        {
            throw new ArgumentNullException(nameof(userId), "There is no recipient with such Id");
        }

        var recipient = new User(id: recipientDb.Id, userName: recipientDb.UserName,
            new Certificate(b64: recipientDb.B64, skid: recipientDb.SKID), new List<Package>());

        var packages = _serverContext.Packages.Where(p => p.RecipientId == recipientDb.Id)
            .ToList();
        return packages.Select(p =>
        {
            var senderDb = _serverContext.Users.Find(p.SenderId) ?? throw new InvalidOperationException("no such Id");
            var sender = new User(id: senderDb.Id, userName: senderDb.UserName,
                new Certificate(b64: senderDb.B64, skid: senderDb.SKID), new List<Package>());
            return new Package(p.Id, p.IsDelivered, sender, recipient, p.FilePath, p.CreationDate, p.CompletedTime);

        }).ToList();
    }

    /// <summary>
    /// Method to get sent packages by user (sender)
    /// </summary>
    public async Task<List<Package>> GetOutboundPackagesAsync(Guid userId, CancellationToken cancellationToken)
    {
        var senderDb = await _serverContext.Users.FindAsync(userId, cancellationToken);
        if (senderDb == null)
        {
            throw new ArgumentNullException(nameof(userId), "There is no user with such Id");
        }
        var sender = new User(id: senderDb.Id, userName: senderDb.UserName,
            new Certificate(b64: senderDb.B64, skid: senderDb.SKID), new List<Package>());

        var packages = _serverContext.Packages.Where(p => p.Sender.Id == userId).ToList();
        return packages.Select(p =>
        {
            var recipientDb = _serverContext.Users.Find(p.RecipientId) ?? throw new InvalidOperationException("no such Id");
            var recipient = new User(id: recipientDb.Id, userName: recipientDb.UserName,
                new Certificate(b64: recipientDb.B64, skid: recipientDb.SKID), new List<Package>());
            return new Package(p.Id, p.IsDelivered, sender, recipient, p.FilePath, p.CreationDate, p.CompletedTime);

        }).ToList();
    }

    public async Task<Guid> CreatePackageAsync(Package package, CancellationToken cancellationToken)
    {
        await _serverContext.Packages.AddAsync(new PackageDb
        {
            Id = package.Id,
            IsDelivered = false,
            Sender = await _serverContext.Users.FindAsync(package.Sender.Id, cancellationToken) ?? throw new InvalidOperationException("no such sender"),
            SenderId = package.Sender.Id,
            RecipientId = package.Recipient.Id,
            FilePath = package.FilePath,
            CreationDate = package.CreationDate,
            CompletedTime = package.CompletedTime
        }, cancellationToken);
        
        return package.Id;
    }

    public async Task<Package> GetPackageByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var packageDb = await _serverContext.Packages
            .Include(p => p.Sender)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (packageDb == null)
        {
            throw new ArgumentNullException(nameof(packageDb), "There is no package with such ID");
        }
        
        var sender = new User(packageDb.Sender.Id, packageDb.Sender.UserName,
            new Certificate(packageDb.Sender.B64, packageDb.Sender.SKID), new List<Package>());
        var recipientDb = await _serverContext.Users.FindAsync(packageDb.RecipientId, cancellationToken) ?? throw new InvalidOperationException("no such recipient");
        var recipient = new User(recipientDb.Id, recipientDb.UserName,
            new Certificate(recipientDb.B64, recipientDb.SKID), new List<Package>());

        return new Package(packageDb.Id, packageDb.IsDelivered, sender, recipient, packageDb.FilePath,
            packageDb.CreationDate, packageDb.CompletedTime);
    }
}