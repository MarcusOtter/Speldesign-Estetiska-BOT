using SpeldesignBotCore.Storage;

namespace SpeldesignBotCore.Entities
{
    public class BotConfiguration
    {
        public readonly string Token;
        public readonly ulong LoggingChannelId;
        public readonly ulong RegistrationChannelId;

        public BotConfiguration(IDataStorage storage)
        {
            // TODO: I would like to just have 1 json file to read from (super easy fix).
            // var obj = restore<botconfiguration>(path)

            Token = storage.RestoreObject<string>("Config/BotToken");
            LoggingChannelId = storage.RestoreObject<ulong>("Config/LogChannel");
            RegistrationChannelId = storage.RestoreObject<ulong>("Config/RegistrationChannel");
        }
    }
}
