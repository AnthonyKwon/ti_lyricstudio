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
            int minute, second, msecond;
            // regex to check if full LRC-formatted time string format is correct
            Regex timeRegexFull = new("\\d+\\:\\d{1,2}\\.\\d{1,2}");
            // regex to check if seconds and milliseconds LRC-formatted time string format is correct
            Regex timeRegexSecAndMsec = new("\\d+\\.\\d{1,2}");
            // regex to check if milliseconds only LRC-formatted time string format is correct
            Regex timeRegexMsecOnly = new("\\d+");

            // check which regex is matched
            if (timeRegexFull.IsMatch(time))
            {
                // convert time string to integer
                int min_raw = int.Parse(time.Split(':')[0]);
                int sec_raw = int.Parse(time.Split(':')[1].Split('.')[0]);
                int msec_raw = int.Parse(time.Split('.')[1]);

                // calculate corrected time just for an foolproof
                minute = min_raw + (sec_raw / 60) + (msec_raw / 100);
                second = (sec_raw % 60) + (msec_raw / 100);
                msecond = msec_raw % 100;
            }
            else if (timeRegexSecAndMsec.IsMatch(time))
            {
                // convert time string to integer
                int sec_raw = int.Parse(time.Split('.')[0]);
                int msec_raw = int.Parse(time.Split('.')[1]);

                // calculate corrected time just for an foolproof
                minute = (sec_raw / 60) + (msec_raw / 100);
                second = (sec_raw % 60) + (msec_raw / 100);
                msecond = msec_raw % 100;
            }
            else if (timeRegexMsecOnly.IsMatch(time))
            {
                // convert time string to integer
                int msec_raw = int.Parse(time);

                // calculate corrected time just for an foolproof
                minute = (msec_raw / 6000);
                second = (msec_raw / 100) % 60;
                msecond = msec_raw % 100;
            }
            else
            {
                throw new FormatException("Incorrectly formatted time string provided.");
            }

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
