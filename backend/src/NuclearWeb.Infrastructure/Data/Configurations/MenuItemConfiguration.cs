using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NuclearWeb.Core.Entities;

namespace NuclearWeb.Infrastructure.Data.Configurations;

/// <summary>
/// 選單項目實體組態
/// MenuItem entity configuration
/// </summary>
public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.ToTable("MenuItems");

        builder.HasKey(mi => mi.Id);

        builder.Property(mi => mi.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(mi => mi.ParentId)
            .IsRequired(false);

        builder.Property(mi => mi.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(mi => mi.LinkType)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(mi => mi.ArticleId)
            .IsRequired(false);

        builder.Property(mi => mi.ExternalUrl)
            .HasMaxLength(500);

        builder.Property(mi => mi.IsVisible)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(mi => mi.CreatedAt)
            .IsRequired();

        builder.Property(mi => mi.UpdatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(mi => mi.ParentId)
            .HasDatabaseName("IX_MenuItems_ParentId");

        builder.HasIndex(mi => mi.DisplayOrder)
            .HasDatabaseName("IX_MenuItems_DisplayOrder");

        builder.HasIndex(mi => mi.ArticleId)
            .HasDatabaseName("IX_MenuItems_ArticleId");

        // Self-referential relationship
        builder.HasOne(mi => mi.Parent)
            .WithMany(mi => mi.Children)
            .HasForeignKey(mi => mi.ParentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship with ContentArticle is configured in ContentArticleConfiguration
    }
}
