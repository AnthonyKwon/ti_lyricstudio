#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using ti_Lyricstudio.Class;
using System.Linq;
using LibVLCSharp.Shared;

namespace ti_Lyricstudio
{
    public partial class MainWindow
    {
        // get version of the application
        private readonly static System.Reflection.Assembly myasm = System.Reflection.Assembly.GetEntryAssembly();
        private readonly static string AppName = myasm.GetName().Name.Replace('_', ':');
        private readonly static string VersionInfo = myasm.GetName().Version.ToString();
        // title of the application
        private readonly string windowTitle = $"{AppName} {VersionInfo}";

        // list of the lyrics to be used at the GridView
        private List<LyricData> lyrics;
        LyricsDataSource dataSource;

        // deprecated: only for legacy support
        private List<LyricsData> CData;
        private List<LyricsData> TData;

        // marker to check if file is opened
        private bool opened = false;
        // marker to check if file has modified
        private bool modified = false;

        // information of the lyrics workspace file
        private LyricsFile file;

        // variables used for LibVLCSharp
        LibVLC vlc = new();
        Media media;
        MediaPlayer player;
        int audioDuration = 0;

        public MainWindow()
        {
            InitializeComponent();
            // remove image margin from menu strip
            // ref: https://stackoverflow.com/a/32579262
            SetValuesOnSubItems(MenuStrip.Items.OfType<ToolStripMenuItem>().ToList());
        }

        private void SetValuesOnSubItems(List<ToolStripMenuItem> items)
        {
            items.ForEach(item =>
            {
                var dropdown = (ToolStripDropDownMenu)item.DropDown;
                if (dropdown != null)
                {
                    dropdown.ShowImageMargin = false;
                    SetValuesOnSubItems(item.DropDownItems.OfType<ToolStripMenuItem>().ToList());
                }
            });
        }

        // Form Function
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            // ask user to continue if file was opened and modified
            if (opened == true && modified == true)
            {
                DialogResult result = MessageBox.Show("File has been modified. Are you sure to continue without saving?", "File modified", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            // stop all running thread
            if (PlayerTimeThread != null && PlayerTimeThread.IsAlive == true)
            {
                // order thread to stop
                running = false;
            }

            // dispose the media player
            if (player != null) player.Dispose();
            if (media != null) media.Dispose();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            // Initialize Form Settings
            Text = windowTitle;
            MainWindow_Resize(sender, e); // Resize all components
        }

        private void MainWindow_Resize(object sender, EventArgs e)
        {
            // Resize DataGridView
            DataGridView.Height = Height - 155;
            DataGridView.Width = Width - 40;
            // Move pnlController
            pnlController.Top = Height - 112;
            // Resize pnlController
            pnlController.Width = Width - 40;
            // Resize trcTime
            TimeBar.Width = pnlController.Width - 305;
            // Resize lblPreview
            PreviewLabel.Width = pnlController.Width - 4;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (audioDuration * player.Position <= 0.9)
            {
                player.Position += 0.1f;
            }
        }

        private void btnPlayPause_Click(object sender, EventArgs e)
        {
            if (player.IsPlaying == true)
            {
                // update label to play symbol and pause
                btnPlayPause.Text = "4";
                // allow edit of DataGridView
                DataGridView.EditMode = DataGridViewEditMode.EditOnKeystroke;

                // pause media player
                player.Pause();

            }
            else
            {
                // update label to pause symbol and play
                btnPlayPause.Text = ";";
                // block edit of DataGridView
                DataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;

                // play/resume media player
                player.Play();
            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (audioDuration * player.Position >= 0.1)
            {
                player.Position -= 0.1f;
            }
        }

        private void btnSetTime_Click(object sender, EventArgs e)
        {
            //CData[DataGridView.CurrentRow.Index].Time = PlayTime;
            DataGridView.Refresh();
            DataGridView.CurrentCell = DataGridView[0, DataGridView.CurrentCellAddress.Y + 1];
            My.MyProject.Forms.DebugWindow.AddDLine("Current Lyrics Database size: " + CData.Count.ToString());
            //IsDirty = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            // set the UI lock
            delegateLock = this;

            // set label of btnPlayPause to play symbol
            btnPlayPause.Text = "4";
            // reset label of TimeLabel
            TimeLabel.Text = $"00:00.00 / {LyricTime.From(audioDuration.ToString()).ToString()}";
            // reset value of time seekbar
            TimeBar.Value = 0;

            // stop the player
            player.Stop();

            // unset the UI lock
            delegateLock = null;
        }

        private void it1AddMultipleLines_Click(object sender, EventArgs e)
        {
            if (!(CData == null))
            {
                My.MyProject.Forms.AddMultipleLineWindow.ShowDialog();
            }
        }

        private void it1ShowDebugWindow_Click(object sender, EventArgs e)
        {
            My.MyProject.Forms.DebugWindow.Show();
        }

        private void it2InsertLine_Click(object sender, EventArgs e)
        {
            if (!(CData == null))
            {
                //DataGridView_AddLine(Constants.vbNullString, Constants.vbNullString);
                for (int i = CData.Count - 2, loopTo = DataGridView.CurrentRow.Index; i >= loopTo; i -= 1)
                {
                    if (i > 0)
                    {
                        CData[i + 1].Time = CData[i].Time;
                        My.MyProject.Forms.DebugWindow.AddDLine("CData.Item(" + (i + 1) + ").Time = " + CData[i + 1].Time);
                        CData[i + 1].Lyric = CData[i].Lyric;
                        My.MyProject.Forms.DebugWindow.AddDLine("CData.Item(" + (i + 1) + ").Lyric = " + CData[i + 1].Lyric);
                    }
                }
                CData[DataGridView.CurrentRow.Index].Time = string.Empty;
                CData[DataGridView.CurrentRow.Index].Lyric = string.Empty;
            }
        }

        private void TimeBar_MouseDown(object sender, MouseEventArgs e)
        {
            // lock the timebar
            if (timebarLock == null) timebarLock = this;
        }

        private void TimeBar_MouseUp(object sender, MouseEventArgs e)
        {
            // set player position to user set position
            float newPosition = (float)TimeBar.Value / audioDuration;
            player.Position = newPosition;

            // unlock the timebar
            if (timebarLock == this) timebarLock = null;
        }
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member