using System;
using System.Runtime.InteropServices;

namespace LgLcd.NET
{
	public static class LgLcd
	{

		#region Enumerations/Constants

		public enum AppletCapabilities
		{
			Basic,
			Bw,
			Qvga,
		}

		public enum Priority
		{
			IdleNoShow = 0,
			Background = 64,
			Normal = 128,
			Alert = 255,
		}

		[Flags]
		public enum SoftButtonFlags
		{
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
		public enum DeviceFamilyFlags : uint
		{
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

		public enum ReturnValue
		{
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

		public enum BitmapFormat
		{
			_160x43x1 = 1,
			QVGAx32 = 3,
		}

		public enum Notification
		{
			DeviceArrival = 1,
			DeviceRemoval = 2,
			CloseConnection = 3,
			AppletDisabled = 4,
			AppletEnabled = 5,
			TerminateApplet = 6,
		}

		public enum BwBmp
		{
			Width = 160,
			Height = 43,
			Bpp = 1,
		}

		public enum Qvga
		{
			Width = 320,
			Height = 240,
			Bpp = 4,
		}

		#endregion

		#region Delegates

		public delegate int ConfigureDelegate(
			int connection,
			object context);

		public delegate int NotificationDelegate(
			int connection,
			object context,
			int notificationCode,
			int notifyParam1,
			int notifyParam2,
			int notifyParam3,
			int notifyParam4);

		public delegate int SoftButtonsDelegate(
			int device,
			SoftButtonFlags buttons,
			object context);

		#endregion

		#region Structures

		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		public struct DeviceDesc
		{
			public int Width;
			public int Height;
			public int Bpp;
			public int NumSoftButtons;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		public struct DeviceDescEx
		{
			public DeviceFamilyFlags FamilyId;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string DisplayName;
			public int Width;
			public int Height;
			public int Bpp;
			public int NumSoftButtons;
			public int Reserved1;
			public int Reserved2;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		public struct NotificationContext
		{
			public NotificationDelegate OnNotification;
			public object Context;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		public struct ConfigureContext
		{
			public ConfigureDelegate OnConfigure;
			public object Context;
		}

		[StructLayout(LayoutKind.Sequential, Pack=8)] // this
		public struct ConnectContext
		{
			[MarshalAs(UnmanagedType.LPTStr)]
			public string AppFriendlyName;
			public int IsPersistent;
			public int IsAutostartable;
			public int OnConfigure;
			public int Connection;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		public struct ConnectContextEx
		{
			public string AppFriendlyName;
			public bool IsPersistent;
			public bool IsAutostartable;
			public ConfigureContext OnConfigure;
			public int Connection;
			public AppletCapabilities AppletCapabilitiesSupported;
			public int Reserved1;
			public NotificationContext OnNotify;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		public struct SoftbuttonsChangedContext
		{
			public SoftButtonsDelegate OnSoftbuttonsChanged;
			public object Context;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		public struct OpenContext
		{
			public int Connection;
			public int Index;
			public SoftbuttonsChangedContext OnSoftbuttonsChanged;
			public int Device;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		public struct OpenByTypeContext
		{
			public int Connection;
			public int DeviceType;
			public SoftbuttonsChangedContext OnSoftbuttonsChanged;
			public int Device;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		public struct Bitmap
		{
			public BitmapFormat Format;
			public byte[] Pixels;
		}

		#endregion

		#region Functions

		[DllImport("lglcd.dll")]
		public static extern ReturnValue Init();

		[DllImport("lglcd.dll")]
		public static extern ReturnValue DeInit();

		[DllImport("lglcd.dll")]
		public static extern ReturnValue Connect(ref ConnectContext ctx);

		[DllImport("lglcd.dll")]
		public static extern ReturnValue ConnectEx(ref ConnectContextEx ctx);

		[DllImport("lglcd.dll")]
		public static extern ReturnValue Disconnect(int connection);

		[DllImport("lglcd.dll")]
		public static extern ReturnValue SetDeviceFamiliesToUse(
			int connection,
			DeviceFamilyFlags familiesSupported,
			int reserved1);

		[DllImport("lglcd.dll")]
		public static extern ReturnValue Enumerate(
			int connection,
			int index,
			out DeviceDesc description);

		[DllImport("lglcd.dll")]
		public static extern ReturnValue EnumerateEx(
			int connection,
			int index,
			out DeviceDescEx description);

		[DllImport("lglcd.dll")]
		public static extern ReturnValue Open(ref OpenContext ctx);

		[DllImport("lglcd.dll")]
		public static extern ReturnValue OpenByType(ref OpenByTypeContext ctx);

		[DllImport("lglcd.dll")]
		public static extern ReturnValue Close(int device);

		[DllImport("lglcd.dll")]
		public static extern ReturnValue ReadSoftButtons(int device, out int buttons);

		[DllImport("lglcd.dll")]
		public static extern int UpdateBitmap(
			int device,
			Bitmap bitmap,
			int priority);

		[DllImport("lglcd.dll")]
		public static extern ReturnValue SetAsForegroundApp(int device,	bool yesNo);

		#endregion

	}
}
