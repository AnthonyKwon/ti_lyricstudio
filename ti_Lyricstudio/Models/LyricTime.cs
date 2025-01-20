using System;
using System.Text.RegularExpressions;

namespace ti_Lyricstudio.Models
{
    /// <summary>
    /// Time position object for the lyric.
    /// </summary>
    /// <param name="minute">Minutes of the time.</param>
    /// <param name="second">Seconds of the time.</param>
    /// <param name="millisecond">LRC-formatted milliseconds of the time.</param>
    public partial class LyricTime(int minute, int second, int millisecond)
    {
        /// <summary>
        /// Minutes of the time.
        /// </summary>
        public int Minute {
            get => minute;
            set
            {
                minute = value;
                if (IsEmpty == true) _isEmpty = false;
            }
        }
        /// <summary>
        /// Seconds of the time.
        /// </summary>
        public int Second
        {
            get => second;
            set
            {
                second = value;
                if (IsEmpty == true) _isEmpty = false;
            }
        }
        /// <summary>
        /// Milliseconds of the time.
        /// </summary>
        public int Millisecond
        {
            get => millisecond;
            set
            {
                millisecond = value;
                if (IsEmpty == true) _isEmpty = false;
            }
        }

        /// <summary>
        /// Total milliseconds of the time.
        /// </summary>
        public long TotalMillisecond { get => (minute * 60000) + (second * 1000) + millisecond; }

        /// <summary>
        /// Result for comparing two <see cref="LyricTime"/> object.
        /// </summary>
        public enum Comparator
        {
            /// <summary>
            /// Left side of the object has bigger.
            /// </summary>
            LeftIsBigger,
            /// <summary>
            /// Right side of the object has bigger.
            /// </summary>
            RightIsBigger,
            /// <summary>
            /// Both object have the same time.
            /// </summary>
            BothAreSame
        }

        /// <summary>
        /// Gets empty <see cref="LyricTime"/> object.
        /// </summary>
        public static LyricTime Empty
        {
            get
            {
                LyricTime newTime = new(0, 0, 0) { _isEmpty = true };
                return newTime;
            }
        }

        /// <summary>
        /// Checks if <see cref="LyricTime"/> object is empty.
        /// </summary>
        public bool IsEmpty { get => _isEmpty; }
        private bool _isEmpty = false;

        // regex to check if full time string format is correct
        [GeneratedRegex("\\d+\\:\\d{1,2}\\.\\d{1,2}")]
        private static partial Regex TimeRegexFull();

        // regex to check if minutes and seconds only time string format is correct
        [GeneratedRegex("\\d+\\:\\d{1,2}")]
        private static partial Regex TimeRegexMinAndSec();

        // regex to check if seconds and milliseconds LRC-formatted time string format is correct
        [GeneratedRegex("\\d+\\.\\d{1,2}")]
        private static partial Regex TimeRegexSecAndMsec();

        // regex to check if milliseconds only LRC-formatted time string format is correct
        [GeneratedRegex("\\d+")]
        private static partial Regex TimeRegexMsecOnly();

        /// <summary>
        /// Compare time of the two <see cref="LyricTime"/> object.<br/>
        /// Empty target will always be considered as smaller.
        /// </summary>
        /// <param name="left">LyricTime object to compare</param>
        /// <param name="right">LyricTime object to compare</param>
        /// <returns>Result of which side of <see cref="LyricTime"/> object is bigger.</returns>
        public static Comparator Compare(LyricTime left, LyricTime right)
        {
            // convert both side as integer
            int leftAsInteger = left.Minute * 60000 + left.Second * 1000 + left.Millisecond;
            int rightAsInteger = right.Minute * 60000 + right.Second * 1000 + right.Millisecond;

            // always treat empty LyricTime as smallest
            if ((leftAsInteger > rightAsInteger) || (left._isEmpty == false && right._isEmpty == true))
            {
                return Comparator.LeftIsBigger;
            }
            else if (leftAsInteger < rightAsInteger || (left._isEmpty == true && right._isEmpty == false))
            {
                return Comparator.RightIsBigger;
            }
            else
            {
                return Comparator.BothAreSame;
            }
        }

