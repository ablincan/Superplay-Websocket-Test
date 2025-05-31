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
    public class ResourceTypeRepository : BaseRepository<ResourceType>
    {
        private readonly DatabaseContext _db;
        public ResourceTypeRepository(DatabaseContext db) : base(db) => _db = db;
    }
}
