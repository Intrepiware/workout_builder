using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutBuilder.Data
{
    public class WorkoutBuilderContext : DbContext
    {
        public DbSet<Focus> Focuses { get; set; }

    }
}
