using MediatR;
using Server.Application.Dtos;
using Server.Application.Interfaces;
using Server.Domain.Entities;
using Server.Domain.ValueObjects;

namespace Server.Application.RequestHandlers;

public class ChangeCertificateRequestHandler: IRequestHandler<ChangeCertificateDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeCertificateRequestHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ChangeCertificateDto request, CancellationToken cancellationToken)
    {
        await _userRepository.EditCertificateAsync(request.Id, request.newB64, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}