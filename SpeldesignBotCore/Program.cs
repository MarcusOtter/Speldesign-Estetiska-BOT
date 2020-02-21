using System.Threading.Tasks;
using SpeldesignBotCore.Entities;

namespace SpeldesignBotCore
{
    internal class Program
    {
        private static async Task Main()
        {
            Unity.RegisterTypes();

            await Unity.Resolve<DiscordCommandHandler>().InstallCommands();
            await Unity.Resolve<Connection>().ConnectAsync(Unity.Resolve<BotConfiguration>());
        }
    }
}
