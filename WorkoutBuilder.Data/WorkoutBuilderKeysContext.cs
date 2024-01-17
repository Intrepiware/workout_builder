using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WorkoutBuilder.Data
{
    public class WorkoutBuilderKeysContext : DbContext, IDataProtectionKeyContext
    {
        // A recommended constructor overload when using EF Core 
        // with dependency injection.
        public WorkoutBuilderKeysContext(DbContextOptions<WorkoutBuilderKeysContext> options)
            : base(options) { }

        // This maps to the table that stores keys.
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Configure default schema
            modelBuilder.HasDefaultSchema("workouts");
        }
    }
}
