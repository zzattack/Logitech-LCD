using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

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
				// TODO: Handle possible errors
				// ServiceNotActive (Init not called)
				// InvalidParameter (Invalid/closed handle/connection/applet or enum value)
				// AlreadyExists (DeviceType already opened)
				// Xxx
				throw new Exception();
			}
			Handle = ctx.Device;
		}

		public void UpdateBitmap(Bitmap bitmap, Priority priority)
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
			LgLcd.UpdateBitmap(Handle, lgBitmap, priority);
		}

		public SoftButtonFlags ReadSoftButtons()
		{
			SoftButtonFlags buttonFlags;
			LgLcd.ReadSoftButtons(Handle, out buttonFlags);
			return buttonFlags;
		}

		public void SetAsLCDForegroundApp(bool yesNo)
		{
			LgLcd.SetAsLCDForegroundApp(Handle, yesNo);
		}

		public void Close()
		{
			LgLcd.Close(Handle);
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
