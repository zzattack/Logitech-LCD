using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;

namespace LgLcd
{
	// brb m8, 1 - 2 mins
	public static class BackLight
	{

		static List<HidDevice> devs = new List<HidDevice>();
		const int vendorId = 0x046D;
		const ushort  lgG19PID = 0xC229; // FUCK how did this happen :L
		const int lgG15PID = 0x229/* ??? */;
		static NativeWindowX win;

		static BackLight()
		{
			var pids = new int[] { lgG19PID, lgG15PID };
			devs = HidDevice.Open(vendorId, pids);
		}

		public static Color GetBackLight(int deviceInstance = 0)
		{
			var dev = devs[deviceInstance];
			byte[] b = new byte[4];
			b[0] = 7;
			dev.GetFeature(b);
			return Color.FromArgb(b[1], b[2], b[3]); // yeah thats fine anyways this
		}

		public static void SetBackLight(Color color, int deviceInstance = 0)
		{
			var dev = devs[deviceInstance];
			byte[] b = new byte[4];
			b[0] = 7;
			b[1] = color.R;
			b[2] = color.G;
			b[3] = color.B;
			dev.SetFeature(b);
		}

		/// <summary>
		/// Registers for device notifications so that the DeviceAdded and DeviceRemoved
		/// events can fire upon hard-removal of the keyboard
		/// </summary>
		/// <param name="windowHandle"></param>
		public static void RegisterDeviceNotifications(IntPtr windowHandle)
		{
			win = new NativeWindowX(windowHandle);
			win.DeviceAdded += DeviceAdded;
			win.DeviceRemoved += DeviceRemoved;
		}

		/// <summary>
		/// Fires when a compatible device is physically added to the system.
		/// Remember to call RegisterDeviceNotifications first!
		/// </summary>
		static void DeviceAdded(object sender, DeviceChangedArgs args)
		{
			if (devs.Any(dev => dev.DevicePath.ToLower() == args.DevicePath.ToLower()))
				System.Windows.Forms.MessageBox.Show("G19 was added");
		}

		/// <summary>
		/// Fires when a compatible device is physically removed from the system.
		/// Remember to call RegisterDeviceNotifications first!
		/// </summary>
		static void DeviceRemoved(object sender, DeviceChangedArgs args)
		{
			if (devs.Any(dev => dev.DevicePath.ToLower() == args.DevicePath.ToLower()))
				System.Windows.Forms.MessageBox.Show("G19 was removed");
		}

		public class DeviceChangedArgs : EventArgs
		{
			public string DevicePath { get; private set; }
			public DeviceChangedArgs(string devicePath)
			{
				this.DevicePath = devicePath;
			}
		}

		internal class NativeWindowX : System.Windows.Forms.NativeWindow
		{
			const int WM_DEVICECHANGE = 0x0219;
			const int DBT_DEVICEARRIVAL = 0x8000;
			const int DBT_DEVICEREMOVECOMPLETE = 0x8004;

			IntPtr notificationHandle;

			public event EventHandler<DeviceChangedArgs> DeviceAdded;
			public event EventHandler<DeviceChangedArgs> DeviceRemoved;


			private NativeWindowX() { }
			public NativeWindowX(IntPtr hWnd)
				: base()
			{
				this.AssignHandle(hWnd);
				var filter = new User32.BroadcastHdr();
				filter.Size = Marshal.SizeOf(filter);
				filter.DeviceType = User32.DBCHDeviceType.DeviceInterface;
				filter.DataX = new User32.BroadcastHdr.Data();
				Guid guid;
				Hid.HidD_GetHidGuid(out guid);
				filter.DataX.Interface.ClasssorGuid = guid;
				notificationHandle = User32.RegisterDeviceNotification(this.Handle, filter, 0);
			}

			~NativeWindowX()
			{
				User32.UnregisterDeviceNotification(notificationHandle);
			}

			protected override void WndProc(ref System.Windows.Forms.Message m)
			{
				if ((int)m.Msg == WM_DEVICECHANGE) // weird it was 0x7 :D
				{
					switch (m.WParam.ToInt32())
					{

						case DBT_DEVICEARRIVAL:
						case DBT_DEVICEREMOVECOMPLETE:

							Debug.WriteLine("Device removal!");
							var hdr = (User32.BroadcastHdr)Marshal.PtrToStructure(
								m.LParam, typeof(User32.BroadcastHdr));

							if (hdr.DeviceType == User32.DBCHDeviceType.DeviceInterface) 
							{
								if (m.WParam.ToInt32() == DBT_DEVICEARRIVAL)
									OnDeviceAdded(hdr.DataX.Interface.Name);
								else
									OnDeviceRemoved(hdr.DataX.Interface.Name);
							}
							break;

						default:
							break;
					}

					base.WndProc(ref m);
				}
			}

			private void OnDeviceAdded(string devicePath)
			{
				if (DeviceAdded != null)
					DeviceAdded(this, new DeviceChangedArgs(devicePath));
			}

			private void OnDeviceRemoved(string devicePath)
			{
				if (DeviceRemoved != null)
					DeviceRemoved(this, new DeviceChangedArgs(devicePath));
			}

		}
	}
}
// aight lets tets
// nah, i give it 0.01 % it'll work :P otherwise nice job/code
