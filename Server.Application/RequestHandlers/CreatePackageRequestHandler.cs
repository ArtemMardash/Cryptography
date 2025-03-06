using MediatR;
using Server.Application.Dtos;
using Server.Application.Interfaces;
using Server.Domain.Entities;

namespace Server.Application.RequestHandlers;

public class CreatePackageRequestHandler : IRequestHandler<CreatePackageDto, CreatePackageResultDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IZipService _zipService;
    private readonly IPackageRepository _packageRepository;

    public CreatePackageRequestHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IZipService zipService, IPackageRepository packageRepository)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _zipService = zipService;
        _packageRepository = packageRepository;
    }

    public async Task<CreatePackageResultDto> Handle(CreatePackageDto request, CancellationToken cancellationToken)
    {
        var sender = await _userRepository.GetUserByIdAsync(request.SenderId, cancellationToken);
        var recipient = await _userRepository.GetUserByIdAsync(request.RecipientId, cancellationToken);
        var package = new Package(sender, recipient);
        //sender.Packages.Add(package);

        var dataToZip = request.Files.Select(f =>
            {
                using var memoryStream = new MemoryStream();
                f.Content.CopyTo(memoryStream);
                return new { f.FileName, Content = memoryStream.ToArray() };
            })
            .ToDictionary(f => f.FileName, c => c.Content);

        var zipFile = await _zipService.CompressAsync(dataToZip, cancellationToken);
        await File.WriteAllBytesAsync(package.FilePath, zipFile, cancellationToken);
        package.CreationDate = DateTime.UtcNow;
        //await _userRepository.UpdateSenderAsync(sender, cancellationToken);
        var packageId = await _packageRepository.CreatePackageAsync(package, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new CreatePackageResultDto
        {
            PackageId = packageId
        };
    }
}