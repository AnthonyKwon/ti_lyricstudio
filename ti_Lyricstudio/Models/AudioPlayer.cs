using LibVLCSharp.Shared;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ti_Lyricstudio.Models
{
    public class AudioPlayer
    {
        // variables used for LibVLCSharp
        private static readonly string[] vlcParams = [
            "--no-video",  // disable video output, (saves some processing power)
            "--aout=directsound",  // force output module to directsound (solve cracking caused by mmdevice)
            "--no-audio-time-stretch",  // disable audio time stretching (enabled by default)
            "--role=music"  // set media role to music
        ];

        // LibVLC and related objects used the for audio player
        private readonly LibVLC vlc = new(options: vlcParams);
        private Media? media;
        private MediaPlayer? player;

        // workaround: use separate Stopwatch timer to track playback audio duration
        //     update speed of the libVLCsharp is slow, which makes not appopriate for tracking time.
        private readonly OffsetStopwatch _sw = new();
        
        /// <summary>
        /// Indicates whether the system supports high-resolution solution to track playback audio duration.<br/>
        /// If does not, it will retrieve time directly from player which is slow and unreliable.
        /// </summary>
        public static bool HighResolutionTimeSupported { get => Stopwatch.IsHighResolution; }

        // event to fire when VLC state is changed
        public event EventHandler<PlayerState>? PlayerStateChanged;
        private void Player_Playing(object? sender, EventArgs e) => PlayerStateChanged?.Invoke(this, PlayerState.Playing);
        private void Player_Paused(object? sender, EventArgs e) => PlayerStateChanged?.Invoke(this, PlayerState.Paused);
        private void Player_Stopped(object? sender, EventArgs e) => PlayerStateChanged?.Invoke(this, PlayerState.Stopped);

        /// <summary>
        /// Get the audio duration of the audio player. (in milliseconds)
        /// </summary>
        public long Duration { get => _duration; }
        private long _duration = -1;

        /// <summary>
        /// Get or set the time position of the audio player. (in milliseconds)
        /// </summary>
        public long Time
        {
            get
            {
                if (HighResolutionTimeSupported == true)
                    return _sw.ElapsedMilliseconds;
                else
                    return player?.Time ?? 0;
            }
            set
            {
                if (player == null) throw new InvalidOperationException("Player time cannot be set when player is not initialized.");

                // set playback audio duration of the VLC player
                player.Time = value;

                // check if system supports high resolution timer
                if (HighResolutionTimeSupported == true)
                {
                    // synchronise the stopwatch
                    if (value >= _sw.ElapsedMilliseconds)
                    {
                        // player seek to forward, add difference to offset
                        _sw.Offset = (value * (Stopwatch.Frequency / 1000)) - _sw.ElapsedTicksWithoutOffset;
                    }
                    else
                    {
                        // player seek to backward, set offset to current time and reset stopwatch
                        // as stopwatch doesn't support negative offset 
                        _sw.Restart();
                        _sw.Offset = value * (Stopwatch.Frequency / 1000);
                    }
                }
            }
        }

        /// <summary>
        /// Open and load the audio file.
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

            // unregister events to player
            player.Playing += Player_Playing;
            player.Paused += Player_Paused;
            player.EndReached += Player_EndReached;
            player.Stopped += Player_Stopped;

            // set volume to 50% for temporary measure (to keep my ear)
            player.Volume = 50;
        }

        /// <summary>
        /// Unload the audio and reset the player to uninitialized state.
        /// </summary>
        public void Close()
        {
            if (player != null)
            {
                // unregister events from player
                player.Playing -= Player_Playing;
                player.Paused -= Player_Paused;
                player.EndReached -= Player_EndReached;
                player.Stopped -= Player_Stopped;

                // destroy and unset the player session
                player.Dispose();
                player = null;
            }

            if (media != null)
            {
                // destroy and unset the media session
                media.Dispose();
                media = null;
            }

            // check if system supports high resolution timer
            if (HighResolutionTimeSupported == true)
            {
                // stop and reset the stopwatch completely
                _sw.Stop();
                _sw.Reset();
                _sw.Offset = 0;
            }

            // reset the duration
            _duration = -1;
        }

        // workaround: VLC goes to EndReached state when playback is finished.
        //     As VLC tends to do nothing until MediaPlayer.Stop() called,
        //     we will stop automatically when player fired EndReached event.
        private void Player_EndReached(object? sender, EventArgs e)
        {
            // workaround: VLC stalls when MediaPlayer.Stop() runs on it's own thread,
            //     so we need to run this on separate thread.
            Task.Run(() => player?.Stop());
        }

        /// <summary>
        /// Play the loaded audio file.
        /// </summary>
        public void Play()
        {
            // ignore request if player is not initialized
            if (player == null || media == null) return;

            // ignore request if player is not ready or already playing
            if (media.Duration == -1 || player.IsPlaying == true) return;

            // play the loaded audio file
            player.Play();

            // check if system supports high resolution timer
            if (HighResolutionTimeSupported == true)
            {
                // if player had ongoing playback, synchronize the stopwatch with player just in case
                if (player.IsSeekable == true)
                {
                    _sw.Restart();
                    _sw.Offset = player.Time * (Stopwatch.Frequency / 1000);
                }

                // start the stopwatch and the tracker thread
                _sw.Start();
            }
        }

        /// <summary>
        /// Pause the current playback.
        /// </summary>
        public void Pause()
        {
            // ignore request if player is not initialized
            if (player == null || media == null) return;

            // ignore request if player is not currently in playing state
            if (player.IsPlaying == false) return;

            // pause the current playback
            player.Pause();

            // check if system supports high resolution timer
            if (HighResolutionTimeSupported == true)
            {
                // stop the stopwatch
                _sw.Stop();
            }
        }

        /// <summary>
        /// Stop the current playback.
        /// </summary>
        public void Stop()
        {
            // ignore request if player is not initialized
            if (player == null || media == null) return;

            // ignore request if player is not currently in playing or paused state
            if (player.IsSeekable == false) return;

            // stop the current playback
            player.Stop();

            // check if system supports high resolution timer
            if (HighResolutionTimeSupported == true)
            {
                // stop and reset the stopwatch completely
                _sw.Stop();
                _sw.Reset();
                _sw.Offset = 0;
            }
        }

        /// <summary>
        /// Rewind the player by 10 seconds. Player will go to start if time passed less then 10 seconds.
        /// </summary>
        public void Rewind()
        {
            // ignore request if player is not initialized
            if (player == null || media == null) return;

            // ignore request if player is not currently in playing or paused state
            if (player.IsSeekable == false) return;

            // rewind player audio duration
            if (player.Time <= 10000) Time = 0;
            else Time -= 10000;
        }

        /// <summary>
        /// Fast-forward the player by 10 seconds. Player will go to end if time left less then 10 seconds.
        /// </summary>
        public void FastForward()
        {
            // ignore request if player is not initialized
            if (player == null || media == null) return;

            // ignore request if player is not currently in playing or paused state
            if (player.IsSeekable == false) return;

            // fast-forward player position
            if (player.Length - Time <= 10000) Time = player.Length;
            else Time += 10000;
        }
    }
}
