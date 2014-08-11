namespace LgLcdTest
{
	partial class LgLcdTestForm
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
			this.lblSuccess = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lblSuccess
			// 
			this.lblSuccess.AutoSize = true;
			this.lblSuccess.Location = new System.Drawing.Point(45, 80);
			this.lblSuccess.Name = "lblSuccess";
			this.lblSuccess.Size = new System.Drawing.Size(89, 13);
			this.lblSuccess.TabIndex = 0;
			this.lblSuccess.Text = "Hurray, success!!";
			// 
			// LgLcdTestForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lblSuccess);
			this.Name = "LgLcdTestForm";
			this.Size = new System.Drawing.Size(320, 240);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblSuccess;
	}
}