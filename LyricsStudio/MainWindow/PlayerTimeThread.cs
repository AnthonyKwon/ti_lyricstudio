using LibVLCSharp.Shared;
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
        private object threadUILock = null;
        private object timebarLock = null;
        // thread job queue
        private List<String> threadJob = [];
        // mark to control running status of thread
        private bool running = true;

        public void TimeThreadTask()
        {
            // permanent variables for job "syncStopwatch"
            bool isPlaying = false;
            long rawTicks;

            // repeat while player is available
            while (running == true && player != null)
            {
                try
                {
                    // run enqueued thread job
                    if (threadJob.Count > 0)
                    {
                        switch (threadJob[0])
                        {
                            case "syncStopwatch":
                                rawTicks = sw.Elapsed.Ticks - sw.Offset.Ticks;

                                // lock the timebar
                                if (timebarLock == null) timebarLock = this;

                                if (isPlaying == false && rawTicks != 0 && player.IsPlaying == true)
                                {
                                    // pause the player if playing
                                    isPlaying = true;
                                    player.Pause();
                                    break;
                                }
                                else if (isPlaying == true && rawTicks != 0 && player.IsPlaying == true)
                                {
                                    // wait for player to pause
                                    break;
                                }

                                // reset the stopwatch
                                sw.Reset();

                                // synchronise audio position with player
                                sw.Offset = new(player.Time * 10000);

                                if (isPlaying == true && player.IsPlaying == false)
                                {
                                    // resume the player and wait if it was playing
                                    isPlaying = false;
                                    player.Play();
                                }
                                //
                                if (timebarLock == this) timebarLock = null;
                                // remove this job from queue
                                threadJob.RemoveAt(0);
                                break;
                        }
                    }

                    // player is playing audio
                    if (player.State == VLCState.Playing)
                    {
                        // get current audio position of the player
                        long position = (long)sw.Elapsed.TotalMilliseconds;
                        if (position > audioDuration) position = audioDuration;

                        // get current lyric time
                        LyricTime currentTime = LyricTime.From(position);

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

                        // skip modifying UI elements if thread UI is locked
                        // set text of the time label to player audio duration information
                        TimeLabel.Invoke((MethodInvoker)delegate
                        {
                            if (threadUILock == null)
                                TimeLabel.Text = $"{currentTime} / {LyricTime.From(audioDuration)}";
                        });

                        // set value of the trackbar to audio position
                        TimeBar.Invoke((MethodInvoker)delegate
                        {
                            if (threadUILock == null && timebarLock == null)
                                TimeBar.Value = (int)position;
                        });

                        // update preview by time and lyrics
                        PreviewLabel.Invoke((MethodInvoker)delegate
                        {
                            // show lyrics to preview
                            if (threadUILock == null)
                                PreviewLabel.Text = $"{time}  {lyric}";
                        });

                        // sleep thread for 10 milliseconds
                        Thread.Sleep(10);
                    }
                    // player has ended playing audio
                    else if (player.State == VLCState.Ended)
                    {
                        // Stop player when it goes to Ended state
                        // Ended state causes issue which player stucks
                        if (player.State == VLCState.Ended) player.Stop();

                        // sleep thread for 100 milliseconds
                        Thread.Sleep(100);
                    }
                    else
                    {
                        // sleep thread for 100 milliseconds
                        Thread.Sleep(100);
                    }
                }
                catch (Exception ex)
                {
                    // thread tried to do something at main thread while got an stop request
                    // this can happen if thread was running when stop was called
                    if (running == false) return;
                    // now this is an exception
                    throw ex;
                }
            }
        }
    }
}
