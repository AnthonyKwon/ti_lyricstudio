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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.PlayerGroup = new System.Windows.Forms.Panel();
            this.TimeLabel = new System.Windows.Forms.Label();
            this.btnSetTime = new System.Windows.Forms.Button();
            this.PreviewLabel = new System.Windows.Forms.Label();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnPlayPause = new System.Windows.Forms.Button();
            this.btnPrev = new System.Windows.Forms.Button();
            this.TimeBar = new System.Windows.Forms.TrackBar();
            this.MenuStrip = new System.Windows.Forms.MenuStrip();
            this.itmFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mItemOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mItemImport = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.mItemSave = new System.Windows.Forms.ToolStripMenuItem();
            this.mItemSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mItemQuit = new System.Windows.Forms.ToolStripMenuItem();
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
            this.CtlUpdTimer = new System.Windows.Forms.Timer(this.components);
            this.SaveDialog = new System.Windows.Forms.SaveFileDialog();
            this.EditorView = new System.Windows.Forms.DataGridView();
            this.OpenDialog = new System.Windows.Forms.OpenFileDialog();
            this.EditorMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.mItemInsert = new System.Windows.Forms.ToolStripMenuItem();
            this.mItemEmpty = new System.Windows.Forms.ToolStripMenuItem();
            this.mItemDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.mItemAddTimeCol = new System.Windows.Forms.ToolStripMenuItem();
            this.mItemDelTimeCol = new System.Windows.Forms.ToolStripMenuItem();
            this.PlayerGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TimeBar)).BeginInit();
            this.MenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.EditorView)).BeginInit();
            this.EditorMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // PlayerGroup
            // 
            this.PlayerGroup.Controls.Add(this.TimeLabel);
            this.PlayerGroup.Controls.Add(this.btnSetTime);
            this.PlayerGroup.Controls.Add(this.PreviewLabel);
            this.PlayerGroup.Controls.Add(this.btnNext);
            this.PlayerGroup.Controls.Add(this.btnStop);
            this.PlayerGroup.Controls.Add(this.btnPlayPause);
            this.PlayerGroup.Controls.Add(this.btnPrev);
            this.PlayerGroup.Controls.Add(this.TimeBar);
            this.PlayerGroup.Location = new System.Drawing.Point(12, 495);
            this.PlayerGroup.Name = "PlayerGroup";
            this.PlayerGroup.Size = new System.Drawing.Size(760, 55);
            this.PlayerGroup.TabIndex = 0;
            // 
            // TimeLabel
            // 
            this.TimeLabel.AutoSize = true;
            this.TimeLabel.Location = new System.Drawing.Point(200, 8);
            this.TimeLabel.Name = "TimeLabel";
            this.TimeLabel.Size = new System.Drawing.Size(102, 13);
            this.TimeLabel.TabIndex = 6;
            this.TimeLabel.Text = "00:00:00 / 00:00:00";
            // 
            // btnSetTime
            // 
            this.btnSetTime.Enabled = false;
            this.btnSetTime.Location = new System.Drawing.Point(119, 3);
            this.btnSetTime.Name = "btnSetTime";
            this.btnSetTime.Size = new System.Drawing.Size(75, 23);
            this.btnSetTime.TabIndex = 0;
            this.btnSetTime.Text = "Set &Time";
            this.btnSetTime.UseVisualStyleBackColor = true;
            this.btnSetTime.Click += new System.EventHandler(this.btnSetTime_Click);
            // 
            // PreviewLabel
            // 
            this.PreviewLabel.Location = new System.Drawing.Point(2, 38);
            this.PreviewLabel.Name = "PreviewLabel";
            this.PreviewLabel.Size = new System.Drawing.Size(756, 14);
            this.PreviewLabel.TabIndex = 7;
            this.PreviewLabel.Text = "Lyrics Preview will be shown here.";
            // 
            // btnNext
            // 
            this.btnNext.Enabled = false;
            this.btnNext.Font = new System.Drawing.Font("Webdings", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnNext.Location = new System.Drawing.Point(90, 3);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(23, 23);
            this.btnNext.TabIndex = 5;
            this.btnNext.Text = "8";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
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
            this.btnPlayPause.Enabled = false;
            this.btnPlayPause.Font = new System.Drawing.Font("Webdings", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPlayPause.Location = new System.Drawing.Point(32, 3);
            this.btnPlayPause.Name = "btnPlayPause";
            this.btnPlayPause.Size = new System.Drawing.Size(23, 23);
            this.btnPlayPause.TabIndex = 3;
            this.btnPlayPause.Text = "4";
            this.btnPlayPause.UseVisualStyleBackColor = true;
            this.btnPlayPause.Click += new System.EventHandler(this.btnPlayPause_Click);
            // 
            // btnPrev
            // 
            this.btnPrev.Enabled = false;
            this.btnPrev.Font = new System.Drawing.Font("Webdings", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPrev.Location = new System.Drawing.Point(3, 3);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(23, 23);
            this.btnPrev.TabIndex = 2;
            this.btnPrev.Text = "7";
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // TimeBar
            // 
            this.TimeBar.AutoSize = false;
            this.TimeBar.Enabled = false;
            this.TimeBar.Location = new System.Drawing.Point(302, 3);
            this.TimeBar.Name = "TimeBar";
            this.TimeBar.Size = new System.Drawing.Size(455, 32);
            this.TimeBar.SmallChange = 5;
            this.TimeBar.TabIndex = 1;
            this.TimeBar.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // MenuStrip
            // 
            this.MenuStrip.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itmFile,
            this.itmEdit,
            this.itmHelp});
            this.MenuStrip.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip.MinimumSize = new System.Drawing.Size(200, 0);
            this.MenuStrip.Name = "MenuStrip";
            this.MenuStrip.Size = new System.Drawing.Size(784, 24);
            this.MenuStrip.TabIndex = 2;
            this.MenuStrip.Text = "Menu";
            // 
            // itmFile
            // 
            this.itmFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.itmFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mItemOpen,
            this.ToolStripSeparator1,
            this.mItemImport,
            this.toolStripSeparator6,
            this.mItemSave,
            this.mItemSaveAs,
            this.ToolStripSeparator2,
            this.mItemQuit});
            this.itmFile.Name = "itmFile";
            this.itmFile.Size = new System.Drawing.Size(37, 20);
            this.itmFile.Text = "File";
            // 
            // mItemOpen
            // 
            this.mItemOpen.Name = "mItemOpen";
            this.mItemOpen.Size = new System.Drawing.Size(123, 22);
            this.mItemOpen.Text = "Open...";
            this.mItemOpen.Click += new System.EventHandler(this.mItemOpen_Click);
            // 
            // ToolStripSeparator1
            // 
            this.ToolStripSeparator1.Name = "ToolStripSeparator1";
            this.ToolStripSeparator1.Size = new System.Drawing.Size(120, 6);
            // 
            // mItemImport
            // 
            this.mItemImport.Enabled = false;
            this.mItemImport.Name = "mItemImport";
            this.mItemImport.Size = new System.Drawing.Size(123, 22);
            this.mItemImport.Text = "Import...";
            this.mItemImport.Click += new System.EventHandler(this.mItemImport_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(120, 6);
            // 
            // mItemSave
            // 
            this.mItemSave.Enabled = false;
            this.mItemSave.Name = "mItemSave";
            this.mItemSave.Size = new System.Drawing.Size(123, 22);
            this.mItemSave.Text = "Save";
            this.mItemSave.Click += new System.EventHandler(this.mItemSave_Click);
            // 
            // mItemSaveAs
            // 
            this.mItemSaveAs.Enabled = false;
            this.mItemSaveAs.Name = "mItemSaveAs";
            this.mItemSaveAs.Size = new System.Drawing.Size(123, 22);
            this.mItemSaveAs.Text = "Save As...";
            this.mItemSaveAs.Click += new System.EventHandler(this.mItemSaveAs_Click);
            // 
            // ToolStripSeparator2
            // 
            this.ToolStripSeparator2.Name = "ToolStripSeparator2";
            this.ToolStripSeparator2.Size = new System.Drawing.Size(120, 6);
            // 
            // mItemQuit
            // 
            this.mItemQuit.Name = "mItemQuit";
            this.mItemQuit.Size = new System.Drawing.Size(123, 22);
            this.mItemQuit.Text = "Quit";
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
            // CtlUpdTimer
            // 
            this.CtlUpdTimer.Enabled = true;
            this.CtlUpdTimer.Interval = 1;
            // 
            // SaveDialog
            // 
            this.SaveDialog.Filter = "LRC format|*.lrc|All Files|*.*";
            this.SaveDialog.Title = "Save lyrics file...";
            // 
            // EditorView
            // 
            this.EditorView.AllowDrop = true;
            this.EditorView.AllowUserToResizeRows = false;
            this.EditorView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.EditorView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.EditorView.DefaultCellStyle = dataGridViewCellStyle2;
            this.EditorView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
            this.EditorView.Location = new System.Drawing.Point(12, 36);
            this.EditorView.Name = "EditorView";
            this.EditorView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.EditorView.Size = new System.Drawing.Size(760, 446);
            this.EditorView.TabIndex = 6;
            this.EditorView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.EditorView_CellValueChanged);
            this.EditorView.ColumnAdded += new System.Windows.Forms.DataGridViewColumnEventHandler(this.EditorView_ColumnAdded);
            // 
            // OpenDialog
            // 
            this.OpenDialog.DefaultExt = "mp3";
            this.OpenDialog.Filter = "Audio Files|*.alac;*.ape;*.flac;*.m4a;*.mp3;*mp4;*.oga;*.ogg;*.opus;*.wav;*wma|Al" +
    "l Files|*.*";
            this.OpenDialog.Title = "Open workspace...";
            // 
            // EditorMenu
            // 
            this.EditorMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator8,
            this.mItemInsert,
            this.mItemEmpty,
            this.mItemDelete,
            this.toolStripSeparator7,
            this.mItemAddTimeCol,
            this.mItemDelTimeCol});
            this.EditorMenu.Name = "contextMenuStrip1";
            this.EditorMenu.ShowImageMargin = false;
            this.EditorMenu.Size = new System.Drawing.Size(210, 126);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(206, 6);
            // 
            // mItemInsert
            // 
            this.mItemInsert.Name = "mItemInsert";
            this.mItemInsert.Size = new System.Drawing.Size(209, 22);
            this.mItemInsert.Text = "Insert line below selected";
            this.mItemInsert.Click += new System.EventHandler(this.mItemInsert_Click);
            // 
            // mItemEmpty
            // 
            this.mItemEmpty.Name = "mItemEmpty";
            this.mItemEmpty.Size = new System.Drawing.Size(209, 22);
            this.mItemEmpty.Text = "Empty content of selected line";
            this.mItemEmpty.Click += new System.EventHandler(this.mItemEmpty_Click);
            // 
            // mItemDelete
            // 
            this.mItemDelete.Name = "mItemDelete";
            this.mItemDelete.Size = new System.Drawing.Size(209, 22);
            this.mItemDelete.Text = "Deleted selected line";
            this.mItemDelete.Click += new System.EventHandler(this.mItemDelete_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(206, 6);
            // 
            // mItemAddTimeCol
            // 
            this.mItemAddTimeCol.Name = "mItemAddTimeCol";
            this.mItemAddTimeCol.Size = new System.Drawing.Size(209, 22);
            this.mItemAddTimeCol.Text = "Add new time column";
            this.mItemAddTimeCol.Click += new System.EventHandler(this.mItemAddTimeCol_Click);
            // 
            // mItemDelTimeCol
            // 
            this.mItemDelTimeCol.Name = "mItemDelTimeCol";
            this.mItemDelTimeCol.Size = new System.Drawing.Size(209, 22);
            this.mItemDelTimeCol.Text = "Delete last time column";
            this.mItemDelTimeCol.Click += new System.EventHandler(this.mItemDelTimeCol_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.EditorView);
            this.Controls.Add(this.PlayerGroup);
            this.Controls.Add(this.MenuStrip);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "MainWindow";
            this.Text = "MainWindow";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.MainWindow_Closing);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.Resize += new System.EventHandler(this.MainWindow_Resize);
            this.PlayerGroup.ResumeLayout(false);
            this.PlayerGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TimeBar)).EndInit();
            this.MenuStrip.ResumeLayout(false);
            this.MenuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.EditorView)).EndInit();
            this.EditorMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        internal Panel PlayerGroup;
        internal TrackBar TimeBar;
        internal Button btnNext;
        internal Button btnStop;
        internal Button btnPlayPause;
        internal Button btnPrev;
        internal Label TimeLabel;
        internal Label PreviewLabel;
        internal MenuStrip MenuStrip;
        internal ToolStripMenuItem itmFile;
        internal ToolStripMenuItem mItemQuit;
        internal Button btnSetTime;
        internal ToolStripMenuItem itmEdit;
        internal ToolStripMenuItem itmHelp;
        internal ToolStripMenuItem mItemOpen;
        internal ToolStripSeparator ToolStripSeparator1;
        internal ToolStripMenuItem mItemSave;
        internal ToolStripMenuItem mItemSaveAs;
        internal ToolStripSeparator ToolStripSeparator2;
        internal ToolStripMenuItem ShowDebugWindowToolStripMenuItem;
        internal OpenFileDialog OpenDialog;
        internal Timer CtlUpdTimer;
        internal ToolStripMenuItem AddMultipleLinesToolStripMenuItem;
        internal SaveFileDialog SaveDialog;
        internal ToolStripMenuItem LineEditingToolStripMenuItem;
        internal ToolStripMenuItem it2InsertLine;
        internal ToolStripMenuItem it2RemoveLine;
        internal ToolStripSeparator ToolStripSeparator3;
        internal ToolStripMenuItem AboutTiLyricsStudioToolStripMenuItem;
        internal ToolStripSeparator ToolStripSeparator4;
        internal DataGridView EditorView;
        internal ToolStripSeparator ToolStripSeparator5;
        internal ToolStripMenuItem it2Optimize;
        private ToolStripMenuItem mItemImport;
        private ToolStripSeparator toolStripSeparator6;
        private ContextMenuStrip EditorMenu;
        private ToolStripMenuItem mItemInsert;
        private ToolStripMenuItem mItemDelete;
        private ToolStripSeparator toolStripSeparator8;
        private ToolStripMenuItem mItemEmpty;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripMenuItem mItemAddTimeCol;
        private ToolStripMenuItem mItemDelTimeCol;
    }
}