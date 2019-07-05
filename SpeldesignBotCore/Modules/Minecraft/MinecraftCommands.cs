using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SpeldesignBotCore.Modules.Minecraft.Entities;
using SpeldesignBotCore.Modules.Minecraft.Helpers;

namespace SpeldesignBotCore.Modules.Minecraft
{
    [Group("mc"), Alias("minecraft")]
    public class MinecraftCommands : ModuleBase<SocketCommandContext>
    {
        private readonly MinecraftServerDataProvider _serverDataProvider;

        public MinecraftCommands()
        {
            _serverDataProvider = Unity.Resolve<MinecraftServerDataProvider>();
        }

        // Temporary command!
        [Command("ftpsetup")]
        public async Task MakeServerConfig(string host, int port, string username, string password, string worldName)
        {
            Unity.Resolve<Storage.IDataStorage>().StoreObject(new MinecraftServerConfig(host, port, username, password, worldName), "Config/MinecraftServerConfig");
            await ReplyAsync("Made new ftp client config.");
        }

        [Command("players"), Alias("gamers")]
        public async Task ListPlayers()
        {
            var stringBuilder = new StringBuilder("*These are the players that have logged in at least once in the past month on the SPE16 minecraft server.*\n\n");

            foreach (var player in (await _serverDataProvider.GetMinecraftPlayersAsync()).OrderByDescending(x => x.LastLogin))
            {
                TimeSpan timeSinceLastLogin = DateTimeOffset.Now - player.LastLogin;
                string readableTimeAgo = string.Empty;

                if (timeSinceLastLogin.TotalHours < 1) { readableTimeAgo = "less than **an hour** ago"; }
                else if (timeSinceLastLogin.TotalHours < 2) { readableTimeAgo = "less than **2 hours** ago"; }
                else if (timeSinceLastLogin.TotalDays > 2) { readableTimeAgo = $"about **{timeSinceLastLogin.TotalDays.ToString("0")} days** ago"; }
                else { readableTimeAgo = $"about **{timeSinceLastLogin.TotalHours.ToString("0")} hours** ago"; }

                stringBuilder.AppendLine($"**{player.Name}** logged in {readableTimeAgo}.");
            }

            var embedBuilder = new EmbedBuilder()
                .WithTitle("__Players on the minecraft server__")
                .WithDescription(stringBuilder.ToString())
                .WithThumbnailUrl("https://gamepedia.cursecdn.com/minecraft_gamepedia/thumb/c/c7/Grass_Block.png/150px-Grass_Block.png?version=d571898fb2d2fbc625bd3faaa060b256")
                .WithColor(118, 196, 177);

            await ReplyAsync("", embed: embedBuilder.Build());
        }

        [Group("most")]
        public class MostCommand : ModuleBase<SocketCommandContext>
        {
            private readonly MinecraftServerDataProvider _serverDataProvider;

            public MostCommand()
            {
                _serverDataProvider = Unity.Resolve<MinecraftServerDataProvider>();
            }

            [Command]
            public async Task Most(string entityString, string actionString)
            {
                entityString = entityString.ToEnumString();
                actionString = actionString.ToEnumString();

                var actionIsValid = EnumHelper.FindSimilarAndTryParse(actionString, out MinecraftStatisticAction action);
                if (!actionIsValid)
                {
                    var closestMatches = Enum.GetNames(typeof(MinecraftStatisticAction)).FindClosestMatch(actionString).Take(5).ToArray();
                    await SendNotFoundErrorAsync($"__Could not find action \"{actionString}\"__", closestMatches);
                    return;
                }

                // If the action is Killed the entityString is a mob, not an item.
                if (action is MinecraftStatisticAction.Killed)
                {
                    var mobIsValid = EnumHelper.FindSimilarAndTryParse(entityString, out MinecraftMob mob);

                    if (!mobIsValid)
                    {
                        var closestMatches = Enum.GetNames(typeof(MinecraftItem)).FindClosestMatch(entityString).Take(5).ToArray();
                        await SendNotFoundErrorAsync($"__Could not find mob \"{entityString}\"__", closestMatches);
                        return;
                    }

                    await SendMostMessageAsync(mob, action);
                }
                else
                {
                    var itemIsValid = EnumHelper.FindSimilarAndTryParse(entityString, out MinecraftItem item);

                    if (!itemIsValid)
                    {
                        var closestMatches = Enum.GetNames(typeof(MinecraftItem)).FindClosestMatch(entityString).Take(5).ToArray();
                        await SendNotFoundErrorAsync($"__Could not find item \"{entityString}\"__", closestMatches);
                        return;
                    }

                    await SendMostMessageAsync(item, action);
                }
            }

            private async Task SendNotFoundErrorAsync(string title, string[] closeMatches)
            {
                var embedBuilder = new EmbedBuilder()
                    .WithTitle(title)
                    .WithColor(255, 79, 79)
                    .WithDescription(closeMatches.Length == 1
                        ? $"Did you mean `{closeMatches[0]}`?"
                        : $"Did you mean one of these?\n`{string.Join("`\n`", closeMatches)}`");

                await ReplyAsync("", embed: embedBuilder.Build());
            }

            public async Task SendMostMessageAsync<TEnum>(TEnum entity, MinecraftStatisticAction action)
            {
                var playerScores = await _serverDataProvider.GetPlayersWithMostInStatisticAsync(entity, action);

                string readableEntityName = entity.ToReadableString();
                string readableActionName = action.ToReadableString();

                if (playerScores.Length == 0)
                {
                    var errorEmbedBuilder = new EmbedBuilder()
                        .WithTitle($"Most {readableEntityName} {readableActionName}")
                        .WithDescription($"There are no players that have {readableActionName} {readableEntityName}.")
                        .WithFooter("The item ID's might be hooked up wrong. Ping @LeMorrow#8192 if you think it's borked!")
                        .WithColor(255, 79, 79);

                    await ReplyAsync("", embed: errorEmbedBuilder.Build());
                    return;
                }

                var stringBuilder = new StringBuilder();
                for (int i = 0; i < playerScores.Length; i++)
                {
                    // Ext method for this string depending on action?
                    stringBuilder.AppendLine($"{i + 1}. **{playerScores[i].player.Name}** has {readableActionName} **{playerScores[i].amount}** {readableEntityName}.");
                }

                var embedBuilder = new EmbedBuilder()
                    .WithTitle($"Most {readableEntityName} {readableActionName}")
                    .WithDescription(stringBuilder.ToString())
                    .WithColor(118, 196, 177);

                await ReplyAsync("", embed: embedBuilder.Build());
            }
        }
    }
}
