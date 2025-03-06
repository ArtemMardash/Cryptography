using MediatR;
using Server.Domain.Entities;

namespace Server.Application.Dtos;

public class GetUserByUserNameDto: IRequest<User>
{
    public string UserName { get; set; }
}