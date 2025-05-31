using Game.Infrastructure.Database.Context;
using Game.Infrastructure.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Infrastructure.Handlers;

namespace Game.Infrastructure.Database.Repositories
{
    public class ResourceBalanceRepository : BaseRepository<ResourceBalance>
    {
        private readonly DatabaseContext _db;
        public ResourceBalanceRepository(DatabaseContext db) : base(db) => _db = db;

        public async Task<List<ResourceStatDto>> GetPlayersStats(Guid playerId)
        {
            return await _db.ResourceBalances
                .Where(r => r.PlayerId == playerId)
                .Select(rt => new ResourceStatDto(
                    rt.ResourceTypeId,
                    _db.ResourceTypes.AsNoTracking().FirstOrDefault(r => r.Id == rt.ResourceTypeId)!.Name!,
                    rt.Total
                ))
                .ToListAsync();
        }

        public async Task<int> UpdateResourceDelta(Guid? playerId, Guid resourceTypeId, int delta)
        {
            var resource = await _db.ResourceBalances
                .FirstOrDefaultAsync(r => r.PlayerId == playerId && r.ResourceTypeId == resourceTypeId);

            if (resource == null)
                return 0;

            resource.Total += delta;
            await _db.SaveChangesAsync();

            return resource.Total;
        }

        public async Task<(bool, int, int)> CalculateBalance(Guid fromPlayer, Guid toPlayer, Guid resourceTypeId, int resourceValue)
        {
            var fromBalance = await _db.ResourceBalances
                .FirstOrDefaultAsync(r => r.PlayerId == fromPlayer && r.ResourceTypeId == resourceTypeId);

            if (fromBalance!.Total - resourceValue < 0)
                return (false, 0, 0);

            var toBalance = await _db.ResourceBalances
                .SingleOrDefaultAsync(r => r.PlayerId == toPlayer && r.ResourceTypeId == resourceTypeId);

            if (toBalance == null)
                return (false, 0, 0);

            fromBalance.Total -= resourceValue;
            _db.Update(fromBalance);

            toBalance.Total += resourceValue;
            _db.Update(toBalance);

            await _db.SaveChangesAsync();
            return (true, fromBalance.Total, toBalance.Total);
        }
    }
}
