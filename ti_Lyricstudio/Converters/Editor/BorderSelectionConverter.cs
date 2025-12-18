using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace ti_Lyricstudio.Converters.Editor
{
    public class BorderSelectionConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool selected && targetType.IsAssignableFrom(typeof(Thickness)))
            {
                if (selected == true)
                    return new Thickness(4d);
                else
                    return new Thickness(0d);
            }
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
