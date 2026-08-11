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
        public static readonly StyledProperty<IBrush> GridBackgroundBrushProperty =
            AvaloniaProperty.Register<CalendarDayPanel, IBrush>(nameof(GridBackgroundBrush), Brushes.White);

        public static readonly StyledProperty<IBrush> RulerBackgroundBrushProperty =
            AvaloniaProperty.Register<CalendarDayPanel, IBrush>(nameof(RulerBackgroundBrush), new SolidColorBrush(Color.Parse("#F0F4FC")));

        public static readonly StyledProperty<IBrush> LineBrushProperty =
            AvaloniaProperty.Register<CalendarDayPanel, IBrush>(nameof(LineBrush), new SolidColorBrush(Color.Parse("#A0B4D5")));

        public static readonly StyledProperty<IBrush> TextBrushProperty =
            AvaloniaProperty.Register<CalendarDayPanel, IBrush>(nameof(TextBrush), new SolidColorBrush(Color.Parse("#51688E")));

        public IBrush GridBackgroundBrush { get => GetValue(GridBackgroundBrushProperty); set => SetValue(GridBackgroundBrushProperty, value); }
        public IBrush RulerBackgroundBrush { get => GetValue(RulerBackgroundBrushProperty); set => SetValue(RulerBackgroundBrushProperty, value); }
        public IBrush LineBrush { get => GetValue(LineBrushProperty); set => SetValue(LineBrushProperty, value); }
        public IBrush TextBrush { get => GetValue(TextBrushProperty); set => SetValue(TextBrushProperty, value); }

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
            Children.Add(_backgroundControl);

            if (ItemsSource == null) return;

            foreach (var rawItem in ItemsSource)
            {
                if (rawItem is ICalendarItem item)
                {
                    var itemControl = new CalendarItemControl
                    {
                        Item = item,
                        DataContext = item
                    };

                    Children.Add(itemControl);
                }
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
            double gridWidth = Math.Max(0, finalSize.Width - TimeRulerWidth);
            double dayWidth = gridWidth / totalDays;
            int totalHours = Math.Max(1, DayEndHour - DayStartHour);
            double effectiveHourHeight = finalSize.Height / totalHours;

            foreach (Control child in Children)
            {
                if (child == _backgroundControl)
                {
                    child.ZIndex = 0;
                    child.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
                    continue;
                }

                child.ZIndex = 10;

                ICalendarItem? item = child.DataContext as ICalendarItem;
                if (item == null && child is CalendarItemControl itemCtrl)
                {
                    item = itemCtrl.Item;
                }

                if (item != null)
                {
                    int startDayOffset = (item.DateStart.Date - ViewStart.Date).Days;
                    int endDayOffset = (item.DateEnd.Date - ViewStart.Date).Days;

                    if (item.DateEnd.TimeOfDay == TimeSpan.Zero && item.DateEnd > item.DateStart)
                    {
                        endDayOffset--;
                    }

                    if (endDayOffset >= 0 && startDayOffset < totalDays)
                    {
                        int visibleStartDay = Math.Max(0, startDayOffset);
                        int visibleEndDay = Math.Min(totalDays - 1, endDayOffset);
                        int daySpan = Math.Max(1, visibleEndDay - visibleStartDay + 1);

                        double x = TimeRulerWidth + (visibleStartDay * dayWidth);
                        double width = dayWidth * daySpan;

                        double y;
                        double height;

                        double totalDurationHours = (item.DateEnd - item.DateStart).TotalHours;
                        bool isMultiDay = totalDurationHours >= 24.0 || item.DateStart.Date != item.DateEnd.Date;

                        if (isMultiDay)
                        {
                            y = 0;
                            height = finalSize.Height;
                        }
                        else
                        {
                            double startHourFraction = item.DateStart.TimeOfDay.TotalHours - DayStartHour;
                            startHourFraction = Math.Max(0, startHourFraction);

                            y = startHourFraction * effectiveHourHeight;
                            height = Math.Max(22.0, totalDurationHours * effectiveHourHeight);
                        }

                        child.Arrange(new Rect(x, y, width, height));
                    }
                    else
                    {
                        child.Arrange(new Rect(0, 0, 0, 0));
                    }
                }
                else
                {
                    child.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
                }
            }

            return finalSize;
        }

        private class CalendarGridBackground : Control
        {
            private readonly CalendarDayPanel _owner;

            private static readonly Typeface HourBigTypeface = new Typeface("Open Sans, sans-serif", FontStyle.Normal, FontWeight.Normal);
            private static readonly Typeface MinuteSmallTypeface = new Typeface("Open Sans, sans-serif", FontStyle.Normal, FontWeight.Bold);

            public CalendarGridBackground(CalendarDayPanel owner) => _owner = owner;

            public override void Render(DrawingContext context)
            {
                base.Render(context);

                double width = Bounds.Width;
                double height = Bounds.Height;

                int totalHours = Math.Max(1, _owner.DayEndHour - _owner.DayStartHour);

                double rulerWidth = _owner.TimeRulerWidth;
                double gridWidth = Math.Max(0, width - rulerWidth);

                var linePen = new Pen(_owner.LineBrush, 1);
                var textBrush = _owner.TextBrush;

                // 1. Preenchimento de Fundo
                context.FillRectangle(_owner.RulerBackgroundBrush, new Rect(0, 0, rulerWidth, height));
                context.FillRectangle(_owner.GridBackgroundBrush, new Rect(rulerWidth, 0, gridWidth, height));

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

                // 4. LINHA VERTICAL DIVISÓRIA DA RÉGUA DE HORAS (Separa as horas dos dias)
                double rulerLineX = Math.Floor(rulerWidth) - 0.5;
                context.DrawLine(linePen, new Point(rulerLineX, 0), new Point(rulerLineX, height));

                // 5. LINHAS VERTICAIS DIVISÓRIAS DOS DIAS (Grade das colunas)
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
