using System;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;
using LgLcd;
using LgBackLight;

namespace LgLcdTest {
	
	class ExampleApplet : Applet, IDisposable {

		public override void OnDeviceArrival(DeviceType deviceType) {
			MessageBox.Show("Device arrived " + deviceType.ToString());
			device.Open(this, DeviceType.Qvga);
			timer.Interval = 1000;
			timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
		}

		public override void OnDeviceRemoval(DeviceType deviceType) {
			MessageBox.Show("Device removed " + deviceType.ToString());
		}

		public override void OnAppletEnabled() {
			MessageBox.Show("Applet enabled");
			timer.Start();
		}

		public override void OnAppletDisabled() {
			MessageBox.Show("Applet disabled");
			timer.Stop();
		}

		public override void OnCloseConnection() {
			MessageBox.Show("Connection closed");
			timer.Stop();
		}

		public override void OnConfigure() {
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
			Application.Run(new LgLcdTestForm());
			BackLight.SetBackLight(Color.Fuchsia);
		}
	}

}
