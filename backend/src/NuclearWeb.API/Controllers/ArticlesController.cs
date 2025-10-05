using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuclearWeb.Core.Interfaces;
using NuclearWeb.Core.Entities;

namespace NuclearWeb.API.Controllers;

/// <summary>
/// 文章管理控制器
/// Content articles controller
/// </summary>
[ApiController]
[Route("api/v1/articles")]
[Authorize]
public class ArticlesController : ControllerBase
{
    private readonly IArticleService _articleService;

    public ArticlesController(IArticleService articleService)
    {
        _articleService = articleService;
    }

    /// <summary>
    /// 取得文章列表
    /// Get list of articles with pagination
    /// GET /api/v1/articles
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetArticles(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        [FromQuery] int? authorId = null)
    {
        if (page < 1 || pageSize < 1 || pageSize > 100)
        {
            return BadRequest(new { error = new { code = "INVALID_PARAMS", message = "無效的分頁參數" } });
        }

        // Non-admin users can only see published articles
        var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        if (userRole != "Admin")
        {
            status = "Published";
        }

        var (items, totalCount) = await _articleService.GetArticlesAsync(page, pageSize, status, authorId);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return Ok(new
        {
            data = items.Select(a => new
            {
                id = a.Id,
                title = a.Title,
                content = a.Content,
                authorId = a.AuthorId,
                authorName = a.Author?.DisplayName,
                publicationStatus = a.PublicationStatus,
                availableFrom = a.AvailableFrom,
                availableUntil = a.AvailableUntil,
                viewCount = a.ViewCount,
                createdAt = a.CreatedAt,
                updatedAt = a.UpdatedAt,
                publishedAt = a.PublishedAt,
                publishedBy = a.PublishedBy,
                publishedByName = a.Publisher?.DisplayName
            }),
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
    /// 建立新文章
    /// Create a new article (draft status)
    /// POST /api/v1/articles
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateArticle([FromBody] CreateArticleRequest request)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized(new { error = new { code = "INVALID_TOKEN", message = "無效的令牌" } });
        }

        if (string.IsNullOrWhiteSpace(request.Title) || request.Title.Length > 255)
        {
            return UnprocessableEntity(new { error = new { code = "VALIDATION_ERROR", message = "標題長度必須為 1-255 字元" } });
        }

        if (string.IsNullOrWhiteSpace(request.Content))
        {
            return UnprocessableEntity(new { error = new { code = "VALIDATION_ERROR", message = "內容不可為空" } });
        }

        var article = new ContentArticle
        {
            Title = request.Title,
            Content = request.Content,
            AuthorId = userId,
            PublicationStatus = "Draft",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _articleService.CreateArticleAsync(article);

        return CreatedAtAction(nameof(GetArticle), new { id = created.Id }, new
        {
            id = created.Id,
            title = created.Title,
            content = created.Content,
            authorId = created.AuthorId,
            authorName = created.Author?.DisplayName,
            publicationStatus = created.PublicationStatus,
            availableFrom = created.AvailableFrom,
            availableUntil = created.AvailableUntil,
            viewCount = created.ViewCount,
            createdAt = created.CreatedAt,
            updatedAt = created.UpdatedAt,
            publishedAt = created.PublishedAt,
            publishedBy = created.PublishedBy,
            publishedByName = created.Publisher?.DisplayName
        });
    }

    /// <summary>
    /// 取得單筆文章詳情
    /// Get article details by ID
    /// GET /api/v1/articles/{id}
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetArticle(int id)
    {
        var article = await _articleService.GetArticleByIdAsync(id);

        if (article == null)
        {
            return NotFound(new { error = new { code = "NOT_FOUND", message = "找不到文章" } });
        }

        // Non-admin users can only see published articles
        var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        if (userRole != "Admin" && article.PublicationStatus != "Published")
        {
            return NotFound(new { error = new { code = "NOT_FOUND", message = "找不到文章" } });
        }

        return Ok(new
        {
            id = article.Id,
            title = article.Title,
            content = article.Content,
            authorId = article.AuthorId,
            authorName = article.Author?.DisplayName,
            publicationStatus = article.PublicationStatus,
            availableFrom = article.AvailableFrom,
            availableUntil = article.AvailableUntil,
            viewCount = article.ViewCount,
            createdAt = article.CreatedAt,
            updatedAt = article.UpdatedAt,
            publishedAt = article.PublishedAt,
            publishedBy = article.PublishedBy,
            publishedByName = article.Publisher?.DisplayName
        });
    }

    /// <summary>
    /// 更新文章
    /// Update an existing article
    /// PUT /api/v1/articles/{id}
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateArticle(int id, [FromBody] UpdateArticleRequest request)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized(new { error = new { code = "INVALID_TOKEN", message = "無效的令牌" } });
        }

        var article = await _articleService.GetArticleByIdAsync(id);

        if (article == null)
        {
            return NotFound(new { error = new { code = "NOT_FOUND", message = "找不到文章" } });
        }

        // Check if user is author or admin
        var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        if (article.AuthorId != userId && userRole != "Admin")
        {
            return Forbid();
        }

        // Validate input
        if (request.Title != null && (string.IsNullOrWhiteSpace(request.Title) || request.Title.Length > 255))
        {
            return UnprocessableEntity(new { error = new { code = "VALIDATION_ERROR", message = "標題長度必須為 1-255 字元" } });
        }

        if (request.Title != null)
            article.Title = request.Title;

        if (request.Content != null)
            article.Content = request.Content;

        article.UpdatedAt = DateTime.UtcNow;

        var updated = await _articleService.UpdateArticleAsync(id, article);

        if (updated == null)
        {
            return NotFound(new { error = new { code = "NOT_FOUND", message = "找不到文章" } });
        }

        return Ok(new
        {
            id = updated.Id,
            title = updated.Title,
            content = updated.Content,
            authorId = updated.AuthorId,
            authorName = updated.Author?.DisplayName,
            publicationStatus = updated.PublicationStatus,
            availableFrom = updated.AvailableFrom,
            availableUntil = updated.AvailableUntil,
            viewCount = updated.ViewCount,
            createdAt = updated.CreatedAt,
            updatedAt = updated.UpdatedAt,
            publishedAt = updated.PublishedAt,
            publishedBy = updated.PublishedBy,
            publishedByName = updated.Publisher?.DisplayName
        });
    }

