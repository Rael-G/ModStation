using ModManager.Core.Entities;
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

    public void DeleteDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }

    public void DeleteFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    public void CopyDirectory(string sourcePath, string destinationPath)
    {
        foreach (var dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(Path.Combine(destinationPath, Path.GetRelativePath(sourcePath, dirPath)));
        }

        foreach (var filePath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
        {
            var targetFilePath = Path.Combine(destinationPath, Path.GetRelativePath(sourcePath, filePath));
            File.Copy(filePath, targetFilePath, overwrite: true);
        }

        Console.WriteLine($"Copied directory '{sourcePath}' to '{destinationPath}'.");
    }

    public void ExtractArchive(string archivePath, string destinationPath)
    {
        if (!File.Exists(archivePath))
        {
            throw new FileNotFoundException($"Archive '{archivePath}' not found.");
        }

        using var archive = ArchiveFactory.Open(archivePath);
        archive.ExtractToDirectory(destinationPath);

        Console.WriteLine($"Extracted archive '{archivePath}' to '{destinationPath}'.");
    }
}
