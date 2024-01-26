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
            modelBuilder.Entity<Exercise>()
                .HasOne(x => x.FocusPart);

            modelBuilder.Entity<User>(x => x.HasIndex(t => t.EmailAddress).IsUnique());

            modelBuilder.Entity<UserPasswordResetRequest>(x => x.HasIndex(t => t.PublicId).IsUnique());
            modelBuilder.Entity<UserPasswordResetRequest>()
                .HasOne(x => x.User);

            modelBuilder.Entity<Workout>()
                .HasOne(x => x.User);
            modelBuilder.Entity<Workout>(x => x.HasIndex(t => t.PublicId).IsUnique());


        }

        public DbSet<Focus> Focuses { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Timing> Timings { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Workout> User { get; set; }
    }
}
