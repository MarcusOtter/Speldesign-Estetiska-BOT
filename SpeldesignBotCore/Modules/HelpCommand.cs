using Discord;
using Discord.Commands;
using SpeldesignBotCore.Entities;
using System.Threading.Tasks;

namespace SpeldesignBotCore.Modules
{
    public class HelpCommand : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _commandService;
        private readonly BotConfiguration _botConfiguration;

        public HelpCommand()
        {
            _commandService = Unity.Resolve<CommandService>();
            _botConfiguration = Unity.Resolve<BotConfiguration>();
        }

        [Command("help")]
        [Summary("Shows a list of all commands."), Remarks("help")]
        public async Task Help()
        {
            var dmChannel = await Context.User.GetOrCreateDMChannelAsync();

            var embedBuilder = GetHelpMessageEmbed();

            IUserMessage dmMessage;

            try
            {
                dmMessage = await dmChannel.SendMessageAsync("", embed: embedBuilder.Build());
            }
            // If the user does not allow direct messages from this server, just send it in the channel.
            catch (Discord.Net.HttpException) 
            {
                embedBuilder.WithFooter("Please consider allowing DMs from this server so this can be sent to you directly.");
                await ReplyAsync("", embed: embedBuilder.Build());
                return;
            }

            if (!Context.IsPrivate)
            {
                var messageLinkEmbedBuilder = new EmbedBuilder()
                    .WithTitle("Sent a list of all commands to your DMs.")
                    .WithDescription($"You can find the message [here](https://discordapp.com/channels/@me/{dmMessage.Channel.Id}/{dmMessage.Id}).")
                    .WithColor(118, 196, 177);

                await ReplyAsync("", embed: messageLinkEmbedBuilder.Build());
            }
        }

        [Command("help")]
        [Summary("*In progress.* Provides more information about a command."), Remarks("help [command]")]
        public async Task Help([Remainder] string command)
        {
            // should make sure to remove the prefix if the user included that.
        }

        // TODO: Split message and return multiple builders if it gets too long.
        private EmbedBuilder GetHelpMessageEmbed()
        {
            var embedBuilder = new EmbedBuilder()
            {
                Title = "__Help__",
                Description = "Below you can find a list of all the commands. Omit square brackets.",
                Color = new Color(118, 196, 177)
            };

            foreach(var command in _commandService.Commands)
            {
                embedBuilder.AddField(
                    $"{_botConfiguration.Prefix}{command.Remarks ?? command.Name}",
                    command.Summary ?? "No description available.");
            }

            return embedBuilder;
        }
    }
}
