using System.Collections.Generic;

namespace SpeldesignBotCore.Entities
{
    public class ContestSubmission
    {
        public ulong MessageId { get; set; }
        public ulong AuthorId { get; set; }
        public string EmojiRawUnicode { get; set; }

        // int placement

        public List<ulong> VotingUsers = new List<ulong>();

        public ContestSubmission(ulong messageId, ulong authorId)
        {
            MessageId = messageId;
            AuthorId = authorId;
        }
    }
}
