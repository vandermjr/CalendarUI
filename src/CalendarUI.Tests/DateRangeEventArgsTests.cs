using CalendarUI.Avalonia.Controls.MonthView;
using Xunit;

namespace CalendarUI.Tests;

public class DateRangeEventArgsTests
{
    [Fact]
    public void Constructor_NormalizesReversedRange()
    {
        var args = new DateRangeEventArgs(
            new DateTime(2026, 1, 15, 18, 30, 0),
            new DateTime(2026, 1, 10, 8, 15, 0));

        Assert.Equal(new DateTime(2026, 1, 10), args.Start);
        Assert.Equal(new DateTime(2026, 1, 15), args.End);
    }

    [Fact]
    public void TotalDays_IsInclusive()
    {
        var args = new DateRangeEventArgs(
            new DateTime(2026, 1, 10),
            new DateTime(2026, 1, 12));

        Assert.Equal(3, args.TotalDays);
    }

    [Fact]
    public void ShouldUseExpandedView_IsTrueAtMaximumFullDays()
    {
        var args = new DateRangeEventArgs(
            new DateTime(2026, 1, 10),
            new DateTime(2026, 1, 16));

        Assert.Equal(MonthView.MaximumFullDays, args.TotalDays);
        Assert.True(args.ShouldUseExpandedView);
    }

    [Fact]
    public void ShouldUseExpandedView_IsFalseAboveMaximumFullDays()
    {
        var args = new DateRangeEventArgs(
            new DateTime(2026, 1, 10),
            new DateTime(2026, 1, 18));

        Assert.True(args.TotalDays > MonthView.MaximumFullDays);
        Assert.False(args.ShouldUseExpandedView);
    }
}
