using System;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;
using LgLcd;
using LgBackLight;

namespace LgLcdTest {
	
	class Program {
		static void Main(string[] args) {
			var ctrl = new LgLcdTestForm();
			BackLight.SetBackLight(Color.LightPink);
			System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
		}
	}

}
