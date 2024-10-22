using System;
using System.Collections.Generic;
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
        // thread job queue
        private List<String> threadJob = [];
        // mark to control running status of thread
        private bool running = true;

        public void PlayerTimeThreadF()
        {
            // repeat while player is available
            while (running == true && player != null)
            {
                // run enqueued thread job
                if (threadJob.Count > 0)
                {
                    Console.WriteLine(threadJob[0]);
                    switch (threadJob[0])
                    {
                        case "startStopwatch":
                            // wait for VLC to start playing
                            if (player.IsPlaying != true) break;
                            // synchronise the stopwatch
                            sw.Offset = new((long)player.Position * audioDuration);
                            // start or resume the stopwatch
                            sw.Start();
                            // remove this job from queue
                            threadJob.RemoveAt(0);
                            break;
                        case "pauseStopwatch":
                            // wait for VLC to pause
                            if (player.IsPlaying == true) break;
                            // pause the stopwatch
                            sw.Stop();
                            // remove this job from queue
                            threadJob.RemoveAt(0);
                            break;
                        case "stopStopwatch":
                            // wait for VLC to pause
                            if (player.IsPlaying == true) break;
                            // stop and reset the stopwatch
                            sw.Reset();
                            sw.Offset = TimeSpan.Zero;
                            // remove this job from queue
                            threadJob.RemoveAt(0);
                            break;
                        case "offsetStopwatch":
                            // save the playing state
                            bool swRunning = sw.IsRunning;
                            // reset the stopwatch
                            sw.Reset();
                            // check if VLC is playing audio (will do nothing when stopped)
                            if (player.IsPlaying == true)
                            {
                                // playing, get value from player
                                sw.Offset = new((long)(player.Position * audioDuration * 10000));
                            }
                            else if (player.IsPlaying == false && player.Length != -1)
                            {
                                // paused, get value from TimeBar
                                sw.Offset = new(TimeBar.Value * 10000);
                            }
                            // restart the stopwatch if it was playing
                            if (swRunning) sw.Start();
                            // remove this job from queue
                            threadJob.RemoveAt(0);
                            break;
                    }
                }

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

                        LyricTime currentTime = LyricTime.From((int)sw.Elapsed.TotalMilliseconds);
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
                        for (int i = 0; i < EditorView.Rows.Count - 1; i++)
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
                // player has paused
                else if (player.IsPlaying == false && player.Length != -1)
                {
                }
                // player has stopped
                else
                {
                }

            // sleep thread for 10 milliseconds
            Thread.Sleep(10);
            }
        }
    }
}
