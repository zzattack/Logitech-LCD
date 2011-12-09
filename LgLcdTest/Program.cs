using System;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;
using LgLcd;

namespace LgLcdTest {

	class ExampleApplet : Applet, IDisposable {

		protected override void OnDeviceArrival(DeviceType deviceType) {
			MessageBox.Show("Device arrived " + deviceType.ToString());
			device.Open(this, DeviceType.Qvga);
			timer.Interval = 1000;
			timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
		}
		
		protected override void OnDeviceRemoval(DeviceType deviceType) {
			MessageBox.Show("Device removed " + deviceType.ToString());
		}

		protected override void OnAppletEnabled() {
			MessageBox.Show("Applet enabled");
			timer.Start();
		}

		protected override void OnAppletDisabled() {
			MessageBox.Show("Applet disabled");
			timer.Stop();
		}

		protected override void OnCloseConnection() {
			MessageBox.Show("Connection closed");
			timer.Stop();
		}

		protected override void OnConfigure() {
			MessageBox.Show("Configure");
		}

		void timer_Elapsed(object sender, ElapsedEventArgs e) {
			Bitmap bmp = (Bitmap)Bitmap.FromFile(@"..\..\qvga_sample" + counter++ + ".bmp");
			if (counter > 2)
				counter = 1;
			device.UpdateBitmap(bmp, Priority.Normal, false, false);
			device.SetAsLCDForegroundApp(true);
		}

		public void Dispose() {
			timer.Dispose();
		}

		private int counter = 1;
		private Device device = new Device();
		private System.Timers.Timer timer = new System.Timers.Timer();
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
