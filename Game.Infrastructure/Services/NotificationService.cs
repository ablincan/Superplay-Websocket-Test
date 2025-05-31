using System.Net.WebSockets;
using System.Text.Json;
using Serilog;

namespace Game.Infrastructure.Services
{
    public class NotificationService
    {
        private readonly ConnectionManager _connectionManager;
        private readonly ILogger _logger;

        public NotificationService(ConnectionManager connectionManager,
                                 ILogger logger)
        {
            _connectionManager = connectionManager;
            _logger = logger;
        }

        public async Task NotifyAsync(Guid playerId, WebSocket ws, object payload)
        {
            if (ws is null || ws.State != WebSocketState.Open)
            {
                _logger.Warning("Cannot notify {PlayerId}, socket not connected", playerId);
                return;
            }

            var resp = new SocketResponse("GiftNotification", payload);
            var bytes = JsonSerializer.SerializeToUtf8Bytes(resp);
            try
            {
                await ws.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error sending notification to {PlayerId}", playerId);
            }
        }
    }
}
