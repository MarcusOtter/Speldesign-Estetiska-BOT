using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SpeldesignBotCore.Entities;
using SpeldesignBotCore.Loggers;
using SpeldesignBotCore.Storage;
using System.Linq;
using System.Threading.Tasks;

namespace SpeldesignBotCore.Contests
{
    public class ContestHandler
    {
        private readonly bool _contestIsInProgress;

        private readonly IDataStorage _dataStorage;
        private readonly ContestSubmission[] _submissions;

        public ContestHandler(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
        }

        private bool IsContestSubmission(ulong messageId) => _submissions.Any(x => x.MessageId == messageId);

        public void AddReactionAction(ulong messageId, Emote emote, ActionOnReactionMessage.ReactionAction action)
        {
            if (IsContestSubmission(messageId))
            {
                _actionOnReactionMessages[messageId].AddAction(emote, action);
            }
            else
            {
                var actionOnReactionMessage = new ActionOnReactionMessage(messageId, (emote, action));
                _actionOnReactionMessages.Add(messageId, actionOnReactionMessage);
            }
        }

        public void RemoveReactionAction(ulong messageId, Emote emote)
        {
            if (!IsContestSubmission(messageId)) { throw new System.ArgumentException("This message does not have any reaction actions"); }

            var actionOnReactionMessage = _actionOnReactionMessages[messageId];

            if (!actionOnReactionMessage.HasActionFor(emote)) { throw new System.ArgumentException("This message does not have an action for the given emoji"); }

            actionOnReactionMessage.RemoveAction(emote);
        }

        public async Task HandleReactionAdded(Cacheable<IUserMessage, ulong> before, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.User.Value.IsBot) { return; }

            var emote = (Emote) reaction.Emote;
            var message = (SocketUserMessage) await channel.GetMessageAsync(reaction.MessageId);

            if (!IsContestSubmission(message.Id)) { return; }

            var actionReactionMessage = _actionOnReactionMessages[message.Id];

            if (!actionReactionMessage.HasActionFor(emote)) { return; }

            var context = new SocketCommandContext(_dataStorage, message);
            await actionReactionMessage.RunActionAsync(emote, context);

            Unity.Resolve<StatusLogger>().LogToConsole($"Reaction with name \"{emote.Id}\" by {reaction.User.Value.Username} on message with ID {reaction.MessageId}");
        }
    }
}
