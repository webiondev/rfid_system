/**************************************************************************************
 * Author : Deepanshu Jouhari
 * Modified Date : 15-Jun-2009
 * Last Modified by : 
 * Last Modified : 
 * Module Name : ClsRampDb
 * Decription : Employee class to interface SQL CE Employee Table
 **************************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using ClsLibBKLogs;
namespace ClsRampdb
{
    public class Employee
    {
        String _TagID,_EmpNo,_UserName;
        Int32 _ID_SecurityGroup;
        Int32 _ServerKey;
        Int32 _RowStatus;

        public Int32 ID_SecurityGroup
        {
            get
            {
                return _ID_SecurityGroup;
            }
        }

        public String TagNo
        {
            get
            {
                return _TagID;
            }
        }

        public String EmpNo
        {
            get
            {
                return _EmpNo;
            }
        }

        public RowStatus Status
        {
            get
            {
                return (RowStatus) _RowStatus;
            }
        }

        public String UserName
        {
            get
            {
                return _UserName;
            }
        }

        public Int32 ServerKey
        {
            get
            {
                return _ServerKey;
            }
        }

        public Employee(Int32 EmpID)
        {
            _ServerKey = EmpID;
            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = " select * from Employees where ServerKey=" + _ServerKey;
                    SqlCeDataReader dr;
                    dr = localDB.getReader(strSql);
                    while (dr.Read())
                    {
                        _ID_SecurityGroup = Convert.ToInt32(dr["ID_SecurityGroup"]);
                        _TagID = (String)dr["TagID"];
                        if (dr["EmpNo"] != DBNull.Value)
                            _EmpNo = (String)dr["EmpNo"];
                        _UserName = (String)dr["UserName"];
                        _ServerKey = Convert.ToInt32(dr["ServerKey"]);
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
                strSql = " select ID_Employee,ID_SecurityGroup,TagID,EmpNo,UserName from Employees where ID_Employee=" + _ServerKey;
                DataTable dt;
                dt = OnConn.getDataTable(strSql);
                if (dt.Rows.Count!=0)
                {
                    _ID_SecurityGroup = Convert.ToInt32(dt.Rows[0]["ID_SecurityGroup"]);
                    _TagID = (String)dt.Rows[0]["TagID"];
                    if (dt.Rows[0]["EmpNo"] != DBNull.Value)
                        _EmpNo = (String)dt.Rows[0]["EmpNo"];
                    _UserName = (String)dt.Rows[0]["UserName"];
                    _ServerKey = Convert.ToInt32(dt.Rows[0]["ID_Employee"]);
                    _RowStatus = Convert.ToInt32(RowStatus.Synchronized);  
                }
                

                //throw new ApplicationException("Online Employee functionality not implemented yet.");
            }
        }

        public Employee()
        {
            _ServerKey = 0;
        }

        public static DataTable getNewRows()
        {
            using (CEConn localDB = new CEConn())
            {
                DataTable dtEmployee = new DataTable("dtEmployee");
                string strSql;


                strSql = "select * from Employees where RowStatus=" + Convert.ToInt32(RowStatus.New) + " OR ServerKey < 0";
                localDB.FillDataTable(ref dtEmployee, strSql);

                //strSql = "update Employees set RowStatus=" + Convert.ToInt32(RowStatus.InProcess) + " where RowStatus=" + Convert.ToInt32(RowStatus.New);
                //localDB.runQuery(strSql);

                return dtEmployee; 
            }
        }

        public static DataTable getModifiedRows()
        {
            using (CEConn localDB = new CEConn())
            {
                DataTable dtEmployee = new DataTable("dtEmployee");
                string strSql;


                strSql = "select * from Employees where RowStatus=" + Convert.ToInt32(RowStatus.Modify);
                localDB.FillDataTable(ref dtEmployee, strSql);

                //strSql = "update Employees set RowStatus=" + Convert.ToInt32(RowStatus.InProcess) + " where RowStatus=" + Convert.ToInt32(RowStatus.New);
                //localDB.runQuery(strSql);

                return dtEmployee;
            }
        }
     
        public void AssignNewTag(String TagNo)
        {
            Int32 p;
            if (_RowStatus != Convert.ToInt32(RowStatus.New))
            {
                p = Convert.ToInt32(RowStatus.TagWrite);
            }
            else
            {
                p = _RowStatus;
            }

            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = " update Employees set TagID='" + TagNo + "' , Date_MOdified=getDate(), ModifiedBy=" + Login.ID + ", RowStatus=" + p + " where ServerKey=" + _ServerKey;
                    localDB.runQuery(strSql);
                    _TagID = TagNo;
                    _RowStatus = p;
                }
            }
            else
            {
                RConnection.RConnection OnConn = new RConnection.RConnection();
                OnConn.Url = Login.webConURL;
                string strSql;
                strSql = " update Employees set TagID='" + TagNo + "' , Date_MOdified=getDate(), Last_Modified_By=" + Login.ID + " where ID_Employee=" + _ServerKey;
                OnConn.runQuery(strSql);
                _TagID = TagNo;
                _RowStatus = p;
                //throw new ApplicationException("Online Employee functionality not implemented yet.");
            }
        }

        public static void AddEmployee(String TagID, String EmpNo, String UserName,String Password , Int32 RoleID)
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
                    
                   
                    TagInfo Ti = new TagInfo(TagID);
                    if (Ti.isAssetTag() || Ti.isLocationTag() || Ti.isEmployeeTag())
                        tagAssigned = true;
                    else
                        tagAssigned = false;

                    //strSql = " select count(*) from Assets A,Employees E,Locations L where A.TagID='" + TagID + "' or E.TagID='" + TagID + "' or L.TagID='" + TagID + "'";
                    ////select count(*) from Locations where TagID='" + TagID + "'";
                    if (tagAssigned == true && !string.IsNullOrEmpty(TagID))
                        throw new ApplicationException("Duplicate Tag ID.");

                    strSql = " insert into Employees(TagID,UserName,EmpNo,Password,Date_Modified,ID_SecurityGroup,ModifiedBy,RowStatus,serverKey)";
                    strSql += " values('" + TagID + "','" + UserName.Replace("'", "''") + "','" + EmpNo.Replace("'", "''") + "','" + Password.Replace("'", "''") + "',getDate()," + RoleID + "," + Login.ID + "," + Convert.ToInt32(RowStatus.New) + "," + Skey + ")";
                    localDB.runQuery(strSql);

                }
            }
            else
            {

                DataTable dtEmployee = new DataTable("dtEmployee");
                
                dtEmployee.Columns.Add("EmpNo", typeof(String));
                dtEmployee.Columns.Add("UserName", typeof(String));
                dtEmployee.Columns.Add("Password", typeof(String));
                dtEmployee.Columns.Add("TagID", typeof(String));
                dtEmployee.Columns.Add("ID_SecurityGroup", typeof(Int32));
                dtEmployee.Columns.Add("ModifiedBy", typeof(Int32));
                dtEmployee.AcceptChanges();

                DataRow dr;
                dr = dtEmployee.NewRow();
                
                dr["EmpNo"] = EmpNo; 
                dr["UserName"] = UserName; 
                dr["Password"] = Password; 
                dr["TagID"] = TagID;
                dr["ID_SecurityGroup"] = RoleID; 
                dr["ModifiedBy"] = Login.ID; 

                dtEmployee.Rows.Add(dr);
                dtEmployee.AcceptChanges();

                Synchronize.Synchronise OnConn = new Synchronize.Synchronise();
                OnConn.Url = Login.webURL; 
                DataTable dtResult;
                dtResult = OnConn.AddEmployee(dtEmployee);

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

                //throw new ApplicationException("Online functionality not implemented yet.");
            }
        }

        private static Int32 minServerKey()
        {
            using (CEConn localDB = new CEConn())
            {
                string strSql;
                strSql = "select min(serverKey) from Employees";
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
