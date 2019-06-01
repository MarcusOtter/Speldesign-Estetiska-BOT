using SpeldesignBotCore.Storage;
using System.Collections.Generic;

namespace SpeldesignBotCore.Entities
{
    // This whole class is pretty sketchy, needs to be refactored and reorganized
    public class BotConfiguration
    {
        // Can't be readonly because then Newtonsoft JSON can't Deserialize to this object.
        public string Token;
        public string Prefix;
        public ulong LoggingChannelId;
        public ulong RegistrationChannelId;
        public List<ulong> SchoolClassesRoleIds;

        private readonly IDataStorage _dataStorage;

        public BotConfiguration()
        {
            // This constructor is used for Newtonsoft JSON to deserialize the object.
            // When resolving with Unity, it's forced to pick the other constructor through InjectionConstructor.
        }
        
        public BotConfiguration(IDataStorage storage)
        {
            _dataStorage = storage;

            var botConfig = _dataStorage.RestoreObject<BotConfiguration>("Config/BotConfig");

            Token = botConfig.Token;
            Prefix = botConfig.Prefix;
            LoggingChannelId = botConfig.LoggingChannelId;
            RegistrationChannelId = botConfig.RegistrationChannelId;
            SchoolClassesRoleIds = botConfig.SchoolClassesRoleIds;
        }

        public void Save()
        {
            _dataStorage.StoreObject(this, "Config/BotConfig");
        }
    }
}
