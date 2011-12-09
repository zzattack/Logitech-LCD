using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using LgLcd;

namespace LgLcdTest {

	class ExampleApplet : Applet {

		protected override void OnDeviceArrival(DeviceType deviceType) {
			MessageBox.Show("Device arrived " + deviceType.ToString());
			// Create and open a device
			Device device = new Device();
			device.Open(this, DeviceType.Qvga);
			// Try display sample bitmap
			Bitmap bmp = (Bitmap)Bitmap.FromFile(@"..\..\qvga_sample.bmp");
			device.UpdateBitmap(bmp as Bitmap, Priority.Normal, false, false);
		}

		protected override void OnDeviceRemoval(DeviceType deviceType) {
			MessageBox.Show("Device removed " + deviceType.ToString());
		}

		protected override void OnAppletEnabled() {
			MessageBox.Show("Applet enabled");
		}

		protected override void OnAppletDisabled() {
			MessageBox.Show("Applet disabled");
		}

		protected override void OnCloseConnection() {
			MessageBox.Show("Connection closed");
		}

		protected override void OnConfigure() {
			MessageBox.Show("Configure");
		}

	}
	
	class Program {
		static void Main(string[] args) {
			// Create and connect applet
			ExampleApplet applet = new ExampleApplet();
			applet.Connect("My Applet", false, AppletCapabilities.Qvga);
			System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
		}
	}

}
