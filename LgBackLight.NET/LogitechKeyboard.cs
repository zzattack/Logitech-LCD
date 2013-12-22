using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LgBackLight
{
    public struct LogitechKeyboardTypes
    {
        public static readonly LogitechKeyboardTypes G19 = new LogitechKeyboardTypes(0xC229, 0x7);
        public static readonly LogitechKeyboardTypes G510 = new LogitechKeyboardTypes(0xC22D, 0x5);

        public byte ReportID;
        public int ProductID;

        public LogitechKeyboardTypes(int productId, byte reportId) 
        {
            ReportID = reportId;
            ProductID = productId;
        }
    }

    public class LogitechKeyboard
    {
        
        
        public HidDevice Device { get; private set; }

        public LogitechKeyboardTypes KeyboardType { get; private set; }

        public Color BackLightColor
        {
            get
            {
                return ReadBackLight(Device);
            }
            set
            {
                // Create the feature buffer.
                byte[] featureBuffer = new byte[] { KeyboardType.ReportID, value.R, value.G, value.B };
                Device.SetFeature(featureBuffer);
            }
        }


        private Color ReadBackLight(HidDevice device)
        {
            byte[] b = new byte[4];
            b[0] = KeyboardType.ReportID;
            device.GetFeature(b);
            return Color.FromArgb(b[1], b[2], b[3]);
        }

        internal LogitechKeyboard(HidDevice device, LogitechKeyboardTypes type)
        {
            Device = device;
            KeyboardType = type;
        }
    }
}
