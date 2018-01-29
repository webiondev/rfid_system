using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TestApplication
{
    public partial class Main : Form, Util_PowerOn_NET.IUtil_PowerOn
    {
        public static AT_UHF_NET.CUHFHost rfid;
        Util_PowerOn_NET.CUtil_PowerOn PowerNotify;

        public Main()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;

            PowerNotify = new Util_PowerOn_NET.CUtil_PowerOn(this);
            PowerNotify.AT_PowerNotifyHWND();
            PowerNotify.AT_PowerNotifyEvent();


            rfid = new AT_UHF_NET.CUHFHost();

            // Turn on RFID Module
            rfid.PowerON();

            // Open Port
            if (rfid.Open())
            {
                button2.Enabled = true;
                button3.Enabled = true;
            }
            else
                MessageBox.Show("Open Fail");
        }

        // PowerOn Notify Callback
        public void On_status_power()
        {
            // *** Must call this fucntion. ***
            rfid.PowerOnInit();
        }

     

        private void Main_Closing(object sender, CancelEventArgs e)
        {
            PowerNotify.AT_PowerNotifyClose();

            rfid.Close();
            rfid.PowerOFF();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            new frmInventory().ShowDialog();
        }

    

    
      

     
    }
}