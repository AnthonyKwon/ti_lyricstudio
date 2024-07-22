namespace com.stu_tonyk_dio.ti_LyricsStudio.Class
{
    /// <summary>
    /// Object for the single line of lyric.
    /// </summary>
    /// <param name="time">Time position of the lyric. (as LyricTime object)</param>
    /// <param name="text">Text of the lyric. (as string)</param>
    public class LyricData(LyricTime time, string text)
    {
        /// <summary>
        /// Time position of the lyric. (as LyricTime object)
        /// </summary>
        public LyricTime Time => time;
        /// <summary>
        /// Text of the lyric. (as string)
        /// </summary>
        public string Text => text;
    }
}
