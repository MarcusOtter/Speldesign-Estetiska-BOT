using Discord;
using Discord.Commands;
using SpeldesignBotCore.Entities;
using System.Threading.Tasks;

namespace SpeldesignBotCore.Modules
{
    public class PrefixCommands : ModuleBase<SocketCommandContext>
    {
        private readonly BotConfiguration _botConfiguration;

        public PrefixCommands()
        {
            _botConfiguration = Unity.Resolve<BotConfiguration>();
        }

        [Command("prefix")]
        [Summary("Responds with the current prefix of the bot."), Remarks("prefix")]
        public async Task Prefix()
        {
            await ReplyAsync($"The command prefix is `{_botConfiguration.Prefix}`\n\nThere are two different ways to invoke a command:" 
                           + $"\n1. `{_botConfiguration.Prefix}help`"
                           + $"\n2. {Context.Client.CurrentUser.Mention} help");
        }

        [Command("setprefix")]
        [Summary("Sets the prefix of the bot."), Remarks("setprefix [prefix]")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task SetPrefix([Remainder] string newPrefix)
        {
            if (newPrefix.Contains(" "))
            {
                await ReplyAsync("The prefix shouldn't contain spaces.");
                return;
            }

            await ReplyAsync($"Changed the prefix from  `{_botConfiguration.Prefix}` to `{newPrefix}`");

            _botConfiguration.Prefix = newPrefix;
            _botConfiguration.Save();

            await Context.Client.SetGameAsync($"for {_botConfiguration.Prefix}help", type: ActivityType.Watching);
        }
    }
}
