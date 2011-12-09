using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace LgLcdNET
{
	// Represents a connection to all LCD devices
	// of a specific type attached to the system.
	public class Device
	{
		// The device handle
		public int Handle { get; private set; }

		// The type of the device
		public DeviceType Type { get; private set; }

		// The bitmap format the device supports
		public int BitmapWidth
		{
			get { return Type == DeviceType.Qvga ? (int)QvgaBmp.Width : (int)BwBmp.Width; }
		}
		public int BitmapHeight
		{
			get { return Type == DeviceType.Qvga ? (int)QvgaBmp.Height : (int)BwBmp.Height; }
		}
		public int BitmapBpp
		{
			get { return Type == DeviceType.Qvga ? (int)QvgaBmp.Bpp : (int)BwBmp.Bpp; }
		}

		public event EventHandler Left;
		public event EventHandler Right;
		public event EventHandler Ok;
		public event EventHandler Cancel;
		public event EventHandler Up;
		public event EventHandler Down;
		public event EventHandler Menu;

		public Device(DeviceType type)
		{
			if (Type != DeviceType.Monochrome && Type != DeviceType.Qvga)
			{
				throw new InvalidEnumArgumentException();
			}
			Type = type;
		}

		public void Open(Applet applet)
		{
			OpenByTypeContext ctx = new OpenByTypeContext()
			{
				Connection = applet.Handle,
				DeviceType = Type,
				OnSoftbuttonsChanged = new SoftbuttonsChangedContext()
				{
					Context = IntPtr.Zero,
					OnSoftbuttonsChanged = new SoftButtonsDelegate(OnSoftButtons),
				}
			};
			ReturnValue error = LgLcd.OpenByType(ref ctx);
			if (error != ReturnValue.ErrorSuccess)
			{
				if (error == ReturnValue.ErrorServiceNotActive)
				{
					throw new Exception("lgLcdInit() has not been called yet.");
				}
				else if (error == ReturnValue.ErrorInvalidParameter)
				{
					// Either ctx is NULL, or ctx->connection is not valid, or ctx->deviceType does not hold a valid device type.
					throw new Exception("The applet must be connected.");
				}
				else if (error == ReturnValue.ErrorAlreadyExists)
				{
					throw new Exception("The specified device has already been opened in the given applet.");
				}
				throw new Win32Exception((int)error);
			}
			Handle = ctx.Device;
		}

		public void UpdateBitmap(Bitmap bitmap, Priority priority, bool syncUpdate, bool syncCompleteWithinFrame)
		{
			if (bitmap.Width != BitmapWidth || bitmap.Height != BitmapHeight)
			{
				throw new ArgumentException("The bitmaps dimensions do not conform.");
			}
			LgBitmap lgBitmap = new LgBitmap()
			{
				Format = Type == DeviceType.Qvga ? BitmapFormat.QVGAx32 : BitmapFormat.Monochrome,
				Pixels = new byte[BitmapWidth * BitmapHeight * BitmapBpp],
			};
			var bitmapData = bitmap.LockBits(
				new Rectangle(0, 0, BitmapWidth, BitmapHeight),
				ImageLockMode.ReadOnly,
				Type == DeviceType.Qvga ? PixelFormat.Format32bppArgb : PixelFormat.Format8bppIndexed);
			Marshal.Copy(bitmapData.Scan0, lgBitmap.Pixels, 0, lgBitmap.Pixels.Length);
			bitmap.UnlockBits(bitmapData);
			ReturnValue error = LgLcd.UpdateBitmap(
				Handle,
				lgBitmap,
				(uint)priority
				| (syncUpdate ? 0x80000000 : 0)
				| (syncCompleteWithinFrame ? 0xC0000000 : 0));
			if (error != ReturnValue.ErrorSuccess)
			{
				if (error == ReturnValue.ErrorServiceNotActive)
				{
					throw new Exception("lgLcdInit() has not been called yet.");
				}
				else if (error == ReturnValue.ErrorInvalidParameter)
				{
					// The specified device handle, the bitmap header pointer or the type of bitmap is invalid.
					throw new Exception();
				}
				else if (error == ReturnValue.ErrorDeviceNotConnected)
				{
					throw new Exception("The specified device has been disconnected.");
				}
				else if (error == ReturnValue.ErrorAccessDenied)
				{
					throw new Exception("Synchronous operation was not displayed on the LCD within the frame interval (30 ms). This error code is only returned when the priority field of the lgLCDUpdateBitmap uses the macro LGLCD_SYNC_COMPLETE_WITHIN_FRAME().");
				}
				throw new Win32Exception((int)error);
			}
		}

		public SoftButtonFlags ReadSoftButtons()
		{
			SoftButtonFlags buttonFlags;
			ReturnValue error = LgLcd.ReadSoftButtons(Handle, out buttonFlags);
			if (error != ReturnValue.ErrorSuccess)
			{
				if (error == ReturnValue.ErrorServiceNotActive)
				{
					throw new Exception("lgLcdInit() has not been called yet.");
				}
				else if (error == ReturnValue.ErrorInvalidParameter)
				{
					// The specified device handle or the result pointer is invalid.
					throw new Exception();
				}
				else if (error == ReturnValue.ErrorDeviceNotConnected)
				{
					throw new Exception("The specified device has been disconnected.");
				}
				throw new Win32Exception((int)error);
			}			
			return buttonFlags;
		}

		public void SetAsLCDForegroundApp(bool yesNo)
		{
			ReturnValue error = LgLcd.SetAsLCDForegroundApp(Handle, yesNo);
			if (error != ReturnValue.ErrorSuccess)
			{
				if (error == ReturnValue.ErrorLockFailed)
				{
					throw new Exception("The operation could not be completed.");
				}
				throw new Win32Exception((int)error);
			}
		}

		public void Close()
		{
			ReturnValue error = LgLcd.Close(Handle);
			if (error != ReturnValue.ErrorSuccess)
			{
				if (error == ReturnValue.ErrorServiceNotActive)
				{
					throw new Exception("lgLcdInit() has not been called yet.");
				}
				else if (error == ReturnValue.ErrorInvalidParameter)
				{
					// The specified device handle is invalid.
					throw new Exception();
				}
				throw new Win32Exception((int)error);
			}
		}

		private int OnSoftButtons(int device, SoftButtonFlags buttons, IntPtr context)
		{
			switch (buttons)
			{
				case SoftButtonFlags.Left:
					if (Left != null)
						Left(this, null);
					break;
				case SoftButtonFlags.Right:
					if (Right != null)
						Right(this, null);
					break;
				case SoftButtonFlags.Ok:
					if (Right != null)
						Ok(this, null);
					break;
				case SoftButtonFlags.Cancel:
					if (Cancel != null)
						Cancel(this, null);
					break;
				case SoftButtonFlags.Up:
					if (Up != null)
						Up(this, null);
					break;
				case SoftButtonFlags.Down:
					if (Down != null)
						Down(this, null);
					break;
				case SoftButtonFlags.Menu:
					if (Menu != null)
						Menu(this, null);
					break;
			}
			return 0;
		}
	}
}
