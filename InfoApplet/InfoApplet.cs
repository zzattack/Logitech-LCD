using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LgLcd;
using System.Drawing;

namespace InfoApplet
{
	class InfoApplet : Applet
	{
		InfoApplet()
		{
			this.Connect("Info Applet", true, AppletCapabilities.Qvga);
		}

		protected override void OnDeviceArrival(DeviceType deviceType)
		{
			lcd.Open(this, DeviceType.Qvga);
		}

		protected override void OnDeviceRemoval(DeviceType deviceType)
		{
			lcd.Close();
		}

		protected override void OnAppletEnabled()
		{
			// our applet got enabled, we should start drawing			
			form.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
			lcd.UpdateBitmap(bmp, Priority.Normal, false, false);
		}

		protected override void OnAppletDisabled()
		{
			// applet got disabled, stop drawing
		}

		protected override void OnCloseConnection()
		{
			// oopz, our applet cant do anything anymore now
			// unless it's connected again
		}

		protected override void OnConfigure()
		{
			// show a configuration form (not mandatory)
		}

		Device lcd = new Device();
		AppletForm form = new AppletForm();
		Bitmap bmp = new Bitmap(320, 240);
	}
}
