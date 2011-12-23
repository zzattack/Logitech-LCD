using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Diagnostics;

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
			// we don't want the designer to really call this constructor
			if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;

			bm = new Bitmap(320, 240, PixelFormat.Format32bppArgb);
			gfx = Graphics.FromImage(bm);
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

		Stopwatch sw = new Stopwatch();
		void WinFormsApplet_UpdateLcdScreen(object sender, EventArgs e) {
			//sw.Reset();
			//sw.Start();
			DrawToBitmap2();
			//System.Diagnostics.Debug.WriteLine("DrawToBitmap took " + sw.ElapsedMilliseconds.ToString() + "ms");
			try {
				device.UpdateBitmap(bm, Priority.Normal);
			}
			catch { }
		}

		/// <summary>
		/// somewhat faster version of DrawToBitmap,
		/// directly specifying a DC to the bitmap instead of 
		/// generating a compatible one and then blitting
		/// </summary>
		private void DrawToBitmap2() {
			if (!this.IsHandleCreated)
				this.CreateHandle();

			IntPtr hdc = gfx.GetHdc();
			SendMessage(new HandleRef(this, this.Handle), 0x317, hdc, (IntPtr)30);
			gfx.ReleaseHdc(hdc);
		}
		Bitmap bm;
		Graphics gfx;

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		static extern IntPtr SendMessage(HandleRef hWnd, int msg, IntPtr wParam, IntPtr lParam);

		#region abstracted interface methods

		public virtual void OnDeviceArrival(DeviceType deviceType) { }
		public virtual void OnDeviceRemoval(DeviceType deviceType) { }
		public virtual void OnAppletEnabled() { }
		public virtual void OnAppletDisabled() { }
		public virtual void OnCloseConnection() { }
		public virtual void OnConfigure() { }
		public abstract string AppletName { get; }

		#endregion

		#region Applet proxy
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

	}
}
