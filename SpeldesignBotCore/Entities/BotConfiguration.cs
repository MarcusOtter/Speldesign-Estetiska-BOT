using SpeldesignBotCore.Storage;
using System.Collections.Generic;
using System.Linq;

namespace SpeldesignBotCore.Entities
{
    public class BotConfiguration
    {
        // Can't be readonly because then Newtonsoft JSON can't Deserialize to this object.
        public string Token;
        public string Prefix;
        public ulong LoggingChannelId;
        public ulong RegistrationChannelId;
        public ulong AlumniRoleId;
        public List<SchoolClass> SchoolClasses;

        public List<ulong> SchoolClassesRoleIds => SchoolClasses.Select(x => x.RoleId).ToList();
        private readonly IDataStorage _dataStorage;

        public BotConfiguration()
        {
            // This constructor is used for Newtonsoft JSON to deserialize the object.
            // When resolving with Unity, it's forced to pick the other constructor through an InjectionConstructor.
        }
        
        public BotConfiguration(IDataStorage storage)
        {
            _dataStorage = storage;

            var botConfig = _dataStorage.RestoreObject<BotConfiguration>("Config/BotConfig");

            Token = botConfig.Token;
            Prefix = botConfig.Prefix;
            LoggingChannelId = botConfig.LoggingChannelId;
            RegistrationChannelId = botConfig.RegistrationChannelId;
            AlumniRoleId = botConfig.AlumniRoleId;
            SchoolClasses = botConfig.SchoolClasses;
        }

        public void Save()
        {
            _dataStorage.StoreObject(this, "Config/BotConfig");
        }
    }
}
