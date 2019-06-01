using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SpeldesignBotCore.Entities;

namespace SpeldesignBotCore
{
    public class DiscordCommandHandler
    {
        private readonly CommandService _commandService;
        private readonly BotConfiguration _botConfiguration;

        public DiscordCommandHandler(CommandService commandService, BotConfiguration botConfiguration)
        {
            _commandService = commandService;
            _botConfiguration = botConfiguration;
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

            var result = await _commandService.ExecuteAsync(context, argPos);

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
