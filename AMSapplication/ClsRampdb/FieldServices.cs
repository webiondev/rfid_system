/**************************************************************************************
 * Author : Deepanshu Jouhari
 * Modified Date : 15-Jul-2009
 * Last Modified by : 
 * Last Modified : 
 * Module Name : ClsRampDb
 * Decription : FieldServices class to implement Field Service functionality
 **************************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using ClsLibBKLogs;
namespace ClsRampdb
{
    public class FieldServices
    {
        #region "variables & Procedures."
        String _Title;
        Int32 _ID_Employee;
        Int32 _ID_Location;
        Int32 _ServerKey;
        Int32 _RowStatus;

        public Int32 ServerKey
        {
            get
            {
                return _ServerKey;
            }
        }

        public String Title
        {
            get
            {
                return _Title;
            }
        }

        public Int32 ID_Employee
        {
            get
            {
                return _ID_Employee;
            }
        }

        public Int32 ID_Location
        {
            get
            {
                return _ID_Location;
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

        public FieldServices(Int32 FSID)
        {

            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = " select * from FieldService where ServerKey=" + FSID;
                    SqlCeDataReader dr;
                    dr = localDB.getReader(strSql);
                    while (dr.Read())
                    {
                        _ServerKey = FSID;
                        _Title = (String)dr["Title"];
                        _ID_Location = (Int32)dr["ID_Location"];  
                        _ID_Employee = (Int32)dr["ID_Employee"];  
                        _RowStatus = Convert.ToInt32(dr["RowStatus"]);
                    }
                    dr.Close();
                }
            }
            else
            {
                //throw new ApplicationException("Online functionality not implemented yet.");

                RConnection.RConnection OnConn = new RConnection.RConnection();
                OnConn.Url = Login.webConURL;
                string strSql;
                strSql = " select Title,ID_Location,ID_Employee from FieldService where ID_FieldService=" + FSID;
                DataTable dt;
                dt = OnConn.getDataTable(strSql);
                if (dt.Rows.Count != 0)
                {
                    _ServerKey = FSID;
                    _Title = (String)dt.Rows[0]["Title"];
                    _ID_Location = (Int32)dt.Rows[0]["ID_Location"];
                    _ID_Employee = (Int32)dt.Rows[0]["ID_Employee"];
                    _RowStatus = Convert.ToInt32(RowStatus.Synchronized);
                }
                
            }
        }

        /* Commented Constructor will uncomment if required to get information by title.
        public Locations(String Title)
        {

            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = " select * from Locations where Title='" + Title + "'";
                    SqlCeDataReader dr;
                    dr = localDB.getReader(strSql);
                    while (dr.Read())
                    {
                        _ServerKey = Convert.ToInt32(dr["ServerKey"].ToString().Trim());
                        _Title = (String)dr["Title"];
                        _Name = (String)dr["Name"];
                        _RowStatus = Convert.ToInt32(dr["RowStatus"]);
                    }
                    dr.Close();
                }
            }
            else
            {
                RConnection.RConnection OnConn = new RConnection.RConnection();
                OnConn.Url = Login.webConURL;
                string strSql;
                strSql = " select Title,Name,ID_Location from Locations where Title='" + Title +"'";
                DataTable dt;
                dt = OnConn.getDataTable(strSql);
                if (dt.Rows.Count != 0)
                {
                    _ServerKey = (int)dt.Rows[0]["ID_Location"];
                    _Title = (String)dt.Rows[0]["Title"];
                    _Name = (String)dt.Rows[0]["Name"];
                    _RowStatus = Convert.ToInt32(RowStatus.Synchronized);
                }
                //throw new ApplicationException("Online functionality not implemented yet.");
            }
        }
        */
        
        //This location list generated with relation to FieldService table entries and employee [and due date when online].
        public static DataTable getLocationList()
        {
            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = " select distinct L.serverKey as ID_Location,L.Name from Locations L,FieldService FS,Employees E where L.ServerKey<>0 and L.ServerKey=FS.ID_Location and E.ServerKey=FS.ID_Employee and FS.ID_Employee="+ Login.ID +" order by L.Name";
                    DataTable dtList = new DataTable();
                    localDB.FillDataTable(ref dtList, strSql);
                    return dtList;
                }
            }
            else
            {
                //throw new ApplicationException("Online functionality not implemented yet.");
                RConnection.RConnection OnConn = new RConnection.RConnection();
                OnConn.Url = Login.webConURL;
                string strSql;
                strSql = "select distinct L.ID_Location,L.Name from Locations L,FieldService FS,Employees E where L.ID_Location=FS.ID_Location and E.ID_Employee=FS.ID_Employee and FS.ID_Employee=" + Login.ID + " and FS.dueDate >= '" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "' order by L.Name";
                return (DataTable)OnConn.getDataTable(strSql);
            }
        }

        //This Group list generate with relation to FieldService table.
        public static DataTable getGroups(Int32 ParentID)
        {
            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = " select ServerKey as ID_AssetGroup,Name from Asset_Groups where ParentID =" + ParentID + "";
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

        //This Asset list generate with relation to fieldservice table.
        public static DataTable getAssetsByLocation(Int32 ID_Location)
        {
            string strSql;
            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    DataTable dtAssets = new DataTable("dtAssets");
                    strSql = "select A.*, isNULL(E.UserName,'') as Employee from Assets A Left outer join Employees E on A.ID_Employee=E.ServerKey where A.ID_Location = " + ID_Location + " and  A.RowStatus=" + Convert.ToInt32(RowStatus.Synchronized);
                    localDB.FillDataTable(ref dtAssets, strSql);
                    return dtAssets;
                }
            }
            else
            {
                RConnection.RConnection OnConn = new RConnection.RConnection();
                OnConn.Url = Login.webConURL;
                strSql = "select Top " + Login.ItemLimit.ToString() + " A.*, isNULL(E.UserName,'') as Employee from Assets A Left outer join Employees E on A.ID_Employee=E.ID_Employee where A.ID_Location = " + ID_Location;
                return OnConn.getDataTable(strSql);
                //strSql = "exec Update_Asset_Location_by_HH '" + TagID + "' ," + ID_Location + ", " + Login.ID + " ,0,'" + ReferenceNo + "'";
                //OnConn.runQuery(strSql);
                //throw new ApplicationException("On line FS section not implemented yet."); 

            }

        }

        //public void AssignNewTag(String Title)
        //{
        //    if (!Login.OnLineMode)
        //    {
        //        using (CEConn localDB = new CEConn())
        //        {
        //            string strSql;
        //            if (_RowStatus != Convert.ToInt32(RowStatus.New))
        //            {
        //                _RowStatus = Convert.ToInt32(RowStatus.TagWrite);
        //            }
        //            strSql = " update Locations set Title='" + Title + "', Date_Modified=getDate(), ModifiedBy=" + Login.ID + ", RowStatus=" + _RowStatus + " where ServerKey=" + _ServerKey;
        //            localDB.runQuery(strSql);
        //            _Title = Title;
        //        }
        //    }
        //    else
        //    {
        //        throw new ApplicationException("Online Location functionality not implemented yet.");
        //    }
        //}

        //public static void AddLocation(String Title, String LocationNo, String LocationName)
        //{
        //    if (!Login.OnLineMode)
        //    {
        //        Int32 Skey;
        //        Skey = minServerKey();
        //        using (CEConn localDB = new CEConn())
        //        {
        //            string strSql;
        //            strSql = " select count(*) from Assets A,Employees E,Locations L where A.Title='" + TagID + "' or E.Title='" + Title + "' or L.Title='" + Title + "'";
        //            //select count(*) from Locations where Title='" + Title + "'";
        //            if (Convert.ToInt32(localDB.getScalerValue(strSql)) > 0)
        //                throw new ApplicationException("Duplicate Tag ID.");

        //            strSql = " insert into Locations(Title,Name,LocationNo,Date_Modified,ModifiedBy,RowStatus,serverKey)";
        //            strSql += " values('" + Title + "','" + LocationName.Replace("'", "''") + "','" + LocationNo.Replace("'", "''") + "',getDate()," + Login.ID + "," + Convert.ToInt32(RowStatus.New) + "," + Skey + ")";
        //            localDB.runQuery(strSql);
        //        }
        //    }
        //    else
        //    {
        //        DataTable dtLocation = new DataTable("dtLocation");

        //        dtLocation.Columns.Add("Title", typeof(String));
        //        dtLocation.Columns.Add("Name", typeof(String));
        //        dtLocation.Columns.Add("LocationNo", typeof(String));
        //        dtLocation.Columns.Add("Date_Modified", typeof(DateTime));
        //        dtLocation.Columns.Add("ModifiedBy", typeof(Int32));
        //        dtLocation.AcceptChanges();

        //        DataRow dr;
        //        dr = dtLocation.NewRow();

        //        dr["Title"] = Title;
        //        dr["Name"] = LocationName;
        //        dr["LocationNo"] = LocationNo;
        //        dr["Date_Modified"] = DateTime.Now;
        //        dr["ModifiedBy"] = Login.ID;

        //        dtLocation.Rows.Add(dr);
        //        dtLocation.AcceptChanges();

        //        Synchronize.Synchronise OnConn = new Synchronize.Synchronise();
        //        OnConn.Url = Login.webURL;
        //        DataTable dtResult;
        //        dtResult = OnConn.AddLocation(dtLocation);

        //        if (dtResult.Rows.Count != 0)
        //        {
        //            if (Convert.ToInt32(dtResult.Rows[0]["RowStatus"]) == Convert.ToInt32(RowStatus.Error))
        //            {
        //                throw new ApplicationException("Insert Failed.");
        //            }
        //            else
        //            {
        //                //Inserted Sucessfully
        //            }
        //        }
        //        //throw new ApplicationException("Online functionality of Location not implemented yet.");
        //    }
        //}

        //private static Int32 minServerKey()
        //{
        //    using (CEConn localDB = new CEConn())
        //    {
        //        string strSql;
        //        strSql = "select min(serverKey) from Locations";
        //        object o;
        //        o = localDB.getScalerValue(strSql);
        //        if (DBNull.Value == o)
        //            return -1;
        //        else if (Convert.ToInt32(o) < 0)
        //            return (Convert.ToInt32(o) - 1);
        //        else
        //            return -1;
        //    }
        //}

        //public static DataTable getNewRows()
        //{
        //    using (CEConn localDB = new CEConn())
        //    {
        //        DataTable dtLocation = new DataTable("dtLocation");
        //        string strSql;


        //        strSql = "select * from Locations where RowStatus=" + Convert.ToInt32(RowStatus.New);
        //        localDB.FillDataTable(ref dtLocation, strSql);

        //        //strSql = "update Locations set RowStatus=" + Convert.ToInt32(RowStatus.InProcess) + " where RowStatus=" + Convert.ToInt32(RowStatus.New);
        //        //localDB.runQuery(strSql);

        //        return dtLocation;
        //    }

        //}

        //public static string getDispatchLocation()
        //{
        //    string strSql;
        //    if (Login.OnLineMode)
        //    {
        //        RConnection.RConnection OnConn = new RConnection.RConnection();
        //        OnConn.Url = Login.webConURL;
       
        //        strSql = " select isNULL(ID_Location,'') from Locations where Name like '%dispatch%' and is_deleted=0 and is_active=1";
        //        return OnConn.getScalerValue(strSql);
        //    }
        //    else
        //    {
        //        using (CEConn localDB = new CEConn())
        //        {
        //            strSql = " select ServerKey as ID_Location from Locations where Name like '%dispatch%' and ServerKey<>0 ";
        //            return Convert.ToString(localDB.getScalerValue(strSql));
        //        }
        //    }
        //}
    }
}
