using Server.Domain.Entities;

namespace Server.Application.Interfaces;

public interface IPackageRepository
{
    public Task<List<Package>> GetInboundPackagesAsync(Guid userId, CancellationToken cancellationToken);

    public Task<List<Package>> GetOutboundPackagesAsync(Guid userId, CancellationToken cancellationToken);

    public Task<Guid> CreatePackageAsync(Package package, CancellationToken cancellationToken);

    public Task<Package> GetPackageByIdAsync(Guid id, CancellationToken cancellationToken);
}