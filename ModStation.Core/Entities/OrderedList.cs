using ModManager.Core.Exceptions;

namespace ModManager.Core.Entities;

public static class ListExtensions
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

public class ModList : List<Mod>
{
    public new void Add(Mod mod)
    {
        Insert(0, mod);
    }

    public new void Insert(int newOrder, Mod mod)
    {
        Remove(mod);
        base.Insert(newOrder, mod);

        FixOrder();
    }

    public new void Remove(Mod mod)
    {
        base.Remove(mod);
        FixOrder();
    }

    public void FixOrder()
    {
        foreach (var mod in this)
        {
            mod.Order = IndexOf(mod);
        }
    }
}
