using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;


namespace LgLcd
{

	internal class HidDevice : IDisposable
	{

		public int VendorId { get; private set; }
		public int ProductId { get; private set; }

		public bool GetFeature(byte[] reportBuffer)
		{
			return Hid.HidD_GetFeature(Handle, reportBuffer, reportBuffer.Length);
		}

		public bool SetFeature(byte[] reportBuffer)
		{
			return Hid.HidD_SetFeature(Handle, reportBuffer, reportBuffer.Length);
		}

		public bool GetInputReport(byte[] reportBuffer)
		{
			return Hid.HidD_GetInputReport(Handle, ref reportBuffer, reportBuffer.Length);
		}

		public bool SetOutputReport(byte[] reportBuffer)
		{
			return Hid.HidD_SetOutputReport(Handle, reportBuffer, reportBuffer.Length);
		}

		public static List<HidDevice> Open(int vendorId, int productId)
		{
			return Open(vendorId, new int[] { productId });
		}

		public static List<HidDevice> Open(int vendorId, IEnumerable<int> productIds)
		{
			// Obtain system-defined GUID for HIDClass devices.
			Guid hidClassGuid;
			Hid.HidD_GetHidGuid(out hidClassGuid);
			var deviceList = new List<HidDevice>();

			// Obtain handle to an opaque device-information-set
			// describing device interface supported by all HID
			// collections currently installed in the system.
			IntPtr deviceInfoSet = SetupApi.SetupDiGetClassDevs(
				ref hidClassGuid,
				null,
				IntPtr.Zero,
				SetupApi.DiGetFlags.DeviceInterface | SetupApi.DiGetFlags.Present);
			if (deviceInfoSet == Kernel32.InvalidHandleValue)
			{
				throw new Win32Exception();
			}
			// Retrieve all available interface information.
			SetupApi.SpDeviceInterfaceData did = new SetupApi.SpDeviceInterfaceData();
			did.Size = Marshal.SizeOf(did);
			int i = 0;
			while (SetupApi.SetupDiEnumDeviceInterfaces(deviceInfoSet, IntPtr.Zero, ref hidClassGuid, i++, ref did))
			{
				// Obtain DevicePath
				var didd = new SetupApi.SpDeviceInterfaceDetailData();
				if (IntPtr.Size == 8) // for 64 bit operating systems
					didd.Size = 8;
				else
					didd.Size = 4 + Marshal.SystemDefaultCharSize; // for 32 bit systems
				if (SetupApi.SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref did, ref didd, Marshal.SizeOf(didd), IntPtr.Zero, IntPtr.Zero))
				{
					// Write access causes failure if not running elevated.
					IntPtr hDevice = Kernel32.CreateFile(
						didd.DevicePath,
						FileAccess.Read, // we just need read
						FileShare.ReadWrite, // and we don't want to restrict others from write
						IntPtr.Zero,
						FileMode.Open,
						FileAttributes.Device,
						IntPtr.Zero);

					if (hDevice != Kernel32.InvalidHandleValue)
					{
						Hid.DeviceAttributes attributes;
						if (Hid.HidD_GetAttributes(hDevice, out attributes))
						{
							if (vendorId == attributes.VendorId && productIds.Contains((int)attributes.ProductId))
							{
								deviceList.Add(new HidDevice(hDevice, didd.DevicePath, vendorId, attributes.ProductId));
								continue;
							}
						}
						// Close the wrong device handle
						Kernel32.CloseHandle(hDevice);
					}
				}
			}
			// Free the device-information-set.
			SetupApi.SetupDiDestroyDeviceInfoList(deviceInfoSet);
			return deviceList;
		}

		public Hid.Caps GetCaps()
		{
			Hid.Caps caps;
			Hid.HidP_GetCaps(preparsedData, out caps);
			return caps;
		}

		public void Dispose()
		{
			Kernel32.CloseHandle(Handle);
			Hid.HidD_FreePreparsedData(preparsedData);
			Hid.HidD_FreePreparsedData(preparsedData);
		}

		// Use Open to get device objects
		private HidDevice(IntPtr handle, string devicePath, int vendorId, int productId)
		{
			Handle = handle;
			DevicePath = devicePath;
			VendorId = vendorId;
			ProductId = productId;
			Hid.HidD_GetPreparsedData(handle, out preparsedData);
		}

