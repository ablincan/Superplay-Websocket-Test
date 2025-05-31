using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Game.Infrastructure.Database.Models
{
    public record GiftTransaction : BaseEntity
    {
        public Guid FromPlayerId { get; set; }
        public Guid ToPlayerId { get; set; }
        public Guid ResourceTypeId { get; set; }
        public int ResourceValue { get; set; }
        public DateTime Timestamp { get; set; }

        public GiftTransaction()
        {
            Created = System.DateTime.UtcNow;
            Updated = System.DateTime.UtcNow;
            IsDeleted = false;
        }
    }
}
