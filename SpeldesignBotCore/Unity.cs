using Discord.Commands;
using Discord.WebSocket;
using SpeldesignBotCore.Entities;
using SpeldesignBotCore.Loggers;
using SpeldesignBotCore.Modules.Giveaways;
using SpeldesignBotCore.Modules.Minecraft;
using SpeldesignBotCore.Registration;
using SpeldesignBotCore.Storage;
using SpeldesignBotCore.Storage.Implementations;
using Unity;
using Unity.Injection;
using Unity.Resolution;

namespace SpeldesignBotCore
{
    public static class Unity
    {
        private static UnityContainer _container;

        public static UnityContainer Container
        {
            get
            {
                if (_container == null) RegisterTypes();
                return _container;
            }
        }

        public static void RegisterTypes()
        {
            _container = new UnityContainer();

            _container.RegisterSingleton<IDataStorage, JsonStorage>();
            _container.RegisterSingleton<BotConfiguration>(new InjectionConstructor(typeof(IDataStorage)));

            _container.RegisterType<DiscordSocketConfig>(new InjectionFactory(i => SocketConfig.GetDefault())); // Return default config when asking for a DiscordSocketConfig
            _container.RegisterSingleton<DiscordSocketClient>(new InjectionConstructor(typeof(DiscordSocketConfig))); // Make DiscordSocketClient use the constructor with DiscordSocketConfig

            _container.RegisterSingleton<StatusLogger>();

            _container.RegisterSingleton<CommandService>();
            _container.RegisterSingleton<DiscordCommandHandler>();
            _container.RegisterSingleton<DiscordMessageLogger>();

            _container.RegisterSingleton<MinecraftServerDataProvider>();
            _container.RegisterSingleton<RegistrationHandler>();
            _container.RegisterSingleton<GiveawayRepo>();

            _container.RegisterSingleton<Connection>();

            Resolve<StatusLogger>().LogToConsole("Registered Unity types");
        }

        public static T Resolve<T>()
        {
            return (T) Container.Resolve(typeof(T), string.Empty, new CompositeResolverOverride());
        }
    }
}
