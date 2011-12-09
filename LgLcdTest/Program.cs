using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LgLcdTest {
	class Program {
		static void Main(string[] args) {
			// Init
			LgLcdNET.LgLcd.Init();
			// Create and connect applet
			LgLcdNET.Applet applet = new LgLcdNET.Applet();
			applet.Connect("My Applet", false, LgLcdNET.AppletCapabilities.Qvga);
			// Create and open device
			LgLcdNET.Device qvgaDevice = new LgLcdNET.Device(LgLcdNET.DeviceType.Qvga);
			qvgaDevice.Open(applet);
		}
	}
}
