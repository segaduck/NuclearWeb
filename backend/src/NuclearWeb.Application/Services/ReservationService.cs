using Microsoft.EntityFrameworkCore;
using NuclearWeb.Core.Entities;
using NuclearWeb.Core.Interfaces;
using NuclearWeb.Infrastructure.Data;

namespace NuclearWeb.Application.Services;

/// <summary>
/// 預約服務實作
/// Reservation service implementation
/// </summary>
public class ReservationService : IReservationService
{
    private readonly ApplicationDbContext _context;

    public ReservationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Reservation>> GetReservationsAsync(
        int page, int pageSize, int? roomId = null, int? userId = null, string? status = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.Reservations
            .Include(r => r.MeetingRoom)
            .Include(r => r.User)
            .AsQueryable();

        if (roomId.HasValue)
        {
            query = query.Where(r => r.MeetingRoomId == roomId.Value);
        }

        if (userId.HasValue)
        {
            query = query.Where(r => r.UserId == userId.Value);
        }

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<ReservationStatus>(status, out var statusEnum))
        {
            query = query.Where(r => r.Status == statusEnum);
        }

        if (startDate.HasValue)
        {
            query = query.Where(r => r.StartTime >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(r => r.EndTime <= endDate.Value);
        }

        return await query
            .OrderByDescending(r => r.StartTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync(int? roomId = null, int? userId = null, string? status = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.Reservations.AsQueryable();

        if (roomId.HasValue)
        {
            query = query.Where(r => r.MeetingRoomId == roomId.Value);
        }

        if (userId.HasValue)
        {
            query = query.Where(r => r.UserId == userId.Value);
        }

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<ReservationStatus>(status, out var statusEnum))
        {
            query = query.Where(r => r.Status == statusEnum);
        }

        if (startDate.HasValue)
        {
            query = query.Where(r => r.StartTime >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(r => r.EndTime <= endDate.Value);
        }

        return await query.CountAsync();
    }

    public async Task<Reservation?> GetReservationByIdAsync(int id)
    {
        return await _context.Reservations
            .Include(r => r.MeetingRoom)
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Reservation?> CreateReservationAsync(Reservation reservation)
    {
        // Check availability first
        var isAvailable = await CheckAvailabilityAsync(
            reservation.MeetingRoomId,
            reservation.StartTime,
            reservation.EndTime
        );

        if (!isAvailable)
        {
            return null; // Conflict exists
        }

        // Validate room exists and is active
        var room = await _context.MeetingRooms.FindAsync(reservation.MeetingRoomId);
        if (room == null || !room.IsActive)
        {
            return null;
        }

        // Validate attendee count doesn't exceed room capacity
        if (reservation.AttendeeCount.HasValue && reservation.AttendeeCount.Value > room.Capacity)
        {
            return null;
        }

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        return await GetReservationByIdAsync(reservation.Id);
    }

    public async Task<Reservation?> UpdateReservationAsync(int id, Reservation reservation)
    {
        var existing = await _context.Reservations.FindAsync(id);
        if (existing == null)
        {
            return null;
        }

        // Check availability (excluding current reservation)
        var isAvailable = await CheckAvailabilityAsync(
            reservation.MeetingRoomId,
            reservation.StartTime,
            reservation.EndTime,
            id
        );

        if (!isAvailable)
        {
            return null; // Conflict exists
        }

        // Update fields
        existing.MeetingRoomId = reservation.MeetingRoomId;
        existing.StartTime = reservation.StartTime;
        existing.EndTime = reservation.EndTime;
        existing.Purpose = reservation.Purpose;
        existing.AttendeeCount = reservation.AttendeeCount;
        existing.Status = reservation.Status;
        existing.ModifiedBy = reservation.ModifiedBy;

        await _context.SaveChangesAsync();

        return await GetReservationByIdAsync(id);
    }

    public async Task<bool> DeleteReservationAsync(int id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null)
        {
            return false;
        }

        // Soft delete by changing status to Cancelled
        reservation.Status = ReservationStatus.Cancelled;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> CheckAvailabilityAsync(int meetingRoomId, DateTime startTime, DateTime endTime, int? excludeReservationId = null)
    {
        var query = _context.Reservations
            .Where(r => r.MeetingRoomId == meetingRoomId && r.Status == ReservationStatus.Confirmed);

        if (excludeReservationId.HasValue)
        {
            query = query.Where(r => r.Id != excludeReservationId.Value);
        }

        // Check for overlapping reservations
        // Overlap exists if: NOT (EndTime <= newStart OR StartTime >= newEnd)
        var hasConflict = await query
            .AnyAsync(r => !(r.EndTime <= startTime || r.StartTime >= endTime));

        return !hasConflict;
    }

    public async Task<IEnumerable<Reservation>> CheckConflictsAsync(int meetingRoomId, DateTime startTime, DateTime endTime, int? excludeReservationId = null)
    {
        var query = _context.Reservations
            .Include(r => r.MeetingRoom)
            .Include(r => r.User)
            .Where(r => r.MeetingRoomId == meetingRoomId && r.Status == ReservationStatus.Confirmed);

        if (excludeReservationId.HasValue)
        {
            query = query.Where(r => r.Id != excludeReservationId.Value);
        }

        // Find overlapping reservations
        return await query
            .Where(r => !(r.EndTime <= startTime || r.StartTime >= endTime))
            .ToListAsync();
    }

    public async Task<bool> CancelReservationAsync(int id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null)
        {
            return false;
        }

        reservation.Status = ReservationStatus.Cancelled;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<Reservation>> GetReservationsByRoomAndDateRangeAsync(int roomId, DateTime startDate, DateTime endDate)
    {
        return await _context.Reservations
            .Include(r => r.MeetingRoom)
            .Include(r => r.User)
            .Where(r => r.MeetingRoomId == roomId &&
                       r.Status == ReservationStatus.Confirmed &&
                       r.StartTime >= startDate &&
                       r.EndTime <= endDate)
            .OrderBy(r => r.StartTime)
            .ToListAsync();
    }
}
