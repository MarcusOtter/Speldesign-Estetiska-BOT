namespace SpeldesignBotCore.Entities
{
    public class ContestSubmission
    {
        public readonly ulong ChannelId;
        public readonly ulong MessageId;

        private ulong[] _votingUsers;

        public ContestSubmission(ulong channelId, ulong messageId)
        {
            ChannelId = channelId;
            MessageId = messageId;
        }
    }
}
