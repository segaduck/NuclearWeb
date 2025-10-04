using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuclearWeb.Core.Interfaces;

namespace NuclearWeb.API.Controllers;

/// <summary>
/// 認證控制器
/// Authentication controller
/// </summary>
[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// 使用者登入
    /// User login
    /// POST /api/v1/auth/login
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request.Username, request.Password);

        if (result == null)
        {
            return Unauthorized(new { message = "使用者名稱或密碼錯誤" });
        }

        // Set refresh token in HTTP-only cookie
        Response.Cookies.Append("refreshToken", result.Value.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });

        return Ok(new
        {
            accessToken = result.Value.AccessToken,
            tokenType = "Bearer",
            expiresIn = 3600,
            user = new
            {
                id = result.Value.User.Id,
                username = result.Value.User.Username,
                displayName = result.Value.User.DisplayName,
                email = result.Value.User.Email,
                role = result.Value.User.Role,
                themePreference = result.Value.User.ThemePreference,
                sidebarCollapsed = result.Value.User.SidebarCollapsed
            }
        });
    }

    /// <summary>
    /// 更新令牌
    /// Refresh access token
    /// POST /api/v1/auth/refresh
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized(new { message = "未提供更新令牌" });
        }

        var result = await _authService.RefreshTokenAsync(refreshToken);

        if (result == null)
        {
            return Unauthorized(new { message = "更新令牌無效或已過期" });
        }

        // Set new refresh token in cookie
        Response.Cookies.Append("refreshToken", result.Value.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });

        return Ok(new
        {
            accessToken = result.Value.AccessToken,
            tokenType = "Bearer",
            expiresIn = 3600
        });
    }

    /// <summary>
    /// 使用者登出
    /// User logout
    /// POST /api/v1/auth/logout
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (!string.IsNullOrEmpty(refreshToken))
        {
            await _authService.LogoutAsync(refreshToken);
        }

        // Clear refresh token cookie
        Response.Cookies.Delete("refreshToken");

        return Ok(new { message = "已成功登出" });
    }

    /// <summary>
    /// 取得當前使用者資訊
    /// Get current user information
    /// GET /api/v1/auth/me
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized(new { message = "無效的令牌" });
        }

        var user = await _authService.GetCurrentUserAsync(userId);

        if (user == null)
        {
            return NotFound(new { message = "找不到使用者" });
        }

        return Ok(new
        {
            id = user.Id,
            username = user.Username,
            displayName = user.DisplayName,
            email = user.Email,
            role = user.Role,
            themePreference = user.ThemePreference,
            sidebarCollapsed = user.SidebarCollapsed,
            lastLoginAt = user.LastLoginAt
        });
    }
}

public record LoginRequest(string Username, string Password);
