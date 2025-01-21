using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ti_Lyricstudio.Models
{
    /// <summary>
    /// Handler for the LRC-formatted lyrics.
    /// </summary>
    public static partial class LRCHandler
    {
        // regex to find LRC-formatted header
        [GeneratedRegex("^\\[[a-z]+:.+\\]$", RegexOptions.Compiled)]
        private static partial Regex Header();

        // regex to find header of LRC-formatted content
        [GeneratedRegex("\\[\\d+:[0-5]\\d\\.\\d{2}\\]", RegexOptions.Compiled)]
        private static partial Regex LRCContentHeader();

        // regex to find full LRC-formatted content
        [GeneratedRegex("^\\[\\d+:[0-5]\\d\\.\\d{2}\\].*$", RegexOptions.Compiled)]
        private static partial Regex LRCContentFull();

        /// <summary>
        /// Convert LRC-formatted <see cref="String"/> to <see cref="LyricData"/>.
        /// </summary>
        /// <param name="line">A single line of LRC format string.</param>
        /// <returns>A <see cref="LyricData"/> contains converted lyric data.<br/>
        /// <see cref="String"/> with <paramref name="line"/> will be returned when conversion failed.</returns>
        public static object From(string line)
        {
            // match for LRC-specific header
            // TODO: implement proper LRC header handler
            if (Header().IsMatch(line)) return line;

            // match for empty line or line only with whitespace
            // return empty LyricData without any timestamp
            if (line.Trim().Length == 0) return new LyricData { Time = [LyricTime.Empty], Text = line };

            // lyrics data object to return
            LyricData lyric = new();

            // check if current line is LRC-formatted content
            if (LRCContentFull().IsMatch(line))
            {
                // find first matching time from lyrics line
                MatchCollection timeMatch = LRCContentHeader().Matches(line);

                foreach (Match match in timeMatch)
                {
                    // extract LRC-tagged time from matched data
                    string rawTime = match.Value;

                    // Add new time from raw time data
                    lyric.Time.Add(LyricTime.From(rawTime.Substring(1, 8)));

                    // remove matched time from current line
                    line = line.Replace(rawTime, string.Empty);
                }
                // set remaining lyrics string as text
                lyric.Text = line;
            } else
            {
                // not a LRC-formatted content; parse as LyricData with empty time
                lyric.Text = line;
            }
            // return lyrics data
            return lyric;
        }

        /// <summary>
        /// Convert <see cref="LyricData"/> to LRC-formatted <see cref="String"/>.
        /// </summary>
        /// <param name="lyric"></param>
        /// <returns>LRC formatted <see cref="String"/> of <see cref="LyricData"/></returns>
        public static string To(LyricData lyric)
        {
            // this does everything ;)
            return lyric.ToString();
        }
    }
}
