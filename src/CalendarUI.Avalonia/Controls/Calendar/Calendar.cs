using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using CalendarUI.Avalonia.Controls.MonthView;

namespace CalendarUI.Avalonia.Controls.Calendar
{
    public class CalendarDayModel
    {
        public DateTime Date { get; set; }

        public bool IsFirstInGrid { get; set; }

        public string DayDisplayNumber
        {
            get
            {
                if (Date.Day == 1 || IsFirstInGrid)
                {
                    string monthName = Date.ToString("MMM", CultureInfo.CurrentCulture).Replace(".", "").ToLower();
                    return $"{Date.Day} {monthName}";
                }
                return Date.Day.ToString();
            }
        }

        public bool IsToday => Date.Date == DateTime.Today;
        public string DayOfWeekName { get; set; } = string.Empty;
        public IEnumerable<ICalendarItem> Items { get; set; } = new List<ICalendarItem>();
    }

    public class Calendar : TemplatedControl
    {
        private MonthView.MonthView? _monthView;

        public static readonly StyledProperty<DateTime> ViewStartProperty =
            AvaloniaProperty.Register<Calendar, DateTime>(
                nameof(ViewStart),
                DateTime.Today,
                defaultBindingMode: BindingMode.TwoWay);

        public static readonly StyledProperty<DateTime> ViewEndProperty =
            AvaloniaProperty.Register<Calendar, DateTime>(
                nameof(ViewEnd),
                DateTime.Today,
                defaultBindingMode: BindingMode.TwoWay);

        public static readonly StyledProperty<int> MaximumFullDaysProperty =
            AvaloniaProperty.Register<Calendar, int>(nameof(MaximumFullDays), 7);

        public static readonly StyledProperty<CalendarDaysMode> DaysModeProperty =
            AvaloniaProperty.Register<Calendar, CalendarDaysMode>(nameof(DaysMode), CalendarDaysMode.Expanded);

        public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty =
            AvaloniaProperty.Register<Calendar, IEnumerable?>(nameof(ItemsSource));

        public static readonly DirectProperty<Calendar, List<CalendarDayModel>> DaysProperty =
            AvaloniaProperty.RegisterDirect<Calendar, List<CalendarDayModel>>(nameof(Days), o => o.Days);

        public static readonly StyledProperty<bool> IsMonthViewVisibleProperty =
            AvaloniaProperty.Register<Calendar, bool>(nameof(IsMonthViewVisible), defaultValue: true);

        public static readonly DirectProperty<Calendar, string> MonthViewTitleProperty =
            AvaloniaProperty.RegisterDirect<Calendar, string>(
                nameof(MonthViewTitle),
                o => o.MonthViewTitle);

        public static readonly DirectProperty<Calendar, List<CalendarWeekModel>> WeeksProperty =
            AvaloniaProperty.RegisterDirect<Calendar, List<CalendarWeekModel>>(nameof(Weeks), o => o.Weeks);

        public static readonly DirectProperty<Calendar, List<CalendarDayModel>> ShortHeadersProperty =
            AvaloniaProperty.RegisterDirect<Calendar, List<CalendarDayModel>>(nameof(ShortHeaders), o => o.ShortHeaders);

        private List<CalendarWeekModel> _weeks = new();
        private List<CalendarDayModel> _shortHeaders = new();

        public List<CalendarWeekModel> Weeks
        {
            get => _weeks;
            private set => SetAndRaise(WeeksProperty, ref _weeks, value);
        }

        public List<CalendarDayModel> ShortHeaders
        {
            get => _shortHeaders;
            private set => SetAndRaise(ShortHeadersProperty, ref _shortHeaders, value);
        }

        private string _monthViewTitle = string.Empty;

        public string MonthViewTitle
        {
            get => _monthViewTitle;
            private set => SetAndRaise(MonthViewTitleProperty, ref _monthViewTitle, value);
        }

        public bool IsMonthViewVisible
        {
            get => GetValue(IsMonthViewVisibleProperty);
            set => SetValue(IsMonthViewVisibleProperty, value);
        }

        private List<CalendarDayModel> _days = new();

        public DateTime ViewStart
        {
            get => GetValue(ViewStartProperty);
            set => SetValue(ViewStartProperty, value);
        }

        public DateTime ViewEnd
        {
            get => GetValue(ViewEndProperty);
            set => SetValue(ViewEndProperty, value);
        }

        public int MaximumFullDays
        {
            get => GetValue(MaximumFullDaysProperty);
            set => SetValue(MaximumFullDaysProperty, value);
        }

        public CalendarDaysMode DaysMode
        {
            get => GetValue(DaysModeProperty);
            set => SetValue(DaysModeProperty, value);
        }

        public IEnumerable? ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public List<CalendarDayModel> Days
        {
            get => _days;
            private set => SetAndRaise(DaysProperty, ref _days, value);
        }

        public List<string> DaysOfWeekHeader { get; } = DateTimeFormatInfo.CurrentInfo.ShortestDayNames
            .Select(d => d.ToUpper(CultureInfo.CurrentCulture))
            .ToList();

        public static readonly StyledProperty<IEnumerable<CalendarHeaderItem>> HeaderDaysProperty =
            AvaloniaProperty.Register<Calendar, IEnumerable<CalendarHeaderItem>>(nameof(HeaderDays));

        public IEnumerable<CalendarHeaderItem> HeaderDays
        {
            get => GetValue(HeaderDaysProperty);
            set => SetValue(HeaderDaysProperty, value);
        }

