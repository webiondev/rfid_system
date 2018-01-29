using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TestApplication
{
    public partial class frmControl : Form, AT_UHF_NET.IUHFHost
    {
        public frmControl()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;

            Main.rfid.ActivatedForm = this;
            ReadControlValue();
        }

        private void ReadControlValue()
        {
            AT_UHF_NET.CUHFHost.CONTROL_VALUE Data = Main.rfid.GET_ControlValue();

            textBox6.Text = Data.Version;
            textBox7.Text = Data.Hopping.ToString();

            textBox1.Text = Data.Qvalue.ToString();
            textBox2.Text = Data.Timeout.ToString();
            textBox3.Text = Data.Power.ToString();
            if (Data.ContinueMode)
                comboBox1.SelectedIndex = 0;
            else
                comboBox1.SelectedIndex = 1;

            textBox4.Text = Data.SessionValue.ToString();

            if (Data.Hopping == AT_UHF_NET.HOPPING_CODE.EURO_LBT || Data.Hopping == AT_UHF_NET.HOPPING_CODE.JAPAN_LBT)
            {
                button7.Enabled = true;
                checkBox1.Checked = Data.ChState.CH1;
                checkBox2.Checked = Data.ChState.CH2;
                checkBox3.Checked = Data.ChState.CH3;
                checkBox4.Checked = Data.ChState.CH4;
                checkBox5.Checked = Data.ChState.CH5;
                checkBox6.Checked = Data.ChState.CH6;
                checkBox7.Checked = Data.ChState.CH7;
                checkBox8.Checked = Data.ChState.CH8;
                checkBox9.Checked = Data.ChState.CH9;
                checkBox10.Checked = Data.ChState.CH10;
            }
            else
                button7.Enabled = false;

            textBox5.Text = Data.LBT_Time.ToString();
        }

        public void GetAccessEPC(string EPC)
        {
        }

        public void GetMemoryData(string MemoryData)
        {
        }

        public void GetReply(string Reply)
        {
            MessageBox.Show(Reply);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Main.rfid.SET_QValue(Convert.ToUInt16(textBox1.Text));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Main.rfid.SET_TimeOut(Convert.ToUInt16(textBox2.Text));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Main.rfid.SET_PowerControl(Convert.ToUInt16(textBox3.Text));
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
                Main.rfid.SET_ContinueModeONOff(true);
            else
                Main.rfid.SET_ContinueModeONOff(false);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Main.rfid.SET_Session(Convert.ToUInt16(textBox4.Text));
        }

        private void button7_Click(object sender, EventArgs e)
        {
            AT_UHF_NET.CUHFHost.CH_STATE CHSTATE = new AT_UHF_NET.CUHFHost.CH_STATE();
            CHSTATE.CH1 = checkBox1.Checked;
            CHSTATE.CH2 = checkBox2.Checked;
            CHSTATE.CH3 = checkBox3.Checked;
            CHSTATE.CH4 = checkBox4.Checked;
            CHSTATE.CH5 = checkBox5.Checked;
            CHSTATE.CH6 = checkBox6.Checked;
            CHSTATE.CH7 = checkBox7.Checked;
            CHSTATE.CH8 = checkBox8.Checked;
            CHSTATE.CH9 = checkBox9.Checked;
            CHSTATE.CH10 = checkBox10.Checked;

            Main.rfid.SET_LBT_CHState(CHSTATE);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Main.rfid.SET_LBT_Time(Convert.ToUInt16(textBox5.Text));
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Main.rfid.SET_Default();

            ReadControlValue();
        }
    }
}