using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NuclearWeb.Core.Entities;

namespace NuclearWeb.Infrastructure.Data.Configurations;

/// <summary>
/// 預約實體組態
/// Reservation entity configuration
/// </summary>
public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("Reservations");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.MeetingRoomId)
            .IsRequired();

        builder.Property(r => r.UserId)
            .IsRequired();

        builder.Property(r => r.StartTime)
            .IsRequired();

        builder.Property(r => r.EndTime)
            .IsRequired();

        builder.Property(r => r.Purpose)
            .HasMaxLength(500);

        builder.Property(r => r.AttendeeCount)
            .IsRequired(false);

        builder.Property(r => r.Status)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("Confirmed");

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.UpdatedAt)
            .IsRequired();

        builder.Property(r => r.CreatedBy)
            .IsRequired();

        builder.Property(r => r.ModifiedBy)
            .IsRequired(false);

        // Indexes
        builder.HasIndex(r => r.MeetingRoomId)
            .HasDatabaseName("IX_Reservations_MeetingRoomId");

        builder.HasIndex(r => r.UserId)
            .HasDatabaseName("IX_Reservations_UserId");

        builder.HasIndex(r => new { r.StartTime, r.EndTime })
            .HasDatabaseName("IX_Reservations_TimeRange");

        builder.HasIndex(r => r.Status)
            .HasDatabaseName("IX_Reservations_Status");

        builder.HasIndex(r => new { r.MeetingRoomId, r.StartTime, r.EndTime })
            .HasDatabaseName("IX_Reservations_ConflictCheck");

        // Relationships are configured in UserConfiguration and MeetingRoomConfiguration
    }
}
