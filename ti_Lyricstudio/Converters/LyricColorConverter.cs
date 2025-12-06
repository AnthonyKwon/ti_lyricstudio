using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Themes.Fluent;
using System;
using System.Globalization;
using System.Linq;

namespace ti_Lyricstudio.Converters
{
    public class LyricColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool active && targetType.IsAssignableFrom(typeof(SolidColorBrush)))
            {
                // get current style to get color scheme
                FluentTheme theme = Application.Current.Styles.OfType<FluentTheme>().FirstOrDefault();

                // get color of active text from current theme
                SolidColorBrush activeTextBrush = SolidColorBrush.Parse("#DFDFDF");
                if (theme.TryGetResource("SystemBaseHighColor", Application.Current.ActualThemeVariant, out object? _activeTextColor) == true)
                    activeTextBrush = new((Color)_activeTextColor);

                // get color of active text from current theme
                SolidColorBrush disabledTextBrush = SolidColorBrush.Parse("#7F7F7F");
                if (theme.TryGetResource("SystemChromeDisabledLowColor", Application.Current.ActualThemeVariant, out object? _disabledTextColor) == true)
                    disabledTextBrush = new((Color)_disabledTextColor);

                if (active == true)
                    return activeTextBrush;
                else
                    return disabledTextBrush;
            }
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
