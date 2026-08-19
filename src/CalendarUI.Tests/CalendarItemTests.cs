using CalendarUI.Avalonia.Controls.Calendar;
using Xunit;

namespace CalendarUI.Tests;

public class CalendarItemTests
{
    [Fact]
    public void IsAllDay_ReturnsTrue_WhenDurationIsAtLeast24Hours()
    {
        var start = new DateTime(2026, 1, 10, 9, 0, 0);
        var item = new CalendarItem("Event", start, start.AddHours(24));

        Assert.True(item.IsAllDay);
    }

    [Fact]
    public void IsAllDay_ReturnsTrue_WhenStartAndEndAreBothMidnight()
    {
        var midnight = new DateTime(2026, 1, 10);
        var item = new CalendarItem("Event", midnight, midnight);

        Assert.True(item.IsAllDay);
    }

    [Fact]
    public void IsAllDay_ReturnsFalse_WhenDurationIsLessThan24HoursAndTimesAreNotBothMidnight()
    {
        var start = new DateTime(2026, 1, 10, 1, 0, 0);
        var end = new DateTime(2026, 1, 11, 0, 0, 0);
        var item = new CalendarItem("Event", start, end);

        Assert.False(item.IsAllDay);
    }

    [Fact]
    public void Range_UpdatesDateStartAndDateEnd()
    {
        var item = new CalendarItem();

        item.Range = EventRange
            .From(2026, 8, 10, 8, 30)
            .To(2026, 8, 10, 10, 0);

        Assert.Equal(new DateTime(2026, 8, 10, 8, 30, 0), item.DateStart);
        Assert.Equal(new DateTime(2026, 8, 10, 10, 0, 0), item.DateEnd);
    }

    [Fact]
    public void Title_CanBeSet()
    {
        var item = new CalendarItem();

        item.Title = "Reunião de planejamento";

        Assert.Equal("Reunião de planejamento", item.Title);
    }
}
