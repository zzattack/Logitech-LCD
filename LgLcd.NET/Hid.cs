using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Collections.Generic;

namespace LgLcd {

	// For testing
	public static class Wtf {
		public static void X() {
			HidDevice.Open(0, 0);
		}
	}

	internal class HidDevice : IDisposable {

		public int VendorId { get; private set; }
		public int ProductId { get; private set; }

		bool GetFeature(ref byte[] reportBuffer) {
			return Hid.HidD_GetFeature(deviceHandle, ref reportBuffer, reportBuffer.Length);
		}

		public bool SetFeature(byte[] reportBuffer) {
			return Hid.HidD_SetFeature(deviceHandle, ref reportBuffer, reportBuffer.Length);
		}

		public static List<HidDevice> Open(int vendorId, int productId) {
			// Obtain system-defined GUID for HIDClass devices.
			Guid hidClassGuid;
			Hid.HidD_GetHidGuid(out hidClassGuid);
			var deviceList = new List<HidDevice>();

			// Obtain handle to an opaque device-information-set
			// describing device interface supported by all HID
			// collections currently installed in the system.
			IntPtr deviceInfoSet = SetupApi.SetupDiGetClassDevs(ref hidClassGuid, null, IntPtr.Zero, SetupApi.DiGetFlags.DeviceInterface | SetupApi.DiGetFlags.Present);

			if (deviceInfoSet.ToInt32() == -1)
				throw new Win32Exception(Marshal.GetLastWin32Error());

			// Retrieve all available interface information.
			SetupApi.SpDeviceInterfaceData did = new SetupApi.SpDeviceInterfaceData();
			did.Size = Marshal.SizeOf(did);
			int i = 0;
			while (SetupApi.SetupDiEnumDeviceInterfaces(deviceInfoSet, IntPtr.Zero, ref hidClassGuid, i++, ref did)) {
				// Obtain DevicePath
				int requiredSize;
				SetupApi.SetupDiGetDeviceInterfaceDetailSize(deviceInfoSet, ref did, IntPtr.Zero, 0, out requiredSize, IntPtr.Zero);

				var didd = new SetupApi.SpDeviceInterfaceDetailData();
				didd.Size = 4 + Marshal.SystemDefaultCharSize; // Marshal.SizeOf fucks up cuz of buffer vs pointer to buffer

				if (!SetupApi.SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref did, ref didd, Marshal.SizeOf(didd), IntPtr.Zero, IntPtr.Zero))
					throw new Win32Exception(Marshal.GetLastWin32Error());

				// todo: get some more info
				HidDevice hid = null;

				deviceList.Add(hid);
			}

			// Free the device-information-set.
			SetupApi.SetupDiDestroyDeviceInfoList(deviceInfoSet);

			return deviceList;
		}

		public void Dispose() {
			if (deviceHandle != IntPtr.Zero) {
				Kernel32.CloseHandle(deviceHandle);
				deviceHandle = IntPtr.Zero;
			}
		}

		// Use Open
		private HidDevice() { }

		private IntPtr deviceHandle = IntPtr.Zero;
	}

	// Hid.dll definitions
	internal static class Hid {

		#region Structures

		public struct DeviceAttributes {
			public int Size;
			public ushort VendorId;
			public ushort ProductId;
			public ushort VersionNumber;
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
			ref byte[] reportBuffer,
			int reportBufferLength);

		[DllImport("hid.dll")]
		public static extern bool HidD_GetFeature(
			IntPtr hidDeviceObject,
			ref byte[] reportBuffer,
			int reportBufferLength);

		#endregion

	}

	// SetupApi.dll definitions
	internal static class SetupApi {

		#region Enumerations

		[Flags]
		public enum DiGetFlags {
			Default = 0x01,
			Present = 0x02,
			AllClasses = 0x04,
			Profile = 0x08,
			DeviceInterface = 0x10,
		}

		[Flags]
		public enum SpIntFlags {
			Active = 0x1,
			Default = 0x2,
			Removed = 0x4,
		}

		#endregion

		#region Structures

		public struct SpDeviceInterfaceData {
			public int Size;
			public Guid interfaceClassGuid;
			public SpIntFlags flags;
			public IntPtr Reserved;
		}

		public struct SpDeviceInfoData {
			public int Size;
			public Guid ClassGuid;
			public int DevInst;
			public IntPtr Reserved;
		}

		// Class because nullable semantics are required
		[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
		public struct SpDeviceInterfaceDetailData {
			public int Size;
			// SizeConst is MaxPath
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
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

		// Delete device info set returned by SetupDiGetClassDevs
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

		[DllImport("setupapi.dll", EntryPoint = "SetupDiGetDeviceInterfaceDetail", SetLastError = true)]
		public static extern bool SetupDiGetDeviceInterfaceDetailSize(
			IntPtr deviceInfoSet,
			ref SpDeviceInterfaceData deviceInterfaceData,
			IntPtr unusedDeviceInterfaceDetailData,
			int unusedDeviceInterfaceDetailDataSize,
			out int requiredSize,
			IntPtr unusedDeviceInfoData); // actually struct ref but optional

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
	internal static class Kernel32 {
		#region Functions

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr CreateFile(
			string fileName,
			int desiredAccess,
			int shareMode,
			IntPtr securityAttributes, // struct but optional
			int creationDisposition,
			int flagsAndAttributes,
			IntPtr templateFile);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool CloseHandle(IntPtr handle);

		#endregion

	}

}
