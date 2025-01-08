using ModManager.Core.Entities;

namespace ModStation.Core.Interfaces;

public interface IModService
{
    Mod Create(string modName, string sourcePath, Game game);
    void Delete(Mod mod);
    void Enable(Mod mod);
    void Disable(Mod mod);
    void SwapOrder(Mod mod, int order);
}
