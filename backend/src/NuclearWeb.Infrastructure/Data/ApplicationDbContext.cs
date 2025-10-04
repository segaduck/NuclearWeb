using Microsoft.EntityFrameworkCore;
using NuclearWeb.Core.Entities;

namespace NuclearWeb.Infrastructure.Data;

/// <summary>
/// 應用程式資料庫上下文
/// Application database context
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets for all entities

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<MeetingRoom> MeetingRooms { get; set; } = null!;
    public DbSet<Reservation> Reservations { get; set; } = null!;
    public DbSet<ContentArticle> ContentArticles { get; set; } = null!;
    public DbSet<MenuItem> MenuItems { get; set; } = null!;
    public DbSet<UploadedFile> UploadedFiles { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from Configurations folder
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// 自動更新 CreatedAt 和 UpdatedAt 時間戳記
    /// Automatically update CreatedAt and UpdatedAt timestamps
    /// </summary>
    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            var entity = entry.Entity;

            if (entry.State == EntityState.Added)
            {
                // Set CreatedAt for new entities
                var createdAtProperty = entry.Property("CreatedAt");
                if (createdAtProperty != null)
                {
                    createdAtProperty.CurrentValue = now;
                }
            }

            // Set UpdatedAt for all modified/added entities
            var updatedAtProperty = entry.Property("UpdatedAt");
            if (updatedAtProperty != null)
            {
                updatedAtProperty.CurrentValue = now;
            }
        }
    }
}
