using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LgBackLight
{
    public class LogitechKeyboardManager
    {
        private const int VENDOR_ID = 0x046D;

        private static DeviceEventNotifier deviceEventNotifier;
        private static List<LogitechKeyboard> devices;

        public static event EventHandler<LogitechKeyboardEventArgs> KeyboardConnected;
        public static event EventHandler<LogitechKeyboardEventArgs> KeyboardDisconnected;

        private static LogitechKeyboardTypes[] KeyboardsToListenFor;

        public static void Init(IntPtr windowHandle, params LogitechKeyboardTypes[] keyboardsToListenFor)
        {
            KeyboardsToListenFor = keyboardsToListenFor;

            // Initialize device event dispatcher.
            Guid hidClassGuid;
            Hid.HidD_GetHidGuid(out hidClassGuid);
            deviceEventNotifier = new DeviceEventNotifier(windowHandle, hidClassGuid);
            // Register for device removal/arrival events so we can keep
            // track of the currently attached devices of interest.
            deviceEventNotifier.DeviceArrival += DeviceAdded;
            deviceEventNotifier.DeviceRemoval += DeviceRemoved;

            devices = new List<LogitechKeyboard>();

            // Open all devices we can find for now.
            var devs = HidDevice.Open(VENDOR_ID, keyboardsToListenFor.Select(d => d.ProductID));

            foreach (var dev in devs)
            {
                devices.Add(new LogitechKeyboard(dev, keyboardsToListenFor.Where(k => k.ProductID == dev.ProductId).First()));
            }
        }

        private static void DeviceAdded(object sender, DeviceEventArgs args)
        {
            HidDevice hidDevice = HidDevice.Open(args.DevicePath);
            // We can be sure a hid device was be returned,
            // since we only registered device notifications
            // for hid devices.
            if (VENDOR_ID == hidDevice.VendorId && KeyboardsToListenFor.Any(d => d.ProductID == hidDevice.ProductId))
            {
                // The device added appears to be a backlight device.
                LogitechKeyboard keyboard = new LogitechKeyboard(hidDevice, KeyboardsToListenFor.Where(k => k.ProductID == hidDevice.ProductId).First());
                devices.Add(keyboard);
                if(KeyboardConnected != null)
                    KeyboardConnected(null, new LogitechKeyboardEventArgs(keyboard));
            }
            else
            {
                // Not a backlight device, dispose!
                hidDevice.Dispose();
            }
        }

        private static void DeviceRemoved(object sender, DeviceEventArgs args)
        {
            // If a device was removed which exists in our dictionary,
            // then this is the time to dispose and remove it.
            string path = args.DevicePath.ToUpper();
            if (devices.Any(d => d.Device.DevicePath.ToUpper().Equals(path)))
            {
                var device = devices.First(d => d.Device.DevicePath.ToUpper().Equals(path));
                device.Device.Dispose();
                devices.Remove(device);
                if (KeyboardDisconnected != null)
                    KeyboardDisconnected(null, new LogitechKeyboardEventArgs(device));
            }
        }

        public static List<LogitechKeyboard> GetKeyboards(LogitechKeyboardTypes keyboardType)
        {
            AssertInitialized();

            return devices.Where(
                d => d.KeyboardType.ProductID == keyboardType.ProductID &&
                d.KeyboardType.ReportID == keyboardType.ReportID
           ).ToList();
        }

        private static void AssertInitialized()
        {
            if (deviceEventNotifier == null)
            {
                throw new ApplicationException(
                        "You must first call Initialize to use the BackLight features.");
            }
        }
    }

    public class LogitechKeyboardEventArgs : EventArgs
    {
        public LogitechKeyboard Keyboard { get; private set; }

        public LogitechKeyboardEventArgs(LogitechKeyboard keyboard)
        {
            Keyboard = keyboard;
        }
    }
}
