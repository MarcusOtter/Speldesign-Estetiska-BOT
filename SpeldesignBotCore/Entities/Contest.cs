using SpeldesignBotCore.Contests;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpeldesignBotCore.Entities
{
    public class Contest
    {
        public ulong VotingMessageId { get; set; }
        public ulong SubmissionChannelId { get; set; }
        public string Title { get; set; }
        public ContestState State { get; set; }
        public DateTime EndDateUtc { get; set; }

        public List<ContestSubmission> Submissions = new List<ContestSubmission>();

        public Contest(string title, ulong submissionChannelId)
        {
            Title = title;
            SubmissionChannelId = submissionChannelId;
            State = ContestState.TakingSubmissions;
        }

        public int GetAmountOfVoters() => Submissions.Sum(x => x.VotingUsers.Count);

        public bool UserHasVoted(ulong userId)
        {
            foreach(var submission in Submissions)
            {
                foreach(var voterId in submission.VotingUsers)
                {
                    if (userId == voterId) { return true; }
                }
            }

            return false;
        }
    }
}
