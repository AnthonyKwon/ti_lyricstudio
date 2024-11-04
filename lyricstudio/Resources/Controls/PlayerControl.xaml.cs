using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Controls;
using ti_Lyricstudio.Class;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace ti_Lyricstudio.Controls
{
    /// <summary>
    /// Interaction logic for PlayerControl.xaml
    /// </summary>
    public partial class PlayerControl : UserControl
    {
        // variables used for LibVLCSharp
        public static string[] vlcParams = [
            "--no-video",  // disable video output, (saves some processing power)
            "--aout=directsound",  // force output module to directsound (solve cracking caused by mmdevice)
            "--no-audio-time-stretch",  // disable audio time stretching (enabled by default)
            "--role=music"  // set media role to music
        ];
        private readonly LibVLC vlc = new(options: vlcParams);
        Media media;
        MediaPlayer player;
        long audioDuration = 0;

        // lyrics list for synchronised view
        private List<LyricData> lyrics;

        // create new stopwatch to count player duration
        private readonly OffsetStopwatch sw = new();

        public PlayerControl()
        {
            InitializeComponent();

            // Reset control to initial state
            Lyric1.Content = string.Empty;
            Lyric2.Content = string.Empty;
            Lyric3.Content = string.Empty;
            Control.IsEnabled = false;
            RewindBtn.IsEnabled = false;
            StopBtn.IsEnabled = false;
            ForwardBtn.IsEnabled = false;
            SetBtn.IsEnabled = false;
            TimeBar.IsEnabled = false;
        }

        public void OnClosing()
        {
            // stop all running thread
            if (TimeTracker?.IsAlive == true)
                running = false;

            // close current audio session (if exists)
            if (media?.IsParsed == true) Close();
            
            // dispose VLC player
            vlc.Dispose();
        }

        /// <summary>
        /// Load the audio file (and lyrics if available).
        /// </summary>
        /// <param name="file">Path to the audio file</param>
        /// <returns>Returns true if success</returns>
        public bool Open(string file, List<LyricData> lyrics)
        {
            // open the audio file
            media = new(vlc, file);

            // parse the audio file
            media.Parse();

            // wait for file parse to be done
            for (int i = 0; i <= 100; i++)
            {
                if (media.ParsedStatus == MediaParsedStatus.Done) break;
                else
                    Thread.Sleep(100);
            }

            // throw exception when file parse failed
            if (media.ParsedStatus != MediaParsedStatus.Done)
                throw new FileLoadException("VLC deadlocked while opening selected file!", file);

            // bind media to player
            player = new(media);

            // get audio duration
            audioDuration = media.Duration;
            media.ParseStop();

            // abort file open on error
            if (audioDuration <= 0)
            {
                player.Dispose();
                player = null;
                media.Dispose();
                media = null;

                return false;
            }

            // start player time tracking thread
            if (TimeTracker?.IsAlive != true)
            {
                // create new thread for playing time parsing from MediaPlayer
                TimeTracker = new Thread(new ThreadStart(TimeTrack));
                // mark thread as running
                running = true;
                // start the thread
                TimeTracker.Start();
            }

            // bind events to player
            player.Playing += Player_Playing;
            player.Paused += Player_Paused;
            player.Stopped += Player_Stopped;

            // enable and configure controls
            Control.IsEnabled = true;
            SongLength.Content = LyricTime.From(audioDuration);
            TimeSlider.Maximum = audioDuration;

            // import lyrics data
            this.lyrics = lyrics;

            // create lyrics preview for initial state
            int lyric1Index = 0;
            int lyric2Index = lyrics[0]?.Time.Count > 0 ? 0 :
                (lyrics.Count > 1 ? 1 : -1);
            int lyric3Index = lyrics[0]?.Time.Count > 1 ? 0 : 
                (lyrics[0]?.Time.Count > 1 ? 0 : 
                ((lyrics.Count > 2 ? 2 : -1)));

            return true;
        }

        /// <summary>
        /// Stops the player and close the audio file.
        /// </summary>
        public void Close()
        {
            // show Play button instead
            PlayBtn.Visibility = System.Windows.Visibility.Visible;
            PauseBtn.Visibility = System.Windows.Visibility.Hidden;

            // disable controls
            // Reset control to initial state
            Lyric1.Content = string.Empty;
            Lyric2.Content = string.Empty;
            Lyric3.Content = string.Empty;
            Control.IsEnabled = false;
            RewindBtn.IsEnabled = false;
            StopBtn.IsEnabled = false;
            ForwardBtn.IsEnabled = false;
            SetBtn.IsEnabled = false;
            TimeBar.IsEnabled = false;
            SongLength.Content = "00:00.00";
            TimeSlider.Maximum = 0;

            // unbind events from player
            player.Playing -= Player_Playing;
            player.Paused -= Player_Paused;
            player.Stopped -= Player_Stopped;

            // stop all running thread
            if (TimeTracker?.IsAlive == true)
                running = false;

            // destory the VLC player
            player.Dispose();
            player = null;
            media.Dispose();
            media = null;
        }

        private void RewindBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (player?.Time > -1)
            {
                // rewind player audio duration
                if (player.Time <= 10000) player.Position = 0;
                else player.Time -= 10000;

                // ask thread to synchronise stopwatch
                threadJob.Add("syncStopwatch");
            }
        }

        private void StopBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            player.Stop();
        }

        private void PlayBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // ask VLC to start playing
            player.Play();
        }

        private void PauseBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // ask VLC to stop playing
            player.Pause();
        }

        private void ForwardBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (player?.Time > -1)
            {
                // fast-forward player position
                if (player.Length - player.Time <= 10000) player.Position = 1;
                else player.Time += 10000;

                // ask thread to synchronise stopwatch
                threadJob.Add("syncStopwatch");
            }
        }

        private void TimeSlider_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // lock the timebar
            if (timebarLock == null) timebarLock = this;
        }

        private void TimeSlider_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // set player position to user set position
            float newPosition = (float)TimeSlider.Value / audioDuration;
            player.Position = newPosition;

            // ask thread to change the offset of stopwatch
            // and unlock the timebar
            threadJob.Add("syncStopwatch");
        }
    }
}
