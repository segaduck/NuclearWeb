namespace NuclearWeb.Core.Entities;

/// <summary>
/// 預約實體 - 表示使用者對會議室的預約
/// Represents a meeting room booking by a user
/// </summary>
public class Reservation
{
    /// <summary>
    /// 主鍵，自動遞增
    /// Primary key, auto-increment
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 會議室外鍵
    /// Foreign key to MeetingRoom
    /// </summary>
    public int MeetingRoomId { get; set; }

    /// <summary>
    /// 使用者外鍵（建立者）
    /// Foreign key to User (creator)
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 預約開始時間
    /// Reservation start time
    /// Validation: Required, future datetime
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// 預約結束時間
    /// Reservation end time
    /// Validation: Required, after StartTime
    /// </summary>
    public DateTime EndTime { get; set; }

    /// <summary>
    /// 預約目的描述
    /// Reservation description/purpose
    /// Validation: Max 500 chars
    /// </summary>
    public string? Purpose { get; set; }

    /// <summary>
    /// 預計參加人數
    /// Expected number of attendees
    /// Validation: Min 1, max room capacity
    /// </summary>
    public int? AttendeeCount { get; set; }

    /// <summary>
    /// 預約狀態
    /// Reservation status
    /// Values: Confirmed, Cancelled
    /// </summary>
    public ReservationStatus Status { get; set; } = ReservationStatus.Confirmed;

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

    /// <summary>
    /// 建立者 ID（非正規化）
    /// User who created (denormalized)
    /// </summary>
    public int CreatedBy { get; set; }

    /// <summary>
    /// 最後修改者 ID
    /// Last user who modified
    /// </summary>
    public int? ModifiedBy { get; set; }

    // Navigation properties

    /// <summary>
    /// 會議室導覽屬性
    /// Meeting room navigation property
    /// </summary>
    public MeetingRoom MeetingRoom { get; set; } = null!;

    /// <summary>
    /// 使用者導覽屬性
    /// User navigation property
    /// </summary>
    public User User { get; set; } = null!;
}
