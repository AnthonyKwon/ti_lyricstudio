#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using ti_Lyricstudio.Class;

namespace ti_Lyricstudio
{
    public partial class MainWindow
    {
        private void AbortFileOpen(string filename)
        {
            AbortFileOpen(filename, AppName);
        }
        private void AbortFileOpen(string filename, string msgBoxTitle)
        {
            // unbind the data source from EditorView
            EditorView.DataSource = null;

            // unbind mouse event to EditorView
            EditorView.DragDrop -= new DragEventHandler(EditorView_DragDrop);
            EditorView.DragOver -= new DragEventHandler(EditorView_DragOver);
            EditorView.MouseDown -= new MouseEventHandler(EditorView_MouseDown);
            EditorView.KeyDown -= new KeyEventHandler(EditorView_KeyDown);
            EditorView.KeyUp -= new KeyEventHandler(EditorView_KeyUp);

            // unbind ContextMenuStrip to EditorView
            EditorView.ContextMenuStrip = null;

            // reset window title
            Text = windowTitle;

            // disable all control button
            btnPrev.Enabled = false;
            btnPlayPause.Enabled = false;
            btnStop.Enabled = false;
            btnNext.Enabled = false;

            // disable set time button
            btnSetTime.Enabled = false;

            // unbind events from time seekbar
            TimeBar.MouseDown -= new MouseEventHandler(TimeBar_MouseDown);
            TimeBar.MouseUp -= new MouseEventHandler(TimeBar_MouseUp);

            // set maximum value of the seekbar to the audio duration
            TimeBar.Maximum = 0;
            // set initial value of label of TimeLabel
            TimeLabel.Text = $"00:00.00 / 00:00.00";

            // disable "Import...", "Save" and "Save As" entries
            mItemImport.Enabled = false;
            mItemSave.Enabled = false;
            mItemSaveAs.Enabled = false;

            // mark file as not opened
            opened = false;

            // show error message to user
            MessageBox.Show($"Unable to open file '{filename}'!", msgBoxTitle);
        }

