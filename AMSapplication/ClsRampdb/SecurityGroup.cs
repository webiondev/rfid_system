/**************************************************************************************
 * Author : Deepanshu Jouhari
 * Modified Date : 15-Jun-2009
 * Last Modified by : 
 * Last Modified : 
 * Module Name : ClsRampDb
 * Decription : SecurityGroup class to interface SecurityGroup Table
 **************************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe; 

namespace ClsRampdb
{
    public class SecurityGroup
    {
        public static DataTable getSecurityGroups()
        {
            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = " select serverKey as ID_SecurityGroup,Name from Master_SecurityGroups where serverKey<>0 order by Name";
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
                strSql = " select ID_SecurityGroup,Name from Master_SecurityGroups where Is_Active=1 and Is_Deleted=0 order by Name";
                return (DataTable) OnConn.getDataTable(strSql);
                
                //throw new ApplicationException("Online functionality not implemented yet.");
            }
        }
    }
}
