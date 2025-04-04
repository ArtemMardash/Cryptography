using System.IO.Compression;
using Server.Application.Interfaces;

namespace Server.Infrastructure.Services;

public class ZipService : IZipService
{
    public async Task<byte[]> CompressAsync(Dictionary<string, byte[]> dataToZip, CancellationToken cancellationToken)
    {
            //var zipFileName = $"file-{Guid.NewGuid()}.zip";
            using var archiveStream = new MemoryStream();
            using var zip = new ZipArchive(archiveStream, ZipArchiveMode.Create, true);
            foreach (var file in dataToZip)
            {
                var entry = zip.CreateEntry(file.Key);
                using (var entryStream = entry.Open())
                using (var fileStream = new MemoryStream(file.Value))
                {
                    await fileStream.CopyToAsync(entryStream, cancellationToken);
                    fileStream.Position = 0;
                }
            }
            
            return archiveStream.ToArray();
    }


    public async Task<Dictionary<string, byte[]>> DeCompressAsync(byte[] file, CancellationToken cancellationToken)
    {
        var decompressedData = new Dictionary<string, byte[]>();
        using (var memoryStream = new MemoryStream(file))
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read))
        {
            foreach (var entry in archive.Entries)
            {
                await using var entryStream = entry.Open();
                using var output = new MemoryStream();
                await entryStream.CopyToAsync(output, cancellationToken);
                decompressedData[entry.Name] = output.ToArray();
            }
        }

        foreach (var data in decompressedData)
        {
           await File.WriteAllBytesAsync($"dec-{data.Key}", data.Value, cancellationToken);
        }
        return decompressedData;
    }
    
}