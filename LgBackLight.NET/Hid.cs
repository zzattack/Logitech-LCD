using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace LgBackLight {

	public class HidDevice : IDisposable {

		public int VendorId { get; private set; }
		public int ProductId { get; private set; }
		public int VersionNumber { get; private set; }
		public string DevicePath { get; private set; }

		internal bool GetFeature(byte[] reportBuffer) {
			return Hid.HidD_GetFeature(handle, reportBuffer, reportBuffer.Length);
		}

        internal bool SetFeature(byte[] reportBuffer)
        {
			return Hid.HidD_SetFeature(handle, reportBuffer, reportBuffer.Length);
		}

        internal bool GetInputReport(byte[] reportBuffer)
        {
			return Hid.HidD_GetInputReport(handle, reportBuffer, reportBuffer.Length);
		}

        internal bool SetOutputReport(byte[] reportBuffer)
        {
			return Hid.HidD_SetOutputReport(handle, reportBuffer, reportBuffer.Length);
		}

        internal static List<HidDevice> Open(int vendorId, int productId)
        {
			return Open(vendorId, new int[] { productId });
		}

        internal static List<HidDevice> Open(int vendorId, IEnumerable<int> productIds)
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
			if (deviceInfoSet == Kernel32.InvalidHandleValue) {
				throw new Win32Exception();
			}
			// Retrieve all available interface information.
			SetupApi.SpDeviceInterfaceData did = new SetupApi.SpDeviceInterfaceData();
			did.Size = Marshal.SizeOf(did);
			int i = 0;
			while (SetupApi.SetupDiEnumDeviceInterfaces(deviceInfoSet, IntPtr.Zero, ref hidClassGuid, i++, ref did)) {
				// Obtain DevicePath
				var didd = new SetupApi.SpDeviceInterfaceDetailData();
				// On x64 only 8 appears to work while on x86 it must be sizeof (int) + sizeof (TCHAR)
				didd.Size = IntPtr.Size == 8 ? 8 : didd.Size + Marshal.SystemDefaultCharSize;
				if (SetupApi.SetupDiGetDeviceInterfaceDetail(
					deviceInfoSet,
					ref did,
					ref didd,
					Marshal.SizeOf(didd),
					IntPtr.Zero,
					IntPtr.Zero)) {
					// Write access causes failure if not running elevated.
					IntPtr hDevice = Kernel32.CreateFile(
						didd.DevicePath,
						(uint)FileAccess.Read, // we just need read
						FileShare.ReadWrite, // and we don't want to restrict others from write
						IntPtr.Zero,
						FileMode.Open,
						FileAttributes.Device,
						IntPtr.Zero);
					if (hDevice != Kernel32.InvalidHandleValue) {
						Hid.DeviceAttributes attributes;
						if (Hid.HidD_GetAttributes(hDevice, out attributes)) {
							if (vendorId == attributes.VendorId && productIds.Contains(attributes.ProductId)) {
								deviceList.Add(
									new HidDevice(
										hDevice,
										didd.DevicePath,
										vendorId,
										attributes.ProductId,
										attributes.VersionNumber));
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

        internal static HidDevice Open(string devicePath)
        {
			IntPtr handle = Kernel32.CreateFile(
				devicePath,
				(uint)FileAccess.Read,
				FileShare.ReadWrite,
				IntPtr.Zero,
				FileMode.Open,
				FileAttributes.Device,
				IntPtr.Zero);
			if (handle != Kernel32.InvalidHandleValue) {
				Hid.DeviceAttributes attributes;
				if (Hid.HidD_GetAttributes(handle, out attributes)) {
					return new HidDevice(
						handle,
						devicePath,
						attributes.VendorId,
						attributes.ProductId,
						attributes.VersionNumber);
				}
				// If we could not get the hid attributes,
				// then what we opened was not a hid device,
				// we need to close the handle nonetheless.
				Kernel32.CloseHandle(handle);
			}
			throw new Exception();
		}

        internal Hid.Caps GetCaps()
        {
			Hid.Caps caps;
			Hid.HidP_GetCaps(preparsedData, out caps);
			return caps;
		}

		public void Dispose() {
			Kernel32.CloseHandle(handle);
			Hid.HidD_FreePreparsedData(preparsedData);
		}

		// Use Open to get device objects
		private HidDevice(IntPtr handle, string devicePath, int vendorId, int productId, int versionNumber) {
			this.handle = handle;
			DevicePath = devicePath;
			VendorId = vendorId;
			ProductId = productId;
			VersionNumber = versionNumber;
			Hid.HidD_GetPreparsedData(handle, out preparsedData);
		}

		private IntPtr handle;
		private IntPtr preparsedData;
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

		public struct Caps {
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
			byte[] reportBuffer,
			int reportBufferLength);

		[DllImport("hid.dll")]
		public static extern bool HidD_SetOutputReport(
			IntPtr hidDeviceObject,
			byte[] reportBuffer,
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

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public struct SpDeviceInterfaceDetailData {
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
			IntPtr deviceInfoData, // actually struct but optional
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
			IntPtr unusedDeviceInfoData); // actually struct but optional

		#endregion

	}

	// Kernel32.dll definitions
	internal static class Kernel32 {

		#region Constants

		public static readonly IntPtr InvalidHandleValue = (IntPtr)(-1);

		#endregion

		#region Functions

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr CreateFile(
			string fileName,
			uint desiredAccess,
			FileShare shareMode,
			IntPtr securityAttributes, // struct but optional
			FileMode mode,
			FileAttributes flagsAndAttributes,
			IntPtr templateFile);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool CloseHandle(IntPtr handle);

		#endregion

	}

}
