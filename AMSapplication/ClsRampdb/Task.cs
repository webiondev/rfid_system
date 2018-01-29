/**************************************************************************************
 * Author : Deepanshu Jouhari
 * Modified Date : 15-Jul-2009
 * Last Modified by : 
 * Last Modified : 
 * Module Name : ClsRampDb
 * Decription : Task class to implement Task Table (Presently It is not used.)
 **************************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using ClsLibBKLogs;

namespace ClsRampdb
{
    public class Task
    {
        public delegate void TaskPerformedNotify(String ID_Asset);
        public static event TaskPerformedNotify taskPerformed;

        #region "variables & Procedures."
        //Int32 _ID_Tasks;
        String _Title;
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

        //public Int32 ID_Tasks
        //{
        //    get
        //    {
        //        return _ID_Tasks;
        //    }
        //}

        public RowStatus Status
        {
            get
            {
                return (RowStatus)_RowStatus;
            }
        }

        #endregion

        public Task(Int32 TaskID)
        {

            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = " select * from Tasks where ServerKey=" + TaskID;
                    SqlCeDataReader dr;
                    dr = localDB.getReader(strSql);
                    while (dr.Read())
                    {
                        _ServerKey = TaskID;
                        _Title = (String)dr["Title"];
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
                strSql = " select Title from Tasks where ID_Tasks=" + TaskID;
                DataTable dt;
                dt = OnConn.getDataTable(strSql);
                if (dt.Rows.Count != 0)
                {
                    _ServerKey = TaskID; 
                    _Title = (String)dt.Rows[0]["Title"];
                    _RowStatus = Convert.ToInt32(RowStatus.Synchronized);
                }
                
            }
        }

        public static DataTable getTasksList()
        {
            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = " select serverKey as ID_Tasks,Title from Tasks where ServerKey<>0 order by Title";
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
                strSql = " select ID_Tasks,Title from Tasks where Is_Active=1 and Is_Deleted=0 order by Title";
                return (DataTable)OnConn.getDataTable(strSql);
            }
        }

        public static void AddTaskPerformed(Int32 TaskID,Int32 EmployeeID,Int32 ItemId)
        {
            if (!Login.OnLineMode)
            {
                Int32 Skey;
                Skey = minServerKey();

                // added by vijay 08 Dec 2009
                if (Skey == -2)
                    Skey = Skey - 1;

                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = " insert into PerformedTasks(ID_Tasks,ID_Employee,ID_Asset,Date_Modified,ModifiedBy,RowStatus,serverKey)";
                    strSql += " values(" + TaskID + "," + EmployeeID + "," + ItemId + ",getDate()," + EmployeeID + "," + Convert.ToInt32(RowStatus.New) + "," + Skey + ")";
                    localDB.runQuery(strSql);
                    if(taskPerformed !=null) 
                        taskPerformed(ItemId.ToString()); 
                }
            }
            else
            {
                DataTable dtPTask = new DataTable("dtPTask");

                dtPTask.Columns.Add("ID_Tasks", typeof(Int32));
                dtPTask.Columns.Add("ID_Employee", typeof(Int32));
                dtPTask.Columns.Add("ID_Asset", typeof(Int32));
                dtPTask.Columns.Add("Date_Modified", typeof(DateTime));
                dtPTask.Columns.Add("ModifiedBy", typeof(Int32));
                dtPTask.AcceptChanges();

                DataRow dr;
                dr = dtPTask.NewRow();

                dr["ID_Tasks"] = TaskID;
                dr["ID_Employee"] = EmployeeID;
                dr["ID_Asset"] = ItemId; 
                dr["Date_Modified"] = DateTime.Now;
                dr["ModifiedBy"] = Login.ID;

                dtPTask.Rows.Add(dr);
                dtPTask.AcceptChanges();

                Synchronize.Synchronise OnConn = new Synchronize.Synchronise();
                OnConn.Url = Login.webURL;
                DataTable dtResult;
                dtResult = OnConn.AddPerformedTask(dtPTask);

                if (dtResult.Rows.Count != 0)
                {
                    if (Convert.ToInt32(dtResult.Rows[0]["RowStatus"]) == Convert.ToInt32(RowStatus.Error))
                    {
                        throw new ApplicationException("Insert Failed.");
                    }
                    else
                    {
                        if (taskPerformed != null)
                            taskPerformed(ItemId.ToString()); 
                        //Inserted Sucessfully
                    }
                }
                //throw new ApplicationException("Online functionality of Location not implemented yet.");
            }
        }

        public static DataTable getTaskPerformedRows(RowStatus Stat)
        {
            //The functionality is for insert data to centeralized data.
            using (CEConn localDB = new CEConn())
            {
                DataTable dtFS = new DataTable("dtFS");
                string strSql;

                strSql = "select ID_Tasks,ID_Employee,ID_Asset,ServerKey from PerformedTask where RowStatus=" + Convert.ToInt32(Stat);
                localDB.FillDataTable(ref dtFS, strSql);


                return dtFS;
            }
        }

        private static Int32 minServerKey()
        {
            using (CEConn localDB = new CEConn())
            {
                string strSql;
                strSql = "select min(serverKey) from PerformedTasks";
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

    }
}
