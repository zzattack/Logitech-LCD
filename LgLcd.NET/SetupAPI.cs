using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
//----------------------------------------------------------------------
// Created with SharpDevelop
// Author: Adam Yarnott, Blue Ninja Software
// Copyright (c) Adam Yarnott, Blue Ninja Software.
// Date: 8/22/2007
// Time: 12:42 PM
//----------------------------------------------------------------------

using System.Runtime.InteropServices;

namespace Interop {

	/// <summary>
	/// Interop declarations from setupapi.dll.
	/// </summary>
	static internal class SetupAPI {

		#region "Constants"

		//from dbt.h
		public const int DBT_DEVICEARRIVAL = 0x8000;
		public const int DBT_DEVICEREMOVECOMPLETE = 0x8004;

		public const int WM_DEVICECHANGE = 0x219;

		//from setupapi.h
		public const short DIGCF_PRESENT = 0x2;
		public const short DIGCF_DEVICEINTERFACE = 0x10;

		#endregion

		#region "Enums"

		/// <summary>
		/// Defines Flags member for the SP_DEVICE_INTERFACE_DATA structure.
		/// </summary>
		[Flags()]
		public enum SPIntFlagsEnum : int {

			/// <summary>The interface is active (enabled).</summary>
			ACTIVE = 1,

			/// <summary>The interface is the default interface for the device class.</summary>
			DEFAULT = 2,

			/// <summary>The interface is removed.</summary>
			REMOVED = 4

		}

		/// <summary>
		/// A variable of type DWORD that specifies control options that filter the device information elements that are added to the device information set. This parameter can be a bitwise OR of zero or more of the following flags.
		/// </summary>
		[Flags()]
		public enum DiGetClassFlagsEnum : int {

			/// <summary>Return only the device that is associated with the system default device interface, if one is set, for the specified device interface classes.</summary>
			DEFAULT = 1,

			/// <summary>Return only devices that are currently present in a system.</summary>
			PRESENT = 2,

			/// <summary>Return a list of installed devices for all device setup classes or all device interface classes.</summary>
			ALLCLASSES = 4,

			/// <summary>Return only devices that are a part of the current hardware profile.</summary>
			PROFILE = 8,

			/// <summary>Return devices that support device interfaces for the specified device interface classes.</summary>
			DEVICEINTERFACE = 0x10

		}

		#endregion

		#region "Structures"

		/// <summary>
		/// An SP_DEVICE_INTERFACE_DATA structure defines a device interface in a device information set.
		/// </summary>
		/// <remarks>A SetupAPI function that takes an instance of the SP_DEVICE_INTERFACE_DATA structure as a parameter verifies whether the cbSize member of the supplied structure is equal to the size, in bytes, of the structure. If the cbSize member is not correctly set, the function will fail and set an error code of ERROR_INVALID_USER_BUFFER.</remarks>
		[StructLayout(LayoutKind.Sequential)]
		public struct SP_DEVICE_INTERFACE_DATA {

			/// <summary>The size, in bytes, of the SP_DEVICE_INTERFACE_DATA structure. For more information, see the Comments section.</summary>
			public int cbSize;

			/// <summary>The GUID for the class to which the device interface belongs.</summary>
			public Guid InterfaceClassGuid;

			/// <summary></summary>
			public SPIntFlagsEnum Flags;

			/// <summary>Reserved. Do not use.</summary>
			public IntPtr Reserved;

		}

