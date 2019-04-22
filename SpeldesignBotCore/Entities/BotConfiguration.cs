﻿using SpeldesignBotCore.Storage;

namespace SpeldesignBotCore.Entities
{
    public class BotConfiguration
    {
        public string Token;
        public string Prefix;
        public ulong LoggingChannelId;
        public ulong RegistrationChannelId;

        public BotConfiguration()
        {
            // Sketchy because this is how Newtonsoft JSON needs to instantiate it,
            // but Unity might pick this constructor over the other one when doing Unity.Resolve<BotConfiguration>()
        }

        public BotConfiguration(IDataStorage storage)
        {
            var botConfig = storage.RestoreObject<BotConfiguration>("Config/BotConfig");

            Token = botConfig.Token;
            Prefix = botConfig.Prefix;
            LoggingChannelId = botConfig.LoggingChannelId;
            RegistrationChannelId = botConfig.RegistrationChannelId;

            //Token = storage.RestoreObject<string>("Config/BotToken");
            //LoggingChannelId = storage.RestoreObject<ulong>("Config/LogChannel");
            //RegistrationChannelId = storage.RestoreObject<ulong>("Config/RegistrationChannel");
        }
    }
}