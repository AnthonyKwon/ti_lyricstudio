using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ti_Lyricstudio.ViewModels;

namespace ti_Lyricstudio.Views.Controls;

public partial class PlayerControl : UserControl
{
    public PlayerControl()
    {
        InitializeComponent();

        // register the tunnel handler for the TimeSlider
        // some elements eat event by itself, so handler needs to be registered as tunnel routing by code-behind.
        // ref: https://github.com/AvaloniaUI/Avalonia/discussions/10673#discussioncomment-6155908
        TimeSlider.AddHandler(PointerPressedEvent, Seekbar_Pressed, RoutingStrategies.Tunnel);
        TimeSlider.AddHandler(PointerReleasedEvent, Seekbar_Released, RoutingStrategies.Tunnel);
    }

    public void Seekbar_Pressed(object? sender, PointerPressedEventArgs e)
    {
        //
    }

    public void Seekbar_Released(object? sender, PointerReleasedEventArgs e)
    {
        long newTime = (long)(sender as Slider)?.Value;
        (DataContext as PlayerControlViewModel).Seek(newTime);
    }
}