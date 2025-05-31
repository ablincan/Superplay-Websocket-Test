
using Game.Core.Interfaces;
using Game.Infrastructure.Database.Models;
using Game.Infrastructure.Database.Repositories;
using Serilog;

namespace Game.Infrastructure.Handlers
{
    public record LoginRequest(string DeviceId);
    public record ResourceStatDto(Guid ResourceTypeId, string ResourceName, int Total);
    public record LoginResponse(bool Success, Guid PlayerId, List<ResourceStatDto> Stats);

    public class LoginHandler : IMessageHandler<Guid?, LoginRequest, LoginResponse>
    {
        private readonly ILogger _logger;
        private readonly PlayersRepository _playerRepository;
        private readonly ResourceBalanceRepository _resourceBalanceRepository;
        private readonly ResourceTypeRepository _resourceTypeRepository;

        public LoginHandler(
            PlayersRepository playerRepository,
            ResourceBalanceRepository resourceBalanceRepository,
            ResourceTypeRepository resourceTypeRepository,
            ILogger logger)
        {
            _logger = logger;
            _playerRepository = playerRepository;
            _resourceBalanceRepository = resourceBalanceRepository;
            _resourceTypeRepository = resourceTypeRepository;
        }

        public async Task<LoginResponse> HandleAsync(Guid? playerId, LoginRequest msg, CancellationToken ct)
        {
            var player = await _playerRepository.GetPlayerByDeviceId(msg.DeviceId);

            if (player is null)
            {
                player = _playerRepository.Save(new Player { DeviceId = msg.DeviceId });

                var resources = _resourceTypeRepository.GetAll();
                foreach (var resource in resources) 
                    _resourceBalanceRepository.Save(new ResourceBalance { PlayerId = player.Id, ResourceTypeId = resource.Id, Total = 0 });
            }

            _logger.Information("Player {PlayerId} logged in successfully. {message}", playerId, msg);

            return new LoginResponse(true, player.Id, await _resourceBalanceRepository.GetPlayersStats(player.Id));
        }
    }
}
