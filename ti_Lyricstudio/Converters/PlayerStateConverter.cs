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
                        if (buttonId == "RewindE") return true;
                        if (buttonId == "StopE") return true;
                        if (buttonId == "PlayE") return false;
                        if (buttonId == "PauseE") return true;
                        if (buttonId == "FastForwardE") return true;
                        if (buttonId == "SetE") return true;
                        if (buttonId == "SeekbarE") return true;
                        // conversion to IsVisible state
                        if (buttonId == "PlayV") return false;
                        if (buttonId == "PauseV") return true;
                        // always return false when wrong Id returned
                        return false;
                    case PlayerState.Paused:
                        // conversion to IsEnabled state
                        if (buttonId == "RewindE") return true;
                        if (buttonId == "StopE") return true;
                        if (buttonId == "PlayE") return true;
                        if (buttonId == "PauseE") return false;
                        if (buttonId == "FastForwardE") return true;
                        if (buttonId == "SetE") return true;
                        if (buttonId == "SeekbarE") return true;
                        // conversion to IsVisible state
                        if (buttonId == "PlayV") return true;
                        if (buttonId == "PauseV") return false;
                        // always return false when wrong Id returned
                        return false;
                    case PlayerState.Stopped:
                        // conversion to IsEnabled state
                        if (buttonId == "RewindE") return false;
                        if (buttonId == "StopE") return false;
                        if (buttonId == "PlayE") return true;
                        if (buttonId == "PauseE") return false;
                        if (buttonId == "FastForwardE") return false;
                        if (buttonId == "SetE") return false;
                        if (buttonId == "SeekbarE") return false;
                        // conversion to IsVisible state
                        if (buttonId == "PlayV") return true;
                        if (buttonId == "PauseV") return false;
                        // always return false when wrong Id returned
                        return false;
                    default:
                        /// conversion to IsEnabled state
                        if (buttonId == "RewindE") return false;
                        if (buttonId == "StopE") return false;
                        if (buttonId == "PlayE") return false;
                        if (buttonId == "PauseE") return false;
                        if (buttonId == "FastForwardE") return false;
                        if (buttonId == "SetE") return false;
                        if (buttonId == "SeekbarE") return false;
                        // conversion to IsVisible state
                        if (buttonId == "PlayV") return true;
                        if (buttonId == "PauseV") return false;
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
