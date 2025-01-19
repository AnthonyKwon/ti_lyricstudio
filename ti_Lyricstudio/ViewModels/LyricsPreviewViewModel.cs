using System;
using System.Diagnostics;
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
            _lyricsTimer.Interval = TimeSpan.FromMilliseconds(67);
            _lyricsTimer.Tick += LyricsTimer_Tick;
        }

        private void LyricsTimer_Tick(object? sender, EventArgs e)
        {
            // 
            if (DataStore.Instance.Lyrics == null || DataStore.Instance.Player == null) return;

            //
            if (DataStore.Instance.Player.IsPlaying == false) return;

            // fallback dummy data to use when search failed
            LyricData lyricData = new() { Text = string.Empty };
            LyricData fallback = lyricData;

            IOrderedEnumerable<LyricData> flattenedData = DataStore.Instance.Lyrics.SelectMany((lyric, index) => lyric.Time.Select(time => new LyricData() { Text = lyric.Text, Time = [time] }))
                .OrderBy(d => d.Time[0].totalMillisecond);

            //LyricData currentLine = DataStore.Instance.Lyrics.LastOrDefault(l => l.Time.FindLast(t => t.totalMillisecond <= DataStore.Instance.Player.Time) is LyricTime, fallback);
            LyricData currentLine = flattenedData.LastOrDefault(l => l.Time.FindLast(t => t.totalMillisecond <= DataStore.Instance.Player.Time) is LyricTime, fallback);
            //LyricData nextLine1 = ;
            //LyricData nextLine2 = ;

            CurrentLine = currentLine.Text;
            //NextLine1 = nextLine1.Text;
            //NextLine2 = nextLine2.Text;
        }

        public void Start()
        {
            _lyricsTimer.Start();
        }
    }
}
