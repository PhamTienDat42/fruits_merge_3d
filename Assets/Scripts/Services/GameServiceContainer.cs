using System;
using System.Collections.Generic;

namespace Services
{
    public class GameServiceContainer : IServiceProvider
    {
        private readonly Dictionary<Type, object> container = new();

        public void AddService(Type iServiceType, object service)
        {
            if (iServiceType == null || service == null)
            {
                Logger.Error("Type null or service null.");
                return;
            }
            if (container.ContainsKey(iServiceType))
            {
                Logger.Error($"Object with type {iServiceType} existed in Container");
                return;
            }
            container.Add(iServiceType, service);
        }

        public void AddService<T>(T provider) where T : class
        {
            if (provider == null)
            {
                Logger.Error("Service null.");
                return;
            }
            AddService(typeof(T), provider);
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == null)
            {
                Logger.Error("Type null.");
                return null;
            }
            if (!container.ContainsKey(serviceType))
            {
                Logger.Error($"Type {serviceType} does't exited in container.");
                return null;
            }
            return container[serviceType];
        }

        public T GetService<T>() where T : class
        {
            if (typeof(T) == null)
            {
                Logger.Error("Type null.");
                return null;
            }
            if (!container.ContainsKey(typeof(T)))
            {
                Logger.Error($"Type {typeof(T)} does't exited in container.");
                return null;
            }
            return (T)container[typeof(T)];
        }
    }
}