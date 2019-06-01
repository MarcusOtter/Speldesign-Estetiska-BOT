using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using SpeldesignBotCore.Entities;
using SpeldesignBotCore.Loggers;
using SpeldesignBotCore.Registration;

namespace SpeldesignBotCore
{
    public class DiscordMessageHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly BotConfiguration _botConfig;
        private readonly DiscordMessageLogger _messageLogger;
        private readonly DiscordCommandHandler _commandHandler;
        private readonly RegistrationHandler _registrationHandler;

        public DiscordMessageHandler(DiscordSocketClient client, BotConfiguration config,
                                     DiscordMessageLogger logger, DiscordCommandHandler commandHandler,
                                     RegistrationHandler registrationHandler)
        {
            _client = client;
            _botConfig = config;
            _messageLogger = logger;
            _commandHandler = commandHandler;
            _registrationHandler = registrationHandler;
        }

        public async Task HandleMessageAsync(SocketMessage message)
        {
            var msg = message as SocketUserMessage;
            var context = new SocketCommandContext(_client, msg);

            if (context.User.IsBot) { return; }
            if (msg is null) { return; }

            // If this is a DM, no logging or registration should happen,
            // so just handle command and return.
            if (context.IsPrivate)
            {
                await _commandHandler.HandleCommand(msg, context);
                return;
            }

            await _messageLogger.LogToLoggingChannel(msg);

            if (context.Channel.Id == _botConfig.RegistrationChannelId)
            {
                await _registrationHandler.TryRegisterNewUser(context);
                return;
            }

            await _commandHandler.HandleCommand(msg, context);
        }
    }
}