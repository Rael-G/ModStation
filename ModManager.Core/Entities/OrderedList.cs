using ModManager.Core.Exceptions;

namespace ModManager.Core.Entities;

public static class ListExtensions
{
    public static ModList ToModList(this IEnumerable<Mod> list)
    {
        var modList = new ModList();
        foreach (var mod in list.OrderBy(a => a.Order))
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
        base.Add(mod);
        mod.Order = Count;
    }

    public new void AddAtIndex(Mod mod, int newOrder)
    {
        base.Remove(mod);

        if (newOrder < mod.Order)
        {
            foreach (var m in this.Where(m => m.Order >= newOrder && m.Order < mod.Order))
            {
                m.Order++;
            }
        }
        else if (newOrder > mod.Order)
        {
            foreach (var m in this.Where(m => m.Order > mod.Order && m.Order <= newOrder))
            {
                m.Order--;
            }
        }

        mod.Order = newOrder;

        base.Add(mod);

        Sort();
    }

    public new void Remove(Mod mod)
    {
        base.Remove(mod);

        foreach (var m in this.Where(m => m.Order > mod.Order))
        {
            m.Order--;
        }
    }

    public void Swap(int orderA, int orderB)
    {
        if (orderA > Count || orderA < 0 || orderB > Count || orderB < 0)
        {
            throw new ModManagerException("The order range is out of bounds.");
        }

        (this[orderA], this[orderB]) = (this[orderB], this[orderA]);
        (this[orderB].Order, this[orderA].Order) = (this[orderA].Order, this[orderB].Order);

        Sort();
    }

    public new void Sort()
    {
        base.Sort((mod1, mod2) => mod1.Order.CompareTo(mod2.Order));
    }
}
