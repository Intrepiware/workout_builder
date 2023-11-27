using Microsoft.EntityFrameworkCore;

namespace WorkoutBuilder.Data
{
    public class WorkoutBuilderContext : DbContext
    {
        public WorkoutBuilderContext(DbContextOptions<WorkoutBuilderContext> options) : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("workouts");
            modelBuilder.Entity<Exercise>()
                .HasOne(x => x.Focus);
        }

        public DbSet<Focus> Focuses { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Timing> Timings { get; set; }
    }
}
