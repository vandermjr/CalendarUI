namespace CalendarUI.Avalonia.Controls.MonthView;

public class DateRangeEventArgs : EventArgs
{
    public DateTime Start { get; }
    public DateTime End { get; }
    public int TotalDays => (End.Date - Start.Date).Days + 1;
    public bool ShouldUseExpandedView => TotalDays <= MonthView.MaximumFullDays;

    public DateRangeEventArgs(DateTime start, DateTime end)
    {
        if (start <= end)
        {
            Start = start.Date;
            End = end.Date;
        }
        else
        {
            Start = end.Date;
            End = start.Date;
        }
    }
}
