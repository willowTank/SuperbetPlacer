using FirefoxBet365Placer.Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;

namespace FirefoxBet365Placer
{
    public partial class frmClickDlg : Form
    {
        public frmClickDlg()
        {
            InitializeComponent();
        }

        //Click
        private void button1_Click(object sender, EventArgs e)
        {
            FirefoxBet365Placer.Json.Point pos = new FirefoxBet365Placer.Json.Point(numPosX.Value, numPosY.Value);
            FakeUserAction.Intance.SimMouseMoveTo(pos);
            Thread.Sleep(2000);
            FakeUserAction.Intance.SimClick();
            // Send Stake
            int stake = 245;
            InputSimulator inputSim = new InputSimulator();
            Thread.Sleep(2000);
            foreach (char digit in stake.ToString().ToCharArray())
            {
                inputSim.Keyboard.KeyPress((VirtualKeyCode)digit);
            }
        }

        //Move
        private void button2_Click(object sender, EventArgs e)
        {
            FirefoxBet365Placer.Json.Point pos = new FirefoxBet365Placer.Json.Point(numPosX.Value, numPosY.Value);
            FakeUserAction.Intance.SimMouseMoveTo(pos);

        }
    }
}
