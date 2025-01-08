using ModManager.Core.Entities;
using ModManager.Core.Repositories;
using ModStation.Core.Interfaces;

namespace ModStation.Core.Services;

public class ArchivesService(IArchiveRepository archiveRepository)
{
    private readonly IArchiveRepository _archiveRepository = archiveRepository;

    public void Create(Archive archive)
    {
        _archiveRepository.Create(archive);
    }

    public void Update(Archive archive)
    {
        _archiveRepository.Update(archive);
    }

    public void Delete(Archive archive)
    {
        _archiveRepository.Delete(archive);
    }

    

}
