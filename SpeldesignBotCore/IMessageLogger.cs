namespace SpeldesignBotCore
{
    public interface IMessageLogger
    {
        void LogMsgSent(string author, string message, string channelName);

        void LogMsgDeleted(string author, string message, string channelName);

        void LogMsgEdited(string author, string newMessage, string channelName);
    }
}
