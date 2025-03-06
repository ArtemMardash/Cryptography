using MediatR;

namespace Server.Application.Dtos;

public class ChangeCertificateDto: IRequest
{
    public Guid Id { get; set; }
    
    public string newB64 { get; set; }
}