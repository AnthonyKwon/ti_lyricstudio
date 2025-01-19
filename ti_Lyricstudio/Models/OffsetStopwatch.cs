using System;
using System.Diagnostics;

namespace ti_Lyricstudio.Models
{
    /// <summary>
    /// Provides a modified version of <see cref="Stopwatch"/> with offset support.
    /// </summary>
    public class OffsetStopwatch : Stopwatch
    {
        /// <summary>
        /// Gets or sets the current offset of the stopwatch. (in ticks)
        /// </summary>
        public long Offset {
            get => _offset;
            set
            {
                _offset = value;
                _elapsed = base.Elapsed.Add(new TimeSpan(value));
            }
        }
        private long _offset;

        /// <inheritdoc cref="Stopwatch.Elapsed"/>
        public new TimeSpan Elapsed => _elapsed;
        private TimeSpan _elapsed;

        // constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="OffsetStopwatch"/> class.
        /// </summary>
        public OffsetStopwatch() => _offset = 0;
        /// <summary>
        /// Initializes a new instance of the <see cref="OffsetStopwatch"/> class.
        /// <param name="offset"><inheritdoc cref="OffsetStopwatch.Offset"/>Current offset of the stopwatch</param>
        /// </summary>
        public OffsetStopwatch(long offset) => _offset = offset;

        /// <inheritdoc cref="Stopwatch.ElapsedMilliseconds"/>
        public new long ElapsedMilliseconds => base.ElapsedMilliseconds + (Offset / (Frequency / 1000));

        /// <inheritdoc cref="Stopwatch.ElapsedTicks"/>
        public new long ElapsedTicks => base.ElapsedTicks + Offset;

        /// <summary>
        /// Gets the elapsed time without offset measured by the current instance, in timer ticks.<br/>
        /// <br/>
        /// Returns:<br/>
        /// A read-only long integer representing the total number of timer ticks measured by the current instance.
        /// </summary>
        public long ElapsedTicksWithoutOffset => base.ElapsedTicks;
    }


}
