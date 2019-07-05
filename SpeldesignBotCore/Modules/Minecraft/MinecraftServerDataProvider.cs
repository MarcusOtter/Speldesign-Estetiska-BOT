using System;
using System.Linq;
using System.IO;
using FluentFTP;
using Newtonsoft.Json;
using SpeldesignBotCore.Modules.Minecraft.Entities;
using SpeldesignBotCore.Modules.Minecraft.Helpers;
using SpeldesignBotCore.Storage;
using System.Threading.Tasks;

namespace SpeldesignBotCore.Modules.Minecraft
{
    public class MinecraftServerDataProvider
    {
        private readonly IDataStorage _dataStorage;

        // Don't get these variables directly, use their respective Get methods instead (they need to be initialized)
        private FtpClient _ftpClient;
        private MinecraftServerConfig _minecraftServerConfig;
        private MinecraftPlayer[] _minecraftPlayers;

        public MinecraftServerDataProvider(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;

            _minecraftServerConfig = GetMinecraftServerConfig();
            _ftpClient = GetConnectedFtpClient();
        }

        public async Task<MinecraftPlayer[]> GetMinecraftPlayersAsync()
        {
            var ftpClient = GetConnectedFtpClient();
            var userCacheStream = await ftpClient.OpenReadAsync("/usercache.json");
            ftpClient.GetReply();

            var userCacheStreamReader = new StreamReader(userCacheStream);
            var jsonData = await userCacheStreamReader.ReadToEndAsync();
            userCacheStreamReader.Close();

            var players = JsonConvert.DeserializeObject<dynamic[]>(jsonData);

            _minecraftPlayers = new MinecraftPlayer[players.Length];

            for (int i = 0; i < players.Length; i++)
            {
                string name = players[i].name;
                string uuid = players[i].uuid;
                string expiresOn = players[i].expiresOn;
                DateTimeOffset lastLogin = expiresOn.MinecraftDateToDateTimeOffset().AddMonths(-1);
                dynamic stats = await GetStatisticsForPlayerAsync(uuid);

                _minecraftPlayers[i] = new MinecraftPlayer(name, uuid, lastLogin, stats);
            }

            return _minecraftPlayers;
        }

        //public CachedMinecraftUser[] GetCachedMinecraftUsers()
        //{
        //    var ftpClient = GetConnectedFtpClient();

        //    var userCacheStream = ftpClient.OpenRead("/usercache.json");
        //    ftpClient.GetReply();

        //    var userCacheStreamReader = new StreamReader(userCacheStream);

        //    return JsonConvert.DeserializeObject<CachedMinecraftUser[]>(userCacheStreamReader.ReadToEnd());
        //}

        public async Task<(MinecraftPlayer player, int score)[]> GetPlayersWithMostInStatisticAsync(MinecraftItem item, MinecraftStatisticAction action, int playersToReturnAmount = 5)
        {
            var itemName = item.ToMinecraftJsonString();
            var actionName = action.ToMinecraftJsonString();

            // Not super clean, I'm sure there's a better way to do this
            var players = await GetMinecraftPlayersAsync();
            players = players
                .Where(x => x.Stats[actionName][itemName] != null)
                .OrderByDescending(x => (int) x.Stats[actionName][itemName])
                .Take(playersToReturnAmount)
                .ToArray();

            var result = new (MinecraftPlayer, int)[players.Length];
            for (int i = 0; i < result.Length; i++)
            {
                var player = players[i];
                result[i] = (player, (int) player.Stats[actionName][itemName]);
            }

            return result;
            //return players
            //    .Where(x => x.Stats[actionName][itemName] != null)
            //    .OrderByDescending(x => (int) x.Stats[actionName][itemName])
            //    .Take(playersToReturnAmount)
            //    .ToArray();
        }

        private async Task<dynamic> GetStatisticsForPlayerAsync(string playerUuid)
        {
            var ftpClient = GetConnectedFtpClient();

            var statisticStream = await ftpClient.OpenReadAsync($"/tantoism/stats/{playerUuid}.json");
            ftpClient.GetReply();

            var statisticStreamReader = new StreamReader(statisticStream);
            var jsonData = await statisticStreamReader.ReadToEndAsync();
            statisticStreamReader.Close();

            return JsonConvert.DeserializeObject<dynamic>(jsonData)["stats"];
        }

        private MinecraftServerConfig GetMinecraftServerConfig()
        {
            if (_minecraftServerConfig != null) { return _minecraftServerConfig; }

            // If the config doesn't exist, return null
            if (!_dataStorage.HasObject("Config/MinecraftServerConfig")) { return null; }

            return _dataStorage.RestoreObject<MinecraftServerConfig>("Config/MinecraftServerConfig");
        }

        private FtpClient GetConnectedFtpClient()
        {
            // If an ftp client already exists, use that
            if (_ftpClient != null)
            {
                if (!_ftpClient.IsConnected) { _ftpClient.Connect(); }
                return _ftpClient;
            }

            var ftpConfig = GetMinecraftServerConfig();
            if (ftpConfig is null) { return null; }

            var ftpClient = new FtpClient(ftpConfig.Host, ftpConfig.Port, ftpConfig.Username, ftpConfig.Password);
            ftpClient.Connect();
            return ftpClient;
        }

    }
}
