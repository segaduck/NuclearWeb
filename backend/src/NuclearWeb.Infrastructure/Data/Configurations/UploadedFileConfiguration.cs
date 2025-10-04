using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NuclearWeb.Core.Entities;

namespace NuclearWeb.Infrastructure.Data.Configurations;

/// <summary>
/// 上傳檔案實體組態
/// UploadedFile entity configuration
/// </summary>
public class UploadedFileConfiguration : IEntityTypeConfiguration<UploadedFile>
{
    public void Configure(EntityTypeBuilder<UploadedFile> builder)
    {
        builder.ToTable("UploadedFiles");

        builder.HasKey(uf => uf.Id);

        builder.Property(uf => uf.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(uf => uf.StoredFileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(uf => uf.FileType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(uf => uf.FileExtension)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(uf => uf.FileSizeBytes)
            .IsRequired();

        builder.Property(uf => uf.UploadedBy)
            .IsRequired();

        builder.Property(uf => uf.UploadedAt)
            .IsRequired();

        builder.Property(uf => uf.Description)
            .HasMaxLength(500);

        builder.Property(uf => uf.Category)
            .HasMaxLength(50);

        builder.Property(uf => uf.DownloadCount)
            .IsRequired()
            .HasDefaultValue(0);

        // Indexes
        builder.HasIndex(uf => uf.UploadedBy)
            .HasDatabaseName("IX_UploadedFiles_UploadedBy");

        builder.HasIndex(uf => uf.Category)
            .HasDatabaseName("IX_UploadedFiles_Category");

        builder.HasIndex(uf => uf.FileType)
            .HasDatabaseName("IX_UploadedFiles_FileType");

        builder.HasIndex(uf => uf.StoredFileName)
            .IsUnique()
            .HasDatabaseName("UX_UploadedFiles_StoredFileName");

        // Relationship with User is configured in UserConfiguration
    }
}
