namespace CalendarUI.Avalonia.Controls.MonthView;

public class MonthViewDayViewModel
{
    public DateTime Date { get; set; }
    public int DayNumber { get; set; }
    public bool IsCurrentMonth { get; set; }
    public bool IsSelected { get; set; }
    public bool IsToday { get; set; }
}
