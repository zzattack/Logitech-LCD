using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LgLcd;
using System.Drawing;
using System.Diagnostics;
using System.Management;

namespace InfoApplet {
	public partial class InfoApplet : WinFormsApplet {

		public InfoApplet() {
			InitializeComponent();
			tmrUpdateScreen.Interval = 1000 - DateTime.Now.Millisecond;
			tmrUpdateScreen.Start();
			
			ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from win32_processor");
			foreach (var o in searcher.Get()) {
				foreach (var prop in o.Properties) {
					Debug.WriteLine(string.Format("{0}: {1}", prop.Name, prop.Value));
				}
			}

			device.SetAsLCDForegroundApp(true);
		}

		public override string AppletName {
			get { return "InfoDisplay"; }
		}

		private void tmrUpdateScreen_Tick(object sender, EventArgs e) {

			tmrUpdateScreen.Stop();
			tmrUpdateScreen.Interval = 1000 - DateTime.Now.Millisecond; // resynchronize all the time
			tmrUpdateScreen.Start();
			this.lblTime.Text = DateTime.Now.ToString("HH:mm:ss");
			Invalidate();

			// request screen update
			UpdateLcdScreen(this, new EventArgs());
		}

		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
		}

		public override event EventHandler UpdateLcdScreen;
		public override void OnDeviceArrival(DeviceType deviceType) { }
		public override void OnDeviceRemoval(DeviceType deviceType) { }
		public override void OnAppletEnabled() { }
		public override void OnAppletDisabled() { }
		public override void OnCloseConnection() { }
		public override void OnConfigure() { }

	}
}
