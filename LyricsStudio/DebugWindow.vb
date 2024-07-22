Public Class DebugWindow
    Public Sub AddDLine(ByVal Text As String, Optional ByVal Grade As Integer = 0)
        'Grade    0 = Information, 1 = Warning, 2 = Error
        If Grade = 2 Then
            RichDebug.Text += "E "
        ElseIf Grade = 1 Then
            RichDebug.Text += "W "
        Else
            RichDebug.Text += "I "
        End If
        RichDebug.Text += Text + vbNewLine
    End Sub

    Private Sub DebugWindow_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        RichDebug.Width = Width
        RichDebug.Height = Height
    End Sub
End Class