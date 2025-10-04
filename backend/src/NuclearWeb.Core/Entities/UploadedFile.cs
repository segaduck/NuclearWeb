namespace NuclearWeb.Core.Entities;

/// <summary>
/// 上傳檔案實體 - 表示上傳至系統供下載的檔案
/// Represents files uploaded to the system for download
/// </summary>
public class UploadedFile
{
    /// <summary>
    /// 主鍵，自動遞增
    /// Primary key, auto-increment
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 原始檔案名稱
    /// Original filename
    /// Validation: Required, 1-255 chars
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// 儲存檔案名稱（UUID 為基礎）
    /// UUID-based storage filename
    /// Unique, auto-generated
    /// </summary>
    public string StoredFileName { get; set; } = string.Empty;

    /// <summary>
    /// MIME 類型
    /// MIME type
    /// Validation: Required, validated
    /// </summary>
    public string FileType { get; set; } = string.Empty;

    /// <summary>
    /// 檔案副檔名
    /// File extension
    /// Validation: Required, validated
    /// </summary>
    public string FileExtension { get; set; } = string.Empty;

    /// <summary>
    /// 檔案大小（位元組）
    /// File size in bytes
    /// Validation: Required, max 104857600 (100MB)
    /// </summary>
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// 上傳者外鍵
    /// Foreign key to User (uploader)
    /// </summary>
    public int UploadedBy { get; set; }

    /// <summary>
    /// 上傳時間戳記
    /// Upload timestamp
    /// </summary>
    public DateTime UploadedAt { get; set; }

    /// <summary>
    /// 檔案描述
    /// File description
    /// Validation: Max 500 chars
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 檔案類別/標籤
    /// File category/tag
    /// Validation: Max 50 chars
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// 下載次數
    /// Number of downloads
    /// Default: 0, auto-increment on download
    /// </summary>
    public int DownloadCount { get; set; } = 0;

    // Navigation properties

    /// <summary>
    /// 上傳者導覽屬性
    /// Uploader navigation property
    /// </summary>
    public User Uploader { get; set; } = null!;
}
