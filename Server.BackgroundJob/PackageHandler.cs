using Server.Application.Interfaces;

namespace Server.BackgroundJob;

public class PackageHandler
{
    private readonly IPackageRepository _packageRepository;

    public PackageHandler(IPackageRepository packageRepository)
    {
        _packageRepository = packageRepository;
    }

    public async Task<List<(Guid id, string fileName, byte[] data)>> HandleAsync(Guid userId, CancellationToken cancellationToken)
    {
        var packages = await _packageRepository.GetInboundPackagesAsync(userId, cancellationToken);
        var result = new List<(Guid,string,byte[])>();
        foreach (var package in packages)
        {
            var bytes = File.ReadAllBytes(package.FilePath);
            var fileName = package.FilePath.Split('/').Last();
            result.Add((package.Id, fileName, bytes));
        }
        return result;
    } 
}