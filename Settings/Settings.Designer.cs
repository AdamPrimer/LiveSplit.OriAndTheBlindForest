namespace LiveSplit.OriAndTheBlindForest.Settings
{
    partial class OriAndTheBlindForestSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.btn100Percent = new System.Windows.Forms.Button();
            this.btnAllCells = new System.Windows.Forms.Button();
            this.lblDefaults = new System.Windows.Forms.Label();
            this.btnAnyPercent = new System.Windows.Forms.Button();
            this.btnAddSplit = new System.Windows.Forms.Button();
            this.flowMain = new System.Windows.Forms.FlowLayoutPanel();
            this.flowDefaults = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAllSkills = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.chkAutoStart = new System.Windows.Forms.CheckBox();
            this.chkAutoReset = new System.Windows.Forms.CheckBox();
            this.chkShowMapDisplay = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.flowMain.SuspendLayout();
            this.flowDefaults.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn100Percent
            // 
            this.btn100Percent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btn100Percent.Location = new System.Drawing.Point(247, 3);
            this.btn100Percent.Name = "btn100Percent";
            this.btn100Percent.Size = new System.Drawing.Size(46, 21);
            this.btn100Percent.TabIndex = 2;
            this.btn100Percent.Text = "100%";
            this.btn100Percent.UseVisualStyleBackColor = true;
            // 
            // btnAllCells
            // 
            this.btnAllCells.Location = new System.Drawing.Point(184, 3);
            this.btnAllCells.Name = "btnAllCells";
            this.btnAllCells.Size = new System.Drawing.Size(57, 21);
            this.btnAllCells.TabIndex = 1;
            this.btnAllCells.Text = "All Cells";
            this.btnAllCells.UseVisualStyleBackColor = true;
            // 
            // lblDefaults
            // 
            this.lblDefaults.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblDefaults.AutoSize = true;
            this.lblDefaults.Location = new System.Drawing.Point(3, 7);
            this.lblDefaults.Name = "lblDefaults";
            this.lblDefaults.Size = new System.Drawing.Size(49, 13);
            this.lblDefaults.TabIndex = 3;
            this.lblDefaults.Text = "Defaults:";
            // 
            // btnAnyPercent
            // 
            this.btnAnyPercent.Location = new System.Drawing.Point(58, 3);
            this.btnAnyPercent.Name = "btnAnyPercent";
            this.btnAnyPercent.Size = new System.Drawing.Size(57, 21);
            this.btnAnyPercent.TabIndex = 0;
            this.btnAnyPercent.Text = "Any%";
            this.btnAnyPercent.UseVisualStyleBackColor = true;
            // 
            // btnAddSplit
            // 
            this.btnAddSplit.Location = new System.Drawing.Point(3, 3);
            this.btnAddSplit.Name = "btnAddSplit";
            this.btnAddSplit.Size = new System.Drawing.Size(57, 21);
            this.btnAddSplit.TabIndex = 0;
            this.btnAddSplit.Text = "Add Split";
            this.btnAddSplit.UseVisualStyleBackColor = true;
            this.btnAddSplit.Click += new System.EventHandler(this.btnAddSplit_Click);
            // 
            // flowMain
            // 
            this.flowMain.AutoSize = true;
            this.flowMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowMain.Controls.Add(this.flowDefaults);
            this.flowMain.Controls.Add(this.flowLayoutPanel1);
            this.flowMain.Controls.Add(this.flowLayoutPanel2);
            this.flowMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowMain.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowMain.Location = new System.Drawing.Point(0, 0);
            this.flowMain.Name = "flowMain";
            this.flowMain.Size = new System.Drawing.Size(396, 85);
            this.flowMain.TabIndex = 1;
            this.flowMain.WrapContents = false;
            // 
            // flowDefaults
            // 
            this.flowDefaults.AutoSize = true;
            this.flowDefaults.Controls.Add(this.lblDefaults);
            this.flowDefaults.Controls.Add(this.btnAnyPercent);
            this.flowDefaults.Controls.Add(this.btnAllSkills);
            this.flowDefaults.Controls.Add(this.btnAllCells);
            this.flowDefaults.Controls.Add(this.btn100Percent);
            this.flowDefaults.Location = new System.Drawing.Point(3, 3);
            this.flowDefaults.Name = "flowDefaults";
            this.flowDefaults.Size = new System.Drawing.Size(296, 27);
            this.flowDefaults.TabIndex = 0;
            this.flowDefaults.WrapContents = false;
            // 
            // btnAllSkills
            // 
            this.btnAllSkills.Location = new System.Drawing.Point(121, 3);
            this.btnAllSkills.Name = "btnAllSkills";
            this.btnAllSkills.Size = new System.Drawing.Size(57, 21);
            this.btnAllSkills.TabIndex = 4;
            this.btnAllSkills.Text = "All Skills";
            this.btnAllSkills.UseVisualStyleBackColor = true;
            this.btnAllSkills.Click += new System.EventHandler(this.btnAllSkills_Click_1);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.btnAddSplit);
            this.flowLayoutPanel1.Controls.Add(this.chkAutoStart);
            this.flowLayoutPanel1.Controls.Add(this.chkAutoReset);
            this.flowLayoutPanel1.Controls.Add(this.chkShowMapDisplay);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 36);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(390, 27);
            this.flowLayoutPanel1.TabIndex = 7;
            // 
            // chkAutoStart
            // 
            this.chkAutoStart.AutoSize = true;
            this.chkAutoStart.Location = new System.Drawing.Point(66, 3);
            this.chkAutoStart.Name = "chkAutoStart";
            this.chkAutoStart.Size = new System.Drawing.Size(103, 17);
            this.chkAutoStart.TabIndex = 1;
            this.chkAutoStart.Text = "Start on 1st Split";
            this.chkAutoStart.UseVisualStyleBackColor = true;
            this.chkAutoStart.CheckedChanged += new System.EventHandler(this.chkBox_CheckedChanged);
            // 
            // chkAutoReset
            // 
            this.chkAutoReset.AutoSize = true;
            this.chkAutoReset.Location = new System.Drawing.Point(175, 3);
            this.chkAutoReset.Name = "chkAutoReset";
            this.chkAutoReset.Size = new System.Drawing.Size(94, 17);
            this.chkAutoReset.TabIndex = 2;
            this.chkAutoReset.Text = "Reset on Start";
            this.chkAutoReset.UseVisualStyleBackColor = true;
            this.chkAutoReset.CheckedChanged += new System.EventHandler(this.chkBox_CheckedChanged);
            // 
            // chkShowMapDisplay
            // 
            this.chkShowMapDisplay.AutoSize = true;
            this.chkShowMapDisplay.Location = new System.Drawing.Point(275, 3);
            this.chkShowMapDisplay.Name = "chkShowMapDisplay";
            this.chkShowMapDisplay.Size = new System.Drawing.Size(112, 17);
            this.chkShowMapDisplay.TabIndex = 3;
            this.chkShowMapDisplay.Text = "Map% Display test";
            this.chkShowMapDisplay.UseVisualStyleBackColor = true;
            this.chkShowMapDisplay.CheckedChanged += new System.EventHandler(this.chkBox_CheckedChanged);
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.Controls.Add(this.label1);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(218, 69);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(175, 13);
            this.flowLayoutPanel2.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(169, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Check these boxes to NOT SPLIT";
            // 
            // OriAndTheBlindForestSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.flowMain);
            this.Name = "OriAndTheBlindForestSettings";
            this.Size = new System.Drawing.Size(396, 85);
            this.Load += new System.EventHandler(this.Settings_Load);
            this.flowMain.ResumeLayout(false);
            this.flowMain.PerformLayout();
            this.flowDefaults.ResumeLayout(false);
            this.flowDefaults.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn100Percent;
        private System.Windows.Forms.Button btnAllCells;
        private System.Windows.Forms.Button btnAnyPercent;
        private System.Windows.Forms.Label lblDefaults;
        private System.Windows.Forms.Button btnAddSplit;
        private System.Windows.Forms.FlowLayoutPanel flowMain;
        private System.Windows.Forms.FlowLayoutPanel flowDefaults;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.CheckBox chkAutoStart;
        private System.Windows.Forms.CheckBox chkAutoReset;
        private System.Windows.Forms.CheckBox chkShowMapDisplay;
        private System.Windows.Forms.Button btnAllSkills;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label label1;
    }
}