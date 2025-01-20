using System;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using ti_Lyricstudio.ViewModels;

namespace ti_Lyricstudio.Views.Controls;

public partial class PlayerControl : UserControl
{
    BindingExpressionBase subscription;

    public static readonly RoutedEvent SetTimeClickEvent =
        RoutedEvent.Register<PlayerControl, RoutedEventArgs>(nameof(SetTimeClick), RoutingStrategies.Direct);

    public event EventHandler<RoutedEventArgs> SetTimeClick
    {
        add => AddHandler(SetTimeClickEvent, value);
        remove => RemoveHandler(SetTimeClickEvent, value);
    }

    public PlayerControl()
    {
        InitializeComponent();

        // register the tunnel handler for the TimeSlider
        // some elements eat event by itself, so handler needs to be registered as tunnel routing by code-behind.
        // ref: https://github.com/AvaloniaUI/Avalonia/discussions/10673#discussioncomment-6155908
        TimeSlider.AddHandler(PointerPressedEvent, Seekbar_Pressed, RoutingStrategies.Tunnel);
        TimeSlider.AddHandler(PointerReleasedEvent, Seekbar_Released, RoutingStrategies.Tunnel);

        // bind seekbar value to player duration variable
        subscription = TimeSlider.Bind(Slider.ValueProperty, new Binding("Time"));

        // bind hotkey to player button
        // Rewind Button
        HotKeyManager.SetHotKey(RewindBtn, new KeyGesture(Key.O, KeyModifiers.Shift));
        // Stop Button
        HotKeyManager.SetHotKey(StopBtn, new KeyGesture(Key.L, KeyModifiers.Shift));
        // Play & Pause Button
        //TODO: implement correct switching of Play & Pause hotkey 
        HotKeyManager.SetHotKey(PlayBtn, new KeyGesture(Key.P, KeyModifiers.Shift));
        // Fast Forward Button
        HotKeyManager.SetHotKey(FastForwardBtn, new KeyGesture(Key.OemOpenBrackets, KeyModifiers.Shift));
        // Set Time Button
        HotKeyManager.SetHotKey(SetTimeBtn, new KeyGesture(Key.OemSemicolon, KeyModifiers.Shift));
    }

    // event when set time button is clicked
    private void SetTimeBtn_Click(object? sender, RoutedEventArgs e)
    {
        // pass event to SetTimeClickEvent
        RoutedEventArgs eventArgs = new() { RoutedEvent = SetTimeClickEvent };
        RaiseEvent(eventArgs);
    }

    // event when seekbar is pressed
    public void Seekbar_Pressed(object? sender, PointerPressedEventArgs e)
    {
        // bind seekbar value from player duration variable
        subscription.Dispose();
        TimeSlider.Value = (double)(DataContext as PlayerControlViewModel)?.Time;
    }

    // event when seekbar is released
    public void Seekbar_Released(object? sender, PointerReleasedEventArgs e)
    {
        long newTime = (long)(sender as Slider).Value;
        (DataContext as PlayerControlViewModel)?.Seek(newTime);

        // bind seekbar value to player duration variable
        subscription = TimeSlider.Bind(Slider.ValueProperty, new Binding("Time"));
    }
}