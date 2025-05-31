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
    public class GiftTransactionRepository : BaseRepository<GiftTransaction>
    {
        private readonly DatabaseContext _db;
        public GiftTransactionRepository(DatabaseContext db) : base(db) => _db = db;
    }
}
