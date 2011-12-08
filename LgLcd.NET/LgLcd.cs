using System;
using System.Runtime.InteropServices;

namespace LgLcdNET {

	#region Enumerations/Constants

	public enum AppletCapabilities {
		Basic,
		Bw,
		Qvga,
	}

	public enum Priority {
		IdleNoShow = 0,
		Background = 64,
		Normal = 128,
		Alert = 255,
	}

	[Flags]
	public enum SoftButtonFlags : uint {
		Button0 = 0x1,
		Button1 = 0x2,
		Button2 = 0x4,
		Button3 = 0x8,
		Button4 = 0x10,
		Button5 = 0x20,
		Button6 = 0x40,
		Button7 = 0x80,
		Left = 0x100,
		Right = 0x200,
		Ok = 0x400,
		Cancel = 0x800,
		Up = 0x1000,
		Down = 0x2000,
		Menu = 0x4000,
	}

	[Flags]
	public enum DeviceFamilyFlags : uint {
		Bw160x43Gaming = 0x1,
		KeyboardG15 = 0x1,
		Bw160x43Audio = 0x2,
		SpeakersZ10 = 0x2,
		Jackbox = 0x4,
		Bw160x43Basic = 0x8,
		LcdEmulatorG15 = 0x8,
		Rainbox = 0x10,
		QvgaBasic = 0x20,
		QvgaGaming = 0x40,
		GameboardG13 = 0x80,
		KeyboardG510 = 0x100,
		Other = 0x80000000,
	}

	public enum ReturnValue : uint {
		ErrorSuccess = 0,
		ErrorFileNotFound = 2,
		ErrorAccessDenied = 5,
		ErrorInvalidParameter = 87,
		ErrorLockFailed = 167,
		ErrorAlreadyExists = 183,
		ErrorNoMoreItems = 259,
		ErrorOldWinVersion = 1150,
		ErrorServiceNotActive = 1062,
		ErrorDeviceNotConnected = 1167,
		ErrorAlreadyInitialized = 1247,
		ErrorNoSystemResources = 1450,
		RcpSServerUnavailable = 1722,
		RcpXWrongPipeVersion = 1832,
	}
	
	public enum BitmapFormat {
		BlackWhite = 1,
		QVGAx32 = 3,
	}

	public enum NotificationCode : uint {
		DeviceArrival = 1,
		DeviceRemoval = 2,
		CloseConnection = 3,
		AppletDisabled = 4,
		AppletEnabled = 5,
		TerminateApplet = 6,
	}

	public enum DeviceType : uint {
		BlackWhite = 1,
		QVGA = 2
	}

	public enum BwBmp {
		Width = 160,
		Height = 43,
		Bpp = 1,
	}

	public enum Qvga {
		Width = 320,
		Height = 240,
		Bpp = 4,
	}

	#endregion

	#region Delegates

	public delegate uint ConfigureDelegate(int connection, IntPtr context);

	public delegate int NotificationDelegate(
		int connection,
		IntPtr context,
		NotificationCode notificationCode,
		int notifyParam1,
		int notifyParam2,
		int notifyParam3,
		int notifyParam4);

	public delegate int SoftButtonsDelegate(int device, SoftButtonFlags buttons, IntPtr context);

	#endregion

	#region Structures

	[StructLayout(LayoutKind.Sequential)]
	public struct DeviceDesc {
		public uint Width;
		public uint Height;
		public uint Bpp;
		public uint NumSoftButtons;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct DeviceDescEx {
		public DeviceFamilyFlags FamilyId;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string DisplayName;
		public uint Width;
		public uint Height;
		public uint Bpp;
		public uint NumSoftButtons;
		public uint Reserved1;
		public uint Reserved2;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public struct NotificationContext {
		public NotificationDelegate OnNotification;
		public IntPtr Context;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ConfigureContext {
		public ConfigureDelegate OnConfigure;
		public IntPtr Context;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ConnectContext {
		public string AppFriendlyName;
		public bool IsPersistent;
		public bool IsAutostartable;
		public ConfigureContext OnConfigure;
		public int Connection;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ConnectContextEx {
		public string AppFriendlyName;
		public bool IsPersistent;
		public bool IsAutostartable;
		public ConfigureContext OnConfigure;
		public int Connection;
		public AppletCapabilities AppletCapabilitiesSupported;
		public int Reserved1;
		public NotificationContext OnNotify;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct SoftbuttonsChangedContext {
		public SoftButtonsDelegate OnSoftbuttonsChanged;
		public IntPtr Context;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct OpenContext {
		public int Connection;
		public int Index;
		public SoftbuttonsChangedContext OnSoftbuttonsChanged;
		public int Device;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct OpenByTypeContext {
		public int Connection;
		public DeviceType DeviceType;
		public SoftbuttonsChangedContext OnSoftbuttonsChanged;
		public int Device;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Bitmap {
		public BitmapFormat Format;
		public byte[] Pixels;
	}

	#endregion

	#region Functions

	public static class LgLcd {

		[DllImport("lglcd.dll", EntryPoint = "lgLcdInit")]
		public static extern ReturnValue Init();

		[DllImport("lglcd.dll", EntryPoint = "lgLcdDeInit")]
		public static extern ReturnValue DeInit();

		[DllImport("lglcd.dll", EntryPoint = "lgLcdConnect", CharSet = CharSet.Auto)]
		public static extern ReturnValue Connect(ref ConnectContext ctx);

		[DllImport("lglcd.dll", EntryPoint = "lgLcdConnectEx", CharSet = CharSet.Auto)]
		public static extern ReturnValue ConnectEx(ref ConnectContextEx ctx);

		[DllImport("lglcd.dll", EntryPoint = "lgLcdDisconnect")]
		public static extern ReturnValue Disconnect(int connection);

		[DllImport("lglcd.dll", EntryPoint = "lgLcdSetDeviceFamiliesToUse")]
		public static extern ReturnValue SetDeviceFamiliesToUse(int connection, DeviceFamilyFlags familiesSupported, uint reserved1);

		[DllImport("lglcd.dll", EntryPoint = "lgLcdEnumerate", CharSet = CharSet.Auto)]
		public static extern ReturnValue Enumerate(int connection, int index, ref DeviceDesc description);

		[DllImport("lglcd.dll", EntryPoint = "lgLcdEnumerateEx", CharSet = CharSet.Auto)]
		public static extern ReturnValue EnumerateEx(int connection, int index, ref DeviceDescEx description);

		[DllImport("lglcd.dll", EntryPoint = "lgLcdOpen")]
		public static extern ReturnValue Open(ref OpenContext ctx);

		[DllImport("lglcd.dll", EntryPoint = "lgLcdOpenByType")]
		public static extern ReturnValue OpenByType(ref OpenByTypeContext ctx);

		[DllImport("lglcd.dll", EntryPoint = "lgLcdClose")]
		public static extern ReturnValue Close(int device);

		[DllImport("lglcd.dll", EntryPoint = "lgLcdReadSoftButtons")]
		public static extern ReturnValue ReadSoftButtons(int device, ref SoftButtonFlags buttons);

		[DllImport("lglcd.dll", EntryPoint = "lgLcdUpdateBitmap")]
		public static extern ReturnValue UpdateBitmap(int device, ref Bitmap bitmap, uint priority);

		[DllImport("lglcd.dll", EntryPoint = "lgLcdSetAsLCDForegroundApp")]
		public static extern ReturnValue SetAsLCDForegroundApp(int device, int foregroundYesNoFlag);

		[DllImport("lglcd.dll")]
		public static extern int UpdateBitmap(int device, Bitmap bitmap, int priority);

	#endregion

	}
}
