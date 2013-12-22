using LgBackLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BackLightTest
{
    public partial class BackLightColorForm : Form
    {
        private List<LogitechKeyboard> keyboards;

        public BackLightColorForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LogitechKeyboardManager.Init(Handle, LogitechKeyboardTypes.G510, LogitechKeyboardTypes.G19);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (keyboards != null)
            {
                lstKeyboards.Controls.Clear();
                foreach (var keyboard in keyboards)
                {
                    lstKeyboards.Controls.Add(new KeyboardListItem(keyboard));
                }
            }
                
        }

        private void cmbKeyboards_SelectedIndexChanged(object sender, EventArgs e)
        {
            LogitechKeyboardTypes selectedKeyboard;
            switch (cmbKeyboards.SelectedIndex)
            {
                case 0: // G19 //
                    selectedKeyboard = LogitechKeyboardTypes.G19;
                    break;
                case 1: // G510 //
                default:
                    selectedKeyboard = LogitechKeyboardTypes.G510;
                    break;

            }

            keyboards = LogitechKeyboardManager.GetKeyboards(selectedKeyboard);

            lblKeyboardCount.Text = keyboards.Count.ToString();
            btnSearch.Enabled = keyboards.Count > 0;
        }
    }
}
