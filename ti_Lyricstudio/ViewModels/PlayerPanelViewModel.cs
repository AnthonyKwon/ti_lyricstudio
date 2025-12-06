using Avalonia.Controls;
using Avalonia.Media;
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
        /// WARNING: DO NOT read this variable for use, it's UI-thread bound and unreliable!
        /// Use GetTime() function instead. It returns time directly from Stopwatch.
        /// </summary>
        [ObservableProperty]
        private long _time = 0;

        // binding to check if player is currently initialized
        [ObservableProperty]
        private bool _isReady;

        // marker if player is playing audio (used for button canexecute)
        [ObservableProperty]
        private bool _isPlaying;

        // marker if player is playing or paused (used for button canexecute)
        [ObservableProperty]
        private bool _isNotStopped;

        // binding for the title of media title
        [ObservableProperty]
        private string _songTitle = "Sample Title";

        // binding for the title of media album info
        [ObservableProperty]
        private string _songAlbumInfo = "Sample Artwork";

        // binding for the title of media artwork
        [ObservableProperty]
        private Bitmap _songArtwork;

        public PlayerPanelViewModel(AudioPlayer _player)
        {
            // initialize the audio player
            player = _player;

            // initialize the player timer
            playerTimer.Interval = TimeSpan.FromTicks(166667);
            playerTimer.Tick += PlayerTimer_Tick;

            // set player state to not ready
            State = PlayerState.Nothing;
        }

        // current state of the audio player
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RewindCommand), [nameof(StopCommand), nameof(PlayOrPauseCommand), nameof(FastForwardCommand)])]
        private PlayerState _state;

        // event handler for player state change
        // this function should only used as handler of AudioPlayer.PlayerStateChangedEvent,
        // so null warning can be disabled
#pragma warning disable CS8602
        private void PlayerStateChanged(object? sender, PlayerState oldState)
        {
            switch (player.State)
            {
                case PlayerState.Playing:
                    IsPlaying = true;
                    IsNotStopped = true;
                    // start the UI update thread
                    playerTimer.Start();
                    break;
                case PlayerState.Paused:
                    IsPlaying = false;
                    IsNotStopped = true;
                    // stop the UI update thread
                    playerTimer.Stop();
                    break;
                case PlayerState.Stopped:
                    IsPlaying = false;
                    IsNotStopped = false;
                    // stop the UI update thread
                    playerTimer.Stop();

                    // reset the audio duration
                    Time = 0;
                    break;
            }

            // change the player state
            Dispatcher.UIThread.Post(() => State = player.State);
        }
#pragma warning restore CS8602

        // audio duration tracker DispatchTimer thread
        private void PlayerTimer_Tick(object? sender, EventArgs e)
        {
            Time = player?.Time ?? 0;
        }

        // Load the audio file
        public async void Open(string audioPath)
        {
            // unload player if there's audio session already exists
            if (State != PlayerState.Nothing)
                Close();

            // create new audio player and open audio file
            //_player = new AudioPlayer();
            await player.Open(audioPath);

            // register event handler to player
            player.PlayerStateChangedEvent += PlayerStateChanged;

            // parse media info
            IAudioInfo info = player.ParseAudioInfo();
            SongTitle = info.Title;
            SongAlbumInfo = $"{info.Artist} – {info.Album}";
            SongArtwork = info.Artwork;


            // set the audio duration
            Duration = player.Duration;

            // set current state as Player's state
            State = player.State;
            IsReady = true;
        }

        // Unload the audio file
        public void Close()
        {
            // ignore request if player is not initialized
            if (player == null) return;

            // uninitialize the player
            player.Close();

            // unregister event handler from player
            player.PlayerStateChangedEvent -= PlayerStateChanged;

            // reset the audio duration
            Duration = -1;

            // set current state as not ready
            State = PlayerState.Nothing;
            IsReady = false;
            IsPlaying = false;
            IsNotStopped = false;
        }

        // seek the audio player
        public void Seek(long time)
        {
            // ignore request if player is not initialized
            if (player == null) return;

            // update value of the Time variable
            // this is required to prevent bounding of the UI because of late update
            Time = time;

            // request player to move time position
            player.Time = time;
        }

        // return true if player is ready
        public bool IsPlayerReady()
        {
            // return false if player is not initialized 
            if (player == null) return false;
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
                player?.Pause();
            else
                player?.Play();
        }

        // stop the player
        [RelayCommand(CanExecute = nameof(IsPlayerPlaying))]
        private void Stop()
        {
            player?.Stop();
        }

        // rewind the player by 10 seconds
        [RelayCommand(CanExecute = nameof(IsPlayerPlaying))]
        private void Rewind()
        {
            player?.Rewind();
        }

        // fast-forward the player by 10 seconds
        [RelayCommand(CanExecute = nameof(IsPlayerPlaying))]
        private void FastForward()
        {
            player?.FastForward();
        }
    }
}
