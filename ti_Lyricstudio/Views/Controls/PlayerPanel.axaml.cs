using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using ti_Lyricstudio.ViewModels;

namespace ti_Lyricstudio.Views.Controls;

public partial class PlayerPanel : UserControl
{
    BindingExpressionBase subscription;

    public PlayerPanel()
    {
        InitializeComponent();

        // register the tunnel handler for the TimeSlider
        // some elements eat event by itself, so handler needs to be registered as tunnel routing by code-behind.
        // ref: https://github.com/AvaloniaUI/Avalonia/discussions/10673#discussioncomment-6155908
        TimeBar.AddHandler(PointerPressedEvent, Seekbar_Pressed, RoutingStrategies.Tunnel);
        TimeBar.AddHandler(PointerReleasedEvent, Seekbar_Released, RoutingStrategies.Tunnel);

        // bind seekbar value to player duration variable
        subscription = TimeBar.Bind(Slider.ValueProperty, new Binding("Time"));
    }

    // event when seekbar is pressed
    public void Seekbar_Pressed(object? sender, PointerPressedEventArgs e)
    {
        // bind seekbar value from player duration variable
        subscription.Dispose();
        TimeBar.Value = (double)(DataContext as PlayerPanelViewModel)?.Time;
    }

    // event when seekbar is released
    public void Seekbar_Released(object? sender, PointerReleasedEventArgs e)
    {
        long newTime = (long)(sender as Slider).Value;
        (DataContext as PlayerPanelViewModel)?.Seek(newTime);

        // bind seekbar value to player duration variable
        subscription = TimeBar.Bind(Slider.ValueProperty, new Binding("Time"));
    }
}