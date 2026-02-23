using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using ti_Lyricstudio.Models;

namespace ti_Lyricstudio.ViewModels
{
    public partial class PlayerPanelViewModel : ViewModelBase
    {
        // audio player to control
        private readonly AudioPlayer player;

        // DispatchTimer to track audio duration of the song
        private readonly DispatcherTimer playerTimer = new();

        // audio duration of the current song
        [ObservableProperty]
        private long _duration = -1;

        /// <summary>
        /// WARNING: DO NOT read this variable for use (except in UI), it's UI-thread bound and unreliable!
        /// </summary>
        [ObservableProperty]
        private long _time = 0;

        // current state of the audio player
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RewindCommand), [nameof(StopCommand), nameof(PlayOrPauseCommand), nameof(FastForwardCommand)])]
        private PlayerState _state;

        // binding for the title of media title
        [ObservableProperty]
        private string? _songTitle;

        // binding for the title of media album info
        [ObservableProperty]
        private string? _songAlbumInfo;

        // binding for the title of media artwork
        [ObservableProperty]
        private Bitmap? _songArtwork;

        // properties to check player states
        public bool IsReady => State != PlayerState.Nothing;
        public bool IsPlaying => State == PlayerState.Playing;
        public bool IsNotStopped => State is PlayerState.Playing or PlayerState.Paused;

        public PlayerPanelViewModel(AudioPlayer _player)
        {
            // initialize the audio player
            player = _player;

            // initialize the player timer
            playerTimer.Interval = TimeSpan.FromTicks(166667);
            playerTimer.Tick += PlayerTimer_Tick;

            // subscribe to player state changes
            player.PlayerStateChangedEvent += PlayerStateChanged;

            // set player state to not ready
            State = PlayerState.Nothing;
        }

        // notify computed properties when State changes
        partial void OnStateChanged(PlayerState value)
        {
            OnPropertyChanged(nameof(IsReady));
            OnPropertyChanged(nameof(IsPlaying));
            OnPropertyChanged(nameof(IsNotStopped));
        }

        // event handler for player state change
        private void PlayerStateChanged(object? sender, PlayerState oldState)
        {
            Dispatcher.UIThread.Post(() =>
            {
                switch (player.State)
                {
                    case PlayerState.Playing:
                        // start the UI update thread
                        playerTimer.Start();
                        break;
                    case PlayerState.Paused:
                        // stop the UI update thread
                        playerTimer.Stop();
                        break;
                    case PlayerState.Stopped:
                        // stop the UI update thread
                        playerTimer.Stop();

                        // set duration on first transition from Nothing (after Open)
                        if (oldState == PlayerState.Nothing)
                            Duration = player.Duration;

                        // reset the playback time
                        Time = 0;
                        break;
                    case PlayerState.Nothing:
                        // stop the UI update thread
                        playerTimer.Stop();

                        // reset the audio duration and time
                        Duration = -1;
                        Time = 0;
                        break;
                }

                // sync the player state
                State = player.State;
            });
        }

        // audio duration tracker DispatchTimer thread
        private void PlayerTimer_Tick(object? sender, EventArgs e)
        {
            Time = player.Time;
        }

        // seek the audio player
        public void Seek(long time)
        {
            // update value of the Time variable
            // this is required to prevent bounding of the UI because of late update
            Time = time;

            // request player to move time position
            player.Time = time;
        }

        // return true if player is ready (used for CanExecute)
        public bool IsPlayerReady() => State != PlayerState.Nothing;

        // return true if player is playing or paused (used for CanExecute)
        public bool IsPlayerPlaying() => State is PlayerState.Playing or PlayerState.Paused;

        // play or pause the player
        [RelayCommand(CanExecute = nameof(IsPlayerReady))]
        private void PlayOrPause()
        {
            if (State == PlayerState.Playing)
                player.Pause();
            else
                player.Play();
        }

        // stop the player
        [RelayCommand(CanExecute = nameof(IsPlayerPlaying))]
        private void Stop()
        {
            player.Stop();
        }

        // rewind the player by 10 seconds
        [RelayCommand(CanExecute = nameof(IsPlayerPlaying))]
        private void Rewind()
        {
            player.Rewind();
        }

        // fast-forward the player by 10 seconds
        [RelayCommand(CanExecute = nameof(IsPlayerPlaying))]
        private void FastForward()
        {
            player.FastForward();
        }
    }
}