		/// <summary>
		/// An SP_DEVICE_INTERFACE_DETAIL_DATA structure contains the path for a device interface.
		/// </summary>
		[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
		public struct SP_DEVICE_INTERFACE_DETAIL_DATA {

			/// <summary>The size, in bytes, of the fixed portion of the SP_DEVICE_INTERFACE_DETAIL_DATA structure.</summary>
			public uint cbSize;

			/// <summary>A NULL-terminated string that contains the device interface path. This path can be passed to Win32 functions such as CreateFile.</summary>
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
			public string DevicePath;

		}

		/// <summary>
		/// An SP_DEVINFO_DATA structure defines a device instance that is a member of a device information set.
		/// </summary>
		/// <remarks>An SP_DEVINFO_DATA structure identifies a device in a device information set. For example, when Setup sends a DIF_INSTALLDEVICE request to a class installer and co-installers, it includes a handle to a device information set and a pointer to an SP_DEVINFO_DATA that specifies the particular device. Besides DIF requests, this structure is also used in some SetupDiXxx functions.
		/// 	<para>SetupDiXxx functions that take an SP_DEVINFO_DATA structure as a parameter verify whether the cbSize member of the supplied structure is equal to the size, in bytes, of the structure. If the cbSize member is not correctly set for an input parameter, the function will fail and set an error code of ERROR_INVALID_PARAMETER. If the cbSize member is not correctly set for an output parameter, the function will fail and set an error code of ERROR_INVALID_USER_BUFFER.</para>
		/// </remarks>
		[StructLayout(LayoutKind.Sequential)]
		public struct SP_DEVINFO_DATA {

			/// <summary></summary>
			/// The size, in bytes, of the SP_DEVINFO_DATA structure. For more information, see the following Comments section.
			public int cbSize;

			/// <summary>The GUID of the device's setup class.</summary>
			public Guid ClassGuid;

			/// <summary>An opaque handle to the device instance (also known as a handle to the devnode).
			/// 	<para>Some functions, such as SetupDiXxx functions, take the whole SP_DEVINFO_DATA structure as input to identify a device in a device information set. Other functions, such as CM_Xxx functions like CM_Get_DevNode_Status, take this DevInst handle as input.</para>
			/// </summary>
			public int DevInst;

			/// <summary>Reserved. For internal use only.</summary>
			public IntPtr Reserved;

		}

		#endregion

		#region "Imports"

		/// <summary>
		/// The SetupDiCreateDeviceInfoList function creates an empty device information set and optionally associates the set with a device setup class and a top-level window.
		/// </summary>
		/// <param name="ClassGuid">A pointer to the GUID of the device setup class to associate with the newly created device information set. If this parameter is specified, only devices of this class can be included in this device information set. If this parameter is set to NULL, the device information set is not associated with a specific device setup class.</param>
		/// <param name="hwndParent">A handle to the top-level window to use for any user interface that is related to non-device-specific actions (such as a select-device dialog box that uses the global class driver list). This handle is optional and can be NULL. If a specific top-level window is not required, set hwndParent to NULL.</param>
		/// <returns>The function returns a handle to an empty device information set if it is successful. Otherwise, it returns INVALID_HANDLE_VALUE. To get extended error information, call GetLastError.</returns>
		/// <remarks>The caller of this function must delete the returned device information set when it is no longer needed by calling SetupDiDestroyDeviceInfoList.
		/// 	<para>To create a device information list for a remote machine use SetupDiCreateDeviceInfoListEx.</para>
		/// </remarks>
		[DllImport("setupapi.dll", SetLastError = true)]
		public static int SetupDiCreateDeviceInfoList(ref Guid ClassGuid, int hwndParent) {
		}

		/// <summary>
		/// The SetupDiDestroyDeviceInfoList function deletes a device information set and frees all associated memory.
		/// </summary>
		/// <param name="DeviceInfoSet">A handle to the device information set to delete.</param>
		/// <returns>The function returns TRUE if it is successful. Otherwise, it returns FALSE and the logged error can be retrieved with a call to GetLastError.</returns>
		[DllImport("setupapi.dll", SetLastError = true)]
		public static int SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet) {
		}

