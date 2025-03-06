using MediatR;


namespace Server.Application.Dtos;

public class GetInboundPackagesDto: IRequest<GetInboundPackagesResult>
{
    public Guid UserId { get; set; }
}

public class GetInboundPackagesResult
{
    public List<PackageDto> Packages { get; set; } = new();
}
