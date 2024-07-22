// reference: https://stackoverflow.com/a/65928123

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing.Text;
using System.Linq;
using System.Net;
using System.Windows.Forms;

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
                        DataColumn timeColumn = new($"Time {i+1}");
                        table.Columns.Add(timeColumn);
                    }
                }
            }
            // create lyrics text column
            DataColumn textColumn = new("Text");
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

            // register list changed event
            table.DefaultView.ListChanged += DefaultView_ListChanged;
        }

        public bool ContainsListCollection => false;

        public IList GetList()
        {
            return table.DefaultView;
        }

        private void DefaultView_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemChanged)  // item has changed
            {
                // find the column which has changed
                if (lyrics[e.NewIndex].Text != table.Rows[e.NewIndex][table.Columns.Count - 1].ToString())
                {
                    // text has changed; apply it to object
                    lyrics[e.NewIndex].Text = table.Rows[e.NewIndex][table.Columns.Count - 1].ToString();
                    MessageBox.Show("row_data_text_modified: " + lyrics[e.NewIndex].Text);
                } else
                {
                    for (int i = 0; i < table.Columns.Count - 1; i++)
                    {
                        // new additional time has added to row
                        if (lyrics[e.NewIndex].Time.Count - 1 < i && table.Rows[e.NewIndex][i].ToString() != "")
                        {
                            // check if value is appended to right next to the existing time value
                            lyrics[e.NewIndex].Time.Add(LyricTime.FromString(table.Rows[e.NewIndex][i].ToString()));
                            // unregister list changed event temporary to prevent distrupt
                            table.DefaultView.ListChanged -= DefaultView_ListChanged;
                            // move appended value to closest empty column
                            table.Rows[e.NewIndex][i] = "";
                            table.Rows[e.NewIndex][lyrics[e.NewIndex].Time.Count - 1] = lyrics[e.NewIndex].Time[lyrics[e.NewIndex].Time.Count - 1];
                            // re-unregister list changed event
                            table.DefaultView.ListChanged += DefaultView_ListChanged;
                            MessageBox.Show("row_data_time_appended: " + lyrics[e.NewIndex].Time[lyrics[e.NewIndex].Time.Count - 1].ToString());
                        }
                        // existing time has modified
                        else if (lyrics[e.NewIndex].Time.Count - 1 >= i && lyrics[e.NewIndex].Time[i].ToString() != table.Rows[e.NewIndex][i].ToString())
                        {
                            lyrics[e.NewIndex].Time[i] = LyricTime.FromString(table.Rows[e.NewIndex][i].ToString());
                            MessageBox.Show("row_data_time_modified: " + lyrics[e.NewIndex].Time[i].ToString());
                        }
                    }
                }
            }
            else if (e.ListChangedType == ListChangedType.ItemAdded)  // item has added
            {
                // ensure new rows are correctly added
                if (table.Rows.Count <= e.NewIndex) return;

                // insert new LyricData to object
                lyrics.Insert(e.NewIndex, new LyricData());

                // find which data has provided to insert
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    if (i < table.Columns.Count - 1 && table.Rows[e.NewIndex][i].ToString() != "")
                    {
                        // time column has value
                        lyrics[e.NewIndex].Time.Add(LyricTime.FromString(table.Rows[e.NewIndex][i].ToString()));
                        MessageBox.Show("row_appended: " + lyrics[e.NewIndex].Time[lyrics[e.NewIndex].Time.Count - 1].ToString());
                        break;
                    }
                    else
                    {
                        // text column has value
                        lyrics[e.NewIndex].Text = table.Rows[e.NewIndex][table.Columns.Count - 1].ToString();
                        MessageBox.Show("row_appended: " + lyrics[e.NewIndex].Text);
                        break;
                    }
                }
            }
            else if (e.ListChangedType == ListChangedType.ItemDeleted)  // item has removed
            {
                MessageBox.Show("row_deleted:\n" + lyrics[e.NewIndex]);
                lyrics.RemoveAt(e.NewIndex);
            }
            else
            {
                // non-supported action, throw an exception
                throw new NotSupportedException();
            }
        }
    }
}
