using MediatR;
using Server.Application.Dtos;
using Server.Application.Interfaces;
using Server.Domain.Entities;

namespace Server.Application.RequestHandlers;

public class GetUserByUserNameRequestHandler: IRequestHandler<GetUserByUserNameDto, User>
{
    private readonly IUserRepository _userRepository;

    public GetUserByUserNameRequestHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<User> Handle(GetUserByUserNameDto request, CancellationToken cancellationToken)
    {
        return await _userRepository.GetUserByUserNameAsync(request.UserName, cancellationToken);
    }
}