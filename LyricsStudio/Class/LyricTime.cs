using System;
using System.Text.RegularExpressions;

namespace ti_Lyricstudio.Class
{
    /// <summary>
    /// Time position object for the lyric.
    /// </summary>
    /// <param name="minute">Minutes of the time.</param>
    /// <param name="second">Seconds of the time.</param>
    /// <param name="millisecond">LRC-formatted milliseconds of the time.</param>
    public class LyricTime(int minute, int second, int millisecond)
    {
        /// <summary>
        /// Minutes of the time.
        /// </summary>
        public int Minute => minute;
        /// <summary>
        /// Seconds of the time.
        /// </summary>
        public int Second => second;
        /// <summary>
        /// LRC-formatted milliseconds of the time.
        /// </summary>
        public int Millisecond => millisecond;
        /// <summary>
        /// Create LyricTime object from LRC-formatted time string
        /// </summary>
        /// <param name="time">Current time position as LRC-formatted time string</param>
        /// <returns>Current time position as LyricTime object</returns>
        public static LyricTime FromString(string time) {
            // regex to check if time string format is correct
            Regex timeRegex = new Regex("\\d+\\:\\d{1,2}\\.\\d{1,2}");
            if (!timeRegex.IsMatch(time))
            {
                throw new FormatException("Incorrectly formatted time string provided.");
            }

            // convert time string to integer
            int minute = int.Parse(time.Split(':')[0]);
            int second = int.Parse(time.Split(':')[1].Split('.')[0]) % 60;
            int msecond = int.Parse(time.Split('.')[1]);

            // return converted LyricTime object
            return new LyricTime(minute, second, msecond);
        }
        /// <summary>
        /// Get time position as string.
        /// </summary>
        /// <returns>Current time position as string</returns>
        public override string ToString() { return $"{minute.ToString("00")}:{second.ToString("00")}.{millisecond.ToString("00")}"; }
    }
}
