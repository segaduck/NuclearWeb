using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuclearWeb.Core.Interfaces;
using NuclearWeb.Core.Entities;
using System.Text.RegularExpressions;

namespace NuclearWeb.API.Controllers;

/// <summary>
/// 使用者管理控制器
/// User account management controller
/// </summary>
[ApiController]
[Route("api/v1/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuthService _authService;

    public UsersController(IUserService userService, IAuthService authService)
    {
        _userService = userService;
        _authService = authService;
    }

    /// <summary>
    /// 取得使用者列表
    /// Get paginated list of users (admin only)
    /// GET /api/v1/users
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? role = null,
        [FromQuery] bool? isActive = null)
    {
        if (page < 1 || pageSize < 1 || pageSize > 100)
        {
            return BadRequest(new { error = new { code = "INVALID_PARAMS", message = "無效的分頁參數" } });
        }

        var activeOnly = !isActive.HasValue || isActive.Value;
        var (items, totalCount) = await _userService.GetUsersAsync(page, pageSize, activeOnly);

        // Filter by role if specified
        if (!string.IsNullOrEmpty(role))
        {
            items = items.Where(u => u.Role == role);
            totalCount = items.Count();
        }

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return Ok(new
        {
            data = items.Select(u => MapUserToResponse(u)),
            pagination = new
            {
                currentPage = page,
                pageSize,
                totalItems = totalCount,
                totalPages
            }
        });
    }

    /// <summary>
    /// 建立新使用者
    /// Create a new user account (admin only)
    /// POST /api/v1/users
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        // Validate username
        if (string.IsNullOrWhiteSpace(request.Username) || request.Username.Length < 3 || request.Username.Length > 50)
        {
            return UnprocessableEntity(new { error = new { code = "VALIDATION_ERROR", message = "使用者名稱長度必須為 3-50 字元" } });
        }

        var usernameRegex = new Regex(@"^[a-zA-Z0-9_]+$");
        if (!usernameRegex.IsMatch(request.Username))
        {
            return UnprocessableEntity(new { error = new { code = "VALIDATION_ERROR", message = "使用者名稱只能包含英數字和底線" } });
        }

        // Validate password
        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8 || request.Password.Length > 100)
        {
            return UnprocessableEntity(new { error = new { code = "VALIDATION_ERROR", message = "密碼長度必須為 8-100 字元" } });
        }

        // Validate display name
        if (string.IsNullOrWhiteSpace(request.DisplayName) || request.DisplayName.Length > 100)
        {
            return UnprocessableEntity(new { error = new { code = "VALIDATION_ERROR", message = "顯示名稱長度必須為 1-100 字元" } });
        }

        // Validate email
        if (string.IsNullOrWhiteSpace(request.Email) || request.Email.Length > 255)
        {
            return UnprocessableEntity(new { error = new { code = "VALIDATION_ERROR", message = "電子郵件長度不可超過 255 字元" } });
        }

        var user = new User
        {
            Username = request.Username,
            DisplayName = request.DisplayName,
            Email = request.Email,
            Role = request.Role ?? "User",
            ThemePreference = "Light",
            SidebarCollapsed = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _userService.CreateUserAsync(user, request.Password);

        if (created == null)
        {
            return Conflict(new { error = new { code = "DUPLICATE_USER", message = "使用者名稱或電子郵件已存在" } });
        }

        return CreatedAtAction(nameof(GetUser), new { id = created.Id }, MapUserToResponse(created));
    }

    /// <summary>
    /// 取得單筆使用者詳情
    /// Get user details by ID (admin only, or own profile)
    /// GET /api/v1/users/{id}
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var currentUserId))
        {
            return Unauthorized(new { error = new { code = "INVALID_TOKEN", message = "無效的令牌" } });
        }

        // Check if user is viewing own profile or is admin
        var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        if (id != currentUserId && userRole != "Admin")
        {
            return Forbid();
        }

        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
        {
            return NotFound(new { error = new { code = "NOT_FOUND", message = "找不到使用者" } });
        }

        return Ok(MapUserToResponse(user));
    }

    /// <summary>
    /// 更新使用者
    /// Update user details (admin only, or own profile with restrictions)
    /// PUT /api/v1/users/{id}
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var currentUserId))
        {
            return Unauthorized(new { error = new { code = "INVALID_TOKEN", message = "無效的令牌" } });
        }

        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
        {
            return NotFound(new { error = new { code = "NOT_FOUND", message = "找不到使用者" } });
        }

        // Check permissions
        var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        var isAdmin = userRole == "Admin";
        var isOwnProfile = id == currentUserId;

        if (!isAdmin && !isOwnProfile)
        {
            return Forbid();
        }

        // Validate display name if provided
        if (request.DisplayName != null && (string.IsNullOrWhiteSpace(request.DisplayName) || request.DisplayName.Length > 100))
        {
            return UnprocessableEntity(new { error = new { code = "VALIDATION_ERROR", message = "顯示名稱長度必須為 1-100 字元" } });
        }

        // Validate email if provided
        if (request.Email != null && (string.IsNullOrWhiteSpace(request.Email) || request.Email.Length > 255))
        {
            return UnprocessableEntity(new { error = new { code = "VALIDATION_ERROR", message = "電子郵件長度不可超過 255 字元" } });
        }

        // Update fields
        if (request.DisplayName != null)
            user.DisplayName = request.DisplayName;

        if (request.Email != null)
            user.Email = request.Email;

        // Admin-only fields
        if (isAdmin)
        {
            if (request.Role != null)
                user.Role = request.Role;

            if (request.IsActive.HasValue)
                user.IsActive = request.IsActive.Value;
        }
        else
        {
            // Non-admin cannot change role or active status
            if (request.Role != null || request.IsActive.HasValue)
            {
                return Forbid();
            }
        }

        user.UpdatedAt = DateTime.UtcNow;

        var updated = await _userService.UpdateUserAsync(id, user);

        if (updated == null)
        {
            return Conflict(new { error = new { code = "DUPLICATE_EMAIL", message = "電子郵件已存在" } });
        }

        return Ok(MapUserToResponse(updated));
    }

    /// <summary>
    /// 停用使用者
    /// Deactivate user (soft delete, admin only)
    /// DELETE /api/v1/users/{id}
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeactivateUser(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
        {
            return NotFound(new { error = new { code = "NOT_FOUND", message = "找不到使用者" } });
        }

        await _userService.DeleteUserAsync(id);

        return NoContent();
    }

    /// <summary>
    /// 重設使用者密碼
    /// Reset user password (admin or own password)
    /// POST /api/v1/users/{id}/reset-password
    /// </summary>
    [HttpPost("{id}/reset-password")]
    public async Task<IActionResult> ResetPassword(int id, [FromBody] ResetPasswordRequest request)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var currentUserId))
        {
            return Unauthorized(new { error = new { code = "INVALID_TOKEN", message = "無效的令牌" } });
        }

        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
        {
            return NotFound(new { error = new { code = "NOT_FOUND", message = "找不到使用者" } });
        }

        // Validate new password
        if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 8 || request.NewPassword.Length > 100)
        {
            return UnprocessableEntity(new { error = new { code = "VALIDATION_ERROR", message = "新密碼長度必須為 8-100 字元" } });
        }

        // Check permissions
        var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        var isAdmin = userRole == "Admin";
        var isOwnProfile = id == currentUserId;

        if (!isAdmin && !isOwnProfile)
        {
            return Forbid();
        }

        // If user is resetting own password, verify current password
        if (!isAdmin && isOwnProfile)
        {
            if (string.IsNullOrWhiteSpace(request.CurrentPassword))
            {
                return BadRequest(new { error = new { code = "CURRENT_PASSWORD_REQUIRED", message = "必須提供目前密碼" } });
            }

            // Verify current password using AuthService
            var validPassword = await _authService.ValidateCredentialsAsync(user.Username, request.CurrentPassword);
            if (validPassword == null)
            {
                return BadRequest(new { error = new { code = "INVALID_CURRENT_PASSWORD", message = "目前密碼不正確" } });
            }
        }

        var success = await _userService.ResetPasswordAsync(id, request.NewPassword);

        if (!success)
        {
            return NotFound(new { error = new { code = "NOT_FOUND", message = "找不到使用者" } });
        }

        return Ok(new { message = "密碼已重設" });
    }

    /// <summary>
    /// 更新使用者偏好設定
    /// Update user preferences (own profile only)
    /// PUT /api/v1/users/me/preferences
    /// </summary>
    [HttpPut("me/preferences")]
    public async Task<IActionResult> UpdatePreferences([FromBody] UpdatePreferencesRequest request)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized(new { error = new { code = "INVALID_TOKEN", message = "無效的令牌" } });
        }

        // Validate theme preference if provided
        if (request.ThemePreference != null && request.ThemePreference != "Light" && request.ThemePreference != "Dark")
        {
            return UnprocessableEntity(new { error = new { code = "VALIDATION_ERROR", message = "主題偏好必須為 Light 或 Dark" } });
        }

        var updated = await _userService.UpdatePreferencesAsync(userId, request.ThemePreference, request.SidebarCollapsed);

        if (updated == null)
        {
            return NotFound(new { error = new { code = "NOT_FOUND", message = "找不到使用者" } });
        }

        return Ok(MapUserToResponse(updated));
    }

    // Helper method to map User to response object (excludes password hash)
    private object MapUserToResponse(User user)
    {
        return new
        {
            id = user.Id,
            username = user.Username,
            displayName = user.DisplayName,
            email = user.Email,
            role = user.Role,
            themePreference = user.ThemePreference,
            sidebarCollapsed = user.SidebarCollapsed,
            isActive = user.IsActive,
            createdAt = user.CreatedAt,
            updatedAt = user.UpdatedAt,
            lastLoginAt = user.LastLoginAt
        };
    }
}

public record CreateUserRequest(
    string Username,
    string Password,
    string DisplayName,
    string Email,
    string? Role);

public record UpdateUserRequest(
    string? DisplayName,
    string? Email,
    string? Role,
    bool? IsActive);

public record ResetPasswordRequest(
    string? CurrentPassword,
    string NewPassword);

public record UpdatePreferencesRequest(
    string? ThemePreference,
    bool? SidebarCollapsed);
