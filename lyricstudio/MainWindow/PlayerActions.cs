using System;
using System.Windows.Forms;
using ti_Lyricstudio.Class;

namespace ti_Lyricstudio
{
    public partial class MainWindow
    {
        // action when player started playing audio
        private void Player_Playing(object sender, EventArgs e)
        {
            // synchronise the stopwatch
            sw.Offset = new((long)(player.Position * audioDuration * 10000));
            // start or resume the stopwatch
            sw.Restart();

            PlayerGroup.Invoke((MethodInvoker)delegate
            {
                // block edit of EditorView
                EditorView.EditMode = DataGridViewEditMode.EditProgrammatically;

                // enable all player control button
                btnStop.Enabled = true;
                btnPrev.Enabled = true;
                btnNext.Enabled = true;
                btnSetTime.Enabled = true;

                // enable the TimeBar
                TimeBar.Enabled = true;
                // update label to pause symbol and play
                btnPlayPause.Text = ";";
            });
        }

        // action when player paused playing audio
        private void Player_Paused(object sender, EventArgs e)
        {
            // pause the stopwatch
            sw.Stop();

            PlayerGroup.Invoke((MethodInvoker)delegate
            {
                // update label to play symbol and pause
                btnPlayPause.Text = "4";
                // allow edit of EditorView
                EditorView.EditMode = DataGridViewEditMode.EditOnKeystroke;
            });
        }

        // action when player stopped playing audio
        private void Player_Stopped(object sender, EventArgs e)
        {
            // stop and reset the stopwatch
            sw.Reset();
            sw.Offset = TimeSpan.Zero;

            PlayerGroup.Invoke((MethodInvoker)delegate
            {
                // allow edit of EditorView
                EditorView.EditMode = DataGridViewEditMode.EditOnKeystroke;

                // disable all player control button except play button
                btnStop.Enabled = false;
                btnPrev.Enabled = false;
                btnNext.Enabled = false;
                btnSetTime.Enabled = false;

                // set label of btnPlayPause to play symbol
                btnPlayPause.Text = "4";
                // reset label of TimeLabel
                TimeLabel.Text = $"00:00.00 / {LyricTime.From(audioDuration)}";
                // reset value of TimeBar
                TimeBar.Value = 0;
                // disable the TimeBar
                TimeBar.Enabled = false;
            });
        }
    }
}
