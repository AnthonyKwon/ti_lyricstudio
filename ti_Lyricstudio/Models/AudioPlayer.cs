using Avalonia.Media;
using Avalonia.Media.Imaging;
using ImageMagick;
using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ti_Lyricstudio.Models
{
    public class AudioPlayer : IAudioPlayer
    {
#if OS_WINDOWS
        // variables used for LibVLCSharp (for windows)
        private static readonly string[] vlcParams = [
            "--no-video",  // disable video output, (saves some processing power)
            "--aout=directsound",  // force output module to directsound (solve cracking caused by mmdevice)
            "--no-audio-time-stretch",  // disable audio time stretching (enabled by default)
            "--audio-replay-gain-mode=track"  // enable ReplayGain as track mode 
        ];
#elif OS_LINUX
        // variables used for LibVLCSharp (for linux)
        private static readonly string[] vlcParams = [
            "--no-video",  // disable video output, (saves some processing power)
            "--aout=pulse",  // force output module to directsound (solve cracking caused by mmdevice)
            "--no-audio-time-stretch",  // disable audio time stretching (enabled by default)
            "--audio-replay-gain-mode=track"  // enable ReplayGain as track mode 
        ];
#else
        // variables used for LibVLCSharp (for other platforms)
        private static readonly string[] vlcParams = [
            "--no-video",  // disable video output, (saves some processing power)
            "--no-audio-time-stretch",  // disable audio time stretching (enabled by default)
            "--audio-replay-gain-mode=track"  // enable ReplayGain as track mode 
        ];
#endif

        // path to the audio file
        private string? filePath;

        // LibVLC and related objects used the for audio player
        private readonly LibVLC vlc = new(options: vlcParams);
        private Media? media;
        private MediaPlayer? player;

        // workaround: use separate Stopwatch timer to track playback audio duration
        //     update speed of the libVLCsharp is slow, which makes not appopriate for tracking time.
        private readonly OffsetStopwatch _sw = new();

        /// <summary>
        /// Get the audio duration of the audio player. (in milliseconds)
        /// </summary>
        public long Duration { get => _duration; }
        private long _duration = -1;

        /// <summary>
        /// Indicates whether the system supports high-resolution solution to track playback audio duration.<br/>
        /// If does not, it will retrieve time directly from player which is slow and unreliable.
        /// </summary>
        public static bool HighResolutionTimeSupported { get => Stopwatch.IsHighResolution; }

        // current state of the audio player
        public PlayerState State { get => _state; }
        private PlayerState _state = PlayerState.Nothing;

        /// <summary>
        /// Media playing state of the player has changed<br/>
        /// Previous <see cref="PlayerState"/> is provided as parameter.
        /// </summary>
        public event EventHandler<PlayerState>? PlayerStateChangedEvent;
        private void PlayerStateChanged(PlayerState newState)
        {
            // save old state to variable temporary
            PlayerState oldState = _state;

            // change the player state to new state
            _state = newState;

            // raise the player state changed event with old state
            PlayerStateChangedEvent?.Invoke(this, oldState);
        }
        private void Player_Playing(object? sender, EventArgs e) => PlayerStateChanged(PlayerState.Playing);
        private void Player_Paused(object? sender, EventArgs e) => PlayerStateChanged(PlayerState.Paused);
        private void Player_Stopped(object? sender, EventArgs e) => PlayerStateChanged(PlayerState.Stopped);

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
                        _sw.Reset();
                        _sw.Offset = value * (Stopwatch.Frequency / 1000);

                        // start timer only when player is playing audio
                        if (_state == PlayerState.Playing) _sw.Start();
                    }
                }
            }
        }

        /// <summary>
        /// Open and load the audio file.
        /// </summary>
        /// <param name="file">Path to the audio file</param>
        public async Task Open(string file)
        {
            // open the audio file
            media = new(vlc, file);

            // parse the audio file
            await media.Parse();

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

            // change player state to stopped
            _state = PlayerState.Stopped;

            // set file path to current file
            filePath = file;
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

            // change player state to not ready
            _state = PlayerState.Nothing;

            // unset the file path
            filePath = null;
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

        /// <summary>
        /// Parse information of the current media.
        /// </summary>
        public IAudioInfo ParseAudioInfo()
        {
            // return empty data if player is not initialized
            if (media == null) return new AudioInfo();

            // load tag from current file
            TagLib.File file = TagLib.File.Create(filePath);

            // parse current song and album data from tag
            string title = file.Tag.Title;
            string artist = file.Tag.FirstPerformer;
            string album = file.Tag.Album;

            // parse raw first artwork data from tag
            byte[] rawArtwork = file.Tag.Pictures[0].Data.Data;
            MemoryStream rawArtworkStream = new(rawArtwork);

            // create artwork bitmap from raw data
            Bitmap artwork = new(rawArtworkStream);

            return new AudioInfo(title, artist, album, artwork);
        }

        /// <summary>
        /// Parse dominent color used by gradient from artwork.
        /// </summary>
        /// <returns></returns>
        public List<Color>? GetGradientColors(string? path)
        {
            // return empty data if player is not initialized
            if ((filePath == null && path == null) || media == null) return null;

            // load tag from current file
            TagLib.File file = TagLib.File.Create(path ?? filePath);

            // parse raw first artwork data from tag
            byte[] rawArtwork = file.Tag.Pictures[0].Data.Data;

            // create ImageMagick instance of artwork
            MagickImage magick = new(rawArtwork);
            // define K-means settings
            KmeansSettings kmeansSettings = new();
            kmeansSettings.NumberColors = 3;
            // scale down image to 32x32
            magick.Scale(32, 32);
            // apply K-means to reduce palette and extract dominant color
            magick.Kmeans(kmeansSettings);
            // create histogram of artwork
            IReadOnlyDictionary<IMagickColor<byte>, uint> histogram = magick.Histogram();
            List<IMagickColor<byte>> colors = histogram.Keys.ToList();

            // add extracted colors to return list
            List<Color> avaColors = new();
            for (int i = 0; i < (colors.Count > 3 ? 3 : colors.Count - 1); i++)
                avaColors.Add(new(colors[i].A, colors[i].R, colors[i].G, colors[i].B));

            // return the extracted color
            return avaColors;
        }
    }
}
