using bowling_backend_persistence.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace bowling_backend_persistence;

public class BowlingDataContext : DbContext
{
    private string _connectionString;

    public BowlingDataContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("(Default)") ?? throw new ArgumentException();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Frame>()
            .HasKey(frame => frame.Id);

        modelBuilder.Entity<Frame>()
            .Property(frame => frame.Rolls)
            .HasConversion(
                rolls => string.Join(",", rolls.Select(r => r.ToString())),
                rolls => rolls.Split(',', StringSplitOptions.RemoveEmptyEntries)
                              .Select(int.Parse)
                              .ToArray());

        modelBuilder.Entity<BowlingGame>()
            .HasKey(game => game.Id);

        modelBuilder.Entity<BowlingGame>()
            .HasIndex(game => new{ game.Id, game.UserId });

        modelBuilder.Entity<BowlingGame>()
            .Property(game => game.PlayerNames)
            .HasConversion(
                players => string.Join(",", players),
                players => players.Split(',', StringSplitOptions.RemoveEmptyEntries));
    }
}