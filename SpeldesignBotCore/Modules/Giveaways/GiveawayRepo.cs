using System;
using System.Collections.Generic;
using SpeldesignBotCore.Storage;

namespace SpeldesignBotCore.Modules.Giveaways
{
    public class GiveawayRepo
    {
        public readonly List<GiveawayItems> GiveawayItems = new List<GiveawayItems>();

        private readonly IDataStorage _storage;
        
        public GiveawayRepo(IDataStorage storage)
        {
            _storage = storage;
            
            // Load items
            for (int i = 0; i < Enum.GetValues(typeof(GiveawayItemType)).Length; i++)
            {
                GiveawayItemType type = (GiveawayItemType) i;
                GiveawayItems.Add(storage.RestoreObject<GiveawayItems>($"Giveaway/Giveaway{type.ToString()}s"));
            }
        }
    }
}