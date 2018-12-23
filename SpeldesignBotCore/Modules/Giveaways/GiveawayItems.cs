using System.Collections.Generic;
using System.Linq;

namespace SpeldesignBotCore.Modules.Giveaways
{
    public class GiveawayItems
    {
        public List<GiveawayItem> Items;

        public GiveawayItems(GiveawayItem[] items)
        {
            Items = items.ToList();
        }
    }
}