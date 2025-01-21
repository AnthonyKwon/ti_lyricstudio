using System.Linq;
using Avalonia;
using Avalonia.Media;
using Avalonia.Themes.Fluent;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ti_Lyricstudio.ViewModels
{
    public partial class ViewModelBase : ObservableObject
    {
        [ObservableProperty]
        public SolidColorBrush _bgBrush;
        [ObservableProperty]
        public SolidColorBrush _fgBrush;
        [ObservableProperty]
        public SolidColorBrush _acBrush;

        public ViewModelBase()
        {
            // get current style to get color scheme
            FluentTheme theme = Application.Current.Styles.OfType<FluentTheme>().FirstOrDefault();

            // get background color from current theme
            if (theme.TryGetResource("SystemRegionColor", Application.Current.ActualThemeVariant, out object? _background) == true)
                BgBrush = new((Color)_background);
            else
                BgBrush = SolidColorBrush.Parse("#1F1F1F");

            // get foreground color from current theme
            if (theme.TryGetResource("SystemBaseHighColor", Application.Current.ActualThemeVariant, out object? _foreground) == true)
                FgBrush = new((Color)_foreground);
            else
                FgBrush = SolidColorBrush.Parse("#FFFFFF");

            // get accent color from current theme
            if (theme.TryGetResource("SystemAccentColor", Application.Current.ActualThemeVariant, out object? _accent) == true)
                AcBrush = new((Color)_accent);
            else
                AcBrush = SolidColorBrush.Parse("#566E9A");
        }
    }
}
