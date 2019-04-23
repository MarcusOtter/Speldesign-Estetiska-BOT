using System;
using System.Threading.Tasks;
using Discord;

namespace SpeldesignBotCore.Loggers
{
    public class StatusLogger
    {
        public Task LogToConsole(LogMessage message)
        {
            LogToConsole(message.Message);
            return Task.CompletedTask;
        }

        public void LogToConsole(string message)
        {
            if (message is null) { throw new ArgumentNullException("Can not log null"); }
            if (string.IsNullOrWhiteSpace(message)) { throw new ArgumentException("Can not log empty message"); }

            Console.WriteLine($"{DateTime.Now}: {message}");
        }
    }
}
