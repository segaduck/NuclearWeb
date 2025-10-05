using Microsoft.EntityFrameworkCore;
using NuclearWeb.Core.Entities;
using NuclearWeb.Core.Interfaces;
using NuclearWeb.Infrastructure.Data;

namespace NuclearWeb.Application.Services;

/// <summary>
/// 會議室服務實作
/// Meeting room service implementation
/// </summary>
public class RoomService : IRoomService
{
    private readonly ApplicationDbContext _context;

    public RoomService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<MeetingRoom> Items, int TotalCount)> GetRoomsAsync(int page, int pageSize, bool activeOnly = true)
    {
        var query = _context.MeetingRooms.AsQueryable();

        if (activeOnly)
        {
            query = query.Where(r => r.IsActive);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(r => r.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<IEnumerable<MeetingRoom>> GetAllRoomsAsync(bool includeInactive = false)
    {
        var query = _context.MeetingRooms.AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(r => r.IsActive);
        }

        return await query.OrderBy(r => r.Name).ToListAsync();
    }

    public async Task<MeetingRoom?> GetRoomByIdAsync(int id)
    {
        return await _context.MeetingRooms.FindAsync(id);
    }

    public async Task<MeetingRoom> CreateRoomAsync(MeetingRoom room)
    {
        _context.MeetingRooms.Add(room);
        await _context.SaveChangesAsync();
        return room;
    }

    public async Task<MeetingRoom?> UpdateRoomAsync(int id, MeetingRoom room)
    {
        var existing = await _context.MeetingRooms.FindAsync(id);
        if (existing == null)
        {
            return null;
        }

        existing.Name = room.Name;
        existing.Capacity = room.Capacity;
        existing.Location = room.Location;
        existing.Amenities = room.Amenities;
        existing.IsActive = room.IsActive;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteRoomAsync(int id)
    {
        var room = await _context.MeetingRooms.FindAsync(id);
        if (room == null)
        {
            return false;
        }

        // Soft delete
        room.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeactivateRoomAsync(int id)
    {
        var room = await _context.MeetingRooms.FindAsync(id);
        if (room == null)
        {
            return false;
        }

        room.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }
}
