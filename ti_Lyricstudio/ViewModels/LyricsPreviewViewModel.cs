using System;
using System.Linq;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using ti_Lyricstudio.Models;

namespace ti_Lyricstudio.ViewModels
{
    public partial class LyricsPreviewViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _currentLine = string.Empty;

        [ObservableProperty]
        private string _nextLine1 = string.Empty;

        [ObservableProperty]
        private string _nextLine2 = string.Empty;

        // DispatchTimer to track lyrics
        private readonly DispatcherTimer _lyricsTimer = new();

        public LyricsPreviewViewModel()
        {
            _lyricsTimer.Interval = TimeSpan.FromTicks(166667);
            _lyricsTimer.Tick += LyricsTimer_Tick;
        }

        private void LyricsTimer_Tick(object? sender, EventArgs e)
        {
            // do not track if lyric list or player is not initialized
            if (DataStore.Instance.Lyrics == null || DataStore.Instance.Player == null) return;

            int _current = -1, _next1 = -1, _next2 = -1;
            if (DataStore.Instance.Player.State == PlayerState.Stopped ||
                DataStore.Instance.Player.Time < DataStore.Instance.Lyrics[0].Time[0].TotalMillisecond)
            {
                // show next lines only if currentLine is not reached to first line
                _next1 = 0;
                _next2 = 1;
            }
            else if (DataStore.Instance.Player.Time >= DataStore.Instance.Lyrics[^2].Time[^1].TotalMillisecond)
            {
                // show last line only if only last line has left
                _current = DataStore.Instance.Lyrics.Count - 2;
            }

            // find current part of lyrics (finish the loop if already matched)
            for (int i = 0; i < DataStore.Instance.Lyrics.Count - 1 && _current + _next1 + _next2 == -3; i++)
            {

                // marker to check if matching lyric has found
                for (int j = 0; j < DataStore.Instance.Lyrics[i].Time.Count; j++)
                {
                    // skip until timestamp larger than current time matches
                    if (DataStore.Instance.Player.Time <= DataStore.Instance.Lyrics[i].Time[j].TotalMillisecond)
                    {
                        // set index of the current line
                        // set previous line of the matched timestamp as target line if timestamp is in the first index,
                        // or current line if it's not
                        _current = j > 0 ? i : i - 1;
                        // set index of the first next line
                        // set current line of the matched timestamp as target line
                        _next1 = i;
                        // set index of the second next line
                        // set next line of the matched timestamp as target line if timestamp is in the last index,
                        // or current line if it's not
                        _next2 = j == DataStore.Instance.Lyrics[i].Time.Count - 1 ? i + 1 : i;

                        // finish the loop if match found
                        break;
                    }
                }
            }

            // update each line to found lyrics
            CurrentLine = DataStore.Instance.Lyrics.ElementAtOrDefault(_current)?.Text ?? string.Empty;
            NextLine1 = DataStore.Instance.Lyrics.ElementAtOrDefault(_next1)?.Text ?? string.Empty;
            NextLine2 = DataStore.Instance.Lyrics.ElementAtOrDefault(_next2)?.Text ?? string.Empty;
        }

        public void Start()
        {
            _lyricsTimer.Start();
        }

        public void Stop()
        {
            CurrentLine = string.Empty;
            NextLine1 = string.Empty;
            NextLine2 = string.Empty;   

            _lyricsTimer.Stop();
        }
    }
}
