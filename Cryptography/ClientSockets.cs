using System.Net.WebSockets;
using System.Text;

namespace Cryptography;

public class ClientSockets
{
    private readonly string _url;

    private ClientWebSocket? _ws;

    public ClientSockets(string url)
    {
        _url = url;
    }

    public Task ConnectAsync()
    {
        _ws = new ClientWebSocket();
        return _ws.ConnectAsync(new Uri(_url), CancellationToken.None);
    }

    public async Task CloseConnectionAsync()
    {
        await _ws.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "closing ws",CancellationToken.None);
    }

    public async Task SendFilesAsync(string folderPath)
    {
        if (_ws == null)
        {
            throw new Exception("No socket connection");
        }

        foreach (var path in Directory.GetFiles(folderPath))
        {
            var name =Path.GetFileName(path);
            var file = File.ReadAllBytes(path);
            await _ws.SendAsync(Encoding.UTF8.GetBytes(name),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
            await _ws.SendAsync(file, WebSocketMessageType.Binary, true, CancellationToken.None);
        }
    }
}