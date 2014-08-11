using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LgLcd;
using LgBackLight;
using System.Timers;

namespace LgLcdTest {
	public partial class LgLcdTestForm : WinFormsApplet, IDisposable {
		public LgLcdTestForm() {
			InitializeComponent();

			base.InitializeApplet();
			Device.SetAsLCDForegroundApp(true);

			this.HandleCreated += LgLcdTestForm_HandleCreated;
			UpdateLcdScreen(this, EventArgs.Empty);
		}

		void LgLcdTestForm_HandleCreated(object sender, EventArgs e) {
			LogitechKeyboardManager.Init(this.Handle, LogitechKeyboardType.G19);
			var kb = LogitechKeyboardManager.GetKeyboards(LogitechKeyboardType.G19);
			kb.First().BackLightColor = Color.HotPink;
		}

		public override void OnDeviceArrival(DeviceType deviceType) {
			Debug.WriteLine("Device arrived " + deviceType);
		}

		public override void OnDeviceRemoval(DeviceType deviceType) {
			Debug.WriteLine("Device removed " + deviceType);
		}

		public override void OnAppletEnabled() {
			Debug.WriteLine("Applet enabled");
		}

		public override void OnAppletDisabled() {
			Debug.WriteLine("Applet disabled");
		}

		public override void OnCloseConnection() {
			Debug.WriteLine("Connection closed");
		}

		public override void OnConfigure() {
			MessageBox.Show("Configure");
		}

		public override string AppletName {
			get { return "test"; }
		}

		public override event EventHandler UpdateLcdScreen;
	}
}
