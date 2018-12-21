using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using SpeldesignBotCore.Entities;

namespace SpeldesignBotCore.Loggers
{
    public class DiscordMessageLogger
    {
        private readonly ulong _logChannelId;

        private ISocketMessageChannel LogChannel 
            => (ISocketMessageChannel) Unity.Resolve<DiscordSocketClient>().GetChannel(_logChannelId);

        public DiscordMessageLogger()
        {
            _logChannelId = Unity.Resolve<BotConfiguration>().LoggingChannelId;
        }

        public async Task LogToLoggingChannel(SocketMessage message)
        {
            var embed = new EmbedBuilder();
            embed.WithAuthor(GetPrettyAuthorName(message.Author));
            embed.WithDescription(message.Content);
            embed.WithFooter($"Skickat i #{message.Channel.Name}");
            embed.WithColor(81, 193, 158);

            await LogChannel.SendMessageAsync("", embed: embed.Build());
        }

        public void LogMsgDeleted(string username, string message, string channelName)
        {
            // TODO
        }

        public void LogMsgEdited(string username, string newMessage, string channelName)
        {
            // TODO
        }

        /// <summary>
        /// Returns a <see langword="string"/> like "Nickname (Username#1234)" or "Username#1234" depending on if the user has a nickname or not.
        /// </summary>
        private string GetPrettyAuthorName(SocketUser user)
        {
            var nickname = ((SocketGuildUser)user).Nickname;

            return string.IsNullOrWhiteSpace(nickname)
                ? $"{user.Username}#{user.Discriminator}"
                : $"{nickname} ({user.Username}#{user.Discriminator})";
        }
    }
}
