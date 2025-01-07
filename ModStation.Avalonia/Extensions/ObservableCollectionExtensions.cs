using System;
using System.Collections.ObjectModel;
using ModManager.Core.Entities;

namespace ModStation.Avalonia.Extensions;

public static class ObservableCollectionExtensions
{
    public static void Refresh<T>(this ObservableCollection<T> items, T item)
    {
        var index = items.IndexOf(item);
        if (index >= 0)
        {
            items[index] = item;
        }
    }

    public static void RefreshAll<T>(this ObservableCollection<T> items)
    {
        for(var i = 0; i < items.Count; i++) 
            Refresh(items, items[i]);
    }
}
