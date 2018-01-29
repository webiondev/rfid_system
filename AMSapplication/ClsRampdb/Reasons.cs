/**************************************************************************************
 * Author : Deepanshu Jouhari
 * Modified Date : 15-Aug-2009
 * Last Modified by : 
 * Last Modified : 
 * Module Name : ClsRampDb
 * Decription : Reasons class to implement Reason functionality for Vernon (Ramp Client)
 **************************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;

namespace ClsRampdb
{
    public class Reasons
    {
        //public Reasons(Int32 ReasonID)
        //{

        //    if (!Login.OnLineMode)
        //    {
        //        using (CEConn localDB = new CEConn())
        //        {
        //            string strSql;
        //            strSql = " select * from Locations where ServerKey=" + LocID;
        //            SqlCeDataReader dr;
        //            dr = localDB.getReader(strSql);
        //            while (dr.Read())
        //            {
        //                _ServerKey = LocID;
        //                _TagNo = (String)dr["TagID"];
        //                _Name = (String)dr["Name"];
        //                _RowStatus = Convert.ToInt32(dr["RowStatus"]);
        //            }
        //            dr.Close();
        //        }
        //    }
        //    else
        //    {
        //        RConnection.RConnection OnConn = new RConnection.RConnection();
        //        OnConn.Url = Login.webConURL;
        //        string strSql;
        //        strSql = " select TagID,Name from Locations where ID_Location=" + LocID;
        //        DataTable dt;
        //        dt = OnConn.getDataTable(strSql);
        //        if (dt.Rows.Count != 0)
        //        {
        //            _ServerKey = LocID;
        //            _TagNo = (String)dt.Rows[0]["TagID"];
        //            _Name = (String)dt.Rows[0]["Name"];
        //            _RowStatus = Convert.ToInt32(RowStatus.Synchronized);
        //        }
        //        //throw new ApplicationException("Online functionality not implemented yet.");
        //    }
        //}

        public static DataTable getReasonsList()
        {
            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = " select serverKey as ID_Reason,Name from Reasons where ServerKey<>0 order by Name";
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
                strSql = " select ID_Reason,Name from Reasons where Is_Active=1 and Is_Deleted=0 order by Name";
                return (DataTable)OnConn.getDataTable(strSql);
                //throw new ApplicationException("Online functionality not implemented yet.");
            }
        }
       
    }
}
