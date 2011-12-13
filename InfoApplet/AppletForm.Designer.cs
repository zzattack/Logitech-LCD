namespace InfoApplet
{
	partial class AppletForm
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
			this.components = new System.ComponentModel.Container();
			this.tmrUpdateScreen = new System.Windows.Forms.Timer(this.components);
			this.lblTime = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// tmrUpdateScreen
			// 
			this.tmrUpdateScreen.Interval = 1000;
			this.tmrUpdateScreen.Tick += new System.EventHandler(this.tmrUpdateScreen_Tick);
			// 
			// lblTime
			// 
			this.lblTime.BackColor = System.Drawing.Color.Transparent;
			this.lblTime.Font = new System.Drawing.Font("Quartz MS", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblTime.ForeColor = System.Drawing.Color.MintCream;
			this.lblTime.Location = new System.Drawing.Point(3, 69);
			this.lblTime.Name = "lblTime";
			this.lblTime.Size = new System.Drawing.Size(314, 72);
			this.lblTime.TabIndex = 0;
			this.lblTime.Text = "label1";
			this.lblTime.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// AppletForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImage = global::InfoApplet.Properties.Resources.background;
			this.ClientSize = new System.Drawing.Size(320, 240);
			this.Controls.Add(this.lblTime);
			this.Name = "AppletForm";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Timer tmrUpdateScreen;
		private System.Windows.Forms.Label lblTime;

	}
}
