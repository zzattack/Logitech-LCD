using System;
using System.Linq;

namespace RandomImageApplet {
	static class Program {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			new RandomImageApplet();
			System.Diagnostics.Process.GetCurrentProcess().WaitForExit();
		}
	}
}
