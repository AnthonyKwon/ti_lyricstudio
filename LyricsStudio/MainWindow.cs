#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using AxWMPLib;
using ti_Lyricstudio.Class;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

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

        // marker to check if file opened
        private bool fileOpened = false;

        // information of the lyrics workspace file
        private LyricsFile file;

        private bool IsDirty = false;
        private FileInfo FileInfo = new FileInfo(false, false, @"%HOMEDRIVE%\Music", "New File", null);
        internal string AudioLength, PlayTime;
        private System.Threading.Thread SecondThread;

        public MainWindow()
        {
            SecondThread = new System.Threading.Thread(TimeCheckingWork);
            InitializeComponent();
        }

        // Form Function
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
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

        private void AxWindowsMediaPlayer_PlayStateChange(object sender, _WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (AxWindowsMediaPlayer.playState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                btnPlayPause.Text = ";";
            }
            else
            {
                btnPlayPause.Text = "4";
            }
        }

        private void btnFF_Click(object sender, EventArgs e)
        {
            if (AxWindowsMediaPlayer.currentMedia.duration - AxWindowsMediaPlayer.Ctlcontrols.currentPosition > 5d)
            {
                AxWindowsMediaPlayer.Ctlcontrols.currentPosition = AxWindowsMediaPlayer.Ctlcontrols.currentPosition + 5d;
            }
        }

        private void btnPlayPause_Click(object sender, EventArgs e)
        {
            if (Conversions.ToBoolean(FileInfo.AudioLoaded) == true)
            {
                // Play/Pause Event
                if (AxWindowsMediaPlayer.playState == WMPLib.WMPPlayState.wmppsPlaying)
                {
                    AxWindowsMediaPlayer.Ctlcontrols.pause();
                }
                else
                {
                    AxWindowsMediaPlayer.Ctlcontrols.play();
                }
            }
            else
            {
            }
        }

        private void btnRewind_Click(object sender, EventArgs e)
        {
            if (AxWindowsMediaPlayer.Ctlcontrols.currentPosition > 5d)
            {
                AxWindowsMediaPlayer.Ctlcontrols.currentPosition = AxWindowsMediaPlayer.Ctlcontrols.currentPosition - 5d;
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
            AxWindowsMediaPlayer.Ctlcontrols.stop();
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
                DataGridView_AddLine(Constants.vbNullString, Constants.vbNullString);
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

        private void it2OpenAudio_Click(object sender, EventArgs e)
        {
            {
                var withBlock = OpenFileDialog;
                withBlock.FileName = FileInfo.Location + @"\" + FileInfo.FileName + FileInfo.Extension;
                withBlock.Filter = "MPEG Audio Layer III (*.mp3)|*.mp3|Waveform Audio File Format (*.wav)|*.wav|All Files (*.*)|*.*";
                withBlock.InitialDirectory = FileInfo.Location;
                withBlock.RestoreDirectory = true;
                if (withBlock.ShowDialog() == DialogResult.OK)
                {
                    // Fill WorkFiles variable
                    FileInfo.Location = System.IO.Path.GetDirectoryName(withBlock.FileName);
                    FileInfo.FileName = System.IO.Path.GetFileNameWithoutExtension(withBlock.FileName);
                    FileInfo.Extension = System.IO.Path.GetExtension(withBlock.FileName);
                    //FileInfoManage("LoadA"); // Call FileInfoManage() with LoadA
                }
            }
        }

        private void it2OpenLyricsFile_Click(object sender, EventArgs e)
        {
            {
                var withBlock = OpenFileDialog;
                withBlock.FileName = FileInfo.Location + @"\" + FileInfo.FileName + ".lrc";
                withBlock.Filter = "LRC Lyrics File (*.lrc)|*.lrc|All Files (*.*)|*.*";
                withBlock.InitialDirectory = FileInfo.Location;
                withBlock.RestoreDirectory = true;
                if (withBlock.ShowDialog() == DialogResult.OK)
                {
                    // Fill WorkFiles variable
                    FileInfo.Location = System.IO.Path.GetDirectoryName(withBlock.FileName);
                    FileInfo.FileName = System.IO.Path.GetFileNameWithoutExtension(withBlock.FileName);
                    //FileInfoManage("LoadL"); // Call FileInfoManage() with LoadL
                }
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
        }

        private void RefreshForm()
        {
            try
            {
                if (Conversions.ToBoolean(FileInfo.AudioLoaded) == true)
                {
                    // Set song's length
                    AudioLength = Math.Truncate(Convert.ToDouble(AxWindowsMediaPlayer.currentMedia.duration) / 60d).ToString("0#") + ":" + (Math.Truncate(AxWindowsMediaPlayer.currentMedia.duration) % 60d).ToString("0#") + "." + (Math.Truncate(AxWindowsMediaPlayer.currentMedia.duration * 100d) % 100d).ToString("0#");
                    trcTime.Maximum = (int)Math.Round(Math.Truncate(AxWindowsMediaPlayer.currentMedia.duration * 100d));
                    btnRewind.Enabled = true;
                    btnPlayPause.Enabled = true;
                    btnStop.Enabled = true;
                    btnFF.Enabled = true;
                    btnSetTime.Enabled = true;
                    lblTime.Text = PlayTime + "/" + AudioLength;
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
                        trcTime.Value = (int)Math.Round(Math.Truncate(AxWindowsMediaPlayer.Ctlcontrols.currentPosition * 100d));
                    }
                }
                else
                {
                    AudioLength = "00:00.00";
                    btnRewind.Enabled = false;
                    btnPlayPause.Enabled = false;
                    btnStop.Enabled = false;
                    btnFF.Enabled = false;
                    btnSetTime.Enabled = false;
                    PlayTime = "00:00.00";
                    Text = title;
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
                    if (Conversions.ToBoolean(FileInfo.AudioLoaded) == true)
                    {
                        PlayTime = Math.Floor(Convert.ToDouble(AxWindowsMediaPlayer.Ctlcontrols.currentPosition) / 60d).ToString("0#") + ":" + (Math.Floor(AxWindowsMediaPlayer.Ctlcontrols.currentPosition) % 60d).ToString("0#") + "." + (Math.Floor(AxWindowsMediaPlayer.Ctlcontrols.currentPosition * 100d) % 100d).ToString("0#");
                    }
                    System.Threading.Thread.Sleep(10);
                }
                catch (Exception ex)
                {
                    if (!(this == null) & !(My.MyProject.Forms.DebugWindow == null))
                    {
                        try
                        {
                            Invoke(DebugWriteInvoker, new[] { "Exception", ex.ToString() });
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

        private void itmHelp_Click(object sender, EventArgs e)
        {

        }

        private void trcTime_Scroll(object sender, EventArgs e)
        {
            AxWindowsMediaPlayer.Ctlcontrols.currentPosition = trcTime.Value / 100d;
        }

        private void RefreshForm(object sender, EventArgs e) => RefreshForm();
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member