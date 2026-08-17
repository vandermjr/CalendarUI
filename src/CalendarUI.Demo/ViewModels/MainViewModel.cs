using System;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Media;
using Avalonia.Styling;
using CalendarUI.Avalonia.Controls.Calendar;

namespace CalendarUI.Demo.ViewModels;

public class MainViewModel : ViewModelBase
{
    private bool _isSidebarOpen = true;
    private bool _isDarkTheme;
    private DateTime _viewStart = new(2026, 8, 16);
    private DateTime _viewEnd = new(2026, 8, 22);

    public ObservableCollection<ICalendarItem> Appointments { get; }

    public DateTime ViewStart
    {
        get => _viewStart;
        set
        {
            if (_viewStart != value)
            {
                _viewStart = value;
                OnPropertyChanged();
            }
        }
    }

    public DateTime ViewEnd
    {
        get => _viewEnd;
        set
        {
            if (_viewEnd != value)
            {
                _viewEnd = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsSidebarOpen
    {
        get => _isSidebarOpen;
        set
        {
            if (_isSidebarOpen != value)
            {
                _isSidebarOpen = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsDarkTheme
    {
        get => _isDarkTheme;
        set
        {
            if (_isDarkTheme != value)
            {
                _isDarkTheme = value;
                OnPropertyChanged();

                if (Application.Current != null)
                {
                    Application.Current.RequestedThemeVariant = value
                        ? ThemeVariant.Dark
                        : ThemeVariant.Light;
                }
            }
        }
    }

    public MainViewModel()
    {
        Appointments = new ObservableCollection<ICalendarItem>
        {
            new CalendarItem
            {
                Text = "Evento de um dia",
                Range = EventRange
                    .From(2026, 8, 16, 9, 0)
                    .To(2026, 8, 16, 10, 0),
                BackgroundColor = Color.Parse("#27AE60"),
                ForeColor = Colors.White
            },

            new CalendarItem
            {
                Text = "Evento de vários dias",
                Range = EventRange
                    .From(2026, 8, 17)
                    .To(2026, 8, 19),
                BackgroundColor = Color.Parse("#2980B9"),
                ForeColor = Colors.White
            },

            new CalendarItem
            {
                Text = "Falta data antes",
                Range = EventRange
                    .From(2026, 8, 12)
                    .To(2026, 8, 17),
                BackgroundColor = Color.Parse("#C0392B"),
                ForeColor = Colors.White
            },

            new CalendarItem
            {
                Text = "Falta data depois",
                Range = EventRange
                    .From(2026, 8, 21)
                    .To(2026, 8, 26),
                BackgroundColor = Color.Parse("#16A085"),
                ForeColor = Colors.White
            },

            new CalendarItem
            {
                Text = "Falta data antes e depois",
                Range = EventRange
                    .From(2026, 8, 10)
                    .To(2026, 8, 30),
                BackgroundColor = Color.Parse("#E74C3C"),
                ForeColor = Colors.White
            }
        };
    }

    public void ToggleSidebarCommand()
    {
        IsSidebarOpen = !IsSidebarOpen;
    }
}
