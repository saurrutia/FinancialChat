using FinancialChat.Bot.Handlers;
using FinancialChat.Service;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
   .AddEnvironmentVariables()
   .AddCommandLine(args)
   .Build();

var port = configuration.GetSection("Port")?.Value != null ? configuration.GetSection("Port").Value : "7105";

var channel = GrpcChannel.ForAddress($"https://localhost:{port}");
var client = new ChatRoom.ChatRoomClient(channel);
var commandHandler = new CommandHandler();

using (var chat = client.Join())
{
    _ = Task.Run(async () =>
    {
        while (await chat.ResponseStream.MoveNext(cancellationToken: CancellationToken.None))
        {
            var response = chat.ResponseStream.Current;
            var result = await commandHandler.Handle(response.Text);
            await chat.RequestStream.WriteAsync(new RequestMessage { Name = "financial-bot", ClientType = RequestMessage.Types.ClientType.Robot, Text = result.Result });
        }
    });

    await chat.RequestStream.WriteAsync(new RequestMessage { Name = "financial-bot", ClientType = RequestMessage.Types.ClientType.Robot, Text = "++ financial-bot ready. ++" });

    string line;
    while ((line = Console.ReadLine()) != null)
    {
        if (line.ToLower() == "bye")
        {
            break;
        }
    }
    await chat.RequestStream.CompleteAsync();
}

Console.WriteLine("Disconnecting");
await channel.ShutdownAsync();