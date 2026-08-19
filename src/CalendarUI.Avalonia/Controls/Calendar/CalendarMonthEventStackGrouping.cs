namespace CalendarUI.Avalonia.Controls.Calendar;

internal readonly record struct CalendarMonthStackItem(
    ICalendarItem Item,
    CalendarItemSegment Segment);

internal static class CalendarMonthEventStackGrouping
{
    public static IReadOnlyList<IReadOnlyList<CalendarMonthStackItem>> Group(
        IEnumerable<CalendarMonthStackItem> items)
    {
        var orderedItems = new List<CalendarMonthStackItem>(items);

        orderedItems.Sort(static (left, right) =>
        {
            int comparison = left.Segment.WeekRow.CompareTo(right.Segment.WeekRow);

            if (comparison != 0)
                return comparison;

            comparison = left.Segment.DayColumn.CompareTo(right.Segment.DayColumn);

            if (comparison != 0)
                return comparison;

            comparison = right.Segment.DayCount.CompareTo(left.Segment.DayCount);

            if (comparison != 0)
                return comparison;

            return left.Item.DateStart.CompareTo(right.Item.DateStart);
        });

        var groups = new List<List<CalendarMonthStackItem>>();

        foreach (var item in orderedItems)
        {
            var matchingGroups = new List<List<CalendarMonthStackItem>>();

            foreach (var group in groups)
            {
                if (group.Exists(existing => Overlaps(existing.Segment, item.Segment)))
                    matchingGroups.Add(group);
            }

            if (matchingGroups.Count == 0)
            {
                groups.Add(
                    new List<CalendarMonthStackItem>
                    {
                        item
                    });

                continue;
            }

            var targetGroup = matchingGroups[0];
            targetGroup.Add(item);

            for (int index = 1; index < matchingGroups.Count; index++)
            {
                var groupToMerge = matchingGroups[index];

                foreach (var mergedItem in groupToMerge)
                    targetGroup.Add(mergedItem);

                groups.Remove(groupToMerge);
            }
        }

        return groups;
    }

    private static bool Overlaps(
        CalendarItemSegment left,
        CalendarItemSegment right)
    {
        if (left.WeekRow != right.WeekRow)
            return false;

        int leftStart = left.DayColumn;
        int leftEnd = left.DayColumn + left.DayCount - 1;

        int rightStart = right.DayColumn;
        int rightEnd = right.DayColumn + right.DayCount - 1;

        return leftStart <= rightEnd &&
               rightStart <= leftEnd;
    }
}