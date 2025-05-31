using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Game.Infrastructure.Database.Models
{
    public record ResourceType : BaseEntity
    {
        public string? Name { get; set; }

        public ResourceType()
        {
            Created = System.DateTime.UtcNow;
            Updated = System.DateTime.UtcNow;
            IsDeleted = false;
        }
    }
}
