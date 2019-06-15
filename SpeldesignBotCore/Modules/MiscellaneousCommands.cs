using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SpeldesignBotCore.Entities;
using System;
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
                .AddField("How to use", $"`{_botConfiguration.Prefix}help`\n__or__\n{botUser.Mention} help", inline: true)
                .WithThumbnailUrl(botUser.GetAvatarUrl(size: 64))
                .AddField("Useful links",
                    "• [Source code](https://github.com/LeMorrow/Speldesign-Estetiska-BOT)\n" +
                    "• [Request a feature](https://github.com/LeMorrow/Speldesign-Estetiska-BOT/issues/new?assignees=LeMorrow&labels=enhancement&template=feature_request.md&title=)\n" +
                    "• [Report an issue](https://github.com/LeMorrow/Speldesign-Estetiska-BOT/issues/new?assignees=LeMorrow&labels=bug&template=bug_report.md&title=Bug+with...)", inline: true)
                .WithFooter("Bot made by LeMorrow#8192")
                .WithColor(118, 196, 177);

            await ReplyAsync("", embed: embedBuilder.Build());
        }

        [Command("shutdown")]
        [Summary("Shuts down the bot."), Remarks("shutdown")]
        [RequireOwner]
        public async Task Shutdown()
        {
            await ReplyAsync("Shutting down...");
            await (Context.Client as DiscordSocketClient).LogoutAsync();
            Environment.Exit(0);
        }

        [Command("tempcommand")]
        [Summary("This is a temporary command"), Remarks("tempcommand")]
        public async Task TempCommand()
        {
            await ReplyAsync("This temporary command was just added. Did it actually run? You're a genius, Marcus. Auto deploy fully works.");
        }
    }
}
