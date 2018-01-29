using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TestApplication
{
    public partial class frmAccessMessage : Form, AT_UHF_NET.IUHFHost
    {
        public frmAccessMessage(string epc)
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;

            Main.rfid.ActivatedForm = this;

            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;

            textBox5.Text = epc;
            if(epc.Length > 0)
                checkBox6.Checked = true;
        }

        private void ResultClear()
        {
            textBox6.Text = String.Empty;
            textBox7.Text = String.Empty;
            textBox8.Text = String.Empty;
        }

        public void GetAccessEPC(string EPC)
        {
            textBox7.Text = EPC;
            AT_UHF_NET.CUHFHost.PlaySuccess();
        }
        public void GetMemoryData(string MemoryData)
        {
            button1.Text = "Access";

            textBox6.Text = MemoryData;
            AT_UHF_NET.CUHFHost.PlaySuccess();
        }
        public void GetReply(string Reply)
        {
            button1.Text = "Access";

            if (!Reply.Equals("OK"))
            {
                textBox8.Text = Reply;
                AT_UHF_NET.CUHFHost.PlaySuccess();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                case 1:
                    label2.Text = "Access PWD";
                    panel1.Location = new Point(5, 111);
                    panel2.Location = new Point(253, 3);
                    panel3.Location = new Point(253, 3);
                    break;
                case 2:
                case 3:
                    label2.Text = "Access PWD";
                    panel1.Location = new Point(253, 3);
                    panel2.Location = new Point(5, 111);
                    panel3.Location = new Point(253, 3);
                    break;
                case 4:
                case 5:
                    label2.Text = "Access PWD";
                    panel1.Location = new Point(253, 3);
                    panel2.Location = new Point(253, 3);
                    panel3.Location = new Point(5, 111);
                    break;
                case 6:
                    label2.Text = "Kill PWD";
                    panel1.Location = new Point(253, 3);
                    panel2.Location = new Point(253, 3);
                    panel3.Location = new Point(253, 3);
                    break;
                default:
                    break;
            }

            textBox1.Focus();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("Access"))
            {
                button1.Text = "Stop";
                ResultClear();

                switch (comboBox1.SelectedIndex)
                {
                    case 0:
                        READ();
                        break;
                    case 1:
                        WRITE();
                        break;
                    case 2:
                        LOCK();
                        break;
                    case 3:
                        UNLOCK();
                        break;
                    case 4:
                        PLOCK();
                        break;
                    case 5:
                        TAGKILL();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                button1.Text = "Access";
                Main.rfid.Stop();
            }
        }

        private void READ()
        {
            if (checkBox6.Checked && textBox5.Text.Length < 2)
                return;

            AT_UHF_NET.MEMBANK_CODE MBank = AT_UHF_NET.MEMBANK_CODE.BANK_EPC;
            uint start = Convert.ToUInt32(textBox2.Text);
            uint length = Convert.ToUInt32(textBox3.Text);
            string EPC = textBox5.Text.Trim();
            string AccessPWD = textBox1.Text;
            
            switch (comboBox2.SelectedIndex)
            {
                case 0:
                    MBank = AT_UHF_NET.MEMBANK_CODE.BANK_EPC;
                    break;
                case 1:
                    MBank = AT_UHF_NET.MEMBANK_CODE.BANK_TID;
                    break;
                case 2:
                    MBank = AT_UHF_NET.MEMBANK_CODE.BANK_USER;
                    break;
                case 3:
                    MBank = AT_UHF_NET.MEMBANK_CODE.BANK_RESERVED;
                    break;
                default:
                    break;
            }

            if (checkBox6.Checked) //Tag Select
            {
                if (AccessPWD.Length > 0)
                    Main.rfid.BankSelectRead_TagSelect(MBank, start, length, EPC, AccessPWD);
                else
                    Main.rfid.BankSelectRead_TagSelect(MBank, start, length, EPC);
            }
            else
            {
                if (AccessPWD.Length > 0)
                    Main.rfid.BankSelectRead(MBank, start, length, AccessPWD);
                else
                    Main.rfid.BankSelectRead(MBank, start, length);
            }
        }

        private void WRITE()
        {
            if (checkBox6.Checked && textBox5.Text.Length < 2)
                return;

            AT_UHF_NET.MEMBANK_CODE MBank = AT_UHF_NET.MEMBANK_CODE.BANK_EPC;
            uint start = Convert.ToUInt32(textBox2.Text);
            string EPC = textBox5.Text.Trim();
            string AccessPWD = textBox1.Text;
            string WriteData = textBox4.Text;

            switch (comboBox2.SelectedIndex)
            {
                case 0:
                    MBank = AT_UHF_NET.MEMBANK_CODE.BANK_EPC;
                    break;
                case 1:
                    MBank = AT_UHF_NET.MEMBANK_CODE.BANK_TID;
                    break;
                case 2:
                    MBank = AT_UHF_NET.MEMBANK_CODE.BANK_USER;
                    break;
                case 3:
                    MBank = AT_UHF_NET.MEMBANK_CODE.BANK_RESERVED;
                    break;
                default:
                    break;
            }

            if (checkBox6.Checked) //Tag Select
            {
                if (AccessPWD.Length > 0)
                    Main.rfid.BankSelectWrite_TagSelect(MBank, start, WriteData, EPC, AccessPWD);
                else
                    Main.rfid.BankSelectWrite_TagSelect(MBank, start, WriteData, EPC);
            }
            else
            {
                if (AccessPWD.Length > 0)
                    Main.rfid.BankSelectWrite(MBank, start, WriteData, AccessPWD);
                else
                    Main.rfid.BankSelectWrite(MBank, start, WriteData);
            }
        }
        private void LOCK()
        {
            if (checkBox6.Checked && textBox5.Text.Length < 2)
                return;

            if (textBox1.Text.Length != 8)
            {
                MessageBox.Show("Input AccessPassword");
                textBox1.Focus();
                return;
            }

            string EPC = textBox5.Text.Trim();

            bool bKill_PWD = checkBox1.Checked;
            bool bAccess_PWD = checkBox2.Checked;
            bool bEPC = checkBox4.Checked;
            bool bTID = checkBox3.Checked;
            bool bUSER = checkBox5.Checked;

            string AccessPWD = textBox1.Text;

            if (checkBox6.Checked) //Tag Select
            {
                Main.rfid.Lock_TagSelect(bKill_PWD, bAccess_PWD, bEPC, bTID, bUSER, AccessPWD, EPC);
            }
            else
            {
                Main.rfid.Lock(bKill_PWD, bAccess_PWD, bEPC, bTID, bUSER, AccessPWD);
            }
        }
        private void UNLOCK()
        {
            if (checkBox6.Checked && textBox5.Text.Length < 2)
                return;

            if (textBox1.Text.Length != 8)
            {
                MessageBox.Show("Input AccessPassword");
                textBox1.Focus();
                return;
            }

            string EPC = textBox5.Text.Trim();

            bool bKill_PWD = checkBox1.Checked;
            bool bAccess_PWD = checkBox2.Checked;
            bool bEPC = checkBox4.Checked;
            bool bTID = checkBox3.Checked;
            bool bUSER = checkBox5.Checked;

            string AccessPWD = textBox1.Text;

            if (checkBox6.Checked) //Tag Select
            {
                Main.rfid.UnLock_TagSelect(bKill_PWD, bAccess_PWD, bEPC, bTID, bUSER, AccessPWD, EPC);
            }
            else
            {
                Main.rfid.UnLock(bKill_PWD, bAccess_PWD, bEPC, bTID, bUSER, AccessPWD);
            }
        }
        private void PLOCK()
        {
            if (checkBox6.Checked && textBox5.Text.Length < 2)
                return;

            if (textBox1.Text.Length != 8)
            {
                MessageBox.Show("Input AccessPassword");
                textBox1.Focus();
                return;
            }

            string EPC = textBox5.Text.Trim();
            AT_UHF_NET.ActField ActionField = AT_UHF_NET.ActField.EPC;
            string AccessPWD = textBox1.Text;
            bool bSecured = radioButton1.Checked;

            if (checkBox6.Checked) //Tag Select
            {
                Main.rfid.Permalock_TagSelect(ActionField, bSecured, AccessPWD, EPC);
            }
            else
            {
                Main.rfid.Permalock(ActionField, bSecured, AccessPWD);
            }
        }
        
        private void TAGKILL()
        {
            if (checkBox6.Checked && textBox5.Text.Length < 2)
                return;

            if (textBox1.Text.Length != 8)
            {
                MessageBox.Show("Input AccessPassword");
                textBox1.Focus();
                return;
            }

            string EPC = textBox5.Text.Trim();
            string Kill_PWD = textBox1.Text;

            if (checkBox6.Checked)
            {
                Main.rfid.TagKill_TagSelect(Kill_PWD, EPC);
            }
            else
            {
                Main.rfid.TagKill(Kill_PWD);
            }
        }

        private void checkBox7_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBox7.Checked)
                Main.rfid.SET_ReportMode(AT_UHF_NET.ReportMode.ACCESS_EPC, true);
            else
                Main.rfid.SET_ReportMode(AT_UHF_NET.ReportMode.ACCESS_EPC, false);
        }
    }
}