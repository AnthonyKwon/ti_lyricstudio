using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ti_Lyricstudio
{
    [Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    public partial class MainWindow : Form
    {

        // Form overrides dispose to clean up the component list.
        [DebuggerNonUserCode()]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && components is not null)
                {
                    components.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        // Required by the Windows Form Designer
        private System.ComponentModel.IContainer components;

        // NOTE: The following procedure is required by the Windows Form Designer
        // It can be modified using the Windows Form Designer.  
        // Do not modify it using the code editor.
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlController = new System.Windows.Forms.Panel();
            this.lblTime = new System.Windows.Forms.Label();
            this.btnSetTime = new System.Windows.Forms.Button();
            this.lblPreview = new System.Windows.Forms.Label();
            this.btnFF = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnPlayPause = new System.Windows.Forms.Button();
            this.btnRewind = new System.Windows.Forms.Button();
            this.trcTime = new System.Windows.Forms.TrackBar();
            this.MenuStrip = new System.Windows.Forms.MenuStrip();
            this.itmFile = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuNew = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuImport = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuQuit = new System.Windows.Forms.ToolStripMenuItem();
            this.itmEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.AddMultipleLinesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.LineEditingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.it2InsertLine = new System.Windows.Forms.ToolStripMenuItem();
            this.it2RemoveLine = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.it2Optimize = new System.Windows.Forms.ToolStripMenuItem();
            this.itmHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.ShowDebugWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.AboutTiLyricsStudioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlHidden = new System.Windows.Forms.Panel();
            this.AxWindowsMediaPlayer = new AxWMPLib.AxWindowsMediaPlayer();
            this.CtlUpdTimer = new System.Windows.Forms.Timer(this.components);
            this.SaveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.DataGridView = new System.Windows.Forms.DataGridView();
            this.LPreviewTimer = new System.Windows.Forms.Timer(this.components);
            this.OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.pnlController.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trcTime)).BeginInit();
            this.MenuStrip.SuspendLayout();
            this.pnlHidden.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AxWindowsMediaPlayer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlController
            // 
            this.pnlController.Controls.Add(this.lblTime);
            this.pnlController.Controls.Add(this.btnSetTime);
            this.pnlController.Controls.Add(this.lblPreview);
            this.pnlController.Controls.Add(this.btnFF);
            this.pnlController.Controls.Add(this.btnStop);
            this.pnlController.Controls.Add(this.btnPlayPause);
            this.pnlController.Controls.Add(this.btnRewind);
            this.pnlController.Controls.Add(this.trcTime);
            this.pnlController.Location = new System.Drawing.Point(12, 488);
            this.pnlController.Name = "pnlController";
            this.pnlController.Size = new System.Drawing.Size(760, 61);
            this.pnlController.TabIndex = 0;
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Location = new System.Drawing.Point(200, 8);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(96, 13);
            this.lblTime.TabIndex = 6;
            this.lblTime.Text = "00:00:00/00:00:00";
            // 
            // btnSetTime
            // 
            this.btnSetTime.Location = new System.Drawing.Point(119, 3);
            this.btnSetTime.Name = "btnSetTime";
            this.btnSetTime.Size = new System.Drawing.Size(75, 23);
            this.btnSetTime.TabIndex = 0;
            this.btnSetTime.Text = "Set &Time";
            this.btnSetTime.UseVisualStyleBackColor = true;
            this.btnSetTime.Click += new System.EventHandler(this.btnSetTime_Click);
            // 
            // lblPreview
            // 
            this.lblPreview.Location = new System.Drawing.Point(2, 38);
            this.lblPreview.Name = "lblPreview";
            this.lblPreview.Size = new System.Drawing.Size(756, 14);
            this.lblPreview.TabIndex = 7;
            this.lblPreview.Text = "Lyrics Preview will be shown here.";
            // 
            // btnFF
            // 
            this.btnFF.Font = new System.Drawing.Font("Webdings", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnFF.Location = new System.Drawing.Point(90, 3);
            this.btnFF.Name = "btnFF";
            this.btnFF.Size = new System.Drawing.Size(23, 23);
            this.btnFF.TabIndex = 5;
            this.btnFF.Text = "8";
            this.btnFF.UseVisualStyleBackColor = true;
            this.btnFF.Click += new System.EventHandler(this.btnFF_Click);
            // 
            // btnStop
            // 
            this.btnStop.Font = new System.Drawing.Font("Webdings", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnStop.Location = new System.Drawing.Point(61, 3);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(23, 23);
            this.btnStop.TabIndex = 4;
            this.btnStop.Text = "<";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnPlayPause
            // 
            this.btnPlayPause.Font = new System.Drawing.Font("Webdings", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPlayPause.Location = new System.Drawing.Point(32, 3);
            this.btnPlayPause.Name = "btnPlayPause";
            this.btnPlayPause.Size = new System.Drawing.Size(23, 23);
            this.btnPlayPause.TabIndex = 3;
            this.btnPlayPause.Text = "4";
            this.btnPlayPause.UseVisualStyleBackColor = true;
            this.btnPlayPause.Click += new System.EventHandler(this.btnPlayPause_Click);
            // 
            // btnRewind
            // 
            this.btnRewind.Font = new System.Drawing.Font("Webdings", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnRewind.Location = new System.Drawing.Point(3, 3);
            this.btnRewind.Name = "btnRewind";
            this.btnRewind.Size = new System.Drawing.Size(23, 23);
            this.btnRewind.TabIndex = 2;
            this.btnRewind.Text = "7";
            this.btnRewind.UseVisualStyleBackColor = true;
            this.btnRewind.Click += new System.EventHandler(this.btnRewind_Click);
            // 
            // trcTime
            // 
            this.trcTime.AutoSize = false;
            this.trcTime.Location = new System.Drawing.Point(302, 3);
            this.trcTime.Name = "trcTime";
            this.trcTime.Size = new System.Drawing.Size(455, 32);
            this.trcTime.TabIndex = 1;
            this.trcTime.Scroll += new System.EventHandler(this.trcTime_Scroll);
            // 
            // MenuStrip
            // 
            this.MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itmFile,
            this.itmEdit,
            this.itmHelp});
            this.MenuStrip.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip.Name = "MenuStrip";
            this.MenuStrip.Size = new System.Drawing.Size(784, 24);
            this.MenuStrip.TabIndex = 2;
            this.MenuStrip.Text = "Menu";
            // 
            // itmFile
            // 
            this.itmFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuNew,
            this.MenuOpen,
            this.ToolStripSeparator1,
            this.MenuImport,
            this.toolStripSeparator6,
            this.MenuSave,
            this.MenuSaveAs,
            this.ToolStripSeparator2,
            this.MenuQuit});
            this.itmFile.Name = "itmFile";
            this.itmFile.Size = new System.Drawing.Size(37, 20);
            this.itmFile.Text = "File";
            // 
            // MenuNew
            // 
            this.MenuNew.Name = "MenuNew";
            this.MenuNew.Size = new System.Drawing.Size(123, 22);
            this.MenuNew.Text = "New";
            this.MenuNew.Click += new System.EventHandler(this.MenuNew_Click);
            // 
            // MenuOpen
            // 
            this.MenuOpen.Name = "MenuOpen";
            this.MenuOpen.Size = new System.Drawing.Size(123, 22);
            this.MenuOpen.Text = "Open...";
            this.MenuOpen.Click += new System.EventHandler(this.MenuOpen_Click);
            // 
            // ToolStripSeparator1
            // 
            this.ToolStripSeparator1.Name = "ToolStripSeparator1";
            this.ToolStripSeparator1.Size = new System.Drawing.Size(120, 6);
            // 
            // MenuImport
            // 
            this.MenuImport.Name = "MenuImport";
            this.MenuImport.Size = new System.Drawing.Size(123, 22);
            this.MenuImport.Text = "Import...";
            this.MenuImport.Click += new System.EventHandler(this.MenuImport_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(120, 6);
            // 
            // MenuSave
            // 
            this.MenuSave.Enabled = false;
            this.MenuSave.Name = "MenuSave";
            this.MenuSave.Size = new System.Drawing.Size(123, 22);
            this.MenuSave.Text = "Save";
            this.MenuSave.Click += new System.EventHandler(this.MenuSave_Click);
            // 
            // MenuSaveAs
            // 
            this.MenuSaveAs.Enabled = false;
            this.MenuSaveAs.Name = "MenuSaveAs";
            this.MenuSaveAs.Size = new System.Drawing.Size(123, 22);
            this.MenuSaveAs.Text = "Save As...";
            this.MenuSaveAs.Click += new System.EventHandler(this.MenuSaveAs_Click);
            // 
            // ToolStripSeparator2
            // 
            this.ToolStripSeparator2.Name = "ToolStripSeparator2";
            this.ToolStripSeparator2.Size = new System.Drawing.Size(120, 6);
            // 
            // MenuQuit
            // 
            this.MenuQuit.Name = "MenuQuit";
            this.MenuQuit.Size = new System.Drawing.Size(123, 22);
            this.MenuQuit.Text = "Quit";
            // 
            // itmEdit
            // 
            this.itmEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AddMultipleLinesToolStripMenuItem,
            this.ToolStripSeparator4,
            this.LineEditingToolStripMenuItem});
            this.itmEdit.Name = "itmEdit";
            this.itmEdit.Size = new System.Drawing.Size(39, 20);
            this.itmEdit.Text = "Edit";
            // 
            // AddMultipleLinesToolStripMenuItem
            // 
            this.AddMultipleLinesToolStripMenuItem.Name = "AddMultipleLinesToolStripMenuItem";
            this.AddMultipleLinesToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.AddMultipleLinesToolStripMenuItem.Text = "Add multiple lines...";
            this.AddMultipleLinesToolStripMenuItem.Click += new System.EventHandler(this.it1AddMultipleLines_Click);
            // 
            // ToolStripSeparator4
            // 
            this.ToolStripSeparator4.Name = "ToolStripSeparator4";
            this.ToolStripSeparator4.Size = new System.Drawing.Size(176, 6);
            // 
            // LineEditingToolStripMenuItem
            // 
            this.LineEditingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.it2InsertLine,
            this.it2RemoveLine,
            this.ToolStripSeparator5,
            this.it2Optimize});
            this.LineEditingToolStripMenuItem.Name = "LineEditingToolStripMenuItem";
            this.LineEditingToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.LineEditingToolStripMenuItem.Text = "Line Editing";
            // 
            // it2InsertLine
            // 
            this.it2InsertLine.Name = "it2InsertLine";
            this.it2InsertLine.Size = new System.Drawing.Size(142, 22);
            this.it2InsertLine.Text = "Insert Line";
            this.it2InsertLine.Click += new System.EventHandler(this.it2InsertLine_Click);
            // 
            // it2RemoveLine
            // 
            this.it2RemoveLine.Name = "it2RemoveLine";
            this.it2RemoveLine.Size = new System.Drawing.Size(142, 22);
            this.it2RemoveLine.Text = "Remove Line";
            this.it2RemoveLine.Click += new System.EventHandler(this.it2RemoveLine_Click);
            // 
            // ToolStripSeparator5
            // 
            this.ToolStripSeparator5.Name = "ToolStripSeparator5";
            this.ToolStripSeparator5.Size = new System.Drawing.Size(139, 6);
            // 
            // it2Optimize
            // 
            this.it2Optimize.Name = "it2Optimize";
            this.it2Optimize.Size = new System.Drawing.Size(142, 22);
            this.it2Optimize.Text = "Optimize";
            this.it2Optimize.Click += new System.EventHandler(this.it2Optimize_Click);
            // 
            // itmHelp
            // 
            this.itmHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ShowDebugWindowToolStripMenuItem,
            this.ToolStripSeparator3,
            this.AboutTiLyricsStudioToolStripMenuItem});
            this.itmHelp.Name = "itmHelp";
            this.itmHelp.Size = new System.Drawing.Size(44, 20);
            this.itmHelp.Text = "Help";
            this.itmHelp.Click += new System.EventHandler(this.itmHelp_Click);
            // 
            // ShowDebugWindowToolStripMenuItem
            // 
            this.ShowDebugWindowToolStripMenuItem.Name = "ShowDebugWindowToolStripMenuItem";
            this.ShowDebugWindowToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.ShowDebugWindowToolStripMenuItem.Text = "Show Debug Window";
            this.ShowDebugWindowToolStripMenuItem.Click += new System.EventHandler(this.it1ShowDebugWindow_Click);
            // 
            // ToolStripSeparator3
            // 
            this.ToolStripSeparator3.Name = "ToolStripSeparator3";
            this.ToolStripSeparator3.Size = new System.Drawing.Size(192, 6);
            // 
            // AboutTiLyricsStudioToolStripMenuItem
            // 
            this.AboutTiLyricsStudioToolStripMenuItem.Name = "AboutTiLyricsStudioToolStripMenuItem";
            this.AboutTiLyricsStudioToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.AboutTiLyricsStudioToolStripMenuItem.Text = "About ti: LyricsStudio...";
            // 
            // pnlHidden
            // 
            this.pnlHidden.Controls.Add(this.AxWindowsMediaPlayer);
            this.pnlHidden.Location = new System.Drawing.Point(572, 27);
            this.pnlHidden.Name = "pnlHidden";
            this.pnlHidden.Size = new System.Drawing.Size(200, 458);
            this.pnlHidden.TabIndex = 5;
            this.pnlHidden.Visible = false;
            // 
            // AxWindowsMediaPlayer
            // 
            this.AxWindowsMediaPlayer.Enabled = true;
            this.AxWindowsMediaPlayer.Location = new System.Drawing.Point(3, 3);
            this.AxWindowsMediaPlayer.Name = "AxWindowsMediaPlayer";
            this.AxWindowsMediaPlayer.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("AxWindowsMediaPlayer.OcxState")));
            this.AxWindowsMediaPlayer.Size = new System.Drawing.Size(194, 45);
            this.AxWindowsMediaPlayer.TabIndex = 0;
            this.AxWindowsMediaPlayer.PlayStateChange += new AxWMPLib._WMPOCXEvents_PlayStateChangeEventHandler(this.AxWindowsMediaPlayer_PlayStateChange);
            // 
            // CtlUpdTimer
            // 
            this.CtlUpdTimer.Enabled = true;
            this.CtlUpdTimer.Interval = 1;
            this.CtlUpdTimer.Tick += new System.EventHandler(this.RefreshForm);
            // 
            // SaveFileDialog
            // 
            this.SaveFileDialog.FileName = "LRC format|*.lrc|All Files|*.*";
            // 
            // DataGridView
            // 
            this.DataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.DataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.DataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DataGridView.DefaultCellStyle = dataGridViewCellStyle1;
            this.DataGridView.Location = new System.Drawing.Point(12, 36);
            this.DataGridView.Name = "DataGridView";
            this.DataGridView.Size = new System.Drawing.Size(554, 446);
            this.DataGridView.TabIndex = 6;
            this.DataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView_CellValueChanged);
            // 
            // LPreviewTimer
            // 
            this.LPreviewTimer.Enabled = true;
            this.LPreviewTimer.Tick += new System.EventHandler(this.LPreviewTimer_Tick);
            // 
            // OpenFileDialog
            // 
            this.OpenFileDialog.DefaultExt = "mp3";
            this.OpenFileDialog.FileName = "OpenFileDialog";
            this.OpenFileDialog.Filter = "Audio Files|*.alac;*.ape;*.flac;*.m4a;*.mp3;*mp4;*.oga;*.ogg;*.opus;*.wav;*wma|Al" +
    "l Files|*.*";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.DataGridView);
            this.Controls.Add(this.pnlHidden);
            this.Controls.Add(this.pnlController);
            this.Controls.Add(this.MenuStrip);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "MainWindow";
            this.Text = "MainWindow";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.MainWindow_Closing);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.Resize += new System.EventHandler(this.MainWindow_Resize);
            this.pnlController.ResumeLayout(false);
            this.pnlController.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trcTime)).EndInit();
            this.MenuStrip.ResumeLayout(false);
            this.MenuStrip.PerformLayout();
            this.pnlHidden.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.AxWindowsMediaPlayer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        internal Panel pnlController;
        internal TrackBar trcTime;
        internal Button btnFF;
        internal Button btnStop;
        internal Button btnPlayPause;
        internal Button btnRewind;
        internal Label lblTime;
        internal Label lblPreview;
        internal MenuStrip MenuStrip;
        internal ToolStripMenuItem itmFile;
        internal ToolStripMenuItem MenuQuit;
        internal Button btnSetTime;
        internal ToolStripMenuItem itmEdit;
        internal ToolStripMenuItem itmHelp;
        internal ToolStripMenuItem MenuNew;
        internal ToolStripMenuItem MenuOpen;
        internal ToolStripSeparator ToolStripSeparator1;
        internal ToolStripMenuItem MenuSave;
        internal ToolStripMenuItem MenuSaveAs;
        internal ToolStripSeparator ToolStripSeparator2;
        internal Panel pnlHidden;
        internal AxWMPLib.AxWindowsMediaPlayer AxWindowsMediaPlayer;
        internal ToolStripMenuItem ShowDebugWindowToolStripMenuItem;
        internal OpenFileDialog OpenFileDialog;
        internal Timer CtlUpdTimer;
        internal ToolStripMenuItem AddMultipleLinesToolStripMenuItem;
        internal SaveFileDialog SaveFileDialog;
        internal ToolStripMenuItem LineEditingToolStripMenuItem;
        internal ToolStripMenuItem it2InsertLine;
        internal ToolStripMenuItem it2RemoveLine;
        internal ToolStripSeparator ToolStripSeparator3;
        internal ToolStripMenuItem AboutTiLyricsStudioToolStripMenuItem;
        internal ToolStripSeparator ToolStripSeparator4;
        internal DataGridView DataGridView;
        internal Timer LPreviewTimer;
        internal ToolStripSeparator ToolStripSeparator5;
        internal ToolStripMenuItem it2Optimize;
        private ToolStripMenuItem MenuImport;
        private ToolStripSeparator toolStripSeparator6;
    }
}