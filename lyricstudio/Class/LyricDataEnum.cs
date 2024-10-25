using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace ti_Lyricstudio.Class
{
    public class LyricDataEnum : IEnumerator<LyricData>
    {
        public List<LyricData> lyrics;
        int index = -1;

        public LyricDataEnum(List<LyricData> lyrics)
        {
            this.lyrics = lyrics;
            index = 0;
        }

        public bool MoveNext()
        {
            index++;
            return (index < lyrics.Count);
        }

        public void Reset()
        {
            index = 0;
        }

        public void Dispose()
        {
            lyrics.Clear();
            index = -1;
        }

        object IEnumerator.Current => lyrics.ElementAt(index);

        LyricData IEnumerator<LyricData>.Current => lyrics.ElementAt(index);
    }
}
