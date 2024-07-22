<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DebugWindow
    Inherits System.Windows.Forms.Form

    'Form은 Dispose를 재정의하여 구성 요소 목록을 정리합니다.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows Form 디자이너에 필요합니다.
    Private components As System.ComponentModel.IContainer

    '참고: 다음 프로시저는 Windows Form 디자이너에 필요합니다.
    '수정하려면 Windows Form 디자이너를 사용하십시오.  
    '코드 편집기에서는 수정하지 마세요.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.RichDebug = New System.Windows.Forms.RichTextBox()
        Me.SuspendLayout()
        '
        'RichDebug
        '
        Me.RichDebug.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.RichDebug.Location = New System.Drawing.Point(0, 0)
        Me.RichDebug.Name = "RichDebug"
        Me.RichDebug.ReadOnly = True
        Me.RichDebug.Size = New System.Drawing.Size(385, 360)
        Me.RichDebug.TabIndex = 0
        Me.RichDebug.Text = ""
        '
        'DebugWindow
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(384, 361)
        Me.Controls.Add(Me.RichDebug)
        Me.MaximizeBox = False
        Me.Name = "DebugWindow"
        Me.Text = "DebugWindow"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents RichDebug As RichTextBox
End Class
