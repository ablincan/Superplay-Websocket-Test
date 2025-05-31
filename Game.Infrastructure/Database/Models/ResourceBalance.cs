using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Infrastructure.Database.Models
{
    public record ResourceBalance : BaseEntity
    {
        public Guid PlayerId { get; set; }
        public Guid ResourceTypeId { get; set; }
        public int Total { get; set; }

        public ResourceBalance() 
        {
            Created = System.DateTime.UtcNow;
            Updated = System.DateTime.UtcNow;
            IsDeleted = false;
        }
    }
}
