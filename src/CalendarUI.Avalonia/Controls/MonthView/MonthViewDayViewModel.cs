using Avalonia.Input;

namespace CalendarUI.Avalonia.Controls.MonthView;

public class MonthViewDayViewModel
{
    private readonly MonthView _owner;

    public DateTime Date { get; set; }
    public int DayNumber { get; set; }
    public bool IsCurrentMonth { get; set; }
    public bool IsSelected { get; set; }
    public bool IsToday { get; set; }

    public MonthViewDayViewModel(MonthView owner)
    {
        _owner = owner;
    }

    public void OnPointerPressed(PointerPressedEventArgs e) => _owner.OnDayPointerPressed(this, e);
    public void OnPointerEntered(PointerEventArgs e) => _owner.OnDayPointerEntered(this, e);
}
