using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LgLcdNET;

namespace LgLcdTest {
	class Program {
		static void Main(string[] args) {
			LgLcd.Init();
			ConnectContext ctx = new ConnectContext();
			ctx.AppFriendlyName = "aaaaaaaaaa";
			ctx.IsAutostartable = false;
			ctx.IsPersistent = false;
			ctx.Connection = 0;
			ctx.OnConfigure = new ConfigureContext();
			ctx.OnConfigure.OnConfigure = new ConfigureDelegate(OnConfigure);
			var ret = LgLcd.Connect(ref ctx);
		}

		static uint OnConfigure(int connection, IntPtr ctx) {
			return 0;
		}
	}
}
