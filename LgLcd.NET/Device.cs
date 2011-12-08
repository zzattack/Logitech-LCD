using System;

namespace LgLcdNET
{
	class Device : IDisposable
	{
		DeviceType Type { get; private set; }

		public Device(int deviceHandle, DeviceType deviceType) {
			device = deviceHandle;
			Type = deviceType;
		}

		public SoftButtonFlags ReadSoftButtons() {
			SoftButtonFlags buttonFlags;
			LgLcd.ReadSoftButtons(device, out buttonFlags);
			return buttonFlags;
		}

		public void SetAsLCDForegroundApp(bool yesNoMaybe) {
			LgLcd.SetAsLCDForegroundApp(device, yesNoMaybe);
		}

		public void UpdateBitmap(Bitmap bitmap, Priority priority) {
			LgLcd.UpdateBitmap(device, bitmap, priority);
		}

		void Close() {
			LgLcd.Close(device);
		}

		private int device;
	}
}
