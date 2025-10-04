using NuclearWeb.Core.Entities;

namespace NuclearWeb.Core.Interfaces;

/// <summary>
/// 使用者服務介面
/// User service interface
/// </summary>
public interface IUserService
{
    /// <summary>
    /// 取得所有使用者（分頁）
    /// Get all users with pagination
    /// </summary>
    /// <param name="page">頁碼 / Page number (1-based)</param>
    /// <param name="pageSize">每頁筆數 / Page size</param>
    /// <param name="activeOnly">僅顯示啟用的 / Active only filter</param>
    /// <returns>Paginated list of users</returns>
    Task<(IEnumerable<User> Items, int TotalCount)> GetUsersAsync(int page, int pageSize, bool activeOnly = true);

    /// <summary>
    /// 取得單筆使用者
    /// Get user by ID
    /// </summary>
    /// <param name="id">使用者 ID / User ID</param>
    /// <returns>User if found; null otherwise</returns>
    Task<User?> GetUserByIdAsync(int id);

    /// <summary>
    /// 建立使用者
    /// Create user
    /// </summary>
    /// <param name="user">使用者資料 / User data</param>
    /// <param name="password">明文密碼 / Plain text password</param>
    /// <returns>Created user if successful; null if username/email already exists</returns>
    Task<User?> CreateUserAsync(User user, string password);

    /// <summary>
    /// 更新使用者
    /// Update user
    /// </summary>
    /// <param name="id">使用者 ID / User ID</param>
    /// <param name="user">更新資料 / Updated data</param>
    /// <returns>Updated user if successful; null if not found</returns>
    Task<User?> UpdateUserAsync(int id, User user);

    /// <summary>
    /// 刪除使用者（軟刪除）
    /// Delete user (soft delete)
    /// </summary>
    /// <param name="id">使用者 ID / User ID</param>
    /// <returns>True if deleted; false if not found</returns>
    Task<bool> DeleteUserAsync(int id);

    /// <summary>
    /// 重設密碼
    /// Reset user password (admin only)
    /// </summary>
    /// <param name="id">使用者 ID / User ID</param>
    /// <param name="newPassword">新密碼 / New password</param>
    /// <returns>True if successful; false if user not found</returns>
    Task<bool> ResetPasswordAsync(int id, string newPassword);

    /// <summary>
    /// 更新使用者偏好設定
    /// Update user preferences
    /// </summary>
    /// <param name="id">使用者 ID / User ID</param>
    /// <param name="themePreference">主題偏好 / Theme preference</param>
    /// <param name="sidebarCollapsed">側邊欄收合 / Sidebar collapsed</param>
    /// <returns>Updated user if successful; null if not found</returns>
    Task<User?> UpdatePreferencesAsync(int id, string? themePreference = null, bool? sidebarCollapsed = null);
}
