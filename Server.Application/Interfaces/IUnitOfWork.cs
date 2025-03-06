namespace Server.Application.Interfaces;

public interface IUnitOfWork: IDisposable
{
    /// <summary>
    /// Method to save async 
    /// </summary>
    public Task SaveChangesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Method to save
    /// </summary>
    public void SaveChanges();
}