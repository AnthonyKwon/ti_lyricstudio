using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using ti_Lyricstudio.Models;

namespace ti_Lyricstudio.ViewModels
{
    public partial class LyricsPreviewViewModel : ViewModelBase
    {
        // audio player to track time
        private readonly ObservableCollection<LyricData> _lyrics;

        // player to get track current audio duration
        private readonly AudioPlayer _player;

        [ObservableProperty]
        private string _currentLine = string.Empty;

        [ObservableProperty]
        private string _nextLine1 = string.Empty;

        [ObservableProperty]
        private string _nextLine2 = string.Empty;

        // DispatchTimer to track lyrics
        private readonly DispatcherTimer _lyricsTimer = new();

        public LyricsPreviewViewModel(ObservableCollection<LyricData> lyrics ,AudioPlayer player)
        {
            //
            _lyrics = lyrics;

            //
            _player = player;

            _lyricsTimer.Interval = TimeSpan.FromTicks(166667);
            _lyricsTimer.Tick += LyricsTimer_Tick;
        }

        private void LyricsTimer_Tick(object? sender, EventArgs e)
        {
            // do not track if lyric list or player is not initialized
            if (_lyrics == null || _player == null) return;

            int _current = -1, _next1 = -1, _next2 = -1;

            // find current part of lyrics
            for (int i = 0; i < _lyrics.Count; i++)
            {
                for (int j = 0; j < _lyrics[i].Time.Count; j++)
                {
                    // compare current time and current target time
                    if (_player.Time >= _lyrics[i].Time[j].TotalMillisecond)
                    {
                        // ignore current time marker if it's empty or not set
                        if (_lyrics[i].Time == null) continue;
                        if (_lyrics[i].Time[j].IsEmpty) continue;

                        _current = i;
                        _next1 = j + 1 < _lyrics[i].Time.Count ? i :
                            (i + 1 < _lyrics.Count ? i + 1 : -1);
                        _next2 = j + 2 < _lyrics[i].Time.Count ? i :
                            (i + 1 < _lyrics.Count && 1 < _lyrics[i + 1].Time.Count ? i + 1 :
                            (i + 2 < _lyrics.Count ? i + 2 : -1));
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
            CurrentLine = _lyrics.ElementAtOrDefault(_current)?.Text ?? string.Empty;
            NextLine1 = _lyrics.ElementAtOrDefault(_next1)?.Text ?? string.Empty;
            NextLine2 = _lyrics.ElementAtOrDefault(_next2)?.Text ?? string.Empty;
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
