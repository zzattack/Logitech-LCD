using System;
using System.ComponentModel;

namespace LgLcd {

	[Flags]
	public enum AppletCapabilities {
		Monochrome = LgLcd.AppletCapabilities.Bw,
		Qvga = LgLcd.AppletCapabilities.Qvga,
	}

	public interface IApplet {
		// Notifications that must be implemented by an applet
		void OnDeviceArrival(DeviceType deviceType);
		void OnDeviceRemoval(DeviceType deviceType);
		void OnAppletEnabled();
		void OnAppletDisabled();
		void OnCloseConnection();
		// Called when the user wishes to configure our application from LCDMon
		void OnConfigure();

		void Connect(string friendlyName, bool autostartable, AppletCapabilities appletCaps);
		void Disconnect();
	}

	public abstract class Applet : IApplet {

		// The connection handle
		private int _handle = LgLcd.InvalidConnection;
		public int Handle { get { return _handle; } }

		// Gets whether the applet is currently connected to LCDMon
		public bool Connected { get { return _handle != LgLcd.InvalidConnection; } }

		// Properties
		public string FriendlyName { get; private set; }
		public bool Autostartable { get; private set; }
		public AppletCapabilities CapabilitiesSupported { get; private set; }
		
		static Applet() {
			LgLcd.Init();
			// Queue DeInit to be called on application exit
			AppDomain.CurrentDomain.ProcessExit += delegate {
				LgLcd.DeInit();
			};
		}

		protected Applet(){
			_configurationDelegate = ConfigureHandler;
			_notificationDelegate = NotifyHandler;
		}

		public void Connect(string friendlyName, bool autostartable, AppletCapabilities appletCaps) {
			if (Connected) {
				throw new Exception("Already connected.");
			}
			FriendlyName = friendlyName;
			Autostartable = autostartable;
			CapabilitiesSupported = appletCaps;
			var ctx = new LgLcd.ConnectContextEx {
				AppFriendlyName = friendlyName,
				AppletCapabilitiesSupported = (LgLcd.AppletCapabilities)appletCaps,
				IsAutostartable = autostartable,
				IsPersistent = true, // deprecated and ignored as of 3.00
				OnConfigure = new LgLcd.ConfigureContext {
					Context = IntPtr.Zero,
					OnConfigure = _configurationDelegate,
				},
				OnNotify = new LgLcd.NotificationContext {
					Context = IntPtr.Zero,
					OnNotification = _notificationDelegate,
				},
				Reserved1 = 0,
			};
			var error = LgLcd.ConnectEx(ref ctx);
			if (error != LgLcd.ReturnValue.ErrorSuccess) {
				if (error == LgLcd.ReturnValue.ErrorInvalidParameter) {
					throw new ArgumentException("FriendlyName must not be null.");
				}
				if (error == LgLcd.ReturnValue.ErrorFileNotFound) {
					throw new Exception("LCDMon is not running on the system.");
				}
				if (error == LgLcd.ReturnValue.ErrorAlreadyExists) {
					throw new Exception("The same client is already connected.");
				}
				if (error == LgLcd.ReturnValue.RcpXWrongPipeVersion) {
					throw new Exception("LCDMon does not understand the protocol.");
				}
				throw new Win32Exception((int)error);
			}
			_handle = ctx.Connection;
		}

		public void Disconnect() {
			if (!Connected) {
				throw new Exception("Not connected.");
			}
			var error = LgLcd.Disconnect(Handle);
			if (error != LgLcd.ReturnValue.ErrorSuccess) {
				throw new Win32Exception((int)error);
			}
			// Reset the handle
			_handle = LgLcd.InvalidConnection;
		}

		private int ConfigureHandler(int connection, IntPtr context) {
			OnConfigure();
			return 0;
		}

		private int NotifyHandler(
			int connection,
			IntPtr context,
			LgLcd.NotificationCode notificationCode,
			int notifyParam1,
			int notifyParam2,
			int notifyParam3,
			int notifyParam4) {
			switch (notificationCode) {
				case LgLcd.NotificationCode.DeviceArrival:
					OnDeviceArrival((DeviceType)notifyParam1);
					break;
				case LgLcd.NotificationCode.DeviceRemoval:
					// All devices of the given type got disabled
					OnDeviceRemoval((DeviceType)notifyParam1);
					break;
				case LgLcd.NotificationCode.AppletEnabled:
					OnAppletEnabled();
					break;
				case LgLcd.NotificationCode.AppletDisabled:
					OnAppletDisabled();
					break;
				case LgLcd.NotificationCode.CloseConnection:
					OnCloseConnection();
					break;
			}
			return 0;
		}

		private LgLcd.ConfigureDelegate _configurationDelegate;
		private LgLcd.NotificationDelegate _notificationDelegate;

		public virtual void OnDeviceArrival(DeviceType deviceType) { }
		public virtual void OnDeviceRemoval(DeviceType deviceType) { }
		public virtual void OnAppletEnabled() { }
		public virtual void OnAppletDisabled() { }
		public virtual void OnCloseConnection() { }
		public virtual void OnConfigure() { }
	}
}
