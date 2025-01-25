using System;
using System.Diagnostics;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ti_Lyricstudio.Models;

namespace ti_Lyricstudio.ViewModels
{
    public partial class PlayerControlViewModel : ViewModelBase
    {
        // color definition for gradient background
        public Avalonia.Media.Color GradientColor { get { return BgBrush.Color; } }
        public Avalonia.Media.Color GradientTransparent { get
            {
                return new(0, BgBrush.Color.R, BgBrush.Color.G, BgBrush.Color.B);
            }
        }

        // color definition for time label background
        public Avalonia.Media.SolidColorBrush TimeLabelBackColor
        {
            get
            {
                return new(BgBrush.Color, 0.3);
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
        [NotifyCanExecuteChangedFor(nameof(RewindCommand), [nameof(StopCommand), nameof(PlayOrPauseCommand), nameof(FastForwardCommand)])]
        private PlayerState _state;

        // marker if player is playing audio (used for button canexecute)
        [ObservableProperty]
        private bool _isPlaying;

        // marker if player is playing or paused (used for button canexecute)
        [ObservableProperty]
        private bool _isPlayingOrPaused;

        public PlayerControlViewModel()
        {
            _playerTimer.Interval = TimeSpan.FromTicks(166667);
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
                    IsPlaying = true;
                    IsPlayingOrPaused = true;
                    // start the UI update thread
                    _playerTimer.Start();
                    break;
                case PlayerState.Paused:
                    IsPlaying = false;
                    IsPlayingOrPaused = true;
                    // stop the UI update thread
                    _playerTimer.Stop();
                    break;
                case PlayerState.Stopped:
                    IsPlaying = false;
                    IsPlayingOrPaused = false;
                    // stop the UI update thread
                    _playerTimer.Stop();

                    // reset the audio duration
                    Time = 0;
                    break;
            }

            // change the player state
            Dispatcher.UIThread.Post(() => State = DataStore.Instance.Player.State);
        }
#pragma warning restore CS8602

        // audio duration tracker DispatchTimer thread
        private void PlayerTimer_Tick(object? sender, EventArgs e)
        {
            Time = DataStore.Instance.Player?.Time ?? 0;
        }

        // Load the audio file
        public async void Open(string audioPath)
        {
            // unload player if there's audio session already exists
            if (State != PlayerState.Nothing)
                Close();

            // create new audio player and open audio file
            DataStore.Instance.Player = new AudioPlayer();
            await DataStore.Instance.Player.Open(audioPath);

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

            // unregister event handler from player
            DataStore.Instance.Player.PlayerStateChangedEvent -= PlayerStateChanged;

            // reset the audio duration
            Duration = -1;

            // set current state as not ready
            State = PlayerState.Nothing;
            IsPlaying = false;
            IsPlayingOrPaused = false;
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

        // return true if player is ready
        public bool IsPlayerReady()
        {
            // return false if player is not initialized 
            if (DataStore.Instance.Player == null) return false;
            // return false if player is not ready
            if (State == PlayerState.Nothing) return false;

            return true;
        }

        // return true if player is playing or paused
        public bool IsPlayerPlaying()
        {
            // return false if player is not ready
            if (IsPlayerReady() == false) return false;
            // return true if player is on playback
            if (State == PlayerState.Playing) return true;
            // return true if player is paused
            if (State == PlayerState.Paused) return true;

            return false;
        }

        // play or pause the player
        [RelayCommand(CanExecute = nameof(IsPlayerReady))]
        private void PlayOrPause()
        {
            if (State == PlayerState.Playing)
                DataStore.Instance.Player?.Pause();
            else
                DataStore.Instance.Player?.Play();
        }

        // stop the player
        [RelayCommand(CanExecute = nameof(IsPlayerPlaying))]
        private void Stop()
        {
            DataStore.Instance.Player?.Stop();
        }

        // rewind the player by 10 seconds
        [RelayCommand(CanExecute = nameof(IsPlayerPlaying))]
        private void Rewind()
        {
            DataStore.Instance.Player?.Rewind();
        }

        // fast-forward the player by 10 seconds
        [RelayCommand(CanExecute = nameof(IsPlayerPlaying))]
        private void FastForward()
        {
            DataStore.Instance.Player?.FastForward();
        }
    }
}
