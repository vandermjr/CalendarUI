using CalendarUI.Avalonia.Controls.MonthView;

namespace CalendarUI.Tests;

public class MonthViewGridCalculatorTests
{
    [Fact]
    public void GetStartDate_ReturnsSundayAtOrBeforeFirstDayOfMonth()
    {
        var displayDate = new DateTime(2026, 8, 15);

        var result = MonthViewGridCalculator.GetStartDate(displayDate);

        Assert.Equal(new DateTime(2026, 7, 26), result);
        Assert.Equal(DayOfWeek.Sunday, result.DayOfWeek);
    }

    [Fact]
    public void GetDates_ReturnsExactlySixWeeks()
    {
        var dates = MonthViewGridCalculator.GetDates(new DateTime(2026, 8, 15)).ToList();

        Assert.Equal(MonthViewGridCalculator.GridDayCount, dates.Count);
    }

    [Fact]
    public void GetDates_ReturnsConsecutiveDates()
    {
        var dates = MonthViewGridCalculator.GetDates(new DateTime(2026, 8, 15)).ToList();

        Assert.Equal(new DateTime(2026, 7, 26), dates.First());
        Assert.Equal(new DateTime(2026, 9, 5), dates.Last());

        for (int i = 1; i < dates.Count; i++)
        {
            Assert.Equal(dates[i - 1].AddDays(1), dates[i]);
        }
    }
}
