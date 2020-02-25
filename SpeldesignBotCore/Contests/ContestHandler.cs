using SpeldesignBotCore.Entities;
using SpeldesignBotCore.Storage;
using System.Collections.Generic;
using System.Linq;

namespace SpeldesignBotCore.Contests
{
    public class ContestHandler
    {
        private const string _contestsStoragePath = "Info/Contests";
        private readonly IDataStorage _dataStorage;

        private bool ContestFileExists => _dataStorage.HasObject(_contestsStoragePath);

        public ContestHandler(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
        }

        public void SaveNewContest(Contest newContest)
        {
            if (!ContestFileExists) 
            {
                _dataStorage.StoreObject(new List<Contest>() { newContest }, _contestsStoragePath);
                return;
            }

            var contests = _dataStorage.RestoreObject<List<Contest>>(_contestsStoragePath);
            contests.Add(newContest);
            _dataStorage.StoreObject(contests, _contestsStoragePath);
        }

        public void CloseContest(Contest contest)
        {
            if (!ContestFileExists) { return; }
            if (!contest.IsActive) { return; }

            var contests = _dataStorage.RestoreObject<List<Contest>>(_contestsStoragePath);

            var index = contests.FindIndex(x => x.Title.ToLower() == contest.Title.ToLower());
            contests.RemoveAt(index);
            contest.Close();
            contests.Add(contest);

            _dataStorage.StoreObject(contests, _contestsStoragePath);
        }
        
        public void DeleteContest(Contest contest)
        {
            if (!ContestFileExists) { return; }

            var contests = _dataStorage.RestoreObject<List<Contest>>(_contestsStoragePath);

            var index = contests.FindIndex(x => x.Title.ToLower() == contest.Title.ToLower());
            contests.RemoveAt(index);

            _dataStorage.StoreObject(contests, _contestsStoragePath);
        }

        public Contest GetContestByTitle(string contestTitle)
        {
            if (!ContestFileExists) { return null; }

            var contests = _dataStorage.RestoreObject<Contest[]>(_contestsStoragePath);
            return contests.FirstOrDefault(x => x.Title.ToLower() == contestTitle.ToLower());
        }

        public bool ContestTitleExists(string contestTitle)
        {
            if (!ContestFileExists) { return false; }

            var contests = _dataStorage.RestoreObject<Contest[]>(_contestsStoragePath);
            return contests.Any(x => x.Title.ToLower() == contestTitle.ToLower());
        }

        public Contest[] GetAllContests()
        {
            if (!ContestFileExists) { return null; }
            return _dataStorage.RestoreObject<Contest[]>(_contestsStoragePath);
        }


        //private bool IsContestSubmission(ulong messageId) => _submissions.Any(x => x.MessageId == messageId);



        //public void AddReactionAction(ulong messageId, Emote emote, ActionOnReactionMessage.ReactionAction action)
        //{
        //    if (IsContestSubmission(messageId))
        //    {
        //        _actionOnReactionMessages[messageId].AddAction(emote, action);
        //    }
        //    else
        //    {
        //        var actionOnReactionMessage = new ActionOnReactionMessage(messageId, (emote, action));
        //        _actionOnReactionMessages.Add(messageId, actionOnReactionMessage);
        //    }
        //}

        //public void RemoveReactionAction(ulong messageId, Emote emote)
        //{
        //    if (!IsContestSubmission(messageId)) { throw new System.ArgumentException("This message does not have any reaction actions"); }

        //    var actionOnReactionMessage = _actionOnReactionMessages[messageId];

        //    if (!actionOnReactionMessage.HasActionFor(emote)) { throw new System.ArgumentException("This message does not have an action for the given emoji"); }

        //    actionOnReactionMessage.RemoveAction(emote);
        //}

        //public async Task HandleReactionAdded(Cacheable<IUserMessage, ulong> before, ISocketMessageChannel channel, SocketReaction reaction)
        //{
        //    if (reaction.User.Value.IsBot) { return; }

        //    var emote = (Emote) reaction.Emote;
        //    var message = (SocketUserMessage) await channel.GetMessageAsync(reaction.MessageId);

        //    if (!IsContestSubmission(message.Id)) { return; }

        //    var actionReactionMessage = _actionOnReactionMessages[message.Id];

        //    if (!actionReactionMessage.HasActionFor(emote)) { return; }

        //    var context = new SocketCommandContext(_dataStorage, message);
        //    await actionReactionMessage.RunActionAsync(emote, context);

        //    Unity.Resolve<StatusLogger>().LogToConsole($"Reaction with name \"{emote.Id}\" by {reaction.User.Value.Username} on message with ID {reaction.MessageId}");
        //}
    }
}
