using FinancialChat.Service.Handlers;
using FinancialChat.Service.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
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
builder.Services.AddSingleton<IChatServiceHandler, ChatServiceHandler>();
builder.Services.AddSingleton<IMessageHandler, MessageHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<ChatService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
