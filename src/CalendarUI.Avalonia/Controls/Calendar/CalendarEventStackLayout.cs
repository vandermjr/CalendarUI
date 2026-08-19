namespace CalendarUI.Avalonia.Controls.Calendar;

internal readonly record struct CalendarEventStackPlacement<T>(
    T Item,
    int StackIndex,
    int StackCount);

internal static class CalendarEventStackLayout
{
    public static IReadOnlyList<CalendarEventStackPlacement<T>> Calculate<T>(
        IEnumerable<T> items)
    {
        var orderedItems = new List<T>(items);

        var placements = new List<CalendarEventStackPlacement<T>>(
            orderedItems.Count);

        for (int index = 0; index < orderedItems.Count; index++)
        {
            placements.Add(
                new CalendarEventStackPlacement<T>(
                    orderedItems[index],
                    index,
                    orderedItems.Count));
        }

        return placements;
    }

    public static IReadOnlyList<CalendarEventStackPlacement<T>> Promote<T>(
        IReadOnlyList<CalendarEventStackPlacement<T>> placements,
        T item,
        IEqualityComparer<T>? comparer = null)
    {
        comparer ??= EqualityComparer<T>.Default;

        int promotedIndex = -1;

        for (int index = 0; index < placements.Count; index++)
        {
            if (comparer.Equals(placements[index].Item, item))
            {
                promotedIndex = index;
                break;
            }
        }

        if (promotedIndex < 0 || promotedIndex == 0)
            return placements;

        var reordered = new List<T>(placements.Count)
        {
            placements[promotedIndex].Item
        };

        for (int index = 0; index < placements.Count; index++)
        {
            if (index == promotedIndex)
                continue;

            reordered.Add(placements[index].Item);
        }

        return Calculate(reordered);
    }
}