#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Drawing;
using System.Windows.Forms;
using ti_Lyricstudio.Class;

namespace ti_Lyricstudio
{
    // DataGridView codes for MainWindow
    public partial class MainWindow
    {
        private Rectangle dragBoxFromMouseDown;
        private int rowIndexFromMouseDown;
        private int rowIndexOfItemUnderMouseToDrop;

        public void DataGridView_AddLine(string Time, string Text)
        {
            /*
            CData.Add(new LyricsData(Time, Text));
            DataGridView.DataSource = TData;
            DataGridView.DataSource = CData;
            IsDirty = true;
            */
        }

        private void DataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            /*
            if (!string.IsNullOrEmpty(CData[CData.Count - 1].Time) | !string.IsNullOrEmpty(CData[CData.Count - 1].Lyric))
            {
                CData.Add(new LyricsData(Constants.vbNullString, Constants.vbNullString));
                DataGridView.DataSource = TData;
                DataGridView.DataSource = CData;
            }
            else
            {
                while (string.IsNullOrWhiteSpace(CData[CData.Count - 1].Time) & string.IsNullOrWhiteSpace(CData[CData.Count - 1].Lyric))
                    CData.Remove(CData[CData.Count - 1]);
                CData.Add(new LyricsData(Constants.vbNullString, Constants.vbNullString));
                DataGridView.DataSource = TData;
                DataGridView.DataSource = CData;
            }
            if (IsDirty == false)
            {
                IsDirty = true;
            }
            */
        }

        private void DataGridView_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            // disable sorting of the column
            DataGridView.Columns[e.Column.Index].SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        // ref: https://stackoverflow.com/a/1623968
        private void DataGridView_DragDrop(object sender, DragEventArgs e)
        {
            // The mouse locations are relative to the screen, so they must be 
            // converted to client coordinates.
            Point clientPoint = DataGridView.PointToClient(new Point(e.X, e.Y));

            // Get the row index of the item the mouse is below. 
            rowIndexOfItemUnderMouseToDrop =
                DataGridView.HitTest(clientPoint.X, clientPoint.Y).RowIndex;

            // If the drag operation was a move then remove and insert the row.
            if (e.Effect == DragDropEffects.Move)
            {
                DataGridViewRow rowToMove = e.Data.GetData(
                    typeof(DataGridViewRow)) as DataGridViewRow;
                dataSource.Move(rowIndexFromMouseDown, rowIndexOfItemUnderMouseToDrop);
            }
        }

        private void DataGridView_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void DataGridView_MouseDown(object sender, MouseEventArgs e)
        {
            // Get the index of the item the mouse is below.
            rowIndexFromMouseDown = DataGridView.HitTest(e.X, e.Y).RowIndex;
            if (rowIndexFromMouseDown != -1)
            {
                // Remember the point where the mouse down occurred. 
                // The DragSize indicates the size that the mouse can move 
                // before a drag event should be started. 
                Size dragSize = SystemInformation.DragSize;

                // Create a rectangle using the DragSize, with the mouse position being
                // at the center of the rectangle.
                dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)), dragSize);
            }
            else
            {
                // Reset the rectangle if the mouse is not over an item in the ListBox.
                dragBoxFromMouseDown = Rectangle.Empty;
            }
        }

        private void DataGridView_MouseMove(object sender, MouseEventArgs e)
        {
            // If the mouse moves outside the rectangle, start the drag
            if (e.Button == MouseButtons.Left)
            {
                if (dragBoxFromMouseDown != Rectangle.Empty && !dragBoxFromMouseDown.Contains(e.X, e.Y))
                {
                    // Proceed with the drag and drop, passing in the list item.
                    DragDropEffects dropEffects = DataGridView.DoDragDrop(
                        DataGridView.Rows[rowIndexFromMouseDown], DragDropEffects.Move);
                }
            }
        }
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member