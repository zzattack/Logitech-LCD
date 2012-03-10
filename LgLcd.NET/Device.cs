using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace LgLcd {

	public enum Priority {
		IdleNoShow = LgLcd.Priority.IdleNoShow,
		Background = LgLcd.Priority.Background,
		Normal = LgLcd.Priority.Normal,
		Alert = LgLcd.Priority.Alert,
	}

	[Flags]
	public enum SoftButtonFlags {
		Left = LgLcd.SoftButtonFlags.Left,
		Right = LgLcd.SoftButtonFlags.Right,
		Ok = LgLcd.SoftButtonFlags.Ok,
		Cancel = LgLcd.SoftButtonFlags.Cancel,
		Up = LgLcd.SoftButtonFlags.Up,
		Down = LgLcd.SoftButtonFlags.Down,
		Menu = LgLcd.SoftButtonFlags.Menu,
	}

	public enum DeviceType {
		Monochrome = LgLcd.DeviceType.Monochrome,
		Qvga = LgLcd.DeviceType.Qvga,
	}

	// Represents a connection to all LCD devices
	// of a specific type attached to the system.
	public class Device {
		
		// The device handle
		private int _handle = LgLcd.InvalidDevice;
		public int Handle { get { return _handle; } }

		// Gets whether the device is currently opened
		public bool Opened { get { return _handle != LgLcd.InvalidDevice; } }

		// The type of the device
		public DeviceType Type { get; private set; }
				
		// The bitmap format the device supports
		public int BitmapWidth { get; private set; }
		public int BitmapHeight { get; private set; }
		public int BitmapBpp { get; private set; }
		private LgLcd.BitmapFormat _bitmapFormat;
		private PixelFormat _pixelFormat;

		public event EventHandler Left;
		public event EventHandler Right;
		public event EventHandler Ok;
		public event EventHandler Cancel;
		public event EventHandler Up;
		public event EventHandler Down;
		public event EventHandler Menu;

		public Device() {
			SoftButtonsDelegate = OnSoftButtons;
		}

		public void Open(Applet applet, DeviceType type) {
			if (Opened) {
				throw new Exception("Already opened.");
			}
			if (!applet.Connected) {
				throw new Exception("Applet must be connected to LCDMon.");
			}
			if (type != DeviceType.Monochrome && type != DeviceType.Qvga) {
				throw new InvalidEnumArgumentException();
			}
			if ((type == DeviceType.Monochrome && applet.CapabilitiesSupported != AppletCapabilities.Monochrome)
				|| (type == DeviceType.Qvga && applet.CapabilitiesSupported != AppletCapabilities.Qvga)) {
				throw new Exception("The applet does not support the device type \"" + type.ToString() + "\".");
			}
			if (type != DeviceType.Monochrome && type != DeviceType.Qvga) {
				throw new InvalidEnumArgumentException();
			}
			Type = type;
			if (type == DeviceType.Monochrome) {
				_bitmapFormat = LgLcd.BitmapFormat.Monochrome;
				BitmapWidth = (int)LgLcd.BwBmp.Width;
				BitmapHeight = (int)LgLcd.BwBmp.Height;
				BitmapBpp = (int)LgLcd.BwBmp.Bpp;
			}
			else if (type == DeviceType.Qvga) {
				_bitmapFormat = LgLcd.BitmapFormat.QVGAx32;
				BitmapWidth = (int)LgLcd.QvgaBmp.Width;
				BitmapHeight = (int)LgLcd.QvgaBmp.Height;
				BitmapBpp = (int)LgLcd.QvgaBmp.Bpp;
				_pixelFormat = PixelFormat.Format32bppArgb;
			}
			var ctx = new LgLcd.OpenByTypeContext {
				Connection = applet.Handle,
				DeviceType = (LgLcd.DeviceType)Type,
				OnSoftbuttonsChanged = new LgLcd.SoftbuttonsChangedContext {
					Context = IntPtr.Zero,
					OnSoftbuttonsChanged = SoftButtonsDelegate,
				}
			};
			LgLcd.ReturnValue error = LgLcd.OpenByType(ref ctx);
			if (error != LgLcd.ReturnValue.ErrorSuccess) {
				if (error == LgLcd.ReturnValue.ErrorAlreadyExists) {
					throw new Exception("The specified device has already been opened in the given applet.");
				}
				throw new Win32Exception((int)error);
			}
			_handle = ctx.Device;
		}

		public void UpdateBitmap(
			Bitmap bitmap,
			Priority priority,
			bool syncUpdate = false,
			bool syncCompleteWithinFrame = false) {
			if (!Opened) {
				throw new Exception("Not opened.");
			}
			if (bitmap.Width != BitmapWidth || bitmap.Height != BitmapHeight) {
				throw new ArgumentException("The bitmaps dimensions do not conform.");
			}
			var lgBitmap = new LgLcd.Bitmap {
				Format = _bitmapFormat,
				Pixels = new byte[BitmapWidth * BitmapHeight * BitmapBpp],
			};
			var bitmapData = bitmap.LockBits(
				new Rectangle(0, 0, BitmapWidth, BitmapHeight),
				ImageLockMode.ReadOnly,
				_pixelFormat);
			Marshal.Copy(bitmapData.Scan0, lgBitmap.Pixels, 0, lgBitmap.Pixels.Length);
			bitmap.UnlockBits(bitmapData);
			var error = LgLcd.UpdateBitmap(
				Handle,
				lgBitmap,
				(uint)priority
				| (syncUpdate ? LgLcd.SyncUpdate : 0)
				| (syncCompleteWithinFrame ? LgLcd.SyncCompleteWithinFrame : 0));
			if (error != LgLcd.ReturnValue.ErrorSuccess) {
				if (error == LgLcd.ReturnValue.ErrorDeviceNotConnected) {
					throw new Exception("The specified device has been disconnected.");
				}
				if (error == LgLcd.ReturnValue.ErrorAccessDenied) {
					throw new Exception("Synchronous operation was not displayed on the LCD within the frame interval (30 ms).");
				}
				throw new Win32Exception((int)error);
			}
		}

		public SoftButtonFlags ReadSoftButtons() {
			if (!Opened) {
				throw new Exception("Not opened.");
			}
			LgLcd.SoftButtonFlags buttonFlags;
			var error = LgLcd.ReadSoftButtons(Handle, out buttonFlags);
			if (error != LgLcd.ReturnValue.ErrorSuccess) {
				if (error == LgLcd.ReturnValue.ErrorDeviceNotConnected) {
					throw new Exception("The specified device has been disconnected.");
				}
				throw new Win32Exception((int)error);
			}
			return (SoftButtonFlags)buttonFlags;
		}

		public void SetAsLCDForegroundApp(bool yesNo) {
			if (!Opened) {
				throw new Exception("Not opened.");
			}			
			var error = LgLcd.SetAsLCDForegroundApp(Handle, yesNo);
			if (error != LgLcd.ReturnValue.ErrorSuccess) {
				if (error == LgLcd.ReturnValue.ErrorLockFailed) {
					throw new Exception("The operation could not be completed.");
				}
				throw new Win32Exception((int)error);
			}
		}

		public void Close() {
			if (!Opened) {
				throw new Exception("Not opened.");
			}
			var error = LgLcd.Close(Handle);
			if (error != LgLcd.ReturnValue.ErrorSuccess) {
				throw new Win32Exception((int)error);
			}
			// Reset device handle
			_handle = LgLcd.InvalidDevice;
		}

		private int OnSoftButtons(int device, LgLcd.SoftButtonFlags buttons, IntPtr context) {
			EventHandler e = null;
			switch (buttons)
			{
				case LgLcd.SoftButtonFlags.Left:
					e = Left;
					break;
				case LgLcd.SoftButtonFlags.Right:
					e = Right;
					break;
				case LgLcd.SoftButtonFlags.Ok:
					e = Ok;
					break;
				case LgLcd.SoftButtonFlags.Cancel:
					e = Cancel;
					break;
				case LgLcd.SoftButtonFlags.Up:
					e = Up;
					break;
				case LgLcd.SoftButtonFlags.Down:
					e = Down;
					break;
				case LgLcd.SoftButtonFlags.Menu:
					e = Menu;
					break;
			}
			if (e != null)
			{
				e(this, EventArgs.Empty);
			}
			return 0;
		}

		private LgLcd.SoftButtonsDelegate SoftButtonsDelegate;
	}
}
