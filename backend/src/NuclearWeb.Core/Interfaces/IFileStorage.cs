namespace NuclearWeb.Core.Interfaces;

/// <summary>
/// 檔案儲存介面
/// File storage interface
/// </summary>
public interface IFileStorage
{
    /// <summary>
    /// 儲存檔案
    /// Store file to disk
    /// </summary>
    /// <param name="fileName">儲存的檔案名稱 / Stored file name (UUID + extension)</param>
    /// <param name="fileStream">檔案串流 / File stream</param>
    /// <returns>儲存的完整路徑 / Full path to stored file</returns>
    Task<string> SaveFileAsync(string fileName, Stream fileStream);

    /// <summary>
    /// 讀取檔案
    /// Read file from disk
    /// </summary>
    /// <param name="fileName">儲存的檔案名稱 / Stored file name</param>
    /// <returns>檔案串流 / File stream if found; null otherwise</returns>
    Task<Stream?> GetFileAsync(string fileName);

    /// <summary>
    /// 刪除檔案
    /// Delete file from disk
    /// </summary>
    /// <param name="fileName">儲存的檔案名稱 / Stored file name</param>
    /// <returns>True if deleted; false if not found</returns>
    Task<bool> DeleteFileAsync(string fileName);

    /// <summary>
    /// 檢查檔案是否存在
    /// Check if file exists
    /// </summary>
    /// <param name="fileName">儲存的檔案名稱 / Stored file name</param>
    /// <returns>True if exists; false otherwise</returns>
    Task<bool> FileExistsAsync(string fileName);
}
