namespace LiveSplit.OriAndTheBlindForest
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
            this.btnAnyPercentMisty = new System.Windows.Forms.Button();
            this.btnLowPercent = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowMain.SuspendLayout();
            this.flowDefaults.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn100Percent
            // 
            this.btn100Percent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btn100Percent.Location = new System.Drawing.Point(265, 3);
            this.btn100Percent.Name = "btn100Percent";
            this.btn100Percent.Size = new System.Drawing.Size(46, 21);
            this.btn100Percent.TabIndex = 2;
            this.btn100Percent.Text = "100%";
            this.btn100Percent.UseVisualStyleBackColor = true;
            // 
            // btnAllCells
            // 
            this.btnAllCells.Location = new System.Drawing.Point(202, 3);
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
            this.flowMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowMain.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowMain.Location = new System.Drawing.Point(0, 0);
            this.flowMain.Name = "flowMain";
            this.flowMain.Size = new System.Drawing.Size(373, 66);
            this.flowMain.TabIndex = 1;
            this.flowMain.WrapContents = false;
            // 
            // flowDefaults
            // 
            this.flowDefaults.AutoSize = true;
            this.flowDefaults.Controls.Add(this.lblDefaults);
            this.flowDefaults.Controls.Add(this.btnAnyPercent);
            this.flowDefaults.Controls.Add(this.btnAnyPercentMisty);
            this.flowDefaults.Controls.Add(this.btnAllCells);
            this.flowDefaults.Controls.Add(this.btn100Percent);
            this.flowDefaults.Controls.Add(this.btnLowPercent);
            this.flowDefaults.Location = new System.Drawing.Point(3, 3);
            this.flowDefaults.Name = "flowDefaults";
            this.flowDefaults.Size = new System.Drawing.Size(367, 27);
            this.flowDefaults.TabIndex = 0;
            this.flowDefaults.WrapContents = false;
            // 
            // btnAnyPercentMisty
            // 
            this.btnAnyPercentMisty.Location = new System.Drawing.Point(121, 3);
            this.btnAnyPercentMisty.Name = "btnAnyPercentMisty";
            this.btnAnyPercentMisty.Size = new System.Drawing.Size(75, 21);
            this.btnAnyPercentMisty.TabIndex = 4;
            this.btnAnyPercentMisty.Text = "Any% (Misty)";
            this.btnAnyPercentMisty.UseVisualStyleBackColor = true;
            // 
            // btnLowPercent
            // 
            this.btnLowPercent.Location = new System.Drawing.Point(317, 3);
            this.btnLowPercent.Name = "btnLowPercent";
            this.btnLowPercent.Size = new System.Drawing.Size(47, 21);
            this.btnLowPercent.TabIndex = 5;
            this.btnLowPercent.Text = "Low%";
            this.btnLowPercent.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.btnAddSplit);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 36);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(63, 27);
            this.flowLayoutPanel1.TabIndex = 7;
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
            this.Size = new System.Drawing.Size(373, 66);
            this.Load += new System.EventHandler(this.Settings_Load);
            this.flowMain.ResumeLayout(false);
            this.flowMain.PerformLayout();
            this.flowDefaults.ResumeLayout(false);
            this.flowDefaults.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
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
        private System.Windows.Forms.Button btnAnyPercentMisty;
        private System.Windows.Forms.Button btnLowPercent;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;

	}
}