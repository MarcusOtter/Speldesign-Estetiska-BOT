using Discord.WebSocket;

namespace SpeldesignBotCore.Discord
{
    // Should maybe be renamed since it doesn't actually log anything, it just translates it to the IMessageLogger
    public class DiscordMessageLogger 
    {
        private readonly IMessageLogger _messageLogger;

        public DiscordMessageLogger(IMessageLogger messageLogger)
        {
            _messageLogger = messageLogger;
        }
        
        public void LogMsgSent(SocketUserMessage msg)
        {
             _messageLogger.LogMsgSent(GetPrettyAuthorName(msg.Author), msg.Content, msg.Channel.Name);
        }

        public void LogMsgDeleted(SocketUserMessage msg)
        {
            _messageLogger.LogMsgDeleted(msg.Author.Username, msg.Content, msg.Channel.Name);
        }

        public void LogMsgEdited(SocketUserMessage msg)
        {
            _messageLogger.LogMsgEdited(msg.Author.Username, msg.Content, msg.Channel.Name);
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
