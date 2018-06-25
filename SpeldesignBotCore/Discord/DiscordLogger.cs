using System.Threading.Tasks;
using Discord;

namespace SpeldesignBotCore.Discord
{
    public class DiscordLogger
    {
        private readonly ILogger _logger;

        public DiscordLogger(ILogger logger)
        {
            _logger = logger;
        }

        public Task Log(LogMessage logMessage)
        {
            _logger.Log(logMessage.Message);
            return Task.CompletedTask;
        }
    }
}
