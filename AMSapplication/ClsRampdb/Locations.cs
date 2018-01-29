/**************************************************************************************
 * Author : Deepanshu Jouhari
 * Modified Date : 15-Jul-2009
 * Last Modified by : 
 * Last Modified : 
 * Module Name : ClsRampDb
 * Decription : Locations class to intrafacing the table Locations
 **************************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Windows.Forms;
using ClsLibBKLogs;

namespace ClsRampdb
{
    public class Locations
    {
        #region "variables & Procedures."
        String _TagNo;
        String _Name;
        Int32 _RowStatus;
        Int32 _ServerKey;
        string _LocNo;

        public Int32 ServerKey
        {
            get
            {
                return _ServerKey;
            }
        }

        public String TagNo
        {
            get
            {
                return _TagNo;
            }
        }

        public String Name
        {
            get
            {
                return _Name;
            }
        }

        public string LocNo
        {
            get
            {
                return _LocNo;
            }
        }

        public RowStatus Status
        {
            get
            {
                return (RowStatus)_RowStatus;
            }
        }

        #endregion

        public Locations(Int32 LocID)
        {

            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = " select * from Locations where ServerKey=" + LocID;
                    SqlCeDataReader dr;
                    dr = localDB.getReader(strSql);
                    while (dr.Read())
                    {
                        _ServerKey = LocID;
                        _TagNo = (String)dr["TagID"];
                        _Name = (String)dr["Name"];                       
                        _RowStatus = Convert.ToInt32(dr["RowStatus"]);
                        if (dr["LocationNo"] != DBNull.Value)
                            _LocNo = (String)dr["LocationNo"];
                        else
                            _LocNo = "";
                    }
                    dr.Close();
                }
            }
            else
            {
                RConnection.RConnection OnConn = new RConnection.RConnection();
                OnConn.Url = Login.webConURL;
                string strSql;
                strSql = " select TagID,Name,LocationNo from Locations where ID_Location=" + LocID;
                DataTable dt;
                dt = OnConn.getDataTable(strSql);
                if (dt.Rows.Count != 0)
                {
                    _ServerKey = LocID;
                    _TagNo = (String)dt.Rows[0]["TagID"];
                    _Name = (String)dt.Rows[0]["Name"];                   
                    _RowStatus = Convert.ToInt32(RowStatus.Synchronized);

                    if (dt.Rows[0]["LocationNo"] != DBNull.Value)
                         _LocNo = (String)dt.Rows[0]["LocationNo"];
                    else
                         _LocNo = "";
                }
                //throw new ApplicationException("Online functionality not implemented yet.");
            }
        }

        public Locations(String TagID)
        {

            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = " select * from Locations where TagID='" + TagID + "'";
                    SqlCeDataReader dr;
                    dr = localDB.getReader(strSql);
                    while (dr.Read())
                    {
                        _ServerKey = Convert.ToInt32(dr["ServerKey"].ToString().Trim());
                        _TagNo = (String)dr["TagID"];
                        _Name = (String)dr["Name"];                      
                        _RowStatus = Convert.ToInt32(dr["RowStatus"]);

                        if (dr["LocationNo"] != DBNull.Value)
                              _LocNo =(String)dr["LocationNo"];
                        else
                              _LocNo ="";

                    }
                    dr.Close();
                }
            }
            else
            {
                RConnection.RConnection OnConn = new RConnection.RConnection();
                OnConn.Url = Login.webConURL;
                string strSql;
                strSql = " select TagID,Name,ID_Location,LocationNo from Locations where TagID='" + TagID +"'";
                DataTable dt;
                dt = OnConn.getDataTable(strSql);
                if (dt.Rows.Count != 0)
                {
                    _ServerKey = (int)dt.Rows[0]["ID_Location"];
                    _TagNo = (String)dt.Rows[0]["TagID"];
                    _Name = (String)dt.Rows[0]["Name"];                  
                    _RowStatus = Convert.ToInt32(RowStatus.Synchronized);

                    if (dt.Rows[0]["LocationNo"] != DBNull.Value)
                        _LocNo = (String)dt.Rows[0]["LocationNo"];
                    else
                        _LocNo = "";
                }
                //throw new ApplicationException("Online functionality not implemented yet.");
            }
        }

        public static DataTable getLocationList()
        {
            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = " select serverKey as ID_Location,Name from Locations where ServerKey<>0 and tagid<>'0000000000000000000000D' order by Name";
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
                strSql = " select ID_Location,Name from Locations where Is_Active=1 and Is_Deleted=0 and tagid<>'0000000000000000000000D' order by Name";
                return (DataTable)OnConn.getDataTable(strSql);
                //throw new ApplicationException("Online functionality not implemented yet.");
            }
        }

        public static DataTable getLocationList(string searchString)
        {
            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = " select serverKey as ID_Location,Name from Locations where ServerKey<>0 and tagid<>'0000000000000000000000D' AND [Name] Like '" + searchString + "%' order by Name";
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
                strSql = " select ID_Location,Name from Locations where Is_Active=1 and Is_Deleted=0 and tagid<>'0000000000000000000000D' AND [Name] Like '" + searchString + "%' order by Name";
                return (DataTable)OnConn.getDataTable(strSql);
                //throw new ApplicationException("Online functionality not implemented yet.");
            }
        }

        public static DataTable getLocationList(ComboBox cb)
        {
            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = " select serverKey as ID_Location,Name from Locations where ServerKey<>0 and tagid<>'0000000000000000000000D' order by Name";
                    DataTable dtList = new DataTable();
                    localDB.FillDataTable(ref dtList, strSql);

                    cb.Items.Clear();

                    LocationItem item;

                    if (dtList != null && dtList.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtList.Rows)
                        {
                            //dtt.ImportRow(dr); 
                            item = new LocationItem(dr["Name"].ToString(), dr["ID_Location"].ToString());
                            cb.Items.Add(item);
                        }

                    }
                    //dtt.Columns["ServerKey"].ColumnName = "ID_Location";

                    return null;

                    //return dtList;
                }
            }
            else
            {
                RConnection.RConnection OnConn = new RConnection.RConnection();
                OnConn.Url = Login.webConURL;
                string strSql;
                strSql = " select ID_Location,Name from Locations where Is_Active=1 and Is_Deleted=0 and tagid<>'0000000000000000000000D' order by Name";

                DataTable dtList = (DataTable)OnConn.getDataTable(strSql);

                cb.Items.Clear();

                LocationItem item;

                if (dtList != null && dtList.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtList.Rows)
                    {
                        //dtt.ImportRow(dr); 
                        item = new LocationItem(dr["Name"].ToString(), dr["ID_Location"].ToString());
                        cb.Items.Add(item);
                    }

                }

                //return (DataTable)OnConn.getDataTable(strSql);
                return null;
                //throw new ApplicationException("Online functionality not implemented yet.");
            }
        }

        public static DataTable getLocationList(string searchString, ComboBox cb)
        {
            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = " select serverKey as ID_Location,Name from Locations where ServerKey<>0 and tagid<>'0000000000000000000000D' AND [Name] Like '" + searchString + "%' order by Name";
                    DataTable dtList = new DataTable();
                    localDB.FillDataTable(ref dtList, strSql);
                    //return dtList;

                    cb.Items.Clear();

                    LocationItem item;

                    if (dtList != null && dtList.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtList.Rows)
                        {
                            //dtt.ImportRow(dr); 
                            item = new LocationItem(dr["Name"].ToString(), dr["ID_Location"].ToString());
                            cb.Items.Add(item);
                        }

                    }
                    //dtt.Columns["ServerKey"].ColumnName = "ID_Location";

                    return null;
                }
            }
            else
            {
                RConnection.RConnection OnConn = new RConnection.RConnection();
                OnConn.Url = Login.webConURL;
                string strSql;
                strSql = " select ID_Location,Name from Locations where Is_Active=1 and Is_Deleted=0 and tagid<>'0000000000000000000000D' AND [Name] Like '" + searchString + "%' order by Name";

                DataTable dtList = (DataTable)OnConn.getDataTable(strSql);

                cb.Items.Clear();

                LocationItem item;

                if (dtList != null && dtList.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtList.Rows)
                    {
                        //dtt.ImportRow(dr); 
                        item = new LocationItem(dr["Name"].ToString(), dr["ID_Location"].ToString());
                        cb.Items.Add(item);
                    }

                }

                //return (DataTable)OnConn.getDataTable(strSql);
                return null;

                //return (DataTable)OnConn.getDataTable(strSql);
                //throw new ApplicationException("Online functionality not implemented yet.");
            }
        }

        public static object getLocationList(ListView lv)
        {
            if (!Login.OnLineMode)
            {
                DataTable dtList = new DataTable();

                using (CEConn localDB = new CEConn())
                {
                    string strSql = " select serverKey as ID_Location,Name,TagID from Locations where ServerKey<>0 and tagid<>'0000000000000000000000D' order by UsageCount Desc,Name";

                    
                    localDB.FillDataTable(ref dtList, strSql);

                    //lv.Items.Clear();

                    //string[] subitems = new string[3];
                    //if (dtList != null && dtList.Rows.Count > 0)
                    //{
                    //    foreach (DataRow dr in dtList.Rows)
                    //    {
                    //        subitems[0] = dr["ID_Location"].ToString();
                    //        subitems[1] = dr["Name"].ToString();
                    //        subitems[2] = dr["TagID"].ToString();

                    //        lv.Items.Add(new ListViewItem(subitems));

                    //    }
                    //}
                }

                //dtt.Columns["ServerKey"].ColumnName = "ID_Location";

                return dtList;
            }
            else
            {
                RConnection.RConnection OnConn = new RConnection.RConnection();
                OnConn.Url = Login.webConURL;
                string strSql;
                strSql = " select ID_Location,Name,TagID from Locations where Is_Active=1 and Is_Deleted=0 and tagid<>'0000000000000000000000D' order by UsageCount Desc,Name";
                //return (DataTable)OnConn.getDataTable(strSql);
                //throw new ApplicationException("Online functionality not implemented yet.");

                // ----------------------------------------------
                DataTable dtLocs = (DataTable)OnConn.getDataTable(strSql);


                //lv.Items.Clear();

                //string[] subitems = new string[3];

                //foreach (DataRow dr in dtLocs.Rows)
                //{
                //    subitems[0] = dr["ID_Location"].ToString();
                //    subitems[1] = dr["Name"].ToString();
                //    subitems[2] = dr["TagID"].ToString();

                //    lv.Items.Add(new ListViewItem(subitems));

                //}

                return dtLocs;
                // -------------------------------------------

            }
        }

        public static object getLocationList(string searchString, bool serachForTagID, ListView lv)
        {
            if (!Login.OnLineMode)
            {

                string strSql;                

                if (serachForTagID)
                {
                    strSql = " select serverKey as ID_Location,Name,TagID from Locations where ServerKey<>0 and tagid<>'0000000000000000000000D' AND [TagID] Like '%" + searchString + "%' order by UsageCount Desc,Name";
                }
                else
                {
                    strSql = " select serverKey as ID_Location,Name,TagID from Locations where ServerKey<>0 and tagid<>'0000000000000000000000D' AND [Name] Like '" + searchString + "%' order by UsageCount Desc,Name";
                }
                DataTable dtList = new DataTable();

               
                    using (CEConn localDB = new CEConn())
                    {

                        localDB.FillDataTable(ref dtList, strSql);

                        //lv.Items.Clear();

                        //string[] subitems = new string[3];
                        //if (dtList != null && dtList.Rows.Count > 0)
                        //{
                        //    foreach (DataRow dr in dtList.Rows)
                        //    {
                        //        subitems[0] = dr["ID_Location"].ToString();
                        //        subitems[1] = dr["Name"].ToString();
                        //        subitems[2] = dr["TagID"].ToString();

                        //        lv.Items.Add(new ListViewItem(subitems));

                        //    }
                        //}
                    }

                    //dtt.Columns["ServerKey"].ColumnName = "ID_Location";

                    return dtList;
              

            }
            else
            {
                RConnection.RConnection OnConn = new RConnection.RConnection();
                OnConn.Url = Login.webConURL;
                string strSql;

                if (serachForTagID)
                {
                    strSql = " select ID_Location,Name,TagID from Locations where Is_Active=1 and Is_Deleted=0 and tagid<>'0000000000000000000000D' AND [TagID] Like '%" + searchString + "%' order by UsageCount Desc,Name";
                }
                else
                {

                    strSql = " select ID_Location,Name,TagID from Locations where Is_Active=1 and Is_Deleted=0 and tagid<>'0000000000000000000000D' AND [Name] Like '" + searchString + "%' order by UsageCount Desc,Name";
                }
                //return (DataTable)OnConn.getDataTable(strSql);
                //throw new ApplicationException("Online functionality not implemented yet.");

                // ----------------------------------------------
                DataTable dtLocs = (DataTable)OnConn.getDataTable(strSql);

                //lv.Items.Clear();

                //string[] subitems = new string[3];

                //foreach (DataRow dr in dtLocs.Rows)
                //{
                //    subitems[0] = dr["ID_Location"].ToString();
                //    subitems[1] = dr["Name"].ToString();
                //    subitems[2] = dr["TagID"].ToString();

                //    lv.Items.Add(new ListViewItem(subitems));

                //}

                return dtLocs;
                // -------------------------------------------

            }
        }

        public void AssignNewTag(String TagNo)
        {
            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    if (_RowStatus != Convert.ToInt32(RowStatus.New))
                    {
                        _RowStatus = Convert.ToInt32(RowStatus.TagWrite);
                    }
                    strSql = " update Locations set TagID='" + TagNo + "', Date_MOdified=getDate(), ModifiedBy=" + Login.ID + ", RowStatus=" + _RowStatus + " where ServerKey=" + _ServerKey;
                    localDB.runQuery(strSql);
                    _TagNo = TagNo;
                }
            }
            else
            {
                throw new ApplicationException("Online Location functionality not implemented yet.");
            }
        }

        public static void AddLocation(String TagID, String LocationNo, String LocationName)
        {
            if (!Login.OnLineMode)
            {
                Int32 Skey;
                Skey = minServerKey();

                // added by vijay 08 Dec 2009
                if (Skey == -2)
                    Skey = Skey - 1;

                bool tagAssigned = false;

                using (CEConn localDB = new CEConn())
                {
                    string strSql;

                    if (TagID.Length > 0) // to allow blank tagid for locations 05-11-2011
                    {
                        TagInfo Ti = new TagInfo(TagID);
                        if (Ti.isAssetTag() || Ti.isLocationTag() || Ti.isEmployeeTag())
                            tagAssigned = true;
                        else
                            tagAssigned = false;

                        //strSql = " select count(*) from Assets A,Employees E,Locations L where A.TagID='" + TagID + "' or E.TagID='" + TagID + "' or L.TagID='" + TagID + "'";
                        ////select count(*) from Locations where TagID='" + TagID + "'";
                        if (tagAssigned == true)
                            throw new ApplicationException("Duplicate Tag ID.");
                    }


                    //strSql = " select count(*) from Assets A,Employees E,Locations L where A.TagID='" + TagID + "' or E.TagID='" + TagID + "' or L.TagID='" + TagID + "'";
                    ////select count(*) from Locations where TagID='" + TagID + "'";
                    //if (Convert.ToInt32(localDB.getScalerValue(strSql)) > 0)
                    //    throw new ApplicationException("Duplicate Tag ID.");

                    strSql = " insert into Locations(TagID,Name,LocationNo,Date_Modified,ModifiedBy,RowStatus,serverKey)";
                    strSql += " values('" + TagID + "','" + LocationName.Replace("'", "''") + "','" + LocationNo.Replace("'", "''") + "',getDate()," + Login.ID + "," + Convert.ToInt32(RowStatus.New) + "," + Skey + ")";
                    localDB.runQuery(strSql);
                }
            }
            else
            {
                DataTable dtLocation = new DataTable("dtLocation");

                dtLocation.Columns.Add("TagID", typeof(String));
                dtLocation.Columns.Add("Name", typeof(String));
                dtLocation.Columns.Add("LocationNo", typeof(String));
                dtLocation.Columns.Add("Date_Modified", typeof(DateTime));
                dtLocation.Columns.Add("ModifiedBy", typeof(Int32));
                dtLocation.AcceptChanges();

                DataRow dr;
                dr = dtLocation.NewRow();

                dr["TagID"] = TagID;
                dr["Name"] = LocationName;
                dr["LocationNo"] = LocationNo;
                dr["Date_Modified"] = DateTime.Now;
                dr["ModifiedBy"] = Login.ID;

                dtLocation.Rows.Add(dr);
                dtLocation.AcceptChanges();

                Synchronize.Synchronise OnConn = new Synchronize.Synchronise();
                OnConn.Url = Login.webURL;
                DataTable dtResult;
                dtResult = OnConn.AddLocation(dtLocation);

                if (dtResult.Rows.Count != 0)
                {
                    if (Convert.ToInt32(dtResult.Rows[0]["RowStatus"]) == Convert.ToInt32(RowStatus.Error))
                    {
                        throw new ApplicationException("Insert Failed.");
                    }
                    else
                    {
                        //Inserted Sucessfully
                    }
                }
                //throw new ApplicationException("Online functionality of Location not implemented yet.");
            }
        }

        public static void EditLocation(String TagID, String LocationNo, String LocationName,Int32 ServerKey)
        {
            if (!Login.OnLineMode)
            { 

                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = "Update Locations Set Name = '" + LocationName.Replace("'", "''") + "'" + ",LocationNo = '" + LocationNo.Replace("'", "''") + "',Date_Modified = getDate(),";
                    strSql += "ModifiedBy = " + Login.ID + ",RowStatus = " + Convert.ToInt32(RowStatus.Modify);
                    strSql += " Where ServerKey = " + ServerKey;

                    //strSql = " insert into Locations(TagID,Name,LocationNo,Date_Modified,ModifiedBy,RowStatus,serverKey)";
                    //strSql += " values('" + TagID + "','" + LocationName.Replace("'", "''") + "','" + LocationNo.Replace("'", "''") + "',getDate()," + Login.ID + "," + Convert.ToInt32(RowStatus.New) + "," + Skey + ")";
                    localDB.runQuery(strSql);
                }
            }
            else
            {
                string strSql;              

                strSql = "Update Locations Set Name = '" + LocationName.Replace("'", "''") + "'" + ",LocationNo = '" + LocationNo.Replace("'", "''") + "',Date_Modified = getDate(),";
                strSql += "Last_Modified_By = " + Login.ID;
                strSql += " Where ID_Location = " + ServerKey;

                RConnection.RConnection OnConn = new RConnection.RConnection();
                OnConn.Url = Login.webConURL;
                OnConn.runQuery(strSql);
            }
        }

       

        public static void EditLocation(String TagID, String LocationNo, String LocationName, Int32 ServerKey, int usageCount)
        {
            if (!Login.OnLineMode)
            {

                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = "Update Locations Set Name = '" + LocationName.Replace("'", "''") + "'" + ",LocationNo = '" + LocationNo.Replace("'", "''") + "',Date_Modified = getDate(),";
                    strSql += "ModifiedBy = " + Login.ID + ",RowStatus = " + Convert.ToInt32(RowStatus.Modify);
                    strSql += " Where ServerKey = " + ServerKey;

                    //strSql = " insert into Locations(TagID,Name,LocationNo,Date_Modified,ModifiedBy,RowStatus,serverKey)";
                    //strSql += " values('" + TagID + "','" + LocationName.Replace("'", "''") + "','" + LocationNo.Replace("'", "''") + "',getDate()," + Login.ID + "," + Convert.ToInt32(RowStatus.New) + "," + Skey + ")";
                    localDB.runQuery(strSql);
                }
            }
            else
            {
                string strSql;

                strSql = "Update Locations Set Name = '" + LocationName.Replace("'", "''") + "'" + ",LocationNo = '" + LocationNo.Replace("'", "''") + "',Date_Modified = getDate(),";
                strSql += "Last_Modified_By = " + Login.ID + ",UsageCount = UsageCount + " + usageCount;
                strSql += " Where ID_Location = " + ServerKey;

                RConnection.RConnection OnConn = new RConnection.RConnection();
                OnConn.Url = Login.webConURL;
                OnConn.runQuery(strSql);
            }
        } 
        
        public static void DeleteLocation(Int32 serverKey)
        {
            if (!Login.OnLineMode)
            {

                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = "Delete from Locations ";
                    strSql += " Where ServerKey = " + serverKey;

                    localDB.runQuery(strSql);
                }
            }
            else
            {
                string strSql;

                strSql = "Delete from Locations "; 
                strSql += " Where ID_Location = " + serverKey;

                RConnection.RConnection OnConn = new RConnection.RConnection();
                OnConn.Url = Login.webConURL;
                OnConn.runQuery(strSql);
            }
        }

        private static Int32 minServerKey()
        {
            using (CEConn localDB = new CEConn())
            {
                string strSql;
                strSql = "select min(serverKey) from Locations";
                object o;
                o = localDB.getScalerValue(strSql);
                if (DBNull.Value == o)
                    return -1;
                else if (Convert.ToInt32(o) < 0)
                    return (Convert.ToInt32(o) - 1);
                else
                    return -1;
            }
        }

        public static DataTable getNewRows()
        {
            using (CEConn localDB = new CEConn())
            {
                DataTable dtLocation = new DataTable("dtLocation");
                string strSql;


                strSql = "select * from Locations where (RowStatus=" + Convert.ToInt32(RowStatus.New) + " OR ServerKey < 0)";
                localDB.FillDataTable(ref dtLocation, strSql);

                //strSql = "update Locations set RowStatus=" + Convert.ToInt32(RowStatus.InProcess) + " where RowStatus=" + Convert.ToInt32(RowStatus.New);
                //localDB.runQuery(strSql);

                return dtLocation;
            }

        }

        public static DataTable getModifiedRows()
        {
            using (CEConn localDB = new CEConn())
            {
                DataTable dtLocation = new DataTable("dtLocation");
                string strSql;


                strSql = "select * from Locations where RowStatus=" + Convert.ToInt32(RowStatus.Modify);
                localDB.FillDataTable(ref dtLocation, strSql);

                //strSql = "update Locations set RowStatus=" + Convert.ToInt32(RowStatus.InProcess) + " where RowStatus=" + Convert.ToInt32(RowStatus.New);
                //localDB.runQuery(strSql);

                return dtLocation;
            }

        }

        public static string getDispatchLocation()
        {
            string strSql;
            if (Login.OnLineMode)
            {
                RConnection.RConnection OnConn = new RConnection.RConnection();
                OnConn.Url = Login.webConURL;
                strSql = " Update Locations Set is_deleted=0, is_active=1 Where tagid='0000000000000000000000D' ;";
                strSql = strSql + " select Top 1 isNULL(ID_Location,'') from Locations where tagid='0000000000000000000000D' and is_deleted=0 and is_active=1";
                return OnConn.getScalerValue(strSql);
            }
            else
            {
                using (CEConn localDB = new CEConn())
                {
                    strSql = " select ServerKey as ID_Location from Locations where tagid='0000000000000000000000D' and ServerKey<>0 ";
                    return Convert.ToString(localDB.getScalerValue(strSql));
                }
            }
        }
        public static string getLocationByName(string locName)
        {
           string strSql;
            if (Login.OnLineMode)
            {
                RConnection.RConnection OnConn = new RConnection.RConnection();
                OnConn.Url = Login.webConURL;

                strSql = " select Top 1 isNULL(ID_Location,'') from Locations where   is_deleted=0 and Name = '" + locName + "'"; 
                return OnConn.getScalerValue(strSql);
            }
            else
            {

                using (CEConn localDB = new CEConn())
                {
                    strSql = " select ServerKey as ID_Location from Locations where   ServerKey<>0 and Name='"+locName+"'";
                    return Convert.ToString(localDB.getScalerValue(strSql));
                }

            }
        
        }
    }
}
