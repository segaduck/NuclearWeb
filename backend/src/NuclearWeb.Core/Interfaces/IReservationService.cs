using NuclearWeb.Core.Entities;

namespace NuclearWeb.Core.Interfaces;

/// <summary>
/// 預約服務介面
/// Reservation service interface
/// </summary>
public interface IReservationService
{
    /// <summary>
    /// 取得所有預約（分頁）
    /// Get all reservations with pagination
    /// </summary>
    /// <param name="page">頁碼 / Page number (1-based)</param>
    /// <param name="pageSize">每頁筆數 / Page size</param>
    /// <param name="roomId">會議室 ID 篩選 / Optional meeting room ID filter</param>
    /// <param name="userId">使用者 ID 篩選 / Optional user ID filter</param>
    /// <param name="status">狀態篩選 / Optional status filter</param>
    /// <param name="startDate">開始日期篩選 / Optional start date filter</param>
    /// <param name="endDate">結束日期篩選 / Optional end date filter</param>
    /// <returns>Paginated list of reservations</returns>
    Task<IEnumerable<Reservation>> GetReservationsAsync(
        int page, int pageSize, int? roomId = null, int? userId = null, string? status = null, DateTime? startDate = null, DateTime? endDate = null);

    /// <summary>
    /// 取得總筆數
    /// Get total count with filters
    /// </summary>
    Task<int> GetTotalCountAsync(int? roomId = null, int? userId = null, string? status = null, DateTime? startDate = null, DateTime? endDate = null);

    /// <summary>
    /// 取得單筆預約
    /// Get reservation by ID
    /// </summary>
    /// <param name="id">預約 ID / Reservation ID</param>
    /// <returns>Reservation if found; null otherwise</returns>
    Task<Reservation?> GetReservationByIdAsync(int id);

    /// <summary>
    /// 建立預約（含衝突檢查）
    /// Create reservation with conflict detection
    /// </summary>
    /// <param name="reservation">預約資料 / Reservation data</param>
    /// <returns>Created reservation if successful; null if conflict exists</returns>
    Task<Reservation?> CreateReservationAsync(Reservation reservation);

    /// <summary>
    /// 更新預約
    /// Update reservation
    /// </summary>
    /// <param name="id">預約 ID / Reservation ID</param>
    /// <param name="reservation">更新資料 / Updated data</param>
    /// <returns>Updated reservation if successful; null if not found or conflict</returns>
    Task<Reservation?> UpdateReservationAsync(int id, Reservation reservation);

    /// <summary>
    /// 刪除預約（取消）
    /// Delete (cancel) reservation
    /// </summary>
    /// <param name="id">預約 ID / Reservation ID</param>
    /// <returns>True if deleted; false if not found</returns>
    Task<bool> DeleteReservationAsync(int id);

    /// <summary>
    /// 檢查可用性
    /// Check availability for a room in a time range
    /// </summary>
    /// <param name="meetingRoomId">會議室 ID / Meeting room ID</param>
    /// <param name="startTime">開始時間 / Start time</param>
    /// <param name="endTime">結束時間 / End time</param>
    /// <param name="excludeReservationId">排除的預約 ID / Exclude reservation ID (for updates)</param>
    /// <returns>True if available; false if conflict exists</returns>
    Task<bool> CheckAvailabilityAsync(int meetingRoomId, DateTime startTime, DateTime endTime, int? excludeReservationId = null);

    /// <summary>
    /// 檢查衝突
    /// Check for conflicting reservations
    /// </summary>
    /// <param name="meetingRoomId">會議室 ID / Meeting room ID</param>
    /// <param name="startTime">開始時間 / Start time</param>
    /// <param name="endTime">結束時間 / End time</param>
    /// <param name="excludeReservationId">排除的預約 ID / Exclude reservation ID (for updates)</param>
    /// <returns>List of conflicting reservations</returns>
    Task<IEnumerable<Reservation>> CheckConflictsAsync(int meetingRoomId, DateTime startTime, DateTime endTime, int? excludeReservationId = null);

    /// <summary>
    /// 取消預約
    /// Cancel reservation
    /// </summary>
    /// <param name="id">預約 ID / Reservation ID</param>
    /// <returns>True if cancelled; false if not found</returns>
    Task<bool> CancelReservationAsync(int id);

    /// <summary>
    /// 取得會議室在日期範圍內的預約
    /// Get reservations for a room within a date range
    /// </summary>
    /// <param name="roomId">會議室 ID / Room ID</param>
    /// <param name="startDate">開始日期 / Start date</param>
    /// <param name="endDate">結束日期 / End date</param>
    /// <returns>List of reservations</returns>
    Task<IEnumerable<Reservation>> GetReservationsByRoomAndDateRangeAsync(int roomId, DateTime startDate, DateTime endDate);
}
