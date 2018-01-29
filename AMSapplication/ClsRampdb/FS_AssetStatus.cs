/**************************************************************************************
 * Author : Deepanshu Jouhari
 * Modified Date : 15-Jul-2009
 * Last Modified by : 
 * Last Modified : 
 * Module Name : ClsRampDb
 * Decription : FS_AssetStatus class to implement Field Service functionality and intrafacing the table FS_AssetStatus
 **************************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using ClsLibBKLogs;

namespace ClsRampdb
{
    public class FS_AssetStatus
    {
        #region "variables & Procedures."
        
        Int32 _ID_Tasks;
        Int32 _ID_Asset;
        Int32 _ID_Template;
        DateTime _DueDate;
        Int32 _TaskStatus;
        Int32 _FSStatus;
        Int32 _ID_Employee;
        Int32 _ServerKey;
        Int32 _RowStatus;

        public Int32 ServerKey
        {
            get
            {
                return _ServerKey;
            }
        }


        public Int32 ID_Tasks
        {
            get
            {
                return _ID_Tasks;
            }
        }
        public Int32 ID_Template
        {
            get
            {
                return _ID_Template;
            }
        }
        public DateTime DueDate
        {
            get
            {
                return _DueDate;
            }
        }
        public Int32 TaskStatus
        {
            get
            {
                return _TaskStatus;
            }
        }
        public Int32 FSStatus
        {
            get
            {
                return _FSStatus;
            }
        }
        public Int32 ID_Employee
        {
            get
            {
                return _ID_Employee;
            }
        }


        public Int32 ID_Asset
        {
            get
            {
                return _ID_Asset;
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

        public FS_AssetStatus(Int32 FSID)
        {

            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = " select * from FS_AssetStatus where ServerKey=" + FSID;
                    SqlCeDataReader dr;
                    dr = localDB.getReader(strSql);
                    while (dr.Read())
                    {
                        _ServerKey = FSID;
                        _ID_Tasks = (Int32)dr["ID_Tasks"];
                        _ID_Asset = (Int32)dr["ID_Asset"];
                        _ID_Template = (Int32)dr["ID_Template"];
                        _DueDate = (DateTime)dr["DueDate"];
                        _FSStatus = (Int32)dr["FSStatus"];
                        _TaskStatus = (Int32)dr["TaskStatus"];  
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
                strSql = " select F.ID_Tasks, F.ID_Asset, F.ID_Template, F.DueDate, F.TaskStatus, F.FSStatus, F.ID_Employee  from FieldService where ID_FieldService=" + FSID;
                DataTable dt;
                dt = OnConn.getDataTable(strSql);
                if (dt.Rows.Count != 0)
                {
                    _ServerKey = FSID;
                    _ID_Tasks = (Int32)dt.Rows[0]["ID_Tasks"];
                    _ID_Asset = (Int32)dt.Rows[0]["ID_Asset"];
                    _ID_Template = (Int32)dt.Rows[0]["ID_Template"];
                    _DueDate = (DateTime)dt.Rows[0]["DueDate"];
                    _FSStatus = (Int32)dt.Rows[0]["FSStatus"];
                    _TaskStatus = (Int32)dt.Rows[0]["TaskStatus"];  
                    _ID_Employee = (Int32)dt.Rows[0]["ID_Employee"];
                    _RowStatus = Convert.ToInt32(RowStatus.Synchronized);
                }
                
            }
        }

        public static string Update(Int32 FSID,FSStatusType fst,FSStatusType tst,String Comments)
        {
            string strSql;
            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    strSql = "Update FS_AssetStatus set FSStatus=" + Convert.ToInt32(fst) + ", TaskStatus =" + Convert.ToInt32(tst) + ", comments ='" + Comments + "', Date_Modified = getDate(), ModifiedBy =" + Login.ID + ", ID_Employee =" + Login.ID + ", RowStatus= " + Convert.ToInt32(RowStatus.Modify) + " where ID_FSAssetStatus=" + FSID;
                    localDB.runQuery(strSql);
                    return "";
                }
            }
            else
            {
                //Update FS_AssetStatus

                Synchronize.Synchronise OnConn = new Synchronize.Synchronise();
                OnConn.Url = Login.webURL;
                String result;
                result = OnConn.UpdateFS_AssetStatus1(FSID, Convert.ToInt32(fst), Convert.ToInt32(tst), Login.ID, DateTime.Now, Comments);

                return result; 
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
            StringBuilder sql = new StringBuilder("");
            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = " select distinct L.serverKey as ID_Location,L.Name " 
                     + " from Locations L,FS_AssetStatus FS,Assets A " 
                     + " where L.ServerKey<>0 and FS.ID_Asset=A.ServerKey "
                     + " and (FS.FSStatus=" + Convert.ToInt32(FSStatusType.New) + " or FS.FSStatus=" + Convert.ToInt32(FSStatusType.InProcess) + ")" 
                     + " and L.ServerKey=A.ID_Location order by L.Name";
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
                //strSql = "select distinct L.ID_Location,L.Name from Locations L,FieldService FS,Employees E where L.ID_Location=FS.ID_Location and E.ID_Employee=FS.ID_Employee and FS.ID_Employee=" + Login.ID + " and FS.dueDate >= '" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "' order by L.Name";
                strSql = " select distinct L.ID_Location,L.Name " 
                     + " from Locations L,FS_AssetStatus FS,Assets A " 
                     + " where FS.ID_Asset=A.ID_Asset "
                     + " and (FS.FSStatus=" + Convert.ToInt32(FSStatusType.New) + " or FS.FSStatus=" + Convert.ToInt32(FSStatusType.InProcess) + ")" 
                     + " and L.ID_Location=A.ID_Location and FS.Is_Active=1 and FS.Is_Deleted=0 order by L.Name";

                return (DataTable)OnConn.getDataTable(strSql);
            }
        }

        //This Group list generate with relation to FieldService table.
        public static DataTable getGroups(Int32 ParentID)
        { 
            StringBuilder sql = new StringBuilder("");
            if (!Login.OnLineMode)
            {              
                using (CEConn localDB = new CEConn())
                {
                    string strSql;  
                    
                    sql.Append(" select distinct G.ServerKey as ID_AssetGroup,G.Name from Asset_Groups G, Assets A, FS_AssetStatus FS ");
                    sql.Append(" where G.ParentID =" + ParentID + " and FS.ID_Asset=A.ServerKey ");
                    sql.Append(" and (FS.FSStatus=" + Convert.ToInt32(FSStatusType.New) + " or FS.FSStatus=" + Convert.ToInt32(FSStatusType.InProcess) + ")");
                    sql.Append(" and (A.GroupIDs like '%,'+ Convert(nvarchar(6),G.ID_AssetGroup) + ',%' or A.ID_AssetGroup=G.ServerKey)");
                    
                    //strSql = " select distinct G.ServerKey as ID_AssetGroup,G.Name from Asset_Groups G, Assets A, FS_AssetStatus FS "
                    // + " where G.ParentID =" + ParentID + " and FS.ID_Asset=A.ServerKey "
                    // + " and (FS.FSStatus=" + Convert.ToInt32(FSStatusType.New) + " or FS.FSStatus=" + Convert.ToInt32(FSStatusType.InProcess) + ")"
                    // + " and (A.GroupIDs like '%,'+ Convert(nvarchar(6),G.ID_AssetGroup) + ',%' or A.ID_AssetGroup=G.ServerKey)";
                    
                    strSql = sql.ToString();

                   // strSql = " select distinct G.ServerKey as ID_AssetGroup,G.Name from Asset_Groups G where G.ParentID = " + ParentID;

                    DataTable dtList = new DataTable();
                    localDB.FillDataTable(ref dtList, strSql);
                    //return localDB.GetGroups(strSql);
                    return dtList;
                }
            }
            else
            {
                RConnection.RConnection OnConn = new RConnection.RConnection();
                OnConn.Url = Login.webConURL;
                string strSql;
                //strSql = " select ID_AssetGroup,Name from Asset_Groups where ParentID =" + ParentID + " and Is_Active=1 and Is_Deleted=0";

                sql.Append(" select distinct G.ID_AssetGroup,G.Name from Asset_Groups G, Assets A, FS_AssetStatus FS ");
                sql.Append(" where G.ParentID =" + ParentID + " and FS.ID_Asset=A.ID_Asset ");
                sql.Append(" and (FS.FSStatus=" + Convert.ToInt32(FSStatusType.New) + " or FS.FSStatus=" + Convert.ToInt32(FSStatusType.InProcess) + ")");
                sql.Append(" and (dbo.GetParentGroups(A.ID_AssetGroup) like '%,'+ Convert(nvarchar(6),G.ID_AssetGroup) + ',%' or A.ID_AssetGroup=G.ID_AssetGroup) and FS.IS_Active=1 and FS.IS_Deleted=0");

                //strSql = " select distinct G.ID_AssetGroup,G.Name from Asset_Groups G, Assets A, FS_AssetStatus FS " 
                //     + " where G.ParentID =" + ParentID + " and FS.ID_Asset=A.ID_Asset "
                //     + " and (FS.FSStatus=" + Convert.ToInt32(FSStatusType.New) + " or FS.FSStatus=" + Convert.ToInt32(FSStatusType.InProcess) + ")"
                //     + " and (dbo.GetParentGroups(A.ID_AssetGroup) like '%,'+ Convert(nvarchar(6),G.ID_AssetGroup) + ',%' or A.ID_AssetGroup=G.ID_AssetGroup) and FS.IS_Active=1 and FS.IS_Deleted=0";
                
                DataTable dt;
                dt = (DataTable)OnConn.getDataTable(sql.ToString());
                return dt;

                //throw new ApplicationException("Online functionality not implemented yet.");
            }
        }

        //This Asset list generate with relation to fieldservice table.
        public static DataTable getAssets(Int64 ID_Location,Int64 ID_AssetGroup,Int64 ID_Asset)
        {
            string strSql;
            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    DataTable dtAssets = new DataTable("dtAssets");
                    strSql = "select distinct A.ServerKey as ID_Asset, A.TagID,A.Name,A.ID_Employee,A.Date_Modified,A.ID_Location,A.ID_AssetGroup,A.RowStatus,A.AssetNo,A.GroupIDs, E.UserName as Employee from Assets A " + 
                         " Left outer join Employees E on A.ID_Employee=E.ServerKey " 
                        + " inner join FS_AssetStatus F on A.ServerKey=F.ID_Asset "
                        + " where (A.ID_Location = " + ID_Location + " or 0=" + ID_Location + "  or -2=" + ID_Location + ")"
                        + " and (A.GroupIDs like '%," + ID_AssetGroup + ",%' or A.ID_AssetGroup = " + ID_AssetGroup + " or 0=" + ID_AssetGroup + " or -2=" + ID_AssetGroup + ")"
                        + " and  (F.FSStatus=" + Convert.ToInt32(FSStatusType.New) + " or F.FSStatus=" + Convert.ToInt32(FSStatusType.InProcess) + ")" 
                        + " and  (A.ServerKey=" + ID_Asset + " or 0 ="+ ID_Asset +") ";
                    
                    localDB.FillDataTable(ref dtAssets, strSql);
                    return dtAssets;
                }
            }
            else
            {
                RConnection.RConnection OnConn = new RConnection.RConnection();
                OnConn.Url = Login.webConURL;
                strSql = "select distinct top 100 A.*, isNULL(E.UserName,'') as Employee from Assets A "  
                        + " Left outer join Employees E on A.ID_Employee=E.ID_Employee " 
                        + " inner join FS_AssetStatus F on A.ID_Asset=F.ID_Asset "
                        + " where (A.ID_Location = " + ID_Location + " or 0=" + ID_Location + " or -2=" + ID_Location + ")"
                        + " and (dbo.GetParentGroups(A.ID_AssetGroup) like '%," + ID_AssetGroup + ",%' or A.ID_AssetGroup = " + ID_AssetGroup + " or 0=" + ID_AssetGroup + " or -2=" + ID_AssetGroup + ")"
                        + " and  (A.ID_Asset=" + ID_Asset + " or 0 =" + ID_Asset + ")"
                        + " and  (F.FSStatus=" + Convert.ToInt32(FSStatusType.New) + " or F.FSStatus=" + Convert.ToInt32(FSStatusType.InProcess) + ")" 
                        + " and  F.IS_Active=1 and F.Is_Deleted=0";

                return OnConn.getDataTable(strSql);
                //strSql = "exec Update_Asset_Location_by_HH '" + TagID + "' ," + ID_Location + ", " + Login.ID + " ,0,'" + ReferenceNo + "'";
                //OnConn.runQuery(strSql);
                //throw new ApplicationException("On line FS section not implemented yet."); 

            }

        }

        public static DataTable getRows(RowStatus Stat)
        {
            using (CEConn localDB = new CEConn())
            {
                DataTable dtFS = new DataTable("dtFS");
                string strSql;


                strSql = "select FS.*,A.TagID,A.Name from FS_AssetStatus FS,Assets A where FS.ID_Asset=A.ServerKey and  FS.RowStatus=" + Convert.ToInt32(Stat);
                localDB.FillDataTable(ref dtFS, strSql);

                return dtFS;
            }
        }

        public static DataTable getTemplateList(Int64 ID_Asset)
        {
            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = " select distinct T.serverKey as ID_Template,T.Title "
                     + " from FS_Templates T,FS_AssetStatus FS, Assets A"
                     + " where T.ServerKey<>0 and FS.ID_Template=T.ServerKey and FS.ID_Asset=A.ServerKey "
                     + " and (FS.FSStatus = " + Convert.ToInt32(FSStatusType.New) + " or FS.FSStatus = " + Convert.ToInt32(FSStatusType.InProcess) + ")" 
                     +" and A.ServerKey="+ ID_Asset +" order by T.Title";
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
                //strSql = "select distinct L.ID_Location,L.Name from Locations L,FieldService FS,Employees E where L.ID_Location=FS.ID_Location and E.ID_Employee=FS.ID_Employee and FS.ID_Employee=" + Login.ID + " and FS.dueDate >= '" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "' order by L.Name";
                
                strSql = " select distinct T.ID_Template,T.Title "
                       + " from FS_Templates T, FS_AssetStatus FS, Assets A "
                       + " where T.ID_Template<>0 and FS.ID_Template=T.ID_Template and FS.ID_Asset=A.ID_Asset and A.ID_Asset=" + ID_Asset 
                       + " and (FS.FSStatus <> " + Convert.ToInt32(FSStatusType.New) + " or FS.FSStatus <> " + Convert.ToInt32(FSStatusType.InProcess) + ")"
                       + " and FS.Is_Active=1 and FS.Is_Deleted=0 order by T.Title";
                
                //strSql = " select distinct L.ID_Location,L.Name "
                //     + "from Locations L,FS_AssetStatus FS,Assets A "
                //     + "where FS.ID_Asset=A.ID_Asset and L.ID_Location=A.ID_Location and FS.Is_Active=1 and FS.Is_Deleted=0 order by L.Name";

                return (DataTable)OnConn.getDataTable(strSql);
            }
        }

        public static DataTable getTaskList(Int64 ID_Asset,Int64 ID_Template)
        {
            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = " select Tsk.serverKey as ID_Tasks,Tsk.Title,FS.FSStatus,FS.TaskStatus,FS.ID_FSAssetStatus,FS.Comments,FS.DueDate "
                     + " from FS_Templates T,FS_AssetStatus FS, Assets A, Tasks Tsk"
                     + " where T.ServerKey<>0 and FS.ID_Template=T.ServerKey and FS.ID_Asset=A.ServerKey "
                     + " and (FS.FSStatus = " + Convert.ToInt32(FSStatusType.New) + " or FS.FSStatus = " + Convert.ToInt32(FSStatusType.InProcess) + ")"
                     + " and T.ServerKey = " + ID_Template + " and FS.ID_Tasks=Tsk.ServerKey"
                     + " and A.ServerKey=" + ID_Asset + " order by Tsk.Title";
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
                //strSql = "select distinct L.ID_Location,L.Name from Locations L,FieldService FS,Employees E where L.ID_Location=FS.ID_Location and E.ID_Employee=FS.ID_Employee and FS.ID_Employee=" + Login.ID + " and FS.dueDate >= '" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "' order by L.Name";

                strSql = " select distinct Tsk.ID_Tasks,Tsk.Title,FS.FSStatus,FS.TaskStatus,FS.ID_FSAssetStatus,FS.Comments,FS.DueDate "
                       + " from FS_Templates T, FS_AssetStatus FS, Assets A, Tasks as Tsk "
                       + " where T.ID_Template<>0 and FS.ID_Template=T.ID_Template and FS.ID_Asset=A.ID_Asset and A.ID_Asset=" + ID_Asset
                       + " and (FS.FSStatus = " + Convert.ToInt32(FSStatusType.New) + " or FS.FSStatus = " + Convert.ToInt32(FSStatusType.InProcess) + ")"
                       + " and T.ID_Template = " + ID_Template + " and FS.ID_Tasks=Tsk.ID_Tasks"
                       + " and FS.Is_Active=1 and FS.Is_Deleted=0 order by Tsk.Title";

                //strSql = " select distinct L.ID_Location,L.Name "
                //     + "from Locations L,FS_AssetStatus FS,Assets A "
                //     + "where FS.ID_Asset=A.ID_Asset and L.ID_Location=A.ID_Location and FS.Is_Active=1 and FS.Is_Deleted=0 order by L.Name";

                return (DataTable)OnConn.getDataTable(strSql);
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
