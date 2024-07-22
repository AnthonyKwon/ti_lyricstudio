#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace com.stu_tonyk_dio.ti_LyricsStudio
{
    public partial class MainWindow
    {
        // Action on "New..." click
        // (re-)initialize the application workspace
        private void it1New_Click(object sender, System.EventArgs e)
        {
        }
        
        // Action on "Open..." click
        // open and load a existing audio and lyrics file
        private void it1Open_Click(object sender, System.EventArgs e)
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
                    // open LRC file
                    // convert audio file to WAVE format using FFmpeg
                    // enable "Save" and "Save As" entries
                }
            }
        }

        private void it1Save_Click(object sender, System.EventArgs e)
        {
            it1Save_Click(sender, e, false);
        }

        // Action on "Save" click
        // save lyrics file to disk
        private void it1Save_Click(object sender, System.EventArgs e, bool Recalled)
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
        private void it1SaveAs_Click(object sender, System.EventArgs e)
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