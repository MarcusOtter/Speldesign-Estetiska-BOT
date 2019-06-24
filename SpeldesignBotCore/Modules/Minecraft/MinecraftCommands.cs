using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SpeldesignBotCore.Modules.Minecraft.Entities;
using SpeldesignBotCore.Modules.Minecraft.Helpers;

namespace SpeldesignBotCore.Modules.Minecraft
{
    public class MinecraftCommands : ModuleBase<SocketCommandContext>
    {
        private readonly MinecraftServerDataProvider _serverDataProvider;

        public MinecraftCommands()
        {
            _serverDataProvider = Unity.Resolve<MinecraftServerDataProvider>();
        }

        // Temporary command!
        [Command("mc ftpsetup")]
        public async Task MakeServerConfig(string host, int port, string username, string password, string worldName)
        {
            Unity.Resolve<Storage.IDataStorage>().StoreObject(new MinecraftServerConfig(host, port, username, password, worldName), "Config/MinecraftServerConfig");
            await ReplyAsync("Made new ftp client config.");
        }

        [Command("mc players")]
        public async Task ListPlayers()
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("__Players on the minecraft server__")
                .WithDescription("*These are the players that have logged in at least once in the past month on the SPE16 minecraft server.*")
                .WithThumbnailUrl("https://gamepedia.cursecdn.com/minecraft_gamepedia/thumb/c/c7/Grass_Block.png/150px-Grass_Block.png?version=d571898fb2d2fbc625bd3faaa060b256")
                .WithColor(118, 196, 177);

            foreach (var user in _serverDataProvider.GetCachedMinecraftUsers().OrderByDescending(x => x.ExpiresOn))
            {
                var expirationDate = user.ExpiresOn.ToDateTime();
                var utcOffset = user.ExpiresOn.GetUtcOffset();
                TimeSpan lastLogin = DateTime.UtcNow.AddHours(utcOffset) - expirationDate.AddMonths(-1);
                embedBuilder.AddField(user.Name, $"Last login {lastLogin.ToPrettyString()} ago.", inline: true);
            }

            await ReplyAsync("", embed: embedBuilder.Build());
        }
    }
}
