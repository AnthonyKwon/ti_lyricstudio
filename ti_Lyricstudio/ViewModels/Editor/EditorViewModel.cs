using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using ti_Lyricstudio.Models;

namespace ti_Lyricstudio.ViewModels.Editor
{
    public partial class EditorViewModel : ViewModelBase
    {
        // original lyrics object to track
        private ObservableCollection<LyricData> _lyrics;

        // flattened lyrics object to be used in UI
        [ObservableProperty]
        private ObservableCollection<FlattenedLyricData> _flattenedLyrics;

        // player to get track current audio duration
        private readonly AudioPlayer _player;

        // DispatchTimer to track lyrics syncing
        private readonly DispatcherTimer _syncTimer = new();

        public EditorViewModel(ObservableCollection<LyricData> lyrics, AudioPlayer player)
        {
            // register lyrics collection
            _lyrics = lyrics;
            // regsiter data updated event to lyrics collection
            _lyrics.CollectionChanged += Lyrics_DataUpdated;

            // register audio player
            _player = player;

            // initialize lyrics sync timer
            _syncTimer.Interval = TimeSpan.FromTicks(166667);
            _syncTimer.Tick += SyncTimer_Tick;
        }

        /// <summary>
        /// Action when file opened.
        /// </summary>
        public void FileOpened()
        {
            // register state changed event to audio player
            //TODO: move to separate file opened function
            _player.PlayerStateChangedEvent += Player_StateChanged;
        }

        /// <summary>
        /// Action when file closed.
        /// </summary>
        public void FileClosed()
        {
            // register state changed event to audio player
            //TODO: move to separate file opened function
            _player.PlayerStateChangedEvent -= Player_StateChanged;
        }

        //
        private void Player_StateChanged(object? sender, PlayerState e)
        {
            switch (_player.State)
            {
                case PlayerState.Playing:
                    // do not enter play mode when lyrics or player is not initialized
                    if (_lyrics == null || _player == null) return;

                    // unmark all line as inactive
                    foreach (FlattenedLyricData lyric in FlattenedLyrics)
                        lyric.Active = false;

                    // enter play mode
                    _syncTimer.Start();
                    break;
                default:
                    // mark all line as active
                    foreach (FlattenedLyricData lyric in FlattenedLyrics)
                        lyric.Active = true;

                    // exit play mode
                    _syncTimer.Stop();
                    break;
            }
        }

        private void Lyrics_DataUpdated(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // create new flattened lyrics object
            IEnumerable<FlattenedLyricData> selection = from l in _lyrics
                                                        from t in l.Time
                                                        select new FlattenedLyricData(t, l.Text, _lyrics.IndexOf(l), l.Time.Count > 1);
            FlattenedLyrics = new(selection);
        }

        private void SyncTimer_Tick(object? sender, EventArgs e)
        {
            // unmark all line as inactive
            foreach (FlattenedLyricData lyric in FlattenedLyrics)
                lyric.Active = false;

            // find the current line with LINQ
            IEnumerable<FlattenedLyricData> query = from fl in FlattenedLyrics
                        where _player.Time >= fl.Time.TotalMillisecond
                        select fl;

            // mark current line as active
            if (query.Any()) query.Last().Active = true;
        }
    }
}
