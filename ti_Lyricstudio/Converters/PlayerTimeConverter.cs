using System;
using System.Globalization;
using Avalonia.Data.Converters;
using ti_Lyricstudio.Models;

namespace ti_Lyricstudio.Converters
{
    public class PlayerTimeConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is long time && parameter is string convertTo)
            {
                switch (convertTo)
                {
                    case "LyricTime":
                        return LyricTime.From(time);
                    case "LyricTime.String":
                        // return 0 when time is not initialized
                        if (time < 0) return "00:00.00";

                        // define each part of time
                        int minute = (int)(time / 60000);
                        int second = (int)(time / 1000 % 60);
                        int millisecond = (int)(time % 1000);

                        // return string-converted time
                        return $"{minute:00}:{second:00}.{millisecond / 10:00}";
                }
            }
            throw new NotImplementedException();
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
