using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LgLcd;
using System.Drawing;
using System.Diagnostics;

namespace InfoApplet {
	public class InfoApplet : Applet {
		public InfoApplet() {
			this.Connect("Info Applet", true, AppletCapabilities.Qvga);
			form.Paint += new System.Windows.Forms.PaintEventHandler(form_Paint);
		}

		protected override void OnDeviceArrival(DeviceType deviceType) {
			lcd.Open(this, DeviceType.Qvga);
		}

		void form_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
			form.Paint -= form_Paint;
			// form.DrawToBitmap(bmp, form.ClientRectangle);
			form.Paint += form_Paint;
			//lcd.UpdateBitmap(bmp, Priority.Normal);
		}

		protected override void OnDeviceRemoval(DeviceType deviceType) {
			lcd.Close();
		}

		protected override void OnAppletEnabled() {
			// form.DrawToBitmap(bmp, form.ClientRectangle);			
		}

		protected override void OnAppletDisabled() {
			// applet got disabled, stop drawing
		}

		protected override void OnCloseConnection() {
			// oopz, our applet cant do anything anymore now
			// unless it's connected again
		}

		protected override void OnConfigure() {
			// show a configuration form (not mandatory)
		}

		Device lcd = new Device();
		AppletForm form = new AppletForm();
		Bitmap bmp = new Bitmap(320, 240);

	}
}
