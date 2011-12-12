using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace LgLcd {

	public static class BackLight {

		public static Color GetBackLight() {
			return currentColor;
		}

		public static void SetBackLight(Color color) {
			// Create the feature buffer.
			byte[] featureBuffer = new byte[] { reportId, color.R, color.G, color.B };
			// Send it to all devices we know of.
			foreach (var device in devices) {
				device.Value.SetFeature(featureBuffer);
			}
			currentColor = color;
		}

		public static void Initialize(IntPtr windowHandle) {
			// Open all devices we can find for now.
			var devs = HidDevice.Open(vendorId, productIds);
			foreach (var dev in devs) {
				// Test if the device is a backlight device we support.
				if (IsBackLightDevice(dev)) {
					// If so, add it to our dictionary with its path as key.
					devices.Add(dev.DevicePath, dev);
				}
				else {
					// Otherwise dispose of it!
					dev.Dispose();
				}
			}
			// Initialize device event dispatcher.
			Guid hidClassGuid;
			Hid.HidD_GetHidGuid(out hidClassGuid);
			deviceEventDispatcher = new DeviceEventDispatcher(windowHandle, hidClassGuid);
			// Register for device removal/arrival events so we can keep
			// track of the currently attached devices of interest.
			deviceEventDispatcher.DeviceArrival += DeviceAdded;
			deviceEventDispatcher.DeviceRemoval += DeviceRemoved;
		}

		static BackLight() {
			if (deviceEventDispatcher == null) {
				throw new ApplicationException(
					"You must first call Initialize to use the BackLight features.");
			}
		}

		private static bool IsBackLightDevice(HidDevice hidDevice) {
			return new NotImplementedException();
		}

		private static void DeviceAdded(object sender, DeviceEventArgs args) {
			HidDevice hidDevice = HidDevice.Open(args.DevicePath);
			// We can be sure a hid device was be returned.
			// Test if the device is a backlight device we support.
			if (IsBackLightDevice(hidDevice)) {
				// If so, add it to our dictionary with its path as key.
				devices.Add(hidDevice.DevicePath, hidDevice);
			}
			else {
				// Dispose of it otherwise!
				hidDevice.Dispose();
			}
		}

		private static void DeviceRemoved(object sender, DeviceEventArgs args) {
			// If a device was removed which exists in our dictionary,
			// then this is the time to dispose and remove it.
			if (devices.ContainsKey(args.DevicePath)) {
				devices[args.DevicePath].Dispose();
				devices.Remove(args.DevicePath);
			}
		}
		
		static DeviceEventDispatcher deviceEventDispatcher;
		static Dictionary<string, HidDevice> devices;
		static Color currentColor;

		// Logitech VendorId
		const int vendorId = 0x046D;
		const int[] productIds = new int[] {
			// G19 ProductId
			0xC229,
		};
		const int reportId = 7;

	}

}
