using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuclearWeb.Core.Interfaces;
using NuclearWeb.Core.Entities;

namespace NuclearWeb.API.Controllers;

/// <summary>
/// 選單管理控制器
/// Menu structure management controller
/// </summary>
[ApiController]
[Route("api/v1/menus")]
public class MenusController : ControllerBase
{
    private readonly IMenuService _menuService;

    public MenusController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    /// <summary>
    /// 取得選單結構
    /// Get complete menu hierarchy
    /// GET /api/v1/menus
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetMenus([FromQuery] bool includeHidden = false)
    {
        // Only admins can see hidden menus
        var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        if (includeHidden && userRole != "Admin")
        {
            includeHidden = false;
        }

        var menus = await _menuService.GetMenusAsync(visibleOnly: !includeHidden);

        return Ok(new
        {
            data = menus.Select(m => MapMenuItemToResponse(m))
        });
    }

    /// <summary>
    /// 建立新選單項目
    /// Create a new menu item (admin only)
    /// POST /api/v1/menus
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateMenuItem([FromBody] CreateMenuItemRequest request)
    {
        // Validate required fields
        if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length > 100)
        {
            return UnprocessableEntity(new { error = new { code = "VALIDATION_ERROR", message = "名稱長度必須為 1-100 字元" } });
        }

        // Validate linkType and corresponding fields
        if (request.LinkType == "Article")
        {
            if (!request.ArticleId.HasValue)
            {
                return UnprocessableEntity(new { error = new { code = "VALIDATION_ERROR", message = "文章連結類型必須提供文章 ID" } });
            }
            if (!string.IsNullOrEmpty(request.ExternalUrl))
            {
                return UnprocessableEntity(new { error = new { code = "VALIDATION_ERROR", message = "文章連結類型不可提供外部 URL" } });
            }
        }
        else if (request.LinkType == "ExternalUrl")
        {
            if (string.IsNullOrWhiteSpace(request.ExternalUrl))
            {
                return UnprocessableEntity(new { error = new { code = "VALIDATION_ERROR", message = "外部連結類型必須提供 URL" } });
            }
            if (request.ArticleId.HasValue)
            {
                return UnprocessableEntity(new { error = new { code = "VALIDATION_ERROR", message = "外部連結類型不可提供文章 ID" } });
            }
            if (request.ExternalUrl.Length > 500)
            {
                return UnprocessableEntity(new { error = new { code = "VALIDATION_ERROR", message = "URL 長度不可超過 500 字元" } });
            }
        }
        else
        {
            return UnprocessableEntity(new { error = new { code = "VALIDATION_ERROR", message = "連結類型必須為 Article 或 ExternalUrl" } });
        }

        var menuItem = new MenuItem
        {
            Name = request.Name,
            ParentId = request.ParentId,
            DisplayOrder = request.DisplayOrder ?? 0,
            LinkType = request.LinkType,
            ArticleId = request.ArticleId,
            ExternalUrl = request.ExternalUrl,
            IsVisible = request.IsVisible ?? true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _menuService.CreateMenuAsync(menuItem);

        return CreatedAtAction(nameof(GetMenuItem), new { id = created.Id }, MapMenuItemToResponse(created));
    }

    /// <summary>
    /// 取得單筆選單項目詳情
    /// Get menu item details by ID
    /// GET /api/v1/menus/{id}
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMenuItem(int id)
    {
        var menuItem = await _menuService.GetMenuByIdAsync(id);

        if (menuItem == null)
        {
            return NotFound(new { error = new { code = "NOT_FOUND", message = "找不到選單項目" } });
        }

        return Ok(MapMenuItemToResponse(menuItem));
    }

