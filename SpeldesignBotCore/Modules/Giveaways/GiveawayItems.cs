using System.Collections.Generic;
using System.Linq;

namespace SpeldesignBotCore.Modules.Giveaways
{
    // TODO: Rename to something better, like GiveawayItemGroup or something
    public class GiveawayItems
    {
        public GiveawayItemType ItemTypes;
        public List<GiveawayItem> Items;

        public GiveawayItems(GiveawayItem[] items, GiveawayItemType itemTypes)
        {
            Items = items.ToList();
            ItemTypes = itemTypes;
        }
    }
}