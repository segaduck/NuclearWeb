using NuclearWeb.Core.Entities;

namespace NuclearWeb.Core.Interfaces;

/// <summary>
/// 檔案服務介面
/// File service interface
/// </summary>
public interface IFileService
{
    /// <summary>
    /// 取得所有檔案（分頁）
    /// Get all files with pagination
    /// </summary>
    /// <param name="page">頁碼 / Page number (1-based)</param>
    /// <param name="pageSize">每頁筆數 / Page size</param>
    /// <param name="category">類別篩選 / Optional category filter</param>
    /// <returns>Paginated list of files</returns>
    Task<(IEnumerable<UploadedFile> Items, int TotalCount)> GetFilesAsync(
        int page, int pageSize, string? category = null);

    /// <summary>
    /// 取得單筆檔案元資料
    /// Get file metadata by ID
    /// </summary>
    /// <param name="id">檔案 ID / File ID</param>
    /// <returns>File metadata if found; null otherwise</returns>
    Task<UploadedFile?> GetFileByIdAsync(int id);

    /// <summary>
    /// 上傳檔案
    /// Upload file
    /// </summary>
    /// <param name="file">檔案資料 / File data</param>
    /// <param name="fileStream">檔案串流 / File stream</param>
    /// <returns>Created file metadata if successful; null if validation failed</returns>
    Task<UploadedFile?> UploadFileAsync(UploadedFile file, Stream fileStream);

    /// <summary>
    /// 更新檔案元資料
    /// Update file metadata
    /// </summary>
    /// <param name="id">檔案 ID / File ID</param>
    /// <param name="file">更新資料 / Updated data</param>
    /// <returns>Updated file metadata if successful; null if not found</returns>
    Task<UploadedFile?> UpdateFileMetadataAsync(int id, UploadedFile file);

    /// <summary>
    /// 刪除檔案
    /// Delete file (metadata and physical file)
    /// </summary>
    /// <param name="id">檔案 ID / File ID</param>
    /// <returns>True if deleted; false if not found</returns>
    Task<bool> DeleteFileAsync(int id);

    /// <summary>
    /// 下載檔案（遞增下載次數）
    /// Download file (increment download count)
    /// </summary>
    /// <param name="id">檔案 ID / File ID</param>
    /// <returns>File stream and metadata if found; null otherwise</returns>
    Task<(Stream FileStream, UploadedFile Metadata)?> DownloadFileAsync(int id);

    /// <summary>
    /// 取得所有檔案類別
    /// Get all file categories
    /// </summary>
    /// <returns>List of unique categories</returns>
    Task<IEnumerable<string>> GetCategoriesAsync();

    /// <summary>
    /// 驗證檔案類型
    /// Validate file type
    /// </summary>
    /// <param name="fileExtension">副檔名 / File extension</param>
    /// <param name="mimeType">MIME 類型 / MIME type</param>
    /// <returns>True if valid; false otherwise</returns>
    bool ValidateFileType(string fileExtension, string mimeType);

    /// <summary>
    /// 驗證檔案大小
    /// Validate file size
    /// </summary>
    /// <param name="fileSizeBytes">檔案大小（位元組）/ File size in bytes</param>
    /// <returns>True if valid; false otherwise</returns>
    bool ValidateFileSize(long fileSizeBytes);
}
