using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LgBackLight {

	public static class BackLight {

		public static Color GetBackLight() {
			AssertInitialized();
			return currentColor;
		}

		public static void SetBackLight(Color color) {
			AssertInitialized();
			// Create the feature buffer.
			byte[] featureBuffer = new byte[] { reportId, color.R, color.G, color.B };
			// Send it to all devices we know of.
			foreach (var device in devices) {
				device.Value.SetFeature(featureBuffer);
			}
			currentColor = color;
		}

		public static void Initialize(IntPtr windowHandle) {
			// Initialize device event dispatcher.
			Guid hidClassGuid;
			Hid.HidD_GetHidGuid(out hidClassGuid);
			deviceEventNotifier = new DeviceEventNotifier(windowHandle, hidClassGuid);
			// Register for device removal/arrival events so we can keep
			// track of the currently attached devices of interest.
			deviceEventNotifier.DeviceArrival += DeviceAdded;
			deviceEventNotifier.DeviceRemoval += DeviceRemoved;
			// Open all devices we can find for now.
			devices = new Dictionary<string, HidDevice>();
			var devs = HidDevice.Open(vendorId, productIds);
			foreach (var dev in devs) {
				devices.Add(dev.DevicePath.ToUpper(), dev);
			}
		}

		private static void AssertInitialized() {
			if (deviceEventNotifier == null) {
				throw new ApplicationException(
					"You must first call Initialize to use the BackLight features.");
			}
		}

		private static Color ReadBackLight(HidDevice device) {
			byte[] b = new byte[4];
			b[0] = reportId;
			device.GetFeature(b);
			return Color.FromArgb(b[1], b[2], b[3]);
		}

		private static void DeviceAdded(object sender, DeviceEventArgs args) {
			HidDevice hidDevice = HidDevice.Open(args.DevicePath);
			// We can be sure a hid device was be returned,
			// since we only registered device notifications
			// for hid devices.
			if (vendorId == hidDevice.VendorId
				&& productIds.Contains(hidDevice.ProductId)) {
				// The device added appears to be a backlight device.
				devices.Add(args.DevicePath.ToUpper(), hidDevice);
			}
			else {
				// Not a backlight device, dispose!
				hidDevice.Dispose();
			}
		}

		private static void DeviceRemoved(object sender, DeviceEventArgs args) {
			// If a device was removed which exists in our dictionary,
			// then this is the time to dispose and remove it.
			string path = args.DevicePath.ToUpper();
			if (devices.ContainsKey(path)) {
				devices[path].Dispose();
				devices.Remove(path);
			}
		}
		
		static DeviceEventNotifier deviceEventNotifier;
		static Dictionary<string, HidDevice> devices;
		static Color currentColor;

		// Logitech VendorId
		static readonly int vendorId = 0x046D;
		static readonly int[] productIds = new int[] {
			// G19 ProductId
			0xC229,
		};
		static readonly byte reportId = 7;

	}

}
