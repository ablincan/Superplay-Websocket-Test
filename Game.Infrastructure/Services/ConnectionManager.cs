using Game.Infrastructure.Database.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Game.Infrastructure.Services
{
    public class ConnectionManager
    {
        private readonly ConcurrentDictionary<string, WebSocket> _connections
            = new();

        public bool TryRegister(string deviceId, WebSocket ws)
        {
            if (_connections.TryGetValue(deviceId, out var existingWs))
            {
                if (ReferenceEquals(existingWs, ws))
                    return true;

                return false;
            }

            return _connections.TryAdd(deviceId, ws);
        }

        public WebSocket? GetSocket(string deviceId)
        {
            if (_connections.TryGetValue(deviceId, out var socket))
                return socket;
            return null;
        }

        public string GetDeviceBySocket(WebSocket ws)
        {
            var pair = _connections.FirstOrDefault(kvp => kvp.Value == ws);
            return pair.Key;
        }

        public bool UnregisterBySocket(WebSocket ws)
        {
            var pair = _connections.FirstOrDefault(kvp => kvp.Value == ws);
            if (pair.Value != null)
                return _connections.TryRemove(pair.Key, out _);
            return false;
        }
    }
}
