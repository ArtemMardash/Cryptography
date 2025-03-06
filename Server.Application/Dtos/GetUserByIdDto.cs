using MediatR;
using Server.Domain.Entities;

namespace Server.Application.Dtos;

public class GetUserByIdDto: IRequest<User>
{
    public Guid Id { get; set; }
}