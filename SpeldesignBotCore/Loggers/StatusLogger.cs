using System;
using System.Threading.Tasks;
using Discord;

namespace SpeldesignBotCore.Loggers
{
    public class StatusLogger
    {
        public Task LogToConsole(LogMessage message)
        {
            Console.WriteLine($"{DateTime.Now}: {message.Message}");
            return Task.CompletedTask;
        }

        public void LogToConsole(string message)
        {
            Console.WriteLine($"{DateTime.Now}: {message}");
        }
    }
}
