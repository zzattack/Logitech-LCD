using System.Drawing;
using LgBackLight;

namespace LgLcdTest {
	
	class Program {
        public static LogitechKeyboard LogitechKeyboard;
		static void Main(string[] args) {
			var ctrl = new LgLcdTestForm();
            LogitechKeyboard.BackLightColor = Color.HotPink;
			System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
		}
	}

}
