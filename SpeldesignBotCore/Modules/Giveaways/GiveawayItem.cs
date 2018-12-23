namespace SpeldesignBotCore.Modules.Giveaways
{
    [System.Serializable]
    public class GiveawayItem
    {
        public ulong OwnerDiscordId;
        public GiveawayItemType ItemType;
        public string ItemName;
        public string ItemLink;
    }
}