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
}
