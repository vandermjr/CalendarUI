using Avalonia.Media;

namespace CalendarUI.Avalonia.Controls.Calendar;

public interface ICalendarItem
{
    DateTime DateStart { get; set; }
    DateTime DateEnd { get; set; }
    string Text { get; set; }
    Color BackgroundColor { get; set; }
    Color ForeColor { get; set; }
    bool IsSelected { get; set; }
    bool IsLocked { get; set; }

    // Propriedade calculada para o visualizador
    bool IsAllDay { get; }
}
