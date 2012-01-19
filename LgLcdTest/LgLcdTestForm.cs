using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LgLcd;
using LgBackLight;
using System.Timers;

namespace LgLcdTest
{
	public partial class LgLcdTestForm : WinFormsApplet, IDisposable
	{
		public LgLcdTestForm()
		{
			InitializeComponent();
			this.HandleCreated += new EventHandler(LgLcdTestForm_HandleCreated);			
		}

		void LgLcdTestForm_HandleCreated(object sender, EventArgs e)
		{
			BackLight.Initialize(base.Handle);
		}

		public override void OnDeviceArrival(DeviceType deviceType)
		{
			MessageBox.Show("Device arrived " + deviceType.ToString());
			timer.Interval = 1000;
			timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
		}

		public override void OnDeviceRemoval(DeviceType deviceType)
		{
			MessageBox.Show("Device removed " + deviceType.ToString());
		}

		public override void OnAppletEnabled()
		{
			MessageBox.Show("Applet enabled");
			timer.Start();
		}

		public override void OnAppletDisabled()
		{
			MessageBox.Show("Applet disabled");
			timer.Stop();
		}

		public override void OnCloseConnection()
		{
			MessageBox.Show("Connection closed");
			timer.Stop();
		}

		public override void OnConfigure()
		{
			MessageBox.Show("Configure");
		}

		void timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			Bitmap bmp = (Bitmap)Bitmap.FromFile(
				@"..\..\qvga_sample" + (b ? 1 : 2) + ".bmp");
			device.UpdateBitmap(bmp, Priority.Normal, false, false);
			device.SetAsLCDForegroundApp(true);
			b = !b;
			UpdateLcdScreen(this, EventArgs.Empty);
		}

		public void Dispose()
		{
			timer.Dispose();
			base.Dispose();
		}

		private bool b = false;
		private System.Timers.Timer timer = new System.Timers.Timer();

		public override event EventHandler UpdateLcdScreen;

		public override string AppletName
		{
			get { return "test"; }
		}
	}
}
