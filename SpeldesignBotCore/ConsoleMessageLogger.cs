using System;

namespace SpeldesignBotCore
{
    public class ConsoleMessageLogger : IMessageLogger
    {
        public void LogMsgSent(string username, string message, string channelName)
        {
            Console.WriteLine($"{DateTime.Now}: New message sent by {username} in {channelName}: '{message}'");
        }

        public void LogMsgDeleted(string username, string message, string channelName)
        {
            Console.WriteLine($"{DateTime.Now}: Message deleted by {username} in {channelName}: '{message}'");
        }

        public void LogMsgEdited(string username, string newMessage, string channelName)
        {
            Console.WriteLine($"{DateTime.Now}: Message edited by {username} in {channelName}: '{newMessage}'");
        }
    }
}
