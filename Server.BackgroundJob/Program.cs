using System.Net;
using System.Net.WebSockets;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseWebSockets();
app.Map("/ws", async context =>
{
    if (!context.WebSockets.IsWebSocketRequest)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    }

    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
    var buffer = new byte[2048];
    using var incomeStream = new MemoryStream();
    WebSocketReceiveResult incomeData;
    do
    {
        incomeData = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
        incomeStream.Write(buffer, 0, incomeData.Count);

    } while (!incomeData.EndOfMessage);

    var jsonString = Encoding.UTF8.GetString(incomeStream.ToArray());
    
});

app.Run();