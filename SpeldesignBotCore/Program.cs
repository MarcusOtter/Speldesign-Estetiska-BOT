using SpeldesignBotCore.Discord;
using SpeldesignBotCore.Discord.Entities;

namespace SpeldesignBotCore
{
    internal class Program
    {
        private static void Main()
        {
            Unity.RegisterTypes();

            var discordBotConfig = new BotConfiguration
            {
                Token = "",
                SocketConfig = SocketConfig.GetDefault()
            };
        }
    }
}
