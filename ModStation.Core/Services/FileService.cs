using ModStation.Core.Interfaces;
using SharpCompress.Archives;

namespace ModStation.Core.Services;

public class FileService : IFileService
{
    public void ValidatePath(string path)
    {
        if (!Directory.Exists(path))
        {
            throw new ArgumentException($"Path '{path}' does not exist.");
        }
    }

    public string CreateDirectory(string basePath, string directoryName)
    {
        var newPath = Path.Combine(basePath, directoryName);
        CreateDirectory(Path.Combine(basePath, directoryName));
        return newPath;
    }

    public void CreateDirectory(string? directoryPath)
    {
        if (!string.IsNullOrWhiteSpace(directoryPath) && !Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }

    public async Task DeleteDirectoryAsync(string path, bool recursive = true)
    {
        if (Directory.Exists(path))
        {
            await Task.Run(() => Directory.Delete(path, recursive));
        }
    }

    public async Task DeleteFileAsync(string filePath)
    {
        if (File.Exists(filePath))
        {
            await Task.Run(() => File.Delete(filePath));
        }
    }

    public async Task CopyDirectoryAsync(string sourcePath, string destinationPath)
    {
        foreach (var dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            await Task.Run(() => CreateDirectory(Path.Combine(destinationPath, Path.GetRelativePath(sourcePath, dirPath))));
        }

        foreach (var filePath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
        {
            var targetFilePath = Path.Combine(destinationPath, Path.GetRelativePath(sourcePath, filePath));
            await Task.Run(() => File.Copy(filePath, targetFilePath, overwrite: true));
        }
    }

    public async Task ExtractArchiveAsync(string archivePath, string destinationPath)
    {
        if (!File.Exists(archivePath))
        {
            throw new FileNotFoundException($"Archive '{archivePath}' not found.");
        }

        using var archive = ArchiveFactory.Open(archivePath);
        await Task.Run(() => archive.ExtractToDirectory(destinationPath));
    }
}
