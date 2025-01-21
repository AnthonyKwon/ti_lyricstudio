using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace ti_Lyricstudio.Converters
{
    public class PlayerStateConverter : IValueConverter
    {
        object? IValueConverter.Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is PlayerState state && parameter is string buttonId)
            {
                switch (state)
                {
                    case PlayerState.Playing:
                        // conversion to IsEnabled state
                        if (buttonId == "SetE") return true;
                        if (buttonId == "SeekbarE") return true;
                        // always return false when wrong Id returned
                        return false;
                    case PlayerState.Paused:
                        // conversion to IsEnabled state
                        if (buttonId == "SetE") return true;
                        if (buttonId == "SeekbarE") return true;
                        // always return false when wrong Id returned
                        return false;
                    case PlayerState.Stopped:
                        if (buttonId == "SetE") return false;
                        if (buttonId == "SeekbarE") return false;
                        // always return false when wrong Id returned
                        return false;
                    default:
                        /// conversion to IsEnabled state
                        if (buttonId == "SetE") return false;
                        if (buttonId == "SeekbarE") return false;
                        // always return false when wrong Id returned
                        return false;
                }
            }
            return false;
        }

        object? IValueConverter.ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
