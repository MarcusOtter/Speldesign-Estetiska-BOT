using System.Collections.Generic;
using SpeldesignBotCore.Storage;

namespace SpeldesignBotCore.Modules.Giveaways
{
    public class GiveawayRepo
    {
        public readonly GiveawayItems GiveawayItems;

        private IDataStorage _storage;

        private GiveawayItem[] _tempItems;

        public GiveawayRepo(IDataStorage storage)
        {
            _storage = storage;

            _tempItems = new[]
            {
                new GiveawayItem
                {
                    OwnerDiscordId = 199969241531678720,
                    ItemType =  GiveawayItemType.Game,
                    ItemName = "Q.U.B.E: Director’s Cut",
                    ItemLink = "https://store.steampowered.com/app/239430/QUBE_Directors_Cut/",
                },
                new GiveawayItem
                {
                    OwnerDiscordId = 199969241531678720,
                    ItemType =  GiveawayItemType.Game,
                    ItemName = "Orwell: Keeping an eye on you",
                    ItemLink = "https://store.steampowered.com/app/491950/Orwell_Keeping_an_Eye_On_You/",
                },
                new GiveawayItem
                {
                    OwnerDiscordId = 199969241531678720,
                    ItemType = GiveawayItemType.Game,
                    ItemName = "Warhammer: End Times - Vermintide",
                    ItemLink = "https://store.steampowered.com/app/235540/Warhammer_End_Times__Vermintide/"
                }
            };

            _storage.StoreObject(new GiveawayItems(_tempItems), "Giveaway/GiveawayItems");
            GiveawayItems = storage.RestoreObject<GiveawayItems>("Giveaway/GiveawayItems");
        }
    }
}