using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace SpeldesignBotCore.Modules
{
    public class HelpCommand : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _commandService;

        public HelpCommand(CommandService commandService)
        {
            _commandService = commandService;
        }

        [Command("help")]
        [Summary("Shows this command."), Remarks("help")]
        public async Task Help()
        {
            var dmChannel = await Context.User.GetOrCreateDMChannelAsync();

            var embedBuilder = GetHelpMessageEmbed();

            try
            {
                await dmChannel.SendMessageAsync("", embed: embedBuilder.Build());
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
                await ReplyAsync("Sent a list of all commands to your DMs.");
            }
        }

        [Command("help")]
        [Summary("*In progress.* Provides more information about a command."), Remarks("help [command]")]
        public async Task Help([Remainder] string query)
        {

        }

        private EmbedBuilder GetHelpMessageEmbed()
        {
            var embedBuilder = new EmbedBuilder()
            {
                Title = "__Help__",
                Description = "Below you can find a list of all the commands.",
                Color = new Color(118, 196, 177)
            };

            foreach(var command in _commandService.Commands)
            {
                embedBuilder.AddField(command.Remarks ?? command.Name, command.Summary ?? "No description available.");
            }

            return embedBuilder;
        }
    }
}
