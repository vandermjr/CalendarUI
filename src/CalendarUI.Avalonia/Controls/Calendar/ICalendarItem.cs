using System;
using Avalonia.Media;

namespace CalendarUI.Avalonia.Controls.Calendar;

public interface ICalendarItem
{
    DateTime DateStart { get; set; }
    DateTime DateEnd { get; set; }

    string Title { get; set; }
    string Text { get; set; }

    Color BackgroundColor { get; set; }
    Color ForeColor { get; set; }
    bool IsSelected { get; set; }
    bool IsLocked { get; set; }

    bool IsAllDay { get; }
}