		public IntPtr Handle { get; private set; }
		public string DevicePath { get; private set; }
		private IntPtr preparsedData;
	}

	// Hid.dll definitions
	internal static class Hid
	{

		#region Structures

		public struct DeviceAttributes
		{
			public int Size;
			public ushort VendorId;
			public ushort ProductId;
			public ushort VersionNumber;
		}

		public struct Caps
		{
			public ushort Usage;
			public ushort UsagePage;
			public ushort InputReportByteLength;
			public ushort OutputReportByteLength;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
			public ushort[] Reserved;
			public ushort NumberLinkCollectionNodes;
			public ushort NumberInputButtonCaps;
			public ushort NumberInputValueCaps;
			public ushort NumberInputDataIndices;
			public ushort NumberOutputButtonCaps;
			public ushort NumberOutputValueCaps;
			public ushort NumberOutputDataIndices;
			public ushort NumberFeatureButtonCaps;
			public ushort NumberFeatureValueCaps;
			public ushort NumberFeatureDataIndices;
		}

		#endregion

		#region Functions

		[DllImport("hid.dll")]
		public static extern void HidD_GetHidGuid(
			out Guid hidGuid);

		[DllImport("hid.dll")]
		public static extern bool HidD_GetAttributes(
			IntPtr hidDeviceObject,
			out DeviceAttributes attributes);

		[DllImport("hid.dll")]
		public static extern bool HidD_SetFeature(
			IntPtr hidDeviceObject,
			byte[] reportBuffer,
			int reportBufferLength);

		[DllImport("hid.dll")]
		public static extern bool HidD_GetFeature(
			IntPtr hidDeviceObject,
			byte[] reportBuffer,
			int reportBufferLength);

		[DllImport("hid.dll")]
		public static extern bool HidD_GetPreparsedData(
			IntPtr hidDeviceObject,
			out IntPtr preparsedData);

		[DllImport("hid.dll")]
		public static extern bool HidD_FreePreparsedData(IntPtr preparsedData);

		[DllImport("hid.dll")]
		public static extern int HidP_GetCaps(IntPtr preparsedData, out Hid.Caps caps);

		[DllImport("hid.dll")]
		public static extern bool HidD_GetInputReport(
			IntPtr hidDeviceObject,
			ref byte[] reportBuffer,
			int reportBufferLength);

		[DllImport("hid.dll")]
		public static extern bool HidD_SetOutputReport(
			IntPtr hidDeviceObject,
			byte[] reportBuffer,
			int reportBufferLength);

		#endregion

	}

	// SetupApi.dll definitions
	internal static class SetupApi
	{

		#region Enumerations

		[Flags]
		public enum DiGetFlags
		{
			Default = 0x01,
			Present = 0x02,
			AllClasses = 0x04,
			Profile = 0x08,
			DeviceInterface = 0x10,
		}

		[Flags]
		public enum SpIntFlags
		{
			Active = 0x1,
			Default = 0x2,
			Removed = 0x4,
		}

		#endregion

		#region Structures

		public struct SpDeviceInterfaceData
		{
			public int Size;
			public Guid interfaceClassGuid;
			public SpIntFlags flags;
			public IntPtr Reserved;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Auto)]
		public struct SpDeviceInterfaceDetailData
		{
			public int Size;
			// SizeConst is MaxPath
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string DevicePath;
		}

		#endregion

		#region Functions

		[DllImport("setupapi.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SetupDiGetClassDevs(
			ref Guid classGuid,
			string enumerator,
			IntPtr hWndParent,
			DiGetFlags flags);

		// Free device info set returned by SetupDiGetClassDevs
		[DllImport("setupapi.dll")]
		public static extern bool SetupDiDestroyDeviceInfoList(
			IntPtr DeviceInfoSet);

		[DllImport("setupapi.dll", SetLastError = true)]
		public static extern bool SetupDiEnumDeviceInterfaces(
			IntPtr deviceInfoSet,
			IntPtr deviceInfoData, // actually struct ref but optional
			ref Guid interfaceClassGuid,
			int memberIndex,
			ref SpDeviceInterfaceData deviceInterfaceData);

		[DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern bool SetupDiGetDeviceInterfaceDetail(
			IntPtr deviceInfoSet,
			ref SpDeviceInterfaceData deviceInterfaceData,
			ref SpDeviceInterfaceDetailData deviceInterfaceDetailData,
			int deviceInterfaceDetailDataSize,
			IntPtr unusedRequiredSize,
			IntPtr unusedDeviceInfoData); // actually struct ref but optional

		#endregion

	}

	// Kernel32.dll definitions
	internal static class Kernel32
	{

		#region Constants

		public static readonly IntPtr InvalidHandleValue = (IntPtr)(-1);

		#endregion

		#region Functions

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr CreateFile(
			string fileName,
			FileAccess desiredAccess,
			FileShare shareMode,
			IntPtr securityAttributes, // struct but optional
			FileMode mode,
			FileAttributes flagsAndAttributes,
			IntPtr templateFile);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool CloseHandle(IntPtr handle);

		#endregion

	}

	internal static class User32
	{
		internal enum DBCHDeviceType : int
		{
			DeviceInterface = 0x00000005,
			Handle = 0x00000006,
			OEM = 0x00000000,
			Port = 0x00000003,
			Volume = 0x00000002,
		}
		
	
		// thta cant be it
		// wtf is this man
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		internal struct BroadcastDeviceInterface
		{
			public Guid ClasssorGuid;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string Name;
		}

		internal struct BroadcastHdr
		{
			public int Size;
			public DBCHDeviceType DeviceType;
			public int Reserved;
			[StructLayout(LayoutKind.Explicit)]
			public struct Data
			{
				[FieldOffset(0)]
				public BroadcastDeviceInterface Interface;
			}
			public Data DataX;
		}

		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		internal static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, BroadcastHdr notificationFilter, uint flags);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool UnregisterDeviceNotification(IntPtr handle);
	}

}
