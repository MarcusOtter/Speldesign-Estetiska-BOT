using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using SpeldesignBotCore.Modules.Minecraft.Entities;

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

        [Command("mc dir")]
        public async Task GetDirectories()
        {
            await ReplyAsync(_serverDataProvider.GetDirectoryNames());
        }
    }
}
