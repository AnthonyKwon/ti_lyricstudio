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

        private void it1Save_Click(object sender, System.EventArgs e, bool Recalled)
        {
            try
            {
                if (!(CData == null))
                {
                    if (System.IO.File.Exists(FileInfo.Location + @"\" + FileInfo.FileName + ".lrc"))
                    {
                        //FileInfoManage("Save"); // Call FileInfoManage() as Save
                    }
                    else if (Recalled == true & !System.IO.File.Exists(FileInfo.Location + @"\" + FileInfo.FileName + ".lrc"))
                    {
                        throw new System.IO.IOException("Failed to create file " + FileInfo.Location + @"\" + FileInfo.FileName + ".lrc");
                    }
                    else
                    {
                        System.IO.File.Create(FileInfo.Location + @"\" + FileInfo.FileName + ".lrc").Dispose();
                        it1Save_Click(sender, e, true);
                    }
                }
                else if (!(CData == null))
                {
                    if (!(FileInfo.Location + @"\" + FileInfo.FileName + FileInfo.FileName == null))
                    {
                        it1Save_Click(sender, e, true);
                    }
                    else if (Recalled == false)
                    {
                        it1SaveAs_Click(sender, e);
                    }
                    else
                    {
                        throw new System.ArgumentException("File save requested without proper filename.");
                    }
                }
            }
            catch (System.Exception ex)
            {
                My.MyProject.Forms.DebugWindow.AddDLine("Exception Thrown while trying to save lyrics file: " + ex.ToString(), 2);
            }
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