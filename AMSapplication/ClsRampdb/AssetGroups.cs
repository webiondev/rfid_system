/**************************************************************************************
 * Author : Deepanshu Jouhari
 * Modified Date : 15-Jun-2009
 * Last Modified by : 
 * Last Modified : 
 * Module Name : ClsRampDb
 * Decription : AssetGroups class to interface SQL CE AssetGroups Table
 **************************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Windows.Forms;

namespace ClsRampdb
{
    public class AssetGroups
    {
        public static DataTable getGroups(Int32 ParentID)
        {
            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = " select ServerKey as ID_AssetGroup,Name from Asset_Groups where ParentID =" + ParentID +"";
                    DataTable dtList = new DataTable();
                    localDB.FillDataTable(ref dtList, strSql);
                    return dtList;
                }
            }
            else
            {
                RConnection.RConnection OnConn = new RConnection.RConnection();
                OnConn.Url = Login.webConURL;
                string strSql;
                strSql = " select ID_AssetGroup,Name from Asset_Groups where ParentID =" + ParentID + " and Is_Active=1 and Is_Deleted=0";
                
                DataTable dt;
                dt = (DataTable)OnConn.getDataTable(strSql);
                return dt;

                //throw new ApplicationException("Online functionality not implemented yet.");
            }
        }

        public static DataTable getGroups(Int32 ParentID,string searchText)
        {
            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = " select ServerKey as ID_AssetGroup,Name from Asset_Groups where ParentID =" + ParentID + " AND [Name] LIKE '" + searchText + "%'";
                    DataTable dtList = new DataTable();
                    localDB.FillDataTable(ref dtList, strSql);
                    return dtList;
                }
            }
            else
            {
                RConnection.RConnection OnConn = new RConnection.RConnection();
                OnConn.Url = Login.webConURL;
                string strSql;
                strSql = " select ID_AssetGroup,Name from Asset_Groups where ParentID =" + ParentID + " AND [Name] LIKE '" + searchText + "%'" + " and Is_Active=1 and Is_Deleted=0";

                DataTable dt;
                dt = (DataTable)OnConn.getDataTable(strSql);
                return dt;

                //throw new ApplicationException("Online functionality not implemented yet.");
            }
        }

        public static DataTable getGroups(Int32 ParentID, ListView lv)
        {
            if (!Login.OnLineMode)
            {
                DataTable dtList = null;
                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = " select ServerKey as ID_AssetGroup,Name from Asset_Groups where ParentID =" + ParentID + "";
                    
                    localDB.FillDataTable(ref dtList, strSql); 

                    //return dtList;
                }

                lv.Items.Clear();

                if (dtList != null)
                {
                    string[] subitems = new string[3];

                    foreach (DataRow dr in dtList.Rows)
                    {
                        subitems[0] = dr["ID_AssetGroup"].ToString();
                        subitems[1] = dr["Name"].ToString();
                        subitems[2] = "";

                        lv.Items.Add(new ListViewItem(subitems));

                    }
                }  

                //dtt.Columns["ServerKey"].ColumnName = "ID_Location";

                return null;


            }
            else
            {
                RConnection.RConnection OnConn = new RConnection.RConnection();
                OnConn.Url = Login.webConURL;
                string strSql;
                strSql = " select ID_AssetGroup,Name from Asset_Groups where ParentID =" + ParentID + " and Is_Active=1 and Is_Deleted=0";

                DataTable dt;
                dt = (DataTable)OnConn.getDataTable(strSql);

                lv.Items.Clear();

                string[] subitems = new string[3];

                foreach (DataRow dr in dt.Rows)
                {
                    subitems[0] = dr["ID_AssetGroup"].ToString();
                    subitems[1] = dr["Name"].ToString();
                    subitems[2] = "";

                    lv.Items.Add(new ListViewItem(subitems));

                }

                //return dt;

                return null;

                //throw new ApplicationException("Online functionality not implemented yet.");
            }
        }

        public static DataTable getGroups(Int32 ParentID, string searchText, ListView lv)
        {
            if (!Login.OnLineMode)
            {
                DataTable dtList = null;
                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = " select ServerKey as ID_AssetGroup,Name from Asset_Groups where ParentID =" + ParentID + " AND [Name] LIKE '" + searchText + "%'";
                    
                    localDB.FillDataTable(ref dtList, strSql);
                   // return dtList;
                }


                lv.Items.Clear();

                if (dtList != null)
                {
                    string[] subitems = new string[3];

                    foreach (DataRow dr in dtList.Rows)
                    {
                        subitems[0] = dr["ID_AssetGroup"].ToString();
                        subitems[1] = dr["Name"].ToString();
                        subitems[2] = "";

                        lv.Items.Add(new ListViewItem(subitems));

                    }
                }

                //dtt.Columns["ServerKey"].ColumnName = "ID_Location";

                return null;


            }
            else
            {
                RConnection.RConnection OnConn = new RConnection.RConnection();
                OnConn.Url = Login.webConURL;
                string strSql;
                strSql = " select ID_AssetGroup,Name from Asset_Groups where ParentID =" + ParentID + " AND [Name] LIKE '" + searchText + "%'" + " and Is_Active=1 and Is_Deleted=0";

                DataTable dt;
                dt = (DataTable)OnConn.getDataTable(strSql);

                lv.Items.Clear();

                string[] subitems = new string[3];

                foreach (DataRow dr in dt.Rows)
                {
                    subitems[0] = dr["ID_AssetGroup"].ToString();
                    subitems[1] = dr["Name"].ToString();
                    subitems[2] = "";

                    lv.Items.Add(new ListViewItem(subitems));

                }

                //return dt;

                return null;

                //throw new ApplicationException("Online functionality not implemented yet.");
            }
        }
    }

    public class AssetStatus
    {
        public static DataTable GetAssetStatus()
        {
            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = " select ServerKey as ID_AssetStatus,Status from Asset_Status";
                    DataTable dtList = new DataTable();
                    localDB.FillDataTable(ref dtList, strSql);
                    return dtList;
                }
            }
            else
            {
                RConnection.RConnection OnConn = new RConnection.RConnection();
                OnConn.Url = Login.webConURL;
                string strSql;
                strSql = " select ID_AssetStatus,Status from Asset_Status where Is_Active=1 and Is_Deleted=0";

                DataTable dt;
                dt = (DataTable)OnConn.getDataTable(strSql);
                return dt;

                //throw new ApplicationException("Online functionality not implemented yet.");
            }
        }
    }

}
