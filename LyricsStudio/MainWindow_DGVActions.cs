#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.ComponentModel;
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

        private void DataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // fire ListChanged() event immediately
            // this will fire this event twice, but there is no other way to update when cells in same row are selected continuously.
            ListChangedType type;
            if (e.RowIndex >= lyrics.Count) type = ListChangedType.ItemAdded;
            else type = ListChangedType.ItemChanged;

            ListChangedEventArgs args = new(type, e.RowIndex);
            dataSource.DefaultView_ListChanged(sender, args);
        }

        // action when new column has added
        // currently used only to set column NotSortable
        private void DataGridView_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            // disable sorting of the column
            DataGridView.Columns[e.Column.Index].SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        // define drag & drop action of DataGridView
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
                // swap two items
                dataSource.Move(rowIndexFromMouseDown, rowIndexOfItemUnderMouseToDrop);
                
                // selected item moved to new position
                DataGridView.ClearSelection();
                DataGridView.Rows[rowIndexOfItemUnderMouseToDrop].Selected = true;
            }
        }

        // define drag & drop action of DataGridView
        // ref: https://stackoverflow.com/a/1623968
        private void DataGridView_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        // action when mouse has clicked
        private void DataGridView_MouseDown(object sender, MouseEventArgs e)
        {
            // show right click context menu
            if (e.Button == MouseButtons.Right)
            {
                // get Row and Column by mouse position
                int posRow = DataGridView.HitTest(e.X, e.Y).RowIndex;
                int posCol = DataGridView.HitTest(e.X, e.Y).ColumnIndex;

                // clear current selection
                //DataGridView.ClearSelection();

                // check where user has selected
                if (posCol != -1 && posRow != -1)
                {
                    // user selected single cell
                    DataGridView.Rows[posRow].Cells[posCol].Selected = true;
                }
                else if (posCol == -1 && posRow != -1)
                {
                    // user selected single row
                    DataGridView.Rows[posRow].Selected = true;
                }
                else if (posCol != -1 && posRow == -1)
                {
                    // user selected single column
                    DataGridView.Columns[posCol].Selected = true;
                }
            }
        }

        private void DataGridView_MouseDownDragDrop(object sender, MouseEventArgs e)
        {
            // define drag & drop action of DataGridView
            // ref: https://stackoverflow.com/a/1623968
            if (e.Button == MouseButtons.Left)
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
        }

        // define drag & drop action of DataGridView
        // ref: https://stackoverflow.com/a/1623968
        private void DataGridView_MouseMoveDragDrop(object sender, MouseEventArgs e)
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

        // marker to check dragging rows
        bool dragging = false;
        // drag & drop events
        MouseEventHandler dragDropDownEvent;
        MouseEventHandler dragDropMoveEvent;
        

        // action when keyboard has pressed
        private void DataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            // check if delete key is pressed
            if (e.KeyCode == Keys.Delete)
            {
                // ignore when current selection is row selection
                if (DataGridView.SelectedRows.Count > 0) return;
                // empty selected cell
                EmptyCells();
            // check if control key is pressed
            }
            else if (e.KeyCode == Keys.Control || e.KeyCode == Keys.ControlKey ||
                e.KeyCode == Keys.LControlKey || e.KeyCode == Keys.RControlKey)
            {
                // check if dragging is already enabled
                if (dragging == false)
                {
                    // bind new event to variable
                    dragDropDownEvent = new(DataGridView_MouseDownDragDrop);
                    dragDropMoveEvent = new(DataGridView_MouseMoveDragDrop);
                    // register drag & drop event
                    DataGridView.MouseDown += dragDropDownEvent;
                    DataGridView.MouseMove += dragDropMoveEvent;
                    dragging = true;
                }
            }
        }

        private void DataGridView_KeyUp(object sender, KeyEventArgs e)
        {
            // check if control key is unpressed
            if (e.KeyCode == Keys.Control || e.KeyCode == Keys.ControlKey ||
                e.KeyCode == Keys.LControlKey || e.KeyCode == Keys.RControlKey)
            {
                // check if dragging is already enabled
                if (dragging == true)
                {
                    // unregister drag & drop event
                    DataGridView.MouseDown -= dragDropDownEvent;
                    DataGridView.MouseMove -= dragDropMoveEvent;
                    dragging = false;
                }
            }
        }

        // action when insert line menu item has clicked
        private void insertLineMenuItem_Click(object sender, EventArgs e)
        {
            // Get the index of the item the mouse is below.
            int selectedRow = DataGridView.Rows.GetFirstRow(DataGridViewElementStates.Selected);

            // do not countinue when index is out of bounds
            if (selectedRow > lyrics.Count - 1) return;

            // add new row below the selected one
            dataSource.Insert(selectedRow+1, new LyricData());
            // select newly created row
            DataGridView.ClearSelection();
            DataGridView.Rows[selectedRow+1].Selected = true;
        }

        // action when empty line menu item has clicked
        private void emptyLineMenuItem_Click(object sender, EventArgs e) => EmptyCells();

        // action when delete line menu item has clicked
        private void deleteLineMenuItem_Click(object sender, EventArgs e)
        {
            // Get the index of the item the mouse is below.
            int selectedRow = DataGridView.Rows.GetFirstRow(DataGridViewElementStates.Selected);

            // ignore deletion when selected row is out of bounds
            if (selectedRow >= lyrics.Count) return;

            // remove selected row
            DataGridView.Rows.RemoveAt(selectedRow);

            // change row selection to change focus
            // check if previous selection was first row
            if (selectedRow != 0) selectedRow--;

            // select row right above deleted one
            DataGridView.ClearSelection();
            DataGridView.Rows[selectedRow].Selected = true;
        }

        //
        private void pushTimeColumnMenuItem_Click(object sender, EventArgs e)
        {
            // append new time column
            dataSource.PushTimeColumn();

            // reset position of newly added time column
            DataGridView.Columns[DataGridView.Columns.Count - 2].DisplayIndex = DataGridView.Columns.Count - 2;

            // copy cell width from first time column
            DataGridView.Columns[DataGridView.Columns.Count - 2].Width = DataGridView.Columns[0].Width;
        }

        // action when delete last entry in time column menu item has clicked
        private void PopTimeColumnMenuItem_Click(object sender, EventArgs e)
        {
            // do not continue when current column is the only one
            if (DataGridView.Columns.Count <= 2) return;

            // remove last time column
            dataSource.PopTimeColumn();
        }

        /// <summary>
        /// Empty the content of selected cellx.
        /// </summary>
        private void EmptyCells()
        {
            DataGridViewSelectedCellCollection cells = DataGridView.SelectedCells;
            for (int i = cells.Count - 1; i >= 0; i--) {
                if (string.IsNullOrEmpty(cells[i].Value.ToString())) continue;
                dataSource.EmptyCell(cells[i].ColumnIndex, cells[i].RowIndex);
            }
        }
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member