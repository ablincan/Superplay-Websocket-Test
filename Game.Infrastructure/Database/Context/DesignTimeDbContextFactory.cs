using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Infrastructure.Database.Context
{
    public class DesignTimeDbContextFactory
        : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            var dbPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Database",
                "superplay.db");

            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseSqlite(
                    $"Data Source={dbPath}",
                    x => x.MigrationsAssembly("Game.Infrastructure")
                )
                .Options;

            return new DatabaseContext(options);
        }
    }
}
