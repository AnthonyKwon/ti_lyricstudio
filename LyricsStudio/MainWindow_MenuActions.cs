#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

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
            lyrics = file.open();

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
            DataGridView.DragDrop += new DragEventHandler(this.DataGridView_DragDrop);
            DataGridView.DragOver += new DragEventHandler(this.DataGridView_DragOver);
            DataGridView.MouseDown += new MouseEventHandler(this.DataGridView_MouseDown);
            DataGridView.MouseMove += new MouseEventHandler(this.DataGridView_MouseMove);

            // bind ContextMenuStrip to DataGridView
            DataGridView.ContextMenuStrip = MenuDGVRightClick;

            // enable "Import...", "Save" and "Save As" entries
            MenuImport.Enabled = true;
            MenuSave.Enabled = true;
            MenuSaveAs.Enabled = true;
        }

        // Action on "Open..." click
        // open and load a existing audio and lyrics file
        private void MenuOpen_Click(object sender, System.EventArgs e)
        {
            {
                // initialize open file dialog
                System.Windows.Forms.OpenFileDialog dialog = OpenFileDialog;
                //dialog.InitialDirectory = FileInfo.Location;
                dialog.RestoreDirectory = true;

                // action after user chose audio file to load
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    // find LRC file with same name as audio file
                    string lrcFile = Path.ChangeExtension(dialog.FileName, ".lrc");
                    // open LRC file if found one
                    if (System.IO.File.Exists(lrcFile))
                    {
                        LoadLyrics(lrcFile);
                    }
                    
                }
            }
        }
        private void MenuImport_Click(object sender, EventArgs e)
        {

        }

        private void MenuSave_Click(object sender, System.EventArgs e)
        {
            MenuSave_Click(sender, e, false);
        }

        // Action on "Save" click
        // save lyrics file to disk
        private void MenuSave_Click(object sender, System.EventArgs e, bool Recalled)
        {
            // do nothing if workspace is not opened
            if (fileOpened == false) { return; }

            // do nothing if file is not edited

            // try to create lyrics file if not available yet
            if (System.IO.File.Exists(file.FilePath))
            {
                try
                {
                    // try to create a file
                    System.IO.File.Create(file.FilePath).Dispose();
                }
                catch
                {
                    //TODO: define an action when failed to create file
                }
            }

            // try to save file
        }

        // Action on "Save As..." click
        // save lyrics file as different name
        private void MenuSaveAs_Click(object sender, System.EventArgs e)
        {
            // do nothing if workspace is not opened
            if (fileOpened == false) { return; }

            // initialize save file dialog
            System.Windows.Forms.SaveFileDialog dialog = SaveFileDialog;
            dialog.FileName = FileInfo.FileName;
            dialog.InitialDirectory = FileInfo.Location;

            // action after user chose where to save
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
            }
        }
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member