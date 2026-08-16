using CalendarUI.Avalonia.Controls.Calendar;
using Xunit;

namespace CalendarUI.Tests;

public class CalendarMonthPanelTests
{
    [Fact]
    public void GridStartDate_IsPreviousSunday_WhenRangeStartsOnSaturday()
    {
        var panel = new CalendarMonthPanel
        {
            ViewStart = new DateTime(2026, 1, 10),
            ViewEnd = new DateTime(2026, 1, 17)
        };

        Assert.Equal(new DateTime(2026, 1, 4), panel.GetGridStartDate());
    }

    [Fact]
    public void GridEndDate_IsSaturday_WhenRangeEndsOnSaturday()
    {
        var panel = new CalendarMonthPanel
        {
            ViewStart = new DateTime(2026, 1, 10),
            ViewEnd = new DateTime(2026, 1, 17)
        };

        Assert.Equal(new DateTime(2026, 1, 17), panel.GetGridEndDate());
    }

    [Fact]
    public void GridContainsCompleteWeeks_ForEightDayRange()
    {
        var panel = new CalendarMonthPanel
        {
            ViewStart = new DateTime(2026, 1, 10),
            ViewEnd = new DateTime(2026, 1, 17)
        };

        Assert.Equal(2, panel.GetTotalWeeks());
        Assert.Equal(14, panel.GetTotalDays());
    }

    [Fact]
    public void GridContainsCompleteWeeks_WhenRangeCrossesMonth()
    {
        var panel = new CalendarMonthPanel
        {
            ViewStart = new DateTime(2026, 1, 29),
            ViewEnd = new DateTime(2026, 2, 5)
        };

        Assert.Equal(new DateTime(2026, 1, 25), panel.GetGridStartDate());
        Assert.Equal(new DateTime(2026, 2, 7), panel.GetGridEndDate());
        Assert.Equal(2, panel.GetTotalWeeks());
        Assert.Equal(14, panel.GetTotalDays());
    }

    [Fact]
    public void GridHasOneWeek_WhenRangeFitsInsideOneWeek()
    {
        var panel = new CalendarMonthPanel
        {
            ViewStart = new DateTime(2026, 1, 5),
            ViewEnd = new DateTime(2026, 1, 9)
        };

        Assert.Equal(new DateTime(2026, 1, 4), panel.GetGridStartDate());
        Assert.Equal(new DateTime(2026, 1, 10), panel.GetGridEndDate());
        Assert.Equal(1, panel.GetTotalWeeks());
        Assert.Equal(7, panel.GetTotalDays());
    }

    [Fact]
    public void GridHasOneWeek_WhenRangeContainsSingleDay()
    {
        var panel = new CalendarMonthPanel
        {
            ViewStart = new DateTime(2026, 1, 7),
            ViewEnd = new DateTime(2026, 1, 7)
        };

        Assert.Equal(new DateTime(2026, 1, 4), panel.GetGridStartDate());
        Assert.Equal(new DateTime(2026, 1, 10), panel.GetGridEndDate());
        Assert.Equal(1, panel.GetTotalWeeks());
        Assert.Equal(7, panel.GetTotalDays());
    }
}
