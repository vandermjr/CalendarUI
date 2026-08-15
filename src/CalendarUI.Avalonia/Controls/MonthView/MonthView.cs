using System.Collections.ObjectModel;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace CalendarUI.Avalonia.Controls.MonthView;

public class MonthView : TemplatedControl
{
    public const int MaximumFullDays = 7;

    public event EventHandler<DateRangeEventArgs>? DateRangeSelected;

    public static readonly DirectProperty<MonthView, string> DisplayDateFormattedProperty =
        AvaloniaProperty.RegisterDirect<MonthView, string>(
            nameof(DisplayDateFormatted),
            o => o.DisplayDateFormatted);

    public static readonly StyledProperty<DateTime> DisplayDateProperty =
        AvaloniaProperty.Register<MonthView, DateTime>(nameof(DisplayDate), DateTime.Today);

    public static readonly StyledProperty<DateTime> SelectionStartProperty =
        AvaloniaProperty.Register<MonthView, DateTime>(nameof(SelectionStart), DateTime.Today);

    public static readonly StyledProperty<DateTime> SelectionEndProperty =
        AvaloniaProperty.Register<MonthView, DateTime>(nameof(SelectionEnd), DateTime.Today);

    public DateTime DisplayDate
    {
        get => GetValue(DisplayDateProperty);
        set => SetValue(DisplayDateProperty, value);
    }

    public string DisplayDateFormatted
    {
        get
        {
            string formatted = DisplayDate.ToString("MMMM yyyy", CultureInfo.CurrentCulture);
            if (string.IsNullOrEmpty(formatted)) return formatted;

            return char.ToUpper(formatted[0], CultureInfo.CurrentCulture) + formatted.Substring(1);
        }
    }

    public DateTime SelectionStart
    {
        get => GetValue(SelectionStartProperty);
        set => SetValue(SelectionStartProperty, value);
    }

    public DateTime SelectionEnd
    {
        get => GetValue(SelectionEndProperty);
        set => SetValue(SelectionEndProperty, value);
    }

    public ObservableCollection<MonthViewDayViewModel> Days { get; } = new();

    public ObservableCollection<string> DayNames { get; } = new();

    private Button? _prevButton;
    private Button? _nextButton;

    private bool _isDragging;
    private DateTime _dragAnchorDate;

    public MonthView()
    {
        PopulateDayNames();
    }

    static MonthView()
    {
        DisplayDateProperty.Changed.AddClassHandler<MonthView>((x, _) =>
        {
            x.UpdateDaysGrid();
            x.RaisePropertyChanged(DisplayDateFormattedProperty, string.Empty, x.DisplayDateFormatted);
        });
        SelectionStartProperty.Changed.AddClassHandler<MonthView>((x, _) => x.OnSelectionChanged());
        SelectionEndProperty.Changed.AddClassHandler<MonthView>((x, _) => x.OnSelectionChanged());
    }

    private void PopulateDayNames()
    {
        DayNames.Clear();

        string[] abbreviatedDayNames = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames;

        for (int i = 0; i < 7; i++)
        {
            string dayName = abbreviatedDayNames[i];
            string initial = !string.IsNullOrEmpty(dayName)
                ? char.ToUpper(dayName.Trim()[0], CultureInfo.CurrentCulture).ToString()
                : "?";

            DayNames.Add(initial);
        }
    }

    private void OnSelectionChanged()
    {
        UpdateDaysGrid();

        DateTime start = SelectionStart < SelectionEnd ? SelectionStart : SelectionEnd;
        DateTime end = SelectionStart < SelectionEnd ? SelectionEnd : SelectionStart;

        DateRangeSelected?.Invoke(this, new DateRangeEventArgs(start, end));
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_prevButton != null) _prevButton.Click -= OnPrevClick;
        if (_nextButton != null) _nextButton.Click -= OnNextClick;

        _prevButton = e.NameScope.Find<Button>("PART_PreviousButton");
        _nextButton = e.NameScope.Find<Button>("PART_NextButton");

        if (_prevButton != null) _prevButton.Click += OnPrevClick;
        if (_nextButton != null) _nextButton.Click += OnNextClick;

        var itemsControl = e.NameScope.Find<ItemsControl>("PART_DaysGrid");
        if (itemsControl != null)
        {
            itemsControl.ItemsSource = Days;
        }

        UpdateDaysGrid();
    }

    private void OnPrevClick(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
    {
        DisplayDate = DisplayDate.AddMonths(-1);
    }

    private void OnNextClick(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
    {
        DisplayDate = DisplayDate.AddMonths(1);
    }

    public void OnDayPointerPressed(MonthViewDayViewModel dayVm, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            _isDragging = true;
            _dragAnchorDate = dayVm.Date;

            SelectionStart = dayVm.Date;
            SelectionEnd = dayVm.Date;
        }
    }

    public void OnDayPointerEntered(MonthViewDayViewModel dayVm, PointerEventArgs e)
    {
        if (_isDragging)
        {
            if (dayVm.Date >= _dragAnchorDate)
            {
                SelectionStart = _dragAnchorDate;
                SelectionEnd = dayVm.Date;
            }
            else
            {
                SelectionStart = dayVm.Date;
                SelectionEnd = _dragAnchorDate;
            }
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        if (_isDragging)
        {
            _isDragging = false;
            e.Pointer.Capture(null);
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            if (e.Source is Control control && control.DataContext is MonthViewDayViewModel dayVm)
            {
                OnDayPointerPressed(dayVm, e);
            }
        }
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        if (_isDragging)
        {
            var point = e.GetPosition(this);
            var hit = this.InputHitTest(point);

            Visual? current = hit as Visual;
            while (current != null && current != this)
            {
                if (current is Control control && control.DataContext is MonthViewDayViewModel dayVm)
                {
                    OnDayPointerEntered(dayVm, e);
                    break;
                }
                current = current.GetVisualParent();
            }
        }
    }

    private void UpdateDaysGrid()
    {
        Days.Clear();

        DateTime activeStart = SelectionStart < SelectionEnd ? SelectionStart : SelectionEnd;
        DateTime activeEnd = SelectionStart < SelectionEnd ? SelectionEnd : SelectionStart;

        foreach (var currentDate in MonthViewGridCalculator.GetDates(DisplayDate))
        {
            bool isCurrentMonth = currentDate.Month == DisplayDate.Month;
            bool isSelected = currentDate.Date >= activeStart.Date && currentDate.Date <= activeEnd.Date;
            bool isToday = currentDate.Date == DateTime.Today;

            var dayVm = new MonthViewDayViewModel()
            {
                Date = currentDate,
                DayNumber = currentDate.Day,
                IsCurrentMonth = isCurrentMonth,
                IsSelected = isSelected,
                IsToday = isToday
            };

            Days.Add(dayVm);
        }
    }
}
