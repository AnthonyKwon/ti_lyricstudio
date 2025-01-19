using System;
using System.Diagnostics;
using System.Linq;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using LibVLCSharp.Shared;
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
            _lyricsTimer.Interval = TimeSpan.FromMilliseconds(67);
            _lyricsTimer.Tick += LyricsTimer_Tick;
        }

        private void LyricsTimer_Tick(object? sender, EventArgs e)
        {
            // 
            if (DataStore.Instance.Lyrics == null || DataStore.Instance.Player == null) return;

            //
            if (DataStore.Instance.Player.IsPlaying == false) return;

            // get current lyric time
            LyricTime currentTime = LyricTime.From(DataStore.Instance.Player.Time);

            // find current part of lyrics
            int lyric1Index = -1, lyric2Index = -1, lyric3Index = -1;
            for (int i = 0; i < DataStore.Instance.Lyrics.Count; i++)
            {
                // marker to check if matching lyric has found
                for (int j = 0; j < DataStore.Instance.Lyrics[i].Time.Count; j++)
                {
                    // compare current time and current target time
                    if (LyricTime.Compare(currentTime, DataStore.Instance.Lyrics[i].Time[j]) != LyricTime.Comparator.RightIsBigger)
                    {
                        lyric1Index = i;
                        lyric2Index = j + 1 < DataStore.Instance.Lyrics[i].Time.Count ? i :
                            (i + 1 < DataStore.Instance.Lyrics.Count ? i + 1 : -1);
                        lyric3Index = j + 2 < DataStore.Instance.Lyrics[i].Time.Count ? i :
                            (i + 1 < DataStore.Instance.Lyrics.Count && 1 < DataStore.Instance.Lyrics[i + 1].Time.Count ? i + 1 :
                            (i + 2 < DataStore.Instance.Lyrics.Count ? i + 2 : -1));
                    }
                    else
                        break;
                }
            }

            // update each line to found lyrics
            CurrentLine = DataStore.Instance.Lyrics.ElementAtOrDefault(lyric1Index)?.Text ?? string.Empty;
            NextLine1 = DataStore.Instance.Lyrics.ElementAtOrDefault(lyric2Index)?.Text ?? string.Empty;
            NextLine2 = DataStore.Instance.Lyrics.ElementAtOrDefault(lyric3Index)?.Text ?? string.Empty;
        }

        public void Start()
        {
            _lyricsTimer.Start();
        }
    }
}
