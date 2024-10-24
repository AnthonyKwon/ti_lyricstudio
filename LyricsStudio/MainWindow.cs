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
        // title of the application
        private readonly string windowTitle = $"{AppName}";

        // list of the lyrics to be used at the GridView
        private List<LyricData> lyrics;
        LyricsDataSource dataSource;

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

        // create new stopwatch to count player duration
        private readonly OffsetStopwatch sw = new();

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            // Start the application.
            Application.Run(new MainWindow());
        }

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

            // purge workspace and exit
            PurgeWorkspace();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            // Initialize Form Settings
            Text = windowTitle;
            MainWindow_Resize(sender, e); // Resize all components
        }

        private void MainWindow_Resize(object sender, EventArgs e)
        {
            // Resize EditorView
            EditorView.Height = Height - 155;
            EditorView.Width = Width - 40;
            // Move Player Controller
            PlayerGroup.Top = Height - 112;
            // Resize Player Controller
            PlayerGroup.Width = Width - 40;
            // Resize Timebar
            TimeBar.Width = PlayerGroup.Width - 305;
            // Resize Lyrics Preview Label
            PreviewLabel.Width = PlayerGroup.Width - 4;
        }

        // rewind player for 10 seconds (if possible)
        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (player != null && player.Time != -1)
            {
                // rewind player audio duration
                if (player.Time <= 10000) player.Position = 0;
                else player.Time -= 10000;

                // ask thread to synchronise stopwatch
                threadJob.Add("syncStopwatch");
            }
        }

        // fast-forward player for 10 seconds (if possible)
        private void btnNext_Click(object sender, EventArgs e)
        {
            if (player != null && player.Time != -1)
            {
                // fast-forward player position
                if (player.Length - player.Time <= 10000) player.Position = 1;
                else player.Time += 10000;

                // ask thread to synchronise stopwatch
                threadJob.Add("syncStopwatch");
            }
        }

        private void btnPlayPause_Click(object sender, EventArgs e)
        {
            // unlock player thread UI editing
            threadUILock = null;

            // start playing audio if player is not player.
            // if not, toggle playing state.
            if (player.State == VLCState.NothingSpecial || 
                player.State == VLCState.Stopped) player.Play();
            else player.Pause();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            // lock player thread UI editing
            threadUILock = this;

            // stop the player
            if (player.State != VLCState.Stopped) player.Stop();
        }

        private void btnSetTime_Click(object sender, EventArgs e)
        {
            // get first cell user have selected
            DataGridViewCell selectedCell = EditorView.SelectedCells[0];

            // do nothing if selected cell is OOB
            if (lyrics.Count == 0) return;
            if (selectedCell.RowIndex >= lyrics.Count) return;

            // do nothing if selected cell is not an time cell
            if (selectedCell.ColumnIndex == EditorView.ColumnCount - 1) return;

            // create new timestamp from current player position
            int newTime = (int)(player.Position * audioDuration);
            if (newTime < 0) newTime = 0;
            // create new time object from new timestamp
            LyricTime newTimeObject = LyricTime.From(newTime);
            // bind new time to current selection
            selectedCell.Value = newTimeObject;

            // select time right below current select
            EditorView.ClearSelection();
            EditorView.Rows[selectedCell.RowIndex + 1].Cells[selectedCell.ColumnIndex].Selected = true;
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

            // ask thread to change the offset of stopwatch
            // and unlock the timebar
            threadJob.Add("syncStopwatch");
        }
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member