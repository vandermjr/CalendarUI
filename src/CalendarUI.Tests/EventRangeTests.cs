using CalendarUI.Avalonia.Controls.Calendar;
using Xunit;

namespace CalendarUI.Tests;

public class EventRangeTests
{
    [Fact]
    public void FromAndTo_StoreDateOnlyRange()
    {
        var range = EventRange
            .From(2026, 8, 10)
            .To(2026, 8, 30);

        Assert.Equal(new DateTime(2026, 8, 10), range.Start);
        Assert.Equal(new DateTime(2026, 8, 30), range.End);
    }

    [Fact]
    public void FromAndTo_StoreDateTimeRange()
    {
        var range = EventRange
            .From(2026, 8, 10, 8, 30)
            .To(2026, 8, 10, 10, 0);

        Assert.Equal(new DateTime(2026, 8, 10, 8, 30, 0), range.Start);
        Assert.Equal(new DateTime(2026, 8, 10, 10, 0, 0), range.End);
    }

    [Fact]
    public void Constructor_RejectsEndBeforeStart()
    {
        Assert.Throws<ArgumentException>(() =>
            EventRange
                .From(2026, 8, 30)
                .To(2026, 8, 10));
    }
}
