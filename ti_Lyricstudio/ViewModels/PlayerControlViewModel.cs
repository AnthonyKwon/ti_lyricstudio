using System;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using ti_Lyricstudio.Models;

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

        // DispatchTimer and Stopwatch to track audio duration of the song
        private readonly DispatcherTimer _playerTimer = new();
        private readonly OffsetStopwatch _sw = new();

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
        /// WARNING: DO NOT read this variable for use, it's UI-thread bound and unreliable!
        /// Use GetTime() function instead. It returns time directly from Stopwatch.
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

        // event handler for player state change
        private void PlayerStateChanged(object? sender, PlayerState newState)
        {
            switch (newState)
            {
                case PlayerState.Playing:
                    // start the stopwatch and the tracker thread
                    _sw.Start();
                    // start the UI update thread
                    _playerTimer.Start();

                    // change the player state
                    State = PlayerState.Playing;
                    break;
                case PlayerState.Paused:
                    // pause the stopwatch
                    _sw.Stop();
                    // stop the UI update thread
                    _playerTimer.Stop();

                    // change the player state
                    State = PlayerState.Paused;
                    break;
                case PlayerState.Stopped:
                    // reset the stopwatch
                    _sw.Stop();
                    _sw.Reset();
                    _sw.Offset = TimeSpan.Zero;
                    // stop the UI update thread
                    _playerTimer.Stop();

                    // reset the audio duration
                    Time = 0;

                    // change the player state
                    State = PlayerState.Stopped;
                    break;
            }
        }

        // audio duration tracker DispatchTimer thread
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

            // register event handler to player
            _player.PlayerStateChanged += PlayerStateChanged;

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

            // register event handler from player
            _player.PlayerStateChanged += PlayerStateChanged;

            // change player state to not ready
            State = PlayerState.Nothing;

            // reset the audio duration
            Duration = -1;
        }

        // seek the audio player
        public void Seek(long time)
        {
            // request player to move time position
            _player.Time = time;

            // restart the stop and change the offset
            _sw.Restart();
            _sw.Offset = new(time * 10000);
        }

        // commands for the player button
        public void PlayBtn_Click()
        {
            // request to start playback of the audio
            _player.Play();
        }
        public void PauseBtn_Click()
        {
            _player.Pause();
        }
        public void StopBtn_Click()
        {
            // stop the player
            _player.Stop();
        }
        public void RewindBtn_Click()
        {
            _player.Rewind();
            
            // get new audio duration after seek
            long newTime = _player.Time;
            // restart the stop and change the offset
            _sw.Restart();
            _sw.Offset = new(newTime * 10000);
        }
        public void FastForwardBtn_Click()
        {
            _player.FastForward();

            // get new audio duration after seek
            long newTime = _player.Time;
            // restart the stop and change the offset
            _sw.Restart();
            _sw.Offset = new(newTime * 10000);
        }
    }
}
