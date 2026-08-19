using CalendarUI.Avalonia.Controls.Calendar;
using Xunit;

namespace CalendarUI.Tests;

public class CalendarItemSegmentsTests
{
    [Fact]
    public void GetSegments_SplitsEventAcrossTwoWeeks()
    {
        var item = new CalendarItem(
            "Evento",
            new DateTime(2026, 8, 10),
            new DateTime(2026, 8, 30));

        var segments = CalendarItemSegments.GetSegments(
            item,
            new DateTime(2026, 8, 9),
            new DateTime(2026, 8, 22)).ToList();

        Assert.Equal(2, segments.Count);

        Assert.Equal(1, segments[0].DayColumn);
        Assert.Equal(6, segments[0].DayCount);
        Assert.True(segments[0].IsFirstSegment);
        Assert.False(segments[0].IsLastSegment);

        Assert.Equal(0, segments[1].DayColumn);
        Assert.Equal(7, segments[1].DayCount);
        Assert.False(segments[1].IsFirstSegment);
        Assert.False(segments[1].IsLastSegment);
    }
}