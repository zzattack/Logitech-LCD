using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Collections.Generic;

namespace LgLcd {
	
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
			while (SetupApi.SetupDiEnumDeviceInterfaces(
				deviceInfoSet,
				IntPtr.Zero,
				ref hidClassGuid,
				i++,
				ref did)) {
				// Obtain DevicePath
				var didd = new SetupApi.SpDeviceInterfaceDetailData();
				didd.Size = 4 + Marshal.SystemDefaultCharSize;
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
						Kernel32.GenericAccess.Read,
						Kernel32.FileShareMode.ShareRead | Kernel32.FileShareMode.ShareWrite,
						IntPtr.Zero,
						Kernel32.CreationDisposition.OpenExisting,
						Kernel32.FileAttribute.Overlapped,
						IntPtr.Zero);
					if (hDevice != Kernel32.InvalidHandleValue) {
						Hid.DeviceAttributes attributes;
						if (Hid.HidD_GetAttributes(hDevice, out attributes)) {
							if (attributes.VendorId == vendorId && attributes.ProductId == productId) {
								deviceList.Add(new HidDevice(vendorId, productId));
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

		public void Dispose() {
			if (deviceHandle != IntPtr.Zero) {
				Kernel32.CloseHandle(deviceHandle);
			}
		}

		// Use Open to get device objects
		private HidDevice(int vendorId, int productId) {
			VendorId = vendorId;
			ProductId = productId;
		}

		private IntPtr deviceHandle;
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

		[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
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
	internal static class Kernel32 {

		#region Constants

		public static readonly IntPtr InvalidHandleValue = (IntPtr)(-1);

		#endregion

		#region Enumerations

		[Flags]
		public enum GenericAccess : uint {
			All = 0x80000000,
			Execute = 0x40000000,
			Read = 0x20000000,
			Write = 0x10000000,
		}

		[Flags]
		public enum FileShareMode {
			ShareNot = 0,
			ShareRead = 1,
			ShareWrite = 2,
			ShareDelete = 4,
		}

		public enum CreationDisposition {
			CreateNew = 1,
			CreateAlways = 2,
			OpenExisting = 3,
			OpenAlways = 4,
			TruncateExisting = 5,
		}

		public enum FileAttribute {
			Overlapped = 0x40000000,
			// Theres more, way more
		}

		#endregion

		#region Functions

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr CreateFile(
			string fileName,
			GenericAccess desiredAccess,
			FileShareMode shareMode,
			IntPtr securityAttributes, // struct but optional
			CreationDisposition creationDisposition,
			FileAttribute flagsAndAttributes,
			IntPtr templateFile);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool CloseHandle(IntPtr handle);

		#endregion

	}

}
