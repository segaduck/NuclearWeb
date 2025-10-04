using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NuclearWeb.Core.Entities;

namespace NuclearWeb.Infrastructure.Data.Configurations;

/// <summary>
/// 內容文章實體組態
/// ContentArticle entity configuration
/// </summary>
public class ContentArticleConfiguration : IEntityTypeConfiguration<ContentArticle>
{
    public void Configure(EntityTypeBuilder<ContentArticle> builder)
    {
        builder.ToTable("ContentArticles");

        builder.HasKey(ca => ca.Id);

        builder.Property(ca => ca.Title)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(ca => ca.Content)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(ca => ca.AuthorId)
            .IsRequired();

        builder.Property(ca => ca.PublicationStatus)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("Draft");

        builder.Property(ca => ca.AvailableFrom)
            .IsRequired(false);

        builder.Property(ca => ca.AvailableUntil)
            .IsRequired(false);

        builder.Property(ca => ca.ViewCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(ca => ca.CreatedAt)
            .IsRequired();

        builder.Property(ca => ca.UpdatedAt)
            .IsRequired();

        builder.Property(ca => ca.PublishedAt)
            .IsRequired(false);

        builder.Property(ca => ca.PublishedBy)
            .IsRequired(false);

        // Indexes
        builder.HasIndex(ca => ca.AuthorId)
            .HasDatabaseName("IX_ContentArticles_AuthorId");

        builder.HasIndex(ca => ca.PublicationStatus)
            .HasDatabaseName("IX_ContentArticles_PublicationStatus");

        builder.HasIndex(ca => new { ca.PublicationStatus, ca.AvailableFrom, ca.AvailableUntil })
            .HasDatabaseName("IX_ContentArticles_Availability");

        builder.HasIndex(ca => ca.PublishedAt)
            .HasDatabaseName("IX_ContentArticles_PublishedAt");

        // Relationships are configured in UserConfiguration
        builder.HasMany(ca => ca.MenuItems)
            .WithOne(mi => mi.Article)
            .HasForeignKey(mi => mi.ArticleId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
