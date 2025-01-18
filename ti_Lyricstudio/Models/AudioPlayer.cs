using LibVLCSharp.Shared;
using System;
using System.IO;
using System.Threading;

namespace ti_Lyricstudio.Models
{
    public class AudioPlayer
    {
        // variables used for LibVLCSharp
        private static string[] vlcParams = [
            "--no-video",  // disable video output, (saves some processing power)
            "--aout=directsound",  // force output module to directsound (solve cracking caused by mmdevice)
            "--no-audio-time-stretch",  // disable audio time stretching (enabled by default)
            "--role=music"  // set media role to music
        ];

        // LibVLC and related objects used the for audio player
        private readonly LibVLC vlc = new(options: vlcParams);
        private Media media;
        private MediaPlayer player;

        // event to fire when VLC state is changed
        public event EventHandler<PlayerState>? PlayerStateChanged;
        private void Player_Playing(object? sender, EventArgs e) => PlayerStateChanged?.Invoke(this, PlayerState.Playing);
        private void Player_Paused(object? sender, EventArgs e) => PlayerStateChanged?.Invoke(this, PlayerState.Paused);
        private void Player_Stopped(object? sender, EventArgs e) => PlayerStateChanged?.Invoke(this, PlayerState.Stopped);

        /// <summary>
        /// Audio duration of the player.
        /// </summary>
        public long Duration { get => _duration; }
        private long _duration = -1;

        /// <summary>
        /// Load the audio file.
        /// </summary>
        /// <param name="file">Path to the audio file</param>
        public void Open(string file)
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
            _duration = media.Duration;
            media.ParseStop();

            // abort file open on error
            if (_duration <= 0)
            {
                Close();
                return;
            }

            // bind events to player
            player.Playing += Player_Playing;
            player.Paused += Player_Paused;
            player.Stopped += Player_Stopped;

            // set volume to 50% for temporary measure (to keep my ear)
            player.Volume = 50;
        }

        /// <summary>
        /// Unload the audio and reset the player to uninitialized state.
        /// </summary>
        public void Close()
        {
            // reset the player and media
            player.Dispose();
            player = null;
            media.Dispose();
            media = null;

            // reset the duration
            _duration = -1;
        }

        /// <summary>
        /// Play the loaded audio file.
        /// </summary>
        public void Play()
        {
            // ignore request if player is not ready or already playing
            if (media.Duration == -1 || player.IsPlaying == true) return;

            // play the loaded audio file
            player.Play();
        }

        /// <summary>
        /// Pause the current playback.
        /// </summary>
        public void Pause()
        {
            // ignore request if player is not currently in playing state
            if (player.IsPlaying == false) return;

            // pause the current playback
            player.Pause();
        }

        /// <summary>
        /// Stop the current playback.
        /// </summary>
        public void Stop()
        {
            // ignore request if player is not currently in playing or paused state
            if (player.IsSeekable == false) return;

            // stop the current playback
            player.Stop();
        }

        /// <summary>
        /// Rewind the player by 10 seconds. Player will go to start if time passed less then 10 seconds.
        /// </summary>
        public void Rewind()
        {
            // ignore request if player is not currently in playing or paused state
            if (player.IsSeekable == false) return;

            // rewind player audio duration
            if (player.Time <= 10000) player.Position = 0;
            else player.Time -= 10000;
        }

        /// <summary>
        /// Fast-forward the player by 10 seconds. Player will go to end if time left less then 10 seconds.
        /// </summary>
        public void FastForward()
        {
            // ignore request if player is not currently in playing or paused state
            if (player.IsSeekable == false) return;

            // fast-forward player position
            if (player.Length - player.Time <= 10000) player.Position = 1;
            else player.Time += 10000;
        }

        /// <summary>
        /// Move the time position of the player.
        /// </summary>
        /// <param name="time">Time position to move</param>
        public void Seek(long time)
        {
            if (player.IsSeekable == false) return;

            // set time position of the VLC to specified one
            player.Time = time;
        }
    }
}
