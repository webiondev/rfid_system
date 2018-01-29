/**************************************************************************************
 * Author : Deepanshu Jouhari
 * Modified Date : 15-Jul-2009
 * Last Modified by : 
 * Last Modified : 
 * Module Name : ClsRampDb
 * Decription : Login class and some structures and enums to validate user and store application specific variables.
 **************************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using ClsLibBKLogs;


namespace ClsRampdb
{

   

    public static class Login
    {
        //http://iibtest/RampService/
        public static string urlStr;
        public static string webURL = urlStr + "Synchronise.asmx";
        public static string webConURL = urlStr + "RConnection.asmx";
        private static string _TagID;
        private static string _EmpNo;
        private static string _UserName;
        private static string _Password;
        private static Int32 _RoleId;
        private static string _Role;
        private static bool _OnLine;
        private static Int32 _ID;
        public static string err = "";
        public static Int64 _ItemLimit = 0;

       // public static AppModules appModules = new AppModules();  

        public static String TagID
        {
            get
            {
                return _TagID;
            }
        }
        public static String EmpNo
        {
            get
            {
                return _EmpNo;
            }
        }
        public static String UserName
        {
            get
            {
                return _UserName;
            }
        }
        public static String Password
        {
            get
            {
                return _Password;
            }
        }
        public static Int32 RoleId
        {
            get
            {
                return _RoleId;
            }
        }
        public static String Role
        {
            get
            {
                return _Role;
            }
        }
        public static bool OnLineMode
        {
            get
            {
                return _OnLine;
            }
            set
            {
                _OnLine = value;
            }
        }
        public static Int32 ID
        {
            get
            {
                return _ID;
            }
        }

        public static Int64 ItemLimit
        {
            get
            {
                return _ItemLimit;
            }
        }

        public static bool verifyPassword(string username, string password)
        {
            bool result = false;
            try
            {
                webURL = urlStr + "Synchronise.asmx";
                webConURL = urlStr + "RConnection.asmx";
                if (!Login.OnLineMode)
                {

                    using (CEConn localDB = new CEConn())
                    {
                        string strSql;
                        strSql = "select E.*,grp.name as Role  from Employees as E inner join Master_SecurityGroups grp ";
                        strSql += " on grp.ServerKey=E.ID_SecurityGroup ";
                        strSql += " where E.username='" + username + "' and E.password='" + password + "'";
                        
                        SqlCeDataReader sr = localDB.getReader(strSql);
                        
                            while (sr.Read())
                            {
                                _ID = Convert.ToInt32(sr["ServerKey"]);
                                _TagID = (String)sr["TagID"];
                                _EmpNo = (String)sr["EmpNo"];
                                _UserName = (String)sr["UserName"];
                                //_Password = (String)sr["Password"];
                                _Password = password;
                                _RoleId = Convert.ToInt32(sr["ID_SecurityGroup"]);
                                _Role = (String)sr["Role"];
                               // sr.Close();

                                result = true;                                
                            }
                                        
                        sr.Close();
                        //return false;
                       
                    }

                    if (result == true)
                    {
                        if (ValidateUserInOnlineMode(username, password))
                            return true;
                        else
                            return false;
                    }
                    else
                        return result;

                }
                else
                {

                    RConnection.RConnection OnConn = new RConnection.RConnection();
                    OnConn.Url = Login.webConURL;
                    DataTable dt;
                    dt = (DataTable)OnConn.GetLogin(username, password);
                    if (dt.Rows.Count > 0)
                    {
                        _ID = Convert.ToInt32(dt.Rows[0]["ServerKey"]);
                        _TagID = (String)dt.Rows[0]["TagID"];
                        _EmpNo = (String)dt.Rows[0]["EmpNo"];
                        _UserName = (String)dt.Rows[0]["UserName"];
                        //_Password = (String)dt.Rows[0]["Password"];
                        _Password = password;
                        _RoleId = Convert.ToInt32(dt.Rows[0]["ID_SecurityGroup"]);
                        _Role = (String)dt.Rows[0]["Role"];
                        return true;
                    }
                    return false;
                    //throw new ApplicationException("Online functionality not implemented yet."); 
                    //Do the Online Coding.
                }

            }
            catch (System.Web.Services.Protocols.SoapException ex)
            {
                if (ex.Actor.ToString().ToLower().Trim() == "getlogin")
                    err = "Request from innvalid IP address.";
                else
                    err = "Soap Exception";

                Logger.LogError(ex.Message);

                throw new Exception(ex.Message);
            }
            catch (Exception ep)
            {
                err = ep.Message;
                Logger.LogError(ep.Message);
                throw ep;
            }
        }

       static bool ValidateUserInOnlineMode(string username,string password)
        {
            bool result = false;
            try
            {
                int serverKey = CheckUserInOnlineDB(username, password);

                if (serverKey == -99) // user not found in online DB.
                {
                    result = true;
                }
                else if (serverKey == _ID) // online and offline DB has same user with same IDs.
                {
                    result = true;
                }
                else if (serverKey != _ID) // online and offline DB has same user with different IDs, update the ServerKey of offline DB with ID_Employee of online Key.
                {
                    // Update Server Key of Offline DB.
                    using (CEConn localDB = new CEConn())
                    {
                        string strSql;
                        strSql = "Update Employees Set ServerKey = " + serverKey ;
                        strSql += " where Employees.username='" + username + "' and Employees.password='" + password + "'";

                        int output = localDB.runQuery(strSql);

                        if (output > 0)
                        {
                            _ID = serverKey;
                            result = true;
                        }
                    }

                }

                // Validate offline user record with online user record 
                // if also exist in online db, check and update the serverkey field of offline user record with ID_employee
                // field of online db.
            }
            catch (Exception ex)
            {
                Logger.LogError("Error while validating the offline login user with Online database : " + ex.Message);
            }
            return result;
        }

       public static int CheckUserInOnlineDB(string username, string password)
       {
           int ID_Employee = -99;
           try
           {
               webURL = urlStr + "Synchronise.asmx";
               webConURL = urlStr + "RConnection.asmx";
                            

                   RConnection.RConnection OnConn = new RConnection.RConnection();
                   OnConn.Url = Login.webConURL;
                   DataTable dt;
                   dt = (DataTable)OnConn.GetLogin(username, password);
                   if (dt.Rows.Count > 0)
                   {  
                      ID_Employee = Convert.ToInt32(dt.Rows[0]["ServerKey"]);                      
                   }
                 
                   //throw new ApplicationException("Online functionality not implemented yet."); 
                   //Do the Online Coding.
               

           }
           catch (System.Web.Services.Protocols.SoapException ex)
           {
               Logger.LogError("Error in CheckUserInOnlineDB method : " + ex.Message);
               ID_Employee = -99;
              // return false;
           }
           catch (Exception ep)
           {
               Logger.LogError("Error in CheckUserInOnlineDB method : " + ep.Message);
              // return false;
               ID_Employee = -99;
           }
           return ID_Employee;
       }

       public static DataTable getMenuOptionTable()
        {
            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = " select M.ID_MenuOptions,M.Name from Authority A ";
                    strSql += " inner join Menu_Options M on M.ServerKey=A.ID_MenuOptions ";
                    strSql += " where A.ID_SecurityGroups=" + _RoleId;

                    DataTable dtOptions = new DataTable("MenuOptions");
                    localDB.FillDataTable(ref dtOptions, strSql);
                    return dtOptions;
                }
            }
            else
            {
                RConnection.RConnection OnConn = new RConnection.RConnection();
                OnConn.Url = Login.webConURL;
                string strSql;
                strSql = " select M.ID_MenuOptions,M.Name from Authority A ";
                strSql += " inner join Menu_Options M on M.ID_MenuOptions=A.ID_MenuOptions ";
                strSql += " where A.ID_SecurityGroups=" + _RoleId;

                DataTable dt;
                dt = (DataTable)OnConn.getDataTable(strSql);
                return dt;

                //throw new ApplicationException("Online functionality not implemented yet."); 
            }
        }

    }

    public static class Util
    {
        public static DateTime getLastSyncedDate(String tblName)
        {
            using (CEConn localDB = new CEConn())
            {
                string strSql;
                strSql = " select min(Date_Modified) as SyncedDate from " + tblName;
                Object o;
                o = localDB.getScalerValue(strSql);
                if (DBNull.Value == o)
                    return DateTime.Now.AddYears(-10);
                else
                    return Convert.ToDateTime(o);
            }
        }

        public static int getRecordCount(String tblName)
        {
            using (CEConn localDB = new CEConn())
            {
                string strSql;
                strSql = " select count(*) as TotalRows from " + tblName;
                Object o;
                o = localDB.getScalerValue(strSql);
                if (DBNull.Value == o)
                    return 0;
                else
                    return Convert.ToInt32(o);
            }
        }

        public static string updateTables(String tblName, DataRow dRow, DateTime modifyDate)
        {
            String errStr = "";
            String displayName = "";
            try
            {
                string strSql;
                string modiBy;
                using (CEConn localDB = new CEConn())
                {
                    strSql = "delete from " + tblName + " where serverKey=" + dRow["ServerKey"];
                    localDB.runQuery(strSql);

                    if ((dRow["Is_Deleted"] == DBNull.Value) || ((Convert.ToInt32(dRow["Is_Deleted"]) == 0) && (Convert.ToInt32(dRow["Is_Active"]) == 1)))
                    {
                        //if (dRow["Date_Modified"] != DBNull.Value)
                            //modifyDate = Convert.ToDateTime(dRow["Date_Modified"]);

                        if (dRow["ModifiedBy"] != DBNull.Value)
                            modiBy = Convert.ToString(dRow["ModifiedBy"]);
                        else
                            modiBy = "0";

                        //strSql += "values('" + dRow["Name"].ToString().Replace("'","''") + "'," + dRow["ParentID"] + ",'" + modifyDate.ToString("yyyy-MM-dd HH:MM:ss") + "'," + dRow["ServerKey"] + "," + Convert.ToInt32(RowStatus.Synchronized) + "," + modiBy + ")";
                        switch (tblName.ToLower())
                        {
                             case "asset_groups":
                                strSql = "insert into Asset_Groups (Name,ParentID,Date_Modified,ServerKey,RowStatus,ModifiedBy) ";
                                strSql += "values('" + dRow["Name"].ToString().Replace("'","''") + "'," + dRow["ParentID"] + ",'" + modifyDate.ToString("yyyy-MM-dd HH:MM:ss") + "'," + dRow["ServerKey"] + "," + Convert.ToInt32(RowStatus.Synchronized) + "," + modiBy + ")";
                                displayName = dRow["Name"].ToString();
                                break;
                            //case "asset_status":
                            //    strSql = "insert into Asset_Status(Status,Date_Modified,ServerKey,RowStatus,ModifiedBy)";
                            //    strSql += "values("+ dRow["Status"] +",'"+ modifyDate.ToString("yyyy-MM-dd HH:MM:ss") +"',"+ dRow["ID_AssetStatus"]+","+ Convert.ToInt32(RowStatus.Synchronized)+ ","+ modiBy+")";
                            //    break;
                            //case "asset_types":
                            //    strSql = "insert into Asset_Types(Type,Date_Modified,ServerKey,RowStatus,ModifiedBy)";
                            //    strSql += "values(" + dRow["Type"] +",'"+ modifyDate.ToString("yyyy-MM-dd HH:MM:ss")+"',"+ dRow["ServerKey"] +","+ Convert.ToInt32(RowStatus.Synchronized)+ ","+ modiBy+")";
                            //    break;
                            case "assets":
                                strSql = " insert into Assets(TagID,LotNo,Name,AssetNo,ID_Employee,Date_Modified,ID_Location,ID_AssetGroup,ModifiedBy,RowStatus,ServerKey,ReferenceNo,GroupIDs)";
                                strSql += " values('" + dRow["TagID"] + "','" + dRow["LotNo"].ToString().Replace("'", "''") + "','" + dRow["Name"].ToString().Replace("'", "''") + "','" + dRow["AssetNo"] + "'," + dRow["ID_Employee"] + ",'" + modifyDate.ToString("yyyy-MM-dd HH:MM:ss") + "'," + dRow["ID_Location"] + "," + dRow["ID_AssetGroup"] + "," + modiBy + "," + Convert.ToInt32(RowStatus.Synchronized) + "," + dRow["serverKey"] + ",'" + dRow["ReferenceNo"] + "','" + dRow["GroupIDs"] + "')";
                                displayName = dRow["Name"].ToString();
                                break;
                            case "authority":
                                strSql = "insert into Authority(ID_MenuOptions,ID_SecurityGroups,Date_Modified,ServerKey,RowStatus,ModifiedBy)";
                                strSql += "values(" + Convert.ToInt32(dRow["ID_MenuOptions"]) + "," + Convert.ToInt32(dRow["ID_SecurityGroups"]) + ",'" + modifyDate.ToString("yyyy-MM-dd HH:MM:ss") + "'," + Convert.ToInt32(dRow["ServerKey"]) + "," + Convert.ToInt32(RowStatus.Synchronized) + "," + modiBy + ")";
                                break;
                            case "employees":
                                strSql = " insert into Employees(TagID,UserName,EmpNo,Password,Date_Modified,ID_SecurityGroup,ModifiedBy,RowStatus,serverKey)";
                                strSql += " values('" + dRow["TagID"] + "','" + dRow["UserName"] + "','" + dRow["EmpNo"] + "','" + dRow["Password"] + "','" + modifyDate.ToString("yyyy-MM-dd HH:MM:ss") + "'," + dRow["ID_SecurityGroup"] + "," + modiBy + "," + Convert.ToInt32(RowStatus.Synchronized) + "," + dRow["ServerKey"] + ")";
                                displayName = dRow["UserName"].ToString();
                                break;
                            case "locations":
                                strSql = " insert into Locations(TagID,Name,LocationNo,Date_Modified,ModifiedBy,RowStatus,serverKey)";
                                strSql += " values('" + dRow["TagID"] + "','" + dRow["Name"].ToString().Replace("'","''") + "','" + dRow["LocationNo"] + "','" + modifyDate.ToString("yyyy-MM-dd HH:MM:ss") + "'," + modiBy + "," + Convert.ToInt32(RowStatus.Synchronized) + "," + dRow["ServerKey"] + ")";
                                displayName = dRow["Name"].ToString();
                                break;
                            case "master_securitygroups":
                                strSql = "insert into Master_SecurityGroups(Name,Date_Modified,ServerKey,ModifiedBy,RowStatus)";
                                strSql += "values('" + dRow["Name"].ToString().Replace("'","''") + "','" + modifyDate.ToString("yyyy-MM-dd HH:mm:ss") + "'," + dRow["ServerKey"] + "," + modiBy + "," + Convert.ToInt32(RowStatus.Synchronized) + ")";
                                displayName = dRow["Name"].ToString();
                                break;
                            case "menu_options":
                                strSql = "insert into Menu_Options(Name,Date_Modified,ServerKey,ModifiedBy,RowStatus)";
                                strSql += "values('" + dRow["Name"].ToString().Replace("'","''") + "','" + modifyDate.ToString("yyyy-MM-dd HH:MM:ss") + "'," + dRow["ServerKey"] + "," + modiBy + "," + Convert.ToInt32(RowStatus.Synchronized) + ")";
                                displayName = dRow["Name"].ToString();
                                break;
                            case "reasons":
                                strSql = "insert into Reasons(ReasonNo,Name,Date_Modified,ServerKey,ModifiedBy,RowStatus)";
                                strSql += "values('" + dRow["ReasonNo"] + "','" + dRow["Name"].ToString().Replace("'","''").ToString().Replace("'","''")   + "','" + modifyDate.ToString("yyyy-MM-dd HH:mm:ss") + "'," + dRow["ServerKey"] + "," + modiBy + "," + Convert.ToInt32(RowStatus.Synchronized) + ")";
                                displayName = dRow["Name"].ToString();
                                break;
                            case "fieldservice":
                                strSql = "insert into FieldService(Title,ID_Employee,ID_Location,Date_Modified,ServerKey,ModifiedBy,RowStatus)";
                                strSql += "values('" + dRow["Title"] + "'," + dRow["ID_Employee"] + "," + dRow["ID_Location"] + ",'" + modifyDate.ToString("yyyy-MM-dd HH:MM:ss") + "'," + dRow["ServerKey"] + "," + modiBy + "," + Convert.ToInt32(RowStatus.Synchronized) + ")";
                                displayName = dRow["Title"].ToString();
                                break;
                            case "tasks":
                                strSql = "insert into Tasks(Title,Date_Modified,ServerKey,ModifiedBy,RowStatus)";
                                strSql += "values('" + dRow["Title"] + "','" + modifyDate.ToString("yyyy-MM-dd HH:MM:ss") + "'," + dRow["ServerKey"] + "," + modiBy + "," + Convert.ToInt32(RowStatus.Synchronized) + ")";
                                displayName = dRow["Title"].ToString();
                                break;
                            case "fs_templates":
                                strSql = "insert into FS_Templates(Title,ID_TemplateType,Frequency,Date_Modified,ServerKey,ModifiedBy,RowStatus)";
                                strSql += "values('" + dRow["Title"] + "'," + dRow["ID_TemplateType"] + "," + dRow["Frequency"] + ",'" + modifyDate.ToString("yyyy-MM-dd HH:MM:ss") + "'," + dRow["ServerKey"] + "," + modiBy + "," + Convert.ToInt32(RowStatus.Synchronized) + ")";
                                displayName = dRow["Title"].ToString();
                                break;
                        }

                        localDB.runQuery(strSql);
                    }
                }
            }
            catch (SqlCeException)
            {
                if (displayName.Length != 0)
                    errStr = tblName + " " + displayName + " not synchronized\r";
            }
            catch (Exception ep)
            {
                throw ep;
            }
            return errStr;
        }
           
      
    }

   

    
}
