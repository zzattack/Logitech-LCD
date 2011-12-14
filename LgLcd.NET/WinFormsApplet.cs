using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

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
		System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
		void WinFormsApplet_UpdateLcdScreen(object sender, EventArgs e) {
			sw.Reset();
			sw.Start();
			DrawToBitmap2(bm, this.ClientRectangle);
			System.Diagnostics.Debug.WriteLine("DrawToBitmap took " + sw.ElapsedMilliseconds.ToString() + "ms");
			device.UpdateBitmap(bm, Priority.Normal);
		}

		private void DrawToBitmap2(Bitmap bitmap, Rectangle targetBounds) {
			if (bitmap == null)
				throw new ArgumentNullException("bitmap");
			if (targetBounds.Width <= 0 || targetBounds.Height <= 0 || targetBounds.X < 0 || targetBounds.Y < 0)
				throw new ArgumentException("targetBounds");
			if (!this.IsHandleCreated)
				this.CreateHandle();

			int width = Math.Min(this.Width, targetBounds.Width);
			int height = Math.Min(this.Height, targetBounds.Height);
			Bitmap image = new Bitmap(width, height, bitmap.PixelFormat);
			using (Graphics graphics = Graphics.FromImage(bitmap)) {
				IntPtr hdc = graphics.GetHdc();
				SendMessage(new HandleRef(this, this.Handle), 0x317, hdc, (IntPtr)30);
				graphics.ReleaseHdcInternal(hdc);
			}
		}
		
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessage(HandleRef hWnd, int msg, IntPtr wParam, IntPtr lParam);


		public abstract void OnDeviceArrival(DeviceType deviceType);
		public abstract void OnDeviceRemoval(DeviceType deviceType);
		public abstract void OnAppletEnabled();
		public abstract void OnAppletDisabled();
		public abstract void OnCloseConnection();
		public abstract void OnConfigure();

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

		public abstract string AppletName { get; }
	}
}
