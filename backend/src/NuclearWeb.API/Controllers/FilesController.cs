using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuclearWeb.Core.Interfaces;
using NuclearWeb.Core.Entities;

namespace NuclearWeb.API.Controllers;

/// <summary>
/// 檔案管理控制器
/// File upload, download, and management controller
/// </summary>
[ApiController]
[Route("api/v1/files")]
[Authorize]
public class FilesController : ControllerBase
{
    private readonly IFileService _fileService;
    private const long MaxFileSize = 104857600; // 100MB

    public FilesController(IFileService fileService)
    {
        _fileService = fileService;
    }

    /// <summary>
    /// 取得上傳檔案列表
    /// Get paginated list of uploaded files
    /// GET /api/v1/files
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetFiles(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? category = null,
        [FromQuery] string? fileType = null,
        [FromQuery] int? uploadedBy = null)
    {
        if (page < 1 || pageSize < 1 || pageSize > 100)
        {
            return BadRequest(new { error = new { code = "INVALID_PARAMS", message = "無效的分頁參數" } });
        }

        var (items, totalCount) = await _fileService.GetFilesAsync(page, pageSize, category);

        // Apply additional filters
        if (!string.IsNullOrEmpty(fileType))
        {
            items = items.Where(f => f.FileType == fileType);
        }

        if (uploadedBy.HasValue)
        {
            items = items.Where(f => f.UploadedBy == uploadedBy.Value);
        }

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return Ok(new
        {
            data = items.Select(f => new
            {
                id = f.Id,
                fileName = f.FileName,
                fileType = f.FileType,
                fileExtension = f.FileExtension,
                fileSizeBytes = f.FileSizeBytes,
                fileSizeMB = Math.Round(f.FileSizeBytes / 1024.0 / 1024.0, 2),
                uploadedBy = f.UploadedBy,
                uploaderName = f.Uploader?.DisplayName,
                uploadedAt = f.UploadedAt,
                description = f.Description,
                category = f.Category,
                downloadCount = f.DownloadCount
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
    /// 上傳檔案
    /// Upload a new file (admin only)
    /// POST /api/v1/files
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [RequestSizeLimit(MaxFileSize)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxFileSize)]
    public async Task<IActionResult> UploadFile([FromForm] IFormFile file, [FromForm] string? description, [FromForm] string? category)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized(new { error = new { code = "INVALID_TOKEN", message = "無效的令牌" } });
        }

        if (file == null || file.Length == 0)
        {
            return BadRequest(new { error = new { code = "NO_FILE", message = "未提供檔案" } });
        }

        // Validate file size
        if (!_fileService.ValidateFileSize(file.Length))
        {
            return BadRequest(new
            {
                error = new
                {
                    code = "FILE_TOO_LARGE",
                    message = "檔案大小超過 100MB 限制",
                    details = new
                    {
                        maxSizeBytes = MaxFileSize,
                        uploadedSizeBytes = file.Length
                    }
                }
            });
        }

        // Get file extension
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant().TrimStart('.');

        // Validate file type
        if (!_fileService.ValidateFileType(extension, file.ContentType))
        {
            return BadRequest(new
            {
                error = new
                {
                    code = "INVALID_FILE_TYPE",
                    message = "不允許的檔案類型",
                    details = new
                    {
                        allowedTypes = new[] { "PDF", "DOC", "DOCX", "XLS", "XLSX", "PPT", "PPTX", "PNG", "JPG", "JPEG", "GIF", "SVG", "MP4", "AVI", "MOV", "WMV", "ZIP", "RAR", "7Z" }
                    }
                }
            });
        }

        // Validate description length
        if (description != null && description.Length > 500)
        {
            return UnprocessableEntity(new { error = new { code = "VALIDATION_ERROR", message = "描述長度不可超過 500 字元" } });
        }

        // Validate category length
        if (category != null && category.Length > 50)
        {
            return UnprocessableEntity(new { error = new { code = "VALIDATION_ERROR", message = "類別長度不可超過 50 字元" } });
        }

        // Create stored filename (UUID + extension)
        var storedFileName = $"{Guid.NewGuid()}.{extension}";

        var uploadedFile = new UploadedFile
        {
            FileName = file.FileName,
            StoredFileName = storedFileName,
            FileType = file.ContentType,
            FileExtension = extension,
            FileSizeBytes = file.Length,
            UploadedBy = userId,
            UploadedAt = DateTime.UtcNow,
            Description = description,
            Category = category,
            DownloadCount = 0
        };

        using var stream = file.OpenReadStream();
        var created = await _fileService.UploadFileAsync(uploadedFile, stream);

        if (created == null)
        {
            return BadRequest(new { error = new { code = "UPLOAD_FAILED", message = "檔案上傳失敗" } });
        }

        return CreatedAtAction(nameof(GetFile), new { id = created.Id }, new
        {
            id = created.Id,
            fileName = created.FileName,
            fileType = created.FileType,
            fileExtension = created.FileExtension,
            fileSizeBytes = created.FileSizeBytes,
            fileSizeMB = Math.Round(created.FileSizeBytes / 1024.0 / 1024.0, 2),
            uploadedBy = created.UploadedBy,
            uploaderName = created.Uploader?.DisplayName,
            uploadedAt = created.UploadedAt,
            description = created.Description,
            category = created.Category,
            downloadCount = created.DownloadCount
        });
    }

    /// <summary>
    /// 取得檔案元資料
    /// Get file metadata by ID
    /// GET /api/v1/files/{id}
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetFile(int id)
    {
        var file = await _fileService.GetFileByIdAsync(id);

        if (file == null)
        {
            return NotFound(new { error = new { code = "NOT_FOUND", message = "找不到檔案" } });
        }

        return Ok(new
        {
            id = file.Id,
            fileName = file.FileName,
            fileType = file.FileType,
            fileExtension = file.FileExtension,
            fileSizeBytes = file.FileSizeBytes,
            fileSizeMB = Math.Round(file.FileSizeBytes / 1024.0 / 1024.0, 2),
            uploadedBy = file.UploadedBy,
            uploaderName = file.Uploader?.DisplayName,
            uploadedAt = file.UploadedAt,
            description = file.Description,
            category = file.Category,
            downloadCount = file.DownloadCount
        });
    }

    /// <summary>
    /// 更新檔案元資料
    /// Update file metadata (description and category only, admin only)
    /// PUT /api/v1/files/{id}
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateFileMetadata(int id, [FromBody] UpdateFileMetadataRequest request)
    {
        var file = await _fileService.GetFileByIdAsync(id);

        if (file == null)
        {
            return NotFound(new { error = new { code = "NOT_FOUND", message = "找不到檔案" } });
        }

        // Validate description length
        if (request.Description != null && request.Description.Length > 500)
        {
            return UnprocessableEntity(new { error = new { code = "VALIDATION_ERROR", message = "描述長度不可超過 500 字元" } });
        }

        // Validate category length
        if (request.Category != null && request.Category.Length > 50)
        {
            return UnprocessableEntity(new { error = new { code = "VALIDATION_ERROR", message = "類別長度不可超過 50 字元" } });
        }

        if (request.Description != null)
            file.Description = request.Description;

        if (request.Category != null)
            file.Category = request.Category;

        var updated = await _fileService.UpdateFileMetadataAsync(id, file);

        if (updated == null)
        {
            return NotFound(new { error = new { code = "NOT_FOUND", message = "找不到檔案" } });
        }

        return Ok(new
        {
            id = updated.Id,
            fileName = updated.FileName,
            fileType = updated.FileType,
            fileExtension = updated.FileExtension,
            fileSizeBytes = updated.FileSizeBytes,
            fileSizeMB = Math.Round(updated.FileSizeBytes / 1024.0 / 1024.0, 2),
            uploadedBy = updated.UploadedBy,
            uploaderName = updated.Uploader?.DisplayName,
            uploadedAt = updated.UploadedAt,
            description = updated.Description,
            category = updated.Category,
            downloadCount = updated.DownloadCount
        });
    }

    /// <summary>
    /// 刪除檔案
    /// Delete an uploaded file (admin only)
    /// DELETE /api/v1/files/{id}
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteFile(int id)
    {
        var file = await _fileService.GetFileByIdAsync(id);

        if (file == null)
        {
            return NotFound(new { error = new { code = "NOT_FOUND", message = "找不到檔案" } });
        }

        await _fileService.DeleteFileAsync(id);

        return NoContent();
    }

    /// <summary>
    /// 下載檔案
    /// Download file content (increments download counter)
    /// GET /api/v1/files/{id}/download
    /// </summary>
    [HttpGet("{id}/download")]
    public async Task<IActionResult> DownloadFile(int id)
    {
        var result = await _fileService.DownloadFileAsync(id);

        if (result == null)
        {
            return NotFound(new { error = new { code = "NOT_FOUND", message = "找不到檔案" } });
        }

        var (fileStream, metadata) = result.Value;

        return File(
            fileStream,
            metadata.FileType,
            metadata.FileName,
            enableRangeProcessing: true
        );
    }

    /// <summary>
    /// 取得所有檔案類別
    /// Get list of all file categories in use
    /// GET /api/v1/files/categories
    /// </summary>
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _fileService.GetCategoriesAsync();

        return Ok(new
        {
            categories = categories.Where(c => !string.IsNullOrEmpty(c)).ToList()
        });
    }
}

public record UpdateFileMetadataRequest(
    string? Description,
    string? Category);
