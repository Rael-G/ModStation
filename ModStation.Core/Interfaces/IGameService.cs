using System;
using ModManager.Core.Entities;

namespace ModStation.Core.Interfaces;

public interface IGameService
{
    Game Create(string gamePath, string name);
    void Delete(Game game);
}
