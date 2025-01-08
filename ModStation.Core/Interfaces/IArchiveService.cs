using ModManager.Core.Entities;

namespace ModStation.Core.Interfaces;

public interface IArchiveService
{
    Task CreateAsync(Archive archive);
    Task UpdateAsync(Archive archive);
    Task DeleteAsync(Archive archive);
}
