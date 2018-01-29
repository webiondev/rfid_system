using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TestApplication
{
    public partial class frmAccessResult : Form
    {
        public frmAccessResult(string epc)
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;

            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;

            textBox5.Text = epc;
            if (epc.Length > 0)
                checkBox6.Checked = true;
        }

        private void ResultClear()
        {
            textBox6.Text = String.Empty;
            textBox7.Text = String.Empty;
            textBox8.Text = String.Empty;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ResultClear();


            // Set TimeOut
            Main.rfid.SET_TimeOut(
                Convert.ToUInt16(textBox9.Text)
                );

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

        private void READ()
        {
            if (checkBox6.Checked && textBox5.Text.Length < 2)
                return;

            string Data = new string(new char[1024]);
            AT_UHF_NET.AccessResult Result = AT_UHF_NET.AccessResult.Unknown;
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
                    Result = Main.rfid.BankSelectRead_TagSelect_Done(Data, MBank, start, length, EPC, AccessPWD);
                else
                    Result = Main.rfid.BankSelectRead_TagSelect_Done(Data, MBank, start, length, EPC);
            }
            else
            {
                if (AccessPWD.Length > 0)
                    Result = Main.rfid.BankSelectRead_Done(Data, MBank, start, length, AccessPWD);
                else
                    Result = Main.rfid.BankSelectRead_Done(Data, MBank, start, length);
            }

            //if (Result == AT_UHF_NET.AccessResult.OK)
            //    textBox6.Text = Data;
            //else
            //    textBox8.Text = Result.ToString();

            string temp = Main.rfid.GET_LAST_ERROR();
            if (temp.Equals("OK"))
                textBox6.Text = Data;
            else
                textBox8.Text = temp;

            AT_UHF_NET.CUHFHost.PlaySuccess();
        }

        private void WRITE()
        {
            if (checkBox6.Checked && textBox5.Text.Length < 2)
                return;

            AT_UHF_NET.AccessResult Result = AT_UHF_NET.AccessResult.Unknown;
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
                    Result = Main.rfid.BankSelectWrite_TagSelect_Done(MBank, start, WriteData, EPC, AccessPWD);
                else
                    Result = Main.rfid.BankSelectWrite_TagSelect_Done(MBank, start, WriteData, EPC);
            }
            else
            {
                if (AccessPWD.Length > 0)
                    Result = Main.rfid.BankSelectWrite_Done(MBank, start, WriteData, AccessPWD);
                else
                    Result = Main.rfid.BankSelectWrite_Done(MBank, start, WriteData);
            }

            //textBox8.Text = Result.ToString();
            textBox8.Text = Main.rfid.GET_LAST_ERROR();
            AT_UHF_NET.CUHFHost.PlaySuccess();
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

            AT_UHF_NET.AccessResult Result = AT_UHF_NET.AccessResult.Unknown;
            string EPC = textBox5.Text.Trim();

            bool bKill_PWD = checkBox1.Checked;
            bool bAccess_PWD = checkBox2.Checked;
            bool bEPC = checkBox4.Checked;
            bool bTID = checkBox3.Checked;
            bool bUSER = checkBox5.Checked;

            string AccessPWD = textBox1.Text;

            if (checkBox6.Checked) //Tag Select
            {
                Result = Main.rfid.Lock_TagSelect_Done(bKill_PWD, bAccess_PWD, bEPC, bTID, bUSER, AccessPWD, EPC);
            }
            else
            {
                Result = Main.rfid.Lock_Done(bKill_PWD, bAccess_PWD, bEPC, bTID, bUSER, AccessPWD);
            }

            //textBox8.Text = Result.ToString();
            textBox8.Text = Main.rfid.GET_LAST_ERROR();
            AT_UHF_NET.CUHFHost.PlaySuccess();
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

            AT_UHF_NET.AccessResult Result = AT_UHF_NET.AccessResult.Unknown;
            string EPC = textBox5.Text.Trim();

            bool bKill_PWD = checkBox1.Checked;
            bool bAccess_PWD = checkBox2.Checked;
            bool bEPC = checkBox4.Checked;
            bool bTID = checkBox3.Checked;
            bool bUSER = checkBox5.Checked;

            string AccessPWD = textBox1.Text;

            if (checkBox6.Checked) //Tag Select
            {
                Result = Main.rfid.UnLock_TagSelect_Done(bKill_PWD, bAccess_PWD, bEPC, bTID, bUSER, AccessPWD, EPC);
            }
            else
            {
                Result = Main.rfid.UnLock_Done(bKill_PWD, bAccess_PWD, bEPC, bTID, bUSER, AccessPWD);
            }

            //textBox8.Text = Result.ToString();
            textBox8.Text = Main.rfid.GET_LAST_ERROR();
            AT_UHF_NET.CUHFHost.PlaySuccess();
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

            AT_UHF_NET.AccessResult Result = AT_UHF_NET.AccessResult.Unknown;
            string EPC = textBox5.Text.Trim();
            AT_UHF_NET.ActField ActionField = AT_UHF_NET.ActField.EPC;
            string AccessPWD = textBox1.Text;
            bool bSecured = radioButton1.Checked;

            if (checkBox6.Checked) //Tag Select
            {
                Result = Main.rfid.Permalock_TagSelect_Done(ActionField, bSecured, AccessPWD, EPC);
            }
            else
            {
                Result = Main.rfid.Permalock_Done(ActionField, bSecured, AccessPWD);
            }

            //textBox8.Text = Result.ToString();
            textBox8.Text = Main.rfid.GET_LAST_ERROR();
            AT_UHF_NET.CUHFHost.PlaySuccess();
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

            AT_UHF_NET.AccessResult Result = AT_UHF_NET.AccessResult.Unknown;
            string EPC = textBox5.Text.Trim();
            string Kill_PWD = textBox1.Text;

            if (checkBox6.Checked)
            {
                Result = Main.rfid.TagKill_TagSelect_Done(Kill_PWD, EPC);
            }
            else
            {
                Result = Main.rfid.TagKill_Done(Kill_PWD);
            }

            //textBox8.Text = Result.ToString();
            textBox8.Text = Main.rfid.GET_LAST_ERROR();
            AT_UHF_NET.CUHFHost.PlaySuccess();
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
        }

    }
}