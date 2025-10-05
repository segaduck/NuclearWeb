using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuclearWeb.Core.Interfaces;
using NuclearWeb.Core.Entities;

namespace NuclearWeb.API.Controllers;

/// <summary>
/// 會議室預約控制器
/// Meeting room reservations controller
/// </summary>
[ApiController]
[Route("api/v1/reservations")]
[Authorize]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationsController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    /// <summary>
    /// 取得預約列表
    /// Get list of reservations with filtering
    /// GET /api/v1/reservations
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetReservations(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] int? roomId = null,
        [FromQuery] int? userId = null,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        if (page < 1 || pageSize < 1 || pageSize > 100)
        {
            return BadRequest(new { error = new { code = "INVALID_PARAMS", message = "無效的分頁參數" } });
        }

        var reservations = await _reservationService.GetReservationsAsync(
            page, pageSize, roomId, userId, status, startDate, endDate);

        var totalItems = await _reservationService.GetTotalCountAsync(roomId, userId, status, startDate, endDate);
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        return Ok(new
        {
            data = reservations.Select(r => new
            {
                id = r.Id,
                meetingRoomId = r.MeetingRoomId,
                meetingRoomName = r.MeetingRoom?.Name,
                userId = r.UserId,
                userDisplayName = r.User?.DisplayName,
                startTime = r.StartTime,
                endTime = r.EndTime,
                purpose = r.Purpose,
                attendeeCount = r.AttendeeCount,
                status = r.Status.ToString(),
                createdAt = r.CreatedAt,
                updatedAt = r.UpdatedAt,
                createdBy = r.CreatedBy,
                modifiedBy = r.ModifiedBy
            }),
            pagination = new
            {
                currentPage = page,
                pageSize,
                totalItems,
                totalPages
            }
        });
    }

    /// <summary>
    /// 建立新預約
    /// Create a new reservation
    /// POST /api/v1/reservations
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateReservation([FromBody] CreateReservationRequest request)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized(new { error = new { code = "INVALID_TOKEN", message = "無效的令牌" } });
        }

        // Check for conflicts
        var conflicts = await _reservationService.CheckConflictsAsync(
            request.MeetingRoomId, request.StartTime, request.EndTime, null);

        if (conflicts.Any())
        {
            var conflict = conflicts.First();
            return Conflict(new
            {
                error = new
                {
                    code = "RESERVATION_CONFLICT",
                    message = "會議室已被預約",
                    details = new
                    {
                        conflictingReservationId = conflict.Id,
                        roomId = request.MeetingRoomId,
                        timeSlot = $"{request.StartTime:yyyy-MM-ddTHH:mm:ssZ} - {request.EndTime:yyyy-MM-ddTHH:mm:ssZ}"
                    }
                }
            });
        }

        var reservation = new Reservation
        {
            MeetingRoomId = request.MeetingRoomId,
            UserId = userId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Purpose = request.Purpose,
            AttendeeCount = request.AttendeeCount,
            Status = ReservationStatus.Confirmed,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _reservationService.CreateReservationAsync(reservation);
        if (created == null)
        {
            return Conflict(new
            {
                error = new
                {
                    code = "RESERVATION_CONFLICT",
                    message = "會議室已被預約或不可用"
                }
            });
        }

        return CreatedAtAction(nameof(GetReservation), new { id = created.Id }, new
        {
            id = created.Id,
            meetingRoomId = created.MeetingRoomId,
            meetingRoomName = created.MeetingRoom?.Name,
            userId = created.UserId,
            userDisplayName = created.User?.DisplayName,
            startTime = created.StartTime,
            endTime = created.EndTime,
            purpose = created.Purpose,
            attendeeCount = created.AttendeeCount,
            status = created.Status.ToString(),
            createdAt = created.CreatedAt,
            updatedAt = created.UpdatedAt,
            createdBy = created.CreatedBy,
            modifiedBy = created.ModifiedBy
        });
    }

    /// <summary>
    /// 取得單筆預約詳情
    /// Get reservation details by ID
    /// GET /api/v1/reservations/{id}
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetReservation(int id)
    {
        var reservation = await _reservationService.GetReservationByIdAsync(id);

        if (reservation == null)
        {
            return NotFound(new { error = new { code = "NOT_FOUND", message = "找不到預約" } });
        }

        return Ok(new
        {
            id = reservation.Id,
            meetingRoomId = reservation.MeetingRoomId,
            meetingRoomName = reservation.MeetingRoom?.Name,
            userId = reservation.UserId,
            userDisplayName = reservation.User?.DisplayName,
            startTime = reservation.StartTime,
            endTime = reservation.EndTime,
            purpose = reservation.Purpose,
            attendeeCount = reservation.AttendeeCount,
            status = reservation.Status.ToString(),
            createdAt = reservation.CreatedAt,
            updatedAt = reservation.UpdatedAt,
            createdBy = reservation.CreatedBy,
            modifiedBy = reservation.ModifiedBy
        });
    }

    /// <summary>
    /// 更新預約
    /// Update an existing reservation
    /// PUT /api/v1/reservations/{id}
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReservation(int id, [FromBody] UpdateReservationRequest request)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized(new { error = new { code = "INVALID_TOKEN", message = "無效的令牌" } });
        }

        var reservation = await _reservationService.GetReservationByIdAsync(id);

        if (reservation == null)
        {
            return NotFound(new { error = new { code = "NOT_FOUND", message = "找不到預約" } });
        }

        // Check if user is owner or admin
        var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        if (reservation.UserId != userId && userRole != "Admin")
        {
            return Forbid();
        }

        // Check for conflicts if time is being changed
        if (request.StartTime.HasValue || request.EndTime.HasValue)
        {
            var newStartTime = request.StartTime ?? reservation.StartTime;
            var newEndTime = request.EndTime ?? reservation.EndTime;

            var conflicts = await _reservationService.CheckConflictsAsync(
                reservation.MeetingRoomId, newStartTime, newEndTime, id);

            if (conflicts.Any())
            {
                return Conflict(new
                {
                    error = new
                    {
                        code = "RESERVATION_CONFLICT",
                        message = "會議室已被預約"
                    }
                });
            }

            reservation.StartTime = newStartTime;
            reservation.EndTime = newEndTime;
        }

        if (request.Purpose != null)
            reservation.Purpose = request.Purpose;

        if (request.AttendeeCount.HasValue)
            reservation.AttendeeCount = request.AttendeeCount.Value;

        reservation.ModifiedBy = userId;
        reservation.UpdatedAt = DateTime.UtcNow;

        var updated = await _reservationService.UpdateReservationAsync(id, reservation);
        if (updated == null)
        {
            return Conflict(new
            {
                error = new
                {
                    code = "UPDATE_FAILED",
                    message = "更新失敗，可能存在衝突或預約不存在"
                }
            });
        }

        return Ok(new
        {
            id = updated.Id,
            meetingRoomId = updated.MeetingRoomId,
            meetingRoomName = updated.MeetingRoom?.Name,
            userId = updated.UserId,
            userDisplayName = updated.User?.DisplayName,
            startTime = updated.StartTime,
            endTime = updated.EndTime,
            purpose = updated.Purpose,
            attendeeCount = updated.AttendeeCount,
            status = updated.Status.ToString(),
            createdAt = updated.CreatedAt,
            updatedAt = updated.UpdatedAt,
            createdBy = updated.CreatedBy,
            modifiedBy = updated.ModifiedBy
        });
    }

    /// <summary>
    /// 取消預約
    /// Cancel a reservation
    /// DELETE /api/v1/reservations/{id}
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelReservation(int id)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized(new { error = new { code = "INVALID_TOKEN", message = "無效的令牌" } });
        }

        var reservation = await _reservationService.GetReservationByIdAsync(id);

        if (reservation == null)
        {
            return NotFound(new { error = new { code = "NOT_FOUND", message = "找不到預約" } });
        }

        // Check if user is owner or admin
        var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        if (reservation.UserId != userId && userRole != "Admin")
        {
            return Forbid();
        }

        await _reservationService.CancelReservationAsync(id);

        return NoContent();
    }

    /// <summary>
    /// 檢查可用性
    /// Check room availability
    /// POST /api/v1/reservations/check-availability
    /// </summary>
    [HttpPost("check-availability")]
    public async Task<IActionResult> CheckAvailability([FromBody] CheckAvailabilityRequest request)
    {
        var conflicts = await _reservationService.CheckConflictsAsync(
            request.RoomId, request.StartTime, request.EndTime, request.ExcludeReservationId);

        return Ok(new
        {
            available = !conflicts.Any(),
            conflicts = conflicts.Select(c => new
            {
                id = c.Id,
                meetingRoomId = c.MeetingRoomId,
                meetingRoomName = c.MeetingRoom?.Name,
                userId = c.UserId,
                userDisplayName = c.User?.DisplayName,
                startTime = c.StartTime,
                endTime = c.EndTime,
                purpose = c.Purpose,
                attendeeCount = c.AttendeeCount,
                status = c.Status.ToString()
            })
        });
    }
}

public record CreateReservationRequest(
    int MeetingRoomId,
    DateTime StartTime,
    DateTime EndTime,
    string? Purpose,
    int? AttendeeCount);

public record UpdateReservationRequest(
    DateTime? StartTime,
    DateTime? EndTime,
    string? Purpose,
    int? AttendeeCount);

public record CheckAvailabilityRequest(
    int RoomId,
    DateTime StartTime,
    DateTime EndTime,
    int? ExcludeReservationId);
