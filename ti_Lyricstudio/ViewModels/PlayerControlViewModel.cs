using System;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using ti_Lyricstudio.Class;

namespace ti_Lyricstudio.ViewModels
{
    public partial class PlayerControlViewModel : ViewModelBase
    {
        // color definition for gradient background
        public Avalonia.Media.Color GradientColor { get { return BackColor.Color; } }
        public Avalonia.Media.Color GradientTransparent { get
            {
                return new(0, BackColor.Color.R, BackColor.Color.G, BackColor.Color.B);
            }
        }

        //
        private DispatcherTimer _playerTimer = new();
        private OffsetStopwatch _sw = new();

        // VLC player to use
        private AudioPlayer? _player;

        // audio duration of the current song
        [ObservableProperty]
        private long _duration = -1;


        /// <summary>
        /// Gets current position of the audio player. (recommended)
        /// </summary>
        public long GetTime() => _sw.ElapsedMilliseconds;

        /// <summary>
        /// Current position of the audio player. (NOT RECOMMENDED)
        /// WARNING: DO NOT read this variable for use, it's UI-thread bound and unreliable!
        /// use GetTime() function instead. It returns time directly from Stopwatch.
        /// </summary>
        [ObservableProperty]
        private long _time = 0;

        // current state of the audio player
        [ObservableProperty]
        private PlayerState _state = PlayerState.Nothing;

        public PlayerControlViewModel()
        {
            _playerTimer.Interval = TimeSpan.FromMilliseconds(33);
            _playerTimer.Tick += PlayerTimer_Tick;
        }

        private void PlayerTimer_Tick(object? sender, EventArgs e)
        {
            Time = _sw.ElapsedMilliseconds;
        }

        // Load the audio file
        public void Open(string audioPath)
        {
            // unload player if there's audio session already exists
            if (State != PlayerState.Nothing)
                Close();

            // create new audio player and open audio file
            _player = new AudioPlayer();
            _player.Open(audioPath);

            // change player state to stopped
            State = PlayerState.Stopped;

            // set the audio duration
            Duration = _player.Duration;
        }

        // Unload the audio file
        public void Close()
        {
            // uninitialize the player
            _player.Close();

            // change player state to not ready
            State = PlayerState.Nothing;

            // reset the audio duration
            Duration = -1;
        }

        // seek the audio player
        public void Seek(long time)
        {
            // request player to move time position
            _player.Seek(time);

            // restart the stop and change the offset
            _sw.Restart();
            _sw.Offset = new(time * 10000);
        }

        // commands for the player button
        public void PlayBtn_Click()
        {
            // request to start playback of the audio
            _player.Play();

            // start the stopwatch and the tracker thread
            _sw.Start();
            _playerTimer.Start();

            // change the player state
            State = PlayerState.Playing;
        }
        public void PauseBtn_Click()
        {
            _player.Pause();
            _sw.Stop();
            _playerTimer.Stop();
            State = PlayerState.Paused;
        }
        public void StopBtn_Click()
        {
            // stop the player
            _player.Stop();
            _playerTimer.Stop();

            // reset the stopwatch
            _sw.Stop();
            _sw.Reset();
            _sw.Offset = TimeSpan.Zero;

            Time = 0;
            State = PlayerState.Stopped;
        }
        public void RewindBtn_Click()
        {
            _player.Rewind();
        }
        public void FastForwardBtn_Click()
        {
            _player.FastForward();
        }
    }
}
