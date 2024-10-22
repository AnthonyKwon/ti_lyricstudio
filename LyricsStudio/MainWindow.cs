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
        OffsetStopwatch sw = new();

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
                // ask thread to pause the stopwatch
                threadJob.Add("pauseStopwatch");

                // update label to play symbol and pause
                btnPlayPause.Text = "4";
                // allow edit of EditorView
                EditorView.EditMode = DataGridViewEditMode.EditOnKeystroke;

                // pause media player
                player.Pause();

            }
            else
            {
                // ask thread to start the stopwatch
                threadJob.Add("startStopwatch");

                // update label to pause symbol and play
                btnPlayPause.Text = ";";
                // block edit of EditorView
                EditorView.EditMode = DataGridViewEditMode.EditProgrammatically;

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

        private void btnStop_Click(object sender, EventArgs e)
        {
            // request thread to stop the stopwatch
            threadJob.Add("stopStopwatch");

            // set the UI lock
            delegateLock = this;

            // allow edit of EditorView
            EditorView.EditMode = DataGridViewEditMode.EditOnKeystroke;

            // set label of btnPlayPause to play symbol
            btnPlayPause.Text = "4";
            // reset label of TimeLabel
            TimeLabel.Text = $"00:00.00 / {LyricTime.From((audioDuration / 10).ToString())}";
            // reset value of time seekbar
            TimeBar.Value = 0;

            // stop the player
            player.Stop();

            // unset the UI lock
            delegateLock = null;
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
            threadJob.Add("offsetStopwatch");

            // unlock the timebar
            if (timebarLock == this) timebarLock = null;
        }
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member