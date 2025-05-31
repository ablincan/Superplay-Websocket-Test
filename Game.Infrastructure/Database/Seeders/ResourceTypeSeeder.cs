using Game.Infrastructure.Database.Context;
using Game.Infrastructure.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Infrastructure.Database.Seeders
{
    public class ResourceTypeSeeder
    {
        private readonly DatabaseContext _db;

        public ResourceTypeSeeder(DatabaseContext db)
        {
            _db = db;
        }

        public async Task SeedAsync()
        {
            if (!await _db.ResourceTypes!.AnyAsync())
            {
                _db.ResourceTypes!.AddRange(
                    new ResourceType { Name = "coins" },
                    new ResourceType { Name = "rolls" }
                );
                await _db.SaveChangesAsync();
            }
        }
    }
}
