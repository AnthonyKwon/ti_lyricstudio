using System;
using System.Linq;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

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

            // find current part of lyrics
            for (int i = 0; i < DataStore.Instance.Lyrics.Count; i++)
            {
                for (int j = 0; j < DataStore.Instance.Lyrics[i].Time.Count; j++)
                {
                    // compare current time and current target time
                    if (DataStore.Instance.Player.Time >= DataStore.Instance.Lyrics[i].Time[j].TotalMillisecond)
                    {
                        // ignore current time marker if it's empty or not set
                        if (DataStore.Instance.Lyrics[i].Time == null) continue;
                        if (DataStore.Instance.Lyrics[i].Time[j].IsEmpty) continue;

                        _current = i;
                        _next1 = j + 1 < DataStore.Instance.Lyrics[i].Time.Count ? i :
                            (i + 1 < DataStore.Instance.Lyrics.Count ? i + 1 : -1);
                        _next2 = j + 2 < DataStore.Instance.Lyrics[i].Time.Count ? i :
                            (i + 1 < DataStore.Instance.Lyrics.Count && 1 < DataStore.Instance.Lyrics[i + 1].Time.Count ? i + 1 :
                            (i + 2 < DataStore.Instance.Lyrics.Count ? i + 2 : -1));
                    }
                    else
                        break;
                }
            }

            // show next lines only if currentLine is not reached to first line
            if (_current + _next1 + _next2 == -3)
            {
                _next1 = 0;
                _next2 = 1;
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
            _lyricsTimer.Stop();

            CurrentLine = string.Empty;
            NextLine1 = string.Empty;
            NextLine2 = string.Empty;
        }
    }
}