		/// <summary>
		/// The SetupDiEnumDeviceInterfaces function enumerates the device interfaces that are contained in a device information set. 
		/// </summary>
		/// <param name="DeviceInfoSet">A pointer to a device information set that contains the device interfaces for which to return information. This handle is typically returned by SetupDiGetClassDevs.</param>
		/// <param name="DeviceInfoData">A pointer to an SP_DEVINFO_DATA structure that specifies a device information element in DeviceInfoSet. This parameter is optional and can be NULL. If this parameter is specified, SetupDiEnumDeviceInterfaces constrains the enumeration to the interfaces that are supported by the specified device. If this parameter is NULL, repeated calls to SetupDiEnumDeviceInterfaces return information about the interfaces that are associated with all the device information elements in DeviceInfoSet. This pointer is typically returned by SetupDiEnumDeviceInfo.</param>
		/// <param name="InterfaceClassGuid">A pointer to a GUID that specifies the device interface class for the requested interface.</param>
		/// <param name="MemberIndex">A zero-based index into the list of interfaces in the device information set. The caller should call this function first with MemberIndex set to zero to obtain the first interface. Then, repeatedly increment MemberIndex and retrieve an interface until this function fails and GetLastError returns ERROR_NO_MORE_ITEMS.
		/// 	<para>If DeviceInfoData specifies a particular device, the MemberIndex is relative to only the interfaces exposed by that device.</para>
		/// </param>
		/// <param name="DeviceInterfaceData">A pointer to a caller-allocated buffer that contains, on successful return, a completed SP_DEVICE_INTERFACE_DATA structure that identifies an interface that meets the search parameters. The caller must set DeviceInterfaceData.cbSize to sizeof(SP_DEVICE_INTERFACE_DATA) before calling this function.</param>
		/// <returns>SetupDiEnumDeviceInterfaces returns TRUE if the function completed without error. If the function completed with an error, FALSE is returned and the error code for the failure can be retrieved by calling GetLastError.</returns>
		/// <remarks>Repeated calls to this function return an SP_DEVICE_INTERFACE_DATA structure for a different device interface. This function can be called repeatedly to get information about interfaces in a device information set that are associated with a particular device information element or that are associated with all device information elements.
		/// 	<para>DeviceInterfaceData points to a structure that identifies a requested device interface. To get detailed information about an interface, call SetupDiGetDeviceInterfaceDetail. The detailed information includes the name of the device interface that can be passed to a Win32 function such as CreateFile (described in Microsoft Windows SDK documentation) to get a handle to the interface.</para>
		/// </remarks>
		[DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static bool SetupDiEnumDeviceInterfaces(IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, ref Guid InterfaceClassGuid, uint MemberIndex, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData) {
		}

		[DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static bool SetupDiEnumDeviceInterfaces(IntPtr DeviceInfoSet, IntPtr DeviceInfoData, ref Guid InterfaceClassGuid, uint MemberIndex, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData) {
			//TODO: Find out why DeviceInfoData can't be SP_DEVINFO_DATA and receive Nothing as a param...
		}

		/// <summary>
		/// The SetupDiGetClassDevs function returns a handle to a device information set that contains requested device information elements for a local machine. 
		/// </summary>
		/// <param name="ClassGuid">A pointer to the GUID for a device setup class or a device interface class. This pointer is optional and can be NULL. For more information about how to set ClassGuid, see the following Comments section.</param>
		/// <param name="Enumerator">A pointer to a NULL-terminated string that supplies the name of a Plug and Play (PnP) enumerator or a PnP device instance identifier. This pointer is optional and can be NULL. For more information about how to set the Enumerator value, see the following Comments section.</param>
		/// <param name="hwndParent">A handle of the top-level window to be used for a user interface that is associated with installing a device instance in the device information set. This handle is optional and can be NULL.</param>
		/// <param name="Flags">A variable of type DWORD that specifies control options that filter the device information elements that are added to the device information set. This parameter can be a bitwise OR of zero or more of the following flags. For more information about combining these flags, see the following Comments section.</param>
		/// <returns>If the operation succeeds, SetupDiGetClassDevs returns a handle to a device information set that contains all installed devices that matched the supplied parameters. If the operation fails, the function returns INVALID_HANDLE_VALUE or another appropriate error. To get extended error information, call GetLastError. GetLastError can return one of the following error values:
		/// 	<para>ERROR_INVALID_PARAMETER: A supplied parameter is invalid.</para>
		/// 	<para>ERROR_INVALID_FLAGS: The DIGCF_DEFAULT flag is not used with the DIGCF_DEVICEINTERFACES flag.</para>
		/// </returns>
		/// <remarks>The caller of SetupDiGetClassDevs must delete the returned device information set when it is no longer needed by calling SetupDiDestroyDeviceInfoList.
		/// <para>Call SetupDiGetClassDevsEx to retrieve the devices for a class on a remote machine.</para>
		/// 	<b>Device Setup Class Control Options</b>
		/// 	<para>Use the following filtering options to control whether SetupDiGetClassDevs returns devices for all device setup classes or only for a specified device setup class:</para>
		/// 	<ul>
		/// 		<li>To return devices for all device setup classes, set the DIGCF_ALLCLASSES flag, and set the ClassGuid parameter to NULL.</li>
		/// 		<li>To return devices only for a specific device setup class, do not set DIGCF_ALLCLASSES, and use ClassGuid to supply the GUID of the device setup class.</li>
		/// 	</ul>
		/// 	<para>In addition, you can use the following filtering options in combination with one another to further restrict which devices are returned:</para>
		/// 	<ul>
		/// 		<li>To return only devices that are present in the system, set the DIGCF_PRESENT flag.</li>
		/// 		<li>To return only devices that are part of the current hardware profile, set the DIGCF_PROFILE flag.</li>
		/// 		<li>To return devices only for a specific PnP enumerator, use the Enumerator parameter to supply the name of the enumerator. If Enumerator is NULL, SetupDiGetClassDevs returns devices for all PnP enumerators.</li>
		/// 	</ul>
		/// 	<b>Device Interface Class Control Options</b>
		/// 	<para>Use the following filtering options to control whether SetupDiGetClassDevs returns devices that support any device interface class or only devices that support a specified device interface class:</para>
		/// 	<ul>
		/// 		<li>To return devices that support a device interface of any class, set the DIFCF_DEVICEINTERFACE flag, set the DIGCF_ALLCLASSES flag, and set ClassGuid to NULL. The function adds to the device information set a device information element that represents such a device and then adds to the device information element a device interface list that contains all the device interfaces that the device supports.</li>
		/// 		<li>To return only devices that support a device interface of a specified class, set the DIFCF_DEVICEINTERFACE flag and use the ClassGuid parameter to supply the class GUID of the device interface class. The function adds to the device information set a device information element that represents such a device and then adds a device interface of the specified class to the device interface list for that device information element.</li>
		/// 	</ul>
		/// 	<para>In addition, you can use the following filtering options to control whether SetupDiGetClassDevs returns only devices that support the system default interface for device interface classes:</para>
		/// 	<ul>
		/// 		<li>To return only the device that supports the system default interface, if one is set, for a specified device interface class, set the DIFCF_DEVICEINTERFACE flag, set the DIGCF_DEFAULT flag, and use ClassGuid to supply the class GUID of the device interface class. The function adds to the device information set a device information element that represents such a device and then adds the system default interface to the device interface list for that device information element.</li>
		/// 		<li>To return a device that supports a system default interface for an unspecified device interface class, set the DIFCF_DEVICEINTERFACE flag, set the DIGCF_ALLCLASSES flag, set the DIGCF_DEFAULT flag, and set ClassGuid to NULL. The function adds to the device information set a device information element that represents such a device and then adds the system default interface to the device interface list for that device information element.</li>
		/// 	</ul>
		/// 	<para>You can also use the following options in combination with the other options to further restrict which devices are returned:</para>
		/// 	<ul>
		/// 		<li>To return only devices that are present in the system, set the DIGCF_PRESENT flag.</li>
		/// 		<li>To return only devices that are part of the current hardware profile, set the DIGCF_PROFILE flag.</li>
		/// 		<li>To return only a specific device, use the Enumerator parameter to supply the device instance identifier of the device. To include all possible devices, set Enumerator to NULL.</li>
		/// 	</ul>
		/// </remarks>
		[DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, [MarshalAs(UnmanagedType.LPTStr)]
string Enumerator, int hwndParent, DiGetClassFlagsEnum Flags) {
		}

		/// <summary>
		/// The SetupDiGetDeviceInterfaceDetail function returns details about a device interface.
		/// </summary>
		/// <param name="DeviceInfoSet">A pointer to the device information set that contains the interface for which to retrieve details. This handle is typically returned by SetupDiGetClassDevs.</param>
		/// <param name="DeviceInterfaceData">A pointer to an SP_DEVICE_INTERFACE_DATA structure that specifies the interface in DeviceInfoSet for which to retrieve details. A pointer of this type is typically returned by SetupDiEnumDeviceInterfaces.</param>
		/// <param name="DeviceInterfaceDetailData">A pointer to an SP_DEVICE_INTERFACE_DETAIL_DATA structure to receive information about the specified interface. This parameter is optional and can be NULL. This parameter must be NULL if DeviceInterfaceDetailSize is zero. If this parameter is specified, the caller must set DeviceInterfaceDetailData.cbSize to sizeof(SP_DEVICE_INTERFACE_DETAIL_DATA) before calling this function. The cbSize member always contains the size of the fixed part of the data structure, not a size reflecting the variable-length string at the end.</param>
		/// <param name="DeviceInterfaceDetailDataSize">The size of the DeviceInterfaceDetailData buffer. The buffer must be at least (offsetof(SP_DEVICE_INTERFACE_DETAIL_DATA, DevicePath) + sizeof(TCHAR)) bytes, to contain the fixed part of the structure and a single NULL to terminate an empty MULTI_SZ string.
		/// 	<para>This parameter must be zero if DeviceInterfaceDetailData is NULL.</para>
		/// </param>
		/// <param name="RequiredSize">A pointer to a variable of type DWORD that receives the required size of the DeviceInterfaceDetailData buffer. This size includes the size of the fixed part of the structure plus the number of bytes required for the variable-length device path string. This parameter is optional and can be NULL.</param>
		/// <param name="DeviceInfoData">A pointer buffer to receive information about the device that supports the requested interface. The caller must set DeviceInfoData.cbSize to sizeof(SP_DEVINFO_DATA). This parameter is optional and can be NULL.</param>
		/// <returns>SetupDiGetDeviceInterfaceDetail returns TRUE if the function completed without error. If the function completed with an error, FALSE is returned and the error code for the failure can be retrieved by calling GetLastError.</returns>
		/// <remarks>Using this function to get details about an interface is typically a two-step process:
		/// 	<ol>
		/// 	<li>Get the required buffer size. Call SetupDiGetDeviceInterfaceDetail with a NULL DeviceInterfaceDetailData pointer, a DeviceInterfaceDetailDataSize of zero, and a valid RequiredSize variable. In response to such a call, this function returns the required buffer size at RequiredSize and fails with GetLastError returning ERROR_INSUFFICIENT_BUFFER.</li>
		/// 	<li>Allocate an appropriately sized buffer and call the function again to get the interface details.</li>
		/// 	</ol>
		/// 	<para>The interface detail returned by this function consists of a device path that can be passed to Win32 functions such as CreateFile. Do not attempt to parse the device path symbolic name. The device path can be reused across system boots.</para>
		/// 	<para>SetupDiGetDeviceInterfaceDetail can be used to get just the DeviceInfoData. If the interface exists but DeviceInterfaceDetailData is NULL, this function fails, GetLastError returns ERROR_INSUFFICIENT_BUFFER, and the DeviceInfoData structure is filled with information about the device that exposes the interface.</para>
		/// </remarks>
		[DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, ref SP_DEVICE_INTERFACE_DETAIL_DATA DeviceInterfaceDetailData, int DeviceInterfaceDetailDataSize, ref int RequiredSize, ref SP_DEVINFO_DATA DeviceInfoData);

		[DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, IntPtr DeviceInterfaceDetailData, int DeviceInterfaceDetailDataSize, ref int RequiredSize, IntPtr DeviceInfoData);

		[DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, ref SP_DEVICE_INTERFACE_DETAIL_DATA DeviceInterfaceDetailData, int DeviceInterfaceDetailDataSize, ref int RequiredSize, IntPtr DeviceInfoData);

		#endregion

	}

}
