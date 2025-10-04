namespace NuclearWeb.Core.Entities;

/// <summary>
/// 會議室實體 - 表示可供預約的實體會議室
/// Represents physical meeting rooms available for reservation
/// </summary>
public class MeetingRoom
{
    /// <summary>
    /// 主鍵，自動遞增
    /// Primary key, auto-increment
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 會議室名稱/識別碼
    /// Room name/identifier
    /// Validation: Unique, 1-100 chars
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 最大容納人數
    /// Maximum occupancy
    /// Validation: Min 1, max 1000
    /// </summary>
    public int Capacity { get; set; }

    /// <summary>
    /// 實體位置描述
    /// Physical location description
    /// Validation: Max 255 chars
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// 設備列表（JSON 陣列）
    /// JSON array of amenities
    /// Example: ["Projector", "Whiteboard", "Video Conference", "Phone"]
    /// </summary>
    public string? Amenities { get; set; }

    /// <summary>
    /// 會議室可用狀態
    /// Room availability status
    /// Default: true
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 記錄建立時間戳記
    /// Record creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 最後修改時間戳記
    /// Last modification timestamp
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    // Navigation properties

    /// <summary>
    /// 此會議室的預約
    /// Reservations for this room
    /// </summary>
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
