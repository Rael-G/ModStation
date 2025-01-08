using System;

namespace ModStation.Core.Interfaces;

public interface IBaseRepository
{
    public IContext Context { get; set; }
}
