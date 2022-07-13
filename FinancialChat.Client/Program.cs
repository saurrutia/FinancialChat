using FinancialChat.Service;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
   .AddEnvironmentVariables()
   .AddCommandLine(args)
   .Build();

var port = configuration.GetSection("Port")?.Value != null ? configuration.GetSection("Port").Value : "7105" ;
var channel = GrpcChannel.ForAddress($"https://localhost:{port}");
var client = new ChatRoom.ChatRoomClient(channel);

var username = string.Empty;
var password = string.Empty;
Console.WriteLine("Welcome to Financial Chat.");
do 
{
    Console.WriteLine("Enter your username:");
    username = Console.ReadLine();
    Console.WriteLine("Enter your password:");
    password = Console.ReadLine();

}
while (!client.Login(new RequestLogin { Username = username, Password = password }).Ok);

Console.WriteLine("Welcome to the chatroom.");

using (var chat = client.Join())
{
    _ = Task.Run(async () =>
    {
        while (await chat.ResponseStream.MoveNext(cancellationToken: CancellationToken.None))
        {
            var response = chat.ResponseStream.Current;
            Console.WriteLine(response.Text);
        }
    });

    await chat.RequestStream.WriteAsync(new RequestMessage { Name = username, ClientType = RequestMessage.Types.ClientType.User, Text = $"{username} has joined the room" });

    string line;
    while ((line = Console.ReadLine()) != null)
    {
        if (line.ToLower() == "bye")
        {
            break;
        }
        await chat.RequestStream.WriteAsync(new RequestMessage { Name = username, ClientType = RequestMessage.Types.ClientType.User, Text = line });
    }
    await chat.RequestStream.CompleteAsync();
}

Console.WriteLine("Disconnecting");
await channel.ShutdownAsync();