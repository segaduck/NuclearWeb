using Microsoft.EntityFrameworkCore;
using NuclearWeb.Core.Entities;
using NuclearWeb.Core.Interfaces;
using NuclearWeb.Infrastructure.Data;

namespace NuclearWeb.Application.Services;

/// <summary>
/// 使用者服務實作
/// User service implementation
/// </summary>
public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IAuthService _authService;

    public UserService(ApplicationDbContext context, IAuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task<(IEnumerable<User> Items, int TotalCount)> GetUsersAsync(int page, int pageSize, bool activeOnly = true)
    {
        var query = _context.Users.AsQueryable();

        if (activeOnly)
        {
            query = query.Where(u => u.IsActive);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(u => u.Username)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> CreateUserAsync(User user, string password)
    {
        // Check for duplicate username or email
        var exists = await _context.Users
            .AnyAsync(u => u.Username == user.Username || u.Email == user.Email);

        if (exists)
        {
            return null; // Duplicate exists
        }

        user.PasswordHash = _authService.HashPassword(password);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User?> UpdateUserAsync(int id, User user)
    {
        var existing = await _context.Users.FindAsync(id);
        if (existing == null)
        {
            return null;
        }

        // Username cannot be changed
        existing.DisplayName = user.DisplayName;
        existing.Email = user.Email;
        existing.Role = user.Role;
        existing.IsActive = user.IsActive;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return false;
        }

        // Soft delete
        user.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ResetPasswordAsync(int id, string newPassword)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return false;
        }

        user.PasswordHash = _authService.HashPassword(newPassword);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<User?> UpdatePreferencesAsync(int id, string? themePreference = null, bool? sidebarCollapsed = null)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return null;
        }

        if (themePreference != null)
        {
            user.ThemePreference = themePreference;
        }

        if (sidebarCollapsed.HasValue)
        {
            user.SidebarCollapsed = sidebarCollapsed.Value;
        }

        await _context.SaveChangesAsync();
        return user;
    }
}
