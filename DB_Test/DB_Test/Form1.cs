using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Threading;



namespace DB_Test
{
    public partial class Form1 : Form
    {

        string connetionString = null;
        SqlConnection game;
        SqlCommand command;
        SqlDataAdapter adapt;
        string sql = null;
        

        public Form1()
        {
            InitializeComponent();
        }

        private void gameBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            //this.gameBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.Database1DataSet1);

        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'Database1DataSet1.Game' table. You can move, or remove it, as needed.
            this.gameTableAdapter.Fill(this.Database1DataSet1.Game);
            // TODO: This line of code loads data into the 'newGamesDataSet.game' table. You can move, or remove it, as needed.
            this.gameTableAdapter.Fill(this.Database1DataSet1.Game);

            //Connect to Database



            connetionString = ConfigurationManager.ConnectionStrings["DB_Test.Properties.Settings.Database1ConnectionString"].ConnectionString;

            game = new SqlConnection(connetionString);
            try
            {
                game.Open();
                MessageBox.Show("Connection Open ! ");

            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not open connection ! ");
            }



        }



       private void removeButton_Click(object sender, EventArgs e)
        {
            string id = idTextBox.Text;
            sql = ("delete Game where ID=@id");
            command = new SqlCommand(sql, game);
            command.Parameters.AddWithValue("@id", idTextBox.Text);

            try
            {

                command.ExecuteNonQuery();
                MessageBox.Show("Record Deleted! ");


            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }





        private void addButton_Click(object sender, EventArgs e)
        {
            DateTime dt = Convert.ToDateTime("2017-09-10");
            string id = idTextBox.Text;
            sql = "INSERT INTO game (ID) VALUES (@id)";
            command = new SqlCommand(sql, game);
            command.Parameters.AddWithValue("@id", idTextBox.Text);
            try
            {

                command.ExecuteNonQuery();
                MessageBox.Show("New game Added! ");


            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

       

        private void gameBindingNavigatorSaveItem_Click_1(object sender, EventArgs e)
        {
            this.Validate();
            this.gameBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.Database1DataSet1);

        }




        private void gameBindingNavigatorSaveItem_Click_2(object sender, EventArgs e)
        {
            this.Validate();
            this.gameBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.Database1DataSet1);

        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            sql = "SELECt * FROM Game";
            DataTable dt = new DataTable();
            adapt = new SqlDataAdapter(sql, game);
            adapt.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int ID = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
            idTextBox.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            string id = idTextBox.Text;
            sql = ("update Game set ID=@id");
            command = new SqlCommand(sql, game);
            command.Parameters.AddWithValue("@id", idTextBox.Text);

            try
            {

                command.ExecuteNonQuery();
                MessageBox.Show("Record Updated! ");


            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void logoutButton_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("Logging Out...");
           
            game.Close();
            this.Close();
            MessageBox.Show("Closed");

        }

     

       

       

       

        

        




    }
}
