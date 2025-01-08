using ModManager.Core.Entities;

namespace ModStation.Core.Interfaces;

public interface IModService
{
    Task<Mod> CreateAsync(string modName, string sourcePath, Game game);
    Task DeleteAsync(Mod mod);
    Task UpdateAsync(Mod mod);
    Task EnableAsync(Mod mod);
    Task DisableAsync(Mod mod);
    Task SwapOrderAsync(Mod mod, int order);
}
