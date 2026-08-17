namespace CalendarUI.Avalonia.Controls.Calendar;

public readonly record struct EventRange
{
    public DateTime Start { get; }
    public DateTime End { get; }

    internal EventRange(DateTime start, DateTime end)
    {
        if (end < start)
        {
            throw new ArgumentException(
                "Event end cannot be before start.");
        }

        Start = start;
        End = end;
    }

    public static EventRangeBuilder From(int year, int month, int day)
    {
        return new EventRangeBuilder(
            new DateTime(year, month, day));
    }

    public static EventRangeBuilder From(
        int year,
        int month,
        int day,
        int hour,
        int minute)
    {
        return new EventRangeBuilder(
            new DateTime(year, month, day, hour, minute, 0));
    }

    public readonly struct EventRangeBuilder
    {
        private readonly DateTime _start;

        internal EventRangeBuilder(DateTime start)
        {
            _start = start;
        }

        public EventRange To(int year, int month, int day)
        {
            return new EventRange(
                _start,
                new DateTime(year, month, day));
        }

        public EventRange To(
            int year,
            int month,
            int day,
            int hour,
            int minute)
        {
            return new EventRange(
                _start,
                new DateTime(year, month, day, hour, minute, 0));
        }
    }
}
