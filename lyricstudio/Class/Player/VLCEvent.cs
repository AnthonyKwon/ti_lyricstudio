using System;

namespace ti_Lyricstudio.Controls
{
    public partial class PlayerControl
    {
        // action when player started playing audio
        private void Player_Playing(object sender, EventArgs e)
        {
            // synchronise the stopwatch
            sw.Offset = new((long)(player.Position * audioDuration * 10000));
            // start or resume the stopwatch
            sw.Restart();

            Control.Dispatcher.Invoke(new(() =>
            {
                // enable the additional control buttons
                RewindBtn.IsEnabled = true;
                StopBtn.IsEnabled = true;
                ForwardBtn.IsEnabled = true;
                SetBtn.IsEnabled = true;
                TimeBar.IsEnabled = true;

                // show Pause button instead
                PauseBtn.Visibility = System.Windows.Visibility.Visible;
                PlayBtn.Visibility = System.Windows.Visibility.Hidden;
            }));
        }

        // action when player paused playing audio
        private void Player_Paused(object sender, EventArgs e)
        {
            // pause the stopwatch
            sw.Stop();

            Control.Dispatcher.Invoke(new(() =>
            { 
                // show Play button instead
                PlayBtn.Visibility = System.Windows.Visibility.Visible;
                PauseBtn.Visibility = System.Windows.Visibility.Hidden;
            }));
        }

        // action when player stopped playing audio
        private void Player_Stopped(object sender, EventArgs e)
        {
            // stop and reset the stopwatch
            sw.Reset();
            sw.Offset = TimeSpan.Zero;

            Control.Dispatcher.Invoke(new(() =>
            {
                // show Play button instead
                PlayBtn.Visibility = System.Windows.Visibility.Visible;
                PauseBtn.Visibility = System.Windows.Visibility.Hidden;

                // disable the additional control buttons
                RewindBtn.IsEnabled = false;
                StopBtn.IsEnabled = false;
                ForwardBtn.IsEnabled = false;
                SetBtn.IsEnabled = false;
                TimeBar.IsEnabled = false;
            }));
        }
    }
}
