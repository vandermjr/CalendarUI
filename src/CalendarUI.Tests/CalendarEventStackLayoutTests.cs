using CalendarUI.Avalonia.Controls.Calendar;
using Xunit;

namespace CalendarUI.Tests;

public class CalendarEventStackLayoutTests
{
    [Fact]
    public void Calculate_AssignsSequentialStackIndexes()
    {
        var first = CreateItem("A");
        var second = CreateItem("B");
        var third = CreateItem("C");

        var result = CalendarEventStackLayout.Calculate(
            new ICalendarItem[]
            {
                first,
                second,
                third
            });

        Assert.Equal(3, result.Count);

        Assert.Equal(0, result[0].StackIndex);
        Assert.Equal(1, result[1].StackIndex);
        Assert.Equal(2, result[2].StackIndex);

        Assert.All(
            result,
            placement => Assert.Equal(3, placement.StackCount));
    }

    [Fact]
    public void Calculate_PreservesInputOrder()
    {
        var first = CreateItem("A");
        var second = CreateItem("B");

        var result = CalendarEventStackLayout.Calculate(
            new ICalendarItem[]
            {
                first,
                second
            });

        Assert.Same(first, result[0].Item);
        Assert.Same(second, result[1].Item);
    }

    [Fact]
    public void Promote_MovesSelectedItemToTop()
    {
        var first = CreateItem("A");
        var second = CreateItem("B");
        var third = CreateItem("C");

        var initial = CalendarEventStackLayout.Calculate(
            new ICalendarItem[]
            {
                first,
                second,
                third
            });

        var result = CalendarEventStackLayout.Promote(
            initial,
            third);

        Assert.Equal(3, result.Count);
        Assert.Same(third, result[0].Item);
        Assert.Same(first, result[1].Item);
        Assert.Same(second, result[2].Item);

        Assert.Equal(0, result[0].StackIndex);
        Assert.Equal(1, result[1].StackIndex);
        Assert.Equal(2, result[2].StackIndex);
    }

    [Fact]
    public void Promote_WhenItemIsAlreadyTop_PreservesOrder()
    {
        var first = CreateItem("A");
        var second = CreateItem("B");

        var initial = CalendarEventStackLayout.Calculate(
            new ICalendarItem[]
            {
                first,
                second
            });

        var result = CalendarEventStackLayout.Promote(
            initial,
            first);

        Assert.Same(initial[0].Item, result[0].Item);
        Assert.Same(initial[1].Item, result[1].Item);
    }

    [Fact]
    public void Promote_WhenItemDoesNotBelongToStack_PreservesOrder()
    {
        var first = CreateItem("A");
        var second = CreateItem("B");
        var external = CreateItem("C");

        var initial = CalendarEventStackLayout.Calculate(
            new ICalendarItem[]
            {
                first,
                second
            });

        var result = CalendarEventStackLayout.Promote(
            initial,
            external);

        Assert.Equal(2, result.Count);
        Assert.Same(first, result[0].Item);
        Assert.Same(second, result[1].Item);
    }

    private static CalendarItem CreateItem(string title)
    {
        return new CalendarItem
        {
            Title = title,
            Text = title,
            DateStart = new DateTime(2026, 8, 17),
            DateEnd = new DateTime(2026, 8, 17)
        };
    }

    [Fact]
    public void Calculate_SupportsCompositeVisualItems()
    {
        var firstItem = CreateItem("A");
        var secondItem = CreateItem("B");

        var firstSegment = new CalendarItemSegment(
            WeekRow: 0,
            DayColumn: 1,
            DayCount: 3,
            IsFirstSegment: true,
            IsLastSegment: false);

        var secondSegment = new CalendarItemSegment(
            WeekRow: 0,
            DayColumn: 2,
            DayCount: 2,
            IsFirstSegment: true,
            IsLastSegment: true);

        var visualItems = new[]
        {
        (Item: (ICalendarItem)firstItem, Segment: firstSegment),
        (Item: (ICalendarItem)secondItem, Segment: secondSegment)
    };

        var result = CalendarEventStackLayout.Calculate(visualItems);

        Assert.Equal(2, result.Count);

        Assert.Equal(0, result[0].StackIndex);
        Assert.Equal(1, result[1].StackIndex);

        Assert.Same(firstItem, result[0].Item.Item);
        Assert.Same(secondItem, result[1].Item.Item);

        Assert.Equal(1, result[0].Item.Segment.DayColumn);
        Assert.Equal(2, result[1].Item.Segment.DayColumn);
    }
}