using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Infrastructure.Database.Models
{
    public record BaseEntity
    {
        public BaseEntity()
        {
            Created = DateTime.UtcNow;
            Updated = DateTime.UtcNow;
            IsDeleted = false;
        }

        public Guid Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public bool IsDeleted { get; set; }
    }
}
