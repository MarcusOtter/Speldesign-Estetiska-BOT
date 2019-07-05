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
            public async Task Most(string args)
            {
                await ReplyAsync($"Args: {args}");
            }

            [Command]
            public async Task Most(MinecraftItem item, MinecraftStatisticAction action)
            {
                var playerScores = await _serverDataProvider.GetPlayersWithMostInStatisticAsync(item, action);

                if (playerScores.Length == 0)
                {
                    var errorEmbedBuilder = new EmbedBuilder()
                        .WithTitle($"Most {item.ToReadableString()} {action.ToReadableString()}")
                        .WithDescription($"There are no players that have {action.ToReadableString()} {item.ToReadableString()}.")
                        .WithFooter("The item ID's might be hooked up wrong. Ping @LeMorrow#8192 if you think it's borked!")
                        .WithColor(255, 79, 79);

                    await ReplyAsync("", embed: errorEmbedBuilder.Build());
                    return;
                }

                var stringBuilder = new StringBuilder();
                for (int i = 0; i < playerScores.Length; i++)
                {
                    stringBuilder.AppendLine($"{i + 1}. **{playerScores[i].player.Name}** has {action.ToReadableString()} **{playerScores[i].score}** {item.ToReadableString()}.");
                }

                var embedBuilder = new EmbedBuilder()
                    .WithTitle($"Most {item.ToReadableString()} {action.ToReadableString()}")
                    .WithDescription(stringBuilder.ToString())
                    .WithColor(118, 196, 177);

                await ReplyAsync("", embed: embedBuilder.Build());
            }

            [Command]
            public async Task Most(string itemString, string actionString)
            {
                itemString = itemString.ToEnumString();
                actionString = actionString.ToEnumString();

                Enum.TryParse(itemString,   ignoreCase: true, out MinecraftItem item);
                Enum.TryParse(actionString, ignoreCase: true, out MinecraftStatisticAction action);

                if (item != MinecraftItem.Invalid && action != MinecraftStatisticAction.Invalid)
                {
                    await Most(item, action);
                    return;
                }

                // TODO: Refactor this whole command into reusable methods.

                // Also check if the action is killed, then check for mobs instead of items

                // And if no action is present, make the action = Custom




                var embedBuilder = new EmbedBuilder();

                if (item is MinecraftItem.Invalid)
                {
                    var allItemNames = Enum.GetNames(typeof(MinecraftItem));
                    var closeMatch = allItemNames.FindClosestMatch(itemString, maxDistance: 5).FirstOrDefault();

                    // If there is a close match to the item name, use that item.
                    if (closeMatch != null)
                    {
                        item = (MinecraftItem) Enum.Parse(typeof(MinecraftItem), closeMatch);
                    }
                    else
                    {
                        var closeMatches = allItemNames.FindClosestMatch(itemString);
                        // Limit to 5 items
                        if (closeMatches.Length > 5) { closeMatches = closeMatches.Take(5).ToArray(); }

                        embedBuilder
                            .WithTitle($"__Could not find item {itemString}__")
                            .WithColor(255, 79, 79)
                            .WithDescription(closeMatches.Length == 1
                                ? $"Did you mean `{closeMatches[0]}`?"
                                : $"Did you mean one of these items?\n`{string.Join("`\n`", closeMatches)}`");

                        await ReplyAsync("", embed: embedBuilder.Build());
                        return;
                    }

                }

                if (action is MinecraftStatisticAction.Invalid)
                {
                    var allActionNames = Enum.GetNames(typeof(MinecraftStatisticAction));
                    var closeMatch = allActionNames.FindClosestMatch(actionString, maxDistance: 3).FirstOrDefault();

                    // If there is a close match to the item name, use that item.
                    if (closeMatch != null)
                    {
                        action = (MinecraftStatisticAction) Enum.Parse(typeof(MinecraftStatisticAction), closeMatch);
                    }
                    else
                    {
                        var closeMatches = allActionNames.FindClosestMatch(itemString);
                        // Limit to 5 items
                        if (closeMatches.Length > 5) { closeMatches = closeMatches.Take(5).ToArray(); }

                        embedBuilder
                            .WithTitle($"__Could not find action {actionString}__")
                            .WithColor(255, 79, 79)
                            .WithDescription(closeMatches.Length == 1
                                ? $"Did you mean `{closeMatches[0]}`?"
                                : $"Did you mean one of these actions?\n`{string.Join("`\n`", closeMatches)}`");

                        await ReplyAsync("", embed: embedBuilder.Build());
                        return;
                    }
                }

                await Most(item, action);

                //var embedBuilder = new EmbedBuilder();

                //if ()
            }
        }
    }
}
