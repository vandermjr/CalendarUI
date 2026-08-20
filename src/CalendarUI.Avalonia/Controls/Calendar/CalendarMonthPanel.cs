using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace CalendarUI.Avalonia.Controls.Calendar
{
    public class CalendarMonthPanel : Panel
    {
        private readonly MonthGridBackground _backgroundControl;

        #region StyledProperties

        public static readonly StyledProperty<DateTime> ViewStartProperty =
            AvaloniaProperty.Register<CalendarMonthPanel, DateTime>(nameof(ViewStart));

        public static readonly StyledProperty<DateTime> ViewEndProperty =
            AvaloniaProperty.Register<CalendarMonthPanel, DateTime>(nameof(ViewEnd));

        public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty =
            AvaloniaProperty.Register<CalendarMonthPanel, IEnumerable?>(nameof(ItemsSource));

        public static readonly StyledProperty<IBrush?> GridBackgroundBrushProperty =
            AvaloniaProperty.Register<CalendarMonthPanel, IBrush?>(nameof(GridBackgroundBrush));

        public static readonly StyledProperty<IBrush?> RulerBackgroundBrushProperty =
            AvaloniaProperty.Register<CalendarMonthPanel, IBrush?>(nameof(RulerBackgroundBrush));

        public static readonly StyledProperty<IBrush?> LineBrushProperty =
            AvaloniaProperty.Register<CalendarMonthPanel, IBrush?>(nameof(LineBrush));

        public static readonly StyledProperty<IBrush?> TextBrushProperty =
            AvaloniaProperty.Register<CalendarMonthPanel, IBrush?>(nameof(TextBrush));

        public static readonly StyledProperty<IBrush?> TodayBrushProperty =
            AvaloniaProperty.Register<CalendarMonthPanel, IBrush?>(nameof(TodayBrush));

        public static readonly StyledProperty<IBrush?> TodayForegroundBrushProperty =
            AvaloniaProperty.Register<CalendarMonthPanel, IBrush?>(nameof(TodayForegroundBrush));

        public static readonly StyledProperty<double> TimeRulerWidthProperty =
            AvaloniaProperty.Register<CalendarMonthPanel, double>(nameof(TimeRulerWidth), 60.0);

        private readonly Dictionary<
            CalendarItemControl,
            CalendarEventStackPlacement<CalendarMonthStackItem>> _itemStackPlacements = new();

        static CalendarMonthPanel()
        {
            AffectsMeasure<CalendarMonthPanel>(ViewStartProperty, ViewEndProperty, TimeRulerWidthProperty);
            AffectsArrange<CalendarMonthPanel>(ViewStartProperty, ViewEndProperty, TimeRulerWidthProperty);
            AffectsRender<CalendarMonthPanel>(
                ViewStartProperty,
                ViewEndProperty,
                GridBackgroundBrushProperty,
                RulerBackgroundBrushProperty,
                LineBrushProperty,
                TextBrushProperty,
                TodayBrushProperty,
                TodayForegroundBrushProperty,
                TimeRulerWidthProperty);
        }

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

        public IEnumerable? ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public IBrush? GridBackgroundBrush
        {
            get => GetValue(GridBackgroundBrushProperty);
            set => SetValue(GridBackgroundBrushProperty, value);
        }

        public IBrush? RulerBackgroundBrush
        {
            get => GetValue(RulerBackgroundBrushProperty);
            set => SetValue(RulerBackgroundBrushProperty, value);
        }

        public IBrush? LineBrush
        {
            get => GetValue(LineBrushProperty);
            set => SetValue(LineBrushProperty, value);
        }

        public IBrush? TextBrush
        {
            get => GetValue(TextBrushProperty);
            set => SetValue(TextBrushProperty, value);
        }

        public IBrush? TodayBrush
        {
            get => GetValue(TodayBrushProperty);
            set => SetValue(TodayBrushProperty, value);
        }

        public IBrush? TodayForegroundBrush
        {
            get => GetValue(TodayForegroundBrushProperty);
            set => SetValue(TodayForegroundBrushProperty, value);
        }

        public double TimeRulerWidth
        {
            get => GetValue(TimeRulerWidthProperty);
            set => SetValue(TimeRulerWidthProperty, value);
        }

        #endregion

        public CalendarMonthPanel()
        {
            _backgroundControl = new MonthGridBackground(this);
            Children.Add(_backgroundControl);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == ViewStartProperty ||
                change.Property == ViewEndProperty ||
                change.Property == TimeRulerWidthProperty)
            {
                _backgroundControl?.InvalidateMeasure();
                _backgroundControl?.InvalidateVisual();

                RebuildItems();

                InvalidateMeasure();
                InvalidateArrange();
                InvalidateVisual();
            }
            else if (change.Property == GridBackgroundBrushProperty ||
                     change.Property == RulerBackgroundBrushProperty ||
                     change.Property == LineBrushProperty ||
                     change.Property == TextBrushProperty ||
                     change.Property == TodayBrushProperty ||
                     change.Property == TodayForegroundBrushProperty)
            {
                _backgroundControl?.InvalidateVisual();
            }
            else if (change.Property == ItemsSourceProperty)
            {
                if (change.OldValue is INotifyCollectionChanged oldNotify)
                {
                    oldNotify.CollectionChanged -= OnItemsCollectionChanged;
                }

                RebuildItems();

                if (change.NewValue is INotifyCollectionChanged newNotify)
                {
                    newNotify.CollectionChanged += OnItemsCollectionChanged;
                }
            }
        }

        private void OnItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            RebuildItems();
        }

        private void RebuildItems()
        {
            Children.Clear();
            _itemStackPlacements.Clear();
            Children.Add(_backgroundControl);

            if (ItemsSource == null)
                return;

            DateTime gridStart = GetGridStartDate();
            DateTime gridEnd = GetGridEndDate();

            var segments = new List<CalendarMonthStackItem>();

            foreach (var item in ItemsSource)
            {
                if (item is not ICalendarItem calItem)
                    continue;

                foreach (var segment in CalendarItemSegments.GetSegments(
                    calItem,
                    gridStart,
                    gridEnd))
                {
                    segments.Add(
                        new CalendarMonthStackItem(
                            calItem,
                            segment));
                }
            }

            var groups =
                CalendarMonthEventStackGrouping.Group(segments);

            foreach (var group in groups)
            {
                var placements =
                    CalendarEventStackLayout.Calculate(group);

                foreach (var placement in placements)
                {
                    var control = CreateItemControl(
                        placement.Item.Item);

                    _itemStackPlacements[control] = placement;
                    Children.Add(control);
                }
            }

            InvalidateMeasure();
            InvalidateArrange();
        }

        private CalendarItemControl CreateItemControl(ICalendarItem item)
        {
            var control = new CalendarItemControl
            {
                DataContext = item
            };
            return control;
        }

        public DateTime GetGridStartDate()
        {
            DateTime start = ViewStart.Date;
            int offset = (int)start.DayOfWeek;
            return start.AddDays(-offset).Date;
        }

        public DateTime GetGridEndDate()
        {
            DateTime end = ViewEnd.Date;
            int offset = 6 - (int)end.DayOfWeek;
            return end.AddDays(offset).Date;
        }

        public int GetTotalWeeks()
        {
            DateTime gridStart = GetGridStartDate();
            DateTime gridEnd = GetGridEndDate();

            double totalDays = (gridEnd - gridStart).TotalDays + 1.0;
            int weeks = (int)Math.Ceiling(totalDays / 7.0);

            return Math.Max(1, weeks);
        }

        public int GetTotalDays() => GetTotalWeeks() * 7;

        private double GetRequiredWeekHeight()
        {
            const double firstWeekHeaderHeight = 58.0;
            const double weekHeaderHeight = 36.0;
            const double itemHeight = 32.0;
            const double stackOffset = 17.0;

            var weekHeights = new Dictionary<int, double>();

            foreach (var placement in _itemStackPlacements.Values)
            {
                int weekRow = placement.Item.Segment.WeekRow;

                double headerHeight =
                    weekRow == 0
                        ? firstWeekHeaderHeight
                        : weekHeaderHeight;

                double requiredHeight =
                    headerHeight +
                    (placement.StackIndex * stackOffset) +
                    itemHeight;

                if (!weekHeights.TryGetValue(weekRow, out double currentHeight) ||
                    requiredHeight > currentHeight)
                {
                    weekHeights[weekRow] = requiredHeight;
                }
            }

            double requiredWeekHeight =
                Math.Max(
                    firstWeekHeaderHeight + itemHeight,
                    weekHeaderHeight + itemHeight);

            foreach (double weekHeight in weekHeights.Values)
            {
                requiredWeekHeight = Math.Max(
                    requiredWeekHeight,
                    weekHeight);
            }

            return requiredWeekHeight;
        }
        
        protected override Size MeasureOverride(Size availableSize)
        {
            _backgroundControl.Measure(availableSize);

            foreach (Control child in Children)
            {
                if (child != _backgroundControl)
                    child.Measure(availableSize);
            }

            int totalWeeks = GetTotalWeeks();

            if (totalWeeks <= 0)
                return availableSize;

            double requiredWeekHeight = GetRequiredWeekHeight();
            double requiredHeight = requiredWeekHeight * totalWeeks;

            double height = double.IsInfinity(availableSize.Height)
                ? requiredHeight
                : Math.Max(availableSize.Height, requiredHeight);

            return new Size(
                availableSize.Width,
                height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            int totalWeeks = GetTotalWeeks();

            if (totalWeeks <= 0)
                return finalSize;

            double rulerWidth = TimeRulerWidth;
            double gridWidth = Math.Max(0, finalSize.Width - rulerWidth);
            double cellWidth = gridWidth / 7.0;
            double cellHeight = finalSize.Height / totalWeeks;

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

            DateTime gridStart = GetGridStartDate();
            DateTime gridEnd = GetGridEndDate();

            foreach (Control child in Children)
            {
                if (child == _backgroundControl)
                    continue;

                child.ZIndex = 10;

                if (child is not CalendarItemControl itemControl ||
                    !_itemStackPlacements.TryGetValue(
                        itemControl,
                        out var placement))
                {
                    continue;
                }

                var item = placement.Item.Item;
                var segment = placement.Item.Segment;

                var visibilityState =
                    CalendarItemVisibility.GetState(
                        item,
                        gridStart,
                        gridEnd);

                itemControl.SetVisibilityState(
                    visibilityState,
                    segment.IsFirstSegment,
                    segment.IsLastSegment);

                itemControl.SetSegmentContinuation(
                    !segment.IsFirstSegment,
                    !segment.IsLastSegment);

                double weekTopY =
                    segment.WeekRow * cellHeight;

                double headerHeight =
                    segment.WeekRow == 0
                        ? 58.0
                        : 36.0;

                double itemHeight = 32.0;

                double stackOffset = 17.0;

                double y =
                    weekTopY +
                    headerHeight +
                    (placement.StackIndex * stackOffset);

                double x =
                    rulerWidth +
                    (segment.DayColumn * cellWidth) +
                    2.0;

                double width =
                    Math.Max(
                        0,
                        (cellWidth * segment.DayCount) - 4.0);

                child.Arrange(
                    new Rect(
                        x,
                        y,
                        width,
                        itemHeight));
            }

            return finalSize;
        }

        private class MonthGridBackground : Control
        {
            private readonly CalendarMonthPanel _owner;

            private static readonly Typeface DayOfWeekTypeface = new Typeface("Open Sans, sans-serif", FontStyle.Normal, FontWeight.SemiBold);
            private static readonly Typeface DayNumberTypeface = new Typeface("Open Sans, sans-serif", FontStyle.Normal, FontWeight.SemiBold);
            private static readonly Typeface RulerWeekTypeface = new Typeface("Open Sans, sans-serif", FontStyle.Normal, FontWeight.SemiBold);

            public MonthGridBackground(CalendarMonthPanel owner) => _owner = owner;

            public override void Render(DrawingContext context)
            {
                base.Render(context);

                double width = Bounds.Width;
                double height = Bounds.Height;

                int totalWeeks = _owner.GetTotalWeeks();
                if (totalWeeks <= 0) return;

                double rulerWidth = _owner.TimeRulerWidth;
                double gridWidth = Math.Max(0, width - rulerWidth);

                double cellWidth = gridWidth / 7.0;
                double cellHeight = height / totalWeeks;

                var lineBrush = _owner.LineBrush ?? Brushes.Gray;
                var textBrush = _owner.TextBrush ?? Brushes.Black;
                var rulerBrush = _owner.RulerBackgroundBrush ?? Brushes.Transparent;
                var gridBrush = _owner.GridBackgroundBrush ?? Brushes.Transparent;
                var todayBgBrush = _owner.TodayBrush ?? Brushes.Blue;
                var todayFgBrush = _owner.TodayForegroundBrush ?? Brushes.White;

                var linePen = new Pen(lineBrush, 1);
                DateTime gridStartDate = _owner.GetGridStartDate();

                // Fundo da régua e da grade
                context.FillRectangle(rulerBrush, new Rect(0, 0, rulerWidth, height));
                context.FillRectangle(gridBrush, new Rect(rulerWidth, 0, gridWidth, height));

                // 1. Linha vertical separadora da régua de semanas
                context.DrawLine(linePen, new Point(rulerWidth, 0), new Point(rulerWidth, height));

                // 2. Linhas verticais dos dias da semana
                for (int day = 1; day < 7; day++)
                {
                    double x = rulerWidth + (cellWidth * day);
                    context.DrawLine(linePen, new Point(x, 0), new Point(x, height));
                }

                // 3. Linhas horizontais das semanas (de ponta a ponta)
                for (int week = 1; week < totalWeeks; week++)
                {
                    double y = Math.Floor(week * cellHeight) + 0.5;
                    context.DrawLine(linePen, new Point(0, y), new Point(width, y));
                }

                // 4. Renderização da Régua Lateral (Intervalo de Datas da Semana - Rotacionado a 90°)
                var culture = CultureInfo.CurrentCulture;
                for (int week = 0; week < totalWeeks; week++)
                {
                    double weekY = week * cellHeight;
                    DateTime weekStart = gridStartDate.AddDays(week * 7);
                    DateTime weekEnd = weekStart.AddDays(6);

                    string rangeText;
                    if (weekStart.Month == weekEnd.Month)
                    {
                        rangeText = $"{weekStart.Day} - {weekEnd.Day} {weekEnd.ToString("MMM", culture).Replace(".", "")}";
                    }
                    else
                    {
                        rangeText = $"{weekStart.Day} {weekStart.ToString("MMM", culture).Replace(".", "")} - {weekEnd.Day} {weekEnd.ToString("MMM", culture).Replace(".", "")}";
                    }

                    var formattedWeekText = new FormattedText(
                        rangeText, culture, FlowDirection.LeftToRight,
                        RulerWeekTypeface, 11.0, textBrush);

                    double centerX = rulerWidth / 2.0;
                    double centerY = weekY + (cellHeight / 2.0);

                    using (context.PushTransform(Matrix.CreateTranslation(-formattedWeekText.Width / 2.0, -formattedWeekText.Height / 2.0) *
                                                Matrix.CreateRotation(Math.PI / 2.0) *
                                                Matrix.CreateTranslation(centerX, centerY)))
                    {
                        context.DrawText(formattedWeekText, new Point(0, 0));
                    }
                }

                // 5. Renderização de cabeçalhos e números dos dias
                var shortestDayNames = culture.DateTimeFormat.ShortestDayNames;

                for (int week = 0; week < totalWeeks; week++)
                {
                    double weekY = week * cellHeight;

                    for (int dayInWeek = 0; dayInWeek < 7; dayInWeek++)
                    {
                        int dayIndex = (week * 7) + dayInWeek;
                        double cellX = rulerWidth + (dayInWeek * cellWidth);

                        DateTime currentDay = gridStartDate.AddDays(dayIndex);
                        bool isToday = currentDay.Date == DateTime.Today;

                        // Nome do dia (DOM, SEG...) na primeira semana
                        if (week == 0)
                        {
                            string dayOfWeekName = shortestDayNames[(int)((DayOfWeek)dayInWeek)]
                                .ToUpper(culture)
                                .Replace(".", "");

                            var formattedHeader = new FormattedText(
                                dayOfWeekName, culture, FlowDirection.LeftToRight,
                                DayOfWeekTypeface, 13.0, textBrush)
                            {
                                MaxTextWidth = Math.Max(1, cellWidth - 4),
                                Trimming = TextTrimming.CharacterEllipsis
                            };

                            double headerTextX = Math.Round(cellX + (cellWidth / 2.0) - (formattedHeader.Width / 2.0));
                            context.DrawText(formattedHeader, new Point(headerTextX, 4.0));
                        }

                        string dayNumText = currentDay.Day.ToString();

                        if (isToday)
                        {
                            var formattedTextToday = new FormattedText(
                                dayNumText, culture, FlowDirection.LeftToRight,
                                DayNumberTypeface, 13.0, todayFgBrush)
                            {
                                MaxTextWidth = Math.Max(1, cellWidth - 8),
                                Trimming = TextTrimming.CharacterEllipsis
                            };

                            double centerX = Math.Round(cellX + (cellWidth / 2.0));
                            double centerY = (week == 0) ? 38.0 : weekY + 18.0;

                            context.DrawEllipse(todayBgBrush, null, new Point(centerX, centerY), 14.0, 14.0);

                            double todayTextX = Math.Round(centerX - (formattedTextToday.Width / 2.0));
                            double todayTextY = Math.Round(centerY - (formattedTextToday.Height / 2.0));
                            context.DrawText(formattedTextToday, new Point(todayTextX, todayTextY));
                        }
                        else
                        {
                            var formattedText = new FormattedText(
                                dayNumText, culture, FlowDirection.LeftToRight,
                                DayOfWeekTypeface, 12.0, textBrush)
                            {
                                MaxTextWidth = Math.Max(1, cellWidth - 8),
                                Trimming = TextTrimming.CharacterEllipsis
                            };

                            double numberY = (week == 0) ? 30.0 : weekY + 10.0;
                            double textX = Math.Round(cellX + (cellWidth / 2.0) - (formattedText.Width / 2.0));
                            context.DrawText(formattedText, new Point(textX, numberY));
                        }
                    }
                }
            }
        }
    }
}