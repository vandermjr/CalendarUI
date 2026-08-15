namespace CalendarUI.Avalonia.Controls.MonthView;

internal static class MonthViewGridCalculator
{
    public const int GridDayCount = 42;

    public static DateTime GetStartDate(DateTime displayDate)
    {
        var firstDayOfMonth = new DateTime(displayDate.Year, displayDate.Month, 1);
        return firstDayOfMonth.AddDays(-(int)firstDayOfMonth.DayOfWeek);
    }

    public static IEnumerable<DateTime> GetDates(DateTime displayDate)
    {
        var startDate = GetStartDate(displayDate);

        for (int i = 0; i < GridDayCount; i++)
        {
            yield return startDate.AddDays(i);
        }
    }
}
