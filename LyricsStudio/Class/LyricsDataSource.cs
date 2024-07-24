// reference: https://stackoverflow.com/a/65928123

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
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

        public void EmptyCell(int column, int row)
        {
            // unregister list changed event temporary to prevent distrupt
            table.DefaultView.ListChanged -= DefaultView_ListChanged;

            // check which data user trying to remove
            if (column == table.Columns.Count - 1)
            {
                // user trying to remove text
                lyrics[row].Text = string.Empty;

                // clear target cell
                table.Rows[row][column] = string.Empty;
            } else {
                // user trying to remove time
                // check if removed time data was in last index
                if (column < lyrics[row].Time.Count - 1)
                {
                    // data not from last index; pull all time from the current one
                    for (int i = column; i < lyrics[row].Time.Count - 1; i++)
                    {
                        table.Rows[row][column] = table.Rows[row][column + 1];
                        lyrics[row].Time[i] = lyrics[row].Time[i + 1];
                    }
                    // remove the value in last index
                    int closestEmptyIndex = lyrics[row].Time.Count - 1;
                    table.Rows[row][closestEmptyIndex] = string.Empty;
                    lyrics[row].Time.RemoveAt(closestEmptyIndex);
                }
                else
                {
                    // data from the last index; just remove it from the lyrics
                    lyrics[row].Time.RemoveAt(column);

                    // clear target cell
                    table.Rows[row][column] = string.Empty;
                }
            }

            // re-register list changed event
            table.DefaultView.ListChanged += DefaultView_ListChanged;
        }
        
        /// <summary>
        /// Insert row to data table.
        /// </summary>
        /// <param name="index">Index of inserted lyrics</param>
        /// <param name="lyric">Lyrics data to insert</param>
        public void Insert(int index, LyricData lyric)
        {
            // unregister list changed event temporary to prevent distrupt
            table.DefaultView.ListChanged -= DefaultView_ListChanged;

            // add data to lyrics list
            lyrics.Insert(index, lyric);

            // create row data to insert
            object[] d = [];
            d.Append(lyric.Time.ToArray());
            d.Append(lyric.Text);

            // create new DataRow based on the data
            DataRow r = table.NewRow();
            r.ItemArray = d;

            // add row to data table
            table.Rows.InsertAt(r, index);

            // re-register list changed event
            table.DefaultView.ListChanged += DefaultView_ListChanged;
        }

        /// <summary>
        /// Move row to another position.
        /// </summary>
        /// <param name="from">Cell to move</param>
        /// <param name="to">Position to place cell</param>
        public void Move(int from, int to)
        {
            // do not continue when cell is out of bounds
            if (from >= table.Rows.Count || to >= table.Rows.Count) return;

            // unregister list changed event temporary to prevent distrupt
            table.DefaultView.ListChanged -= DefaultView_ListChanged;

            // swap table data
            // cast cell data to array and remove previous row
            object[] d = table.Rows[from].ItemArray;
            table.Rows.RemoveAt(from);

            // create new row and insert to new position
            DataRow r = table.NewRow();
            r.ItemArray = d;
            table.Rows.InsertAt(r, to);

            // swap array data
            LyricData data = lyrics[from];
            lyrics.RemoveAt(from);
            lyrics.Insert(to, data);

            // re-register list changed event
            table.DefaultView.ListChanged += DefaultView_ListChanged;
        }

        /// <summary>
        /// Removes last time column from the data table.
        /// </summary>
        public void PopTimeColumn()
        {
            // do not continue when time column is already small (<= 1)
            if (table.Columns.Count <= 1) return;

            // get index value to remove
            int popIndex = table.Columns.Count - 2;

            // match and remove time data in target index of every lyric
            foreach (LyricData lyric in lyrics)
            {
                if (popIndex < lyric.Time.Count)
                {
                    lyric.Time.RemoveAt(popIndex);
                }
            }

            // remove last time column
            table.Columns.RemoveAt(popIndex);
        }

        /// <summary>
        /// Appends new time column to the data table.
        /// </summary>
        public void PushTimeColumn()
        {
            // get index value to add
            int pushIndex = table.Columns.Count - 1;

            // create new time column
            DataColumn newTimeColumn = new($"Time {pushIndex + 1}");
            table.Columns.Add(newTimeColumn);
            
            // reorder new time column
            table.Columns[table.Columns.Count - 1].SetOrdinal(pushIndex);
        }

        public void DefaultView_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemChanged)  // item has changed
            {
                // find the column which has changed
                int column = -1;
                // get column from Property Descriptor
                if (e.PropertyDescriptor != null) column = table.Columns.IndexOf(e.PropertyDescriptor.Name);
                // fallback to slower method
                else
                {
                    int colSize = table.Columns.Count;
                    for (int i = 0; i < colSize; i++)
                    {
                        // time has changed
                        if (i < colSize - 1)
                        {
                            // new time has added
                            if (i > lyrics[e.NewIndex].Time.Count - 1 && table.Rows[e.NewIndex][i].ToString() != "")
                            {
                                column = i;
                                break;
                            }
                            // existing time has modified
                            else if (i <= lyrics[e.NewIndex].Time.Count - 1 && table.Rows[e.NewIndex][i].ToString() != lyrics[e.NewIndex].Time[i].ToString())
                            {
                                column = i;
                                break;
                            }
                        }
                        // text has changed
                        else
                        {
                            column = i;
                            break;
                        }
                    }
                }

                if (column >= 1 && column == table.Columns.Count - 1)
                {
                    // text has changed; apply it to object
                    lyrics[e.NewIndex].Text = table.Rows[e.NewIndex][column].ToString();
                }
                else
                {
                    // new additional time has added to row
                    if (column > lyrics[e.NewIndex].Time.Count - 1)
                    {
                        // add new LyricTime object to lyrics data
                        lyrics[e.NewIndex].Time.Add(LyricTime.From(table.Rows[e.NewIndex][column].ToString()));
                        // unregister list changed event temporary to prevent distrupt
                        table.DefaultView.ListChanged -= DefaultView_ListChanged;
                        // move appended value to closest empty column
                        int closestEmptyIndex = lyrics[e.NewIndex].Time.Count - 1;
                        table.Rows[e.NewIndex][column] = "";
                        table.Rows[e.NewIndex][closestEmptyIndex] = lyrics[e.NewIndex].Time[closestEmptyIndex];
                        // re-register list changed event
                        table.DefaultView.ListChanged += DefaultView_ListChanged;
                    }
                    // existing time data has deleted
                    else if (column <= lyrics[e.NewIndex].Time.Count - 1 && table.Rows[e.NewIndex][column].ToString() == "")
                    {

                        // check if removed time data was in last index
                        if (column < lyrics[e.NewIndex].Time.Count - 1)
                        {
                            // unregister list changed event temporary to prevent distrupt
                            table.DefaultView.ListChanged -= DefaultView_ListChanged;

                            // not in last index, we need to pull every value after current one
                            for (int j = column; j < lyrics[e.NewIndex].Time.Count - 1; j++)
                            {
                                table.Rows[e.NewIndex][j] = table.Rows[e.NewIndex][j + 1];
                                lyrics[e.NewIndex].Time[j] = lyrics[e.NewIndex].Time[j + 1];
                            }
                            // remove the value in last index
                            int closestEmptyIndex = lyrics[e.NewIndex].Time.Count - 1;
                            table.Rows[e.NewIndex][closestEmptyIndex] = string.Empty;
                            lyrics[e.NewIndex].Time.RemoveAt(closestEmptyIndex);

                            // re-register list changed event
                            table.DefaultView.ListChanged += DefaultView_ListChanged;
                        }
                        else
                        {
                            // remove specified time from lyrics data
                            lyrics[e.NewIndex].Time.RemoveAt(column);
                        }
                    }
                    // existing time data has modified
                    else if (lyrics[e.NewIndex].Time.Count - 1 >= column && lyrics[e.NewIndex].Time[column].ToString() != table.Rows[e.NewIndex][column].ToString())
                    {
                        // unregister list changed event temporary to prevent distrupt
                        table.DefaultView.ListChanged -= DefaultView_ListChanged;

                        lyrics[e.NewIndex].Time[column] = LyricTime.From(table.Rows[e.NewIndex][column].ToString());
                        table.Rows[e.NewIndex][column] = lyrics[e.NewIndex].Time[column];

                        // re-register list changed event
                        table.DefaultView.ListChanged += DefaultView_ListChanged;
                    }
                    else
                    {
                        // never thought about it; maybe throw some exception?
                        throw new NotImplementedException();
                    }
                }
            }
            else if (e.ListChangedType == ListChangedType.ItemAdded)  // item has added
            {   
                // ensure new rows are correctly added
                if (table.Rows.Count <= e.NewIndex) return;

                // detached row is not actual added row, reject adding data from it
                if (table.Rows[e.NewIndex].RowState == DataRowState.Detached) return;

                // insert new LyricData to object
                lyrics.Insert(e.NewIndex, new LyricData());

                // find which data has provided to insert
                for (int i = 0; i < table.Columns.Count - 1; i++)
                {
                    // check if time column has value
                    if (table.Rows[e.NewIndex][i].ToString() != "")
                    {
                        // unregister list changed event temporary to prevent distrupt
                        table.DefaultView.ListChanged -= DefaultView_ListChanged;

                        // add LyricTime object from the cell value
                        lyrics[e.NewIndex].Time.Add(LyricTime.From(table.Rows[e.NewIndex][i].ToString()));
                        // update value of the cell
                        // first time object always saved at 0, so move it to first column of the cell
                        table.Rows[e.NewIndex][i] = "";
                        table.Rows[e.NewIndex][0] = lyrics[e.NewIndex].Time[0];

                        // re-register list changed event
                        table.DefaultView.ListChanged += DefaultView_ListChanged;
                        return;
                    }
                }

                // check if text column has value
                if (table.Rows[e.NewIndex][table.Columns.Count - 1] != null && table.Rows[e.NewIndex][table.Columns.Count - 1].ToString() != "")
                {
                    // add text from the cell value
                    lyrics[e.NewIndex].Text = table.Rows[e.NewIndex][table.Columns.Count - 1].ToString();
                    return;
                }

            }
            else if (e.ListChangedType == ListChangedType.ItemDeleted)  // item has removed
            {
                // check if target row is in index
                if (e.NewIndex < table.Rows.Count && e.NewIndex < lyrics.Count)
                {
                    // delete selected row
                    lyrics.RemoveAt(e.NewIndex);
                }
            }
            else if (e.ListChangedType is ListChangedType.PropertyDescriptorAdded or ListChangedType.PropertyDescriptorChanged or ListChangedType.PropertyDescriptorDeleted)
            {
                // do nothing, actual job will be done in each function
            }
            else
            {
                // non-supported action, throw an exception
                throw new NotSupportedException();
            }
        }
    }
}
