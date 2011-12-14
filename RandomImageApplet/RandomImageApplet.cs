using System.IO;
using System.Net;
using System.Drawing;
using LgLcd;
using System;
using System.Threading;

namespace RandomImageApplet {

	// example of a simple Applet,
	// compare to FormApplet

	class RandomImageApplet : Applet {

		private WebClient wc;
		private readonly static Uri url = new Uri(
			@"http://a.random-image.net/handler.aspx?username=dahlia&randomizername=sailormoonland2&random=1507.626&fromrandomrandomizer=yes");
		private Device device;

		public RandomImageApplet() {
			// setup webclient and callback
			wc = new WebClient();
			wc.DownloadDataCompleted += new DownloadDataCompletedEventHandler(wc_DownloadDataCompleted);
			wc.Proxy = null;

			// register applet with logitech gaming softwware
			this.Connect("Random Image", true, AppletCapabilities.Qvga);

			// register device with applet
			this.device = new Device();
			device.Open(this, DeviceType.Qvga);
		}

		private Timer scheduler; // keep reference to prevent early GC
		void wc_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e) {
			if (e.Cancelled == false && e.Error == null) {
				try {
					using (Bitmap bm = new Bitmap(new MemoryStream(e.Result))) {
						device.UpdateBitmap(bm.GetThumbnailImage(320, 240, null, IntPtr.Zero) as Bitmap, Priority.Normal);
					}
				}
				catch {
					// shitty image, too bad
				}
			}

			// schedule next image refresh in 3 seconds
			scheduler = new System.Threading.Timer(delegate(object s) {
				RefreshImage();
			}, null, new TimeSpan(30000000), new TimeSpan(-1));

		}

		private void RefreshImage() {
			wc.DownloadDataAsync(url);
		}


		public override void OnDeviceArrival(DeviceType deviceType) {
			RefreshImage();
		}

		public override void OnDeviceRemoval(DeviceType deviceType) { }
		public override void OnAppletEnabled() { }
		public override void OnAppletDisabled() { }
		public override void OnCloseConnection() { }
		public override void OnConfigure() { }
	}
}
