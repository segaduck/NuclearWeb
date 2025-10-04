using NuclearWeb.Core.Entities;

namespace NuclearWeb.Core.Interfaces;

/// <summary>
/// 選單服務介面
/// Menu service interface
/// </summary>
public interface IMenuService
{
    /// <summary>
    /// 取得所有選單（階層結構）
    /// Get all menus in hierarchical structure
    /// </summary>
    /// <param name="visibleOnly">僅顯示可見的 / Visible only filter</param>
    /// <returns>Hierarchical list of menu items</returns>
    Task<IEnumerable<MenuItem>> GetMenusAsync(bool visibleOnly = true);

    /// <summary>
    /// 取得單筆選單
    /// Get menu item by ID
    /// </summary>
    /// <param name="id">選單 ID / Menu item ID</param>
    /// <returns>Menu item if found; null otherwise</returns>
    Task<MenuItem?> GetMenuByIdAsync(int id);

    /// <summary>
    /// 建立選單
    /// Create menu item
    /// </summary>
    /// <param name="menuItem">選單資料 / Menu item data</param>
    /// <returns>Created menu item</returns>
    Task<MenuItem> CreateMenuAsync(MenuItem menuItem);

    /// <summary>
    /// 更新選單
    /// Update menu item
    /// </summary>
    /// <param name="id">選單 ID / Menu item ID</param>
    /// <param name="menuItem">更新資料 / Updated data</param>
    /// <returns>Updated menu item if successful; null if not found</returns>
    Task<MenuItem?> UpdateMenuAsync(int id, MenuItem menuItem);

    /// <summary>
    /// 刪除選單
    /// Delete menu item
    /// </summary>
    /// <param name="id">選單 ID / Menu item ID</param>
    /// <returns>True if deleted; false if not found</returns>
    Task<bool> DeleteMenuAsync(int id);

    /// <summary>
    /// 重新排序選單
    /// Reorder menu items
    /// </summary>
    /// <param name="menuOrders">選單 ID 和新順序的對應 / Menu ID to new DisplayOrder mappings</param>
    /// <returns>True if successful</returns>
    Task<bool> ReorderMenusAsync(Dictionary<int, int> menuOrders);
}
