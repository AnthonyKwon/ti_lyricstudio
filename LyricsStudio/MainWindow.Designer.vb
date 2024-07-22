<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MainWindow
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainWindow))
        Me.pnlController = New System.Windows.Forms.Panel()
        Me.lblTime = New System.Windows.Forms.Label()
        Me.btnSetTime = New System.Windows.Forms.Button()
        Me.lblPreview = New System.Windows.Forms.Label()
        Me.btnFF = New System.Windows.Forms.Button()
        Me.btnStop = New System.Windows.Forms.Button()
        Me.btnPlayPause = New System.Windows.Forms.Button()
        Me.btnRewind = New System.Windows.Forms.Button()
        Me.trcTime = New System.Windows.Forms.TrackBar()
        Me.MenuStrip = New System.Windows.Forms.MenuStrip()
        Me.itmFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.it1New = New System.Windows.Forms.ToolStripMenuItem()
        Me.it1Open = New System.Windows.Forms.ToolStripMenuItem()
        Me.it2OpenAudio = New System.Windows.Forms.ToolStripMenuItem()
        Me.it2OpenLyricsFIle = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.it1Save = New System.Windows.Forms.ToolStripMenuItem()
        Me.it1SaveAs = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.it1Quit = New System.Windows.Forms.ToolStripMenuItem()
        Me.itmEdit = New System.Windows.Forms.ToolStripMenuItem()
        Me.AddMultipleLinesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
        Me.LineEditingToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.it2InsertLine = New System.Windows.Forms.ToolStripMenuItem()
        Me.it2RemoveLine = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator()
        Me.it2Optimize = New System.Windows.Forms.ToolStripMenuItem()
        Me.itmHelp = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowDebugWindowToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.AboutTiLyricsStudioToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.pnlHidden = New System.Windows.Forms.Panel()
        Me.AxWindowsMediaPlayer = New AxWMPLib.AxWindowsMediaPlayer()
        Me.OpenFileDialog = New System.Windows.Forms.OpenFileDialog()
        Me.CtlUpdTimer = New System.Windows.Forms.Timer(Me.components)
        Me.SaveFileDialog = New System.Windows.Forms.SaveFileDialog()
        Me.DataGridView = New System.Windows.Forms.DataGridView()
        Me.LPreviewTimer = New System.Windows.Forms.Timer(Me.components)
        Me.pnlController.SuspendLayout()
        CType(Me.trcTime, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MenuStrip.SuspendLayout()
        Me.pnlHidden.SuspendLayout()
        CType(Me.AxWindowsMediaPlayer, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'pnlController
        '
        Me.pnlController.Controls.Add(Me.lblTime)
        Me.pnlController.Controls.Add(Me.btnSetTime)
        Me.pnlController.Controls.Add(Me.lblPreview)
        Me.pnlController.Controls.Add(Me.btnFF)
        Me.pnlController.Controls.Add(Me.btnStop)
        Me.pnlController.Controls.Add(Me.btnPlayPause)
        Me.pnlController.Controls.Add(Me.btnRewind)
        Me.pnlController.Controls.Add(Me.trcTime)
        Me.pnlController.Location = New System.Drawing.Point(12, 488)
        Me.pnlController.Name = "pnlController"
        Me.pnlController.Size = New System.Drawing.Size(760, 61)
        Me.pnlController.TabIndex = 0
        '
        'lblTime
        '
        Me.lblTime.AutoSize = True
        Me.lblTime.Location = New System.Drawing.Point(200, 8)
        Me.lblTime.Name = "lblTime"
        Me.lblTime.Size = New System.Drawing.Size(96, 13)
        Me.lblTime.TabIndex = 6
        Me.lblTime.Text = "00:00:00/00:00:00"
        '
        'btnSetTime
        '
        Me.btnSetTime.Location = New System.Drawing.Point(119, 3)
        Me.btnSetTime.Name = "btnSetTime"
        Me.btnSetTime.Size = New System.Drawing.Size(75, 23)
        Me.btnSetTime.TabIndex = 0
        Me.btnSetTime.Text = "Set &Time"
        Me.btnSetTime.UseVisualStyleBackColor = True
        '
        'lblPreview
        '
        Me.lblPreview.Location = New System.Drawing.Point(2, 38)
        Me.lblPreview.Name = "lblPreview"
        Me.lblPreview.Size = New System.Drawing.Size(756, 14)
        Me.lblPreview.TabIndex = 7
        Me.lblPreview.Text = "Lyrics Preview will be shown here."
        '
        'btnFF
        '
        Me.btnFF.Font = New System.Drawing.Font("Webdings", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.btnFF.Location = New System.Drawing.Point(90, 3)
        Me.btnFF.Name = "btnFF"
        Me.btnFF.Size = New System.Drawing.Size(23, 23)
        Me.btnFF.TabIndex = 5
        Me.btnFF.Text = "8"
        Me.btnFF.UseVisualStyleBackColor = True
        '
        'btnStop
        '
        Me.btnStop.Font = New System.Drawing.Font("Webdings", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.btnStop.Location = New System.Drawing.Point(61, 3)
        Me.btnStop.Name = "btnStop"
        Me.btnStop.Size = New System.Drawing.Size(23, 23)
        Me.btnStop.TabIndex = 4
        Me.btnStop.Text = "<"
        Me.btnStop.UseVisualStyleBackColor = True
        '
        'btnPlayPause
        '
        Me.btnPlayPause.Font = New System.Drawing.Font("Webdings", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.btnPlayPause.Location = New System.Drawing.Point(32, 3)
        Me.btnPlayPause.Name = "btnPlayPause"
        Me.btnPlayPause.Size = New System.Drawing.Size(23, 23)
        Me.btnPlayPause.TabIndex = 3
        Me.btnPlayPause.Text = "4"
        Me.btnPlayPause.UseVisualStyleBackColor = True
        '
        'btnRewind
        '
        Me.btnRewind.Font = New System.Drawing.Font("Webdings", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.btnRewind.Location = New System.Drawing.Point(3, 3)
        Me.btnRewind.Name = "btnRewind"
        Me.btnRewind.Size = New System.Drawing.Size(23, 23)
        Me.btnRewind.TabIndex = 2
        Me.btnRewind.Text = "7"
        Me.btnRewind.UseVisualStyleBackColor = True
        '
        'trcTime
        '
        Me.trcTime.AutoSize = False
        Me.trcTime.Location = New System.Drawing.Point(302, 3)
        Me.trcTime.Name = "trcTime"
        Me.trcTime.Size = New System.Drawing.Size(455, 32)
        Me.trcTime.TabIndex = 1
        '
        'MenuStrip
        '
        Me.MenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.itmFile, Me.itmEdit, Me.itmHelp})
        Me.MenuStrip.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip.Name = "MenuStrip"
        Me.MenuStrip.Size = New System.Drawing.Size(784, 24)
        Me.MenuStrip.TabIndex = 2
        Me.MenuStrip.Text = "Menu"
        '
        'itmFile
        '
        Me.itmFile.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.it1New, Me.it1Open, Me.ToolStripSeparator1, Me.it1Save, Me.it1SaveAs, Me.ToolStripSeparator2, Me.it1Quit})
        Me.itmFile.Name = "itmFile"
        Me.itmFile.Size = New System.Drawing.Size(37, 20)
        Me.itmFile.Text = "File"
        '
        'it1New
        '
        Me.it1New.Name = "it1New"
        Me.it1New.Size = New System.Drawing.Size(180, 22)
        Me.it1New.Text = "New"
        '
        'it1Open
        '
        Me.it1Open.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.it2OpenAudio, Me.it2OpenLyricsFIle})
        Me.it1Open.Name = "it1Open"
        Me.it1Open.Size = New System.Drawing.Size(180, 22)
        Me.it1Open.Text = "Open..."
        '
        'it2OpenAudio
        '
        Me.it2OpenAudio.Name = "it2OpenAudio"
        Me.it2OpenAudio.Size = New System.Drawing.Size(124, 22)
        Me.it2OpenAudio.Text = "Audio"
        '
        'it2OpenLyricsFIle
        '
        Me.it2OpenLyricsFIle.Name = "it2OpenLyricsFIle"
        Me.it2OpenLyricsFIle.Size = New System.Drawing.Size(124, 22)
        Me.it2OpenLyricsFIle.Text = "Lyrics File"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(177, 6)
        '
        'it1Save
        '
        Me.it1Save.Name = "it1Save"
        Me.it1Save.Size = New System.Drawing.Size(180, 22)
        Me.it1Save.Text = "Save"
        '
        'it1SaveAs
        '
        Me.it1SaveAs.Name = "it1SaveAs"
        Me.it1SaveAs.Size = New System.Drawing.Size(180, 22)
        Me.it1SaveAs.Text = "Save As..."
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(177, 6)
        '
        'it1Quit
        '
        Me.it1Quit.Name = "it1Quit"
        Me.it1Quit.Size = New System.Drawing.Size(180, 22)
        Me.it1Quit.Text = "Quit"
        '
        'itmEdit
        '
        Me.itmEdit.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AddMultipleLinesToolStripMenuItem, Me.ToolStripSeparator4, Me.LineEditingToolStripMenuItem})
        Me.itmEdit.Name = "itmEdit"
        Me.itmEdit.Size = New System.Drawing.Size(39, 20)
        Me.itmEdit.Text = "Edit"
        '
        'AddMultipleLinesToolStripMenuItem
        '
        Me.AddMultipleLinesToolStripMenuItem.Name = "AddMultipleLinesToolStripMenuItem"
        Me.AddMultipleLinesToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.AddMultipleLinesToolStripMenuItem.Text = "Add multiple lines..."
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        Me.ToolStripSeparator4.Size = New System.Drawing.Size(177, 6)
        '
        'LineEditingToolStripMenuItem
        '
        Me.LineEditingToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.it2InsertLine, Me.it2RemoveLine, Me.ToolStripSeparator5, Me.it2Optimize})
        Me.LineEditingToolStripMenuItem.Name = "LineEditingToolStripMenuItem"
        Me.LineEditingToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.LineEditingToolStripMenuItem.Text = "Line Editing"
        '
        'it2InsertLine
        '
        Me.it2InsertLine.Name = "it2InsertLine"
        Me.it2InsertLine.Size = New System.Drawing.Size(142, 22)
        Me.it2InsertLine.Text = "Insert Line"
        '
        'it2RemoveLine
        '
        Me.it2RemoveLine.Name = "it2RemoveLine"
        Me.it2RemoveLine.Size = New System.Drawing.Size(142, 22)
        Me.it2RemoveLine.Text = "Remove Line"
        '
        'ToolStripSeparator5
        '
        Me.ToolStripSeparator5.Name = "ToolStripSeparator5"
        Me.ToolStripSeparator5.Size = New System.Drawing.Size(139, 6)
        '
        'it2Optimize
        '
        Me.it2Optimize.Name = "it2Optimize"
        Me.it2Optimize.Size = New System.Drawing.Size(142, 22)
        Me.it2Optimize.Text = "Optimize"
        '
        'itmHelp
        '
        Me.itmHelp.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ShowDebugWindowToolStripMenuItem, Me.ToolStripSeparator3, Me.AboutTiLyricsStudioToolStripMenuItem})
        Me.itmHelp.Name = "itmHelp"
        Me.itmHelp.Size = New System.Drawing.Size(44, 20)
        Me.itmHelp.Text = "Help"
        '
        'ShowDebugWindowToolStripMenuItem
        '
        Me.ShowDebugWindowToolStripMenuItem.Name = "ShowDebugWindowToolStripMenuItem"
        Me.ShowDebugWindowToolStripMenuItem.Size = New System.Drawing.Size(195, 22)
        Me.ShowDebugWindowToolStripMenuItem.Text = "Show Debug Window"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(192, 6)
        '
        'AboutTiLyricsStudioToolStripMenuItem
        '
        Me.AboutTiLyricsStudioToolStripMenuItem.Name = "AboutTiLyricsStudioToolStripMenuItem"
        Me.AboutTiLyricsStudioToolStripMenuItem.Size = New System.Drawing.Size(195, 22)
        Me.AboutTiLyricsStudioToolStripMenuItem.Text = "About ti: LyricsStudio..."
        '
        'pnlHidden
        '
        Me.pnlHidden.Controls.Add(Me.AxWindowsMediaPlayer)
        Me.pnlHidden.Location = New System.Drawing.Point(572, 27)
        Me.pnlHidden.Name = "pnlHidden"
        Me.pnlHidden.Size = New System.Drawing.Size(200, 458)
        Me.pnlHidden.TabIndex = 5
        Me.pnlHidden.Visible = False
        '
        'AxWindowsMediaPlayer
        '
        Me.AxWindowsMediaPlayer.Enabled = True
        Me.AxWindowsMediaPlayer.Location = New System.Drawing.Point(3, 3)
        Me.AxWindowsMediaPlayer.Name = "AxWindowsMediaPlayer"
        Me.AxWindowsMediaPlayer.OcxState = CType(resources.GetObject("AxWindowsMediaPlayer.OcxState"), System.Windows.Forms.AxHost.State)
        Me.AxWindowsMediaPlayer.Size = New System.Drawing.Size(194, 45)
        Me.AxWindowsMediaPlayer.TabIndex = 0
        '
        'OpenFileDialog
        '
        Me.OpenFileDialog.FileName = "OpenFileDialog"
        '
        'CtlUpdTimer
        '
        Me.CtlUpdTimer.Enabled = True
        Me.CtlUpdTimer.Interval = 1
        '
        'DataGridView
        '
        Me.DataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.DataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView.Location = New System.Drawing.Point(12, 36)
        Me.DataGridView.Name = "DataGridView"
        Me.DataGridView.Size = New System.Drawing.Size(554, 446)
        Me.DataGridView.TabIndex = 6
        '
        'LPreviewTimer
        '
        Me.LPreviewTimer.Enabled = True
        '
        'MainWindow
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(784, 561)
        Me.Controls.Add(Me.DataGridView)
        Me.Controls.Add(Me.pnlHidden)
        Me.Controls.Add(Me.pnlController)
        Me.Controls.Add(Me.MenuStrip)
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "MainWindow"
        Me.Text = "MainWindow"
        Me.pnlController.ResumeLayout(False)
        Me.pnlController.PerformLayout()
        CType(Me.trcTime, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MenuStrip.ResumeLayout(False)
        Me.MenuStrip.PerformLayout()
        Me.pnlHidden.ResumeLayout(False)
        CType(Me.AxWindowsMediaPlayer, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents pnlController As Panel
    Friend WithEvents trcTime As TrackBar
    Friend WithEvents btnFF As Button
    Friend WithEvents btnStop As Button
    Friend WithEvents btnPlayPause As Button
    Friend WithEvents btnRewind As Button
    Friend WithEvents lblTime As Label
    Friend WithEvents lblPreview As Label
    Friend WithEvents MenuStrip As MenuStrip
    Friend WithEvents itmFile As ToolStripMenuItem
    Friend WithEvents it1Quit As ToolStripMenuItem
    Friend WithEvents btnSetTime As Button
    Friend WithEvents itmEdit As ToolStripMenuItem
    Friend WithEvents itmHelp As ToolStripMenuItem
    Friend WithEvents it1New As ToolStripMenuItem
    Friend WithEvents it1Open As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents it1Save As ToolStripMenuItem
    Friend WithEvents it1SaveAs As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator2 As ToolStripSeparator
    Friend WithEvents pnlHidden As Panel
    Friend WithEvents AxWindowsMediaPlayer As AxWMPLib.AxWindowsMediaPlayer
    Friend WithEvents ShowDebugWindowToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents it2OpenAudio As ToolStripMenuItem
    Friend WithEvents OpenFileDialog As OpenFileDialog
    Friend WithEvents CtlUpdTimer As Timer
    Friend WithEvents AddMultipleLinesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SaveFileDialog As SaveFileDialog
    Friend WithEvents LineEditingToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents it2InsertLine As ToolStripMenuItem
    Friend WithEvents it2RemoveLine As ToolStripMenuItem
    Friend WithEvents it2OpenLyricsFIle As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator3 As ToolStripSeparator
    Friend WithEvents AboutTiLyricsStudioToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator4 As ToolStripSeparator
    Friend WithEvents DataGridView As DataGridView
    Friend WithEvents LPreviewTimer As Timer
    Friend WithEvents ToolStripSeparator5 As ToolStripSeparator
    Friend WithEvents it2Optimize As ToolStripMenuItem
End Class
