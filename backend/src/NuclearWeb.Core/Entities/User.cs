namespace NuclearWeb.Core.Entities;

/// <summary>
/// 使用者實體 - 表示系統的已驗證使用者（一般使用者和管理員）
/// Represents authenticated users of the system (both normal users and administrators)
/// </summary>
public class User
{
    /// <summary>
    /// 主鍵，自動遞增
    /// Primary key, auto-increment
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 唯一登入識別碼（使用者名稱）
    /// Unique login identifier (username)
    /// Validation: 3-50 chars, alphanumeric + underscore
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Bcrypt 雜湊密碼
    /// Bcrypt hashed password
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// 使用者顯示名稱
    /// User's display name
    /// Validation: 1-100 chars
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// 使用者電子郵件地址
    /// User's email address
    /// Validation: Valid email format, unique
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 使用者角色
    /// User role
    /// Values: "Admin" or "User"
    /// </summary>
    public string Role { get; set; } = "User";

    /// <summary>
    /// 使用者介面主題偏好
    /// UI theme preference
    /// Values: "Light" or "Dark", default "Light"
    /// </summary>
    public string ThemePreference { get; set; } = "Light";

    /// <summary>
    /// 側邊欄收合狀態
    /// Sidebar UI state
    /// Default: false
    /// </summary>
    public bool SidebarCollapsed { get; set; } = false;

    /// <summary>
    /// 帳戶啟用狀態
    /// Account active status
    /// Default: true
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 帳戶建立時間戳記
    /// Account creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 最後修改時間戳記
    /// Last modification timestamp
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// 最後成功登入時間
    /// Last successful login timestamp
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    // Navigation properties

    /// <summary>
    /// 此使用者建立的預約
    /// Reservations created by this user
    /// </summary>
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    /// <summary>
    /// 此使用者撰寫的文章
    /// Articles authored by this user
    /// </summary>
    public ICollection<ContentArticle> AuthoredArticles { get; set; } = new List<ContentArticle>();

    /// <summary>
    /// 此使用者發布的文章（管理員核准）
    /// Articles published by this user (admin approval)
    /// </summary>
    public ICollection<ContentArticle> PublishedArticles { get; set; } = new List<ContentArticle>();

    /// <summary>
    /// 此使用者上傳的檔案
    /// Files uploaded by this user
    /// </summary>
    public ICollection<UploadedFile> UploadedFiles { get; set; } = new List<UploadedFile>();

    /// <summary>
    /// 此使用者的更新令牌
    /// Refresh tokens for this user
    /// </summary>
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
