namespace NuclearWeb.Core.Entities;

/// <summary>
/// 內容文章實體 - 表示知識資料庫中的 CMS 文章
/// Represents CMS articles in the knowledge database
/// </summary>
public class ContentArticle
{
    /// <summary>
    /// 主鍵，自動遞增
    /// Primary key, auto-increment
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 文章標題
    /// Article title
    /// Validation: Required, 1-255 chars
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 文章內容（TipTap JSON 格式）
    /// Article body (TipTap JSON format)
    /// Validation: Required, valid JSON
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 作者外鍵
    /// Foreign key to User (author)
    /// </summary>
    public int AuthorId { get; set; }

    /// <summary>
    /// 發布狀態
    /// Current publication status
    /// Values: "Draft", "PendingApproval", "Published", "Rejected"
    /// </summary>
    public string PublicationStatus { get; set; } = "Draft";

    /// <summary>
    /// 可用期間開始時間
    /// Start of availability period
    /// Required for Published status
    /// </summary>
    public DateTime? AvailableFrom { get; set; }

    /// <summary>
    /// 可用期間結束時間
    /// End of availability period
    /// Nullable (null = indefinite)
    /// </summary>
    public DateTime? AvailableUntil { get; set; }

    /// <summary>
    /// 瀏覽次數
    /// Number of views
    /// Default: 0, auto-increment on view
    /// </summary>
    public int ViewCount { get; set; } = 0;

    /// <summary>
    /// 文章建立時間戳記
    /// Article creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 最後修改時間戳記
    /// Last modification timestamp
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// 發布時間戳記
    /// Publication timestamp
    /// Set when status → Published
    /// </summary>
    public DateTime? PublishedAt { get; set; }

    /// <summary>
    /// 核准者外鍵（管理員）
    /// Foreign key to User (admin who approved)
    /// </summary>
    public int? PublishedBy { get; set; }

    // Navigation properties

    /// <summary>
    /// 作者導覽屬性
    /// Author navigation property
    /// </summary>
    public User Author { get; set; } = null!;

    /// <summary>
    /// 發布者導覽屬性
    /// Publisher navigation property
    /// </summary>
    public User? Publisher { get; set; }

    /// <summary>
    /// 選單項目導覽屬性
    /// Menu items linking to this article
    /// </summary>
    public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
}
