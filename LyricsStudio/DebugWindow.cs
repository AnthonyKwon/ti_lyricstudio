using System;
using Microsoft.VisualBasic;

namespace ti_Lyricstudio
{
    public partial class DebugWindow
    {
        public DebugWindow()
        {
            InitializeComponent();
        }
        public void AddDLine(string Text, int Grade = 0)
        {
            // Grade    0 = Information, 1 = Warning, 2 = Error
            if (Grade == 2)
            {
                RichDebug.Text += "E ";
            }
            else if (Grade == 1)
            {
                RichDebug.Text += "W ";
            }
            else
            {
                RichDebug.Text += "I ";
            }
            RichDebug.Text += Text + Constants.vbNewLine;
        }

        private void DebugWindow_Resize(object sender, EventArgs e)
        {
            RichDebug.Width = Width;
            RichDebug.Height = Height;
        }
    }
}