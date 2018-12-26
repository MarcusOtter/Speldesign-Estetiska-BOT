namespace SpeldesignBotCore.Modules.Giveaways
{
    [System.Serializable]
    public class GiveawayItem
    {
        public ulong OwnerDiscordId;
        public string ItemName;
        public string SteamPageLink;
        // TODO: Add website link, trailer link etc and then add everything that is not null in the giveawaycommand
    }
}