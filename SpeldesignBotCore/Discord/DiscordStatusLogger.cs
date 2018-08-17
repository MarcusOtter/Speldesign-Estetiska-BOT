using System.Threading.Tasks;
using Discord;

namespace SpeldesignBotCore.Discord
{
    public class DiscordStatusLogger
    {
        private readonly IStatusLogger _statusLogger;

        public DiscordStatusLogger(IStatusLogger statusLogger)
        {
            _statusLogger = statusLogger;
        }

        public Task Log(LogMessage logMessage)
        {
            _statusLogger.Log(logMessage.Message);
            return Task.CompletedTask;
        }
    }
}
