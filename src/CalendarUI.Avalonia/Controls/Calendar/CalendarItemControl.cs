using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace CalendarUI.Avalonia.Controls.Calendar;

public class CalendarItemControl : TemplatedControl
{
    public static readonly StyledProperty<ICalendarItem?> ItemProperty =
        AvaloniaProperty.Register<CalendarItemControl, ICalendarItem?>(nameof(Item));

    public static readonly StyledProperty<bool> IsSelectedProperty =
        AvaloniaProperty.Register<CalendarItemControl, bool>(nameof(IsSelected));

    public ICalendarItem? Item
    {
        get => GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    internal void SetVisibilityState(CalendarItemVisibilityState state)
    {
        PseudoClasses.Set(":continues-before", state is CalendarItemVisibilityState.StartsBeforeView or CalendarItemVisibilityState.ExtendsBeyondView);
        PseudoClasses.Set(":continues-after", state is CalendarItemVisibilityState.EndsAfterView or CalendarItemVisibilityState.ExtendsBeyondView);
    }

    static CalendarItemControl()
    {
        IsSelectedProperty.Changed.AddClassHandler<CalendarItemControl>((x, e) =>
            x.UpdatePseudoClasses((bool)e.NewValue!));

        // Quando a propriedade Item mudar, atualiza o DataContext interno do controle
        ItemProperty.Changed.AddClassHandler<CalendarItemControl>((x, e) =>
        {
            if (e.NewValue is ICalendarItem newItem)
            {
                x.DataContext = newItem;
            }
        });
    }

    private void UpdatePseudoClasses(bool isSelected)
    {
        PseudoClasses.Set(":selected", isSelected);
    }
}
