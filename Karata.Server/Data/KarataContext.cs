using Microsoft.EntityFrameworkCore;
using Karata.Shared.Models;
using Karata.Server.Models;

namespace Karata.Server.Data
{
    public class KarataContext : DbContext
    {
        public KarataContext(DbContextOptions<KarataContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

            modelBuilder.Entity<RefreshToken>().HasIndex(r => r.TokenString).IsUnique();
            modelBuilder.Entity<RefreshToken>().HasIndex(r => r.Email);
        }
    }
}
