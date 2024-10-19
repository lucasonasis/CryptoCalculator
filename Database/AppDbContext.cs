using CryptoCalculator.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<CryptoCurrency> CryptoCurrencies { get; set; }
    public DbSet<PriceData> PriceData { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PriceData>()
            .HasOne(p => p.CryptoCurrency)
            .WithMany()
            .HasForeignKey(p => p.CryptoCurrencyId);

        modelBuilder.Entity<PriceData>()
            .Property(p => p.Price)
            .HasPrecision(18, 8);

        // Seed data
        modelBuilder.Entity<CryptoCurrency>().HasData(
            new CryptoCurrency { Id = 1, Name = "Bitcoin", Symbol = "BTC" },
            new CryptoCurrency { Id = 2, Name = "Ethereum", Symbol = "ETH" },
            new CryptoCurrency { Id = 3, Name = "Tether", Symbol = "USDT" },
            new CryptoCurrency { Id = 4, Name = "BNB", Symbol = "BNB" }
        );
    }
}