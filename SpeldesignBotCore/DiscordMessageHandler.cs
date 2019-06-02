using System.Threading.Tasks;
using Discord;
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

        public async Task HandleMessageEditedAsync(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
        {
            var messageBeforeEdit = before.Value as SocketUserMessage;
            var messageAfterEdit = after as SocketUserMessage;

            if (messageBeforeEdit is null || messageAfterEdit is null) { return; }

            var context = new SocketCommandContext(_client, messageAfterEdit);
            if (context.User.IsBot) { return; }

            // If this is the first time this message is edited, try executing a command on it
            // (some users edit their messages to fix their incorrect command usage)
            if (messageBeforeEdit.EditedTimestamp is null)
            {
                await _commandHandler.HandleCommand(messageAfterEdit, context);
            }

            // Don't do anything else if the message was sent in DMs
            if (context.IsPrivate) { return; }

            await _messageLogger.LogMessageEdited(messageBeforeEdit, messageAfterEdit, context.Guild);

            // If the message is in the registration channel, try to register with the updated message
            if (context.Channel.Id == _botConfig.RegistrationChannelId)
            {
                await _registrationHandler.TryRegisterNewUser(context);
                return;
            }
        }

        public async Task HandleMessageSentAsync(SocketMessage message)
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

            await _messageLogger.LogMessageSent(msg, context.Guild);

            if (context.Channel.Id == _botConfig.RegistrationChannelId)
            {
                await _registrationHandler.TryRegisterNewUser(context);
                return;
            }

            await _commandHandler.HandleCommand(msg, context);
        }
    }
}