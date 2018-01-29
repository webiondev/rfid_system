using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TestApplication
{
    public partial class frmInventory : Form, AT_UHF_NET.IUHFHost
    {
        System.Collections.ArrayList Tags = new System.Collections.ArrayList();

        public frmInventory()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.KeyPreview = true;

            // Setting application Windows Form
            Main.rfid.ActivatedForm = this;
        }

        public void GetAccessEPC(string EPC)
        {
            if (!checkBox1.Checked && !checkBox2.Checked)
                button1_Click(null, null);

            string RSSI = String.Empty;
            if (checkBox3.Checked)
            {
                string[] temp = EPC.Split(",".ToCharArray());
                EPC = temp[0];
                RSSI = temp[1].Split("=".ToCharArray())[1];
            }

            if (Tags.Contains(EPC))
            {
                int idx = 0;
                foreach (ListViewItem item in listView1.Items)
                {
                    if (item.SubItems[0].Text == EPC)
                        break;
                    else
                        idx++;
                }

                listView1.Items[idx].SubItems[1].Text = Convert.ToString(Convert.ToInt32(listView1.Items[idx].SubItems[1].Text) + 1);

                if (checkBox3.Checked)
                    listView1.Items[idx].SubItems[2].Text = RSSI;
            }
            else
            {
                ListViewItem item = new ListViewItem(EPC);
                item.SubItems.Add("1");
                if (checkBox3.Checked)
                    item.SubItems.Add(RSSI);
                else
                    item.SubItems.Add("");

                listView1.Items.Add(item);
                
                Tags.Add(EPC);
                label1.Text = Convert.ToString(Convert.ToInt32(label1.Text) + 1);
            }
            AT_UHF_NET.CUHFHost.PlaySuccess();
        }

        public void GetMemoryData(string MemoryData)
        {
        }

        public void GetReply(string Reply)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("Inventory"))
            {
                button1.Text = "Stop";
                if (checkBox1.Checked)
                {
                    if (checkBox2.Checked)
                        Main.rfid.ReadEPC_Continuous();
                    else
                        Main.rfid.ReadUID(AT_UHF_NET.UIDREAD_CODE.EPC_GEN2_MULTI_TAG);
                }
                else
                {
                    if (checkBox2.Checked)
                    {
                        Main.rfid.SET_ContinueModeONOff(true);
                        Main.rfid.ReadUID(AT_UHF_NET.UIDREAD_CODE.EPC_GEN2_ONE_TAG);
                    }
                    else
                    {
                        Main.rfid.SET_ContinueModeONOff(false);
                        Main.rfid.ReadUID(AT_UHF_NET.UIDREAD_CODE.EPC_GEN2_ONE_TAG);
                    }
                }
            }
            else
            {
                button1.Text = "Inventory";
                Main.rfid.Stop();
            }
        }
        //
        protected override void  OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode) 
            {
                case Keys.F7:
                case Keys.F8:
                case Keys.F19:
                    button1_Click(null, new EventArgs());
                    break;
                default :
                    break;
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F7:
                case Keys.F8:
                case Keys.F19:
                    button1.Text = "Inventory";
                    Main.rfid.Stop();
                    break;
                default:
                    break;
            }
        }    
        //
        private void button2_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            Tags.Clear();
            label1.Text = "0";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count > 0)
            {
                string tag = listView1.Items[listView1.SelectedIndices[0]].Text;
                string epc = tag.Substring(4, tag.Length - 4);
                new frmAccessMessage(epc).ShowDialog();
                Main.rfid.ActivatedForm = this;
            }
            else
            {
                new frmAccessMessage("").ShowDialog();
                Main.rfid.ActivatedForm = this;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Change report mode
            Main.rfid.SET_ReportMode(AT_UHF_NET.ReportMode.WAIT_DONE, true);

            if (listView1.SelectedIndices.Count > 0)
            {
                string tag = listView1.Items[listView1.SelectedIndices[0]].Text;
                string epc = tag.Substring(4, tag.Length - 4);
                new frmAccessResult(epc).ShowDialog();
            }
            else
            {
                new frmAccessResult("").ShowDialog();
            }

            // Change report mode
            Main.rfid.SET_ReportMode(AT_UHF_NET.ReportMode.WAIT_DONE, false);
            Main.rfid.SET_TimeOut(0);
        }

        private void checkBox3_CheckStateChanged(object sender, EventArgs e)
        {
            // Change report mode (get rssi)            
            if (checkBox3.Checked)
                Main.rfid.SET_ReportMode(AT_UHF_NET.ReportMode.EXTENDED_INFORMATION, true);
            else
                Main.rfid.SET_ReportMode(AT_UHF_NET.ReportMode.EXTENDED_INFORMATION, false);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}