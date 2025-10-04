using Microsoft.EntityFrameworkCore;
using NuclearWeb.Core.Entities;
using NuclearWeb.Core.Interfaces;
using NuclearWeb.Infrastructure.Data;

namespace NuclearWeb.Application.Services;

/// <summary>
/// 文章服務實作
/// Article service implementation
/// </summary>
public class ArticleService : IArticleService
{
    private readonly ApplicationDbContext _context;

    public ArticleService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<ContentArticle> Items, int TotalCount)> GetArticlesAsync(
        int page, int pageSize, string? status = null, int? authorId = null)
    {
        var query = _context.ContentArticles
            .Include(a => a.Author)
            .Include(a => a.Publisher)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(a => a.PublicationStatus == status);
        }

        if (authorId.HasValue)
        {
            query = query.Where(a => a.AuthorId == authorId.Value);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<(IEnumerable<ContentArticle> Items, int TotalCount)> GetPublishedArticlesAsync(int page, int pageSize)
    {
        var now = DateTime.UtcNow;
        var query = _context.ContentArticles
            .Include(a => a.Author)
            .Where(a => a.PublicationStatus == "Published"
                     && a.AvailableFrom <= now
                     && (a.AvailableUntil == null || a.AvailableUntil >= now));

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(a => a.PublishedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<ContentArticle?> GetArticleByIdAsync(int id)
    {
        return await _context.ContentArticles
            .Include(a => a.Author)
            .Include(a => a.Publisher)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<ContentArticle> CreateArticleAsync(ContentArticle article)
    {
        article.PublicationStatus = "Draft";
        _context.ContentArticles.Add(article);
        await _context.SaveChangesAsync();
        return await GetArticleByIdAsync(article.Id) ?? article;
    }

    public async Task<ContentArticle?> UpdateArticleAsync(int id, ContentArticle article)
    {
        var existing = await _context.ContentArticles.FindAsync(id);
        if (existing == null)
        {
            return null;
        }

        existing.Title = article.Title;
        existing.Content = article.Content;

        await _context.SaveChangesAsync();
        return await GetArticleByIdAsync(id);
    }

    public async Task<bool> DeleteArticleAsync(int id)
    {
        var article = await _context.ContentArticles.FindAsync(id);
        if (article == null)
        {
            return false;
        }

        _context.ContentArticles.Remove(article);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ContentArticle?> SubmitArticleAsync(int id)
    {
        var article = await _context.ContentArticles.FindAsync(id);
        if (article == null || article.PublicationStatus != "Draft")
        {
            return null;
        }

        article.PublicationStatus = "PendingApproval";
        await _context.SaveChangesAsync();
        return await GetArticleByIdAsync(id);
    }

    public async Task<ContentArticle?> ApproveArticleAsync(int id, int publishedBy, DateTime availableFrom, DateTime? availableUntil)
    {
        var article = await _context.ContentArticles.FindAsync(id);
        if (article == null || article.PublicationStatus != "PendingApproval")
        {
            return null;
        }

        article.PublicationStatus = "Published";
        article.PublishedBy = publishedBy;
        article.PublishedAt = DateTime.UtcNow;
        article.AvailableFrom = availableFrom;
        article.AvailableUntil = availableUntil;

        await _context.SaveChangesAsync();
        return await GetArticleByIdAsync(id);
    }

    public async Task<ContentArticle?> RejectArticleAsync(int id)
    {
        var article = await _context.ContentArticles.FindAsync(id);
        if (article == null || article.PublicationStatus != "PendingApproval")
        {
            return null;
        }

        article.PublicationStatus = "Rejected";
        await _context.SaveChangesAsync();
        return await GetArticleByIdAsync(id);
    }
}
