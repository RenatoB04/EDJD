using System;
using System.Collections.Generic;
namespace InfimaGames.LowPolyShooterPack
{
    public class ServiceLocator
    {
        private readonly Dictionary<string, IGameService> services = new Dictionary<string, IGameService>();
        public static ServiceLocator Current { get; private set; }
        public static void Initialize() { Current = new ServiceLocator(); }
        public T Get<T>() where T : IGameService
        {
            string key = typeof(T).Name;
            if (!services.ContainsKey(key))
            {
                Log.kill($"{key} not registered with {GetType().Name}");
                throw new InvalidOperationException();
            }
            return (T)services[key];
        }
        public void Register<T>(T service) where T : IGameService
        {
            string key = typeof(T).Name;
            if (services.ContainsKey(key))
            {
                Log.kill($"Attempted to register service of type {key} which is already registered with the {GetType().Name}.");
                return;
            }
            services.Add(key, service);
        }
        public void Unregister<T>() where T : IGameService
        {
            string key = typeof(T).Name;
            if (!services.ContainsKey(key))
            {
                Log.kill($"Attempted to unregister service of type {key} which is not registered with the {GetType().Name}.");
                return;
            }
            services.Remove(key);
        }
    }
}