<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AddMultipleLineWindow
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.LineInput = New System.Windows.Forms.TextBox()
        Me.AddButton = New System.Windows.Forms.Button()
        Me._CancelButton = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'LineInput
        '
        Me.LineInput.Location = New System.Drawing.Point(12, 12)
        Me.LineInput.Multiline = True
        Me.LineInput.Name = "LineInput"
        Me.LineInput.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.LineInput.Size = New System.Drawing.Size(440, 225)
        Me.LineInput.TabIndex = 0
        '
        'AddButton
        '
        Me.AddButton.Location = New System.Drawing.Point(377, 246)
        Me.AddButton.Name = "AddButton"
        Me.AddButton.Size = New System.Drawing.Size(75, 23)
        Me.AddButton.TabIndex = 1
        Me.AddButton.Text = "Add Lines"
        Me.AddButton.UseVisualStyleBackColor = True
        '
        '_CancelButton
        '
        Me._CancelButton.Location = New System.Drawing.Point(296, 246)
        Me._CancelButton.Name = "_CancelButton"
        Me._CancelButton.Size = New System.Drawing.Size(75, 23)
        Me._CancelButton.TabIndex = 2
        Me._CancelButton.Text = "Cancel"
        Me._CancelButton.UseVisualStyleBackColor = True
        '
        'AddMultipleLineWindow
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(464, 281)
        Me.Controls.Add(Me._CancelButton)
        Me.Controls.Add(Me.AddButton)
        Me.Controls.Add(Me.LineInput)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(640, 480)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(480, 320)
        Me.Name = "AddMultipleLineWindow"
        Me.Text = "Add Multiple Line..."
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents LineInput As TextBox
    Friend WithEvents AddButton As Button
    Friend WithEvents _CancelButton As Button
End Class
