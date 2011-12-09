using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using LgLcdNET;

namespace LgLcdTest
{
	class Program
	{
		class ExampleApplet : LgLcdNET.Applet
		{
			protected override void OnDeviceArrival(DeviceType deviceType)
			{
				MessageBox.Show("Device arrived " + deviceType.ToString());
				// Create and open a device
				Device device = new Device(DeviceType.Qvga);
				device.Open(this);
				// Try dislplay sample bitmap
				Bitmap bmp = (Bitmap)Bitmap.FromFile(@"..\..\qvga_sample.bmp");
				device.UpdateBitmap(bmp as Bitmap, Priority.Normal);
			}

			protected override void OnDeviceRemoval(DeviceType deviceType)
			{
				MessageBox.Show("Device removed " + deviceType.ToString());
			}

			protected override void OnAppletEnabled()
			{
				MessageBox.Show("Applet enabled");
			}

			protected override void OnAppletDisabled()
			{
				MessageBox.Show("Applet disabled");
			}

			protected override void OnCloseConnection()
			{
				MessageBox.Show("Connection closed");
			}

			protected override void OnConfigure()
			{
				MessageBox.Show("Configure");
			}
		}

		static void Main(string[] args)
		{
			// Init
			LgLcdNET.LgLcd.Init();
			// Create and connect applet
			ExampleApplet applet = new ExampleApplet();
			applet.Connect("appletname", false, AppletCapabilities.Qvga);
			System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
		}
	}
}
