using System;

namespace ModStation.Core.Interfaces;

public interface IFileService
{
    void CopyDirectory(string archiveFilePath, string modPath);
    string CreateDirectory(string basePath, string directoryName);
    void CreateDirectory(string? directoryPath);
    void DeleteDirectory(string path);
    void DeleteFile(string filePath);
    void ExtractArchive(string archiveFilePath, string modPath);
    void ValidatePath(string archiveFilePath);
}
