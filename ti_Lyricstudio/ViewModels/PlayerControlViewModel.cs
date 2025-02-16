using System;
using Avalonia;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ti_Lyricstudio.Models;

namespace ti_Lyricstudio.ViewModels
{
    public partial class PlayerControlViewModel : ViewModelBase
    {
        // audio player to control
        private readonly AudioPlayer _player;

        // color definition for gradient background
        [ObservableProperty]
        private Avalonia.Media.Color _gradientTransparent;

        // color definition for time label background
        [ObservableProperty]
        private Avalonia.Media.SolidColorBrush _timeLabelBackColor;

        // DispatchTimer to track audio duration of the song
        private readonly DispatcherTimer _playerTimer = new();

        // audio duration of the current song
        [ObservableProperty]
        private long _duration = -1;

        /// <summary>
        /// Gets current position of the audio player. (recommended)
        /// </summary>
        public long GetTime() => _player.Time;

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

        public PlayerControlViewModel(AudioPlayer player)
        {
            //
            _player = player;

            // initialize the player timer
            _playerTimer.Interval = TimeSpan.FromTicks(166667);
            _playerTimer.Tick += PlayerTimer_Tick;

            // set player state to not ready
            State = PlayerState.Nothing;

            // update the player control color scheme
            UpdateControlColor();
            // register the system theme changed event
            Application.Current.ActualThemeVariantChanged += ThemeChanged;
        }

        // event when system theme scheme changed
        private void ThemeChanged(object? sender, EventArgs e) => UpdateControlColor();

        // update the player control color scheme
        private void UpdateControlColor()
        {
            GradientTransparent = new(0, BgBrush.Color.R, BgBrush.Color.G, BgBrush.Color.B);
            TimeLabelBackColor = new(BgBrush.Color, 0.3);
        }

        // event handler for player state change
        // this function should only used as handler of AudioPlayer.PlayerStateChangedEvent,
        // so null warning can be disabled
#pragma warning disable CS8602
        private void PlayerStateChanged(object? sender, PlayerState oldState)
        {
            switch (_player.State)
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
            Dispatcher.UIThread.Post(() => State = _player.State);
        }
#pragma warning restore CS8602

        // audio duration tracker DispatchTimer thread
        private void PlayerTimer_Tick(object? sender, EventArgs e)
        {
            Time = _player?.Time ?? 0;
        }

        // Load the audio file
        public async void Open(string audioPath)
        {
            // unload player if there's audio session already exists
            if (State != PlayerState.Nothing)
                Close();

            // create new audio player and open audio file
            //_player = new AudioPlayer();
            await _player.Open(audioPath);

            // register event handler to player
            _player.PlayerStateChangedEvent += PlayerStateChanged;

            // set the audio duration
            Duration = _player.Duration;

            // set current state as Player's state
            State = _player.State;
        }

        // Unload the audio file
        public void Close()
        {
            // ignore request if player is not initialized
            if (_player == null) return;

            // uninitialize the player
            _player.Close();

            // unregister event handler from player
            _player.PlayerStateChangedEvent -= PlayerStateChanged;

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
            if (_player == null) return;

            // update value of the Time variable
            // this is required to prevent bounding of the UI because of late update
            Time = time;

            // request player to move time position
            _player.Time = time;
        }

        // return true if player is ready
        public bool IsPlayerReady()
        {
            // return false if player is not initialized 
            if (_player == null) return false;
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
                _player?.Pause();
            else
                _player?.Play();
        }

        // stop the player
        [RelayCommand(CanExecute = nameof(IsPlayerPlaying))]
        private void Stop()
        {
            _player?.Stop();
        }

        // rewind the player by 10 seconds
        [RelayCommand(CanExecute = nameof(IsPlayerPlaying))]
        private void Rewind()
        {
            _player?.Rewind();
        }

        // fast-forward the player by 10 seconds
        [RelayCommand(CanExecute = nameof(IsPlayerPlaying))]
        private void FastForward()
        {
            _player?.FastForward();
        }
    }
}
