using System.Threading.Tasks;
using SpeldesignBotCore.Entities;

namespace SpeldesignBotCore
{
    internal class Program
    {
        private static async Task Main()
        {
            Unity.RegisterTypes();

            var commandHandler = Unity.Resolve<DiscordCommandHandler>();
            await commandHandler.InstallCommands();

            var connection = Unity.Resolve<Connection>();
            await connection.ConnectAsync(Unity.Resolve<BotConfiguration>());
        }
    }
}
