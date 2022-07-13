using FinancialChat.Domain.Repositories;
using FinancialChat.Infrastructure.Persistence;
using FinancialChat.Infrastructure.Repositories;
using FinancialChat.Service.Handlers;
using FinancialChat.Service.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
    {
        // Setup a HTTP/2 endpoint without TLS.
        options.ListenLocalhost(int.Parse(Environment.GetEnvironmentVariable("Port")), o => o.Protocols =
            HttpProtocols.Http2);
    }    
});


// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddDbContext<FinancialChatDbContext>(opt => opt.UseInMemoryDatabase("FinancialChatDb").EnableSensitiveDataLogging());
builder.Services.AddSingleton<IChatServiceHandler, ChatServiceHandler>();
builder.Services.AddSingleton<IMessageHandler, MessageHandler>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<ChatService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
using (var serviceScope = app.Services.CreateScope()) 
{
    var context = serviceScope.ServiceProvider.GetRequiredService<FinancialChatDbContext>();
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
}

app.Run();
