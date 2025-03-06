using MediatR;
using Server.Application.Dtos;
using Server.Application.Interfaces;
using Server.Domain.Entities;
using Server.Domain.ValueObjects;

namespace Server.Application.RequestHandlers;

public class CreateUserRequestHandler: IRequestHandler<CreateUserDto, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserRequestHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Guid> Handle(CreateUserDto request, CancellationToken cancellationToken)
    {
        if (await _userRepository.IsUserExistWithSkidAsync(request.SKID, cancellationToken))
        {
            throw new InvalidOperationException("User with such Subject Key ID already exist");
        }

        if (await _userRepository.IsUserExistWithUserNameAsync(request.UserName, cancellationToken))
        {
            throw new InvalidOperationException("User with such UserName already exist");
        }
        
        if (await _userRepository.IsUserExistWithB64Async(request.B64, cancellationToken))
        {
            throw new InvalidOperationException("User with such B64 already exist");
        }

        var user = new User(request.UserName, new Certificate(request.B64, request.SKID));
        var id = await _userRepository.CreateUserAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return id;
    }
}