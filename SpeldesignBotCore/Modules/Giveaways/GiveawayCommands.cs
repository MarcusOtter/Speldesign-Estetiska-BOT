using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace SpeldesignBotCore.Modules.Giveaways
{
    public class GiveawayCommands : ModuleBase<SocketCommandContext>
    {
        [Command("giveawayrepo")]
        public async Task Test1()
        {
            var giveawayRepo = Unity.Resolve<GiveawayRepo>();
            var items = giveawayRepo.GiveawayItems.Items;

            var embedBuilder = new EmbedBuilder();

            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                string itemType = item.ItemType == GiveawayItemType.Other
                    ? "An item"
                    : $"A {item.ItemType.ToString().ToLowerInvariant()}";

                var owner = Context.Client.GetUser(item.OwnerDiscordId);
                var ownerName = $"{owner.Username}#{owner.Discriminator}"; 

                embedBuilder.AddField($"[{i + 1}] {itemType} donated by {ownerName}", $"[{item.ItemName}]({item.ItemLink})", true);
            }

            await Context.Channel.SendMessageAsync("", false, embedBuilder.Build());
        }
    }
}