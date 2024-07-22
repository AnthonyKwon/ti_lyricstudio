using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ti_Lyricstudio
{
    [Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    public partial class DebugWindow : Form
    {

        // Form은 Dispose를 재정의하여 구성 요소 목록을 정리합니다.
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

        // Windows Form 디자이너에 필요합니다.
        private System.ComponentModel.IContainer components;

        // 참고: 다음 프로시저는 Windows Form 디자이너에 필요합니다.
        // 수정하려면 Windows Form 디자이너를 사용하십시오.  
        // 코드 편집기에서는 수정하지 마세요.
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            RichDebug = new RichTextBox();
            SuspendLayout();
            // 
            // RichDebug
            // 
            RichDebug.BorderStyle = BorderStyle.None;
            RichDebug.Location = new Point(0, 0);
            RichDebug.Name = "RichDebug";
            RichDebug.ReadOnly = true;
            RichDebug.Size = new Size(385, 360);
            RichDebug.TabIndex = 0;
            RichDebug.Text = "";
            // 
            // DebugWindow
            // 
            AutoScaleDimensions = new SizeF(96.0f, 96.0f);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(384, 361);
            Controls.Add(RichDebug);
            MaximizeBox = false;
            Name = "DebugWindow";
            Text = "DebugWindow";
            Resize += new EventHandler(DebugWindow_Resize);
            ResumeLayout(false);

        }

        internal RichTextBox RichDebug;
    }
}