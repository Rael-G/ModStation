using ModStation.Core.Interfaces;

namespace ModStation.Core.Repositories;

public class BaseRepository(IContext context) : IBaseRepository
{
    public IContext Context { get; set; } = context;
}
