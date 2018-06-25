using SpeldesignBotCore.Discord;
using SpeldesignBotCore.Storage;
using SpeldesignBotCore.Storage.Implementations;
using Unity;
using Unity.Lifetime;
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
            _container.RegisterType<IDataStorage, InMemoryStorage>(new ContainerControlledLifetimeManager());
            _container.RegisterType<Connection>(new ContainerControlledLifetimeManager());
        }

        public static T Resolve<T>()
        {
            return (T) Container.Resolve(typeof(T), string.Empty, new CompositeResolverOverride());
        }
    }
}
