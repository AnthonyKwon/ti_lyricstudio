#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using LibVLCSharp.Shared;
using System;
using System.IO;
using System.Windows.Forms;
using ti_Lyricstudio.Class;

namespace ti_Lyricstudio
{
    public partial class MainWindow
    {
        // Load lyrics file
        private void LoadLyrics(string filePath)
        {
            // (re-)initialise lyrics file object
            file = new(filePath);
            // open file and save lyrics to list
            lyrics = file.Open();

            // unbind the data source from DataGridView
            DataGridView.DataSource = null;
            // (re-)bind the data source from DataGridView
            dataSource = new(lyrics);
            DataGridView.DataSource = dataSource;
            // resize columns to fit screen
            DataGridView.AutoResizeColumns(System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells);
            DataGridView.Columns[DataGridView.Columns.Count - 1].AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            // left-align the text column
            DataGridViewCellStyle style = new(DataGridView.DefaultCellStyle);
            style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            DataGridView.Columns[DataGridView.Columns.Count - 1].DefaultCellStyle = style;

            // bind mouse event to DataGridView
            DataGridView.DragDrop += new DragEventHandler(DataGridView_DragDrop);
            DataGridView.DragOver += new DragEventHandler(DataGridView_DragOver);
            DataGridView.MouseDown += new MouseEventHandler(DataGridView_MouseDown);
            DataGridView.KeyDown += new KeyEventHandler(DataGridView_KeyDown);
            DataGridView.KeyUp += new KeyEventHandler(DataGridView_KeyUp);

            // bind ContextMenuStrip to DataGridView
            DataGridView.ContextMenuStrip = MenuDGVRightClick;

            // enable "Import...", "Save" and "Save As" entries
            MenuImport.Enabled = true;
            MenuSave.Enabled = true;
            MenuSaveAs.Enabled = true;

            // mark file as opened
            opened = true;
        }

        // Action on "Open..." click
        // open and load a existing audio and lyrics file
        private void MenuOpen_Click(object sender, EventArgs e)
        {
            // ask user to continue if file was opened and modified
            if (opened == true && modified == true)
            {
                DialogResult result = MessageBox.Show("File has been modified. Are you sure to continue without saving?", "File modified", MessageBoxButtons.YesNo);
                if (result == DialogResult.No) return;

                // dispose previous media session
                player.Dispose();
                media.Dispose();
                player.Play();
            }

            // initialize open file dialog
            OpenFileDialog dialog = OpenFileDialog;
            dialog.RestoreDirectory = true;

            // action after user chose audio file to load
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // open the audio file
                media = new(vlc, dialog.FileName);
                media.Parse();
                player = new(media);

                // get audio duration
                AudioDuration = (int)media.Duration / 10;

                // find LRC file with same name as audio file
                string lrcFile = Path.ChangeExtension(dialog.FileName, ".lrc");
                // open LRC file if found one
                if (File.Exists(lrcFile))
                {
                    LoadLyrics(lrcFile);
                } 
            }
        }

        // Action on "Import" click
        // import lyrics from other file
        private void MenuImport_Click(object sender, EventArgs e)
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
            dialog.Filter = SaveFileDialog.Filter;
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
        private void MenuSave_Click(object sender, System.EventArgs e)
        {
            // do nothing if workspace is not opened or edited
            if (opened == false) return;

            // try to save file
            file.Save(lyrics);
        }

        // Action on "Save As..." click
        // save lyrics file as different name
        private void MenuSaveAs_Click(object sender, System.EventArgs e)
        {
            // do nothing if workspace is not opened
            if (opened == false) { return; }

            // initialize save file dialog
            SaveFileDialog dialog = SaveFileDialog;
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