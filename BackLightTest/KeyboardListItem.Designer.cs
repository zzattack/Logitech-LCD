namespace BackLightTest
{
    partial class KeyboardListItem
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblKeyboardName = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnChangeColor = new System.Windows.Forms.LinkLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblKeyboardName
            // 
            this.lblKeyboardName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblKeyboardName.AutoSize = true;
            this.lblKeyboardName.Location = new System.Drawing.Point(7, 31);
            this.lblKeyboardName.Name = "lblKeyboardName";
            this.lblKeyboardName.Size = new System.Drawing.Size(472, 13);
            this.lblKeyboardName.TabIndex = 0;
            this.lblKeyboardName.Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec non sapien in mass" +
    "a porttitor tempor.";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(10, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(28, 24);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // btnChangeColor
            // 
            this.btnChangeColor.AutoSize = true;
            this.btnChangeColor.Location = new System.Drawing.Point(44, 9);
            this.btnChangeColor.Name = "btnChangeColor";
            this.btnChangeColor.Size = new System.Drawing.Size(50, 13);
            this.btnChangeColor.TabIndex = 2;
            this.btnChangeColor.TabStop = true;
            this.btnChangeColor.Text = "#FF00FF";
            this.btnChangeColor.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.btnChangeColor_LinkClicked);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 56);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(517, 1);
            this.panel1.TabIndex = 3;
            // 
            // KeyboardListItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnChangeColor);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblKeyboardName);
            this.Name = "KeyboardListItem";
            this.Size = new System.Drawing.Size(517, 57);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblKeyboardName;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.LinkLabel btnChangeColor;
        private System.Windows.Forms.Panel panel1;
    }
}
