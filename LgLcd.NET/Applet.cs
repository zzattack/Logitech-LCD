using System;
using System.Collections.Generic;
using System.Text;
using LgLcdNET;

namespace LgLcdNET
{
	class Applet : IDisposable
	{
		// Properties
		public string FriendlyName { get; private set; }
		public bool Autostartable { get; private set; }
		public AppletCapabilities CapabilitiesSupported { get; set; }
	
		// Config
		protected virtual void OnConfigure();
		// Notifications
		protected virtual void OnDeviceArrival(DeviceType deviceType);
		protected virtual void OnDeviceRemoval(DeviceType deviceType);
		protected virtual void OnAppletEnabled();
		protected virtual void OnAppletDisabled();
		protected virtual void OnCloseConnection();

		Applet(
			string friendlyName,
			bool autostartable,
			AppletCapabilities appletCaps) {
			FriendlyName = friendlyName;
			Autostartable = autostartable;
			CapabilitiesSupported = appletCaps;
		}

		public void Connect() {
			ConnectContextEx cctx = new ConnectContextEx();
			cctx.AppFriendlyName = FriendlyName;
			cctx.IsPersistent = true; // deprecated as of 3.00
			cctx.IsAutostartable = Autostartable;
			cctx.AppletCapabilitiesSupported = CapabilitiesSupported;
			cctx.OnConfigure.Context = IntPtr.Zero;
			cctx.OnConfigure.OnConfigure = new ConfigureDelegate(ConfigureHandler);
			cctx.OnNotify.Context = IntPtr.Zero;
			cctx.OnNotify.OnNotification = new NotificationDelegate(NotifyHandler);
			cctx.Reserved1 = 0;
			LgLcd.ConnectEx(ref cctx);
			connection = cctx.Connection;
		}

		public Device OpenByType(DeviceType deviceType) {
			OpenByTypeContext typectx = new OpenByTypeContext();
			typectx.Connection = connection;
			typectx.DeviceType = deviceType;
			typectx.OnSoftbuttonsChanged.Context = null;
			typectx.OnSoftbuttonsChanged.OnSoftbuttonsChanged = new SoftButtonsDelegate(SoftButtonHandler);
			LgLcd.OpenByType(ref typectx);
			Device device = new Device(typectx.Device, typectx.DeviceType);

			return device;
		}

		public void Disconnect() {
			LgLcd.Disconnect(connection);
		}

		private int ConfigureHandler(int connection, object context) {
			OnConfigure();
			return 0;
		}

		private int NotifyHandler(
			int connection,
			object context,
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
					// All devices of the given type disabled
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

		private int SoftButtonHandler(int device, SoftButtonFlags buttonFlags, object context) {
			return 0;
		}

		private int connection;
	}	
}
