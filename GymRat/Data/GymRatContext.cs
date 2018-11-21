using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GymRat.Models
{
    public class GymRatContext : DbContext
    {
        public GymRatContext (DbContextOptions<GymRatContext> options)
            : base(options)
        {
        }

        public DbSet<GymRat.Models.GymItem> GymItem { get; set; }
    }
}