    /// <summary>
    /// 刪除文章
    /// Delete an article (admin only, or author for draft articles)
    /// DELETE /api/v1/articles/{id}
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteArticle(int id)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized(new { error = new { code = "INVALID_TOKEN", message = "無效的令牌" } });
        }

        var article = await _articleService.GetArticleByIdAsync(id);

        if (article == null)
        {
            return NotFound(new { error = new { code = "NOT_FOUND", message = "找不到文章" } });
        }

        // Check permissions: admin can delete any, author can delete own drafts
        var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        if (userRole != "Admin" && (article.AuthorId != userId || article.PublicationStatus != "Draft"))
        {
            return Forbid();
        }

        await _articleService.DeleteArticleAsync(id);

        return NoContent();
    }

    /// <summary>
    /// 提交文章供審核
    /// Submit article for approval (Draft → PendingApproval)
    /// POST /api/v1/articles/{id}/submit
    /// </summary>
    [HttpPost("{id}/submit")]
    public async Task<IActionResult> SubmitArticle(int id)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized(new { error = new { code = "INVALID_TOKEN", message = "無效的令牌" } });
        }

        var article = await _articleService.GetArticleByIdAsync(id);

        if (article == null)
        {
            return NotFound(new { error = new { code = "NOT_FOUND", message = "找不到文章" } });
        }

        // Check if user is author or admin
        var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        if (article.AuthorId != userId && userRole != "Admin")
        {
            return Forbid();
        }

        // Validate state transition (only Draft can be submitted)
        if (article.PublicationStatus != "Draft")
        {
            return BadRequest(new
            {
                error = new
                {
                    code = "INVALID_STATE_TRANSITION",
                    message = "只有草稿狀態的文章可以提交審核"
                }
            });
        }

        var updated = await _articleService.SubmitArticleAsync(id);

        if (updated == null)
        {
            return BadRequest(new { error = new { code = "SUBMIT_FAILED", message = "提交失敗" } });
        }

        return Ok(new
        {
            id = updated.Id,
            title = updated.Title,
            content = updated.Content,
            authorId = updated.AuthorId,
            authorName = updated.Author?.DisplayName,
            publicationStatus = updated.PublicationStatus,
            availableFrom = updated.AvailableFrom,
            availableUntil = updated.AvailableUntil,
            viewCount = updated.ViewCount,
            createdAt = updated.CreatedAt,
            updatedAt = updated.UpdatedAt,
            publishedAt = updated.PublishedAt,
            publishedBy = updated.PublishedBy,
            publishedByName = updated.Publisher?.DisplayName
        });
    }

    /// <summary>
    /// 核准文章
    /// Approve article for publication (admin only)
    /// POST /api/v1/articles/{id}/approve
    /// </summary>
    [HttpPost("{id}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ApproveArticle(int id, [FromBody] ApproveArticleRequest request)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized(new { error = new { code = "INVALID_TOKEN", message = "無效的令牌" } });
        }

        var article = await _articleService.GetArticleByIdAsync(id);

        if (article == null)
        {
            return NotFound(new { error = new { code = "NOT_FOUND", message = "找不到文章" } });
        }

        // Validate date range
        if (request.AvailableUntil.HasValue && request.AvailableUntil.Value <= request.AvailableFrom)
        {
            return BadRequest(new
            {
                error = new
                {
                    code = "INVALID_DATE_RANGE",
                    message = "結束時間必須晚於開始時間"
                }
            });
        }

        var updated = await _articleService.ApproveArticleAsync(
            id, userId, request.AvailableFrom, request.AvailableUntil);

        if (updated == null)
        {
            return BadRequest(new { error = new { code = "APPROVE_FAILED", message = "核准失敗" } });
        }

        return Ok(new
        {
            id = updated.Id,
            title = updated.Title,
            content = updated.Content,
            authorId = updated.AuthorId,
            authorName = updated.Author?.DisplayName,
            publicationStatus = updated.PublicationStatus,
            availableFrom = updated.AvailableFrom,
            availableUntil = updated.AvailableUntil,
            viewCount = updated.ViewCount,
            createdAt = updated.CreatedAt,
            updatedAt = updated.UpdatedAt,
            publishedAt = updated.PublishedAt,
            publishedBy = updated.PublishedBy,
            publishedByName = updated.Publisher?.DisplayName
        });
    }

    /// <summary>
    /// 拒絕文章
    /// Reject article (admin only)
    /// POST /api/v1/articles/{id}/reject
    /// </summary>
    [HttpPost("{id}/reject")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RejectArticle(int id, [FromBody] RejectArticleRequest? request)
    {
        var article = await _articleService.GetArticleByIdAsync(id);

        if (article == null)
        {
            return NotFound(new { error = new { code = "NOT_FOUND", message = "找不到文章" } });
        }

        var updated = await _articleService.RejectArticleAsync(id);

        if (updated == null)
        {
            return BadRequest(new { error = new { code = "REJECT_FAILED", message = "拒絕失敗" } });
        }

        return Ok(new
        {
            id = updated.Id,
            title = updated.Title,
            content = updated.Content,
            authorId = updated.AuthorId,
            authorName = updated.Author?.DisplayName,
            publicationStatus = updated.PublicationStatus,
            availableFrom = updated.AvailableFrom,
            availableUntil = updated.AvailableUntil,
            viewCount = updated.ViewCount,
            createdAt = updated.CreatedAt,
            updatedAt = updated.UpdatedAt,
            publishedAt = updated.PublishedAt,
            publishedBy = updated.PublishedBy,
            publishedByName = updated.Publisher?.DisplayName
        });
    }

    /// <summary>
    /// 取得已發布文章
    /// Get published articles (public endpoint)
    /// GET /api/v1/articles/published
    /// </summary>
    [HttpGet("published")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublishedArticles(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (page < 1 || pageSize < 1 || pageSize > 100)
        {
            return BadRequest(new { error = new { code = "INVALID_PARAMS", message = "無效的分頁參數" } });
        }

        var (items, totalCount) = await _articleService.GetPublishedArticlesAsync(page, pageSize);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return Ok(new
        {
            data = items.Select(a => new
            {
                id = a.Id,
                title = a.Title,
                content = a.Content,
                authorName = a.Author?.DisplayName,
                viewCount = a.ViewCount,
                publishedAt = a.PublishedAt
            }),
            pagination = new
            {
                currentPage = page,
                pageSize,
                totalItems = totalCount,
                totalPages
            }
        });
    }
}

public record CreateArticleRequest(
    string Title,
    string Content);

public record UpdateArticleRequest(
    string? Title,
    string? Content);

public record ApproveArticleRequest(
    DateTime AvailableFrom,
    DateTime? AvailableUntil);

public record RejectArticleRequest(
    string? Reason);
