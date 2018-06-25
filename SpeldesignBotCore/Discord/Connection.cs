using System.Threading.Tasks;
using Discord.WebSocket;
using SpeldesignBotCore.Discord.Entities;

namespace SpeldesignBotCore.Discord
{
    public class Connection
    {
        private DiscordSocketClient _client;
        private readonly DiscordLogger _logger;

        public Connection(DiscordLogger logger)
        {
            _logger = logger;
        }

        internal async Task ConnectAsync(BotConfiguration config)
        {
            _client = new DiscordSocketClient(config.SocketConfig);
            _client.Log += _logger.Log;
        }
    }
}