    /// <summary>
    /// 更新選單項目
    /// Update menu item (admin only)
    /// PUT /api/v1/menus/{id}
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateMenuItem(int id, [FromBody] UpdateMenuItemRequest request)
    {
        var menuItem = await _menuService.GetMenuByIdAsync(id);

        if (menuItem == null)
        {
            return NotFound(new { error = new { code = "NOT_FOUND", message = "找不到選單項目" } });
        }

        // Validate name if provided
        if (request.Name != null && (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length > 100))
        {
            return UnprocessableEntity(new { error = new { code = "VALIDATION_ERROR", message = "名稱長度必須為 1-100 字元" } });
        }

        // Update fields
        if (request.Name != null)
            menuItem.Name = request.Name;

        if (request.ParentId.HasValue || request.ParentId == null)
            menuItem.ParentId = request.ParentId;

        if (request.DisplayOrder.HasValue)
            menuItem.DisplayOrder = request.DisplayOrder.Value;

        if (request.LinkType != null)
        {
            // Validate linkType and corresponding fields
            if (request.LinkType == "Article")
            {
                if (!request.ArticleId.HasValue && menuItem.ArticleId == null)
                {
                    return UnprocessableEntity(new { error = new { code = "VALIDATION_ERROR", message = "文章連結類型必須提供文章 ID" } });
                }
                menuItem.LinkType = request.LinkType;
                if (request.ArticleId.HasValue)
                    menuItem.ArticleId = request.ArticleId;
                menuItem.ExternalUrl = null;
            }
            else if (request.LinkType == "ExternalUrl")
            {
                if (string.IsNullOrWhiteSpace(request.ExternalUrl) && string.IsNullOrWhiteSpace(menuItem.ExternalUrl))
                {
                    return UnprocessableEntity(new { error = new { code = "VALIDATION_ERROR", message = "外部連結類型必須提供 URL" } });
                }
                if (request.ExternalUrl != null && request.ExternalUrl.Length > 500)
                {
                    return UnprocessableEntity(new { error = new { code = "VALIDATION_ERROR", message = "URL 長度不可超過 500 字元" } });
                }
                menuItem.LinkType = request.LinkType;
                if (request.ExternalUrl != null)
                    menuItem.ExternalUrl = request.ExternalUrl;
                menuItem.ArticleId = null;
            }
            else
            {
                return UnprocessableEntity(new { error = new { code = "VALIDATION_ERROR", message = "連結類型必須為 Article 或 ExternalUrl" } });
            }
        }
        else
        {
            // LinkType not changed, but individual fields might be updated
            if (request.ArticleId.HasValue)
                menuItem.ArticleId = request.ArticleId;
            if (request.ExternalUrl != null)
                menuItem.ExternalUrl = request.ExternalUrl;
        }

        if (request.IsVisible.HasValue)
            menuItem.IsVisible = request.IsVisible.Value;

        menuItem.UpdatedAt = DateTime.UtcNow;

        var updated = await _menuService.UpdateMenuAsync(id, menuItem);

        if (updated == null)
        {
            return NotFound(new { error = new { code = "NOT_FOUND", message = "找不到選單項目" } });
        }

        return Ok(MapMenuItemToResponse(updated));
    }

    /// <summary>
    /// 刪除選單項目
    /// Delete menu item (admin only, cascades to children)
    /// DELETE /api/v1/menus/{id}
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteMenuItem(int id)
    {
        var menuItem = await _menuService.GetMenuByIdAsync(id);

        if (menuItem == null)
        {
            return NotFound(new { error = new { code = "NOT_FOUND", message = "找不到選單項目" } });
        }

        await _menuService.DeleteMenuAsync(id);

        return NoContent();
    }

    /// <summary>
    /// 重新排序選單項目
    /// Reorder menu items (admin only)
    /// PUT /api/v1/menus/reorder
    /// </summary>
    [HttpPut("reorder")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ReorderMenus([FromBody] ReorderMenusRequest request)
    {
        if (request.Items == null || !request.Items.Any())
        {
            return UnprocessableEntity(new { error = new { code = "VALIDATION_ERROR", message = "必須提供至少一個選單項目" } });
        }

        var menuOrders = request.Items.ToDictionary(item => item.Id, item => item.DisplayOrder);
        var success = await _menuService.ReorderMenusAsync(menuOrders);

        if (!success)
        {
            return BadRequest(new { error = new { code = "REORDER_FAILED", message = "重新排序失敗" } });
        }

        return Ok(new { message = "選單項目已重新排序" });
    }

    // Helper method to map MenuItem to response object
    private object MapMenuItemToResponse(MenuItem menuItem)
    {
        return new
        {
            id = menuItem.Id,
            name = menuItem.Name,
            parentId = menuItem.ParentId,
            displayOrder = menuItem.DisplayOrder,
            linkType = menuItem.LinkType,
            articleId = menuItem.ArticleId,
            articleTitle = menuItem.Article?.Title,
            externalUrl = menuItem.ExternalUrl,
            isVisible = menuItem.IsVisible,
            children = menuItem.Children?.Select(c => MapMenuItemToResponse(c)).ToList() ?? new List<object>(),
            createdAt = menuItem.CreatedAt,
            updatedAt = menuItem.UpdatedAt
        };
    }
}

public record CreateMenuItemRequest(
    string Name,
    int? ParentId,
    int? DisplayOrder,
    string LinkType,
    int? ArticleId,
    string? ExternalUrl,
    bool? IsVisible);

public record UpdateMenuItemRequest(
    string? Name,
    int? ParentId,
    int? DisplayOrder,
    string? LinkType,
    int? ArticleId,
    string? ExternalUrl,
    bool? IsVisible);

public record ReorderMenusRequest(
    List<MenuOrderItem> Items);

public record MenuOrderItem(
    int Id,
    int DisplayOrder);
