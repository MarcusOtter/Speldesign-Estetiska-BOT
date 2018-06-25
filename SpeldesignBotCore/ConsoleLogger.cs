using System;

namespace SpeldesignBotCore
{
    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine($"Message '{message}' was sent at {DateTime.UtcNow} (UTC)");
        }
    }
}
