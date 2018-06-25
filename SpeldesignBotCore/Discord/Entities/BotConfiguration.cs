using Discord.WebSocket;

namespace SpeldesignBotCore.Discord.Entities
{
    public class BotConfiguration
    {
        public string Token { get; set; }
        public DiscordSocketConfig SocketConfig { get; set; }
    }
}