        /// <summary>
        /// Create <see cref="LyricTime"/> object from time
        /// </summary>
        /// <param name="time">Current time position as milliseconds</param>
        /// <returns>Current time position as LyricTime object</returns>
        public static LyricTime From(long time)
        {
            if (time < 0) throw new ArgumentOutOfRangeException("Time value should not be negative.");

            int minute, second, millisecond;

            minute = (int)(time / 60000);
            second = (int)(time / 1000 % 60);
            millisecond = (int)(time % 1000);

            return new LyricTime(minute, second, millisecond);
        }
        /// <summary>
        /// Create <see cref="LyricTime"/> object from LRC-formatted time string
        /// </summary>
        /// <param name="time">Current time position as LRC-formatted time string</param>
        /// <returns>Current time position as <see cref="LyricTime"/> object</returns>
        public static LyricTime From(string time)
        {
            int minute, second, msecond;

            // check which regex is matched
            if (TimeRegexFull().IsMatch(time))
            {
                // convert time string to integer
                int min_raw = int.Parse(time.Split(':')[0]);
                int sec_raw = int.Parse(time.Split(':')[1].Split('.')[0]);
                int msec_raw = int.Parse(time.Split('.')[1]);

                // calculate corrected time just for an foolproof
                minute = min_raw + sec_raw / 60 + msec_raw / 100;
                second = sec_raw % 60 + msec_raw / 100;
                msecond = msec_raw % 100;
            }
            else if (TimeRegexMinAndSec().IsMatch(time))
            {
                // convert time string to integer
                int min_raw = int.Parse(time.Split(':')[0]);
                int sec_raw = int.Parse(time.Split(':')[1]);

                // calculate corrected time just for an foolproof
                minute = min_raw + sec_raw / 60;
                second = sec_raw % 60;
                msecond = 0;
            }
            else if (TimeRegexSecAndMsec().IsMatch(time))
            {
                // convert time string to integer
                int sec_raw = int.Parse(time.Split('.')[0]);
                int msec_raw = int.Parse(time.Split('.')[1]);

                // calculate corrected time just for an foolproof
                minute = sec_raw / 60 + msec_raw / 100;
                second = sec_raw % 60 + msec_raw / 100;
                msecond = msec_raw % 100;
            }
            else if (TimeRegexMsecOnly().IsMatch(time))
            {
                // convert time string to integer
                int msec_raw = int.Parse(time);

                // calculate corrected time just for an foolproof
                minute = msec_raw / 6000;
                second = msec_raw / 100 % 60;
                msecond = msec_raw % 100;
            }
            else
            {
                throw new FormatException("Incorrectly formatted time string provided.");
            }

            // return converted LyricTime object
            return new LyricTime(minute, second, msecond * 10);
        }

        /// <summary>
        /// Verify if <paramref name="time"/> is valid.
        /// </summary>
        /// <param name="time"><see cref="String"/> to verify</param>
        /// <returns>Return true if valid, false if not.</returns>
        public static bool Verify(string time)
        {
            if (TimeRegexFull().IsMatch(time)) return true;
            else if (TimeRegexMinAndSec().IsMatch(time)) return true;
            else if (TimeRegexSecAndMsec().IsMatch(time)) return true;
            else if (TimeRegexMsecOnly().IsMatch(time)) return true;
            else return false;
        }

        /// <summary>
        /// Get time position as string.
        /// </summary>
        /// <returns>Current time position as string</returns>
        public override string ToString()
        {
            // return emptry string if time has nothing
            if (_isEmpty) return string.Empty;
            // return time position interpreted by LRC-time format
            return $"{minute:00}:{second:00}.{millisecond / 10:00}";
        }
    }
}
