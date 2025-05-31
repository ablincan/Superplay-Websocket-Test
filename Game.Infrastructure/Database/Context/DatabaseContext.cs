using Game.Infrastructure.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Game.Infrastructure.Database.Context
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<ResourceBalance> ResourceBalances { get; set; }
        public DbSet<ResourceType> ResourceTypes { get; set; }
        public DbSet<GiftTransaction> GiftTransactions { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> opts)
            : base(opts)
        { }
    }
}
