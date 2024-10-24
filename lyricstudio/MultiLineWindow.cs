using System;
using System.ComponentModel;

namespace ti_Lyricstudio
{

    public partial class MultiLineWindow
    {
        public MultiLineWindow()
        {
            InitializeComponent();
        }
        private void AddMultipleLineWindow_Load(object sender, EventArgs e)
        {
            Width = MinimumSize.Width;
            Height = MinimumSize.Height;
        }

        private void AddMultipleLineWindow_Resize(object sender, EventArgs e)
        {
            LineInput.Width = Width - 40;
            LineInput.Height = Height - 95;
            AddButton.Top = Height - (AddButton.Height + 51);
            AddButton.Left = Width - (AddButton.Width + 27);
            _CancelButton.Top = Height - (_CancelButton.Height + 51);
            _CancelButton.Left = AddButton.Left - (_CancelButton.Width + 6);
        }

        private void _CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void AddMultipleLineWindow_Closing(object sender, CancelEventArgs e)
        {
            LineInput.Text = String.Empty;
        }
    }
}