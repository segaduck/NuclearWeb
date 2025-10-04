using NuclearWeb.Core.Entities;

namespace NuclearWeb.Core.Interfaces;

/// <summary>
/// 文章服務介面
/// Article service interface
/// </summary>
public interface IArticleService
{
    /// <summary>
    /// 取得所有文章（分頁）
    /// Get all articles with pagination
    /// </summary>
    /// <param name="page">頁碼 / Page number (1-based)</param>
    /// <param name="pageSize">每頁筆數 / Page size</param>
    /// <param name="status">狀態篩選 / Optional status filter</param>
    /// <param name="authorId">作者 ID 篩選 / Optional author ID filter</param>
    /// <returns>Paginated list of articles</returns>
    Task<(IEnumerable<ContentArticle> Items, int TotalCount)> GetArticlesAsync(
        int page, int pageSize, string? status = null, int? authorId = null);

    /// <summary>
    /// 取得已發布的文章（前端公開）
    /// Get published articles (public frontend)
    /// </summary>
    /// <param name="page">頁碼 / Page number (1-based)</param>
    /// <param name="pageSize">每頁筆數 / Page size</param>
    /// <returns>Paginated list of published and available articles</returns>
    Task<(IEnumerable<ContentArticle> Items, int TotalCount)> GetPublishedArticlesAsync(int page, int pageSize);

    /// <summary>
    /// 取得單筆文章
    /// Get article by ID
    /// </summary>
    /// <param name="id">文章 ID / Article ID</param>
    /// <returns>Article if found; null otherwise</returns>
    Task<ContentArticle?> GetArticleByIdAsync(int id);

    /// <summary>
    /// 建立文章
    /// Create article
    /// </summary>
    /// <param name="article">文章資料 / Article data</param>
    /// <returns>Created article</returns>
    Task<ContentArticle> CreateArticleAsync(ContentArticle article);

    /// <summary>
    /// 更新文章
    /// Update article
    /// </summary>
    /// <param name="id">文章 ID / Article ID</param>
    /// <param name="article">更新資料 / Updated data</param>
    /// <returns>Updated article if successful; null if not found</returns>
    Task<ContentArticle?> UpdateArticleAsync(int id, ContentArticle article);

    /// <summary>
    /// 刪除文章
    /// Delete article
    /// </summary>
    /// <param name="id">文章 ID / Article ID</param>
    /// <returns>True if deleted; false if not found</returns>
    Task<bool> DeleteArticleAsync(int id);

    /// <summary>
    /// 提交文章供審核
    /// Submit article for approval
    /// </summary>
    /// <param name="id">文章 ID / Article ID</param>
    /// <returns>Updated article if successful; null if not found or invalid state</returns>
    Task<ContentArticle?> SubmitArticleAsync(int id);

    /// <summary>
    /// 核准文章
    /// Approve article (admin only)
    /// </summary>
    /// <param name="id">文章 ID / Article ID</param>
    /// <param name="publishedBy">核准者 ID / Publisher user ID</param>
    /// <param name="availableFrom">可用開始時間 / Available from</param>
    /// <param name="availableUntil">可用結束時間 / Available until (null = indefinite)</param>
    /// <returns>Updated article if successful; null if not found or invalid state</returns>
    Task<ContentArticle?> ApproveArticleAsync(int id, int publishedBy, DateTime availableFrom, DateTime? availableUntil);

    /// <summary>
    /// 拒絕文章
    /// Reject article (admin only)
    /// </summary>
    /// <param name="id">文章 ID / Article ID</param>
    /// <returns>Updated article if successful; null if not found or invalid state</returns>
    Task<ContentArticle?> RejectArticleAsync(int id);
}
