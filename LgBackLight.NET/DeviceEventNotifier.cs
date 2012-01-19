using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LgBackLight {

	internal class DeviceEventArgs : EventArgs {
		public string DevicePath { get; private set; }
		public DeviceEventArgs(string devicePath) {
			DevicePath = devicePath;
		}
	}

	internal class DeviceEventNotifier : NativeWindow {

		/// <summary>
		/// Fires when a compatible device is physically added to the system.
		/// </summary>
		public event EventHandler<DeviceEventArgs> DeviceArrival;
		/// <summary>
		/// Fires when a compatible device is physically removed from the system.
		/// </summary>
		public event EventHandler<DeviceEventArgs> DeviceRemoval;

		public DeviceEventNotifier(IntPtr windowHandle, Guid deviceClass) {
			base.AssignHandle(windowHandle);
			var filter = new User32.BroadcastHdr();
			filter.Size = Marshal.SizeOf(filter);
			filter.DeviceType = User32.DeviceType.Interface;
			filter.Interface = new User32.BroadcastDeviceInterface();
			filter.Interface.DeviceClass = deviceClass;
			notificationHandle = User32.RegisterDeviceNotification(
				base.Handle,
				filter,
				User32.DeviceNotificationFlags.WindowHandle);
		}

		~DeviceEventNotifier() {
			User32.UnregisterDeviceNotification(notificationHandle);
		}

		protected override void WndProc(ref System.Windows.Forms.Message m) {
			if (m.Msg == (int)User32.WindowMessage.DeviceChange) {
				User32.BroadcastHdr hdr = new User32.BroadcastHdr();
				if (m.LParam != IntPtr.Zero) {
					hdr = (User32.BroadcastHdr)Marshal.PtrToStructure(
						m.LParam,
						typeof(User32.BroadcastHdr));	
				}
				switch (m.WParam.ToInt32()) {
					case (int)User32.DeviceEvent.DeviceArrival:
						OnDeviceArrived(hdr.Interface.Name);
						break;					
					case (int)User32.DeviceEvent.DeviceRemoveComplete:
						OnDeviceRemoval(hdr.Interface.Name);
						break;
				}
			}
			base.WndProc(ref m);
		}

		private void OnDeviceArrived(string devicePath) {
			if (DeviceArrival != null)
				DeviceArrival(this, new DeviceEventArgs(devicePath));
		}

		private void OnDeviceRemoval(string devicePath) {
			if (DeviceRemoval != null)
				DeviceRemoval(this, new DeviceEventArgs(devicePath));
		}

		IntPtr notificationHandle;

	}
	
	// User32.dll definitions
	internal static class User32  {

		#region Enumerations

		public enum DeviceType {
			Oem = 0,
			Volume = 2,
			Port = 3,
			Interface = 5,
			Handle = 6,			
		}

		[Flags]
		public enum DeviceNotificationFlags {
			WindowHandle,
			ServiceHandle,
			AllInterfaceClasses = 4,
		}

		public enum WindowMessage {
			DeviceChange = 0x0219,
		}

		public enum DeviceEvent {
			DeviceArrival = 0x8000,
			DeviceRemoveComplete = 0x8004,
		}

		#endregion

		#region Structures

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public struct BroadcastDeviceInterface {
			public Guid DeviceClass;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string Name;
		}

		public struct BroadcastHdr {
			public int Size;
			public DeviceType DeviceType;
			public int Reserved;
			public BroadcastDeviceInterface Interface;
		}

		#endregion

		#region Functions

		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern IntPtr RegisterDeviceNotification(
			IntPtr recipient,
			BroadcastHdr filter,
			DeviceNotificationFlags flags);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool UnregisterDeviceNotification(IntPtr handle);

		#endregion

	}

}
