using Avalonia.Data;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace ti_Lyricstudio.Converters.Editor
{
    public class TextOpacityConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool active && targetType.IsAssignableFrom(typeof(double)))
            {
                if (active == true)
                    return 1d;
                else
                    return 0.4d;
            }
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
