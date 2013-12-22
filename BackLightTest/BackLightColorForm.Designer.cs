namespace BackLightTest
{
    partial class BackLightColorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmbKeyboards = new System.Windows.Forms.ComboBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblKeyboards = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblKeyboardCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.lstKeyboards = new System.Windows.Forms.FlowLayoutPanel();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbKeyboards
            // 
            this.cmbKeyboards.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbKeyboards.FormattingEnabled = true;
            this.cmbKeyboards.Items.AddRange(new object[] {
            "G19",
            "G510"});
            this.cmbKeyboards.Location = new System.Drawing.Point(13, 13);
            this.cmbKeyboards.Name = "cmbKeyboards";
            this.cmbKeyboards.Size = new System.Drawing.Size(602, 21);
            this.cmbKeyboards.TabIndex = 0;
            this.cmbKeyboards.SelectedIndexChanged += new System.EventHandler(this.cmbKeyboards_SelectedIndexChanged);
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Enabled = false;
            this.btnSearch.Location = new System.Drawing.Point(622, 12);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(79, 23);
            this.btnSearch.TabIndex = 1;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblKeyboards,
            this.lblKeyboardCount});
            this.statusStrip1.Location = new System.Drawing.Point(0, 364);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(713, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblKeyboards
            // 
            this.lblKeyboards.Name = "lblKeyboards";
            this.lblKeyboards.Size = new System.Drawing.Size(105, 17);
            this.lblKeyboards.Text = "Keyboards Found: ";
            // 
            // lblKeyboardCount
            // 
            this.lblKeyboardCount.Name = "lblKeyboardCount";
            this.lblKeyboardCount.Size = new System.Drawing.Size(13, 17);
            this.lblKeyboardCount.Text = "0";
            // 
            // lstKeyboards
            // 
            this.lstKeyboards.AutoScroll = true;
            this.lstKeyboards.AutoSize = true;
            this.lstKeyboards.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstKeyboards.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.lstKeyboards.Location = new System.Drawing.Point(13, 41);
            this.lstKeyboards.Name = "lstKeyboards";
            this.lstKeyboards.Size = new System.Drawing.Size(688, 320);
            this.lstKeyboards.TabIndex = 3;
            this.lstKeyboards.WrapContents = false;
            // 
            // BackLightColorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(713, 386);
            this.Controls.Add(this.lstKeyboards);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.cmbKeyboards);
            this.Name = "BackLightColorForm";
            this.Text = "Logitech Keyboard Color Changer";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbKeyboards;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblKeyboards;
        private System.Windows.Forms.ToolStripStatusLabel lblKeyboardCount;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.FlowLayoutPanel lstKeyboards;
    }
}

