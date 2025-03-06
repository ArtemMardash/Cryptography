using MediatR;
using Server.Application.Dtos;
using Server.Application.Interfaces;

namespace Server.Application.RequestHandlers;

public class GetOutboundPackagesRequestHandler: IRequestHandler<GetOutboundPackagesDto, GetOutboundPackagesResult>
{
    private readonly IPackageRepository _packageRepository;

    public GetOutboundPackagesRequestHandler( IPackageRepository packageRepository)
    {
        _packageRepository = packageRepository;
    }
    public async Task<GetOutboundPackagesResult> Handle(GetOutboundPackagesDto request, CancellationToken cancellationToken)
    {
        var packages =await _packageRepository.GetOutboundPackagesAsync(request.UserId, cancellationToken);
    
        return new GetOutboundPackagesResult
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