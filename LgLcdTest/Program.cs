using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LgLcdNET;

namespace LgLcdTest {
	class Program {

		class ExampleApplet : LgLcdNET.Applet
		{
			protected override void OnDeviceArrival(DeviceType deviceType)
			{
				MessageBox.Show("Device arrived " + deviceType.ToString());
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

			protected override void  OnCloseConnection()
			{
				MessageBox.Show("Connection closed");
			}
		}

		static void Main(string[] args) {
			// Init
			LgLcdNET.LgLcd.Init();			
			// Create and connect applet
			ExampleApplet applet = new ExampleApplet();
			applet.Connect("My Applet", false, AppletCapabilities.Qvga);
			System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
		}
	}
}
