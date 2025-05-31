using Game.Core.Interfaces;
using Game.Infrastructure.Database.Repositories;
using Serilog;

namespace Game.Infrastructure.Handlers
{
    public record UpdateResourceRequest(Guid ResourceTypeId, int Delta);
    public record UpdateResourceResponse(bool Success, Guid ResourceTypeId, int NewTotal);

    public class UpdateResourceHandler : IMessageHandler<Guid?, UpdateResourceRequest, UpdateResourceResponse>
    {
        private readonly ILogger _logger;
        private readonly ResourceBalanceRepository _resourceBalanceRepository;

        public UpdateResourceHandler(
            ResourceBalanceRepository resourceBalanceRepository,
            ILogger logger)
        {
            _logger = logger;
            _resourceBalanceRepository = resourceBalanceRepository;
        }

        public async Task<UpdateResourceResponse> HandleAsync(Guid? playerId, UpdateResourceRequest msg, CancellationToken ct)
        {
            _logger.Information("Player {PlayerId} updated its resources. {message}", playerId, msg);

            var newTotal = await _resourceBalanceRepository.UpdateResourceDelta(playerId, msg.ResourceTypeId, msg.Delta);

            return new UpdateResourceResponse(true, msg.ResourceTypeId, newTotal);
        }
    }
}
