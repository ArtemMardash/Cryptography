using Server.Domain.Entities;

namespace Server.Application.Interfaces;

public interface IUserRepository
{
    public Task<Guid> CreateUserAsync(User user, CancellationToken cancellationToken);

    public Task EditCertificateAsync(Guid id, string newB64, CancellationToken cancellationToken);

    public Task<User> GetUserByIdAsync(Guid id, CancellationToken cancellationToken);

    public Task<User> GetUserByUserNameAsync(string userName, CancellationToken cancellationToken);
    

    public Task<bool> IsUserExistWithSkidAsync(string skid, CancellationToken cancellationToken);

    public Task<bool> IsUserExistWithUserNameAsync(string userName, CancellationToken cancellationToken);

    public Task<bool> IsUserExistWithB64Async(string b64, CancellationToken cancellationToken);

    public Task UpdateSenderAsync(User user, CancellationToken cancellationToken);

}