        private void SetupWorkspace()
        {
            // resize columns to fit screen
            EditorView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
            EditorView.Columns[EditorView.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            // left-align the text column
            DataGridViewCellStyle style = new(EditorView.DefaultCellStyle);
            style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            EditorView.Columns[EditorView.Columns.Count - 1].DefaultCellStyle = style;

            // bind mouse event to EditorView
            EditorView.DragDrop += new DragEventHandler(EditorView_DragDrop);
            EditorView.DragOver += new DragEventHandler(EditorView_DragOver);
            EditorView.MouseDown += new MouseEventHandler(EditorView_MouseDown);
            EditorView.KeyDown += new KeyEventHandler(EditorView_KeyDown);
            EditorView.KeyUp += new KeyEventHandler(EditorView_KeyUp);

            // bind ContextMenuStrip to EditorView
            EditorView.ContextMenuStrip = EditorMenu;

            // set form title as workspace name
            Text = $"{windowTitle} :: {Path.GetFileName(file.FilePath)}";

            // enable set time button
            btnSetTime.Enabled = true;

            // enable "Import...", "Save" and "Save As" entries
            mItemImport.Enabled = true;
            mItemSave.Enabled = true;
            mItemSaveAs.Enabled = true;

            // mark file as opened
            opened = true;
        }

        // create new lyrics file
        private void NewLyrics(string filePath)
        {
            // (re-)initialise lyrics file object
            file = new(filePath);

            // create empty data for new lyrics file
            LyricTime emptyTime = new(0, 0, 0);
            LyricData emptyData = new();
            emptyData.Time.Add(emptyTime);
            emptyData.Text = "Start typing your lyrics here!";
            List<LyricData> emptyList = [emptyData];
            lyrics = emptyList;

            // unbind the data source from EditorView
            EditorView.DataSource = null;
            // (re-)bind the data source from EditorView
            dataSource = new(lyrics);
            EditorView.DataSource = dataSource;
            // setup and enable the workspace
            SetupWorkspace();

            // remove unneeded first empty row
            EditorView.Rows.RemoveAt(0);
        }

        // load lyrics file
        private void LoadLyrics(string filePath)
        {
            // (re-)initialise lyrics file object
            file = new(filePath);
            // open file and save lyrics to list
            lyrics = file.Open();

            // unbind the data source from EditorView
            EditorView.DataSource = null;
            // (re-)bind the data source from EditorView
            dataSource = new(lyrics);
            EditorView.DataSource = dataSource;
            // setup and enable the workspace
            SetupWorkspace();
        }

        // Action on "Open..." click
        // open and load a existing audio and lyrics file
        private void mItemOpen_Click(object sender, EventArgs e)
        {
            // ask user to continue if file was opened and modified
            if (opened == true || modified == true)
            {
                if (modified == true)
                {
                    DialogResult result = MessageBox.Show("File has been modified. Are you sure to continue without saving?", "File modified", MessageBoxButtons.YesNo);
                    if (result == DialogResult.No) return;
                }

                // stop all running threads first
                if (PlayerTimeThread.IsAlive == true)
                {
                    running = false;
                }

                // stop the player
                btnStop_Click(sender, e);

                // dispose previous media session
                player.Dispose();
                media.Dispose();
            }

            // initialize open file dialog
            OpenFileDialog dialog = OpenDialog;
            dialog.RestoreDirectory = true;

            // action after user chose audio file to load
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // open and parse the audio file
                media = new(vlc, dialog.FileName);
                media.Parse();
                // wait for file parse to be done
                for (int i = 0; i <= 100; i++)
                {
                    if (media.ParsedStatus == LibVLCSharp.Shared.MediaParsedStatus.Done) break;
                    Thread.Sleep(100);
                }

                player = new(media);

                // enable all control button
                btnPrev.Enabled = true;
                btnPlayPause.Enabled = true;
                btnStop.Enabled = true;
                btnNext.Enabled = true;

                // bind events to TimeBar
                TimeBar.MouseDown += new MouseEventHandler(TimeBar_MouseDown);
                TimeBar.MouseUp += new MouseEventHandler(TimeBar_MouseUp);

                // get audio duration
                audioDuration = (int)media.Duration;
                media.ParseStop();

                // abort file open on error
                if (audioDuration <= 0)
                {
                    AbortFileOpen(dialog.FileName, dialog.Title);
                    return;
                }

                // set maximum value of the seekbar to the audio duration
                TimeBar.Maximum = audioDuration;
                // set initial value of label of TimeLabel
                TimeLabel.Text = $"00:00.00 / {LyricTime.From(audioDuration)}";

                // start player time tracking thread
                if (PlayerTimeThread == null || PlayerTimeThread.IsAlive == false)
                {
                    // create new thread for playing time parsing from MediaPlayer
                    PlayerTimeThread = new Thread(new ThreadStart(PlayerTimeThreadF));
                    // mark thread as running
                    running = true;
                    // start the thread
                    PlayerTimeThread.Start();
                }

                // find LRC file with same name as audio file
                string lrcFile = Path.ChangeExtension(dialog.FileName, ".lrc");
                // open LRC file if found one
                if (File.Exists(lrcFile))
                {
                    // load existing lyrics file
                    LoadLyrics(lrcFile);
                } else
                {
                    // create new lyrics file workspace
                    NewLyrics(lrcFile);
                }
            }
        }

        // Action on "Import" click
        // import lyrics from other file
        private void mItemImport_Click(object sender, EventArgs e)
        {
            // ask user to continue if file was opened and modified
            if (opened == true && modified == true)
            {
                DialogResult result = MessageBox.Show("File has been modified. Are you sure to continue without saving?", "File modified", MessageBoxButtons.YesNo);
                if (result == DialogResult.No) return;
            }

            // initialize open file dialog
            OpenFileDialog dialog = new();
            dialog.Title = "Import lyrics file...";
            dialog.Filter = SaveDialog.Filter;
            dialog.RestoreDirectory = true;

            // action after user chose audio file to load
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // open LRC file if found one
                if (File.Exists(dialog.FileName))
                {
                    LoadLyrics(dialog.FileName);
                }
            }
        }

        // Action on "Save" click
        // save lyrics file to disk
        private void mItemSave_Click(object sender, System.EventArgs e)
        {
            // do nothing if workspace is not opened or edited
            if (opened == false) return;

            // try to save file
            file.Save(lyrics);

            // remove modified mark from the title
            if (modified == true)
            {
                Text = $"{windowTitle} :: {Path.GetFileName(file.FilePath)}";
                modified = false;
            }
        }

        // Action on "Save As..." click
        // save lyrics file as different name
        private void mItemSaveAs_Click(object sender, System.EventArgs e)
        {
            // do nothing if workspace is not opened
            if (opened == false) { return; }

            // initialize save file dialog
            SaveFileDialog dialog = SaveDialog;
            dialog.FileName = Path.GetFileName(file.FilePath);
            dialog.InitialDirectory = file.FilePath;

            // action after user chose where to save
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                file.Save(lyrics, dialog.FileName);
            }
        }
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member