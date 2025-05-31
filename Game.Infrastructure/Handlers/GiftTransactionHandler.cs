using Game.Core.Interfaces;
using Game.Infrastructure.Database.Models;
using Game.Infrastructure.Database.Repositories;
using Game.Infrastructure.Services;
using System.Reflection.Metadata;
using Serilog;

namespace Game.Infrastructure.Handlers
{
    public record SendGiftRequest(Guid FriendPlayerId, Guid ResourceTypeId, int ResourceValue);
    public record SendGiftResponse(bool Success, Guid ResourceTypeId, int NewTotal, string Message);

    public class GiftTransactionHandler : IMessageHandler<Guid?, SendGiftRequest, SendGiftResponse>
    {
        private readonly ILogger _logger;

        private readonly ConnectionManager _connectionManager;
        private readonly NotificationService _notificationService;

        private readonly PlayersRepository _playersRepository;
        private readonly ResourceBalanceRepository _resourceBalanceRepository;
        private readonly ResourceTypeRepository _resourceTypeRepository;
        private readonly GiftTransactionRepository _giftTransactionRepository;

        public GiftTransactionHandler(
            ILogger logger,
            ConnectionManager connectionManager,
            NotificationService notificationService,
            PlayersRepository playerRepository,
            ResourceBalanceRepository resourceBalanceRepository,
            ResourceTypeRepository resourceTypeRepository,
            GiftTransactionRepository giftTransactionRepository)
        {
            _logger = logger;
            _connectionManager = connectionManager;
            _notificationService = notificationService;

            _playersRepository = playerRepository;
            _resourceBalanceRepository = resourceBalanceRepository;
            _resourceTypeRepository = resourceTypeRepository;
            _giftTransactionRepository = giftTransactionRepository;
        }

        public async Task<SendGiftResponse> HandleAsync(Guid? playerId, SendGiftRequest msg, CancellationToken ct)
        {
            if(playerId == msg.FriendPlayerId)
                return new SendGiftResponse(false, new Guid(), 0, $"You cannot send resources to yourself!");

            var resourceType = _resourceTypeRepository.GetAsNoTracking(msg.ResourceTypeId)!;
            (bool ok, int sourceTotal, int destinationTotal) obj = await _resourceBalanceRepository.CalculateBalance((Guid)playerId!, msg.FriendPlayerId, msg.ResourceTypeId, msg.ResourceValue);
            if (!obj.ok)
                return new SendGiftResponse(false, msg.ResourceTypeId, obj.destinationTotal, $"You don\'t have enough {resourceType?.Name}");

            _logger.Information("Player {PlayerId} sent resources to a friend. {message}", playerId, msg);

            var friendPlayer = _playersRepository.GetAsNoTracking(msg.FriendPlayerId);
            var destionationSocket = _connectionManager.GetSocket(friendPlayer!.DeviceId!);

            _giftTransactionRepository.Save(new GiftTransaction
            {
                Id = Guid.NewGuid(),
                FromPlayerId = (Guid)playerId!,
                ToPlayerId = msg.FriendPlayerId,
                ResourceTypeId = msg.ResourceTypeId,
                ResourceValue = msg.ResourceValue,
                Timestamp = DateTime.UtcNow
            });

            if (destionationSocket != null)
            {
                await _notificationService.NotifyAsync(
                    msg.FriendPlayerId,
                    destionationSocket,
                    new
                    {
                        Message = $"Congratulations! You received {msg.ResourceValue} {resourceType?.Name} from {playerId}",
                        ResourceTypeId = msg.ResourceTypeId,
                        Total = obj.destinationTotal
                    }
                );
                return new SendGiftResponse(true, msg.ResourceTypeId, obj.sourceTotal, "Gift sent! Player received a notification.");
            }
            else
            {
                return new SendGiftResponse(true, msg.ResourceTypeId, obj.sourceTotal, "The gift was sent, but the player is not online!");
            }
            
        }
    }
}
