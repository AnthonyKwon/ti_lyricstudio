using System;
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
        [GeneratedRegex("^(?:\\[\\d+:[0-5]\\d\\.\\d{2}\\])+", RegexOptions.Compiled)]
        private static partial Regex LRCContentHeader();

        // regex to find full LRC-formatted content
        [GeneratedRegex("^(?:\\[\\d+:[0-5]\\d\\.\\d{2}\\])+.*$", RegexOptions.Compiled)]
        private static partial Regex LRCContentFull();

        /// <summary>
        /// Convert LRC-formatted <see cref="String"/> to <see cref="LyricData"/>.
        /// </summary>
        /// <param name="line">A single line of LRC format string.</param>
        /// <returns>A <see cref="LyricData"/> contains converted lyric data.<br/>
        /// <see cref="String"/> with <paramref name="line"/> will be returned when conversion failed.</returns>
        public static object From(string line)
        {
            // match for empty line or line only with whitespace
            // it will be ignored while parsing
            if (line.Trim().Length == 0) return line;

            // match for LRC-specific header
            // TODO: implement proper LRC header handler
            if (Header().IsMatch(line)) return line;

            // lyrics data object to return
            LyricData lyric = new();

            // find first matching time from lyrics line
            MatchCollection timeMatch = LRCContentHeader().Matches(line);

            // not a LRC-formatted content; parse as LyricData with empty time
            if (timeMatch.Count == 0) return new LyricData { Text = line };
                
            // loop over every LRC time header
            foreach (Match match in timeMatch)
            {
                foreach (string rawHeader in match.Value.Split("]["))
                {
                    // extract time from the header
                    string rawTime = rawHeader.Replace("[", string.Empty);
                    rawTime = rawTime.Replace("]", string.Empty);

                    // Add new time from raw time data
                    lyric.Time.Add(LyricTime.From(rawTime));
                }

                // set remaining lyrics string as text
                lyric.Text = line.Replace(match.Value, string.Empty);
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
