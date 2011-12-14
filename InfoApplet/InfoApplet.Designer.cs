using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LgLcd;
using System.Drawing;
using System.Diagnostics;

namespace InfoApplet {
	public partial class InfoApplet : WinFormsApplet {
		private Timer tmrUpdateScreen;
		private Label lblTime;
		private System.ComponentModel.IContainer components;

		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.tmrUpdateScreen = new System.Windows.Forms.Timer(this.components);
			this.lblTime = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// tmrUpdateScreen
			// 
			this.tmrUpdateScreen.Tick += new System.EventHandler(this.tmrUpdateScreen_Tick);
			// 
			// lblTime
			// 
			this.lblTime.BackColor = System.Drawing.Color.Transparent;
			this.lblTime.Font = new System.Drawing.Font("Tahoma", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblTime.ForeColor = System.Drawing.Color.Azure;
			this.lblTime.Location = new System.Drawing.Point(14, 80);
			this.lblTime.Name = "lblTime";
			this.lblTime.Size = new System.Drawing.Size(277, 92);
			this.lblTime.TabIndex = 0;
			this.lblTime.Text = "label1";
			this.lblTime.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// pictureBox1
			// 
			this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
			this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox1.Image = global::InfoApplet.Properties.Resources.background;
			this.pictureBox1.Location = new System.Drawing.Point(0, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(320, 240);
			this.pictureBox1.TabIndex = 1;
			this.pictureBox1.TabStop = false;
			// 
			// InfoApplet
			// 
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.lblTime);
			this.Controls.Add(this.pictureBox1);
			this.Name = "InfoApplet";
			this.Size = new System.Drawing.Size(320, 240);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);

		}

		private PictureBox pictureBox1;
	}
}
