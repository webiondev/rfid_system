using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using ClsRampdb;
using ClsLibBKLogs;

namespace TestApplication
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        DataSet dsLocalDB = new DataSet();
        int nextSet = 25000;
         

        private void Form1_Load(object sender, EventArgs e)
        {
           
                string localDBFile = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
                CEConn.dbFilePath = localDBFile + "\\Data\\ReaderDB.sdf"; 
                //for (int i = 1; i <= 2500; i++)
                //{
                //    string tagID = "000000000000000000B" + i.ToString();
                //    string LocationName = "Loc" + i.ToString();
                //    string LocationNo = "LocNo" + i.ToString();
                //    string strSql = " insert into Locations(TagID,Name,LocationNo,Date_Modified,ModifiedBy,RowStatus,serverKey)";
                //    strSql += " values('" + tagID + "','" + LocationName + "','" + LocationNo + "',getDate()," + 1 + "," + Convert.ToInt32(RowStatus.New) + "," + i + ")";
                //    localdb.runQuery(strSql);
                //} 
         

        }

        private void button1_Click(object sender, EventArgs e)
        {
            String[] tblArr = { "reasons", "employees", "locations", "fs_templates", "fieldservice", "tasks",  "menu_options", "asset_groups", "master_securitygroups", "authority", "asset_status", "asset_types" }; //, "asset_status", "asset_types"
            DataTable dt;

            dsLocalDB.Tables.Clear();
            using (CEConn localce = new CEConn())
            {
                foreach (string tableName in tblArr)
                {
                    dt = new DataTable(tableName);
                    string sqlText = "Select * From " + tableName;
                    localce.FillDataTable(ref dt, sqlText);
                    dsLocalDB.Tables.Add(dt);
                    dsLocalDB.AcceptChanges();
                }
            }

            MessageBox.Show("Locations Count " + dsLocalDB.Tables["locations"].Rows.Count); 

            using (CEConn localce = new CEConn())
            {
                string str = "assets";
                dt = new DataTable(str);
                string sqlText = "Select * From " + str;
                    localce.FillDataTable(ref dt, sqlText);
                    dsLocalDB.Tables.Add(dt);
                    dsLocalDB.AcceptChanges();
                
            } 

            MessageBox.Show("Assets Count " + dsLocalDB.Tables["assets"].Rows.Count);

         


           // dataGrid1.DataSource = dsLocalDB.Tables["locations"];          

            
        }
        int locid = 0;

        private void button2_Click(object sender, EventArgs e)
        {

            //using (CEConn localce = new CEConn())
            //{
            //    for (int i = 6001; i <= 25000; i++)
            //    {
            //        string tagID = "000000000000000000B" + i.ToString();
            //        string LocationName = "Loc" + i.ToString();
            //        string LocationNo = "LocNo" + i.ToString();
            //        string strSql = " insert into Locations(TagID,Name,LocationNo,Date_Modified,ModifiedBy,RowStatus,serverKey)";
            //        strSql += " values('" + tagID + "','" + LocationName + "','" + LocationNo + "',getDate()," + 1 + "," + Convert.ToInt32(RowStatus.New) + "," + i + ")";
            //        localce.runQuery(strSql);
            //    }
            //}

            int i = 0;

            using (CEConn localce = new CEConn())
            {
                for (i = nextSet; i <= (nextSet + 5000); i++)
                {
                    if (locid < 5000)
                    {
                        locid++;
                    }
                    else
                    {
                        locid = 1;
                    }
                    string tagID = "000000000000000000A" + i.ToString();
                    string AssetName = "Asset" + i.ToString();
                    string AssetNo = "AssetNo" + i.ToString();

                    string strSql = " insert into Assets(TagID,Name,AssetNo,ID_Employee,Date_Modified,ID_Location,ModifiedBy,RowStatus,ServerKey,Description,ID_AssetGroup)";
                    strSql += " values('" + tagID + "','" + AssetName + "','" + AssetNo + "'," + 1 + ",getDate()," + locid + "," + 1 + "," + Convert.ToInt32(RowStatus.New) + "," + i + ",'',1)";
                    localce.runQuery(strSql);
                }
            }

            nextSet = i;

            if (nextSet >= 50000)
            {
                button2.Enabled = false;
            }

            MessageBox.Show("Assets Loaded");
        }
    }
}