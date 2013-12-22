using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LgBackLight;

namespace BackLightTest
{
    public partial class KeyboardListItem : UserControl
    {
        private LogitechKeyboard keyboard;

        public KeyboardListItem()
        {
            InitializeComponent();
        }

        public KeyboardListItem(LogitechKeyboard keyboard)
        {
            InitializeComponent();

            lblKeyboardName.Text = keyboard.Device.DevicePath;
            pictureBox1.BackColor = keyboard.BackLightColor;
            btnChangeColor.Text = HexConverter(keyboard.BackLightColor);

            this.keyboard = keyboard;
            
        }

        private void btnChangeColor_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ColorDialog d = new ColorDialog();
            if (d.ShowDialog() == DialogResult.OK)
            {
                keyboard.BackLightColor = d.Color;
                lblKeyboardName.Text = keyboard.Device.DevicePath;
                pictureBox1.BackColor = keyboard.BackLightColor;
                btnChangeColor.Text = HexConverter(keyboard.BackLightColor);
            }
        }

        private static string HexConverter(Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

    }
}
