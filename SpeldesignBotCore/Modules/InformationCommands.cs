using Discord;
using Discord.Commands;
using SpeldesignBotCore.Entities;
using SpeldesignBotCore.Storage;
using System;
using System.Threading.Tasks;

namespace SpeldesignBotCore.Modules
{
    public class InformationCommands : ModuleBase<SocketCommandContext>
    {
        private readonly IDataStorage _dataStorage;
        private readonly BotConfiguration _botConfiguration;

        public InformationCommands()
        {
            _dataStorage = Unity.Resolve<IDataStorage>();
            _botConfiguration = Unity.Resolve<BotConfiguration>();
        }

        [Command("uptime")]
        [Summary("How long the bot has been running"), Remarks("uptime")]
        public async Task Uptime()
        {
            await ReplyAsync($"Uptime: `{GetUptime()}`");
        }

        [Command("info"), Alias("information")]
        [Summary("Information about this bot"), Remarks("info")]
        public async Task Info()
        {
            var botUser = Context.Client.CurrentUser;

            var embedBuilder = new EmbedBuilder()
                .WithTitle($"Information about {botUser.Username}")
                .WithThumbnailUrl(botUser.GetAvatarUrl(size: 64))
                .AddField("__How to use__", $"`{_botConfiguration.Prefix}help` __or__ `@{botUser.Username} help`", inline: true)
                .AddField("__Uptime__", GetUptime(), inline: true)
                .AddField("__Useful links__",
                    "• [Source code](https://github.com/LeMorrow/Speldesign-Estetiska-BOT)\n" +
                    "• [Request a feature](https://github.com/LeMorrow/Speldesign-Estetiska-BOT/issues/new?assignees=LeMorrow&labels=enhancement&template=feature_request.md&title=)\n" +
                    "• [Report an issue](https://github.com/LeMorrow/Speldesign-Estetiska-BOT/issues/new?assignees=LeMorrow&labels=bug&template=bug_report.md&title=Bug+with...)")
                .WithFooter("Made by LeMorrow#8192")
                .WithColor(118, 196, 177);

            await ReplyAsync("", embed: embedBuilder.Build());
        }

        private string GetUptime()
        {
            var timeOfStartup = _dataStorage.RestoreObject<BotInformation>("Info/BotInformation").TimeOfPreviousStartup;
            var uptimeTimeSpan = (DateTime.Now - timeOfStartup);
            return uptimeTimeSpan.ToPrettyString();
        }
    }
}
