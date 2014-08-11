using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LgBackLight;

namespace LgLcdTest {
	
	class Program {
        public static LogitechKeyboard LogitechKeyboard;
		static void Main(string[] args) {
			Application.EnableVisualStyles();
			var ctrl = new LgLcdTestForm();
			Application.Run();
		}
	}

}
