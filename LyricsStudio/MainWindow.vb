Imports System.ComponentModel
Imports AxWMPLib

Public Class MainWindow
    Dim AppTitle As String = "ti: LyricsStudio Alpha " _
        & My.Application.Info.Version.Major & "." & My.Application.Info.Version.Minor _
        & " r" & My.Application.Info.Version.MinorRevision
    Dim IsDirty As Boolean = False
    Dim FileInfo As New FileInfo(False, False, "%HOMEDRIVE%\Music", "New File", vbNullString)
    Friend CData As List(Of LyricsData)
    Friend TData As List(Of LyricsData) = New List(Of LyricsData)
    Friend AudioLength, PlayTime As String
    Dim SecondThread As System.Threading.Thread = New Threading.Thread(AddressOf TimeCheckingWork)

    'User Functions
    Private Sub FileInfoManage(ByVal Behavior As String)
        Try
            Select Case (Behavior)
                Case "LoadA"
                    'Initialize Windows Media Player Component
                    AxWindowsMediaPlayer.URL = FileInfo.Location & "\" & FileInfo.FileName & FileInfo.Extension
                    AxWindowsMediaPlayer.Ctlcontrols.stop()
                    'Mark audio as opened
                    FileInfo.AudioLoaded = True
                    PlayTime = "00:00.00"
                    If FileInfo.LyricsLoaded = False Then
                        If System.IO.File.Exists(FileInfo.Location & "\" & FileInfo.FileName & ".lrc") Then
                            FileInfoManage("LoadL") 'Call myself as LoadL
                        Else
                            FileInfoManage("New") 'Call mysself as New
                        End If
                    End If
                    Exit Select
                Case "LoadL"
                    CData = New List(Of LyricsData) 'Reset CData
                    'Read File Line by Line
                    Dim FileO As New System.IO.StreamReader(FileInfo.Location & "\" & FileInfo.FileName & ".lrc")
                    Dim ReadLine As String
                    Dim Regex As New System.Text.RegularExpressions.Regex("\[(.*?)\]")
                    Do While FileO.Peek() <> -1
                        ReadLine = FileO.ReadLine()
                        Dim SplitT() As String = ReadLine.Split("]")
                        CData.Add(New LyricsData(Regex.Match(ReadLine).Value.Substring(1, Regex.Match(ReadLine).Value.Length - 2), SplitT(1).Trim()))
                    Loop
                    CData.Add(New LyricsData(vbNullString, vbNullString))
                    DataGridView.DataSource = TData
                    DataGridView.DataSource = CData
                    FileInfo.LyricsLoaded = True
                    If System.IO.File.Exists(FileInfo.Location & "\" & FileInfo.FileName & ".mp3") Then
                        FileInfo.Extension = ".mp3"
                        FileInfoManage("LoadA") 'Call myself as LoadA
                    ElseIf System.IO.File.Exists(FileInfo.Location & "\" & FileInfo.FileName & ".wav") Then
                        FileInfo.Extension = ".wav"
                        FileInfoManage("LoadA") 'Call myself as LoadA
                    End If
                    Exit Select
                Case "New"
                    'Initialize Workspace
                    CData = New List(Of LyricsData) 'Reset CData
                    CData.Add(New LyricsData(vbNullString, vbNullString))
                    DataGridView.DataSource = CData 'Set DataGridView's source to CData
                    Exit Select
                Case "Save"
                    Dim FileW As New System.IO.StreamWriter(FileInfo.Location & "\" & FileInfo.FileName & ".lrc")
                    For i As Integer = 1 To CData.Count - 1
                        If Not String.IsNullOrWhiteSpace(CData.Item(i - 1).Time) And Not String.IsNullOrWhiteSpace(CData.Item(i - 1).Lyric) Then
                            FileW.Write("[" & CData.Item(i - 1).Time & "]" & CData.Item(i - 1).Lyric & vbNewLine)
                        End If
                    Next i
                    FileW.Close()
                    IsDirty = False
                    Exit Select
            End Select
        Catch ex As Exception
            DebugWindow.AddDLine("Exception Thrown while working with file(s): " + ex.ToString, 2)
        End Try
    End Sub

    'Form Function
    Private Sub MainWindow_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If SecondThread.IsAlive = True Then
            SecondThread.Abort()
        End If
    End Sub

    Private Sub MainWindow_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'Initialize Form Settings
        Text = AppTitle
        If SecondThread.IsAlive = False Then
            SecondThread.Start()
        End If
        MainWindow_Resize(sender, e) 'Resize all components
    End Sub

    Private Sub MainWindow_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        'Resize DataGridView
        DataGridView.Height = Height - 155
        DataGridView.Width = Width - 40
        'Move pnlController
        pnlController.Top = Height - 112
        'Resize pnlController
        pnlController.Width = Width - 40
        'Resize trcTime
        trcTime.Width = pnlController.Width - 305
        'Resize lblPreview
        lblPreview.Width = pnlController.Width - 4
    End Sub

    Private Sub AxWindowsMediaPlayer_PlayStateChange(sender As Object, e As _WMPOCXEvents_PlayStateChangeEvent) Handles AxWindowsMediaPlayer.PlayStateChange
        If AxWindowsMediaPlayer.playState = WMPLib.WMPPlayState.wmppsPlaying Then
            btnPlayPause.Text = ";"
        Else
            btnPlayPause.Text = "4"
        End If
    End Sub

    Private Sub btnFF_Click(sender As Object, e As EventArgs) Handles btnFF.Click
        If AxWindowsMediaPlayer.currentMedia.duration - AxWindowsMediaPlayer.Ctlcontrols.currentPosition > 5 Then
            AxWindowsMediaPlayer.Ctlcontrols.currentPosition = AxWindowsMediaPlayer.Ctlcontrols.currentPosition + 5
        End If
    End Sub

    Private Sub btnPlayPause_Click(sender As Object, e As EventArgs) Handles btnPlayPause.Click
        If FileInfo.AudioLoaded = True Then
            'Play/Pause Event
            If AxWindowsMediaPlayer.playState = WMPLib.WMPPlayState.wmppsPlaying Then
                AxWindowsMediaPlayer.Ctlcontrols.pause()
            Else
                AxWindowsMediaPlayer.Ctlcontrols.play()
            End If
        Else
        End If
    End Sub

    Private Sub btnRewind_Click(sender As Object, e As EventArgs) Handles btnRewind.Click
        If AxWindowsMediaPlayer.Ctlcontrols.currentPosition > 5 Then
            AxWindowsMediaPlayer.Ctlcontrols.currentPosition = AxWindowsMediaPlayer.Ctlcontrols.currentPosition - 5
        End If
    End Sub

    Private Sub btnSetTime_Click(sender As Object, e As EventArgs) Handles btnSetTime.Click
        CData.Item(DataGridView.CurrentRow.Index).Time = PlayTime
        DataGridView.Refresh()
        DataGridView.CurrentCell = DataGridView.Item(0, DataGridView.CurrentCellAddress.Y + 1)
        DebugWindow.AddDLine("Current Lyrics Database size: " + CData.Count.ToString)
        IsDirty = True
    End Sub

    Private Sub btnStop_Click(sender As Object, e As EventArgs) Handles btnStop.Click
        AxWindowsMediaPlayer.Ctlcontrols.stop()
    End Sub

    Public Sub DataGridView_AddLine(ByVal Time As String, ByVal Text As String)
        CData.Add(New LyricsData(Time, Text))
        DataGridView.DataSource = TData
        DataGridView.DataSource = CData
        IsDirty = True
    End Sub

    Private Sub DataGridView_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView.CellValueChanged
        If CData.Item(CData.Count - 1).Time <> vbNullString Or CData.Item(CData.Count - 1).Lyric <> vbNullString Then
            CData.Add(New LyricsData(vbNullString, vbNullString))
            DataGridView.DataSource = TData
            DataGridView.DataSource = CData
        Else
            While String.IsNullOrWhiteSpace(CData.Item(CData.Count - 1).Time) And String.IsNullOrWhiteSpace(CData.Item(CData.Count - 1).Lyric)
                CData.Remove(CData.Item(CData.Count - 1))
            End While
            CData.Add(New LyricsData(vbNullString, vbNullString))
            DataGridView.DataSource = TData
            DataGridView.DataSource = CData
        End If
        If IsDirty = False Then
            IsDirty = True
        End If
    End Sub

    Private Sub it1AddMultipleLines_Click(sender As Object, e As EventArgs) Handles AddMultipleLinesToolStripMenuItem.Click
        If Not IsNothing(CData) Then
            AddMultipleLineWindow.ShowDialog()
        End If
    End Sub

    Private Sub it1New_Click(sender As Object, e As EventArgs) Handles it1New.Click
        'Fill WorkFiles variable
        FileInfo.Location = "%HOMEDRIVE%\Music"
        FileInfo.FileName = "New File"
        FileInfoManage("New") 'Call FileInfoManage() as New
    End Sub

    Private Sub it1Save_Click(sender As Object, e As EventArgs, Optional Recalled As Boolean = 0) Handles it1Save.Click
        Try
            If Not IsNothing(CData) Then
                If System.IO.File.Exists(FileInfo.Location & "\" & FileInfo.FileName & ".lrc") Then
                    FileInfoManage("Save") 'Call FileInfoManage() as Save
                ElseIf Recalled = True And Not System.IO.File.Exists(FileInfo.Location & "\" & FileInfo.FileName & ".lrc") Then
                    Throw New IO.IOException("Failed to create file " & FileInfo.Location & "\" & FileInfo.FileName & ".lrc")
                Else
                    System.IO.File.Create(FileInfo.Location & "\" & FileInfo.FileName & ".lrc").Dispose()
                    it1Save_Click(sender, e, True)
                End If
            ElseIf Not IsNothing(CData) Then
                If Not IsNothing(FileInfo.Location & "\" & FileInfo.FileName & FileInfo.FileName) Then
                    it1Save_Click(sender, e, True)
                Else
                    If Recalled = False Then
                        it1SaveAs_Click(sender, e)
                    Else
                        Throw New ArgumentException("File save requested without proper filename.")
                    End If
                End If
            End If
        Catch ex As Exception
            DebugWindow.AddDLine("Exception Thrown while trying to save lyrics file: " + ex.ToString, 2)
        End Try
    End Sub

    Private Sub it1SaveAs_Click(sender As Object, e As EventArgs) Handles it1SaveAs.Click
        With SaveFileDialog
            .FileName = FileInfo.FileName
            .InitialDirectory = FileInfo.Location
            .Filter = "LRC Lyrics File (*.lrc)|*.lrc|All Files (*.*)|*.*"
            .RestoreDirectory = True
            If .ShowDialog = DialogResult.OK Then
                it1Save_Click(sender, e, False)
            End If
        End With
    End Sub

    Private Sub it1ShowDebugWindow_Click(sender As Object, e As EventArgs) Handles ShowDebugWindowToolStripMenuItem.Click
        DebugWindow.Show()
    End Sub

    Private Sub it2InsertLine_Click(sender As Object, e As EventArgs) Handles it2InsertLine.Click
        If Not IsNothing(CData) Then
            DataGridView_AddLine(vbNullString, vbNullString)
            For i As Integer = CData.Count - 2 To DataGridView.CurrentRow.Index Step -1
                If i > 0 Then
                    CData.Item(i + 1).Time = CData.Item(i).Time
                    DebugWindow.AddDLine("CData.Item(" & i + 1 & ").Time = " & CData.Item(i + 1).Time)
                    CData.Item(i + 1).Lyric = CData.Item(i).Lyric
                    DebugWindow.AddDLine("CData.Item(" & i + 1 & ").Lyric = " & CData.Item(i + 1).Lyric)
                End If
            Next i
            CData.Item(DataGridView.CurrentRow.Index).Time = vbNullString
            CData.Item(DataGridView.CurrentRow.Index).Lyric = vbNullString
        End If
    End Sub

    Private Sub it2OpenAudio_Click(sender As Object, e As EventArgs) Handles it2OpenAudio.Click
        With OpenFileDialog
            .FileName = FileInfo.Location & "\" & FileInfo.FileName & FileInfo.Extension
            .Filter = "MPEG Audio Layer III (*.mp3)|*.mp3|Waveform Audio File Format (*.wav)|*.wav|All Files (*.*)|*.*"
            .InitialDirectory = FileInfo.Location
            .RestoreDirectory = True
            If .ShowDialog() = DialogResult.OK Then
                'Fill WorkFiles variable
                FileInfo.Location = System.IO.Path.GetDirectoryName(.FileName)
                FileInfo.FileName = System.IO.Path.GetFileNameWithoutExtension(.FileName)
                FileInfo.Extension = System.IO.Path.GetExtension(.FileName)
                FileInfoManage("LoadA") 'Call FileInfoManage() with LoadA
            End If
        End With
    End Sub

    Private Sub it2OpenLyricsFile_Click(sender As Object, e As EventArgs) Handles it2OpenLyricsFIle.Click
        With OpenFileDialog
            .FileName = FileInfo.Location & "\" & FileInfo.FileName & ".lrc"
            .Filter = "LRC Lyrics File (*.lrc)|*.lrc|All Files (*.*)|*.*"
            .InitialDirectory = FileInfo.Location
            .RestoreDirectory = True
            If .ShowDialog() = DialogResult.OK Then
                'Fill WorkFiles variable
                FileInfo.Location = System.IO.Path.GetDirectoryName(.FileName)
                FileInfo.FileName = System.IO.Path.GetFileNameWithoutExtension(.FileName)
                FileInfoManage("LoadL") 'Call FileInfoManage() with LoadL
            End If
        End With
    End Sub

    Private Sub it2Optimize_Click(sender As Object, e As EventArgs) Handles it2Optimize.Click

    End Sub

    Private Sub it2RemoveLine_Click(sender As Object, e As EventArgs) Handles it2RemoveLine.Click

    End Sub

    Private Sub LPreviewTimer_Tick(sender As Object, e As EventArgs) Handles LPreviewTimer.Tick
        Dim TCurT, TNextT As Double
        Dim TempStr(2) As String
        Try
            If Not (IsNothing(CData)) And AxWindowsMediaPlayer.playState = WMPLib.WMPPlayState.wmppsPlaying Then
                For i As Integer = 1 To CData.Count - 1
                    If Not String.IsNullOrWhiteSpace(CData.Item(i - 1).Time) Then
                        'Get current lyric's time as position
                        TempStr = CData.Item(i - 1).Time.Split({Chr(58), Chr(46)})
                        TCurT = ((((Convert.ToDouble(TempStr(0)) * 60) + Convert.ToDouble(TempStr(1))) * 100) + Convert.ToDouble(TempStr(2))) / 100
                        If i < CData.Count - 1 And Not String.IsNullOrWhiteSpace(CData.Item(i).Time) Then
                            'Get next lyric's time as position
                            TempStr = CData.Item(i).Time.Split({Chr(58), Chr(46)})
                            TNextT = ((((Convert.ToDouble(TempStr(0)) * 60) + Convert.ToDouble(TempStr(1))) * 100) + Convert.ToDouble(TempStr(2))) / 100
                        ElseIf i < CData.Count - 2 Then
                            If String.IsNullOrWhiteSpace(CData.Item(i).Time) Then
                                For j As Integer = i To CData.Count - 1
                                    If Not String.IsNullOrWhiteSpace(CData.Item(j).Time) Then
                                        'Get next lyric's time as position
                                        TempStr = CData.Item(j).Time.Split({Chr(58), Chr(46)})
                                        TNextT = ((((Convert.ToDouble(TempStr(0)) * 60) + Convert.ToDouble(TempStr(1))) * 100) + Convert.ToDouble(TempStr(2))) / 100
                                        Exit For
                                    End If
                                Next j
                            End If
                        End If
                        If (i < CData.Count - 1 And AxWindowsMediaPlayer.Ctlcontrols.currentPosition > TCurT And AxWindowsMediaPlayer.Ctlcontrols.currentPosition < TNextT) Or (i = CData.Count - 1 And AxWindowsMediaPlayer.Ctlcontrols.currentPosition > TCurT) Then
                            lblPreview.Text = CData.Item(i - 1).Lyric
                        End If
                    End If
                Next i
            Else
                lblPreview.Text = "Lyrics Preview will be shown here."
            End If
        Catch ex As Exception
            If Not IsNothing(Me) And Not IsNothing(DebugWindow) Then
            Try
                DebugWindow.AddDLine("Thread thrown exception-type message: " & ex.ToString, 2)
            Catch Ignore As Exception
            End Try
        End If
        End Try
    End Sub

    Private Sub RefreshForm() Handles CtlUpdTimer.Tick
        Try
            If FileInfo.AudioLoaded = True Then
                'Set song's length
                AudioLength = (Math.Truncate(Convert.ToDouble(AxWindowsMediaPlayer.currentMedia.duration) / 60)).ToString("0#") + ":" +
                               (Math.Truncate(AxWindowsMediaPlayer.currentMedia.duration) Mod 60).ToString("0#") + "." +
                               (Math.Truncate(AxWindowsMediaPlayer.currentMedia.duration * 100) Mod 100).ToString("0#")
                trcTime.Maximum = Math.Truncate(AxWindowsMediaPlayer.currentMedia.duration * 100)
                btnRewind.Enabled = True
                btnPlayPause.Enabled = True
                btnStop.Enabled = True
                btnFF.Enabled = True
                btnSetTime.Enabled = True
                lblTime.Text = PlayTime + "/" + AudioLength
                If IsDirty = True Then
                    Text = AppTitle & " - " & FileInfo.Location & "\" & FileInfo.FileName & ".lrc*"
                Else
                    Text = AppTitle & " - " & FileInfo.Location & "\" & FileInfo.FileName & ".lrc"
                End If
                If trcTime.Maximum > 0 Then
                    trcTime.Value = Math.Truncate(AxWindowsMediaPlayer.Ctlcontrols.currentPosition * 100)
                End If
            Else
                AudioLength = "00:00.00"
                btnRewind.Enabled = False
                btnPlayPause.Enabled = False
                btnStop.Enabled = False
                btnFF.Enabled = False
                btnSetTime.Enabled = False
                PlayTime = "00:00.00"
                Text = AppTitle
            End If
        Catch ex As Exception
            If Not IsNothing(Me) And Not IsNothing(DebugWindow) Then
                Try
                    DebugWindow.AddDLine("Thread thrown exception-type message: " & ex.ToString, 2)
                Catch Ignore As Exception
                End Try
            End If
        End Try
    End Sub

    Private Delegate Sub TimeCheckingAppliesInvoker(ByVal ReturnText As String)
    Public Sub TimeCheckingApplies(ByVal ReturnText As String)
        lblPreview.Text = ReturnText
    End Sub

    Sub TimeCheckingWork()
        Dim DebugWriteInvoker As ThreadDebugWriteInvoker
        DebugWriteInvoker = AddressOf ThreadDebugWrite
        Dim iTimeCheckingApplies As TimeCheckingAppliesInvoker
        iTimeCheckingApplies = AddressOf TimeCheckingApplies
        While True
            Try
                If FileInfo.AudioLoaded = True Then
                    PlayTime = (Math.Floor(Convert.ToDouble(AxWindowsMediaPlayer.Ctlcontrols.currentPosition) / 60)).ToString("0#") + ":" +
                        (Math.Floor(AxWindowsMediaPlayer.Ctlcontrols.currentPosition) Mod 60).ToString("0#") + "." +
                        (Math.Floor(AxWindowsMediaPlayer.Ctlcontrols.currentPosition * 100) Mod 100).ToString("0#")
                End If
                Threading.Thread.Sleep(10)
            Catch ex As Exception
                If Not IsNothing(Me) And Not IsNothing(DebugWindow) Then
                    Try
                        Invoke(DebugWriteInvoker, {"Exception", ex.ToString})
                    Catch Ignore As Exception
                    End Try
                End If
            End Try
        End While
    End Sub

    Private Delegate Sub ThreadDebugWriteInvoker(ByVal Type As String, ByVal Message As String)
    Public Sub ThreadDebugWrite(ByVal Type As String, ByVal Message As String)
        If Type = "Exception" Then
            DebugWindow.AddDLine("Thread thrown exception-type message: " & Message, 2)
        ElseIf Type = "Message" Then
            DebugWindow.AddDLine("Thread output: " & Message)
        End If
    End Sub

    Private Sub itmHelp_Click(sender As Object, e As EventArgs) Handles itmHelp.Click

    End Sub

    Private Sub trcTime_Scroll(sender As Object, e As EventArgs) Handles trcTime.Scroll
        AxWindowsMediaPlayer.Ctlcontrols.currentPosition = trcTime.Value / 100
    End Sub
End Class