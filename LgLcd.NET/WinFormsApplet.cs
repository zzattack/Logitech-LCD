using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace LgLcd {

	/// <summary>
	/// Very generic applet based on a UserControl. The implementer
	/// can simply use the Designer to generate a UserControl which
	/// using WinFormsApplet's functionality gets rendered off-screen and
	/// pushed to the device
	/// </summary>
	[TypeDescriptionProvider(typeof(ConcreteClassProvider))]
	public abstract class WinFormsApplet : UserControl, IApplet {
		protected Device device;
		private Applet applet;

		protected WinFormsApplet() {
			bool designMode = (LicenseManager.UsageMode == LicenseUsageMode.Designtime);
			if (designMode) return;

			this.UpdateLcdScreen += new EventHandler(WinFormsApplet_UpdateLcdScreen);

			applet = new AppletProxy(this);
			Connect(AppletName, true, AppletCapabilities.Qvga);

			device = new Device();
			device.Open(applet, DeviceType.Qvga);
		}

		/// <summary>
		/// Forces deriving classes to implement a callback for when the screen needs to be updated
		/// </summary>
		public abstract event EventHandler UpdateLcdScreen;

		// called when deriving FormApplet requests the lcd to be repainted
		Bitmap bm = new Bitmap(320, 240, PixelFormat.Format32bppArgb);
		void WinFormsApplet_UpdateLcdScreen(object sender, EventArgs e) {
			lock (bm)
			{
				this.DrawToBitmap(bm, this.ClientRectangle);
			}
			device.UpdateBitmap(bm, Priority.Normal);
		}
		
		public abstract void OnDeviceArrival(DeviceType deviceType);
		public abstract void OnDeviceRemoval(DeviceType deviceType);
		public abstract void OnAppletEnabled();
		public abstract void OnAppletDisabled();
		public abstract void OnCloseConnection();
		public abstract void OnConfigure();

		#region Applet proxy
		// @alex: I wanted to make it easier for the end user to just make a form,
		// and have them fire some event like UpdateLcd(), in which we use Control.DrawToBitmap()
		// and update the LCD for them.. 
		// Sadly.. without multiple inheritance, a proxy as below is our best bet
		// of getting the functionality from Form and Applet combined :(

		internal class AppletProxy : Applet {
			private readonly IApplet _proxy;
			public AppletProxy(IApplet proxy) {
				this._proxy = proxy;
			}
			public override void OnDeviceArrival(DeviceType deviceType) { _proxy.OnDeviceArrival(deviceType); }
			public override void OnDeviceRemoval(DeviceType deviceType) { _proxy.OnDeviceRemoval(deviceType); }
			public override void OnAppletEnabled() { _proxy.OnAppletEnabled(); }
			public override void OnAppletDisabled() { _proxy.OnAppletDisabled(); }
			public override void OnCloseConnection() { _proxy.OnCloseConnection(); }
			public override void OnConfigure() { _proxy.OnConfigure(); }
		}

		public void Connect(string friendlyName, bool autostartable, AppletCapabilities appletCaps) {
			applet.Connect(friendlyName, autostartable, appletCaps);
		}
		public void Disconnect() {
			applet.Disconnect();
		}

		#endregion

		public abstract string AppletName { get; }
	}
}
