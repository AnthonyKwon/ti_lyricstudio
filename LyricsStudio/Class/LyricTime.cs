namespace com.stu_tonyk_dio.ti_LyricsStudio.Class
{
    /// <summary>
    /// Time position object for the lyric.
    /// </summary>
    /// <param name="minute">Minutes of the time.</param>
    /// <param name="second">Seconds of the time.</param>
    /// <param name="msec_hundred">LRC-formatted milliseconds of the time.</param>
    public class LyricTime(int minute, int second, int msec_hundred)
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
        public int Msec_Hundred => msec_hundred;
        /// <summary>
        /// Get time position as string.
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return $"{minute.ToString("00")}:{second.ToString("00")}.{msec_hundred.ToString("00")}"; }
    }
}
