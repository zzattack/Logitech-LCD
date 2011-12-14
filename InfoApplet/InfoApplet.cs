using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LgLcd;
using System.Drawing;
using System.Diagnostics;

namespace InfoApplet {
	public partial class InfoApplet : WinFormsApplet 
	{

		public InfoApplet() {
			InitializeComponent();
			tmrUpdateScreen.Interval = 1000 - DateTime.Now.Millisecond;
			tmrUpdateScreen.Start();
		}

		public override string AppletName {
			get { return "InfoDisplay"; }
		}

		private void tmrUpdateScreen_Tick(object sender, EventArgs e) {
			tmrUpdateScreen.Stop();
			tmrUpdateScreen.Interval = 1000 - DateTime.Now.Millisecond; // resynchronize all the time
			tmrUpdateScreen.Start();
			this.lblTime.Text = DateTime.Now.ToString("HH:mm:ss");

			// request screen update
			UpdateLcdScreen(this, new EventArgs());
		}

		public override event EventHandler UpdateLcdScreen;

		public override void OnDeviceArrival(DeviceType deviceType) {}
		public override void OnDeviceRemoval(DeviceType deviceType) {}
		public override void OnAppletEnabled() {}
		public override void OnAppletDisabled() {}
		public override void OnCloseConnection() {}
		public override void OnConfigure() {}

	}
}
