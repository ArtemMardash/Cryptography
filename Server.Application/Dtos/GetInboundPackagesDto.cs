using MediatR;


namespace Server.Application.Dtos;

public class GetInboundPackagesDto: IRequest<GetInboundPackagesResultDto>
{
    public Guid UserId { get; set; }
}

public class GetInboundPackagesResultDto
{
    public List<PackageDto> Packages { get; set; } = new();
    
}
