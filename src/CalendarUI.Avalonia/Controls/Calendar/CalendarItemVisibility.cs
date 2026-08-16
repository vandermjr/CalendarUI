namespace CalendarUI.Avalonia.Controls.Calendar;

internal enum CalendarItemVisibilityState
{
    FullyVisible,
    StartsBeforeView,
    EndsAfterView,
    ExtendsBeyondView
}

internal static class CalendarItemVisibility
{
    public static CalendarItemVisibilityState GetState(
        ICalendarItem item,
        DateTime gridStart,
        DateTime gridEnd)
    {
        bool startsBeforeView = item.DateStart.Date < gridStart.Date;
        bool endsAfterView = item.DateEnd.Date > gridEnd.Date;

        if (startsBeforeView && endsAfterView)
            return CalendarItemVisibilityState.ExtendsBeyondView;

        if (startsBeforeView)
            return CalendarItemVisibilityState.StartsBeforeView;

        if (endsAfterView)
            return CalendarItemVisibilityState.EndsAfterView;

        return CalendarItemVisibilityState.FullyVisible;
    }
}
