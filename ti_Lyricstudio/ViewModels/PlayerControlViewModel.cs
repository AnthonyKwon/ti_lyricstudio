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

        // color definition for time label background
        public Avalonia.Media.SolidColorBrush TimeLabelBackColor
        {
            get
            {
                return new(BackColor.Color, 0.3);
            }
        }

        // DispatchTimer to track audio duration of the song
        private readonly DispatcherTimer _playerTimer = new();

        // audio duration of the current song
        [ObservableProperty]
        private long _duration = -1;

        /// <summary>
        /// Gets current position of the audio player. (recommended)
        /// </summary>
        public static long GetTime() => DataStore.Instance.Player?.Time ?? -1;

        /// <summary>
        /// WARNING: DO NOT read this variable for use, it's UI-thread bound and unreliable!
        /// Use GetTime() function instead. It returns time directly from Stopwatch.
        /// </summary>
        [ObservableProperty]
        private long _time = 0;

        // current state of the audio player
        [ObservableProperty]
        private PlayerState _state;

        public PlayerControlViewModel()
        {
            _playerTimer.Interval = TimeSpan.FromMilliseconds(33);
            _playerTimer.Tick += PlayerTimer_Tick;

            State = PlayerState.Nothing;
        }

        // event handler for player state change
        // this function should only used as handler of AudioPlayer.PlayerStateChangedEvent,
        // so null warning can be disabled
#pragma warning disable CS8602
        private void PlayerStateChanged(object? sender, PlayerState oldState)
        {
            switch (DataStore.Instance.Player.State)
            {
                case PlayerState.Playing:
                    // start the UI update thread
                    _playerTimer.Start();

                    // change the player state
                    State = PlayerState.Playing;
                    break;
                case PlayerState.Paused:
                    // stop the UI update thread
                    _playerTimer.Stop();

                    // change the player state
                    State = PlayerState.Paused;
                    break;
                case PlayerState.Stopped:
                    // stop the UI update thread
                    _playerTimer.Stop();

                    // reset the audio duration
                    Time = 0;

                    // change the player state
                    State = PlayerState.Stopped;
                    break;
            }

            State = DataStore.Instance.Player.State;
        }
#pragma warning restore CS8602

        // audio duration tracker DispatchTimer thread
        private void PlayerTimer_Tick(object? sender, EventArgs e)
        {
            Time = DataStore.Instance.Player?.Time ?? 0;
        }

        // Load the audio file
        public void Open(string audioPath)
        {
            // unload player if there's audio session already exists
            if (State != PlayerState.Nothing)
                Close();

            // create new audio player and open audio file
            DataStore.Instance.Player = new AudioPlayer();
            DataStore.Instance.Player.Open(audioPath);

            // register event handler to player
            DataStore.Instance.Player.PlayerStateChangedEvent += PlayerStateChanged;

            // set the audio duration
            Duration = DataStore.Instance.Player.Duration;

            // set current state as Player's state
            State = DataStore.Instance.Player.State;
        }

        // Unload the audio file
        public void Close()
        {
            // ignore request if player is not initialized
            if (DataStore.Instance.Player == null) return;

            // uninitialize the player
            DataStore.Instance.Player.Close();

            // register event handler from player
            DataStore.Instance.Player.PlayerStateChangedEvent += PlayerStateChanged;

            // reset the audio duration
            Duration = -1;

            // set current state as not ready
            State = PlayerState.Nothing;
        }

        // seek the audio player
        public void Seek(long time)
        {
            // ignore request if player is not initialized
            if (DataStore.Instance.Player == null) return;

            // update value of the Time variable
            // this is required to prevent bounding of the UI because of late update
            Time = time;

            // request player to move time position
            DataStore.Instance.Player.Time = time;
        }

        // commands for the player button
        public void PlayBtn_Click()
        {
            // ignore request if player is not initialized
            if (DataStore.Instance.Player == null) return;

            // request to start playback of the audio
            DataStore.Instance.Player.Play();
        }
        public void PauseBtn_Click()
        {
            // ignore request if player is not initialized
            if (DataStore.Instance.Player == null) return;

            DataStore.Instance.Player.Pause();
        }
        public void StopBtn_Click()
        {
            // ignore request if player is not initialized
            if (DataStore.Instance.Player == null) return;

            // stop the player
            DataStore.Instance.Player.Stop();
        }
        public void RewindBtn_Click()
        {
            // ignore request if player is not initialized
            if (DataStore.Instance.Player == null) return;

            // rewind the player by 10 seconds
            DataStore.Instance.Player.Rewind();
        }
        public void FastForwardBtn_Click()
        {
            // ignore request if player is not initialized
            if (DataStore.Instance.Player == null) return;

            // fast-forward the player by 10 seconds
            DataStore.Instance.Player.FastForward();
        }
    }
}
