using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Selection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ti_Lyricstudio.Models;

namespace ti_Lyricstudio.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        // ViewModel for VLC player control
        public PlayerControlViewModel PlayerDataContext { get; }

        // ViewModel for Lyrics Preview
        public LyricsPreviewViewModel PreviewDataContext { get; }

        // currently working file
        private LyricsFile? file;

        // maximum size of the time column
        private int timeColumnMaxSize = 0;

        // audio player to control
        private readonly AudioPlayer _player = new AudioPlayer();

        // lyrics data used by application
        private readonly ObservableCollection<LyricData> _lyrics = [];

        // Lyrics TreeDataGrid source to attach at EditorView
        private FlatTreeDataGridSource<LyricData>? _lyricsGridSource;
        public FlatTreeDataGridSource<LyricData> LyricsGridSource { get
            {
                if (_lyricsGridSource == null) throw new InvalidOperationException("Lyrics can't be accessed without loading a file.");
                return _lyricsGridSource;
            }
        }

        // marker to check if file is opened
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddTimeColumnCommand))]
        private bool _opened = false;

        // marker to check if file has modified
        [ObservableProperty]
        private bool _modified = false;

        public MainWindowViewModel()
        {
            PlayerDataContext = new(_player);
            PreviewDataContext = new(_lyrics, _player);
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
                _lyrics.Clear();
                foreach (LyricData line in file.Open())
                {
                    _lyrics.Add(line);
                }
            } else
            {
                // create empty data for new lyrics file
                LyricTime emptyTime = new(0, 0, 0);
                LyricData emptyData = new();
                emptyData.Time.Add(emptyTime);
                emptyData.Text = "Start typing your lyrics here!";

                // bind empty data to new list
                _lyrics.Clear();
                _lyrics.Add(emptyData);
            }

            // initialize TreeDataGrid source
            _lyricsGridSource = new(_lyrics);

            // get max size of the time column
            timeColumnMaxSize = _lyrics.Max(l => l.Time.Count);

            for (int i = 0; i < timeColumnMaxSize; i++)
            {
                // create new time column
                TextColumn<LyricData, string> timeCol = CreateTimeColumn(i);

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

                    // set lyric text to cell value
                    lyric.Text = value;

                    // add new additional row if user already added data to the existing one
                    if (_lyrics.IndexOf(lyric) == _lyrics.Count - 1)
                        _lyrics.Add(new LyricData() { Time = [] });
                }, GridLength.Star);

            // insert the created column to source
            _lyricsGridSource.Columns.Add(textCol);

            // append empty data at the end of the list
            _lyrics.Add(new LyricData() { Time = [] });

            // open the audio
            PlayerDataContext.Open(audioPath);

            // load the preview
            PreviewDataContext.Start();

            // mark file as opened
            Opened = true;
        }

        // save currently working lyrics to a file
        public void SaveFile()
        {
            // ignore request if file is not opened
            if (file == null || _lyrics == null) return;

            // delete the additional row before save
            _lyrics.RemoveAt(_lyrics.Count - 1);

            // request file save
            file.Save(_lyrics);

            // re-generate additional row
            _lyrics.Add(new LyricData() { Time = [] });
        }

        // UI interaction on file close
        public void CloseFile()
        {
            // destroy the audio session
            PlayerDataContext.Close();
            // stop the lyrics preview
            PreviewDataContext.Stop();
            // mark file as not opened
            Opened = false;
        }

        /// <summary>
        /// Create new time column.
        /// </summary>
        /// <param name="column">Index of column to create</param>
        /// <returns>Created new <see cref="TextColumn{LyricData, String}"/> object</returns>
        private TextColumn<LyricData, string> CreateTimeColumn(int column)
        {
            // index of current column
            // required to avoid issue with variable referencing with closures
            int currentColumn = column;

            // create each time column to the lyrics data source
            // safeguard: set to empty string when index of current column is
            //            higher then size of time list
            //TODO: merge code here with AddTimeColumn() part
            TextColumn<LyricData, string> timeCol = new($"Time {column + 1}",
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

                    }
                    else
                    {
                        // cell with value non-existant; new LyricTime object required

                        // emptying these cell should be ignored
                        if (string.IsNullOrEmpty(value)) return;

                        // ignore when value is in incorrect format
                        if (LyricTime.Verify(value) == false) return;

                        // add empty LyricTime object for reserving
                        lyric.Time.Add(LyricTime.From(value));
                    }

                    if (_lyrics.IndexOf(lyric) == _lyrics.Count - 1)
                    {
                        // add new additional row if user already added data to the existing one
                        _lyrics.Add(new LyricData() { Time = [] });
                    }
                }, new GridLength(90));
            // configure the created column
            timeCol.Options.TextAlignment = Avalonia.Media.TextAlignment.Center;
            timeCol.Options.CanUserResizeColumn = false;

            return timeCol;
        }

        // empty the content of selected cell
        public void EmptyCell()
        {
            // ignore request when workspace not ready
            if (_lyrics == null) return;

            // ignore request when cell is not selected
            if (_lyricsGridSource?.CellSelection == null) return;

            // get target cell to delete
            int targetRow = _lyricsGridSource.CellSelection.SelectedIndex.RowIndex[0];
            int targetColumn = _lyricsGridSource.CellSelection.SelectedIndex.ColumnIndex;

            // ignore deletion when target cell is invalid or located in additional row
            if (targetRow > _lyrics.Count - 2) return;

            // check if targetColumn is Time column
            if (_lyrics[targetRow].Time.Count > targetColumn)
            {
                // targetColumn is Time column
                _lyrics[targetRow].Time[targetColumn] = LyricTime.Empty;
            }
            else
            {
                // targetColumn is Text column
                _lyrics[targetRow].Text = string.Empty;
            }
        }

        /// <summary>
        /// Add the new row below at the selection.
        /// </summary>
        [RelayCommand(CanExecute = nameof(Opened))]
        public void AddRow()
        {
            // ignore request when cell is not selected
            if (_lyricsGridSource == null ||
                _lyricsGridSource.CellSelection == null ||
                _lyricsGridSource.CellSelection.SelectedIndex.RowIndex.Count == 0) return;

            // get row of the currently selected cell
            int index = _lyricsGridSource.CellSelection.SelectedIndex.RowIndex[0];

            // ignore request when target row is additional row
            if (index >= _lyrics.Count - 1) return;

            // insert new empty row below the selection
            _lyrics.Insert(index + 1, new LyricData() { Time = [], Text = string.Empty });
        }

        /// <summary>
        /// Insert single or multiple row(s) from clipboard, starting below at the selection.<br/>
        /// Data type can be simple text or LRC-formatted data.
        /// </summary>
        [RelayCommand(CanExecute = nameof(Opened))]
        public void InsertRow()
        {
            // ignore request when cell is not selected
            if (_lyricsGridSource == null ||
                _lyricsGridSource.CellSelection == null ||
                _lyricsGridSource.CellSelection.SelectedIndex.RowIndex.Count == 0) return;

            // get row of the currently selected cell
            int index = _lyricsGridSource.CellSelection.SelectedIndex.RowIndex[0];

            // ignore request when target row is additional row
            if (index >= _lyrics.Count - 1) return;
        }

        /// <summary>
        /// Move the selected row up.
        /// </summary>
        [RelayCommand (CanExecute = nameof(Opened))]
        public void MoveRowUp()
        {
            // ignore request when cell is not selected
            if (_lyricsGridSource == null ||
                _lyricsGridSource.CellSelection == null ||
                _lyricsGridSource.CellSelection.SelectedIndex.RowIndex.Count == 0) return;

            // get row of the currently selected cell
            int index = _lyricsGridSource.CellSelection.SelectedIndex.RowIndex[0];

            // ignore request if selected row is at the first index
            if (index <= 0) return;

            // ignore request when target row is additional row
            if (index >= _lyrics.Count - 1) return;

            _lyrics.Move(index, index - 1);
        }

        /// <summary>
        /// Move the selected row down.
        /// </summary>
        [RelayCommand(CanExecute = nameof(Opened))]
        public void MoveRowDown()
        {
            // ignore request when cell is not selected
            if (_lyricsGridSource == null ||
                _lyricsGridSource.CellSelection == null ||
                _lyricsGridSource.CellSelection.SelectedIndex.RowIndex.Count == 0) return;

            // get row of the currently selected cell
            int index = _lyricsGridSource.CellSelection.SelectedIndex.RowIndex[0];

            // ignore request if selected row is at the last index or additional row
            if (index >= _lyrics.Count - 2) return;

            _lyrics.Move(index, index + 1);
        }

        /// <summary>
        /// Delete the selected row from the table.
        /// </summary>
        public void DeleteRow()
        {
            // ignore request when workspace not ready
            if (_lyrics == null) return;

            // ignore request when cell is not selected
            if (_lyricsGridSource?.CellSelection == null) return;

            // get target row to delete
            int targetRow = _lyricsGridSource.CellSelection.SelectedIndex.RowIndex[0];

            // ignore deletion when target row is additional row
            if (targetRow >= _lyrics.Count - 1) return;

            // delete the target row
            _lyrics.RemoveAt(targetRow);
        }

        /// <summary>
        /// Add the new timestamp column at the table.
        /// </summary>
        [RelayCommand(CanExecute = nameof(Opened))]
        private void AddTimeColumn()
        {
            // ignore request when editor is not initialized
            if (_lyricsGridSource == null) return;

            // create new time column based on the current column max size
            TextColumn<LyricData, string> newColumn = CreateTimeColumn(timeColumnMaxSize);

            // insert the created column to source
            _lyricsGridSource.Columns.Insert(timeColumnMaxSize, newColumn);

            // increase time column max size
            timeColumnMaxSize += 1;

            // reset position of the text column
            IColumn<LyricData> textColumn = _lyricsGridSource.Columns[timeColumnMaxSize];
            _lyricsGridSource.Columns.RemoveAt(timeColumnMaxSize);
            _lyricsGridSource.Columns.Add(textColumn);

        }

        /// <summary>
        /// Remove the last timestamp column from the table.
        /// </summary>
        [RelayCommand(CanExecute = nameof(Opened))]
        private void RemoveTimeColumn()
        {
            // ignore request when editor is not initialized
            if (_lyricsGridSource == null) return;

            // ignore request when size of the time column is 1 or less
            if (timeColumnMaxSize <= 1) return;

            // remove the last time column from grid source
            _lyricsGridSource.Columns.RemoveAt(timeColumnMaxSize - 1);

            // filter the time lyric data which contains target time column index
            IEnumerable<ObservableCollection<LyricTime>> filteredTimeList = _lyrics.Where(lyric => lyric.Time.Count >= timeColumnMaxSize).Select(lyric => lyric.Time);
            foreach (ObservableCollection<LyricTime> timeList in filteredTimeList)
            {
                if (timeList.Count >= timeColumnMaxSize)
                {
                    // remove all column until column matches target size
                    for (int i = timeColumnMaxSize - 1; i < timeList.Count; i++)
                    {
                        timeList.RemoveAt(i);
                    }
                }
            }

            // decrease time column max size
            timeColumnMaxSize -= 1;

            // reset position of the text column
            IColumn<LyricData> textColumn = _lyricsGridSource.Columns[timeColumnMaxSize];
            _lyricsGridSource.Columns.RemoveAt(timeColumnMaxSize);
            _lyricsGridSource.Columns.Add(textColumn);
        }

        /// <summary>
        /// Move cell timestamp selection
        /// </summary>
        /// <param name="direction">Direction to move.<br/>0: Up, 1: Down</param>
        public void MoveTimeSelection(int direction)
        {
            // ignore request when workspace not ready
            if (_lyrics == null ||
                _player == null) return;

            // ignore request when datagrid is not ready to set time (cell not selected, etc)
            if (_lyricsGridSource == null ||
                _lyricsGridSource.CellSelection == null ||
                _lyricsGridSource.CellSelection.SelectedIndex.RowIndex.Count == 0) return;

            // get current selected cell
            int currentRow = _lyricsGridSource.CellSelection.SelectedIndex.RowIndex[0];
            int currentColumn = _lyricsGridSource.CellSelection.SelectedIndex.ColumnIndex;

            // ignore request when current cell is invalid or located in additional row
            if (currentRow > _lyrics.Count - 2) return;

            CellIndex newCellIndex;
            switch (direction)
            {
                case 0:
                    // move timpstamp selection up
                    // ignore request when current cell is already in first row
                    if (currentRow <= 0) return;

                    if (currentColumn <= 0)
                        newCellIndex = new(_lyrics[currentRow - 1].Time.Count - 1, currentRow - 1);
                    else
                        newCellIndex = new(currentColumn - 1, currentRow);
                    break;
                case 1:
                    // move timpstamp selection down
                    // ignore request when target cell is invalid or located in additional row
                    if (currentRow > _lyrics.Count - 2) return;

                    // get index of the next timestamp cell
                    if (currentColumn < _lyrics[currentRow].Time.Count - 1)
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

        /// <summary>
        /// Set time of selected cell to current playback duration.
        /// </summary>
        public void SetTime()
        {
            // ignore request when workspace not ready
            if (_lyrics == null || 
                _player == null) return;

            // ignore request when datagrid is not ready to set time (cell not selected, etc)
            if (_lyricsGridSource == null ||
                _lyricsGridSource.CellSelection == null ||
                _lyricsGridSource.CellSelection.SelectedIndex.RowIndex.Count == 0) return;

            // get target cell to set
            int targetRow = _lyricsGridSource.CellSelection.SelectedIndex.RowIndex[0];
            int targetColumn = _lyricsGridSource.CellSelection.SelectedIndex.ColumnIndex;

            // ignore request when target cell is invalid or located in additional row
            if (targetRow > _lyrics.Count - 2) return;

            // ignore request when target cell is not a time column
            if (targetColumn > timeColumnMaxSize - 1) return;

            // check if user is trying to add or edit
            if (targetColumn > _lyrics[targetRow].Time.Count - 1)
            {
                // user trying to add; append the new timestamp to list
                _lyrics[targetRow].Time.Add(LyricTime.From(_player.Time));
            }
            else
            {
                // user trying to edit; set time of selected cell to current time
                _lyrics[targetRow].Time[targetColumn] = LyricTime.From(_player.Time);
            }

            // freeze selection position when it's the last cell (excluding additional row)


            CellIndex newCellIndex;
            // get index of the next timestamp cell
            if (targetColumn < _lyrics[targetRow].Time.Count - 1)
                newCellIndex = new(targetColumn + 1, targetRow);
            else
                newCellIndex = new(0, targetRow + 1);

            // move selection to next timestamp cell
            _lyricsGridSource.CellSelection.SetSelectedRange(newCellIndex, 1, 1);
        }
    }
}
