using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlServerCe;

namespace Test_Rfid_Forms
{
    public partial class Shelves : Form
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
        public Shelves()
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
            listView1.Columns.Add("Shelf ID", 70, HorizontalAlignment.Center);
            listView1.Columns.Add("Shelf No", 70, HorizontalAlignment.Center);
           


            string cm = "SELECT * from shelves";
            cmd = new SqlCeCommand(cm, conn);
            dt = new DataTable();
            da = new SqlCeDataAdapter(cmd);
            ds = new DataSet();
            da.Fill(ds, "shelves");
            dt = ds.Tables["shelves"];



            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {

                listView1.Items.Add(dt.Rows[i].ItemArray[0].ToString());

                listView1.Items[i].SubItems.Add(dt.Rows[i].ItemArray[1].ToString());
               

            }



        }

        private void button1_Click(object sender, EventArgs e)
        {
            //save to DB
            save[0] = textBox1.Text;//shelf ID
            save[1] = textBox2.Text;//shelf no
            



            string cm = "INSERT INTO shelves(shelf_id, shelf_no) VALUES ('" + save[0] + "','" + save[1] + "')";

            cmd = new SqlCeCommand(cm, conn);
            cmd.Parameters.AddWithValue("@save[0]", textBox1.Text);
            cmd.Parameters.AddWithValue("@save[1]", textBox2.Text);
           


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

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Quiting");
            conn.Close();
            this.Close();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string delete = listView1.SelectedItems[0].Text;

                if (MessageBox.Show("Delete " + delete + "?") == DialogResult.OK)
                {
                    cmd = new SqlCeCommand("DELETE FROM shelves WHERE shelf_id=('" + @delete + "')", conn);
                    cmd.Parameters.AddWithValue("@delete", listView1.SelectedItems[0].Text);
                    cmd.ExecuteNonQuery();

                }
                listView1.SelectedItems.Clear();
            }

            catch (Exception ex) { }
            listView1.Clear();
            onLoad();
        }

        private void Shelves_Load(object sender, EventArgs e)
        {
            onLoad();
        }
    }
}
