using System.Threading.Tasks;
using Grpc.Net.Client;
using FinancialChat.Client;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

using var channel = GrpcChannel.ForAddress($"https://localhost:{configuration.GetValue<int>("Server:Port")}");
var client = new Greeter.GreeterClient(channel);
Console.WriteLine("What's your name?.");
var name = Console.ReadLine();
var reply = await client.SayHelloAsync(
                  new HelloRequest { Name = name });
Console.WriteLine("Greeting: " + reply.Message);
Console.WriteLine("Press any key to exit...");

Console.ReadKey();