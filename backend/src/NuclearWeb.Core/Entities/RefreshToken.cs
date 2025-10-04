namespace NuclearWeb.Core.Entities;

/// <summary>
/// 更新令牌實體 - 表示 JWT 認證的更新令牌
/// Represents refresh tokens for JWT authentication
/// </summary>
public class RefreshToken
{
    /// <summary>
    /// 主鍵，自動遞增
    /// Primary key, auto-increment
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 使用者外鍵
    /// Foreign key to User
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 更新令牌值（UUID）
    /// Refresh token value (UUID)
    /// Validation: Unique
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// 令牌過期時間
    /// Token expiration timestamp
    /// Typically +7 days from creation
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// 令牌建立時間戳記
    /// Token creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 令牌撤銷時間戳記
    /// Token revocation timestamp
    /// Nullable, set on logout/refresh
    /// </summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// 輪替時的新令牌
    /// New token on rotation
    /// Nullable
    /// </summary>
    public string? ReplacedByToken { get; set; }

    // Navigation properties

    /// <summary>
    /// 使用者導覽屬性
    /// User navigation property
    /// </summary>
    public User User { get; set; } = null!;
}
