using CalendarUI.Avalonia.Controls.Calendar;
using Xunit;

namespace CalendarUI.Tests;

public class CalendarMonthEventStackGroupingTests
{
    [Fact]
    public void Group_SeparatesNonOverlappingEvents()
    {
        var first = CreateItem(
            "A",
            new DateTime(2026, 8, 17),
            new DateTime(2026, 8, 17));

        var second = CreateItem(
            "B",
            new DateTime(2026, 8, 19),
            new DateTime(2026, 8, 19));

        var result = CalendarMonthEventStackGrouping.Group(
            new[]
            {
                Segment(first, dayColumn: 1, dayCount: 1),
                Segment(second, dayColumn: 3, dayCount: 1)
            });

        Assert.Equal(2, result.Count);

        Assert.Single(result[0]);
        Assert.Single(result[1]);
    }

    [Fact]
    public void Group_PlacesOverlappingEventsInSameStack()
    {
        var first = CreateItem(
            "A",
            new DateTime(2026, 8, 17),
            new DateTime(2026, 8, 19));

        var second = CreateItem(
            "B",
            new DateTime(2026, 8, 18),
            new DateTime(2026, 8, 18));

        var result = CalendarMonthEventStackGrouping.Group(
            new[]
            {
                Segment(first, dayColumn: 1, dayCount: 3),
                Segment(second, dayColumn: 2, dayCount: 1)
            });

        var group = Assert.Single(result);

        Assert.Equal(2, group.Count);
    }

    [Fact]
    public void Group_TransitiveOverlapCreatesSingleStack()
    {
        var first = CreateItem(
            "A",
            new DateTime(2026, 8, 17),
            new DateTime(2026, 8, 18));

        var second = CreateItem(
            "B",
            new DateTime(2026, 8, 18),
            new DateTime(2026, 8, 18));

        var third = CreateItem(
            "C",
            new DateTime(2026, 8, 18),
            new DateTime(2026, 8, 19));

        var result = CalendarMonthEventStackGrouping.Group(
            new[]
            {
                Segment(first, dayColumn: 1, dayCount: 2),
                Segment(second, dayColumn: 2, dayCount: 1),
                Segment(third, dayColumn: 2, dayCount: 2)
            });

        var group = Assert.Single(result);

        Assert.Equal(3, group.Count);
    }

    [Fact]
    public void Group_DoesNotMergeSegmentsFromDifferentWeeks()
    {
        var first = CreateItem(
            "A",
            new DateTime(2026, 8, 10),
            new DateTime(2026, 8, 30));

        var second = CreateItem(
            "B",
            new DateTime(2026, 8, 16),
            new DateTime(2026, 8, 17));

        var result = CalendarMonthEventStackGrouping.Group(
            new[]
            {
                Segment(
                    first,
                    weekRow: 0,
                    dayColumn: 1,
                    dayCount: 6),

                Segment(
                    first,
                    weekRow: 1,
                    dayColumn: 0,
                    dayCount: 7),

                Segment(
                    second,
                    weekRow: 1,
                    dayColumn: 0,
                    dayCount: 2)
            });

        Assert.Equal(2, result.Count);

        Assert.Single(result[0]);
        Assert.Equal(2, result[1].Count);
    }

    [Fact]
    public void Group_OrdersByStartDayAndThenBySpan()
    {
        var longEvent = CreateItem(
            "Long",
            new DateTime(2026, 8, 17),
            new DateTime(2026, 8, 19));

        var shortEvent = CreateItem(
            "Short",
            new DateTime(2026, 8, 17),
            new DateTime(2026, 8, 17));

        var result = CalendarMonthEventStackGrouping.Group(
            new[]
            {
                Segment(shortEvent, dayColumn: 1, dayCount: 1),
                Segment(longEvent, dayColumn: 1, dayCount: 3)
            });

        var group = Assert.Single(result);

        Assert.Same(longEvent, group[0].Item);
        Assert.Same(shortEvent, group[1].Item);
    }

    private static CalendarMonthStackItem Segment(
        ICalendarItem item,
        int dayColumn,
        int dayCount,
        int weekRow = 0)
    {
        return new CalendarMonthStackItem(
            item,
            new CalendarItemSegment(
                weekRow,
                dayColumn,
                dayCount,
                IsFirstSegment: true,
                IsLastSegment: true));
    }

    private static CalendarItem CreateItem(
        string title,
        DateTime start,
        DateTime end)
    {
        return new CalendarItem
        {
            Title = title,
            Text = title,
            DateStart = start,
            DateEnd = end
        };
    }
}