using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using OsxPhotos;

namespace SofiBot
{
    class Program
    {
        private DiscordSocketClient client;
        private CommandService commandService;
        private ServiceProvider services;
        private CommandHandler commandHandler;

        static void Main(string[] args) =>
            new Program().MainAsync().GetAwaiter().GetResult();

        Program()
        {
            client = new DiscordSocketClient();

            client.Log += Log;
            client.JoinedGuild += JoinedGuild;
            client.GuildAvailable += JoinedGuild;

            commandService = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Debug,

                CaseSensitiveCommands = false,
            });

            services = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(commandService)
                .AddSingleton<OsxPhotoService>()
                .AddTransient<ShellCommand>()
                .BuildServiceProvider();

            // unused
            // .AddSingleton<ViewFactory>()
            // .AddSingleton<CommandListeners>()
            // .AddScoped<ReactionContext>()

            commandHandler = ActivatorUtilities.CreateInstance<CommandHandler>(services);
        }

        public async Task MainAsync()
        {
            await commandHandler.InstallCommandsAsync();

            await client.LoginAsync(TokenType.Bot, File.ReadAllText("token.txt"));
            await client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task JoinedGuild(SocketGuild guild)
        {
            Console.WriteLine($"Guild Event {guild}");
            // await guild.DefaultChannel.SendMessageAsync("Hello World!");
            await Task.CompletedTask;
        }

        private async Task Log(LogMessage msg)
        {
            Console.WriteLine($"[SofiBot.Log] {msg.ToString()}");
            await Task.CompletedTask;
        }
    }
}
