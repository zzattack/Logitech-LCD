using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace InfoApplet {
	public partial class AppletForm : UserControl {
		public AppletForm() {
			InitializeComponent();
			tmrUpdateScreen.Interval = 1000 - DateTime.Now.Millisecond;
			tmrUpdateScreen.Start();
		}

		private void tmrUpdateScreen_Tick(object sender, EventArgs e) {
			tmrUpdateScreen.Stop();
			tmrUpdateScreen.Interval = 1000 - DateTime.Now.Millisecond; // resynchronize all the time
			tmrUpdateScreen.Start();
			this.lblTime.Text = DateTime.Now.ToString("HH:mm:ss");

			this.Invalidate(this.ClientRectangle, true);
			this.Update();

			Debug.WriteLine("next second");

			// OnPrint(new PaintEventArgs(this.CreateGraphics(), this.ClientRectangle));
		}

		protected override void OnPaint(PaintEventArgs e) {
			int i = 0;
		}

		protected override void OnPrint(PaintEventArgs e) {
			base.OnPrint(e);
		}

	}
}
