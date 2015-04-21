using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LiveSplit.UI.Components
{
    public static class DataGridViewHelper
    {
        public static void createInputBox(DataGridView grid, int colIdx, int rowIdx, string value) {
            DataGridViewTextBoxCell TextBoxCell = new DataGridViewTextBoxCell() { };
            grid[colIdx, rowIdx] = TextBoxCell;
            grid[colIdx, rowIdx].Value = value;
        }

        public static void createBooleanComboBox(DataGridView grid, int colIdx, int rowIdx, string value) {
            DataGridViewComboBoxCell ComboBoxCell = new DataGridViewComboBoxCell() {
                FlatStyle = System.Windows.Forms.FlatStyle.Flat,
            };
            ComboBoxCell.Items.AddRange(new string[] { "True", "False" });
            grid[colIdx, rowIdx] = ComboBoxCell;
            grid[colIdx, rowIdx].Value = value;
        }

        public static void RemoveRow(DataGridView grid) {
            try {
                int totalRows = grid.Rows.Count;
                int idx = grid.SelectedCells[0].OwningRow.Index;
                DataGridViewRowCollection rows = grid.Rows;
                DataGridViewRow row = rows[idx];
                rows.Remove(row);
                grid.ClearSelection();
            } catch { }
        }

        public static void MoveRowUp(DataGridView grid) {
            try {
                int totalRows = grid.Rows.Count;
                int idx = grid.SelectedCells[0].OwningRow.Index;
                if (idx == 0)
                    return;
                int col = grid.SelectedCells[0].OwningColumn.Index;
                DataGridViewRowCollection rows = grid.Rows;
                DataGridViewRow row = rows[idx];
                rows.Remove(row);
                rows.Insert(idx - 1, row);
                grid.ClearSelection();
                grid.Rows[idx - 1].Cells[col].Selected = true;
                grid.ClearSelection();
            } catch { }
        }

        public static void MoveRowDown(DataGridView grid) {
            try {
                int totalRows = grid.Rows.Count;
                int idx = grid.SelectedCells[0].OwningRow.Index;
                if (idx == totalRows - 2)
                    return;
                int col = grid.SelectedCells[0].OwningColumn.Index;
                DataGridViewRowCollection rows = grid.Rows;
                DataGridViewRow row = rows[idx];
                rows.Remove(row);
                rows.Insert(idx + 1, row);
                grid.ClearSelection();
                grid.Rows[idx + 1].Cells[col].Selected = true;
                grid.ClearSelection();
            } catch { }
        }
    }
}
