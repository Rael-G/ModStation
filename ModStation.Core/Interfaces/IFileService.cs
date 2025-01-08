namespace ModStation.Core.Interfaces;

public interface IFileService
{
    Task CopyDirectoryAsync(string archiveFilePath, string modPath);
    string CreateDirectory(string basePath, string directoryName);
    void CreateDirectory(string? directoryPath);
    Task DeleteDirectoryAsync(string path, bool recursive = true);
    Task DeleteFileAsync(string filePath);
    Task ExtractArchiveAsync(string archiveFilePath, string modPath);
    void ValidatePath(string archiveFilePath);
}
