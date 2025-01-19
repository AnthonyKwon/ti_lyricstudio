using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Selection;
using ti_Lyricstudio.Models;

namespace ti_Lyricstudio.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        // ViewModel for VLC player control
        public PlayerControlViewModel PlayerDataContext { get; } = new();

        // ViewModel for Lyrics Preview
        public LyricsPreviewViewModel PreviewDataContext { get; } = new();

        // Lyrics TreeDataGrid source to attach at EditorView
        private FlatTreeDataGridSource<LyricData>? _lyricsGridSource;
        public FlatTreeDataGridSource<LyricData> LyricsGridSource { get
            {
                if (_lyricsGridSource == null) throw new InvalidOperationException("Lyrics can't be accessed without loading a file.");
                return _lyricsGridSource;
            }
        }

        // UI interaction on "Open" button clicked
        // check if current workspace is modified and open file dialog
        public void OpenFile(string audioPath)
        {
            // generated lrc file path based on the audio file path
            string lrcPath = Path.ChangeExtension(audioPath, ".lrc");

            // check if lrc file exists
            if (File.Exists(lrcPath))
            {
                // load the file
                LyricsFile file = new(lrcPath);
                DataStore.Instance.Lyrics = new(file.Open());
            } else
            {
                // create empty data for new lyrics file
                LyricTime emptyTime = new(0, 0, 0);
                LyricData emptyData = new();
                emptyData.Time.Add(emptyTime);
                emptyData.Text = "Start typing your lyrics here!";

                // bind empty data to new list
                DataStore.Instance.Lyrics = [emptyData];
            }

            // initialize TreeDataGrid source
            _lyricsGridSource = new(DataStore.Instance.Lyrics);

            // get max size of the time column
            int timeColumnMaxSize = DataStore.Instance.Lyrics.Max(l => l.Time.Count);

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
                _lyricsGridSource.Columns.Add(timeCol);
                _lyricsGridSource.Selection = new TreeDataGridCellSelectionModel<LyricData>(_lyricsGridSource);
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
            _lyricsGridSource.Columns.Add(textCol);

            // append empty data at the end of the list
            DataStore.Instance.Lyrics.Add(new LyricData() { Time = [LyricTime.Empty] });

            // open the audio
            PlayerDataContext.Open(audioPath);

            // load the preview
            PreviewDataContext.Start();
        }
    }
}
