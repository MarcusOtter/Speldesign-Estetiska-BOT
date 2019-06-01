using SpeldesignBotCore.Storage;

namespace SpeldesignBotCore.Entities
{
    // This whole class is pretty sketchy, needs to be refactored and reorganized
    public class BotConfiguration
    {
        // Can't be readonly because then json can't convert to this format, I think.
        public string Token;
        public string Prefix;
        public ulong LoggingChannelId;
        public ulong RegistrationChannelId;

        private readonly IDataStorage _dataStorage;

        public BotConfiguration()
        {
            // Sketchy because this is how Newtonsoft JSON needs to instantiate it,
            // but Unity might pick this constructor over the other one when doing Unity.Resolve<BotConfiguration>()
        }

        public BotConfiguration(IDataStorage storage)
        {
            _dataStorage = storage;

            var botConfig = storage.RestoreObject<BotConfiguration>("Config/BotConfig");

            Token = botConfig.Token;
            Prefix = botConfig.Prefix;
            LoggingChannelId = botConfig.LoggingChannelId;
            RegistrationChannelId = botConfig.RegistrationChannelId;
        }

        public void Save()
        {
            _dataStorage.StoreObject(this, "Config/BotConfig");
        }
    }
}
