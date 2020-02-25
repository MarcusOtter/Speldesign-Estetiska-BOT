using System.Collections.Generic;

namespace SpeldesignBotCore.Entities
{
    public class ContestSubmission
    {
        public readonly ulong MessageId;
        public readonly ulong AuthorId;
        // int placement

        private readonly List<ulong> _votingUsersIds = new List<ulong>();

        public ContestSubmission(ulong messageId, ulong authorId)
        {
            MessageId = messageId;
            AuthorId = authorId;
        }

        public List<ulong> GetAllVotersIds() => _votingUsersIds;
    }
}
