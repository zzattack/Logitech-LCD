using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LgLcd;

namespace LgLcdTest
{
	public partial class LgLcdTestForm : Form
	{
		public LgLcdTestForm()
		{
			InitializeComponent();
			this.HandleCreated += new EventHandler(LgLcdTestForm_HandleCreated);			
		}

		void LgLcdTestForm_HandleCreated(object sender, EventArgs e)
		{
			BackLight.Initialize(base.Handle);
		}
	}
}
