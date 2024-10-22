using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ti_Lyricstudio
{
    [Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    public partial class MultiLineWindow : Form
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
            LineInput = new TextBox();
            AddButton = new Button();
            AddButton.Click += new EventHandler(AddButton_Click);
            _CancelButton = new Button();
            _CancelButton.Click += new EventHandler(_CancelButton_Click);
            SuspendLayout();
            // 
            // LineInput
            // 
            LineInput.Location = new Point(12, 12);
            LineInput.Multiline = true;
            LineInput.Name = "LineInput";
            LineInput.ScrollBars = ScrollBars.Both;
            LineInput.Size = new Size(440, 225);
            LineInput.TabIndex = 0;
            // 
            // AddButton
            // 
            AddButton.Location = new Point(377, 246);
            AddButton.Name = "AddButton";
            AddButton.Size = new Size(75, 23);
            AddButton.TabIndex = 1;
            AddButton.Text = "Add Lines";
            AddButton.UseVisualStyleBackColor = true;
            // 
            // _CancelButton
            // 
            _CancelButton.Location = new Point(296, 246);
            _CancelButton.Name = "_CancelButton";
            _CancelButton.Size = new Size(75, 23);
            _CancelButton.TabIndex = 2;
            _CancelButton.Text = "Cancel";
            _CancelButton.UseVisualStyleBackColor = true;
            // 
            // AddMultipleLineWindow
            // 
            AutoScaleDimensions = new SizeF(6.0f, 13.0f);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(464, 281);
            Controls.Add(_CancelButton);
            Controls.Add(AddButton);
            Controls.Add(LineInput);
            MaximizeBox = false;
            MaximumSize = new Size(640, 480);
            MinimizeBox = false;
            MinimumSize = new Size(480, 320);
            Name = "AddMultipleLineWindow";
            Text = "Add Multiple Line...";
            Load += new EventHandler(AddMultipleLineWindow_Load);
            Resize += new EventHandler(AddMultipleLineWindow_Resize);
            Closing += new System.ComponentModel.CancelEventHandler(AddMultipleLineWindow_Closing);
            ResumeLayout(false);
            PerformLayout();

        }

        internal TextBox LineInput;
        internal Button AddButton;
        internal Button _CancelButton;
    }
}