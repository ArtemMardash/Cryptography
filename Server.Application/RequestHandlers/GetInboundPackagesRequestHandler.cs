using MediatR;
using Server.Application.Dtos;
using Server.Application.Interfaces;

namespace Server.Application.RequestHandlers;

public class GetInboundPackagesRequestHandler : IRequestHandler<GetInboundPackagesDto, GetInboundPackagesResultDto>
{
    private readonly IPackageRepository _packageRepository;

    public GetInboundPackagesRequestHandler(IPackageRepository packageRepository)
    {
        _packageRepository = packageRepository;
    }

    public async Task<GetInboundPackagesResultDto> Handle(GetInboundPackagesDto request,
        CancellationToken cancellationToken)
    {
        var packages = await _packageRepository.GetInboundPackagesAsync(request.UserId, cancellationToken);

        return new GetInboundPackagesResultDto
        {
            Packages = packages.Select(p => new PackageDto
            {
                Id = p.Id,
                IsDelivered = p.IsDelivered,
                SenderId = p.Sender.Id,
                RecipientId = p.Recipient.Id,
                CreationDate = p.CreationDate,
                CompletedTime = p.CompletedTime,
            }).ToList()
        };
    }
}