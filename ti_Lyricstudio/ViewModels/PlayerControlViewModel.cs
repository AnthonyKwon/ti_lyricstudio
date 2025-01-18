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

        // VLC player to use
        private AudioPlayer _player;

        // audio duration of the current song
        [ObservableProperty]
        private long _duration = -1;

        // current state of the audio player 
        [ObservableProperty]
        private PlayerState _state = PlayerState.Nothing;

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

        //
        public void Seek(long time)
        {
            _player.Seek(time);
        }

        // commands for the player button
        public void PlayBtn_Click()
        {
            _player.Play();
            State = PlayerState.Playing;
        }
        public void PauseBtn_Click()
        {
            _player.Pause();
            State = PlayerState.Paused;
        }
        public void StopBtn_Click()
        {
            _player.Stop();
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
