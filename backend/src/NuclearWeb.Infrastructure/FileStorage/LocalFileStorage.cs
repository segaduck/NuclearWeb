using NuclearWeb.Core.Interfaces;

namespace NuclearWeb.Infrastructure.FileStorage;

/// <summary>
/// 本地磁碟檔案儲存實作
/// Local disk file storage implementation
/// </summary>
public class LocalFileStorage : IFileStorage
{
    private readonly string _uploadPath;

    public LocalFileStorage(string uploadPath)
    {
        _uploadPath = uploadPath;

        // Ensure upload directory exists
        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }
    }

    /// <summary>
    /// 儲存檔案到本地磁碟
    /// Store file to local disk
    /// </summary>
    public async Task<string> SaveFileAsync(string fileName, Stream fileStream)
    {
        var filePath = Path.Combine(_uploadPath, fileName);

        // Ensure directory exists (in case of subdirectories)
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using var outputStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true);
        await fileStream.CopyToAsync(outputStream);

        return filePath;
    }

    /// <summary>
    /// 讀取檔案從本地磁碟
    /// Read file from local disk
    /// </summary>
    public Task<Stream?> GetFileAsync(string fileName)
    {
        var filePath = Path.Combine(_uploadPath, fileName);

        if (!File.Exists(filePath))
        {
            return Task.FromResult<Stream?>(null);
        }

        // Return FileStream that will be disposed by the consumer
        Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
        return Task.FromResult<Stream?>(stream);
    }

    /// <summary>
    /// 刪除檔案從本地磁碟
    /// Delete file from local disk
    /// </summary>
    public Task<bool> DeleteFileAsync(string fileName)
    {
        var filePath = Path.Combine(_uploadPath, fileName);

        if (!File.Exists(filePath))
        {
            return Task.FromResult(false);
        }

        try
        {
            File.Delete(filePath);
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    /// <summary>
    /// 檢查檔案是否存在
    /// Check if file exists
    /// </summary>
    public Task<bool> FileExistsAsync(string fileName)
    {
        var filePath = Path.Combine(_uploadPath, fileName);
        return Task.FromResult(File.Exists(filePath));
    }
}
