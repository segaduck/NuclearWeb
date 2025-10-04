using Microsoft.EntityFrameworkCore;
using NuclearWeb.Core.Entities;
using NuclearWeb.Core.Interfaces;
using NuclearWeb.Infrastructure.Data;

namespace NuclearWeb.Application.Services;

/// <summary>
/// 選單服務實作
/// Menu service implementation
/// </summary>
public class MenuService : IMenuService
{
    private readonly ApplicationDbContext _context;

    public MenuService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MenuItem>> GetMenusAsync(bool visibleOnly = true)
    {
        var query = _context.MenuItems
            .Include(m => m.Children)
            .Include(m => m.Article)
            .Where(m => m.ParentId == null); // Get only top-level items

        if (visibleOnly)
        {
            query = query.Where(m => m.IsVisible);
        }

        var menus = await query
            .OrderBy(m => m.DisplayOrder)
            .ToListAsync();

        return menus;
    }

    public async Task<MenuItem?> GetMenuByIdAsync(int id)
    {
        return await _context.MenuItems
            .Include(m => m.Parent)
            .Include(m => m.Children)
            .Include(m => m.Article)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<MenuItem> CreateMenuAsync(MenuItem menuItem)
    {
        _context.MenuItems.Add(menuItem);
        await _context.SaveChangesAsync();
        return await GetMenuByIdAsync(menuItem.Id) ?? menuItem;
    }

    public async Task<MenuItem?> UpdateMenuAsync(int id, MenuItem menuItem)
    {
        var existing = await _context.MenuItems.FindAsync(id);
        if (existing == null)
        {
            return null;
        }

        existing.Name = menuItem.Name;
        existing.ParentId = menuItem.ParentId;
        existing.DisplayOrder = menuItem.DisplayOrder;
        existing.LinkType = menuItem.LinkType;
        existing.ArticleId = menuItem.ArticleId;
        existing.ExternalUrl = menuItem.ExternalUrl;
        existing.IsVisible = menuItem.IsVisible;

        await _context.SaveChangesAsync();
        return await GetMenuByIdAsync(id);
    }

    public async Task<bool> DeleteMenuAsync(int id)
    {
        var menuItem = await _context.MenuItems.FindAsync(id);
        if (menuItem == null)
        {
            return false;
        }

        _context.MenuItems.Remove(menuItem);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ReorderMenusAsync(Dictionary<int, int> menuOrders)
    {
        foreach (var (menuId, displayOrder) in menuOrders)
        {
            var menuItem = await _context.MenuItems.FindAsync(menuId);
            if (menuItem != null)
            {
                menuItem.DisplayOrder = displayOrder;
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }
}
