using Discord.WebSocket;
using SpeldesignBotCore.Discord;
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
            _container.RegisterSingleton<IStatusLogger, ConsoleStatusLogger>();
            _container.RegisterSingleton<IMessageLogger, DiscordMessageActualLogger>();
            _container.RegisterType<DiscordSocketConfig>(new InjectionFactory(i => SocketConfig.GetDefault())); // Return default config when asking for a DiscordSocketConfig
            _container.RegisterSingleton<DiscordSocketClient>(new InjectionConstructor(typeof(DiscordSocketConfig))); // Make DiscordSocketClient use the constructor with DiscordSocketConfig
            _container.RegisterSingleton<Connection>();
        }

        public static T Resolve<T>()
        {
            return (T) Container.Resolve(typeof(T), string.Empty, new CompositeResolverOverride());
        }
    }
}
