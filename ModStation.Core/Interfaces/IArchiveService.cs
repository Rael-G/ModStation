using System;
using ModManager.Core.Entities;

namespace ModStation.Core.Interfaces;

public interface IArchiveService
{
    void Create(Archive archive);
    void Update(Archive archive);
    void Delete(Archive archive);
}
