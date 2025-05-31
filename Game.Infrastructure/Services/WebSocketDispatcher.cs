using Game.Core.Interfaces;
using Game.Infrastructure.Database.Repositories;
using Game.Infrastructure.Handlers;
using Microsoft.Extensions.DependencyInjection;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Serilog;

namespace Game.Infrastructure.Services
{
    public record SocketMessage(string MessageType, JsonElement Payload);
    public record SocketResponse(string MessageType, object Payload);

    public class WebSocketDispatcher
    {
        private readonly HandlerRegistry _registry;
        private readonly IServiceProvider _services;
        private readonly ILogger _logger;

        private readonly ConnectionManager _connectionManager;
        private readonly PlayersRepository _playersRepository;

        public WebSocketDispatcher(
            HandlerRegistry registry,
            IServiceProvider services,
            ILogger logger,
            ConnectionManager connectionManager,
            PlayersRepository playersRepository)
        {
            _registry = registry;
            _services = services;
            _logger = logger;
            _connectionManager = connectionManager;
            _playersRepository = playersRepository;
        }

        public async Task DispatchAsync(WebSocket ws, CancellationToken ct)
        {
            using var scope = _services.CreateScope();
            var buffer = new byte[4096];

            while (ws.State == WebSocketState.Open && !ct.IsCancellationRequested)
            {
                _logger.Information("Connection opened!");

                var result = await ws.ReceiveAsync(buffer, ct);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    _logger.Information("Connection aborted!");

                    _connectionManager.UnregisterBySocket(ws);
                    break;
                }

                var rawMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var message = JsonSerializer.Deserialize<SocketMessage>(rawMessage);

                if (message is null || !_registry.TryGet(message.MessageType, out var desc))
                {
                    _logger.Warning("Unknown socket connection. {message}" , message);

                    await SendErrorAsync(ws, ct, message?.MessageType ?? "Unknown", "UnknownMessageType");
                    continue;
                }

                if (message.MessageType == nameof(LoginRequest))
                {
                    if (!HandleLogin(message, ws))
                    {
                        _logger.Warning("Device already has an opened connection. {message}", message);

                        await SendErrorAsync(ws, ct, message.MessageType, "Device Already Connected");
                        continue;
                    }
                }

                var deviceId = _connectionManager.GetDeviceBySocket(ws);
                if (message.MessageType != nameof(LoginRequest) && !await IsAuthenticatedAsync(deviceId))
                {
                    _logger.Warning("Handler {Type} used without Authentication", desc.HandlerInterface);

                    await SendErrorAsync(ws, ct, message.MessageType, "Player not authenticated. Please log in first.");
                    continue;
                }

                var handler = scope.ServiceProvider.GetService(desc.HandlerInterface);
                if (handler is null)
                {
                    _logger.Warning("Handler {Type} not registered in DI", desc.HandlerInterface);

                    await SendErrorAsync(ws, ct, message.MessageType, "UnknownMessageType");
                    continue;
                }

                try
                {
                    var request = message.Payload.Deserialize(desc.RequestType!, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    var playerId = (await _playersRepository.GetPlayerByDeviceId(deviceId))?.Id;
                    var resultValue = await InvokeHandlerAsync(handler, desc.HandlerInterface!, desc.RequestType!, playerId, request, ct);

                    var response = new SocketResponse(message.MessageType, resultValue);
                    var responseBytes = JsonSerializer.SerializeToUtf8Bytes(response);
                    await ws.SendAsync(responseBytes, WebSocketMessageType.Text, true, ct);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed handling message {Type}", message.MessageType);

                    await SendErrorAsync(ws, ct, message.MessageType, "HandlerExecutionFailed");
                }
            }
        }

        private bool HandleLogin(SocketMessage message, WebSocket ws)
        {
            var loginReq = message.Payload.Deserialize<LoginRequest>();
            return _connectionManager.TryRegister(loginReq!.DeviceId, ws);
        }

        private async Task<bool> IsAuthenticatedAsync(string? deviceId)
        {
            return deviceId != null && await _playersRepository.GetPlayerByDeviceId(deviceId) != null;
        }

        private async Task SendErrorAsync(WebSocket ws, CancellationToken ct, string messageType, string code)
        {
            var error = new SocketResponse(messageType, new { Success = false, Error = code });
            var outBytes = JsonSerializer.SerializeToUtf8Bytes(error);
            await ws.SendAsync(outBytes, WebSocketMessageType.Text, true, ct);
        }

        private async Task<object> InvokeHandlerAsync(object handler, Type handlerType, Type requestType, Guid? playerId, object request, CancellationToken ct)
        {
            var method = handlerType.GetMethod("HandleAsync", new[] { typeof(Guid?), requestType, typeof(CancellationToken) });
            if (method == null)
                throw new InvalidOperationException("Handler method not found");

            var task = (Task)method.Invoke(handler, new[] { playerId, request, ct })!;
            await task.ConfigureAwait(false);

            return task.GetType().GetProperty("Result")!.GetValue(task)!;
        }
    }
}
