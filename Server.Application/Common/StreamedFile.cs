namespace Server.Application.Common;

public class StreamedFile: IDisposable, IAsyncDisposable
{
    public string FileName { get; set; }

    public Stream Content { get; set; }

    public StreamedFile(string fileName, Stream content)
    {
        FileName = fileName;
        Content = content;
    }

    public void Dispose()
    {
        Content.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await Content.DisposeAsync();
    }
}