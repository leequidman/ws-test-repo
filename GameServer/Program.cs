using System.Net;
using Common.EventHandling;
using Common.Transport;
using GameServer.Handlers;
using GameServer.Repositories;
using GameServer.Services;
using Serilog;
using ILogger = Serilog.ILogger;

namespace GameServer
{
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
                builder.WebHost.UseUrls("http://localhost:13371"); // todo: to appsettings/cmd params/constants
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

            var handlerTypes = typeof(Program).Assembly.GetTypes()
                .Where(type => typeof(IEventHandler).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);

            foreach (var rule in handlerTypes) 
                builder.Services.Add(new(typeof(IEventHandler), rule, ServiceLifetime.Singleton));
        }
    }
}