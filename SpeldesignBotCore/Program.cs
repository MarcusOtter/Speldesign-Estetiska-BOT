using System;
using System.Threading.Tasks;
using SpeldesignBotCore.Discord;
using SpeldesignBotCore.Discord.Entities;
using SpeldesignBotCore.Storage;

namespace SpeldesignBotCore
{
    internal class Program
    {
        private static async Task Main()
        {
            Unity.RegisterTypes();
            Console.WriteLine($"{DateTime.Now}: Registered Unity types");

            var storage = Unity.Resolve<IDataStorage>();

            var connection = Unity.Resolve<Connection>();
            await connection.ConnectAsync(new BotConfiguration
            {
                Token = storage.RestoreObject<string>("Config/BotToken")
            });
        }
    }
}
