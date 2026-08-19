using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace CalendarUI.Avalonia.Controls.Calendar
{
    public class CalendarDayPanel : Panel
    {
        public static readonly StyledProperty<IBrush?> GridBackgroundBrushProperty =
            AvaloniaProperty.Register<CalendarDayPanel, IBrush?>(nameof(GridBackgroundBrush));

        public static readonly StyledProperty<IBrush?> RulerBackgroundBrushProperty =
            AvaloniaProperty.Register<CalendarDayPanel, IBrush?>(nameof(RulerBackgroundBrush));

        public static readonly StyledProperty<IBrush?> LineBrushProperty =
            AvaloniaProperty.Register<CalendarDayPanel, IBrush?>(nameof(LineBrush));

        public static readonly StyledProperty<IBrush?> TextBrushProperty =
            AvaloniaProperty.Register<CalendarDayPanel, IBrush?>(nameof(TextBrush));

        public IBrush? GridBackgroundBrush { get => GetValue(GridBackgroundBrushProperty); set => SetValue(GridBackgroundBrushProperty, value); }
        public IBrush? RulerBackgroundBrush { get => GetValue(RulerBackgroundBrushProperty); set => SetValue(RulerBackgroundBrushProperty, value); }
        public IBrush? LineBrush { get => GetValue(LineBrushProperty); set => SetValue(LineBrushProperty, value); }
        public IBrush? TextBrush { get => GetValue(TextBrushProperty); set => SetValue(TextBrushProperty, value); }

        public static readonly StyledProperty<DateTime> ViewStartProperty =
            AvaloniaProperty.Register<CalendarDayPanel, DateTime>(nameof(ViewStart), DateTime.Today);

        public static readonly StyledProperty<DateTime> ViewEndProperty =
            AvaloniaProperty.Register<CalendarDayPanel, DateTime>(nameof(ViewEnd), DateTime.Today);

        public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty =
            AvaloniaProperty.Register<CalendarDayPanel, IEnumerable?>(nameof(ItemsSource));

        public static readonly StyledProperty<double> HourHeightProperty =
            AvaloniaProperty.Register<CalendarDayPanel, double>(nameof(HourHeight), 60.0);

        public static readonly StyledProperty<int> DayStartHourProperty =
            AvaloniaProperty.Register<CalendarDayPanel, int>(nameof(DayStartHour), 0);

        public static readonly StyledProperty<int> DayEndHourProperty =
            AvaloniaProperty.Register<CalendarDayPanel, int>(nameof(DayEndHour), 24);

        public static readonly StyledProperty<double> TimeRulerWidthProperty =
            AvaloniaProperty.Register<CalendarDayPanel, double>(nameof(TimeRulerWidth), 60.0);

        public DateTime ViewStart { get => GetValue(ViewStartProperty); set => SetValue(ViewStartProperty, value); }
        public DateTime ViewEnd { get => GetValue(ViewEndProperty); set => SetValue(ViewEndProperty, value); }
        public IEnumerable? ItemsSource { get => GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }
        public double HourHeight { get => GetValue(HourHeightProperty); set => SetValue(HourHeightProperty, value); }
        public int DayStartHour { get => GetValue(DayStartHourProperty); set => SetValue(DayStartHourProperty, value); }
        public int DayEndHour { get => GetValue(DayEndHourProperty); set => SetValue(DayEndHourProperty, value); }
        public double TimeRulerWidth { get => GetValue(TimeRulerWidthProperty); set => SetValue(TimeRulerWidthProperty, value); }

        private readonly CalendarGridBackground _backgroundControl;

        private const double ItemHeight = 32.0;
        private const double StackOffset = 17.0;
        private const double AllDayAreaSpacing = 4.0;

        private readonly Dictionary<CalendarItemControl, int> _allDayStackIndexes = new();

        public CalendarDayPanel()
        {
            this.UseLayoutRounding = true;
            _backgroundControl = new CalendarGridBackground(this);
            Children.Add(_backgroundControl);
        }

        static CalendarDayPanel()
        {
            ItemsSourceProperty.Changed.AddClassHandler<CalendarDayPanel>((x, e) => x.OnItemsSourceChanged(e));
            ViewStartProperty.Changed.AddClassHandler<CalendarDayPanel>((x, e) =>
            {
                x.RebuildItemControls();
                x.InvalidateArrange();
                x._backgroundControl.InvalidateVisual();
            });

            ViewEndProperty.Changed.AddClassHandler<CalendarDayPanel>((x, e) =>
            {
                x.RebuildItemControls();
                x.InvalidateArrange();
                x._backgroundControl.InvalidateVisual();
            });
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == GridBackgroundBrushProperty ||
                change.Property == RulerBackgroundBrushProperty ||
                change.Property == LineBrushProperty ||
                change.Property == TextBrushProperty ||
                change.Property == TimeRulerWidthProperty)
            {
                _backgroundControl?.InvalidateVisual();
            }
        }

        private void OnItemsSourceChanged(AvaloniaPropertyChangedEventArgs e)
        {
            if (e.OldValue is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= OnCollectionChanged;
            }

            RebuildItemControls();

            if (e.NewValue is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged += OnCollectionChanged;
            }
        }

        private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            RebuildItemControls();
        }

        private void RebuildItemControls()
        {
            Children.Clear();
            _allDayStackIndexes.Clear();

            Children.Add(_backgroundControl);

            if (ItemsSource == null)
                return;

            int allDayIndex = 0;

            foreach (var rawItem in ItemsSource)
            {
                if (rawItem is not ICalendarItem item)
                    continue;

                var itemControl = new CalendarItemControl
                {
                    Item = item,
                    DataContext = item
                };

                if (item.IsAllDay)
                {
                    _allDayStackIndexes[itemControl] = allDayIndex++;
                }

                Children.Add(itemControl);
            }

            InvalidateMeasure();
            InvalidateArrange();
        }

        private int GetTotalDays()
        {
            int days = (ViewEnd.Date - ViewStart.Date).Days + 1;
            return Math.Max(1, days);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            int totalHours = Math.Max(1, DayEndHour - DayStartHour);
            double desiredHeight = totalHours * HourHeight;

            foreach (Control child in Children)
            {
                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }

            return new Size(availableSize.Width, desiredHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            int totalDays = GetTotalDays();

            double gridWidth =
                Math.Max(0, finalSize.Width - TimeRulerWidth);

            double dayWidth =
                gridWidth / totalDays;

            int totalHours =
                Math.Max(1, DayEndHour - DayStartHour);

            var allDayControls = new List<(CalendarItemControl Control, ICalendarItem Item)>();

            foreach (Control child in Children)
            {
                if (child == _backgroundControl)
                    continue;

                if (child is not CalendarItemControl itemControl)
                    continue;

                ICalendarItem? item =
                    itemControl.DataContext as ICalendarItem ??
                    itemControl.Item;

                if (item == null)
                    continue;

                if (item.IsAllDay)
                    allDayControls.Add((itemControl, item));
            }

            double allDayAreaHeight = allDayControls.Count == 0
                ? 0
                : ItemHeight +
                  ((allDayControls.Count - 1) * StackOffset) +
                  AllDayAreaSpacing;

            double timedAreaHeight =
                Math.Max(0, finalSize.Height - allDayAreaHeight);

            double effectiveHourHeight =
                timedAreaHeight / totalHours;

            if (_backgroundControl != null)
            {
                _backgroundControl.ZIndex = 0;
                _backgroundControl.Arrange(
                    new Rect(
                        0,
                        0,
                        finalSize.Width,
                        finalSize.Height));
            }

            foreach (Control child in Children)
            {
                if (child == _backgroundControl)
                    continue;

                child.ZIndex = 10;

                if (child is not CalendarItemControl itemControl)
                    continue;

                ICalendarItem? item =
                    itemControl.DataContext as ICalendarItem ??
                    itemControl.Item;

                if (item == null)
                {
                    child.Arrange(new Rect(0, 0, 0, 0));
                    continue;
                }

                CalendarItemVisibilityState visibilityState =
                    CalendarItemVisibility.GetState(
                        item,
                        ViewStart.Date,
                        ViewEnd.Date);

                bool continuesBefore =
                    visibilityState is
                        CalendarItemVisibilityState.StartsBeforeView
                        or CalendarItemVisibilityState.ExtendsBeyondView;

                bool continuesAfter =
                    visibilityState is
                        CalendarItemVisibilityState.EndsAfterView
                        or CalendarItemVisibilityState.ExtendsBeyondView;

                itemControl.SetSegmentContinuation(
                    continuesBefore,
                    continuesAfter);

                int startDayOffset =
                    (item.DateStart.Date - ViewStart.Date).Days;

                int endDayOffset =
                    (item.DateEnd.Date - ViewStart.Date).Days;

                if (item.DateEnd.TimeOfDay == TimeSpan.Zero &&
                    item.DateEnd > item.DateStart)
                {
                    endDayOffset--;
                }

                if (endDayOffset < 0 ||
                    startDayOffset >= totalDays)
                {
                    child.Arrange(new Rect(0, 0, 0, 0));
                    continue;
                }

                int visibleStartDay =
                    Math.Max(0, startDayOffset);

                int visibleEndDay =
                    Math.Min(totalDays - 1, endDayOffset);

                int daySpan =
                    Math.Max(
                        1,
                        visibleEndDay - visibleStartDay + 1);

                double x =
                    TimeRulerWidth +
                    (visibleStartDay * dayWidth);

                double width =
                    dayWidth * daySpan;

                if (item.IsAllDay)
                {
                    int stackIndex =
                        _allDayStackIndexes[itemControl];

                    double y =
                        stackIndex * StackOffset;

                    child.Arrange(
                        new Rect(
                            x,
                            y,
                            width,
                            ItemHeight));

                    continue;
                }

                double startHourFraction =
                    item.DateStart.TimeOfDay.TotalHours -
                    DayStartHour;

                startHourFraction =
                    Math.Max(0, startHourFraction);

                double yTimed =
                    allDayAreaHeight +
                    (startHourFraction * effectiveHourHeight);

                double totalDurationHours =
                    (item.DateEnd - item.DateStart).TotalHours;

                double height =
                    Math.Max(
                        ItemHeight,
                        totalDurationHours * effectiveHourHeight);

                child.Arrange(
                    new Rect(
                        x,
                        yTimed,
                        width,
                        height));
            }

            return finalSize;
        }

        private class CalendarGridBackground : Control
        {
            private readonly CalendarDayPanel _owner;

            private static readonly Typeface HourBigTypeface = new Typeface("Open Sans, sans-serif", FontStyle.Normal, FontWeight.Normal);
            private static readonly Typeface MinuteSmallTypeface = new Typeface("Open Sans, sans-serif", FontStyle.Normal, FontWeight.SemiBold);

            public CalendarGridBackground(CalendarDayPanel owner) => _owner = owner;

            public override void Render(DrawingContext context)
            {
                base.Render(context);

                double width = Bounds.Width;
                double height = Bounds.Height;

                int totalHours = Math.Max(1, _owner.DayEndHour - _owner.DayStartHour);

                double rulerWidth = _owner.TimeRulerWidth;
                double gridWidth = Math.Max(0, width - rulerWidth);

                var lineBrush = _owner.LineBrush ?? Brushes.Gray;
                var textBrush = _owner.TextBrush ?? Brushes.Black;
                var rulerBrush = _owner.RulerBackgroundBrush ?? Brushes.Transparent;
                var gridBrush = _owner.GridBackgroundBrush ?? Brushes.Transparent;

                var linePen = new Pen(lineBrush, 1);

                // 1. Preenchimento de Fundo
                context.FillRectangle(rulerBrush, new Rect(0, 0, rulerWidth, height));
                context.FillRectangle(gridBrush, new Rect(rulerWidth, 0, gridWidth, height));

                // 2. Linhas Horizontais das Horas
                double effectiveHourHeight = height / totalHours;

                for (int hour = 1; hour < totalHours; hour++)
                {
                    double y = Math.Floor(hour * effectiveHourHeight) + 0.5;
                    context.DrawLine(linePen, new Point(0, y), new Point(width, y));
                }

                // 3. Textos e Linhas de Meia Hora (30min)
                for (int hour = 0; hour < totalHours; hour++)
                {
                    double y = Math.Floor(hour * effectiveHourHeight);
                    int actualHour = _owner.DayStartHour + hour;

                    if (actualHour <= 24)
                    {
                        var formattedHour = new FormattedText(
                            $"{actualHour:D2}", CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                            HourBigTypeface, 14, textBrush);

                        var formattedMinute = new FormattedText(
                            "00", CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                            MinuteSmallTypeface, 8, textBrush);

                        double textY = y + 2.0;
                        context.DrawText(formattedHour, new Point(8, textY));
                        context.DrawText(formattedMinute, new Point(32, textY + 1));

                        // Linha fina de meia hora
                        double halfHourY = Math.Floor(y + (effectiveHourHeight / 2.0)) + 0.5;
                        context.DrawLine(linePen, new Point(32, halfHourY), new Point(width, halfHourY));
                    }
                }

                // 4. Linha vertical divisória da régua de horas
                double rulerLineX = Math.Floor(rulerWidth) - 0.5;
                context.DrawLine(linePen, new Point(rulerLineX, 0), new Point(rulerLineX, height));

                // 5. Linhas verticais divisórias dos dias
                int totalDays = _owner.GetTotalDays();
                if (totalDays > 1)
                {
                    double dayWidth = gridWidth / totalDays;
                    for (int day = 1; day < totalDays; day++)
                    {
                        double x = Math.Floor(rulerWidth + (day * dayWidth)) + 0.5;
                        context.DrawLine(linePen, new Point(x, 0), new Point(x, height));
                    }
                }
            }
        }
    }
}