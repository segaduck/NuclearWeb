using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuclearWeb.Core.Interfaces;
using NuclearWeb.Core.Entities;
using System.Text.Json;

namespace NuclearWeb.API.Controllers;

/// <summary>
/// 會議室管理控制器
/// Meeting rooms controller
/// </summary>
[ApiController]
[Route("api/v1/rooms")]
[Authorize]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;
    private readonly IReservationService _reservationService;

    public RoomsController(IRoomService roomService, IReservationService reservationService)
    {
        _roomService = roomService;
        _reservationService = reservationService;
    }

    /// <summary>
    /// 取得會議室列表
    /// Get list of meeting rooms
    /// GET /api/v1/rooms
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetRooms([FromQuery] bool includeInactive = false)
    {
        // Only admins can see inactive rooms
        var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        if (includeInactive && userRole != "Admin")
        {
            includeInactive = false;
        }

        var rooms = await _roomService.GetAllRoomsAsync(includeInactive);

        return Ok(new
        {
            data = rooms.Select(r => new
            {
                id = r.Id,
                name = r.Name,
                capacity = r.Capacity,
                location = r.Location,
                amenities = r.Amenities,
                isActive = r.IsActive,
                createdAt = r.CreatedAt,
                updatedAt = r.UpdatedAt
            })
        });
    }

    /// <summary>
    /// 建立新會議室
    /// Create a new meeting room (admin only)
    /// POST /api/v1/rooms
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateRoom([FromBody] CreateRoomRequest request)
    {
        var room = new MeetingRoom
        {
            Name = request.Name,
            Capacity = request.Capacity,
            Location = request.Location,
            Amenities = request.Amenities != null ? JsonSerializer.Serialize(request.Amenities) : null,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _roomService.CreateRoomAsync(room);

        return CreatedAtAction(nameof(GetRoom), new { id = created.Id }, new
        {
            id = created.Id,
            name = created.Name,
            capacity = created.Capacity,
            location = created.Location,
            amenities = created.Amenities,
            isActive = created.IsActive,
            createdAt = created.CreatedAt,
            updatedAt = created.UpdatedAt
        });
    }

    /// <summary>
    /// 取得單筆會議室詳情
    /// Get meeting room details by ID
    /// GET /api/v1/rooms/{id}
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRoom(int id)
    {
        var room = await _roomService.GetRoomByIdAsync(id);

        if (room == null)
        {
            return NotFound(new { error = new { code = "NOT_FOUND", message = "找不到會議室" } });
        }

        return Ok(new
        {
            id = room.Id,
            name = room.Name,
            capacity = room.Capacity,
            location = room.Location,
            amenities = room.Amenities,
            isActive = room.IsActive,
            createdAt = room.CreatedAt,
            updatedAt = room.UpdatedAt
        });
    }

    /// <summary>
    /// 更新會議室
    /// Update meeting room (admin only)
    /// PUT /api/v1/rooms/{id}
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateRoom(int id, [FromBody] UpdateRoomRequest request)
    {
        var room = await _roomService.GetRoomByIdAsync(id);

        if (room == null)
        {
            return NotFound(new { error = new { code = "NOT_FOUND", message = "找不到會議室" } });
        }

        if (!string.IsNullOrEmpty(request.Name))
            room.Name = request.Name;

        if (request.Capacity.HasValue)
            room.Capacity = request.Capacity.Value;

        if (request.Location != null)
            room.Location = request.Location;

        if (request.Amenities != null)
            room.Amenities = JsonSerializer.Serialize(request.Amenities);

        if (request.IsActive.HasValue)
            room.IsActive = request.IsActive.Value;

        room.UpdatedAt = DateTime.UtcNow;

        var updated = await _roomService.UpdateRoomAsync(id, room);
        if (updated == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            id = updated.Id,
            name = updated.Name,
            capacity = updated.Capacity,
            location = updated.Location,
            amenities = updated.Amenities,
            isActive = updated.IsActive,
            createdAt = updated.CreatedAt,
            updatedAt = updated.UpdatedAt
        });
    }

    /// <summary>
    /// 停用會議室
    /// Deactivate meeting room (admin only)
    /// DELETE /api/v1/rooms/{id}
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeactivateRoom(int id)
    {
        var room = await _roomService.GetRoomByIdAsync(id);

        if (room == null)
        {
            return NotFound(new { error = new { code = "NOT_FOUND", message = "找不到會議室" } });
        }

        await _roomService.DeactivateRoomAsync(id);

        return NoContent();
    }

    /// <summary>
    /// 取得會議室行程表
    /// Get room schedule
    /// GET /api/v1/rooms/{id}/schedule
    /// </summary>
    [HttpGet("{id}/schedule")]
    public async Task<IActionResult> GetRoomSchedule(
        int id,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var room = await _roomService.GetRoomByIdAsync(id);

        if (room == null)
        {
            return NotFound(new { error = new { code = "NOT_FOUND", message = "找不到會議室" } });
        }

        var reservations = await _reservationService.GetReservationsByRoomAndDateRangeAsync(
            id, startDate, endDate);

        return Ok(new
        {
            roomId = room.Id,
            roomName = room.Name,
            reservations = reservations.Select(r => new
            {
                id = r.Id,
                startTime = r.StartTime,
                endTime = r.EndTime,
                purpose = r.Purpose,
                userDisplayName = r.User?.DisplayName,
                status = r.Status.ToString()
            })
        });
    }
}

public record CreateRoomRequest(
    string Name,
    int Capacity,
    string? Location,
    List<string>? Amenities);

public record UpdateRoomRequest(
    string? Name,
    int? Capacity,
    string? Location,
    List<string>? Amenities,
    bool? IsActive);
