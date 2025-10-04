using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuclearWeb.Core.Entities;
using NuclearWeb.Core.Interfaces;
using NuclearWeb.Infrastructure.Data;
using BCrypt.Net;

namespace NuclearWeb.Application.Services;

/// <summary>
/// 認證服務實作
/// Authentication service implementation
/// </summary>
public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly string _jwtSecret;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;
    private readonly int _jwtExpiryMinutes;
    private readonly int _refreshTokenExpiryDays;

    public AuthService(ApplicationDbContext context, string jwtSecret, string jwtIssuer, string jwtAudience, int jwtExpiryMinutes = 60, int refreshTokenExpiryDays = 7)
    {
        _context = context;
        _jwtSecret = jwtSecret;
        _jwtIssuer = jwtIssuer;
        _jwtAudience = jwtAudience;
        _jwtExpiryMinutes = jwtExpiryMinutes;
        _refreshTokenExpiryDays = refreshTokenExpiryDays;
    }

    public async Task<(string AccessToken, string RefreshToken, User User)?> LoginAsync(string username, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

        if (user == null || !VerifyPassword(password, user.PasswordHash))
        {
            return null;
        }

        // Update last login timestamp
        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Generate tokens
        var accessToken = GenerateAccessToken(user);
        var refreshToken = await GenerateRefreshTokenAsync(user.Id);

        return (accessToken, refreshToken, user);
    }

    public async Task<(string AccessToken, string RefreshToken)?> RefreshTokenAsync(string refreshToken)
    {
        var token = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow);

        if (token == null || !token.User.IsActive)
        {
            return null;
        }

        // Revoke old token
        token.RevokedAt = DateTime.UtcNow;

        // Generate new tokens
        var newAccessToken = GenerateAccessToken(token.User);
        var newRefreshToken = await GenerateRefreshTokenAsync(token.User.Id);

        // Set replacement reference
        token.ReplacedByToken = newRefreshToken;

        await _context.SaveChangesAsync();

        return (newAccessToken, newRefreshToken);
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (token != null && token.RevokedAt == null)
        {
            token.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<User?> GetCurrentUserAsync(int userId)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);
    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }

    private string GenerateAccessToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("DisplayName", user.DisplayName)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtIssuer,
            audience: _jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtExpiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<string> GenerateRefreshTokenAsync(int userId)
    {
        // Revoke existing active tokens for this user
        var existingTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
            .ToListAsync();

        foreach (var existingToken in existingTokens)
        {
            existingToken.RevokedAt = DateTime.UtcNow;
        }

        // Create new refresh token
        var refreshToken = new RefreshToken
        {
            UserId = userId,
            Token = Guid.NewGuid().ToString(),
            ExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays),
            CreatedAt = DateTime.UtcNow
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return refreshToken.Token;
    }
}
