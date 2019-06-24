using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using SpeldesignBotCore.Entities;

namespace SpeldesignBotCore.Loggers
{
    public class DiscordMessageLogger
    {
        private readonly ulong _logChannelId;
        private readonly DiscordSocketClient _client;

        private ISocketMessageChannel _logChannel 
            => (ISocketMessageChannel) _client.GetChannel(_logChannelId);

        public DiscordMessageLogger(DiscordSocketClient client)
        {
            _client = client;
            _logChannelId = Unity.Resolve<BotConfiguration>().LoggingChannelId;
        }

        public async Task LogMessageSent(SocketMessage message, SocketGuild guild)
        {
            if (_logChannel is null) { return; }

            var embedBuilder = new EmbedBuilder()
                .WithAuthor(GetPrettyAuthorName(message.Author))
                .WithThumbnailUrl(message.Author.GetAvatarUrl(size: 32))
                .WithDescription($"{message.Content}\n" + @"**\_\_\_\_\_\_\_\_\_\_**" 
                    + $"\n[*Message sent in #{message.Channel.Name}*](https://discordapp.com/channels/{guild.Id}/{message.Channel.Id}/{message.Id})")
                .WithColor(118, 196, 177)
                .WithCurrentTimestamp();

            await _logChannel.SendMessageAsync("", embed: embedBuilder.Build());
        }

        public async Task LogMessageEdited(SocketMessage messageBefore, SocketMessage messageAfter, SocketGuild guild)
        {
            if (_logChannel is null) { return; }

            var embedBuilder = new EmbedBuilder()
                .WithAuthor(GetPrettyAuthorName(messageAfter.Author))
                .WithThumbnailUrl(messageAfter.Author.GetAvatarUrl(size: 32))
                .AddField("__Message before__", messageBefore.Content, inline: true)
                .AddField("__Message after__", messageAfter.Content, inline: true)
                .AddField(@"\_\_\_\_\_\_\_\_\_\_", $"\n\n[*Message edited in #{messageAfter.Channel.Name}*](https://discordapp.com/channels/{guild.Id}/{messageAfter.Channel.Id}/{messageAfter.Id})")
                .WithColor(118, 196, 177)
                .WithCurrentTimestamp();

            await _logChannel.SendMessageAsync("", embed: embedBuilder.Build());  
        }

        public void LogMessageDeleted(string username, string message, string channelName)
        {
            // TODO
        }

        /// <summary>
        /// Returns a <see langword="string"/> like "Nickname (Username#1234)" or "Username#1234" depending on if the user has a nickname or not.
        /// </summary>
        private string GetPrettyAuthorName(SocketUser user)
        {
            var nickname = ((SocketGuildUser) user).Nickname;

            return string.IsNullOrWhiteSpace(nickname)
                ? $"{user.Username}#{user.Discriminator}"
                : $"{nickname} ({user.Username}#{user.Discriminator})";
        }
    }
}
