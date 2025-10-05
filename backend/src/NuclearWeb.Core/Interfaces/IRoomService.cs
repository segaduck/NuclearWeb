using NuclearWeb.Core.Entities;

namespace NuclearWeb.Core.Interfaces;

/// <summary>
/// 會議室服務介面
/// Meeting room service interface
/// </summary>
public interface IRoomService
{
    /// <summary>
    /// 取得所有會議室（分頁）
    /// Get all meeting rooms with pagination
    /// </summary>
    /// <param name="page">頁碼 / Page number (1-based)</param>
    /// <param name="pageSize">每頁筆數 / Page size</param>
    /// <param name="activeOnly">僅顯示啟用的 / Active only filter</param>
    /// <returns>Paginated list of meeting rooms</returns>
    Task<(IEnumerable<MeetingRoom> Items, int TotalCount)> GetRoomsAsync(int page, int pageSize, bool activeOnly = true);

    /// <summary>
    /// 取得所有會議室（非分頁）
    /// Get all meeting rooms without pagination
    /// </summary>
    /// <param name="includeInactive">包含停用的會議室 / Include inactive rooms</param>
    /// <returns>List of meeting rooms</returns>
    Task<IEnumerable<MeetingRoom>> GetAllRoomsAsync(bool includeInactive = false);

    /// <summary>
    /// 取得單筆會議室
    /// Get meeting room by ID
    /// </summary>
    /// <param name="id">會議室 ID / Meeting room ID</param>
    /// <returns>Meeting room if found; null otherwise</returns>
    Task<MeetingRoom?> GetRoomByIdAsync(int id);

    /// <summary>
    /// 建立會議室
    /// Create meeting room
    /// </summary>
    /// <param name="room">會議室資料 / Meeting room data</param>
    /// <returns>Created meeting room</returns>
    Task<MeetingRoom> CreateRoomAsync(MeetingRoom room);

    /// <summary>
    /// 更新會議室
    /// Update meeting room
    /// </summary>
    /// <param name="id">會議室 ID / Meeting room ID</param>
    /// <param name="room">更新資料 / Updated data</param>
    /// <returns>Updated meeting room if successful; null if not found</returns>
    Task<MeetingRoom?> UpdateRoomAsync(int id, MeetingRoom room);

    /// <summary>
    /// 刪除會議室（軟刪除）
    /// Delete meeting room (soft delete)
    /// </summary>
    /// <param name="id">會議室 ID / Meeting room ID</param>
    /// <returns>True if deleted; false if not found</returns>
    Task<bool> DeleteRoomAsync(int id);

    /// <summary>
    /// 停用會議室
    /// Deactivate meeting room
    /// </summary>
    /// <param name="id">會議室 ID / Meeting room ID</param>
    /// <returns>True if deactivated; false if not found</returns>
    Task<bool> DeactivateRoomAsync(int id);
}
