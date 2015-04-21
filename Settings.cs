using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace LiveSplit.UI.Components
{
    public partial class OriAndTheBlindForestSettings : UserControl
    {
        Dictionary<string, string> splits = new Dictionary<string, string>() 
        {
            {"Start",              "Boolean"},
            {"Soul Flame",         "Boolean"},
            {"Spirit Flame",       "Boolean"},
            {"Wall Jump",          "Boolean"},
            {"Charge Flame",       "Boolean"},
            {"Double Jump",        "Boolean"},
            {"Gumo Free",          "Boolean"},
            {"Water Vein",         "Boolean"},
            {"Ginso Tree Entered", "Boolean"},
            {"Bash",               "Boolean"},
            {"Clean Water",        "Boolean"},
            {"Stomp",              "Boolean"},
            {"Glide",              "Boolean"},
            {"Sunstone",           "Boolean"},
            {"Mount Horu Entered", "Boolean"},
            {"Warmth Returned",    "Boolean"},
            {"End",                "Boolean"},
            {"Health Cells",       "Value"},
        };

        public List<Devil.Split> SplitsState = new List<Devil.Split>();
        public List<Devil.Split> tempSplitsState = new List<Devil.Split>();

        public OriComponent parent;

        public OriAndTheBlindForestSettings(OriComponent component) {
            InitializeComponent();
            parent = component;
        }

        private bool isLoading = false;

        private void write(string str) {
            StreamWriter wr = new StreamWriter("test.log", true);
            wr.WriteLine("[" + DateTime.Now + "] " + str);
            wr.Close();
        }

        public void LoadSettings() {
            write("LoadSettings();");
            foreach (var split in SplitsState) {
                write(split.name);
            }

            isLoading = true;
            splitGrid.DataSource = null;
            splitGrid.Rows.Clear();
            foreach (var split in SplitsState) {
                string name = split.name;
                string type = splits[name];
                string value = split.value;

                int idx = splitGrid.Rows.Add(name);

                if (type == "Value") {
                    DataGridViewHelper.createInputBox(splitGrid, 1, idx, value);
                } else if (type == "Boolean") {
                    DataGridViewHelper.createBooleanComboBox(splitGrid, 1, idx, value);
                }
            }
            isLoading = false;
        }

        public void UpdateSettings() {
            if (isLoading) return;
            write("UpdateSettings();");

            SplitsState.Clear();
            foreach (DataGridViewRow row in splitGrid.Rows) {
                if (row != null && row.Cells["SplitName"] != null && row.Cells["SplitName"].Value != null && row.Cells["Value"].Value != null) {
                    string name = row.Cells["SplitName"].Value.ToString();
                    string value = row.Cells["Value"].Value.ToString();

                    Devil.Split split = new Devil.Split();
                    split.name = name;
                    split.value = value;

                    SplitsState.Add(split);
                }
            }
            
            parent.oriState.InitializeTriggers(SplitsState, parent.OnSplit);
        }

        public XmlNode GetSettings(XmlDocument document) {
            var settingsNode = document.CreateElement("Settings");

            var splitsNode = document.CreateElement("Splits");
            settingsNode.AppendChild(splitsNode);
            
            foreach (var split in SplitsState) {
                string name = split.name;
                string value = split.value;

                var splitNode = document.CreateElement("Split");
                splitNode.InnerText = name;

                var valueAttribute = document.CreateAttribute("Value");
                valueAttribute.Value = value;
                splitNode.Attributes.Append(valueAttribute);

                splitsNode.AppendChild(splitNode);
            }

            return settingsNode;
        }

        public void SetSettings(XmlNode settings) {
            write(settings.FirstChild.Name);

            XmlNodeList splitNodes = settings.SelectNodes("//Splits/Split");

            write(splitNodes.Count.ToString());
            SplitsState.Clear();
            foreach (XmlNode splitNode in splitNodes) {
                string name = splitNode.InnerText;
                string value = splitNode.Attributes["Value"].Value;

                Devil.Split split = new Devil.Split();
                split.name = name;
                split.value = value;

                SplitsState.Add(split);
            }
        }

        public DataTable SplitComboData() {
            DataTable dt = new DataTable();
            dt.Columns.Add("SplitName", typeof(string));
            dt.Columns.Add("Type", typeof(string));
            foreach (var pair in splits) {
                dt.Rows.Add(pair.Key, pair.Value);
            }
            return dt;
        }

        private void Settings_Load(object sender, EventArgs e) {
            write("Settings_Load();");
            DataTable dt = SplitComboData();

            DataGridViewComboBoxColumn col = new DataGridViewComboBoxColumn() {
                MinimumWidth = 150,
                AutoComplete = true,
                Name = "SplitName",
                DataSource = dt,
                DisplayMember = "SplitName",
                ValueMember = "SplitName",
                DataPropertyName = "SplitName",
                HeaderText = "Name",
                FlatStyle = System.Windows.Forms.FlatStyle.Flat,
            };

            DataGridViewColumn col2 = new DataGridViewComboBoxColumn() {
                MinimumWidth = 75,
                Name = "Value",
                HeaderText = "Value",
                FlatStyle = System.Windows.Forms.FlatStyle.Flat,
            };

            DataGridViewButtonColumn col3 = new DataGridViewButtonColumn() {
                Name = "btnMoveUp",
                MinimumWidth = 50,
                Text = "Up",
                HeaderText = "Up",
                UseColumnTextForButtonValue = true,
                FlatStyle = System.Windows.Forms.FlatStyle.Flat,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
            };

            DataGridViewButtonColumn col4 = new DataGridViewButtonColumn() {
                Name = "btnMoveDown",
                MinimumWidth = 50,
                Text = "Down",
                HeaderText = "Down",
                UseColumnTextForButtonValue = true,
                FlatStyle = System.Windows.Forms.FlatStyle.Flat,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
            };

            DataGridViewButtonColumn col5 = new DataGridViewButtonColumn() {
                Name = "btnRemove",
                MinimumWidth = 50,
                Text = "Remove",
                HeaderText = "Remove",
                UseColumnTextForButtonValue = true,
                FlatStyle = System.Windows.Forms.FlatStyle.Flat,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
            };

            col.DefaultCellStyle.SelectionForeColor = Color.Black;
            col.DefaultCellStyle.SelectionBackColor = Color.White;

            splitGrid.Columns.Add(col);
            splitGrid.Columns.Add(col2);
            splitGrid.Columns.Add(col3);
            splitGrid.Columns.Add(col4);
            splitGrid.Columns.Add(col5);

            splitGrid.CurrentCellDirtyStateChanged += new EventHandler(splitGrid_CurrentCellDirtyStateChanged);
            splitGrid.CellValueChanged += new DataGridViewCellEventHandler(splitGrid_CellValueChanged);
            splitGrid.CellContentClick += new DataGridViewCellEventHandler(splitGrid_CellContentClick);

            splitGrid.Width = splitGrid.Columns.Cast<DataGridViewColumn>().Sum(x => x.Width)
                + (splitGrid.RowHeadersVisible ? splitGrid.RowHeadersWidth : 0) + 20;

            splitGrid.GridColor = Color.BlueViolet;

            LoadSettings();
        }


        private void splitGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
            if (splitGrid.CurrentCell != null && splitGrid.Columns[e.ColumnIndex].Name == "SplitName") {
                try {
                    string name = splitGrid.CurrentCell.EditedFormattedValue.ToString();
                    string type = splits[name];
                    int idx = splitGrid.CurrentRow.Index;

                    if (type == "Boolean") {
                        DataGridViewHelper.createBooleanComboBox(splitGrid, 1, idx, "True");
                    } else if (type == "Value") {
                        DataGridViewHelper.createInputBox(splitGrid, 1, idx, "1");
                    }
                } catch (Exception ex) {
                    Console.WriteLine(ex.ToString());
                }
            }

            if (splitGrid.CurrentCell != null && (splitGrid.Columns[e.ColumnIndex].Name == "SplitName" || splitGrid.Columns[e.ColumnIndex].Name == "Value")) {
                UpdateSettings();
            }
        }

        private void splitGrid_CurrentCellDirtyStateChanged(object sender, EventArgs e) {
            if (splitGrid.IsCurrentCellDirty) {
                splitGrid.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void splitGrid_CellContentClick(object sender, DataGridViewCellEventArgs e) {
            var senderGrid = (DataGridView)sender;

            // Handle Buttons
            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0) {
                switch (senderGrid.Columns[e.ColumnIndex].Name) {
                    case "btnMoveUp":
                        DataGridViewHelper.MoveRowUp(senderGrid);
                        UpdateSettings();
                        break;
                    case "btnMoveDown":
                        DataGridViewHelper.MoveRowDown(senderGrid);
                        UpdateSettings();
                        break;
                    case "btnRemove":
                        DataGridViewHelper.RemoveRow(senderGrid);
                        UpdateSettings();
                        break;
                }
            }
        }
    }

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
            ComboBoxCell.Items.AddRange(new string[] {"True", "False"});
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
