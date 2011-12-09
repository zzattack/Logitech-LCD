using System;
using System.Collections.Generic;
using System.Text;
using LgLcdNET;
using System.ComponentModel;

namespace LgLcdNET
{
	public class Applet
	{
		// The connection handle
		public int Handle { get; private set; }

		// Properties
		public string FriendlyName { get; private set; }
		public bool Autostartable { get; private set; }
		public AppletCapabilities CapabilitiesSupported { get; private set; }

		// Config
		protected virtual void OnConfigure() { }
		// Notifications
		protected abstract void OnDeviceArrival(DeviceType deviceType) { }
		protected abstract void OnDeviceRemoval(DeviceType deviceType) { }
		protected abstract void OnAppletEnabled() { }
		protected abstract void OnAppletDisabled() { }
		protected abstract void OnCloseConnection() { }

		public void Connect(string friendlyName, bool autostartable, AppletCapabilities appletCaps) {
			FriendlyName = friendlyName;
			Autostartable = autostartable;
			CapabilitiesSupported = appletCaps;			
			ConnectContextEx ctx = new ConnectContextEx() {
				AppFriendlyName = friendlyName,
				AppletCapabilitiesSupported = appletCaps,
				IsAutostartable = autostartable,
				IsPersistent = true, // deprecated as of 3.00
				OnConfigure = new ConfigureContext() {
					Context = IntPtr.Zero,
					OnConfigure = new ConfigureDelegate(ConfigureHandler),
				},
				OnNotify = new NotificationContext() {
					Context = IntPtr.Zero,
					OnNotification = new NotificationDelegate(NotifyHandler),
				},
				Reserved1 = 0,								
			};			
			ReturnValue error = LgLcd.ConnectEx(ref ctx);
			if (error != ReturnValue.ErrorSuccess) {
				// TODO: Handle errors
				// ServiceNotActive
				// InvalidParameter
				// FileNotFound
				// AlreadyExists
				// RcpXWrongPipeVersion
				// Xxx
				throw new Exception();
			}			
			Handle = ctx.Connection;
		}

		public void Disconnect() {
			LgLcd.Disconnect(Handle);
		}

		private int ConfigureHandler(int connection, IntPtr context) {
			OnConfigure();
			return 0;
		}

		private int NotifyHandler(
			int connection,
			IntPtr context,
			NotificationCode notificationCode,
			int notifyParam1,
			int notifyParam2,
			int notifyParam3,
			int notifyParam4) {
			switch (notificationCode) {
				case NotificationCode.DeviceArrival:
					OnDeviceArrival((DeviceType)notifyParam1);
					break;
				case NotificationCode.DeviceRemoval:
					// All devices of the given type got disabled
					OnDeviceRemoval((DeviceType)notifyParam1);
					break;
				case NotificationCode.AppletEnabled:
					OnAppletEnabled();
					break;
				case NotificationCode.AppletDisabled:
					OnAppletDisabled();
					break;
				case NotificationCode.CloseConnection:
					OnCloseConnection();
					break;
			}
			return 0;
		}
	}	
}
