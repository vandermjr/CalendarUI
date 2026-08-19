using System;
using Avalonia.Media;

namespace CalendarUI.Avalonia.Controls.Calendar;

public class CalendarItem : ICalendarItem
{
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public Color BackgroundColor { get; set; } = Colors.DodgerBlue;
    public Color ForeColor { get; set; } = Colors.White;
    public bool IsSelected { get; set; }
    public bool IsLocked { get; set; }

    /// <summary>
    /// Identifica se o compromisso é de dia inteiro ou se dura mais de 24h
    /// </summary>
    public bool IsAllDay => (DateEnd - DateStart).TotalHours >= 24 ||
                            (DateStart.TimeOfDay == TimeSpan.Zero && DateEnd.TimeOfDay == TimeSpan.Zero);

    public EventRange Range
    {
        get => new(DateStart, DateEnd);
        set
        {
            DateStart = value.Start;
            DateEnd = value.End;
        }
    }

    // Construtor vazio
    public CalendarItem()
    {
        DateStart = DateTime.Now;
        DateEnd = DateTime.Now.AddHours(1);
    }

    // Construtor Básico
    public CalendarItem(string text, DateTime start, DateTime end)
    {
        Text = text;
        DateStart = start;
        DateEnd = end;
    }

    // Construtor Completo (Opcional, útil para testes e demos)
    public CalendarItem(string text, DateTime start, DateTime end, Color bgColor, Color foreColor)
        : this(text, start, end)
    {
        BackgroundColor = bgColor;
        ForeColor = foreColor;
    }
}
