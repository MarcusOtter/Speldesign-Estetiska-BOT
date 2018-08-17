using System;

namespace SpeldesignBotCore
{
    public class ConsoleStatusLogger : IStatusLogger
    {
        public void Log(string message)
        {
            Console.WriteLine($"{DateTime.Now}: {message}");
        }
    }
}
