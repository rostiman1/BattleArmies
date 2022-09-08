using BattleArmies.Shared;
using Microsoft.EntityFrameworkCore;

namespace BattleArmies.Server.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Battle>()
                .HasOne(x => x.Attacker)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Battle>()
                .HasOne(x => x.Opponent)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Battle>()
                .HasOne(x => x.Winner)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

        }
        public DbSet<Unit> Units { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet <UserUnit> UserUnits { get; set; }
        public DbSet <Battle> Battles { get; set; }
    }
}
