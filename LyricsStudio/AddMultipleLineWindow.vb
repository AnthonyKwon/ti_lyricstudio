Imports System.ComponentModel

Public Class AddMultipleLineWindow
    Private Sub AddMultipleLineWindow_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Width = MinimumSize.Width
        Height = MinimumSize.Height
    End Sub

    Private Sub AddMultipleLineWindow_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        LineInput.Width = Width - 40
        LineInput.Height = Height - 95
        AddButton.Top = Height - (AddButton.Height + 51)
        AddButton.Left = Width - (AddButton.Width + 27)
        _CancelButton.Top = Height - (_CancelButton.Height + 51)
        _CancelButton.Left = AddButton.Left - (_CancelButton.Width + 6)
    End Sub

    Private Sub _CancelButton_Click(sender As Object, e As EventArgs) Handles _CancelButton.Click
        Close()
    End Sub

    Private Sub AddButton_Click(sender As Object, e As EventArgs) Handles AddButton.Click
        For Each Lyric In LineInput.Text.Split(vbNewLine)
            MainWindow.DataGridView_AddLine(vbNullString, Lyric.Replace(vbCr, vbNullString).Replace(vbLf, vbNullString))
        Next
        Close()
    End Sub

    Private Sub AddMultipleLineWindow_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        LineInput.Text = vbNullString
    End Sub
End Class