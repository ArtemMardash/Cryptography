using MediatR;
using Server.Application.Interfaces;

namespace Server.Persistence;

public class UnitOfWork: IUnitOfWork
{
    private readonly ServerContext _serverContext;

    public UnitOfWork(ServerContext serverContext)
    {
        _serverContext = serverContext;
    }

    public void Dispose()
    {
        _serverContext.Dispose();
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _serverContext.SaveChangesAsync(cancellationToken);
    }

    public void SaveChanges()
    {
        _serverContext.SaveChanges();
    }
}