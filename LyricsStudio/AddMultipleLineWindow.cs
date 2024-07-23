using System;
using System.ComponentModel;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace ti_Lyricstudio
{

    public partial class AddMultipleLineWindow
    {
        public AddMultipleLineWindow()
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
            foreach (var Lyric in LineInput.Text.Split(Conversions.ToChar(Constants.vbNewLine)))
                //My.MyProject.Forms.MainWindow.DataGridView_AddLine(Constants.vbNullString, Lyric.Replace(Constants.vbCr, Constants.vbNullString).Replace(Constants.vbLf, Constants.vbNullString));
            Close();
        }

        private void AddMultipleLineWindow_Closing(object sender, CancelEventArgs e)
        {
            LineInput.Text = Constants.vbNullString;
        }
    }
}