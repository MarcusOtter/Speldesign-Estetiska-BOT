using Discord;
using Discord.WebSocket;

namespace SpeldesignBotCore.Discord
{
    // TODO: Change tempname
    public class DiscordMessageActualLogger : IMessageLogger
    {
        public async void LogMsgSent(string username, string message, string channelName)
        {
            var embed = new EmbedBuilder();
            embed.WithAuthor(username);
            embed.WithDescription(message);
            embed.WithFooter($"Skickat i #{channelName}");
            embed.WithColor(81, 193, 158);

            // TODO: Get log channel ID from somewhere else.
            var channel = (ISocketMessageChannel)Unity.Resolve<DiscordSocketClient>().GetChannel(458197224489353239);
            await channel.SendMessageAsync("", embed: embed.Build());
        }

        public void LogMsgDeleted(string username, string message, string channelName)
        {
            
        }

        public void LogMsgEdited(string username, string newMessage, string channelName)
        {
            
        }
    }
}
