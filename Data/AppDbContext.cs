using GameApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GameApi.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Character> Characters => Set<Character>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(account => account.Id);
            entity.Property(account => account.Username).HasMaxLength(50).IsRequired();
            entity.Property(account => account.PasswordHash).HasMaxLength(255).IsRequired();
            entity.Property(account => account.CreatedAt).IsRequired();
            entity.HasIndex(account => account.Username).IsUnique();
            entity.HasMany(account => account.Characters)
                .WithOne(character => character.Account)
                .HasForeignKey(character => character.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Character>(entity =>
        {
            entity.HasKey(character => character.Id);
            entity.Property(character => character.Name).HasMaxLength(50).IsRequired();
            entity.Property(character => character.Class).HasMaxLength(30).IsRequired();
            entity.Property(character => character.Level).HasDefaultValue(1);
            entity.Property(character => character.Hp).HasDefaultValue(100);
            entity.Property(character => character.CreatedAt).IsRequired();
        });
    }
}
