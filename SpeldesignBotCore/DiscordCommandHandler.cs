using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using SpeldesignBotCore.Contests;
using SpeldesignBotCore.Entities;

namespace SpeldesignBotCore
{
    public class DiscordCommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly BotConfiguration _botConfiguration;
        private readonly IServiceProvider _serviceProvider;

        public DiscordCommandHandler(DiscordSocketClient client, CommandService commandService, BotConfiguration botConfiguration, ContestHandler reactionHandler)
        {
            _client = client;
            _commandService = commandService;
            _botConfiguration = botConfiguration;

            _serviceProvider = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commandService)
                .AddSingleton(this)
                .AddSingleton(reactionHandler)
                .AddSingleton<InteractiveService>()
                .BuildServiceProvider();
        }

        public async Task InstallCommands()
        {
            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly());
            Unity.Resolve<Loggers.StatusLogger>().LogToConsole("Installed command modules");
        }

        public async Task HandleCommand(SocketUserMessage message, SocketCommandContext context)
        {
            int argPos = 0;

            // If the message doesn't start with a prefix nor a mention of this bot
            if (!(message.HasStringPrefix(_botConfiguration.Prefix, ref argPos) 
               || message.HasMentionPrefix(context.Client.CurrentUser, ref argPos)))
            {
                return;
            }

            var result = await _commandService.ExecuteAsync(context, argPos, _serviceProvider);

            if (result.IsSuccess) { return; }

            switch (result.Error)
            {
                case CommandError.UnknownCommand:
                    Unity.Resolve<Loggers.StatusLogger>().LogToConsole(result.ErrorReason);
                    break;

                default:
                    Unity.Resolve<Loggers.StatusLogger>().LogToConsole($"[ERROR] {result.ErrorReason}");

                    var embedBuilder = new EmbedBuilder()
                        .WithTitle("An error occured.")
                        .WithDescription($"Error reason: __{result.ErrorReason}__")
                        .WithColor(255, 79, 79)
                        .WithFooter($"Try {_botConfiguration.Prefix}help {message.Content.Split(' ')[0].Remove(0, _botConfiguration.Prefix.Length)} to get more information");

                    await context.Channel.SendMessageAsync("", embed: embedBuilder.Build());
                    break;
            }
        }
    }
}
