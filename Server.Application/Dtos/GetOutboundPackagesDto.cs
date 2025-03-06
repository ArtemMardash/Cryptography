using MediatR;
using Server.Domain.Entities;

namespace Server.Application.Dtos;

public class GetOutboundPackagesDto: IRequest<GetOutboundPackagesResult>
{
    public Guid UserId { get; set; }

}

public class GetOutboundPackagesResult
{
    public List<PackageDto> Packages { get; set; } = new ();
}