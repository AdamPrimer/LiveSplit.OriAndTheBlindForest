using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LiveSplit.OriAndTheBlindForest
{
    public partial class SplitSettings : UserControl
    {
        public string ControlType = "";

        public SplitSettings() {
            InitializeComponent();
        }

        private void cboName_SelectedIndexChanged(object sender, EventArgs e) {
            bool isValue = cboName.SelectedValue.ToString().Equals("Value");
            bool isHitbox = cboName.SelectedValue.ToString().Equals("Hitbox");
            txtValue.Visible = isValue || isHitbox;

            int hitboxTextWidth = 130;

            if (ControlType == "Hitbox") {
                txtValue.Width -= hitboxTextWidth;
                btnDown.Left -= hitboxTextWidth;
                btnRemove.Left -= hitboxTextWidth;
                btnUp.Left -= hitboxTextWidth;
            }

            this.ControlType = cboName.SelectedValue.ToString();

            if (isValue) {
                txtValue.Text = "1";
            } else if (isHitbox) {
                txtValue.Text = "";
                txtValue.Focus();
                txtValue.Width += hitboxTextWidth;
                btnDown.Left += hitboxTextWidth;
                btnRemove.Left += hitboxTextWidth;
                btnUp.Left += hitboxTextWidth;
            } else { 
                txtValue.Text = "True";
            }
        }
    }
}
