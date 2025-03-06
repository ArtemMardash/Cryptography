using MediatR;

namespace Server.Application.Dtos;

public class CreateUserDto: IRequest<Guid>
{
    public string UserName { get; set; }
    
    public string B64 { get; set; }
    
    public string SKID { get; set; }

}