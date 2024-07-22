using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace com.stu_tonyk_dio.ti_LyricsStudio.Class
{
    public static class LRCHandler
    {
        /// <summary>
        /// Convert LRC to lyrics data object.
        /// </summary>
        /// <param name="line">A single line of LRC format string.</param>
        /// <returns>A single object of lyrics data.</returns>
        public static object From(string line)
        {
            // regex to find LRC-specific header
            Regex LrcHeaders = new Regex("^\\[(ti|ar|al|au|length|by|offset|re|tool|ve|#)\\:");

            // match for LRC-specific header and return it if matches
            Match match = LrcHeaders.Match(line);
            if (match.Success)
            {
                return line;
            }

            // regex to find first matching time "[MM:SS:xx]"
            Regex TimeRegex = new Regex("^\\[\\d\\d\\:\\d\\d\\.\\d\\d\\]");

            // lyrics data object to return
            LyricData lyric = null;

            // extract all time from lyrics line
            do
            {
                // find first matching time from lyrics line
                match = TimeRegex.Match(line);
                
                // break out of while when time is not found anymore
                if (lyric != null && !match.Success)
                {
                    // set remaining lyrics string as text
                    lyric.Text = line;
                    break;
                } else if (lyric == null && !match.Success)
                {
                    // invalid LRC string provided; throw invalid operation exception
                    throw new InvalidOperationException($"Invalid string \"{line}\" provided.");
                }

                // extract time from matched value
                string rawTime = match.Groups[0].Value;
                // remove time from lyrics
                line = line.Replace(rawTime, "");

                // parse time from rawTime variable
                LyricTime time = new(int.Parse(rawTime.Substring(1, 2)), int.Parse(rawTime.Substring(4, 2)), int.Parse(rawTime.Substring(7, 2)));

                // set new time to lyrics data
                // initialise lyrics data object if not done yet
                if (lyric == null) lyric = new LyricData(time, "This is a dummy text; This should not be exposed.");
                else lyric.addTime(time);
            } while (true);

            // return lyrics data
            return lyric;
        }
    }
}
