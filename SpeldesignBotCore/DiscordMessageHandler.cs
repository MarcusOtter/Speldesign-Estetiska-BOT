using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SpeldesignBotCore.Contests;
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
        private readonly ContestHandler _contestHandler;

        public DiscordMessageHandler(DiscordSocketClient client, BotConfiguration config,
                                     DiscordMessageLogger logger, DiscordCommandHandler commandHandler,
                                     RegistrationHandler registrationHandler, ContestHandler contestHandler)
        {
            _client = client;
            _botConfig = config;
            _messageLogger = logger;
            _commandHandler = commandHandler;
            _registrationHandler = registrationHandler;
            _contestHandler = contestHandler;
        }

        public async Task HandleReactionAddedAsync(Cacheable<IUserMessage, ulong> before, ISocketMessageChannel channel, SocketReaction reaction)
        {
            var contestsInVotingPeriod = _contestHandler.GetContestsInState(ContestState.VotingPeriod);
            if (contestsInVotingPeriod.Length is 0) { return; }

            foreach (var contest in contestsInVotingPeriod)
            {
                if (contest.SubmissionChannelId != channel.Id) { continue; }
                // check message id
                // check if user has already voted in this contest (if so, change votes)
                // probably have contest handler deal with all of this stuff anyways
            }


        }

        public async Task HandleMessageDeletedAsync(Cacheable<IMessage, ulong> deletedMessage, ISocketMessageChannel channel)
        {
            if (!_contestHandler.MessageIsContestSubmission(deletedMessage.Id)) { return; }

            // remove the submission if it's in voting or taking submission stage
        }

        public async Task HandleMessageEditedAsync(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
        {
            var messageBeforeEdit = before.Value as SocketUserMessage;
            var messageAfterEdit = after as SocketUserMessage;

            if (messageBeforeEdit is null || messageAfterEdit is null) { return; }

            // If the new message is not edited, this edit event was likely caused by a delayed link embed
            if (messageAfterEdit.EditedTimestamp is null) { return; }

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

            // If this is a DM no logging, registration, or contest submission
            // should happen, so just handle command and return.
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

            int argPos = 0;
            if (_commandHandler.MessageContainsCommandInvocation(msg, context, ref argPos))
            {
                await _commandHandler.HandleCommand(msg, context);
            }
            else if (_contestHandler.ChannelIsContestSubmissionChannel(context.Channel.Id))
            {
                await _contestHandler.TryAddNewContestSubmission(context);
            }
        }
    }
}