        public void UpdateHeaderDays(DateTime startDate)
        {
            var items = new List<CalendarHeaderItem>();

            for (int i = 0; i < 7; i++)
            {
                DateTime date = startDate.AddDays(i);

                items.Add(new CalendarHeaderItem
                {
                    DayOfWeekName = date.ToString("ddd", CultureInfo.CurrentCulture).ToUpper().Replace(".", ""),
                    DayNumberDisplay = date.Day.ToString(),
                    IsToday = date.Date == DateTime.Today
                });
            }

            HeaderDays = items;
        }

        static Calendar()
        {
            ViewStartProperty.Changed.AddClassHandler<Calendar>((x, e) => x.UpdateDaysAndMode());
            ViewEndProperty.Changed.AddClassHandler<Calendar>((x, e) => x.UpdateDaysAndMode());
            MaximumFullDaysProperty.Changed.AddClassHandler<Calendar>((x, e) => x.UpdateDaysAndMode());
            ItemsSourceProperty.Changed.AddClassHandler<Calendar>((x, e) => x.UpdateDaysAndMode());
        }

        public Calendar()
        {
            UpdateDaysAndMode();
        }

        private ScrollViewer? _scrollViewer;

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            if (_monthView != null)
            {
                _monthView.DateRangeSelected -= OnMonthViewDateRangeSelected;
            }

            _monthView = e.NameScope.Find<MonthView.MonthView>("PART_MonthView");
            _scrollViewer = e.NameScope.Find<ScrollViewer>("PART_ScrollViewer");

            if (_monthView != null)
            {
                _monthView.DateRangeSelected += OnMonthViewDateRangeSelected;
            }
        }

        private void OnMonthViewDateRangeSelected(object? sender, DateRangeEventArgs e)
        {
            SetDateRange(e.Start, e.End);
        }

        public void SetDateRange(DateTime start, DateTime end)
        {
            if (start > end)
            {
                var temp = start;
                start = end;
                end = temp;
            }

            ViewStart = start.Date;
            ViewEnd = end.Date;
        }

        private void UpdateDaysAndMode()
        {
            DateTime current = ViewStart.Date;
            DateTime end = ViewEnd.Date;

            if (end < current) end = current;

            int totalDays = (end - current).Days + 1;

            DaysMode = totalDays <= MaximumFullDays ? CalendarDaysMode.Expanded : CalendarDaysMode.Short;

            var listDays = new List<CalendarDayModel>();
            var listWeeks = new List<CalendarWeekModel>();
            var allItems = ItemsSource?.OfType<ICalendarItem>().ToList() ?? new List<ICalendarItem>();

            if (DaysMode == CalendarDaysMode.Expanded)
            {
                while (current <= end)
                {
                    var dayModel = CreateDayModel(current, allItems);
                    listDays.Add(dayModel);
                    current = current.AddDays(1);
                }
                ShortHeaders = listDays.Take(7).ToList();
            }
            else
            {
                int startOffset = (int)current.DayOfWeek;
                DateTime gridStart = current.AddDays(-startOffset);

                int endOffset = 6 - (int)end.DayOfWeek;
                DateTime gridEnd = end.AddDays(endOffset);

                int totalGridDays = (gridEnd - gridStart).Days + 1;
                int totalWeeks = Math.Max(1, totalGridDays / 7);

                DateTime iter = gridStart;
                bool isFirst = true;

                while (iter <= gridEnd)
                {
                    var dayModel = CreateDayModel(iter, allItems, isFirst);
                    listDays.Add(dayModel);
                    iter = iter.AddDays(1);
                    isFirst = false;
                }

                DateTime weekIter = gridStart;
                for (int i = 0; i < totalWeeks; i++)
                {
                    listWeeks.Add(new CalendarWeekModel
                    {
                        StartDate = weekIter,
                        EndDate = weekIter.AddDays(6)
                    });
                    weekIter = weekIter.AddDays(7);
                }

                ShortHeaders = listDays.Take(7).ToList();
            }

            Days = listDays;
            Weeks = listWeeks;

            InvalidateMeasure();
            InvalidateArrange();
            InvalidateVisual();
        }

        private CalendarDayModel CreateDayModel(DateTime date, List<ICalendarItem> itemsSource, bool isFirstInGrid = false)
        {
            return new CalendarDayModel
            {
                Date = date,
                IsFirstInGrid = isFirstInGrid,
                DayOfWeekName = date.ToString("ddd", CultureInfo.CurrentCulture).ToUpper().Replace(".", ""),
                Items = itemsSource.Where(item => item.DateStart.Date <= date.Date && item.DateEnd.Date >= date.Date).ToList()
            };
        }
    }

    public class CalendarWeekModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string WeekRangeDisplay
        {
            get
            {
                string startStr;
                string endStr;

                if (StartDate.Month == EndDate.Month)
                {
                    startStr = StartDate.Day.ToString();
                    endStr = $"{EndDate.Day} {EndDate.ToString("MMM", CultureInfo.CurrentCulture).Replace(".", "").ToLower()}";
                }
                else
                {
                    startStr = $"{StartDate.Day} {StartDate.ToString("MMM", CultureInfo.CurrentCulture).Replace(".", "").ToLower()}";
                    endStr = $"{EndDate.Day} {EndDate.ToString("MMM", CultureInfo.CurrentCulture).Replace(".", "").ToLower()}";
                }

                return $"{startStr} - {endStr}";
            }
        }
    }

    public class CalendarHeaderItem
    {
        public string DayOfWeekName { get; set; } = string.Empty;
        public string DayNumberDisplay { get; set; } = string.Empty;
        public bool IsToday { get; set; }
    }
}
