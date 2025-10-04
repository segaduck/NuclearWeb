using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NuclearWeb.Core.Entities;

namespace NuclearWeb.Infrastructure.Data.Configurations;

/// <summary>
/// 會議室實體組態
/// MeetingRoom entity configuration
/// </summary>
public class MeetingRoomConfiguration : IEntityTypeConfiguration<MeetingRoom>
{
    public void Configure(EntityTypeBuilder<MeetingRoom> builder)
    {
        builder.ToTable("MeetingRooms");

        builder.HasKey(mr => mr.Id);

        builder.Property(mr => mr.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(mr => mr.Capacity)
            .IsRequired();

        builder.Property(mr => mr.Location)
            .HasMaxLength(255);

        builder.Property(mr => mr.Amenities)
            .HasColumnType("json");

        builder.Property(mr => mr.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(mr => mr.CreatedAt)
            .IsRequired();

        builder.Property(mr => mr.UpdatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(mr => mr.Name)
            .IsUnique()
            .HasDatabaseName("UX_MeetingRooms_Name");

        builder.HasIndex(mr => mr.IsActive)
            .HasDatabaseName("IX_MeetingRooms_IsActive");

        // Relationships
        builder.HasMany(mr => mr.Reservations)
            .WithOne(r => r.MeetingRoom)
            .HasForeignKey(r => r.MeetingRoomId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
