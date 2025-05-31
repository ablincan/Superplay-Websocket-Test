using Game.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Infrastructure.Services
{
    public class HandlerRegistry
    {
        private readonly Dictionary<string, HandlerDescriptor> _map;

        public HandlerRegistry()
        {
            var handlerTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); }
                    catch { return Array.Empty<Type>(); }
                })
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .Select(t => new
                {
                    Impl = t,
                    Iface = t.GetInterfaces()
                             .FirstOrDefault(i =>
                                 i.IsGenericType &&
                                 i.GetGenericTypeDefinition() == typeof(IMessageHandler<,,>))
                })
                .Where(x => x.Iface != null)
                .ToList();

            _map = handlerTypes
                .ToDictionary(
                    x =>
                    {
                        var reqType = x.Iface!.GetGenericArguments()[1];
                        return reqType.Name;
                    },
                    x => new HandlerDescriptor
                    {
                        PlayerId = x.Iface!.GetGenericArguments()[0],
                        RequestType = x.Iface!.GetGenericArguments()[1],
                        ResponseType = x.Iface!.GetGenericArguments()[2],
                        HandlerInterface = x.Iface!
                    },
                    StringComparer.OrdinalIgnoreCase
                );
        }

        public bool TryGet(string messageType, out HandlerDescriptor desc)
            => _map.TryGetValue(messageType, out desc);
    }

    public class HandlerDescriptor
    {
        public Type? PlayerId { get; set; }
        public Type? RequestType { get; init; }
        public Type? ResponseType { get; init; }
        public Type? HandlerInterface { get; init; }
    }
}
