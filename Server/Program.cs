using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Application.Common;
using Server.Application.Dtos;
using Server.Infrastructure;
using Server.Persistence;

namespace Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var connectionString = builder.Configuration["DbConnectionString"] ??
                               builder.Configuration.GetConnectionString("DefaultConnection");
        //var rabbitHost = builder.Configuration.GetConnectionString("RabbitHost");
        //var rabbitPort = builder.Configuration.GetConnectionString("RabbitPort");

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddAntiforgery();
        builder.Services.RegisterPersistence(connectionString);
        builder.Services.RegisterInfrastructure();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            var context = services.GetService<ServerContext>();
            context?.Database.Migrate();
        }


        app.UseRouting();
        app.UseAntiforgery();
        app.UseAuthorization();


        app.MapPost("/api/server/users/create",
                async ([FromBody] CreateUserDto dto, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var id = await mediator.Send(dto, cancellationToken);
                    return new { id };
                })
            .WithName("CreateUser")
            .WithTags("Users")
            .WithOpenApi();

        app.MapPut("/api/server/users/update",
                async ([FromBody] ChangeCertificateDto dto, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    await mediator.Send(dto, cancellationToken);
                })
            .WithName("ChangeCertificate")
            .WithTags("Users")
            .WithOpenApi();

        app.MapGet("/api/server/users/{id:guid}",
                async (Guid id, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var dto = new GetUserByIdDto { Id = id };
                    return await mediator.Send(dto, cancellationToken);
                })
            .WithName("GetUserById")
            .WithTags("Users")
            .WithOpenApi();

        app.MapGet("/api/server/users/{userName}",
                async (string userName, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var dto = new GetUserByUserNameDto
                    {
                        UserName = userName
                    };
                    return await mediator.Send(dto, cancellationToken);
                })
            .WithName("GetUserByUserName")
            .WithTags("Users")
            .WithOpenApi();

        app.MapPost("/api/server/packages/create/{senderId:guid}/{recipientId:guid}",
                async (Guid senderId, Guid recipientId, IFormFileCollection files,
                    IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var dto = new CreatePackageDto
                    {
                        SenderId = senderId,
                        RecipientId = recipientId,
                        Files = files.Select(f => new StreamedFile(f.FileName, f.OpenReadStream())).ToList()
                    };
                    return await mediator.Send(dto, cancellationToken);
                })
            .WithName("SendPackage")
            .WithTags("Packages")
            .WithOpenApi()
            .DisableAntiforgery();

        //Method to get sent undelivered packages
        app.MapGet("/api/server/packages/outbound/{senderId:guid}",
                async (Guid senderId, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var dto = new GetOutboundPackagesDto
                    {
                        UserId = senderId
                    };
                    return await mediator.Send(dto, cancellationToken);
                })
            .WithName("GetSentPackages")
            .WithTags("Packages")
            .WithOpenApi();

        //Method to get recipient undelivered packages
        app.MapGet("/api/server/packages/inbound/{recipientId:guid}",
                async (Guid recipientId, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var dto = new GetInboundPackagesDto
                    {
                        UserId = recipientId
                    };
                    return await mediator.Send(dto, cancellationToken);
                })
            .WithName("GetInboundPackages")
            .WithTags("Packages")
            .WithOpenApi();

        //Method to get package by Id
        app.MapGet("/api/server/packages/{id:guid}",
                async (Guid id, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var dto = new GetPackageByIdDto
                    {
                        Id = id
                    };
                    return await mediator.Send(dto, cancellationToken);
                })
            .WithName("GetPackageById")
            .WithTags("Packages")
            .WithOpenApi();
        
        app.Run();
    }
}