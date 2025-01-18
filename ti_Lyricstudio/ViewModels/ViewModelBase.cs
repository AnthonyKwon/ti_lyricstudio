using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ti_Lyricstudio.ViewModels
{
    public class ViewModelBase : ObservableObject
    {
        public Avalonia.Media.SolidColorBrush BackColor { get; set; } = Avalonia.Media.SolidColorBrush.Parse("#1F1F1F");
        public Avalonia.Media.SolidColorBrush ForeColor { get; set; } = Avalonia.Media.SolidColorBrush.Parse("#CCCCCC");
    }
}
