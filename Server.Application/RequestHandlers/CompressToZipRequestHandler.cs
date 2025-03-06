using MediatR;
using Server.Application.Dtos;
using Server.Application.Interfaces;

namespace Server.Application.RequestHandlers;

public class CompressToZipRequestHandler: IRequestHandler<CompressToZipDto>
{

    
    public async Task Handle(CompressToZipDto request, CancellationToken cancellationToken)
    {
        
    }
}