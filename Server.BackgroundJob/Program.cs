using System.Collections.Concurrent;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using MediatR;
using Server.Application.Common;
using Server.Application.Dtos;
using Server.Application.RequestHandlers;
using Server.BackgroundJob;
using Server.Infrastructure;
using Server.Persistence;   

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration["DbConnectionString"] ??
                       builder.Configuration.GetConnectionString("DefaultConnection");

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.RegisterInfrastructure();
builder.Services.RegisterPersistence(connectionString);

var app = builder.Build();
// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseWebSockets();
var connections = new ConcurrentDictionary<Guid, WebSocket>();

//получаем от пользователя файлы(надо использовать Zip Service(create package))
app.Map("/send", async (HttpContext context, IMediator mediator) =>
{
    if (!context.WebSockets.IsWebSocketRequest)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    }

    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
    var recipientId = Guid.Parse(context.Request.Query["recipientId"]);
    var senderId = Guid.Parse(context.Request.Query["senderId"]);
    connections.TryAdd(senderId, webSocket);
    CreatePackageResultDto result = null;
    try
    {
        result = await ReceiveFilesAsync(webSocket, senderId, recipientId, mediator);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }

    return result;
    
});

// // Обрабатываем пакеты и возвращаем GetInboundPackages
// app.Map("/get-inbound", async context =>
// {
//     if (!context.WebSockets.IsWebSocketRequest)
//     {
//         context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
//     }
//
//
//     using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
//     var getInboundHandler = new GetInboundPackagesRequestHandler();
//     var packageHandler = new PackageHandler()
//     var dto = new GetInboundPackagesDto
//     {
//         UserId = default
//     }
//     //нужно периудом в 10 сек доставать пакеты пользователя, которые не обработаны и отправлять обратно 
//     while (true)
//     {
//         var packages = await getInboundHandler.Handle(dto);
//     }
// });
app.Run();

async Task<CreatePackageResultDto> ReceiveFilesAsync(WebSocket webSocket, Guid senderId, Guid recipientId,
    IMediator mediator)
{
    var buffer = new byte[2048];
    var fileData = new List<byte>();
    string? fileName = null;
    var directoryPath = Path.Combine("./outbound", senderId.ToString());
    try
    {
        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Text)
            {
                fileName = Encoding.UTF8.GetString(buffer, 0, result.Count);
            }
            else if (result.MessageType == WebSocketMessageType.Binary)
            {
                fileData.AddRange(buffer.Take(result.Count));
                if (result.EndOfMessage && !string.IsNullOrEmpty(fileName))
                {
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    await File.WriteAllBytesAsync(Path.Combine(directoryPath, fileName), fileData.ToArray());
                    fileData.Clear();
                    fileName = null;
                }
            }
        }
    }
    catch(WebSocketException ex)
    {
        throw new Exception(ex.Message);
    }

    var files = new List<StreamedFile>();
    foreach (var name in Directory.GetFiles(directoryPath))
    {
        var stream = File.OpenRead(name);
        var sf = new StreamedFile(name, stream);
        files.Add(sf);
    }

    var dto = new CreatePackageDto
    {
        SenderId = senderId,
        RecipientId = recipientId,
        Files = files
    };
    return await mediator.Send(dto, CancellationToken.None);
}