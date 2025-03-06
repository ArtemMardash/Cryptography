using MediatR;
using Server.Application.Dtos;
using Server.Application.Interfaces;
using Server.Domain.Entities;

namespace Server.Application.RequestHandlers;

public class GetUserByIdRequestHandler: IRequestHandler<GetUserByIdDto, User>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdRequestHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<User> Handle(GetUserByIdDto request, CancellationToken cancellationToken)
    {
        return await _userRepository.GetUserByIdAsync(request.Id, cancellationToken);
    }
}