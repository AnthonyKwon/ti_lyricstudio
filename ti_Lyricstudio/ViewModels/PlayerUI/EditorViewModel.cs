using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ti_Lyricstudio.Models;

namespace ti_Lyricstudio.ViewModels
{
    public partial class EditorViewModel : ViewModelBase
    {
        // original lyrics object to track
        private ObservableCollection<LyricData>? _lyrics;

        // flattened lyrics object to be used in UI
        [ObservableProperty]
        private ObservableCollection<FlattenedLyricData>? _flattenedLyrics;

        // currently selected line from editor
        [ObservableProperty]
        private ObservableCollection<int>? _selectedLines = [];

        // player to get track current audio duration
        private readonly AudioPlayer _player;

        // DispatchTimer to track lyrics syncing
        private readonly DispatcherTimer _syncTimer = new();

        // marker if player is playing or paused (used for button canexecute)
        [ObservableProperty]
        private bool _isNotStopped = false;

        // index of the currently active (playing) line
        [ObservableProperty]
        private int _activeLineIndex = -1;

        // width of the Timestamp text block
        [ObservableProperty]
        private double _timestampWidth = 80d;

        // maximum width of the lyrics line
        [ObservableProperty]
        private double _maxLineWidth = -1d;

        // height of the each editor lyrics line
        [ObservableProperty]
        private double _lineHeight = 55d;

        public EditorViewModel(ObservableCollection<LyricData> lyrics, AudioPlayer player)
        {
            // register lyrics collection
            _lyrics = lyrics;
            // regsiter data updated event to lyrics collection
            _lyrics.CollectionChanged += Lyrics_DataUpdated;

            // register audio player and its state change event
            _player = player;
            _player.PlayerStateChangedEvent += Player_StateChanged;

            // initialize lyrics sync timer
            _syncTimer.Interval = TimeSpan.FromTicks(166667);
            _syncTimer.Tick += SyncTimer_Tick;

            // register line selection changed event
            _selectedLines.CollectionChanged += Editor_SelectionChanged;
        }
        
        public void SelectLine(int index)
        {
            // ignore when no line is selected
            if (SelectedLines == null) return;

            // clear the selected line index
            // temporarily detach handler to avoid double-fire
            SelectedLines.CollectionChanged -= Editor_SelectionChanged;
            SelectedLines.Clear();
            SelectedLines.CollectionChanged += Editor_SelectionChanged;

            // add line to selected index
            SelectedLines.Add(index);
        }

        /// <summary>
        /// Action when selected line is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Editor_SelectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // ignore if lyrics or selected line is not set
            if (FlattenedLyrics == null || SelectedLines == null) return;

            // reset all lines to Normal state
            foreach (FlattenedLyricData lyric in FlattenedLyrics)
                lyric.LineState = LyricLineState.Normal;

            // when stopped: compute which lines should be active, then set in a single pass
            // to avoid intermediate false→true flicker on selected lines
            if (!IsNotStopped)
            {
                HashSet<int> activeLineNumbers = [];
                foreach (int index in SelectedLines)
                    activeLineNumbers.Add(FlattenedLyrics[index].Line);

                bool noneSelected = SelectedLines.Count == 0;
                foreach (FlattenedLyricData lyric in FlattenedLyrics)
                    lyric.Active = noneSelected || activeLineNumbers.Contains(lyric.Line);
            }

            // mark chosen lines as selected
            foreach (int index in SelectedLines)
                FlattenedLyrics[index].LineState = LyricLineState.Selected;
        }

        private void Player_StateChanged(object? sender, PlayerState oldState)
        {
            // ignore if workspace is not loaded
            if (_lyrics == null || FlattenedLyrics == null) return;

            switch (_player.State)
            {
                case PlayerState.Playing:
                    // do not enter play mode when lyrics or player is not initialized
                    if (_lyrics == null || _player == null) return;

                    // reset active line when player was stopped
                    if (oldState == PlayerState.Stopped)
                        ActiveLineIndex = -1;

                    // set as not stopped
                    IsNotStopped = true;

                    // unmark all line as inactive
                    foreach (FlattenedLyricData lyric in FlattenedLyrics)
                        lyric.Active = false;

                    // start the Play mode lyric-tracking thread
                    _syncTimer.Start();
                    break;
                case PlayerState.Paused:
                    // set as not stopped
                    IsNotStopped = true;

                    // stop the Play mode lyric-tracking thread
                    _syncTimer.Stop();
                    break;
                default:
                    // mark all line as active
                    foreach (FlattenedLyricData lyric in FlattenedLyrics)
                        lyric.Active = true;

                    // reset active line index
                    ActiveLineIndex = -1;

                    // set as stopped
                    IsNotStopped = false;

                    // stop the Play mode lyric-tracking thread
                    _syncTimer.Stop();
                    break;
            }
        }

        private void Lyrics_DataUpdated(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // ignore if workspace is not loaded
            if (_lyrics == null) return;

            // create new flattened lyrics object
            IEnumerable<FlattenedLyricData> selection = from l in _lyrics
                                                        from t in l.Time
                                                        select new FlattenedLyricData(t, l.Text, _lyrics.IndexOf(l), l.Time.Count > 1);
            FlattenedLyrics = new(selection);
        }

        private void SyncTimer_Tick(object? sender, EventArgs e)
        {
            // ignore if workspace is not loaded
            if (_lyrics == null || FlattenedLyrics == null) return;

            // unmark all line as inactive
            foreach (FlattenedLyricData lyric in FlattenedLyrics)
                lyric.Active = false;

            // find the current line with LINQ
            IEnumerable<FlattenedLyricData> query = from fl in FlattenedLyrics
                        where _player.Time >= fl.Time.TotalMillisecond
                        select fl;

            if (query.Any())
            {
                // mark current line as active
                FlattenedLyricData activeLine = query.Last();
                activeLine.Active = true;

                // set new active line based on current index
                ActiveLineIndex = FlattenedLyrics.IndexOf(activeLine);
            }
        }
    }
}
