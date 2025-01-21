using System;
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

        // currently working file
        private LyricsFile? file;

        // maximum size of the time column
        private int timeColumnMaxSize = 0;

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

            // initialize the file object
            file = new(lrcPath);

            // check if lrc file exists
            if (File.Exists(lrcPath))
            {
                // load the file
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
            timeColumnMaxSize = DataStore.Instance.Lyrics.Max(l => l.Time.Count);

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
                        // nothing changed; do nothing 
                        if (value == null) return;

                        // check if user selected cell with existing value or not
                        if (lyric.Time.Count > currentColumn)
                        {
                            // cell with existing value; new LyricTime object not required

                            // value has emptied; remove the timestamp from list
                            if (string.IsNullOrEmpty(value)) lyric.Time.RemoveAt(currentColumn);

                            // ignore when value is in incorrect format
                            if (LyricTime.Verify(value) == false) return;

                            // set lyric time to updated value
                            lyric.Time[currentColumn] = LyricTime.From(value);

                        } else
                        {
                            // cell with value non-existant; new LyricTime object required

                            // emptying these cell should be ignored
                            if (string.IsNullOrEmpty(value)) return;

                            // ignore when value is in incorrect format
                            if (LyricTime.Verify(value) == false) return;

                            // add empty LyricTime object for reserving
                            lyric.Time.Add(LyricTime.From(value));
                        }

                        if (DataStore.Instance.Lyrics.IndexOf(lyric) == DataStore.Instance.Lyrics.Count - 1)
                        {
                            // add new additional row if user already added data to the existing one
                            DataStore.Instance.Lyrics.Add(new LyricData() { Time = [] });
                        }
                    }, new GridLength(90));
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
                    // nothing changed; do nothing 
                    if (value == null) return;

                    // looks like I tried to handle empty line separately, but LyricData/LyricTime handles it correctly by itself
                    //if (string.IsNullOrWhiteSpace(value) && lyric.Time.Count == 0) return;

                    // set lyric text to cell value
                    lyric.Text = value;

                    // add new additional row if user already added data to the existing one
                    if (DataStore.Instance.Lyrics.IndexOf(lyric) == DataStore.Instance.Lyrics.Count - 1)
                        DataStore.Instance.Lyrics.Add(new LyricData() { Time = [] });
                }, GridLength.Star);
            // insert the created column to source
            _lyricsGridSource.Columns.Add(textCol);

            // append empty data at the end of the list
            DataStore.Instance.Lyrics.Add(new LyricData() { Time = [] });

            // open the audio
            PlayerDataContext.Open(audioPath);

            // load the preview
            PreviewDataContext.Start();
        }

        // save currently working lyrics to a file
        public void SaveFile()
        {
            // ignore request if lyrics are not loaded
            if (DataStore.Instance.Lyrics == null) return;

            // delete the additional row before save
            DataStore.Instance.Lyrics.RemoveAt(DataStore.Instance.Lyrics.Count - 1);

            // request file save
            file.Save(DataStore.Instance.Lyrics);

            // re-generate additional row
            DataStore.Instance.Lyrics.Add(new LyricData() { Time = [] });
        }

        // UI interaction on file close
        public void CloseFile()
        {
            // destroy the audio session
            PlayerDataContext.Close();
            // stop the lyrics preview
            PreviewDataContext.Stop();
        }

        // delete the selected row
        public void DeleteRow()
        {
            // ignore request when workspace not ready
            if (DataStore.Instance.Lyrics == null) return;

            // ignore request when cell is not selected
            if (_lyricsGridSource?.CellSelection == null) return;

            // get target row to delete
            int targetRow = _lyricsGridSource.CellSelection.SelectedIndex.RowIndex[0];

            // ignore deletion when target row is additional row
            if (targetRow >= DataStore.Instance.Lyrics.Count - 1) return;

            // delete the target row
            DataStore.Instance.Lyrics.RemoveAt(targetRow);
        }

        // empty the content of selected cell
        public void EmptyCell()
        {
            // ignore request when workspace not ready
            if (DataStore.Instance.Lyrics == null) return;

            // ignore request when cell is not selected
            if (_lyricsGridSource?.CellSelection == null) return;

            // get target cell to delete
            int targetRow = _lyricsGridSource.CellSelection.SelectedIndex.RowIndex[0];
            int targetColumn = _lyricsGridSource.CellSelection.SelectedIndex.ColumnIndex;

            // ignore deletion when target cell is invalid or located in additional row
            if (targetRow > DataStore.Instance.Lyrics.Count - 2) return;

            // check if targetColumn is Time column
            if (DataStore.Instance.Lyrics[targetRow].Time.Count > targetColumn)
            {
                // targetColumn is Time column
                DataStore.Instance.Lyrics[targetRow].Time[targetColumn] = LyricTime.Empty;
            }
            else
            {
                // targetColumn is Text column
                DataStore.Instance.Lyrics[targetRow].Text = string.Empty;
            }
        }
        /// <summary>
        /// Move cell timestamp selection
        /// </summary>
        /// <param name="direction">Direction to move.<br/>0: Up, 1: Down</param>
        public void MoveTimeSelection(int direction)
        {
            // ignore request when workspace not ready
            if (DataStore.Instance.Lyrics == null ||
                DataStore.Instance.Player == null) return;

            // ignore request when datagrid is not ready to set time (cell not selected, etc)
            if (_lyricsGridSource == null ||
                _lyricsGridSource.CellSelection == null ||
                _lyricsGridSource.CellSelection.SelectedIndex.RowIndex.Count == 0) return;

            // get current selected cell
            int currentRow = _lyricsGridSource.CellSelection.SelectedIndex.RowIndex[0];
            int currentColumn = _lyricsGridSource.CellSelection.SelectedIndex.ColumnIndex;

            // ignore request when current cell is invalid or located in additional row
            if (currentRow > DataStore.Instance.Lyrics.Count - 2) return;

            CellIndex newCellIndex;
            switch (direction)
            {
                case 0:
                    // move timpstamp selection up
                    // ignore request when current cell is already in first row
                    if (currentRow <= 0) return;

                    if (currentColumn <= 0)
                        newCellIndex = new(DataStore.Instance.Lyrics[currentRow - 1].Time.Count - 1, currentRow - 1);
                    else
                        newCellIndex = new(currentColumn - 1, currentRow);
                    break;
                case 1:
                    // move timpstamp selection down
                    // ignore request when target cell is invalid or located in additional row
                    if (currentRow > DataStore.Instance.Lyrics.Count - 2) return;

                    // get index of the next timestamp cell
                    if (currentColumn < DataStore.Instance.Lyrics[currentRow].Time.Count - 1)
                        newCellIndex = new(currentColumn + 1, currentRow);
                    else
                        newCellIndex = new(0, currentRow + 1);
                    break;
                default:
                    // fallback: reset to first cell
                    newCellIndex = new(0, 0);
                    break;

            }

            // move selection to next timestamp cell
            _lyricsGridSource.CellSelection.SetSelectedRange(newCellIndex, 1, 1);
        }

        // set the time of selected cell
        public void SetTime()
        {
            // ignore request when workspace not ready
            if (DataStore.Instance.Lyrics == null || 
                DataStore.Instance.Player == null) return;

            // ignore request when datagrid is not ready to set time (cell not selected, etc)
            if (_lyricsGridSource == null ||
                _lyricsGridSource.CellSelection == null ||
                _lyricsGridSource.CellSelection.SelectedIndex.RowIndex.Count == 0) return;

            // get target cell to set
            int targetRow = _lyricsGridSource.CellSelection.SelectedIndex.RowIndex[0];
            int targetColumn = _lyricsGridSource.CellSelection.SelectedIndex.ColumnIndex;

            // ignore request when target cell is invalid or located in additional row
            if (targetRow > DataStore.Instance.Lyrics.Count - 2) return;

            // ignore request when target cell is not a time column
            if (targetColumn > timeColumnMaxSize - 1) return;

            // check if user is trying to add or edit
            if (targetColumn > DataStore.Instance.Lyrics[targetRow].Time.Count - 1)
            {
                // user trying to add; append the new timestamp to list
                DataStore.Instance.Lyrics[targetRow].Time.Add(LyricTime.From(DataStore.Instance.Player.Time));
            }
            else
            {
                // user trying to edit; set time of selected cell to current time
                DataStore.Instance.Lyrics[targetRow].Time[targetColumn] = LyricTime.From(DataStore.Instance.Player.Time);
            }

            // freeze selection position when it's the last cell (excluding additional row)


            CellIndex newCellIndex;
            // get index of the next timestamp cell
            if (targetColumn < DataStore.Instance.Lyrics[targetRow].Time.Count - 1)
                newCellIndex = new(targetColumn + 1, targetRow);
            else
                newCellIndex = new(0, targetRow + 1);

            // move selection to next timestamp cell
            _lyricsGridSource.CellSelection.SetSelectedRange(newCellIndex, 1, 1);
        }
    }
}
