using System;
using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Threading;
using System.Windows.Forms;
using ti_Lyricstudio.Class;

namespace ti_Lyricstudio
{
    public partial class MainWindow
    {
        // new thread to update time
        private Thread PlayerTimeThread;
        // lock for modifying UI elements
        private object delegateLock = null;
        private object timebarLock = null;
        // mark to control running status of thread
        private bool running = true;

        public void PlayerTimeThreadF()
        {
            // repeat while player is available
            while (running == true && player != null)
            {
                // player is playing audio
                if (player.IsPlaying == true)
                {
                    // get current audio position of the player
                    int position = (int)(player.Position * audioDuration);
                    if (position < 0) position = 0;

                    try
                    {
                        // skip modifying UI elements if main thread is locked
                        if (delegateLock != null) continue;

                        LyricTime currentTime = LyricTime.From(position);
                        // set text of the time label to player audio duration information
                        TimeLabel.Invoke((MethodInvoker)delegate
                        {
                            TimeLabel.Text = $"{currentTime} / {LyricTime.From(audioDuration)}";
                        });

                        if (timebarLock == null)
                        {
                            // set value of the trackbar to audio position
                            TimeBar.Invoke((MethodInvoker)delegate
                            {
                                TimeBar.Value = position;
                            });
                        }

                        // search the lyrics time list
                        string time = "";
                        string lyric = "";
                        for (int i = 0; i < DataGridView.Rows.Count - 1; i++)
                        {
                            // marker to check if matching lyric has found
                            bool found = false;
                            for (int j = 0; j < lyrics[i].Time.Count; j++)
                            {
                                // compare current time and current target time
                                //Console.WriteLine($"i: {i}, j: {j}, compare: {LyricTime.Compare(currentTime, lyrics[i].Time[j])}");
                                if (LyricTime.Compare(currentTime, lyrics[i].Time[j]) != LyricTime.Comparator.RightIsBigger)
                                {
                                    time = lyrics[i].Time[j].ToString();
                                    lyric = lyrics[i].Text;
                                }
                                else
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (found == true) break;
                        }
                        // update preview by time and lyrics
                        PreviewLabel.Invoke((MethodInvoker)delegate
                        {
                            // show lyrics to preview
                            PreviewLabel.Text = $"{time}  {lyric}";
                        });
                    }
                    catch (Exception ex)
                    {
                        // thread tried to do something at main thread while got an stop request, this is supposed
                        if (running == false) return;
                        // now this is an exception
                        throw ex;
                    }
                }
                // player is paused. do nothing.
                else if (player.IsPlaying == false && player.Length != -1)
                { 
                }
                // player has stopped. do nothing.
                else
                {
                }

            // sleep thread for 10 milliseconds
            Thread.Sleep(10);
            }
        }
    }
}
