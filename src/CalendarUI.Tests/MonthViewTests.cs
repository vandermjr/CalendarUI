using CalendarUI.Avalonia.Controls.MonthView;
using Xunit;

namespace CalendarUI.Tests;

public class MonthViewTests
{
    [Fact]
    public void Constructor_PopulatesSevenDayNames()
    {
        var monthView = new MonthView();

        Assert.Equal(7, monthView.DayNames.Count);
        Assert.All(monthView.DayNames, dayName => Assert.False(string.IsNullOrWhiteSpace(dayName)));
    }

    [Fact]
    public void DisplayDate_UpdatesDaysGridWithSixWeeks()
    {
        var monthView = new MonthView();

        monthView.DisplayDate = new DateTime(2026, 8, 15);
        monthView.SelectionStart = monthView.SelectionStart.AddDays(1);

        Assert.Equal(new DateTime(2026, 8, 15), monthView.DisplayDate);
        Assert.Equal(42, monthView.Days.Count);
        Assert.Equal(new DateTime(2026, 7, 26), monthView.Days[0].Date);
        Assert.Equal(new DateTime(2026, 9, 5), monthView.Days[^1].Date);
    }

    [Fact]
    public void DisplayDate_ChangingMonth_RebuildsDaysGrid()
    {
        var monthView = new MonthView();

        monthView.DisplayDate = new DateTime(2026, 8, 15);
        monthView.SelectionStart = monthView.SelectionStart.AddDays(1);

        Assert.Equal(new DateTime(2026, 7, 26), monthView.Days[0].Date);

        monthView.DisplayDate = new DateTime(2026, 9, 15);

        Assert.Equal(new DateTime(2026, 8, 30), monthView.Days[0].Date);
        Assert.Equal(new DateTime(2026, 10, 10), monthView.Days[^1].Date);
    }

    [Fact]
    public void DisplayDate_IdentifiesCurrentMonthAndAdjacentDays()
    {
        var monthView = new MonthView();

        monthView.DisplayDate = new DateTime(2026, 8, 15);
        monthView.SelectionStart = monthView.SelectionStart.AddDays(1);

        Assert.Equal(new DateTime(2026, 8, 15), monthView.DisplayDate);

        var previousMonthDay = monthView.Days[0];
        var currentMonthDay = monthView.Days.Single(day => day.Date == new DateTime(2026, 8, 15));
        var nextMonthDay = monthView.Days[^1];

        Assert.False(previousMonthDay.IsCurrentMonth);
        Assert.True(currentMonthDay.IsCurrentMonth);
        Assert.False(nextMonthDay.IsCurrentMonth);
    }

    [Fact]
    public void Selection_UpdatesSelectedDaysAndRaisesNormalizedEvent()
    {
        var monthView = new MonthView
        {
            DisplayDate = new DateTime(2026, 8, 15)
        };

        DateRangeEventArgs? raisedArgs = null;
        monthView.DateRangeSelected += (_, args) => raisedArgs = args;

        monthView.SelectionStart = new DateTime(2026, 8, 20);
        monthView.SelectionEnd = new DateTime(2026, 8, 18);

        Assert.NotNull(raisedArgs);
        Assert.Equal(new DateTime(2026, 8, 18), raisedArgs!.Start);
        Assert.Equal(new DateTime(2026, 8, 20), raisedArgs.End);

        var selectedDates = monthView.Days
            .Where(day => day.IsSelected)
            .Select(day => day.Date)
            .ToList();

        Assert.Equal(3, selectedDates.Count);
        Assert.Equal(
            new[]
            {
                new DateTime(2026, 8, 18),
                new DateTime(2026, 8, 19),
                new DateTime(2026, 8, 20)
            },
            selectedDates);
    }

    [Fact]
    public void Selection_CanSpanAdjacentMonths_WithoutChangingDisplayDate()
    {
        var monthView = new MonthView
        {
            DisplayDate = new DateTime(2026, 8, 15)
        };

        monthView.SelectionStart = new DateTime(2026, 8, 21);
        monthView.SelectionEnd = new DateTime(2026, 9, 5);

        Assert.Equal(new DateTime(2026, 8, 15), monthView.DisplayDate);
        Assert.Equal(new DateTime(2026, 8, 21), monthView.SelectionStart);
        Assert.Equal(new DateTime(2026, 9, 5), monthView.SelectionEnd);

        Assert.All(
            monthView.Days.Where(x => x.Date >= new DateTime(2026, 8, 21) &&
                                      x.Date <= new DateTime(2026, 9, 5)),
            day => Assert.True(day.IsSelected));
    }
}