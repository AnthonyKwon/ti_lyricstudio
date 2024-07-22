using System.Collections.Generic;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace com.stu_tonyk_dio.ti_LyricsStudio.Class
{
    /// <summary>
    /// Object for the single line of lyric.
    /// </summary>
    /// <param name="time">Time position of the lyric. (as LyricTime object)</param>
    /// <param name="text">Text of the lyric. (as string)</param>
    public class LyricData(LyricTime time, string text)
    {
        private List<LyricTime> timeList = [time];

        /// <summary>
        /// Add new time position to the lyric.
        /// </summary>
        /// <param name="time"></param>
        public void addTime(LyricTime time)
        {
            timeList.Add(time);
        }
        /// <summary>
        /// Get time positions of the lyric. (as LyricTime object)
        /// </summary>
        public List<LyricTime> getTimes() { return timeList; }
        /// <summary>
        /// Text of the lyric. (as string)
        /// </summary>
        public string Text { get => text; set { text = value; } }
    }

    // deprecated: only for legacy support
    public class LyricsData(string time, string text)
    {
        // convert string time to integer time and create new LyricData object
        private LyricData lyric = new(new LyricTime(int.Parse(time.Substring(0, 2)), int.Parse(time.Substring(0, 2)), int.Parse(time.Substring(0, 2))), text);

        public string Time { get => lyric.getTimes()[0].ToString(); set { } }
        public string Lyric { get => lyric.Text; set { } }
    }
}
