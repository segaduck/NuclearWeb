using Microsoft.EntityFrameworkCore;
using NuclearWeb.Core.Entities;
using NuclearWeb.Core.Interfaces;
using NuclearWeb.Infrastructure.Data;

namespace NuclearWeb.Application.Services;

/// <summary>
/// 檔案服務實作
/// File service implementation
/// </summary>
public class FileService : IFileService
{
    private readonly ApplicationDbContext _context;
    private readonly string _uploadDirectory;
    private const long MaxFileSizeBytes = 104857600; // 100MB

    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx",
        ".png", ".jpg", ".jpeg", ".gif", ".svg",
        ".mp4", ".avi", ".mov", ".wmv",
        ".zip", ".rar", ".7z"
    };

    private static readonly Dictionary<string, string> MimeTypeMap = new(StringComparer.OrdinalIgnoreCase)
    {
        { ".pdf", "application/pdf" },
        { ".doc", "application/msword" },
        { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
        { ".xls", "application/vnd.ms-excel" },
        { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
        { ".ppt", "application/vnd.ms-powerpoint" },
        { ".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
        { ".png", "image/png" },
        { ".jpg", "image/jpeg" },
        { ".jpeg", "image/jpeg" },
        { ".gif", "image/gif" },
        { ".svg", "image/svg+xml" },
        { ".mp4", "video/mp4" },
        { ".avi", "video/x-msvideo" },
        { ".mov", "video/quicktime" },
        { ".wmv", "video/x-ms-wmv" },
        { ".zip", "application/zip" },
        { ".rar", "application/x-rar-compressed" },
        { ".7z", "application/x-7z-compressed" }
    };

    public FileService(ApplicationDbContext context, string uploadDirectory)
    {
        _context = context;
        _uploadDirectory = uploadDirectory;

        // Ensure upload directory exists
        Directory.CreateDirectory(_uploadDirectory);
    }

    public async Task<(IEnumerable<UploadedFile> Items, int TotalCount)> GetFilesAsync(
        int page, int pageSize, string? category = null)
    {
        var query = _context.UploadedFiles
            .Include(f => f.Uploader)
            .AsQueryable();

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(f => f.Category == category);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(f => f.UploadedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<UploadedFile?> GetFileByIdAsync(int id)
    {
        return await _context.UploadedFiles
            .Include(f => f.Uploader)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<UploadedFile?> UploadFileAsync(UploadedFile file, Stream fileStream)
    {
        // Validate file size and type
        if (!ValidateFileSize(file.FileSizeBytes) || !ValidateFileType(file.FileExtension, file.FileType))
        {
            return null;
        }

        // Generate unique stored filename
        file.StoredFileName = $"{Guid.NewGuid()}{file.FileExtension}";
        var filePath = Path.Combine(_uploadDirectory, file.StoredFileName);

        // Save file to disk
        using (var fileStreamOut = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(fileStreamOut);
        }

        _context.UploadedFiles.Add(file);
        await _context.SaveChangesAsync();

        return await GetFileByIdAsync(file.Id);
    }

    public async Task<UploadedFile?> UpdateFileMetadataAsync(int id, UploadedFile file)
    {
        var existing = await _context.UploadedFiles.FindAsync(id);
        if (existing == null)
        {
            return null;
        }

        existing.Description = file.Description;
        existing.Category = file.Category;

        await _context.SaveChangesAsync();
        return await GetFileByIdAsync(id);
    }

    public async Task<bool> DeleteFileAsync(int id)
    {
        var file = await _context.UploadedFiles.FindAsync(id);
        if (file == null)
        {
            return false;
        }

        // Delete physical file
        var filePath = Path.Combine(_uploadDirectory, file.StoredFileName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        // Delete database record
        _context.UploadedFiles.Remove(file);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<(Stream FileStream, UploadedFile Metadata)?> DownloadFileAsync(int id)
    {
        var file = await _context.UploadedFiles.FindAsync(id);
        if (file == null)
        {
            return null;
        }

        var filePath = Path.Combine(_uploadDirectory, file.StoredFileName);
        if (!File.Exists(filePath))
        {
            return null;
        }

        // Increment download count
        file.DownloadCount++;
        await _context.SaveChangesAsync();

        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return (fileStream, file);
    }

    public async Task<IEnumerable<string>> GetCategoriesAsync()
    {
        return await _context.UploadedFiles
            .Where(f => f.Category != null)
            .Select(f => f.Category!)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
    }

    public bool ValidateFileType(string fileExtension, string mimeType)
    {
        if (!AllowedExtensions.Contains(fileExtension))
        {
            return false;
        }

        // Verify MIME type matches extension
        if (MimeTypeMap.TryGetValue(fileExtension, out var expectedMimeType))
        {
            return string.Equals(mimeType, expectedMimeType, StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }

    public bool ValidateFileSize(long fileSizeBytes)
    {
        return fileSizeBytes > 0 && fileSizeBytes <= MaxFileSizeBytes;
    }
}
