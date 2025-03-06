using MediatR;
using Server.Application.Common;

namespace Server.Application.Dtos;

public class CreatePackageDto: IRequest<CreatePackageResultDto>
{
    public Guid SenderId { get; set; }
    
    public Guid RecipientId { get; set; }
    
    public List<StreamedFile> Files { get; set; } 
    
}

public class CreatePackageResultDto
{
    public Guid PackageId { get; set; }
}


