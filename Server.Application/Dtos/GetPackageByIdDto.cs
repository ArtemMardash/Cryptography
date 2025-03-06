using MediatR;

namespace Server.Application.Dtos;

public class GetPackageByIdDto: IRequest<GetPackageByIdResult>
{
    public Guid Id { get; set; }
}

public class GetPackageByIdResult
{
    public Guid SenderId { get; set; }
    
    public Guid RecipientId { get; set; }
}