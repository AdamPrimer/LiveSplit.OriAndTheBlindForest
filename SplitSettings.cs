using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LiveSplit.OriAndTheBlindForest {
	public partial class SplitSettings : UserControl {
		public SplitSettings() {
			InitializeComponent();
		}

		private void cboName_SelectedIndexChanged(object sender, EventArgs e) {
			bool isValue = cboName.SelectedValue.ToString().Equals("Value");
			txtValue.Visible = isValue;
			txtValue.Text = isValue ? "1" : "True";
		}
	}
}
