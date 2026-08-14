using CalendarUI.Avalonia.Controls.Calendar;
using Xunit;

namespace CalendarUI.Tests;

public class CalendarTests
{
    [Fact]
    public void SetDateRange_NormalizesReversedDatesAndUsesDateOnlyValues()
    {
        var calendar = new Calendar();

        calendar.SetDateRange(
            new DateTime(2026, 1, 15, 18, 30, 0),
            new DateTime(2026, 1, 10, 8, 15, 0));

        Assert.Equal(new DateTime(2026, 1, 10), calendar.ViewStart);
        Assert.Equal(new DateTime(2026, 1, 15), calendar.ViewEnd);
    }

    [Fact]
    public void DaysMode_IsExpanded_WhenRangeFitsMaximumFullDays()
    {
        var calendar = new Calendar
        {
            MaximumFullDays = 7
        };

        calendar.SetDateRange(
            new DateTime(2026, 1, 10),
            new DateTime(2026, 1, 16));

        Assert.Equal(CalendarDaysMode.Expanded, calendar.DaysMode);
        Assert.Equal(7, calendar.Days.Count);
    }

    [Fact]
    public void DaysMode_IsShort_WhenRangeExceedsMaximumFullDays()
    {
        var calendar = new Calendar
        {
            MaximumFullDays = 7
        };

        calendar.SetDateRange(
            new DateTime(2026, 1, 10),
            new DateTime(2026, 1, 17));

        Assert.Equal(CalendarDaysMode.Short, calendar.DaysMode);
        Assert.NotEmpty(calendar.Weeks);
        Assert.Equal(calendar.Weeks.Count * 7, calendar.Days.Count);
    }

    [Fact]
    public void Days_ContainsOnlyItemsThatOverlapEachDay()
    {
        var calendar = new Calendar();
        var matchingItem = new CalendarItem(
            "Matching",
            new DateTime(2026, 1, 10, 12, 0, 0),
            new DateTime(2026, 1, 11, 12, 0, 0));
        var outsideItem = new CalendarItem(
            "Outside",
            new DateTime(2026, 1, 20),
            new DateTime(2026, 1, 21));

        calendar.ItemsSource = new[] { matchingItem, outsideItem };
        calendar.SetDateRange(
            new DateTime(2026, 1, 10),
            new DateTime(2026, 1, 11));

        Assert.Equal(2, calendar.Days.Count);
        Assert.All(calendar.Days, day =>
            Assert.Single(day.Items, item => ReferenceEquals(item, matchingItem)));
    }

    [Fact]
    public void DaysMode_IsExpanded_WhenRangeHasExactlySevenDays()
    {
        var calendar = new Calendar
        {
            MaximumFullDays = 7
        };

        calendar.SetDateRange(
            new DateTime(2026, 1, 10),
            new DateTime(2026, 1, 16));

        Assert.Equal(CalendarDaysMode.Expanded, calendar.DaysMode);
        Assert.Equal(7, calendar.Days.Count);
        Assert.Empty(calendar.Weeks);
    }

    [Fact]
    public void DaysMode_IsShort_WhenRangeHasExactlyEightDays()
    {
        var calendar = new Calendar
        {
            MaximumFullDays = 7
        };

        calendar.SetDateRange(
            new DateTime(2026, 1, 10),
            new DateTime(2026, 1, 17));

        Assert.Equal(CalendarDaysMode.Short, calendar.DaysMode);
        Assert.NotEmpty(calendar.Weeks);
        Assert.Equal(calendar.Weeks.Count * 7, calendar.Days.Count);
    }

    [Fact]
    public void ShortMode_StartsGridOnSundayAndEndsOnSaturday()
    {
        var calendar = new Calendar
        {
            MaximumFullDays = 7
        };

        calendar.SetDateRange(
            new DateTime(2026, 1, 10), // sábado
            new DateTime(2026, 1, 17)); // sábado

        Assert.Equal(CalendarDaysMode.Short, calendar.DaysMode);

        Assert.NotEmpty(calendar.Weeks);
        Assert.Equal(new DateTime(2026, 1, 4), calendar.Weeks[0].StartDate);
        Assert.Equal(new DateTime(2026, 1, 10), calendar.Weeks[0].EndDate);

        Assert.Equal(new DateTime(2026, 1, 11), calendar.Weeks[1].StartDate);
        Assert.Equal(new DateTime(2026, 1, 17), calendar.Weeks[1].EndDate);

        Assert.Equal(14, calendar.Days.Count);
    }

    [Fact]
    public void ShortMode_UsesCompleteWeeks_WhenRangeCrossesMonth()
    {
        var calendar = new Calendar
        {
            MaximumFullDays = 7
        };

        calendar.SetDateRange(
            new DateTime(2026, 1, 29),
            new DateTime(2026, 2, 5));

        Assert.Equal(CalendarDaysMode.Short, calendar.DaysMode);
        Assert.Equal(calendar.Weeks.Count * 7, calendar.Days.Count);

        Assert.Equal(
            calendar.Weeks[0].StartDate.AddDays(6),
            calendar.Weeks[0].EndDate);

        Assert.Equal(
            calendar.Weeks[^1].StartDate.AddDays(6),
            calendar.Weeks[^1].EndDate);
    }
}
