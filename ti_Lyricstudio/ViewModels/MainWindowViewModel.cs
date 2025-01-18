using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using ti_Lyricstudio.Class;

namespace ti_Lyricstudio.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        // ViewModel for VLC player control
        public PlayerControlViewModel PlayerDataContext { get; } = new();

        // Lyrics TreeDataGrid source to attach at EditorView
        private FlatTreeDataGridSource<LyricData> _lyrics;
        public FlatTreeDataGridSource<LyricData> Lyrics { get => _lyrics; }

        // UI interaction on "Open" button clicked
        // check if current workspace is modified and open file dialog
        public void OpenFile(string audioPath)
        {
            // generated lrc file path based on the audio file path
            string lrcPath = Path.ChangeExtension(audioPath, ".lrc");

            // check if lrc file exists
            if (File.Exists(lrcPath))
            {
                LyricsFile file = new(lrcPath);
                ObservableCollection<LyricData> lyrics = new(file.Open());

                // initialize TreeDataGrid source
                _lyrics = new(lyrics);

                // get max size of the time column
                int timeColumnMaxSize = lyrics.Max(l => l.Time.Count);

                for (int i = 0; i < timeColumnMaxSize; i++)
                {
                    // index of current column
                    // required to avoid issue with variable referencing with closures
                    int currentColumn = i;

                    // create each time column to the lyrics data source
                    // safeguard: set to empty string when index of current column is
                    //            higher then size of time list
                    TextColumn<LyricData, string> timeCol = new($"Time {i + 1}",
                        lyric => lyric.Time.Count > currentColumn ? lyric.Time[currentColumn].ToString() : string.Empty,
                        (lyric, value) =>
                        {
                            if (lyric.Time.Count > currentColumn && !string.IsNullOrEmpty(value))
                                lyric.Time[currentColumn] = LyricTime.From(value);
                        });
                    // configure the created column
                    timeCol.Options.TextAlignment = Avalonia.Media.TextAlignment.Center;
                    timeCol.Options.CanUserResizeColumn = false;
                    // insert the created column to source
                    _lyrics.Columns.Add(timeCol);
                }
                // add text column to the lyrics data source
                TextColumn<LyricData, string> textCol = new("Text",
                    lyric => lyric.Text,
                    (lyric, value) =>
                    {
                        if (!string.IsNullOrEmpty(value))
                            lyric.Text = value;
                    });
                // insert the created column to source
                _lyrics.Columns.Add(textCol);
            }

            // open the audio
            PlayerDataContext.Open(audioPath);
        }
    }
}
