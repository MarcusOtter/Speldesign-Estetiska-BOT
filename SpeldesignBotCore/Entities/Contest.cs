using SpeldesignBotCore.Contests;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpeldesignBotCore.Entities
{
    public class Contest
    {
        public readonly ulong SubmissionChannelId;

        public string Title { get; set; }
        public ContestState State { get; set; }
        public DateTime EndDateUtc { get; set; }

        private readonly List<ContestSubmission> _submissions = new List<ContestSubmission>();

        public Contest(string title, ulong submissionChannelId)
        {
            Title = title;
            SubmissionChannelId = submissionChannelId;
            State = ContestState.TakingSubmissions;
        }

        public int GetAmountOfSubmissions() => _submissions.Count;
        public int GetAmountOfVoters() => _submissions.Sum(x => x.GetAllVotersIds().Count);

        public void Close()
        {
            if (State is ContestState.VotingPeriod) { return; }

            State = ContestState.VotingPeriod;
            EndDateUtc = DateTime.UtcNow;
        }

        public bool UserHasVoted(ulong userId)
        {
            foreach(var submission in _submissions)
            {
                foreach(var voterId in submission.GetAllVotersIds())
                {
                    if (userId == voterId) { return true; }
                }
            }

            return false;
        }
    }
}
