using System;
using System.Collections.Generic;
using System.IO;
using FluentFTP;
using Newtonsoft.Json;
using SpeldesignBotCore.Modules.Minecraft.Entities;
using SpeldesignBotCore.Storage;

namespace SpeldesignBotCore.Modules.Minecraft
{
    public class MinecraftServerDataProvider
    {
        private readonly IDataStorage _dataStorage;

        // Don't get these variable directly, use their respective Get methods instead
        private FtpClient _ftpClient;
        private MinecraftServerConfig _minecraftServerConfig;

        public MinecraftServerDataProvider(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;

            _minecraftServerConfig = GetMinecraftServerConfig();
            _ftpClient = GetConnectedFtpClient();
        }

        public CachedMinecraftUser[] GetCachedMinecraftUsers()
        {
            var userCacheStream = GetConnectedFtpClient().OpenRead("/usercache.json");
            var userCacheStreamReader = new StreamReader(userCacheStream);

            return JsonConvert.DeserializeObject<CachedMinecraftUser[]>(userCacheStreamReader.ReadToEnd());
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
