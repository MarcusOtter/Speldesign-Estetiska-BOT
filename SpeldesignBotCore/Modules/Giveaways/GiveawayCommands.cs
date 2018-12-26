using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SpeldesignBotCore.Helpers;

namespace SpeldesignBotCore.Modules.Giveaways
{
    public class GiveawayCommands : ModuleBase<SocketCommandContext>
    {
        [Command("draw")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task GiveawayDraw(int amount, [Remainder] string args)
        {
            // TODO: Major refactor

            if (amount <= 0)
            {
                await ReplyAsync("You cannot draw less than 1 item.");
                return;
            }

            args = args.ToLowerInvariant().UppercaseFirstChar();
            if (args.EndsWith('s'))
            {
                args = args.Remove(args.Length - 1);
            }

            bool success = Enum.TryParse(args, out GiveawayItemType typeToDraw);

            if (!success)
            {
                await ReplyAsync("That is not a valid giveaway item.");
                return;
            }

            var giveawayRepo = Unity.Resolve<GiveawayRepo>();
            var giveawayItems = giveawayRepo.GiveawayItems.FirstOrDefault(x => x.ItemTypes == typeToDraw);
            var items = giveawayItems.Items;
            var pickedItems = new bool[items.Count];

            var embedBuilder = new EmbedBuilder();

            int itemsToDraw = amount > items.Count ? items.Count : (int) amount;

            var random = new Random();

            var donors = new List<string>();

            for (var i = 0; i < itemsToDraw; i++)
            {
                int randomNumber = random.Next(0, items.Count);
                while (pickedItems[randomNumber] == true)
                {
                    randomNumber = random.Next(0, items.Count);
                }

                var item = items[randomNumber];

                var owner = Context.Client.GetUser(item.OwnerDiscordId);
                var ownerName = $"{owner.Username}#{owner.Discriminator}";

                if (!donors.Contains(ownerName))
                {
                    donors.Add(ownerName);
                }

                var gameNumber = items.IndexOf(item);

                embedBuilder.AddField($"__{gameNumber + 1}. {item.ItemName}__", $"[Steam page]({item.SteamPageLink})", false);
                pickedItems[randomNumber] = true;
            }

            embedBuilder.WithTitle($"Drawing {itemsToDraw} {typeToDraw.ToString().ToLowerInvariant()}s...");
            embedBuilder.WithFooter($"{typeToDraw.ToString()}s donated by {string.Join(", ", donors)}");
            embedBuilder.WithThumbnailUrl("https://images.emojiterra.com/twitter/v11/512px/1f389.png");
            embedBuilder.WithColor(221, 46, 68);
            embedBuilder.WithDescription($"Congratulations! Pick {(typeToDraw == GiveawayItemType.Item ? "an" : "a")} {typeToDraw.ToString().ToLowerInvariant()}!");

            await ReplyAsync("", false, embedBuilder.Build());
        }

        //TODO: Owner command
    }
}