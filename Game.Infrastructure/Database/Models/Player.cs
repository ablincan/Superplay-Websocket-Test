using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Infrastructure.Database.Models
{
    public record Player : BaseEntity
    {
        public string? DeviceId { get; set; }

        public Player()
        {
            Created = System.DateTime.UtcNow;
            Updated = System.DateTime.UtcNow;
            IsDeleted = false;
        }
    }
}
