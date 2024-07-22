// reference: https://stackoverflow.com/a/65928123

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing.Text;
using System.Linq;

namespace ti_Lyricstudio.Class
{
    public class LyricsDataSource : IListSource
    {
        private List<LyricData> lyrics;
        private DataTable table;

        public LyricsDataSource(List<LyricData> lyrics)
        {
            // bind lyrics to local variable
            this.lyrics = lyrics;
            // initialise data table
            table = new DataTable();

            // create time column of data table by size of the lyrics data
            foreach (LyricData lyric in lyrics)
            {
                // create new columns when it has less then lyrics time data
                if (table.Columns.Count < lyric.Time.Count)
                {
                    for (int i = table.Columns.Count; i < lyric.Time.Count; i++)
                    {
                        DataColumn timeColumn = new($"Time {i}", typeof(LyricTime));
                        table.Columns.Add(timeColumn);
                    }
                }
            }
            // create lyrics text column
            DataColumn textColumn = new("Text", typeof(String));
            table.Columns.Add(textColumn);

            foreach (LyricData lyric in lyrics)
            {
                // create row and append data
                DataRow r = table.NewRow();
                for (int i = 0; i < lyric.Time.Count; i++)
                {
                    r[i] = lyric.Time[i];
                }
                r[table.Columns.Count - 1] = lyric.Text;
                
                // append row to data table
                table.Rows.Add(r);
            }

            //
            table.DefaultView.ListChanged += DefaultView_ListChanged;
        }

        public bool ContainsListCollection => false;

        public IList GetList()
        {
            return table.DefaultView;
        }

        private void DefaultView_ListChanged(object sender, ListChangedEventArgs e)
        {
            //
        }
    }
}
