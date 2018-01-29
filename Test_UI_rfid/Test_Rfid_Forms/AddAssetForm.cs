using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlServerCe;
using System.Configuration;
namespace Test_Rfid_Forms
{
    public partial class AddAssetForm : Form
    {
        SqlCeConnection conn = null;
        SqlCeCommand cmd = null;

        SqlCeDataReader reader = null;
        SqlCeResultSet result_set = null;
        string[] save = new string[11];
        string cancel = "";
        string select = "";
        SqlCeDataAdapter da;
        DataTable dt;
        DataSet ds;


        public AddAssetForm()
        {
            InitializeComponent();
            try
            {
                conn = new SqlCeConnection("Data Source=C:\\Users\\rahman\\Documents\\Visual Studio 2008\\Projects\\Test_UI_rfid\\Test_Rfid_Forms\\ReaderDB.sdf; Persist Security Info=False");

                conn.Open();
                MessageBox.Show("Connected to MSSQL CE Server");

            }

            catch (Exception ex)
            {
                MessageBox.Show("Connection to MSSQL CE Server Failed");
                MessageBox.Show(ex.ToString());
            }
        }
        private void onLoad()
        {
            listView1.Scrollable = true;
            listView1.View = View.Details;
            listView1.GridLines = true;
            listView1.Columns.Add("ID Asset", 70, HorizontalAlignment.Center);
            listView1.Columns.Add("Name", 70, HorizontalAlignment.Center);
            listView1.Columns.Add("Code", 70, HorizontalAlignment.Center);
            listView1.Columns.Add("Reference", 70, HorizontalAlignment.Center);
            listView1.Columns.Add("Lat Event", 70, HorizontalAlignment.Center);
            listView1.Columns.Add("description", 70, HorizontalAlignment.Center);
            listView1.Columns.Add("Tag ID", 70, HorizontalAlignment.Center);
            listView1.Columns.Add("Shelf ID", 70, HorizontalAlignment.Center);
            listView1.Columns.Add("Category", 70, HorizontalAlignment.Center);
            listView1.Columns.Add("Status", 70, HorizontalAlignment.Center);
            listView1.Columns.Add("Warehouse", 70, HorizontalAlignment.Center);


            string cm = "SELECT * from assets";
            cmd = new SqlCeCommand(cm, conn);
            dt = new DataTable();
            da = new SqlCeDataAdapter(cmd);
            ds = new DataSet();
            da.Fill(ds, "asset");
            dt = ds.Tables["asset"];



            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {

                listView1.Items.Add(dt.Rows[i].ItemArray[0].ToString());

                listView1.Items[i].SubItems.Add(dt.Rows[i].ItemArray[1].ToString());
                listView1.Items[i].SubItems.Add(dt.Rows[i].ItemArray[2].ToString());
                listView1.Items[i].SubItems.Add(dt.Rows[i].ItemArray[3].ToString());
                listView1.Items[i].SubItems.Add(dt.Rows[i].ItemArray[4].ToString());
                listView1.Items[i].SubItems.Add(dt.Rows[i].ItemArray[5].ToString());
                listView1.Items[i].SubItems.Add(dt.Rows[i].ItemArray[6].ToString());
                listView1.Items[i].SubItems.Add(dt.Rows[i].ItemArray[7].ToString());
                listView1.Items[i].SubItems.Add(dt.Rows[i].ItemArray[8].ToString());
                listView1.Items[i].SubItems.Add(dt.Rows[i].ItemArray[9].ToString());
                listView1.Items[i].SubItems.Add(dt.Rows[i].ItemArray[10].ToString());

            }



        }
        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {


            //save to DB
            save[0] = textBox1.Text;//Tag ID
            save[1] = textBox2.Text;//asset name
            save[2] = textBox3.Text;//reference
            save[3] = textBox4.Text;//description
            save[4] = textBox5.Text;//asset code
            save[5] = textBox6.Text;//last event
            save[6] = textBox7.Text;//status
            save[7] = textBox8.Text;//warehouse
            save[8] = textBox9.Text;//Asset ID
            save[9] = textBox10.Text;//Shelf id
            save[10] = textBox11.Text;//Category ID


            string cm = "INSERT INTO assets(id_asset, asset_name, asset_code, no_of_assets, lastevent, description, tag_id_FK, shelf_id_FK, asset_category_id_FK, status_id_FK, warehouse_id_FK) VALUES ('" + save[8] + "','" + save[2] + "','" + save[4] + "','" + save[2] + "','" + save[5] + "','" + save[3] + "','" + "-" + "','" + "-" + "','" + "-" + "','" + "-" + "','" + "-" + "')";

            cmd = new SqlCeCommand(cm, conn);
            cmd.Parameters.AddWithValue("@save[0]", textBox1.Text);
            cmd.Parameters.AddWithValue("@save[1]", textBox2.Text);
            cmd.Parameters.AddWithValue("@save[2]", textBox3.Text);
            cmd.Parameters.AddWithValue("@save[3]", textBox4.Text);
            cmd.Parameters.AddWithValue("@save[4]", textBox5.Text);
            cmd.Parameters.AddWithValue("@save[5]", textBox6.Text);
            cmd.Parameters.AddWithValue("@save[6]", textBox7.Text);
            cmd.Parameters.AddWithValue("@save[7]", textBox8.Text);
            cmd.Parameters.AddWithValue("@save[8]", textBox9.Text);
            cmd.Parameters.AddWithValue("@save[9]", textBox10.Text);
            cmd.Parameters.AddWithValue("@save[10]", textBox11.Text);

            try
            {

                cmd.ExecuteNonQuery();
                MessageBox.Show("Added");

                listView1.Clear();
                onLoad();


            }

            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }


        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Quiting");
            conn.Close();
            this.Close();


        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }


        private void AddAssetForm_Load(object sender, EventArgs e)
        {


            onLoad();



        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string delete = listView1.SelectedItems[0].Text;

                if (MessageBox.Show("Delete " + delete + "?") == DialogResult.OK)
                {
                    cmd = new SqlCeCommand("DELETE FROM assets WHERE id_asset=('" + @delete + "')", conn);
                    cmd.Parameters.AddWithValue("@delete", listView1.SelectedItems[0].Text);
                    cmd.ExecuteNonQuery();

                }
                listView1.SelectedItems.Clear();
            }

            catch (Exception ex) { }
            listView1.Clear();
            onLoad();


        }



    }
}
