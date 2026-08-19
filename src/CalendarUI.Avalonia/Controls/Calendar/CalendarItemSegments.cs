namespace CalendarUI.Avalonia.Controls.Calendar;

internal readonly record struct CalendarItemSegment(
    int WeekRow,
    int DayColumn,
    int DayCount,
    bool IsFirstSegment,
    bool IsLastSegment);

internal static class CalendarItemSegments
{
    public static IEnumerable<CalendarItemSegment> GetSegments(
        ICalendarItem item,
        DateTime gridStart,
        DateTime gridEnd)
    {
        DateTime itemStart = item.DateStart.Date;
        DateTime itemEnd = item.DateEnd.Date;

        if (itemEnd < gridStart || itemStart > gridEnd)
            yield break;

        DateTime visibleStart = itemStart < gridStart
            ? gridStart
            : itemStart;

        DateTime visibleEnd = itemEnd > gridEnd
            ? gridEnd
            : itemEnd;

        int firstWeek = (visibleStart - gridStart).Days / 7;
        int lastWeek = (visibleEnd - gridStart).Days / 7;

        for (int weekRow = firstWeek; weekRow <= lastWeek; weekRow++)
        {
            DateTime weekStart = gridStart.AddDays(weekRow * 7);
            DateTime weekEnd = weekStart.AddDays(6);

            DateTime segmentStart = visibleStart > weekStart
                ? visibleStart
                : weekStart;

            DateTime segmentEnd = visibleEnd < weekEnd
                ? visibleEnd
                : weekEnd;

            yield return new CalendarItemSegment(
                weekRow,
                (segmentStart - weekStart).Days,
                (segmentEnd - segmentStart).Days + 1,
                segmentStart == itemStart,
                segmentEnd == itemEnd);
        }
    }
}