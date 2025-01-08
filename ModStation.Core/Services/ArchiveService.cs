using ModManager.Core.Entities;
using ModStation.Core.Interfaces;

namespace ModStation.Core.Services;

public class ArchiveService(IArchiveRepository archiveRepository) : IArchiveService
{
    private readonly IArchiveRepository _archiveRepository = archiveRepository;

    public async Task CreateAsync(Archive archive)
    {
        await _archiveRepository.CreateAsync(archive);
    }

    public async Task UpdateAsync(Archive archive)
    {
        await _archiveRepository.UpdateAsync(archive);
    }

    public async Task DeleteAsync(Archive archive)
    {
        await _archiveRepository.DeleteAsync(archive);
    }

}
