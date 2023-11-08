using Microsoft.EntityFrameworkCore;

namespace WorkoutBuilder.Data
{
    public class WorkoutBuilderContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Exercise>()
                .HasOne(x => x.Focus);
        }

        public DbSet<Focus> Focuses { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Timing> Timings { get; set; }
    }
}
