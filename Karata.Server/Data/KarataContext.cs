using Microsoft.EntityFrameworkCore;
using Karata.Server.Models;

namespace Karata.Server.Data
{
    public class KarataContext : DbContext
    {
        public KarataContext(DbContextOptions<KarataContext> options) : base(options)
        {
        }
        public DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        }
    }
}
