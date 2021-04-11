using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace SofiBot
{
    public class CommandHandler
    {
        private readonly char PREFIX = '!';

        private readonly IServiceProvider services;
        private readonly DiscordSocketClient client;
        private readonly CommandService commands;

        public CommandHandler(IServiceProvider services, DiscordSocketClient client, CommandService commands)
        {
            this.services = services;
            this.client = client;
            this.commands = commands;

            commands.Log += CommandLog;
        }

        public async Task InstallCommandsAsync()
        {
            client.MessageReceived += HandleCommandAsync;
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);
        }

        private async Task CommandLog(LogMessage arg)
        {
            Console.WriteLine($"[CommandService] {arg}");
            await Task.CompletedTask;
        }

        private async Task HandleCommandAsync(SocketMessage msgParam)
        {
            var message = msgParam as SocketUserMessage;
            if (message == null) { return; }

            int argPos = 0;

            if (!(message.HasCharPrefix(PREFIX, ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos)) || message.Author.IsBot)
            {
                return;
            }

            var context = new CommandContext(client, message);
            await commands.ExecuteAsync(context, argPos, services);
        }
    }
}