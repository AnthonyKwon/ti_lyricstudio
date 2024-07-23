#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using AxWMPLib;
using ti_Lyricstudio.Class;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
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
        private readonly string title = $"{AppName} {VersionInfo}";

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
        int AudioDuration = 0;

        private bool IsDirty = false;
        private FileInfo FileInfo = new FileInfo(false, false, @"%HOMEDRIVE%\Music", "New File", null);
        internal string AudioLength, PlayTime;
        private System.Threading.Thread SecondThread;

        public MainWindow()
        {
            SecondThread = new System.Threading.Thread(TimeCheckingWork);
            InitializeComponent();
            // remove image margin from menu strip
            // ref: https://stackoverflow.com/a/32579262
            SetValuesOnSubItems(MenuStrip.Items.OfType<ToolStripMenuItem>().ToList());
            // bind event to drag & drop event variable
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
                if (result == DialogResult.No) e.Cancel = true;
                return;
            }

            if (SecondThread.IsAlive == true)
            {
                SecondThread.Abort();
            }
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            // Initialize Form Settings
            Text = title;
            if (SecondThread.IsAlive == false)
            {
                SecondThread.Start();
            }
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
            trcTime.Width = pnlController.Width - 305;
            // Resize lblPreview
            lblPreview.Width = pnlController.Width - 4;
        }

        private void btnFF_Click(object sender, EventArgs e)
        {
            if (AudioDuration * player.Position <= 0.9)
            {
                player.Position += 0.1f;
            }
        }

        private void btnPlayPause_Click(object sender, EventArgs e)
        {
            // Play/Pause Event
            if (player.IsPlaying == true)
            {
                player.Pause();
            }
            else
            {
                player.Play();
            }
        }

        private void btnRewind_Click(object sender, EventArgs e)
        {
            if (AudioDuration * player.Position >= 0.1)
            {
                player.Position -= 0.1f;
            }
        }

        private void btnSetTime_Click(object sender, EventArgs e)
        {
            CData[DataGridView.CurrentRow.Index].Time = PlayTime;
            DataGridView.Refresh();
            DataGridView.CurrentCell = DataGridView[0, DataGridView.CurrentCellAddress.Y + 1];
            My.MyProject.Forms.DebugWindow.AddDLine("Current Lyrics Database size: " + CData.Count.ToString());
            IsDirty = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (player.IsSeekable == true)
            {
                player.Stop();
            }
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
                CData[DataGridView.CurrentRow.Index].Time = Constants.vbNullString;
                CData[DataGridView.CurrentRow.Index].Lyric = Constants.vbNullString;
            }
        }

        private void it2Optimize_Click(object sender, EventArgs e)
        {

        }

        private void it2RemoveLine_Click(object sender, EventArgs e)
        {

        }

        private void LPreviewTimer_Tick(object sender, EventArgs e)
        {
            /*
            double TCurT, TNextT = default;
            var TempStr = new string[3];
            try
            {
                if (!(CData == null) & AxWindowsMediaPlayer.playState == WMPLib.WMPPlayState.wmppsPlaying)
                {
                    for (int i = 1, loopTo = CData.Count - 1; i <= loopTo; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(CData[i - 1].Time))
                        {
                            // Get current lyric's time as position
                            TempStr = CData[i - 1].Time.Split(new[] { ':', '.' });
                            TCurT = ((Convert.ToDouble(TempStr[0]) * 60d + Convert.ToDouble(TempStr[1])) * 100d + Convert.ToDouble(TempStr[2])) / 100d;
                            if (i < CData.Count - 1 & !string.IsNullOrWhiteSpace(CData[i].Time))
                            {
                                // Get next lyric's time as position
                                TempStr = CData[i].Time.Split(new[] { ':', '.' });
                                TNextT = ((Convert.ToDouble(TempStr[0]) * 60d + Convert.ToDouble(TempStr[1])) * 100d + Convert.ToDouble(TempStr[2])) / 100d;
                            }
                            else if (i < CData.Count - 2)
                            {
                                if (string.IsNullOrWhiteSpace(CData[i].Time))
                                {
                                    for (int j = i, loopTo1 = CData.Count - 1; j <= loopTo1; j++)
                                    {
                                        if (!string.IsNullOrWhiteSpace(CData[j].Time))
                                        {
                                            // Get next lyric's time as position
                                            TempStr = CData[j].Time.Split(new[] { ':', '.' });
                                            TNextT = ((Convert.ToDouble(TempStr[0]) * 60d + Convert.ToDouble(TempStr[1])) * 100d + Convert.ToDouble(TempStr[2])) / 100d;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (i < CData.Count - 1 & AxWindowsMediaPlayer.Ctlcontrols.currentPosition > TCurT & AxWindowsMediaPlayer.Ctlcontrols.currentPosition < TNextT | i == CData.Count - 1 & AxWindowsMediaPlayer.Ctlcontrols.currentPosition > TCurT)
                            {
                                lblPreview.Text = CData[i - 1].Lyric;
                            }
                        }
                    }
                }
                else
                {
                    lblPreview.Text = "Lyrics Preview will be shown here.";
                }
            }
            catch (Exception ex)
            {
                if (!(this == null) & !(My.MyProject.Forms.DebugWindow == null))
                {
                    try
                    {
                        My.MyProject.Forms.DebugWindow.AddDLine("Thread thrown exception-type message: " + ex.ToString(), 2);
                    }
                    catch (Exception Ignore)
                    {
                    }
                }
            }
            */
        }

        private void RefreshForm()
        {
            if (opened == true)
            {
                // Set song's length
                trcTime.Maximum = AudioDuration;
                btnRewind.Enabled = true;
                btnPlayPause.Enabled = true;
                btnStop.Enabled = true;
                btnFF.Enabled = true;
                btnSetTime.Enabled = true;
                lblTime.Text = ((int)(player.Position * AudioDuration)) + "/" + AudioDuration;
                if (IsDirty == true)
                {
                    Text = title + " - " + FileInfo.Location + @"\" + FileInfo.FileName + ".lrc*";
                }
                else
                {
                    Text = title + " - " + FileInfo.Location + @"\" + FileInfo.FileName + ".lrc";
                }
                if (trcTime.Maximum > 0)
                {
                    trcTime.Value = Math.Abs((int)(player.Position * AudioDuration));
                }
            }
            else
            {
                btnRewind.Enabled = false;
                btnPlayPause.Enabled = false;
                btnStop.Enabled = false;
                btnFF.Enabled = false;
                btnSetTime.Enabled = false;
                Text = title;
            }
        }

        private delegate void TimeCheckingAppliesInvoker(string ReturnText);
        public void TimeCheckingApplies(string ReturnText)
        {
            lblPreview.Text = ReturnText;
        }

        public void TimeCheckingWork()
        {
            ThreadDebugWriteInvoker DebugWriteInvoker;
            DebugWriteInvoker = ThreadDebugWrite;
            TimeCheckingAppliesInvoker iTimeCheckingApplies;
            iTimeCheckingApplies = TimeCheckingApplies;
            while (true)
            {
                try
                {
                    System.Threading.Thread.Sleep(10);
                }
                catch (Exception ex)
                {
                    if (!(this == null) & !(My.MyProject.Forms.DebugWindow == null))
                    {
                        try
                        {
                            Invoke(DebugWriteInvoker, ["Exception", ex.ToString()]);
                        }
                        catch (Exception Ignore)
                        {
                        }
                    }
                }
            }
        }

        private delegate void ThreadDebugWriteInvoker(string Type, string Message);
        public void ThreadDebugWrite(string Type, string Message)
        {
            if (Type == "Exception")
            {
                My.MyProject.Forms.DebugWindow.AddDLine("Thread thrown exception-type message: " + Message, 2);
            }
            else if (Type == "Message")
            {
                My.MyProject.Forms.DebugWindow.AddDLine("Thread output: " + Message);
            }
        }

        private void trcTime_Scroll(object sender, EventArgs e)
        {
            //AxWindowsMediaPlayer.Ctlcontrols.currentPosition = trcTime.Value / 100d;
        }

        private void RefreshForm(object sender, EventArgs e) => RefreshForm();
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member