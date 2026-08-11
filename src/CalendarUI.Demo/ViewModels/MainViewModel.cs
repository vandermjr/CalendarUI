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
    private DateTime _viewStart = DateTime.Today;
    private DateTime _viewEnd = DateTime.Today.AddDays(6);

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
        var today = DateTime.Today;

        Appointments = new ObservableCollection<ICalendarItem>
        {
            new CalendarItem
            {
                Text = "First Release",
                DateStart = today.AddHours(0.5),
                DateEnd = today.AddHours(1.5),
                BackgroundColor = Color.Parse("#27AE60"),
                ForeColor = Colors.White
            },
            new CalendarItem
            {
                Text = "Some appointment",
                DateStart = today.AddDays(1).AddHours(8.5),
                DateEnd = today.AddDays(1).AddHours(10),
                BackgroundColor = Color.Parse("#2980B9"),
                ForeColor = Colors.White
            },
            new CalendarItem
            {
                Text = "Some meeting",
                DateStart = today.AddDays(1).AddHours(10),
                DateEnd = today.AddDays(1).AddHours(10.5),
                BackgroundColor = Color.Parse("#E67E22"),
                ForeColor = Colors.White
            },
            new CalendarItem
            {
                Text = "It will be a boring day",
                DateStart = today.AddDays(2).AddHours(11.5),
                DateEnd = today.AddDays(2).AddHours(12.5),
                BackgroundColor = Color.Parse("#8E44AD"),
                ForeColor = Colors.White
            },
            new CalendarItem
            {
                Text = "Lazy days",
                DateStart = today.AddDays(3),
                DateEnd = today.AddDays(4).AddHours(23).AddMinutes(59),
                BackgroundColor = Color.Parse("#F1C40F"),
                ForeColor = Colors.Black
            }
        };
    }

    public void ToggleSidebarCommand()
    {
        IsSidebarOpen = !IsSidebarOpen;
    }
}
