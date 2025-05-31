using Game.Infrastructure.Database.Context;
using Game.Infrastructure.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Infrastructure.Database.Repositories
{
    public class PlayersRepository : BaseRepository<Player>
    {
        private readonly DatabaseContext _db;
        public PlayersRepository(DatabaseContext db) : base(db) => _db = db;

        public async Task<Player?> GetPlayerByDeviceId(string? deviceId)
        {
            if (deviceId is null)
                return null;

            Player? player = await _db.Players.FirstOrDefaultAsync(p => p.DeviceId == deviceId && p.IsDeleted == false);

            if (player is null)
                return null;
            return player;
        }
    }
}
