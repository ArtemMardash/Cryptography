using MediatR;
using Server.Application.Dtos;
using Server.Application.Interfaces;

namespace Server.Application.RequestHandlers;

public class GetPackageByIdRequestHandler: IRequestHandler<GetPackageByIdDto, GetPackageByIdResult>
{
    private readonly IPackageRepository _packageRepository;

    public GetPackageByIdRequestHandler(IPackageRepository packageRepository)
    {
        _packageRepository = packageRepository;
    }
    
    public async Task<GetPackageByIdResult> Handle(GetPackageByIdDto request, CancellationToken cancellationToken)
    {
        var package = await _packageRepository.GetPackageByIdAsync(request.Id, cancellationToken);

        return new GetPackageByIdResult
        {
            SenderId = package.Sender.Id,
            RecipientId = package.Recipient.Id
        };
    }
}