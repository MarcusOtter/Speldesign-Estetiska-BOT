using Discord;
using Discord.Commands;
using SpeldesignBotCore.Entities;
using SpeldesignBotCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpeldesignBotCore.Contests
{
    public class ContestHandler
    {
        private const string _contestsStoragePath = "Info/Contests";
        private readonly string[] _validEmojis = new string[] { "🐶", "🐱", "🐭", "🐹", "🐰", "🦊", "🐻", "🐼", "🐨", "🐯", "🦁", "🐮", "🐷", "🐸", "🐥", "🦉", "🐧", "🐔", "🐵", "🦇", "🐺", "🐗", "🐙", "🦌", "🦔", "🦝", "🦍", "🦆", "🦅", "🐴", "🐝", "🐛", "🦋", "🐌", "🐞", "🐢", "🐍", "🦀", "🐠", "🐬", "🦓", "🐘", "🐪", "🐑", "🐩", "🦥", "🦦", "🐿️", "🦩", "🦢", "🦖" };
        private readonly IDataStorage _dataStorage;

        public ContestHandler(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
        }

        public List<Contest> GetStoredContests()
        {
            if (!_dataStorage.HasObject(_contestsStoragePath))
            {
                _dataStorage.StoreObject(new List<Contest>(), _contestsStoragePath);
            }

            return _dataStorage.RestoreObject<List<Contest>>(_contestsStoragePath);
        }

        public void SaveNewContest(Contest newContest)
        {
            var contests = GetStoredContests();
            contests.Add(newContest);
            _dataStorage.StoreObject(contests, _contestsStoragePath);
        }

        private void UpdateContest(Contest contest)
        {
            var contests = GetStoredContests();

            if (!contests.Any(x => x.Title.ToLower() == contest.Title.ToLower()))
            {
                contests.Add(contest);
                _dataStorage.StoreObject(contests, _contestsStoragePath);
                return;
            }

            var index = contests.FindIndex(x => x.Title.ToLower() == contest.Title.ToLower());
            contests.RemoveAt(index);
            contests.Insert(index, contest);

            _dataStorage.StoreObject(contests, _contestsStoragePath);
        }

        public void DeleteContest(Contest contest)
        {
            var contests = GetStoredContests();
            if (!contests.Any(x => x.Title.ToLower() == contest.Title.ToLower())) { return; }

            var index = contests.FindIndex(x => x.Title.ToLower() == contest.Title.ToLower());
            contests.RemoveAt(index);

            _dataStorage.StoreObject(contests, _contestsStoragePath);
        }
        
        // Returns the updated contest without the submission
        public Contest DeleteContestSubmission(ContestSubmission submission, Contest contest)
        {
            var submissionIndex = contest.Submissions.FindIndex(x => x.AuthorId == submission.AuthorId);
            contest.Submissions.RemoveAt(submissionIndex);

            UpdateContest(contest);

            return contest;
        }

        public void StartVotingPeriod(Contest contest, ulong votingMessageId)
        {
            if (contest.State is ContestState.VotingPeriod) { return; }

            contest.State = ContestState.VotingPeriod;
            contest.VotingMessageId = votingMessageId;

            UpdateContest(contest);
        }
        
        public void RandomizeContestSubmissionEmojis(Contest contest)
        {
            var emojis = GetUniqueRandomEmojis(contest.Submissions.Count);

            for (int i = 0; i < contest.Submissions.Count; i++)
            {
                contest.Submissions[i].EmojiRawUnicode = emojis[i];
            }
        }

        private string[] GetUniqueRandomEmojis(int amount)
        {
            var random = new Random();
            var output = new string[amount];

            for (int i = 0; i < output.Length; i++)
            {
                string newEmoji;
                do
                {
                    newEmoji = _validEmojis[random.Next(0, _validEmojis.Length)];
                }
                while (output.Contains(newEmoji));

                output[i] = newEmoji;
            }

            return output;
        }

        public async Task TryAddNewContestSubmission(SocketCommandContext context)
        {
            var activeContestsInThisChannel = GetContestsInState(ContestState.TakingSubmissions).
                Where(x => x.SubmissionChannelId == context.Channel.Id);

            foreach(var contest in activeContestsInThisChannel)
            {
                // If the user has already submitted:
                if (contest.Submissions.Select(x => x.AuthorId).Any(x => x == context.User.Id)) 
                {
                    var dmChannel = await context.User.GetOrCreateDMChannelAsync();
                    var errorMessage = $"You have already made a submission to the contest \"{contest.Title}\". Please delete this message: https://discordapp.com/channels/{context.Guild.Id}/{context.Channel.Id}/{context.Message.Id}";

                    // Try sending the error message to the user's DM channel.
                    // If they don't allow DMs from this server, send it in the channel instead.
                    try {  await dmChannel.SendMessageAsync(errorMessage); }
                    catch (Discord.Net.HttpException)  { await context.Channel.SendMessageAsync(errorMessage); }

                    continue;
                }

                var submission = new ContestSubmission(context.Message.Id, context.Message.Author.Id);
                contest.Submissions.Add(submission);

                UpdateContest(contest);

                var embedBuilder = new EmbedBuilder()
                    .WithTitle("Success!")
                    .WithDescription(
                        $"{context.User.Mention} has added a submission to the contest \"{contest.Title}\"\n" +
                        $"Good luck! :four_leaf_clover:")
                    .WithColor(new Color(118, 196, 177))
                    .WithFooter($"Didn't mean to do this? Contact a moderator/teacher on the server.");

                await context.Channel.SendMessageAsync("", embed: embedBuilder.Build());
            }
        }

        public bool MessageIsContestSubmission(ulong messageId)
            => GetStoredContests()
                .Any(x => x.Submissions
                .Select(x => x.MessageId)
                .Contains(messageId));

        // Returns default if none is found
        public ContestSubmission GetContestSubmissionFromUserId(Contest contest, ulong userId)
            => contest.Submissions.FirstOrDefault(x => x.AuthorId == userId);

        public Contest GetContestByTitle(string contestTitle)
        {
            var contests = GetStoredContests();
            return contests.FirstOrDefault(x => x.Title.ToLower() == contestTitle.ToLower());
        }

        public bool ContestTitleExists(string contestTitle)
        {
            var contests = GetStoredContests();
            return contests.Any(x => x.Title.ToLower() == contestTitle.ToLower());
        }

        public bool ChannelIsContestSubmissionChannel(ulong channelId)
        {
            var contests = GetContestsInState(ContestState.TakingSubmissions);
            if (contests is null || contests.Length is 0) { return false; }
            return contests.Any(x => x.SubmissionChannelId == channelId);
        }

        public Contest[] GetContestsInState(params ContestState[] states)
        {
            var contests = GetStoredContests();
            return contests.Where(x => states.Contains(x.State)).ToArray();
        }

        public string[] GetStoredContestNames()
            => GetStoredContests().Select(x => x.Title).ToArray();
    }
}
