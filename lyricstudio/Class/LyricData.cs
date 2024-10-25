using System;
using System.Collections.Generic;

namespace ti_Lyricstudio.Class
{
    /// <summary>
    /// Object for the single line of lyric.
    /// </summary>
    public class LyricData() : IEquatable<LyricData>
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
        
        /// <inheritdoc/>
        public bool Equals(LyricData other)
        {
            if (other == null) return false;
            return (time == other.time) && (text == other.text);
        }
    }
}
