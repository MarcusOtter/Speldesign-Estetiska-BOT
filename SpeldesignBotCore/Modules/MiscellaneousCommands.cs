using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SpeldesignBotCore.Entities;
using System.Threading.Tasks;

namespace SpeldesignBotCore.Modules
{
    public class MiscellaneousCommands : ModuleBase<SocketCommandContext>
    {
        private readonly BotConfiguration _botConfiguration;

        public MiscellaneousCommands()
        {
            _botConfiguration = Unity.Resolve<BotConfiguration>();
        }

        [Command("info")]
        [Summary("Information about this bot."), Remarks("info")]
        public async Task Info()
        {
            var botUser = Context.Client.CurrentUser;

            var embedBuilder = new EmbedBuilder()
                .WithTitle($"Information about {botUser.Username}")
                .AddField("Usage info", $"Current prefix: `{_botConfiguration.Prefix}`\nHelp command: `{_botConfiguration.Prefix}help`", inline: true)
                .AddField("Created", botUser.CreatedAt.ToLocalTime().Date.ToString("dd/MM-yyyy"), inline: true)
                .AddField("Useful links",
                    "[Source code](https://github.com/LeMorrow/Speldesign-Estetiska-BOT)\n" +
                    "[Planned features](https://github.com/LeMorrow/Speldesign-Estetiska-BOT/issues?q=is%3Aopen+is%3Aissue+label%3Aenhancement)\n", inline: true)
                .WithFooter("Made by LeMorrow#8192")
                .WithColor(118, 196, 177);

            await ReplyAsync("", embed: embedBuilder.Build());
        }
    }
}
