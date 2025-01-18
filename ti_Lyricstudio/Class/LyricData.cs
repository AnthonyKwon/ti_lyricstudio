using System;
using System.Collections.Generic;
using System.Text;

namespace ti_Lyricstudio.Class
{
    /// <summary>
    /// Object for the single line of lyric.
    /// </summary>
    public class LyricData()
    {
        private List<LyricTime>? _time;  // list of the time of current lyric
        private string _text = string.Empty;  // text of the current lyric

        /// <summary>
        /// Text of the lyric. (as string)
        /// </summary>
        public string Text {
            get => _text;
            set { _text = value; }
        }

        /// <summary>
        /// Time of the lyric. (as list of the LyricTime object)
        /// </summary>
        public List<LyricTime> Time {
            get => _time ??= [];  // null checked but shouldn't be null under normal condition!
            set { _time = value ?? throw new ArgumentNullException(nameof(value), "Time object should not be null."); }
        }

        /// <summary>
        /// Get current lyric as LRC-formatted string.
        /// </summary>
        /// <returns>LRC-formatted string of current lyric</returns>
        public override string ToString()
        {
            // string to return
            StringBuilder sb = new();

            // loop over timestamp only when _timp is not null
            if (_time != null)
            {
                // append all existing timestamp to string
                foreach (LyricTime t in _time)
                    sb.Append($"[{t}]");
            }
            // append lyric text to string
            sb.Append(_text);

            // return final string
            return sb.ToString();
        }
    }
}
