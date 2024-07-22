using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms.VisualStyles;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace ti_Lyricstudio.Class
{
    /// <summary>
    /// Object for the single line of lyric.
    /// </summary>
    public class LyricData()
    {
        private List<LyricTime> time = [];  // list of the time of current lyric
        private string text;  // text of the current lyric

        /// <summary>
        /// Text of the lyric. (as string)
        /// </summary>
        public string Text {
            get => text;
            set { text = value; }
        }

        /// <summary>
        /// Time of the lyric. (as list of the LyricTime object)
        /// </summary>
        public List<LyricTime> Time {
            get => time;
            set { time = value; }
        }

        /// <summary>
        /// Get current lyric as LRC-formatted string.
        /// </summary>
        /// <returns>LRC-formatted string of current lyric</returns>
        public override string ToString()
        {
            // string to return
            string combinedString = string.Empty;

            // append all existing timestamp to string
            foreach (LyricTime t in time) combinedString += $"[{t}]";
            // append lyric text to string
            combinedString += text;

            // return final string
            return combinedString;
        }
    }

    // deprecated: only for legacy support
    public class LyricsData
    {
        private LyricData lyric;
        public LyricsData(string time, string text)
        {
            // create new LyricData object
            lyric = new();
            // convert string time to LyricTime object and add it
            lyric.Time.Add(LyricTime.FromString(time));
            // set text to object
            lyric.Text = text;
        }
        

        public string Time { get => lyric.Time[0].ToString(); set { } }
        public string Lyric { get => lyric.Text; set { } }
    }
}
