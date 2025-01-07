using ModManager.Core.Entities;

namespace ModStation.Core.Extensions;

public static class IEnumerableExtensions
{
    public static ModList ToModList(this IEnumerable<Mod> list)
    {
        var modList = new ModList();
        foreach (var mod in list.OrderByDescending(a => a.Order))
        {
            modList.Add(mod);
        }
        return modList;
    }
}
