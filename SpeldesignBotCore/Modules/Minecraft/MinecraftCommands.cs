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
            public async Task Most(string statisticString)
            {
                statisticString = statisticString.ToEnumString();

                var statisticIsValid = EnumHelper.FindSimilarAndTryParse(statisticString, out MinecraftStatistic statistic);
                if (!statisticIsValid)
                {
                    var closestMatches = Enum.GetNames(typeof(MinecraftStatistic)).FindClosestMatch(statisticString).Take(5).ToArray();
                    await SendNotFoundErrorAsync($"__Could not find statistic \"{statisticString.ToReadableString()}\"__", closestMatches);
                    return;
                }

                await SendMostMessageAsync(statistic, MinecraftAction.Custom);
            }

            [Command]
            public async Task Most(string entityString, string actionString)
            {
                entityString = entityString.ToEnumString();
                actionString = actionString.ToEnumString();

                var actionIsValid = EnumHelper.FindSimilarAndTryParse(actionString, out MinecraftAction action);
                if (!actionIsValid)
                {
                    var closestMatches = Enum.GetNames(typeof(MinecraftAction)).FindClosestMatch(actionString).Take(5).ToArray();
                    await SendNotFoundErrorAsync($"__Could not find action \"{actionString.ToReadableString()}\"__", closestMatches);
                    return;
                }

                // If the action is Killed or KilledBy the entityString is a mob, not an item.
                if (action is MinecraftAction.Killed || action is MinecraftAction.KilledBy)
                {
                    var mobIsValid = EnumHelper.FindSimilarAndTryParse(entityString, out MinecraftMob mob);

                    if (!mobIsValid)
                    {
                        var closestMatches = Enum.GetNames(typeof(MinecraftItem)).FindClosestMatch(entityString).Take(5).ToArray();
                        await SendNotFoundErrorAsync($"__Could not find mob \"{entityString.ToReadableString()}\"__", closestMatches);
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
                        await SendNotFoundErrorAsync($"__Could not find item \"{entityString.ToReadableString()}\"__", closestMatches);
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
                        ? $"Did you mean `{closeMatches[0].ToReadableString()}`?"
                        : $"Did you mean one of these?\n`{string.Join("`\n`", closeMatches.Select(x => x.ToReadableString()))}`");

                await ReplyAsync("", embed: embedBuilder.Build());
            }

            public async Task SendMostMessageAsync<TEnum>(TEnum entity, MinecraftAction action)
            {
                var playerScores = await _serverDataProvider.GetPlayersWithMostInStatisticAsync(entity, action);

                if (playerScores.Length == 0)
                {
                    var errorEmbedBuilder = new EmbedBuilder()
                        .WithTitle(GetEmbedTitleForAction(action, entity))
                        .WithDescription($"There are no players that have done that.")
                        .WithFooter("The entity ID's might be hooked up wrong. Ping @LeMorrow#8192 if you think it's borked!")
                        .WithColor(255, 79, 79);

                    await ReplyAsync("", embed: errorEmbedBuilder.Build());
                    return;
                }

                var stringBuilder = new StringBuilder();
                for (int i = 0; i < playerScores.Length; i++)
                {
                    var statisticString = GetStatisticStringForAction(action, entity, playerScores[i].player.Name, playerScores[i].amount);
                    stringBuilder.AppendLine($"{i + 1}. {statisticString}");
                }

                var embedBuilder = new EmbedBuilder()
                    .WithTitle(GetEmbedTitleForAction(action, entity))
                    .WithDescription(stringBuilder.ToString())
                    .WithColor(118, 196, 177);

                await ReplyAsync("", embed: embedBuilder.Build());
            }

            private string GetEmbedTitleForAction<TEnum>(MinecraftAction action, TEnum entity)
            {
                switch (action)
                {
                    default:                       return $"Most {entity.ToReadableString()} {action.ToReadableString()}";

                    case MinecraftAction.Used:     return $"Most times {entity.ToReadableString()} {action.ToReadableString()}";
                    case MinecraftAction.KilledBy: return $"Most times {action.ToReadableString()} {entity.ToReadableString()}";
                    case MinecraftAction.Custom:   return $"Most {entity.ToReadableString()}";
                }
            }

            private string GetStatisticStringForAction<TEnum>(MinecraftAction action, TEnum entity, string username, int amount)
            {
                if (action is MinecraftAction.Custom)
                {
                    var statistic = (MinecraftStatistic) Enum.Parse(typeof(MinecraftStatistic), entity.ToString());

                    switch (statistic)
                    {
                        default: return $"**{username}** has **{amount}** {statistic.ToReadableString()}.";

                        // Time in ticks
                        case MinecraftStatistic.SneakTime:
                        case MinecraftStatistic.SinceLastDeath:
                        case MinecraftStatistic.SinceLastRest:
                        case MinecraftStatistic.TimePlayed:
                            var timeSpan = new TimeSpan(0, 0, (int) (amount * 0.05f));
                            return timeSpan.TotalDays > 1
                                ? $"**{username}** has **{timeSpan.TotalDays.ToString("0.#")} days** {statistic.ToReadableString()}."
                                : $"**{username}** has **{timeSpan.TotalHours.ToString("0.#")} hours** {statistic.ToReadableString()}.";

                        // Distance in cm
                        case MinecraftStatistic.DistanceByBoat:
                        case MinecraftStatistic.DistanceByElytra:
                        case MinecraftStatistic.DistanceByHorse:
                        case MinecraftStatistic.DistanceByMinecart:
                        case MinecraftStatistic.DistanceByPig:
                            return $"**{username}** has traveled **{amount / 100}** blocks {statistic.ToReadableString().Replace("distance ", "")}.";
                        
                        // Distance in cm
                        case MinecraftStatistic.DistanceClimbed:
                        case MinecraftStatistic.DistanceCrouched:
                        case MinecraftStatistic.DistanceFallen:
                        case MinecraftStatistic.DistanceFlown:
                        case MinecraftStatistic.DistanceSprinted:
                        case MinecraftStatistic.DistanceSwum:
                        case MinecraftStatistic.DistanceWalked:
                        case MinecraftStatistic.DistanceWalkedOnWater:
                        case MinecraftStatistic.DistanceWalkedUnderWater:
                            return $"**{username}** has {statistic.ToReadableString().Replace("distance ", "")} **{amount / 100}** blocks.";
                    }
                }

                switch (action)
                {
                    default:                       return $"**{username}** has {action.ToReadableString()} **{amount}** {entity.ToReadableString()}.";

                    case MinecraftAction.Used:     return $"**{username}** has {action.ToReadableString()} {entity.ToReadableString()} **{amount}** time(s).";
                    case MinecraftAction.KilledBy: return $"**{username}** has been {action.ToReadableString()} {entity.ToReadableString()} **{amount}** time(s).";
                }
            }
        }
    }
}
