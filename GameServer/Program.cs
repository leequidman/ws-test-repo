using System.Net;
using GameServer.Repositories;
using GameServer.Services;
using GameServer.Transport;
using Serilog;
using ILogger = Serilog.ILogger;

namespace GameServer;

public class Program
{
    public static async Task Main(string[] args)
    {
        var log = Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        try
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.UseUrls("http://" + Common.Constants.EndpointUrl);
            builder.Host.UseSerilog();

            RegisterDependencies(builder, log);


            var app = builder.Build();

            app.UseWebSockets();


            app.Map("/ws",
                async context =>
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        using var ws = await context.WebSockets.AcceptWebSocketAsync();

                        var webSocketHandler = app.Services.GetService<IWebSocketHandler>();
                        await webSocketHandler!.ReceiveMessage(ws);

                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    }
                });


            await app.RunAsync();
        }
        catch (Exception e)
        {
            Log.Fatal(e, "Unexpected error");
            throw;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }

    private static void RegisterDependencies(WebApplicationBuilder builder, ILogger log)
    {
        builder.Services.AddSingleton(log);
        builder.Services.AddSingleton<IPlayersService, PlayersService>();
        builder.Services.AddSingleton<IConnectionService, ConnectionService>();
        builder.Services.AddSingleton<IPlayersRepository, PlayersRepository>();
        builder.Services.AddSingleton<IWebSocketHandler, WebSocketHandler>();
        builder.Services.AddSingleton<IBaseMessageHandler, BaseMessageHandler>();
        builder.Services.AddSingleton<IEventHandlerProvider, EventHandlerProvider>();
        builder.Services.AddSingleton<IEventParserProvider, EventParserProvider>();

        var handlerTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(IEventHandler).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);
        foreach (var handler in handlerTypes)
            builder.Services.Add(new(typeof(IEventHandler), handler, ServiceLifetime.Singleton));


        var parserTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(IEventParser).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);
        foreach (var parser in parserTypes)
            builder.Services.Add(new(typeof(IEventParser), parser, ServiceLifetime.Singleton));
    }
}