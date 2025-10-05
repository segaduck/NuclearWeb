using NuclearWeb.Core.Entities;

namespace NuclearWeb.Core.Interfaces;

/// <summary>
/// 認證服務介面
/// Authentication service interface
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// 使用者登入
    /// User login with username and password
    /// </summary>
    /// <param name="username">使用者名稱 / Username</param>
    /// <param name="password">密碼 / Password</param>
    /// <returns>Access token, refresh token and user profile if successful; null if failed</returns>
    Task<(string AccessToken, string RefreshToken, User User)?> LoginAsync(string username, string password);

    /// <summary>
    /// 更新存取令牌
    /// Refresh access token using refresh token
    /// </summary>
    /// <param name="refreshToken">更新令牌 / Refresh token</param>
    /// <returns>New access token and refresh token if successful; null if failed</returns>
    Task<(string AccessToken, string RefreshToken)?> RefreshTokenAsync(string refreshToken);

    /// <summary>
    /// 使用者登出
    /// User logout (revoke refresh token)
    /// </summary>
    /// <param name="refreshToken">更新令牌 / Refresh token</param>
    Task LogoutAsync(string refreshToken);

    /// <summary>
    /// 取得當前使用者資料
    /// Get current user profile by user ID
    /// </summary>
    /// <param name="userId">使用者 ID / User ID</param>
    /// <returns>User profile if found; null otherwise</returns>
    Task<User?> GetCurrentUserAsync(int userId);

    /// <summary>
    /// 雜湊密碼
    /// Hash password using BCrypt
    /// </summary>
    /// <param name="password">明文密碼 / Plain text password</param>
    /// <returns>Hashed password</returns>
    string HashPassword(string password);

    /// <summary>
    /// 驗證密碼
    /// Verify password against hash
    /// </summary>
    /// <param name="password">明文密碼 / Plain text password</param>
    /// <param name="passwordHash">雜湊密碼 / Hashed password</param>
    /// <returns>True if password matches; false otherwise</returns>
    bool VerifyPassword(string password, string passwordHash);

    /// <summary>
    /// 驗證使用者憑證
    /// Validate user credentials (username and password)
    /// </summary>
    /// <param name="username">使用者名稱 / Username</param>
    /// <param name="password">密碼 / Password</param>
    /// <returns>User if credentials are valid; null otherwise</returns>
    Task<User?> ValidateCredentialsAsync(string username, string password);
}
