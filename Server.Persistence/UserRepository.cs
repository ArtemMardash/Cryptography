using Microsoft.EntityFrameworkCore;
using Server.Application.Interfaces;
using Server.Domain.Entities;
using Server.Domain.ValueObjects;
using System.IO.Compression;

namespace Server.Persistence;

public class UserRepository : IUserRepository
{
    private readonly ServerContext _serverContext;

    public UserRepository(ServerContext serverContext)
    {
        _serverContext = serverContext;
    }

    public async Task<Guid> CreateUserAsync(User user, CancellationToken cancellationToken)
    {
        if (await _serverContext.Users.AnyAsync(u =>
                u.Id == user.Id ||
                u.B64 == user.Certificate.B64 ||
                u.UserName == user.UserName ||
                u.SKID == user.Certificate.SKID, cancellationToken))
        {
            throw new InvalidOperationException("User with that data already exists");
        }

        await _serverContext.Users.AddAsync(UserToUserDb(user), cancellationToken);
        return user.Id;
    }

    public async Task EditCertificateAsync(Guid id, string newB64, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(newB64))
        {
            throw new InvalidOperationException("Edit Certificate can not be empty");
        }

        var userDb = await _serverContext.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (userDb == null)
        {
            throw new ArgumentNullException(nameof(userDb), "There is no user with such Id");
        }

        userDb.B64 = newB64;
        _serverContext.Users.Update(userDb);
    }

    public async Task<User> GetUserByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var senderDb = await _serverContext.Users
            .Include(u=>u.Packages)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (senderDb == null)
        {
            throw new ArgumentNullException(nameof(senderDb), "There is no user with such Id");
        }

        var recipientsId = senderDb.Packages.Select(p => p.RecipientId).Distinct();
        var recipientsDb = _serverContext.Users.Where(u => recipientsId.Contains(u.Id));
        var recipients = recipientsDb.Select(u => new User(
            u.Id,
            u.UserName,
            new Certificate(u.B64, u.SKID),
            new List<Package>()
        )).ToList();
        
        var sender = new User(
            id: senderDb.Id,
            userName: senderDb.UserName,
            new Certificate(b64: senderDb.B64, skid: senderDb.SKID),
            new List<Package>());

        var packages = senderDb.Packages.Select(p => new Package(
            id: p.Id,
            isDelivered: p.IsDelivered,
            sender: sender,
            recipient: recipients.First(u => u.Id == p.RecipientId),
            filePath: p.FilePath,
            creationDate: p.CreationDate,
            completedTime: p.CompletedTime
        ));
        sender.Packages.AddRange(packages);
        return sender;
    }

    public async Task<User> GetUserByUserNameAsync(string userName, CancellationToken cancellationToken)
    {
        var senderDb = await _serverContext.Users
            .Include(u=>u.Packages)
            .FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);
        if (senderDb == null)
        {
            throw new ArgumentNullException(nameof(senderDb), "There is no user with such user name");
        }

        var recipientsId = senderDb.Packages.Select(p => p.RecipientId).Distinct();
        var recipientsDb = _serverContext.Users.Where(u => recipientsId.Contains(u.Id));
        var recipients = recipientsDb.Select(u => new User(
            u.Id,
            u.UserName,
            new Certificate(u.B64, u.SKID),
            new List<Package>()
        )).ToList();
        
        var sender = new User(
            id: senderDb.Id,
            userName: senderDb.UserName,
            new Certificate(b64: senderDb.B64, skid: senderDb.SKID),
            new List<Package>());

        var packages = senderDb.Packages.Select(p => new Package(
            id: p.Id,
            isDelivered: p.IsDelivered,
            sender: sender,
            recipient: recipients.First(u => u.Id == p.RecipientId),
            filePath: p.FilePath,
            creationDate: p.CreationDate,
            completedTime: p.CompletedTime
        ));
        sender.Packages.AddRange(packages);
        return sender;
    }


    public Task<bool> IsUserExistWithSkidAsync(string skid, CancellationToken cancellationToken)
    {
        return _serverContext.Users.AnyAsync(u => u.SKID == skid, cancellationToken);
    }

    public Task<bool> IsUserExistWithUserNameAsync(string userName, CancellationToken cancellationToken)
    {
        return _serverContext.Users.AnyAsync(u => u.UserName == userName, cancellationToken);
    }

    public Task<bool> IsUserExistWithB64Async(string b64, CancellationToken cancellationToken)
    {
        return _serverContext.Users.AnyAsync(u => u.B64 == b64, cancellationToken);
    }
    
    public async Task UpdateSenderAsync(User user, CancellationToken cancellationToken)
    {
        var userDb = await _serverContext.Users.FindAsync(user.Id, cancellationToken);
        if (userDb == null)
        {
            throw new ArgumentNullException(nameof(user), "There is no user with such Id");
        }

        userDb.UserName = user.UserName;
        userDb.B64 = user.Certificate.B64;
        userDb.SKID = user.Certificate.SKID;
        userDb.Packages = user.Packages.Select(p=>PackageToPackageDb(p,userDb)).ToList();
    }

    internal UserDb UserToUserDb(User user)
    {
        var userDb = new UserDb
        {
            Id = user.Id,
            UserName = user.UserName,
            B64 = user.Certificate.B64,
            SKID = user.Certificate.SKID,
        };
        var packages = user.Packages.Select(p => new PackageDb
        {
            Id = p.Id,
            CompletedTime = p.CompletedTime,
            CreationDate = p.CreationDate,
            FilePath = p.FilePath,
            IsDelivered = p.IsDelivered,
            RecipientId = p.Recipient.Id,
            Sender = userDb
        }).ToList();
        userDb.Packages = packages;
        return userDb;
    }

    // internal User UserDbToUser(UserDb userDb)
    // {
    //     var recipientsId = userDb.Packages.Select(p => p.RecipientId).Distinct();
    //     var recipients = _serverContext.Users.Where(u => recipientsId.Contains(u.Id)).ToList();
    //     return new User(userDb.Id, userDb.UserName, new Certificate(userDb.B64, userDb.SKID),
    //         userDb.Packages.Select(p=>PackageDbToPackage(p, recipients.First(u=>u.Id == p.RecipientId))).ToList());
    // }

    internal PackageDb PackageToPackageDb(Package package, UserDb sender)
    {
        return new PackageDb
        {
            Id = package.Id,
            IsDelivered = package.IsDelivered,
            RecipientId = package.Recipient.Id,
            Sender = sender,
            FilePath = package.FilePath,
            CompletedTime = package.CompletedTime,
            CreationDate = package.CreationDate
        };
    }

    // internal Package PackageDbToPackage(PackageDb packageDb, User recipient)
    // {
    //     return new Package(packageDb.Id, packageDb.IsDelivered, UserDbToUser(packageDb.Sender),recipient
    //         , packageDb.FilePath, packageDb.CreationDate, packageDb.CompletedTime);
    // }


}