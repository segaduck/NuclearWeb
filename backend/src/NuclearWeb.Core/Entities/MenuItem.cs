namespace NuclearWeb.Core.Entities;

/// <summary>
/// 選單項目實體 - 表示 CMS 前端的導覽選單項目
/// Represents navigation menu items in the CMS frontend
/// </summary>
public class MenuItem
{
    /// <summary>
    /// 主鍵，自動遞增
    /// Primary key, auto-increment
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 選單項目顯示名稱
    /// Menu item display name
    /// Validation: Required, 1-100 chars
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 父選單項目 ID（自參考）
    /// Parent menu item ID (self-referential)
    /// Nullable (null = top-level)
    /// </summary>
    public int? ParentId { get; set; }

    /// <summary>
    /// 層級內的排序順序
    /// Sort order within level
    /// Default: 0
    /// </summary>
    public int DisplayOrder { get; set; } = 0;

    /// <summary>
    /// 連結類型
    /// Type of link
    /// Values: "Article", "ExternalUrl"
    /// </summary>
    public string LinkType { get; set; } = string.Empty;

    /// <summary>
    /// 連結的文章 ID
    /// Linked article ID
    /// Required if LinkType = "Article"
    /// </summary>
    public int? ArticleId { get; set; }

    /// <summary>
    /// 外部 URL
    /// External URL
    /// Required if LinkType = "ExternalUrl"
    /// </summary>
    public string? ExternalUrl { get; set; }

    /// <summary>
    /// 可見性標記
    /// Visibility flag
    /// Default: true
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// 記錄建立時間戳記
    /// Record creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 最後修改時間戳記
    /// Last modification timestamp
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    // Navigation properties

    /// <summary>
    /// 父選單項目導覽屬性
    /// Parent menu item navigation property
    /// </summary>
    public MenuItem? Parent { get; set; }

    /// <summary>
    /// 子選單項目導覽屬性
    /// Child menu items navigation property
    /// </summary>
    public ICollection<MenuItem> Children { get; set; } = new List<MenuItem>();

    /// <summary>
    /// 連結的文章導覽屬性
    /// Linked article navigation property
    /// </summary>
    public ContentArticle? Article { get; set; }
}
