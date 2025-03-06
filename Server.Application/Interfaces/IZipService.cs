namespace Server.Application.Interfaces;

public interface IZipService
{
    public Task<byte[]> CompressAsync(Dictionary<string,byte[]> dataToZip, CancellationToken cancellationToken);

    public Task<Dictionary<string, byte[]>> DeCompressAsync(byte[] file, CancellationToken cancellationToken);
}