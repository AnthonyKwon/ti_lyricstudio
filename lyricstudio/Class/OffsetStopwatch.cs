using System;
using System.Diagnostics;

namespace ti_Lyricstudio.Class
{
    public class OffsetStopwatch : Stopwatch
    {
        public TimeSpan Offset { get; set; }

        public OffsetStopwatch()
        {
            Offset = TimeSpan.Zero;
        }

        public OffsetStopwatch(TimeSpan offset)
        {
            Offset = offset;
        }

        public new TimeSpan Elapsed
        {
            get
            {
                return base.Elapsed.Add(Offset);
            }
        }

        public new long ElapsedMilliseconds
        {
            get
            {
                return base.ElapsedMilliseconds + (long)Offset.TotalMilliseconds;
            }
        }

        public new long ElapsedTicks
        {
            get
            {
                return base.ElapsedTicks + Offset.Ticks;
            }
        }
    }


}
