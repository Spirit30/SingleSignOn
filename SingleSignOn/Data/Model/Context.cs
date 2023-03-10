using Microsoft.EntityFrameworkCore;

namespace SingleSignOn.Data.Model
{
    public class Context : DbContext
    {
        public DbSet<User> Users { get; set; }
        public Context()
        {
            Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=Data.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                //Make this properties unique in database.
                .HasIndex(u => new { u.Email, u.DisplayName, u.AccessToken, u.BearerToken })
                .IsUnique();
        }
    }
}