using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace CalendarUI.Avalonia.Controls.Calendar
{
    public class CalendarGridLinesOverlay : Control
    {
        public static readonly StyledProperty<double> TimeRulerWidthProperty =
            AvaloniaProperty.Register<CalendarGridLinesOverlay, double>(nameof(TimeRulerWidth), 60.0);

        public static readonly StyledProperty<int> TotalDaysProperty =
            AvaloniaProperty.Register<CalendarGridLinesOverlay, int>(nameof(TotalDays), 7);

        public static readonly StyledProperty<IBrush?> LineBrushProperty =
            AvaloniaProperty.Register<CalendarGridLinesOverlay, IBrush?>(nameof(LineBrush));

        public double TimeRulerWidth
        {
            get => GetValue(TimeRulerWidthProperty);
            set => SetValue(TimeRulerWidthProperty, value);
        }

        public int TotalDays
        {
            get => GetValue(TotalDaysProperty);
            set => SetValue(TotalDaysProperty, value);
        }

        public IBrush? LineBrush
        {
            get => GetValue(LineBrushProperty);
            set => SetValue(LineBrushProperty, value);
        }

        static CalendarGridLinesOverlay()
        {
            AffectsRender<CalendarGridLinesOverlay>(TimeRulerWidthProperty, TotalDaysProperty, LineBrushProperty);
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            double width = Bounds.Width;
            double height = Bounds.Height;

            int days = Math.Max(1, TotalDays);
            double rulerWidth = TimeRulerWidth;
            double gridWidth = Math.Max(0, width - rulerWidth);

            var pen = new Pen(LineBrush, 1);

            // 1. Linha vertical separadora da régua de horas
            context.DrawLine(pen, new Point(rulerWidth, 0), new Point(rulerWidth, height));

            // 2. Divisórias verticais de ponta a ponta para cada dia
            for (int day = 1; day < days; day++)
            {
                double x = rulerWidth + (gridWidth * day / days);
                context.DrawLine(pen, new Point(x, 0), new Point(x, height));
            }
        }
    }
}
