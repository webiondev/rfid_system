using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using ClsLibBKLogs;

namespace ClsRampdb
{
    public class Assets
    {
        #region "Variable and Properties"
        public string PartCode { get; set; }

        Int32 _ServerKey;
        String _TagID;
        public String Name { get; set; }
        public String AssetNo { get; set; }
        public Int32 ID_Employee { get; set; }
        Int32 _ID_Location;
        Int32 _RowStatus;
        public Int32 ID_AssetGroup { get; set; }
        public DateTime Date_Modified { get; set; }
        String _LotNo = "";

        public String ReferenceNo { get; set; }
        public string Description { get; set; }

        public Int32 ServerKey
        {
            get { return _ServerKey; }
            set { _ServerKey = value; }
        }

        public String TagID
        {
            get { return _TagID; }
            set { _TagID = value; }
        }

        //public String Name
        //{
        //    get { return _Name; }
        //}

        //public String AssetNo
        //{
        //    get { return _AssetNo; }
        //}

        //public Int32 ID_Employee
        //{
        //    get { return _ID_Employee; }
        //}

        //public Int32 ID_AssetGroup
        //{
        //    get { return _ID_AssetGroup; }
        //}

        //public DateTime Date_Modified
        //{
        //    get { return _Date_Modified;}
        //}

        public Int32 ID_Location
        {
            get
            {
                return _ID_Location;
            }
            set
            {
                _ID_Location = value;
            }
        }

        public RowStatus Status
        {
            get
            {
                return (RowStatus)_RowStatus;
            }
            set
            {
                _RowStatus = Convert.ToInt32(value);
            }

        }

        public String LotNo
        {
            get { return _LotNo; }
            set { _LotNo = value; }
        }

        #endregion
        public Assets()
        {
        }
        public Assets(String TagID)
        {
            this.TagID = TagID;
            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = " select * from Assets where TAGID='" + TagID + "'";
                    SqlCeDataReader dr;
                    dr = localDB.getReader(strSql);
                    while (dr.Read())
                    {
                        ServerKey = Convert.ToInt32(dr["ServerKey"]);
                        if (dr["Name"] != DBNull.Value)
                            Name = (String)dr["Name"];
                        if (dr["AssetNo"] != DBNull.Value)
                            AssetNo = (String)dr["AssetNo"];
                        ID_Employee = Convert.ToInt32(dr["ID_Employee"]);
                        if (dr["ID_Location"] != DBNull.Value)
                            ID_Location = Convert.ToInt32(dr["ID_Location"]);
                        if (dr["ID_AssetGroup"] != DBNull.Value)
                            ID_AssetGroup = Convert.ToInt32(dr["ID_AssetGroup"]);
                        else
                            ID_AssetGroup = 0;

                        if (dr["Date_Modified"] != DBNull.Value)
                            Date_Modified = Convert.ToDateTime(dr["Date_Modified"]);
                        else
                            Date_Modified = DateTime.Now;

                        if (dr["LotNo"] != DBNull.Value)
                            _LotNo = (String)dr["LotNo"];

                        if (dr["ReferenceNo"] != DBNull.Value)
                            ReferenceNo = (String)dr["ReferenceNo"];

                        if (dr["Description"] != DBNull.Value)
                            Description = (string)dr["Description"];
                        else
                            Description = "";


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
                strSql = " select ID_Asset,Name,LotNo,AssetNo,ID_Employee,ID_Location,ID_AssetGroup,Date_Modified,ReferenceNo,Description from Assets where TAGID='" + TagID + "' and Is_Deleted=0 and IS_Active=1 ";

                DataTable dt;
                dt = (DataTable)OnConn.getDataTable(strSql);
                if (dt.Rows.Count != 0)
                {
                    ServerKey = Convert.ToInt32(dt.Rows[0]["ID_Asset"]);
                    if (dt.Rows[0]["Name"] != DBNull.Value)
                        Name = (String)dt.Rows[0]["Name"];

                    if (dt.Rows[0]["AssetNo"] != DBNull.Value)
                        AssetNo = (String)dt.Rows[0]["AssetNo"];
                    ID_Employee = Convert.ToInt32(dt.Rows[0]["ID_Employee"]);

                    if (dt.Rows[0]["ID_Location"] != DBNull.Value)
                        ID_Location = Convert.ToInt32(dt.Rows[0]["ID_Location"]);

                    if (dt.Rows[0]["ID_AssetGroup"] != DBNull.Value)
                        ID_AssetGroup = Convert.ToInt32(dt.Rows[0]["ID_AssetGroup"]);
                    else
                        ID_AssetGroup = 0;

                    if (dt.Rows[0]["Description"] != DBNull.Value)
                        Description = (string)dt.Rows[0]["Description"];
                    else
                        Description = "";

                    if (dt.Rows[0]["Date_Modified"] != DBNull.Value)
                        Date_Modified = Convert.ToDateTime(dt.Rows[0]["Date_Modified"]);
                    else
                        Date_Modified = DateTime.Now;

                    if (dt.Rows[0]["LotNo"] != DBNull.Value)
                        _LotNo = (String)dt.Rows[0]["LotNo"];

                    if (dt.Rows[0]["ReferenceNo"] != DBNull.Value)
                        ReferenceNo = (String)dt.Rows[0]["ReferenceNo"];

                    _RowStatus = Convert.ToInt32(RowStatus.Synchronized);
                }

                //throw new ApplicationException("Online functionality not implemented yet.");
            }
        }



        public static void AddAsset(String TagID, String AssetNo, String AssetName, Int32 LocationKey, string description, string ReferenceNo)
        {
            if (!Login.OnLineMode)
            {
                Int32 Skey;
                try
                {
                    Skey = minServerKey();

                    // added by vijay 08 Dec 2009
                    if (Skey == -2)
                        Skey = Skey - 1;

                    bool tagAssigned = false;

                    using (CEConn localDB = new CEConn())
                    {
                        if (Login.ItemLimit > 0)
                        {
                            string strSql;

                            TagInfo Ti = new TagInfo(TagID);
                            if (Ti.isAssetTag() || Ti.isLocationTag() || Ti.isEmployeeTag())
                                tagAssigned = true;
                            else
                                tagAssigned = false;

                            //strSql = " select count(*) from Assets A,Employees E,Locations L where A.TagID='" + TagID + "' or E.TagID='" + TagID + "' or L.TagID='" + TagID + "'";
                            ////select count(*) from Locations where TagID='" + TagID + "'";
                            if (tagAssigned == true)
                                throw new ApplicationException("Duplicate Tag ID.");

                            strSql = " insert into Assets(TagID,Name,AssetNo,ID_Employee,Date_Modified,ID_Location,ModifiedBy,RowStatus,ServerKey,Description,ReferenceNo)";
                            strSql += " values('" + TagID + "','" + AssetName.Replace("'", "''") + "','" + AssetNo.Replace("'", "''") + "'," + Login.ID + ",getDate()," + LocationKey + "," + Login.ID + "," + Convert.ToInt32(RowStatus.New) + "," + Skey + ",'" + description + "','" + ReferenceNo + "')";
                            localDB.runQuery(strSql);
                            Login._ItemLimit = Login.ItemLimit - 1;
                        }
                        else
                        {
                            throw new ApplicationException("Operation Suspended, the no. of items exceeds the limit.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("Add Items (offline mode)" + ex.Message);
                    throw ex;
                }
            }
            else
            {
                try
                {
                    DataTable dtAsset = new DataTable("dtAsset");
                    dtAsset.Columns.Add("TagID", typeof(String));
                    dtAsset.Columns.Add("Name", typeof(String));
                    dtAsset.Columns.Add("AssetNo", typeof(String));
                    dtAsset.Columns.Add("ID_Employee", typeof(Int32));
                    dtAsset.Columns.Add("ID_Location", typeof(Int32));
                    dtAsset.Columns.Add("Date_Modified", typeof(DateTime));
                    dtAsset.Columns.Add("ServerKey", typeof(Int32));
                    dtAsset.Columns.Add("ModifiedBy", typeof(Int32));
                    dtAsset.Columns.Add("Description", typeof(String));
                    dtAsset.Columns.Add("ReferenceNo", typeof(String));
                    dtAsset.AcceptChanges();

                    DataRow dr;
                    dr = dtAsset.NewRow();
                    dr["TagID"] = TagID;
                    dr["Name"] = AssetName;
                    dr["AssetNo"] = AssetName;
                    dr["ID_Employee"] = Login.ID;
                    dr["ID_Location"] = LocationKey;
                    dr["Date_Modified"] = DateTime.Now;
                    dr["ServerKey"] = -1;
                    dr["ModifiedBy"] = Login.ID;
                    dr["Description"] = description;
                    dr["ReferenceNo"] = ReferenceNo;

                    dtAsset.Rows.Add(dr);
                    dtAsset.AcceptChanges();

                    Synchronize.Synchronise OnConn = new Synchronize.Synchronise();
                    OnConn.Url = Login.webURL;
                    DataTable dtResult;
                    dtResult = OnConn.AddAssets(dtAsset);

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

                }
                catch (Exception ex)
                {
                    Logger.LogError("Add Items (online mode)" + ex.Message);
                }
                //string strSql;
                //strSql = " select Name as Name from Locations where ID_Location=" + _ID_Location;
                //return Convert.ToString(OnConn.getScalerValue(strSql));

                //throw new ApplicationException("Online functionality not implemented yet.");
            }
        }

        public static void EditAsset(String TagID, String AssetNo, String AssetName, Int32 LocationKey, Int32 ServerKey, string description, string ReferenceNo)
        {
            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    if (Login.ItemLimit > 0)
                    {
                        string strSql;

                        //strSql = " select count(*) from Assets A,Employees E,Locations L where A.TagID='" + TagID + "' or E.TagID='" + TagID + "' or L.TagID='" + TagID + "'";
                        ////select count(*) from Locations where TagID='" + TagID + "'";
                        //if (Convert.ToInt32(localDB.getScalerValue(strSql)) > 0)
                        //    throw new ApplicationException("Duplicate Tag ID.");

                        strSql = "Update Assets Set Name = '" + AssetName.Replace("'", "''") + "'" + ",AssetNo = '" + AssetNo.Replace("'", "''") + "'" + ",ID_Location = " + LocationKey + ",Description = '" + description + "',ReferenceNo = '" + ReferenceNo + "',Date_Modified = getDate(),";
                        strSql += "ModifiedBy = " + Login.ID + ",RowStatus = " + Convert.ToInt32(RowStatus.Modify);
                        strSql += " Where ServerKey = " + ServerKey;

                        localDB.runQuery(strSql);
                        Login._ItemLimit = Login.ItemLimit - 1;
                    }
                    else
                    {
                        throw new ApplicationException("Operation Suspended, the no. of items exceeds the limit.");
                    }
                }
            }
            else
            {
                string strSql;
                strSql = "Update Assets Set Name = '" + AssetName.Replace("'", "''") + "'" + ",AssetNo = '" + AssetNo.Replace("'", "''") + "'" + ",ID_Location = " + LocationKey + ",Description = '" +description + "',ReferenceNo = '" + ReferenceNo + "',Date_Modified = getDate(),";
                strSql += "Last_Modified_By = " + Login.ID;
                strSql += " Where ID_Asset = " + ServerKey;

                RConnection.RConnection OnConn = new RConnection.RConnection();
                OnConn.Url = Login.webConURL;
                OnConn.runQuery(strSql);
            }
        }

        public static void DeleteAsset(Int32 ServerKey)
        {
            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    if (Login.ItemLimit > 0)
                    {
                        string strSql;

                      
                        strSql = "Delete from Assets ";
                        strSql += " Where ServerKey = " + ServerKey;

                        localDB.runQuery(strSql);
                        Login._ItemLimit = Login.ItemLimit - 1;
                    }
                    else
                    {
                        throw new ApplicationException("Operation Suspended, the no. of items exceeds the limit.");
                    }
                }
            }
            else
            {
                string strSql;
                strSql = "Delete from Assets ";
                strSql += " Where ID_Asset = " + ServerKey;

                RConnection.RConnection OnConn = new RConnection.RConnection();
                OnConn.Url = Login.webConURL;
                OnConn.runQuery(strSql);
            }
        }


        public static Int32 getNoOfItems()
        {
            Int32 itemlimit = 0;
            using (CEConn localDB = new CEConn())
            {
                string strSql;
                strSql = " select count(*) from Assets ";
                DataTable dtList = new DataTable();
                itemlimit = Convert.ToInt32(localDB.getScalerValue(strSql));
            }
            return itemlimit;
        }

        public String getLocationName()
        {
            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    String strSql;
                    strSql = " select Name as Name from Locations where serverKey=" + _ID_Location;
                    return Convert.ToString(localDB.getScalerValue(strSql));
                }
            }
            else
            {
                RConnection.RConnection OnConn = new RConnection.RConnection();
                OnConn.Url = Login.webConURL;
                string strSql;
                strSql = " select Name as Name from Locations where ID_Location=" + _ID_Location;
                return Convert.ToString(OnConn.getScalerValue(strSql));
            }
        }

        /// <summary>
        /// Retrun List of assets, by retriving data from TagIDs
        /// </summary>
        /// <param name="TagIDs">Accepts Comma seperated Tag IDs</param>
        /// <returns>Array of Assets</returns>
        public static List<Assets> GetAssetsByTagID(String TagIDs)
        {
            List<Assets> lstAsset = new List<Assets>();
            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = " select * from Assets where TAGID in ('" + TagIDs.Replace(",", "','") + "')";
                    SqlCeDataReader dr;
                    dr = localDB.getReader(strSql);
                    while (dr.Read())
                    {
                        try
                        {
                            Assets t = new Assets();
                            t.TagID = Convert.ToString(dr["TagID"]);
                            t.ServerKey = Convert.ToInt32(dr["ServerKey"]);
                            if (dr["Name"] != DBNull.Value)
                                t.Name = (String)dr["Name"];
                            if (dr["AssetNo"] != DBNull.Value)
                                t.AssetNo = (String)dr["AssetNo"];
                            t.ID_Employee = Convert.ToInt32(dr["ID_Employee"]);
                            if (dr["ID_Location"] != DBNull.Value)
                                t.ID_Location = Convert.ToInt32(dr["ID_Location"]);

                            if (dr["ID_AssetGroup"] != DBNull.Value)
                                t.ID_AssetGroup = Convert.ToInt32(dr["ID_AssetGroup"]);
                            else
                                t.ID_AssetGroup = 0;

                            if (dr["Date_Modified"] != DBNull.Value)
                                t.Date_Modified = Convert.ToDateTime(dr["Date_Modified"]);
                            else
                                t.Date_Modified = DateTime.Now;

                            if (dr["LotNo"] != DBNull.Value)
                                t.LotNo = (String)dr["LotNo"];

                            if (dr["ReferenceNo"] != DBNull.Value)
                                t.ReferenceNo = (String)dr["ReferenceNo"];

                            if (dr["Description"] != DBNull.Value)
                                t.Description = (string)dr["Description"];
                            else
                                t.Description = "";

                            t.Status = (RowStatus)Convert.ToInt32(dr["RowStatus"]);

                            lstAsset.Add(t);
                        }
                        catch { }
                    }
                    dr.Close();
                }
            }
            else
            {
                RConnection.RConnection OnConn = new RConnection.RConnection();
                OnConn.Url = Login.webConURL;
                string strSql;
                strSql = " select TagID,ID_Asset,Name,LotNo,AssetNo,ID_Employee,ID_Location,ID_AssetGroup,Date_Modified,ReferenceNo,Description from Assets where TAGID in ('" + TagIDs.Replace(",", "','") + "') and Is_Deleted=0 and IS_Active=1 ";

                DataTable dt;
                dt = (DataTable)OnConn.getDataTable(strSql);
                if (dt.Rows.Count >= 0)
                {
                    for (Int32 ic = 0; ic < dt.Rows.Count; ic++)
                    {
                        try
                        {
                            Assets t = new Assets();
                            t.TagID = Convert.ToString(dt.Rows[ic]["TagID"]);
                            t.ServerKey = Convert.ToInt32(dt.Rows[ic]["ID_Asset"]);
                            if (dt.Rows[ic]["Name"] != DBNull.Value)
                                t.Name = (String)dt.Rows[ic]["Name"];

                            if (dt.Rows[ic]["AssetNo"] != DBNull.Value)
                                t.AssetNo = (String)dt.Rows[ic]["AssetNo"];
                            t.ID_Employee = Convert.ToInt32(dt.Rows[ic]["ID_Employee"]);

                            if (dt.Rows[ic]["ID_Location"] != DBNull.Value)
                                t.ID_Location = Convert.ToInt32(dt.Rows[ic]["ID_Location"]);

                            if (dt.Rows[ic]["ID_AssetGroup"] != DBNull.Value)
                                t.ID_AssetGroup = Convert.ToInt32(dt.Rows[ic]["ID_AssetGroup"]);
                            else
                                t.ID_AssetGroup = 0;

                            if (dt.Rows[ic]["Date_Modified"] != DBNull.Value)
                                t.Date_Modified = Convert.ToDateTime(dt.Rows[ic]["Date_Modified"]);
                            else
                                t.Date_Modified = DateTime.Now;

                            if (dt.Rows[ic]["LotNo"] != DBNull.Value)
                                t.LotNo = (String)dt.Rows[ic]["LotNo"];

                            if (dt.Rows[ic]["ReferenceNo"] != DBNull.Value)
                                t.ReferenceNo = (String)dt.Rows[ic]["ReferenceNo"];

                            if (dt.Rows[ic]["Description"] != DBNull.Value)
                                t.Description = (string)dt.Rows[ic]["Description"];
                            else
                                t.Description = "";

                            t.Status = (RowStatus)Convert.ToInt32(RowStatus.Synchronized);

                            lstAsset.Add(t);
                        }
                        catch { }
                    }
                }

                //throw new ApplicationException("Online functionality not implemented yet.");
            }
            return lstAsset;

        }

        /// <summary>
        /// Retrun List of assets, by retriving data from TagIDs
        /// </summary>
        /// <param name="TagIDs">Accepts Comma seperated Tag IDs</param>
        /// <returns>Array of Assets</returns>
        public static List<Assets> GetAssets_ByTagID(String TagIDs, int Capacity)
        {
            List<Assets> lstAsset = new List<Assets>(Capacity + 1);
            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = " select distinct TagID,ServerKey,Name,AssetNo,ID_Employee,ID_Location,ID_AssetGroup,Date_Modified,LotNo,RowStatus,ReferenceNo,Description from Assets where TAGID in (" + TagIDs + ")";
                    SqlCeDataReader dr;
                    dr = localDB.getReader(strSql);
                    while (dr.Read())
                    {
                        try
                        {
                            Assets t = new Assets();
                            t.TagID = Convert.ToString(dr["TagID"]);
                            t.ServerKey = Convert.ToInt32(dr["ServerKey"]);
                            if (dr["Name"] != DBNull.Value)
                                t.Name = (String)dr["Name"];
                            if (dr["AssetNo"] != DBNull.Value)
                                t.AssetNo = (String)dr["AssetNo"];
                            t.ID_Employee = Convert.ToInt32(dr["ID_Employee"]);
                            if (dr["ID_Location"] != DBNull.Value)
                                t.ID_Location = Convert.ToInt32(dr["ID_Location"]);

                            if (dr["ID_AssetGroup"] != DBNull.Value)
                                t.ID_AssetGroup = Convert.ToInt32(dr["ID_AssetGroup"]);
                            else
                                t.ID_AssetGroup = 0;

                            if (dr["Date_Modified"] != DBNull.Value)
                                t.Date_Modified = Convert.ToDateTime(dr["Date_Modified"]);
                            else
                                t.Date_Modified = DateTime.Now;

                            if (dr["LotNo"] != DBNull.Value)
                                t.LotNo = (String)dr["LotNo"];

                            if (dr["ReferenceNo"] != DBNull.Value)
                                t.ReferenceNo = (String)dr["ReferenceNo"];

                            if (dr["Description"] != DBNull.Value)
                                t.Description = (string)dr["Description"];
                            else
                                t.Description = "";

                            t.Status = (RowStatus)Convert.ToInt32(dr["RowStatus"]);

                            lstAsset.Add(t);
                        }
                        catch { }
                    }
                    dr.Close();
                }
            }
            else
            {
                RConnection.RConnection OnConn = new RConnection.RConnection();
                OnConn.Url = Login.webConURL;
                string strSql;
                strSql = " select distinct TagID,ID_Asset,Name,LotNo,AssetNo,ID_Employee,ID_Location,ID_AssetGroup,Date_Modified,ReferenceNo,Description from Assets where TAGID in (" + TagIDs + ") and Is_Deleted=0 and IS_Active=1 ";

                DataTable dt;
                dt = (DataTable)OnConn.getDataTable(strSql);
                if (dt.Rows.Count >= 0)
                {
                    for (Int32 ic = 0; ic < dt.Rows.Count; ic++)
                    {
                        try
                        {
                            Assets t = new Assets();
                            t.TagID = Convert.ToString(dt.Rows[ic]["TagID"]);
                            t.ServerKey = Convert.ToInt32(dt.Rows[ic]["ID_Asset"]);
                            if (dt.Rows[ic]["Name"] != DBNull.Value)
                                t.Name = (String)dt.Rows[ic]["Name"];

                            if (dt.Rows[ic]["AssetNo"] != DBNull.Value)
                                t.AssetNo = (String)dt.Rows[ic]["AssetNo"];


                            t.ID_Employee = Convert.ToInt32(dt.Rows[ic]["ID_Employee"]);

                            if (dt.Rows[ic]["ID_Location"] != DBNull.Value)
                                t.ID_Location = Convert.ToInt32(dt.Rows[ic]["ID_Location"]);

                            if (dt.Rows[ic]["ID_AssetGroup"] != DBNull.Value)
                                t.ID_AssetGroup = Convert.ToInt32(dt.Rows[ic]["ID_AssetGroup"]);
                            else
                                t.ID_AssetGroup = 0;

                            if (dt.Rows[ic]["Date_Modified"] != DBNull.Value)
                                t.Date_Modified = Convert.ToDateTime(dt.Rows[ic]["Date_Modified"]);
                            else
                                t.Date_Modified = DateTime.Now;

                            if (dt.Rows[ic]["LotNo"] != DBNull.Value)
                                t.LotNo = (String)dt.Rows[ic]["LotNo"];

                            if (dt.Rows[ic]["ReferenceNo"] != DBNull.Value)
                                t.ReferenceNo = (String)dt.Rows[ic]["ReferenceNo"];

                            if (dt.Rows[ic]["Description"] != DBNull.Value)
                                t.Description = (string)dt.Rows[ic]["Description"];
                            else
                                t.Description = "";

                            t.Status = (RowStatus)Convert.ToInt32(RowStatus.Synchronized);

                            lstAsset.Add(t);
                        }
                        catch { }
                    }
                }

                //throw new ApplicationException("Online functionality not implemented yet.");
            }
            return lstAsset;

        }


        /// <summary>
        /// Retrun List of assets, by retriving data from TagIDs
        /// </summary>
        /// <param name="TagIDs">Accepts Comma seperated Tag IDs</param>
        /// <returns>Array of Assets</returns>
        public static DataTable GetLotNoGroups(String TagIDs)
        {
            DataTable dtTotal;
            dtTotal = new DataTable();
            dtTotal.Columns.Add("LotNO", typeof(String));
            dtTotal.Columns.Add("Total", typeof(Int32));
            dtTotal.AcceptChanges();
            DataRow drTotal;

            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = " select distinct LotNo,count(*) as Total from Assets where TAGID in ('" + TagIDs.Replace(",", "','") + "') group by LotNo ";
                    SqlCeDataReader dr;
                    dr = localDB.getReader(strSql);
                    while (dr.Read())
                    {
                        drTotal = dtTotal.NewRow();
                        if (dr["LotNo"] != DBNull.Value)
                            drTotal["LotNo"] = (String)dr["LotNo"];
                        else
                            drTotal["LotNo"] = "NA";
                        drTotal["Total"] = Convert.ToInt32(dr["Total"]);
                        dtTotal.Rows.Add(drTotal);
                    }
                    dr.Close();
                }
            }
            else
            {
                RConnection.RConnection OnConn = new RConnection.RConnection();
                OnConn.Url = Login.webConURL;
                string strSql;
                strSql = " select distinct isNull(LotNo,'') as LotNo, count(*) as Total from Assets where TAGID in ('" + TagIDs.Replace(",", "','") + "') and Is_Deleted=0 and IS_Active=1 Group By LotNO ";

                DataTable dt;
                dt = (DataTable)OnConn.getDataTable(strSql);
                if (dt != null && dt.Rows.Count >= 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        drTotal = dtTotal.NewRow();
                        if (dr["LotNo"] != DBNull.Value)
                            drTotal["LotNo"] = (String)dr["LotNo"];
                        else
                            drTotal["LotNo"] = "NA";
                        drTotal["Total"] = Convert.ToInt32(dr["Total"]);
                        dtTotal.Rows.Add(drTotal);
                    }
                }
                dtTotal.AcceptChanges();
                //throw new ApplicationException("Online functionality not implemented yet.");
            }
            return dtTotal;

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
                    strSql = " update Assets set TagID='" + TagNo + "', Date_MOdified=getDate(), ModifiedBy=" + Login.ID + ", RowStatus=" + p + " where ServerKey=" + _ServerKey;
                    localDB.runQuery(strSql);

                    strSql = " update Inventory set TagID='" + TagNo + "' where TagID='" + _TagID + "'";
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
                strSql = " update Assets set TagID='" + TagNo + "', Date_MOdified=getDate(), Last_Modified_By=" + Login.ID + " where ID_Asset=" + _ServerKey;
                OnConn.runQuery(strSql);
                _TagID = TagNo;
                _RowStatus = p;
                //throw new ApplicationException("Online Asset functionality not implemented yet.");
            }
        }

        public void updateInventory(Int32 locID)
        {
            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = "select count(*) from Inventory where TagID='" + _TagID + "'";
                    if (Convert.ToInt32(localDB.getScalerValue(strSql)) > 0)
                    {
                        strSql = "update Inventory set ID_Location=" + locID + ", Date_Modified=getDate(), ModifiedBy=" + Login.ID + ", RowStatus=" + Convert.ToInt32(RowStatus.Inventory) + " where TagID='" + _TagID + "'";
                        localDB.runQuery(strSql);
                    }
                    else
                    {
                        strSql = "insert into Inventory(TagID,Id_location,Date_Modified,ModifiedBy,RowStatus,ServerKey) ";
                        strSql += " values ('" + _TagID + "'," + locID + ",getDate()," + Login.ID + "," + Convert.ToInt32(RowStatus.Inventory) + "," + _ServerKey + ")";
                        localDB.runQuery(strSql);
                    }
                    //_ID_Location = locID;
                }
            }
            else
            {
                //throw new ApplicationException("Online Inventory functionality not implemented yet.");
            }
        }

        public static int update_Inventory(string TagIDs, Int32 locID)
        {
            int result = 0;
            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    try
                    {
                        StringBuilder sqlstr = new StringBuilder();

                        sqlstr.Append("update Inventory Set ID_Location = " + locID + " where TagID IN (" + TagIDs + ");");

                        result = result + localDB.runQuery(sqlstr.ToString());

                        sqlstr = new StringBuilder("");

                        sqlstr.Append("Insert into Inventory (TagID,ID_Location,Date_Modified,ModifiedBy,RowStatus,ServerKey) ");
                        sqlstr.Append("Select TagID," + locID + ",getDate()," + Login.ID + "," + Convert.ToInt32(RowStatus.Inventory) + ",ServerKey from Assets where TagID IN (" + TagIDs + ")");
                        sqlstr.Append(" AND TagID NOT IN ( Select TagID from Inventory where TagID IN (" + TagIDs + ") );");

                        result = result + localDB.runQuery(sqlstr.ToString());
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex.Message);
                        throw ex;

                    }
                    //_ID_Location = locID;

                }
            }
            return result;

        }

        public static DataTable getSearchReasult(String AssetNo, String AssetName, Int32 locKey, Int32 GroupKey, Int32 subGroupKey, String LotNo, int option)
        {
            //return getSearchReasult(AssetNo, AssetName, locKey, GroupKey, subGroupKey, LotNo, "");
            if (option < 0)
                return getSearchReasult(AssetNo, AssetName, locKey, GroupKey, subGroupKey, LotNo, "");
            else
                return getSearchReasultNew(AssetNo, AssetName, locKey, GroupKey, subGroupKey, LotNo, "", option);
        }

        public static DataTable getSearchReasult(String AssetNo, String AssetName, Int32 locKey, Int32 GroupKey, Int32 subGroupKey, String LotNo, String TagID)
        {
            string strSql;
            if (Login.OnLineMode)
            {
                strSql = " select Top " + Login.ItemLimit.ToString() + " ID_Asset,Name,TagID,LotNo from Assets where 1=1 ";
            }
            else
            {
                strSql = " select ID_Asset,Name,TagID from Assets where 1=1 ";
            }

            if (TagID.Trim().Length != 0)
            {
                strSql += " and TagID = '" + TagID + "'";
            }

            if (AssetNo.Trim().Length != 0)
            {
                strSql += " and AssetNo like '%" + AssetNo + "%'";
            }

            if (AssetName.Trim().Length != 0)
            {
                strSql += " and Name like '%" + AssetName + "%'";
            }

            if (LotNo.Trim().Length != 0)
            {
                strSql += " and LotNo like '%" + LotNo + "%'";
            }

            if (locKey != -2 && locKey != 0)
            {
                strSql += " and ID_Location =" + locKey + " ";
            }

            if (!Login.OnLineMode)
            {
                if (subGroupKey != -2 && subGroupKey != 0)
                {
                    strSql += " and (ID_AssetGroup =" + subGroupKey + " or GroupIDs like '%," + subGroupKey + ",%' ) ";
                }
                else
                {
                    if (GroupKey != -2 && GroupKey != 0)
                    {
                        strSql += " and (ID_AssetGroup =" + GroupKey + " or GroupIDs like '%," + GroupKey + ",%' ) ";
                    }
                }

                strSql += " and RowStatus <>" + Convert.ToInt32(RowStatus.Dispatched) + " ";

                using (CEConn localDB = new CEConn())
                {
                    strSql += " order by Name";
                    DataTable dtList = new DataTable();
                    localDB.FillDataTable(ref dtList, strSql);
                    return dtList;
                }
            }
            else
            {
                if (subGroupKey != -2 && subGroupKey != 0)
                {
                    strSql += " and (ID_AssetGroup =" + subGroupKey + " or dbo.GetParentGroups(ID_AssetGroup) like '%," + subGroupKey + ",%' ) ";
                }
                else
                {
                    if (GroupKey != -2 && GroupKey != 0)
                    {
                        strSql += " and (ID_AssetGroup =" + GroupKey + " or dbo.GetParentGroups(ID_AssetGroup) like '%," + GroupKey + ",%' ) ";
                    }
                }

                strSql += " and Is_Active=1 and Is_Deleted=0 order by Name";
                RConnection.RConnection OnConn = new RConnection.RConnection();
                OnConn.Url = Login.webConURL;
                return OnConn.getDataTable(strSql);

            }
        }

        public static DataTable getSearchReasultNew(String AssetNo, String AssetName, Int32 locKey, Int32 GroupKey, Int32 subGroupKey, String LotNo, String TagID, int option)
        {
            string strSql;
            if (Login.OnLineMode)
            {
                strSql = " select Top " + Login.ItemLimit.ToString() + " ID_Asset,Name,TagID,LotNo from Assets where 1=1 ";
            }
            else
            {
                strSql = " select ID_Asset,Name,TagID from Assets where 1=1 ";
            }

            if (TagID.Trim().Length != 0)
            {
                strSql += " and TagID = '" + TagID + "'";
            }

            if (AssetNo.Trim().Length != 0)
            {
                // strSql += " and AssetNo like '%" + AssetNo + "%'";
                switch (option)
                {
                    case 1: // Contains any digit
                        //if (Login.OnLineMode)
                        //{
                        //   // strSql += " and AssetNo like '[" + AssetNo + "]%'";
                        //    string[] allWords = AssetNo.Split(' ');
                        //    int k = 0;
                        //    if (allWords != null && allWords.Length > 0)
                        //    {
                        //        strSql += " and ( ";
                        //        foreach (string str in allWords)
                        //        {
                        //            k++;
                        //            if (str.Trim() != "")
                        //            { 
                        //                // if str is numeric                                       
                        //                int i = 0;
                        //                if (isNumeric(str))
                        //                {
                        //                    strSql += " AssetNo like '[" + str.Trim() + "]%'"; 
                        //                }
                        //                else
                        //                {
                        //                    strSql += " AssetNo like '%" + str.Trim() + " %' OR (AssetNo Like '%" + str.Trim() + "' AND AssetNo Like '" + str.Trim() + "%')";
                        //                }

                        //                if (k < allWords.Length)
                        //                    strSql += " OR "; 
                        //            }
                        //        }
                        //        strSql += " )";
                        //    }                            

                        //}
                        // else 
                        // {
                        string[] allWords = AssetNo.Trim().Split(' ');
                        int k = 0;
                        if (allWords != null && allWords.Length > 0)
                        {
                            strSql += " and ( ";
                            foreach (string str in allWords)
                            {
                                k++;
                                if (str.Trim() != "")
                                {
                                    // if str is numeric                                       
                                    int i = 0;
                                    if (isNumeric(str))
                                    {
                                        for (i = 0; i < str.Length; i++)
                                        {
                                            strSql += " AssetNo like '%" + str[i] + "%'";
                                            if (i < str.Length - 1)
                                                strSql += " OR ";
                                        }
                                    }
                                    else
                                    {
                                        strSql += " AssetNo like '%" + str.Trim() + " %' OR (AssetNo Like '%" + str.Trim() + "' AND AssetNo Like '" + str.Trim() + "%')";
                                    }

                                    if (k < allWords.Length)
                                        strSql += " OR ";

                                }
                            }
                            strSql += " )";
                        }
                        else
                        {
                            strSql += " and AssetNo like '%" + AssetNo.Trim() + "%'";
                        }
                        // }
                        //else
                        //{
                        //    strSql += " and AssetNo like '%" + AssetNo + "%'";
                        //}
                        break;
                    case 2: // Contains whole string 
                        strSql += " and AssetNo like '%" + AssetNo.Trim() + "%'";
                        break;
                    case 3: // Search Exact
                        strSql += " and AssetNo = '" + AssetNo.Trim() + "'";
                        break;
                    default:
                        strSql += " and AssetNo like '%" + AssetNo.Trim() + "%'";
                        break;
                }

            }

            if (AssetName.Trim().Length != 0)
            {
                switch (option)
                {
                    case 1: // Contains any Word
                        {
                            string[] allWords = AssetName.Trim().Split(' ');
                            int i = 0;
                            if (allWords != null && allWords.Length > 1)
                            {
                                strSql += " and ( ";
                                foreach (string str in allWords)
                                {
                                    i++;
                                    if (str.Trim() != "")
                                    {
                                        strSql += " Name like '%" + str.Trim() + " %' OR (Name Like '%" + str.Trim() + "' AND Name Like '" + str.Trim() + "%')";
                                        if (i < allWords.Length)
                                        {
                                            strSql += " OR ";
                                        }
                                    }
                                }
                                strSql += " )";
                            }
                            else
                                strSql += " and Name like '%" + AssetName.Trim() + " %'";
                        }
                        break;
                    case 2: // Contains whole string 
                        strSql += " and Name like '%" + AssetName.Trim() + "%'";
                        break;
                    case 3: // Search Exact
                        strSql += " and Name = '" + AssetName.Trim() + "'";
                        break;
                    default:
                        strSql += " and Name like '%" + AssetName.Trim() + "%'";
                        break;

                }
                // strSql += " and Name like '%" + AssetName + "%'";
            }

            if (LotNo.Trim().Length != 0)
            {
                strSql += " and LotNo like '%" + LotNo + "%'";
            }

            if (locKey != -2 && locKey != 0)
            {
                strSql += " and ID_Location =" + locKey + " ";
            }

            if (!Login.OnLineMode)
            {
                if (subGroupKey != -2 && subGroupKey != 0)
                {
                    strSql += " and (ID_AssetGroup =" + subGroupKey + " or GroupIDs like '%," + subGroupKey + ",%' ) ";
                }
                else
                {
                    if (GroupKey != -2 && GroupKey != 0)
                    {
                        strSql += " and (ID_AssetGroup =" + GroupKey + " or GroupIDs like '%," + GroupKey + ",%' ) ";
                    }
                }

                strSql += " and RowStatus <>" + Convert.ToInt32(RowStatus.Dispatched) + " ";

                using (CEConn localDB = new CEConn())
                {
                    strSql += " order by Name";
                    DataTable dtList = new DataTable();
                    localDB.FillDataTable(ref dtList, strSql);
                    return dtList;
                }
            }
            else
            {
                if (subGroupKey != -2 && subGroupKey != 0)
                {
                    strSql += " and (ID_AssetGroup =" + subGroupKey + " or dbo.GetParentGroups(ID_AssetGroup) like '%," + subGroupKey + ",%' ) ";
                }
                else
                {
                    if (GroupKey != -2 && GroupKey != 0)
                    {
                        strSql += " and (ID_AssetGroup =" + GroupKey + " or dbo.GetParentGroups(ID_AssetGroup) like '%," + GroupKey + ",%' ) ";
                    }
                }

                strSql += " and Is_Active=1 and Is_Deleted=0 order by Name";
                RConnection.RConnection OnConn = new RConnection.RConnection();
                OnConn.Url = Login.webConURL;
                return OnConn.getDataTable(strSql);

            }
        }

        public static DataTable getSearchReasultNew(Dictionary<string, string> columnsToSearch, Int32 locKey, Int32 GroupKey, Int32 subGroupKey, int option)
        {
            string strSql;
            if (Login.OnLineMode)
            {
                strSql = " select Top " + Login.ItemLimit.ToString() + " ID_Asset,Name,TagID,LotNo from Assets where 1=1 ";
            }
            else
            {
                strSql = " select ID_Asset,Name,TagID from Assets where 1=1 ";
            }

            string value = "";

            if (columnsToSearch.Count > 0)
            {
                foreach (string colName in columnsToSearch.Keys)
                {
                    value = columnsToSearch[colName];
                    if (value.Trim().Length != 0)
                    {
                        // strSql += " and AssetNo like '%" + AssetNo + "%'";
                        switch (option)
                        {
                            case 1:
                                string[] allWords = value.Trim().Split(' ');
                                int k = 0;
                                if (allWords != null && allWords.Length > 0)
                                {
                                    strSql += " and ( ";
                                    foreach (string str in allWords)
                                    {
                                        k++;
                                        if (str.Trim() != "")
                                        {
                                            // if str is numeric                                       
                                            //int i = 0;
                                            //if (isNumeric(str))
                                            //{
                                            //    for (i = 0; i < str.Length; i++)
                                            //    {
                                            //        strSql += " " + colName + " like '%" + str[i] + "%'";
                                            //        if (i < str.Length - 1)
                                            //            strSql += " OR ";
                                            //    }
                                            //}
                                            //else
                                            //{
                                            //    strSql += " " + colName + " like '%" + str.Trim() + " %' OR (" + colName + " Like '%" + str.Trim() + "' AND " + colName + " Like '" + str.Trim() + "%')";
                                            //}

                                            strSql += " " + colName + " like '%" + str.Trim() + "%' OR (" + colName + " Like '%" + str.Trim() + "' AND " + colName + " Like '" + str.Trim() + "%')";

                                            if (k < allWords.Length)
                                                strSql += " OR ";

                                        }
                                    }
                                    strSql += " )";
                                }
                                else
                                {
                                    strSql += " and " + colName + " like '%" + value.Trim() + "%'";
                                }
                                // }
                                //else
                                //{
                                //    strSql += " and AssetNo like '%" + AssetNo + "%'";
                                //}
                                break;
                            case 2: // Contains whole string 
                                strSql += " and " + colName + " like '%" + value.Trim() + "%'";
                                break;
                            case 3: // Search Exact
                                strSql += " and " + colName + " = '" + value.Trim() + "'";
                                break;
                            default:
                                strSql += " and " + colName + " like '%" + value.Trim() + "%'";
                                break;
                        }

                    }

                }
            }


            if (locKey != -2 && locKey != 0)
            {
                strSql += " and ID_Location =" + locKey + " ";
            }

            if (!Login.OnLineMode)
            {
                if (subGroupKey != -2 && subGroupKey != 0)
                {
                    strSql += " and (ID_AssetGroup =" + subGroupKey + " or GroupIDs like '%," + subGroupKey + ",%' ) ";
                }
                else
                {
                    if (GroupKey != -2 && GroupKey != 0)
                    {
                        strSql += " and (ID_AssetGroup =" + GroupKey + " or GroupIDs like '%," + GroupKey + ",%' ) ";
                    }
                }

                strSql += " and RowStatus <>" + Convert.ToInt32(RowStatus.Dispatched) + " ";

                using (CEConn localDB = new CEConn())
                {
                    strSql += " order by Name";
                    DataTable dtList = new DataTable();
                    localDB.FillDataTable(ref dtList, strSql);

                    try
                    {
                        if (locKey != -2 && locKey != 0)
                        {
                            strSql = " Update Locations Set UsageCount = UsageCount + 1,OfflineUsageCount = OfflineUsageCount + 1,RowStatus=" + Convert.ToInt32(RowStatus.Modify) + ", Date_MOdified=getDate(), ModifiedBy=" + Login.ID + " Where ServerKey =" + locKey;
                            localDB.runQuery(strSql);
                        }
                    }
                    catch
                    { }

                    return dtList;
                }
            }
            else
            {
                if (subGroupKey != -2 && subGroupKey != 0)
                {
                    strSql += " and (ID_AssetGroup =" + subGroupKey + " or dbo.GetParentGroups(ID_AssetGroup) like '%," + subGroupKey + ",%' ) ";
                }
                else
                {
                    if (GroupKey != -2 && GroupKey != 0)
                    {
                        strSql += " and (ID_AssetGroup =" + GroupKey + " or dbo.GetParentGroups(ID_AssetGroup) like '%," + GroupKey + ",%' ) ";
                    }
                }

                strSql += " and Is_Active=1 and Is_Deleted=0 order by Name; ";

                // 
                if (locKey != -2 && locKey != 0)
                {
                    strSql += " Update Locations Set UsageCount = UsageCount + 1, Date_MOdified=getDate(), Last_Modified_By=" + Login.ID + " Where ID_Location =" + locKey;
                }

                RConnection.RConnection OnConn = new RConnection.RConnection();
                OnConn.Url = Login.webConURL;
                return OnConn.getDataTable(strSql);

            }
        }

        private static bool isNumeric(string value)
        {
            // return System.Text.RegularExpressions.Regex.IsMatch(value, "[0-9]+");
            return System.Text.RegularExpressions.Regex.IsMatch(value, @"^\d+$");
        }

        private static Int32 minServerKey()
        {
            using (CEConn localDB = new CEConn())
            {
                string strSql;
                strSql = "select min(serverKey) from Assets";
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
                DataTable dtAssets = new DataTable("dtAssets");
                string strSql;


                strSql = "select * from Assets where RowStatus=" + Convert.ToInt32(RowStatus.New) + " OR ServerKey < 0";
                localDB.FillDataTable(ref dtAssets, strSql);

                //strSql = "update Assets set RowStatus=" + Convert.ToInt32(RowStatus.InProcess) + " where RowStatus=" + Convert.ToInt32(RowStatus.New);
                //localDB.runQuery(strSql);

                return dtAssets;
            }
        }

        public static DataTable getRows(RowStatus Stat)
        {
            using (CEConn localDB = new CEConn())
            {
                DataTable dtAssets = new DataTable("dtAssets");
                string strSql;

                if (Stat == RowStatus.Modify)
                {
                    strSql = "select * from Assets where (RowStatus=" + Convert.ToInt32(Stat) + " or RowStatus=" + Convert.ToInt32(RowStatus.Dispatched) + ")";
                }
                else
                {
                    strSql = "select * from Assets where RowStatus=" + Convert.ToInt32(Stat);
                }
                localDB.FillDataTable(ref dtAssets, strSql);

                return dtAssets;
            }
        }

        // Get Inventory New Rows
        public static DataTable getInventoryRows(RowStatus Stat)
        {
            using (CEConn localDB = new CEConn())
            {
                DataTable dtInventory = new DataTable("dtInventory");
                string strSql;

                strSql = "select TagID,ID_Location,ServerKey from Inventory where RowStatus=" + Convert.ToInt32(Stat);
                localDB.FillDataTable(ref dtInventory, strSql);


                return dtInventory;
            }

        }

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

        public static DataTable OnLineInventory(ref DataTable dtInventory)
        {
            DataTable dtResult = new DataTable("result");
            try
            {
                if (dtInventory.Rows.Count > 0)
                {
                    Synchronize.Synchronise syn;
                    syn = new Synchronize.Synchronise();
                    syn.Url = Login.webURL;
                    dtResult = syn.InventoryCheck(dtInventory);
                }
            }
            catch(Exception ex)
            {
                Logger.LogError("Error in syn.InventoryCheck. Error: "+ex.Message);
            }
            return dtResult;
        }

        public static int UpdateAssetLocation(string TagIDs, Int64 ID_Location, Int64 ID_Reason)
        {
            bool madeOfflineUpdate = false;
            string strSql;

            int result = 0;

            if (Login.OnLineMode)
            {
                //TagID = "'" + TagID + "'";

                try
                {
                    using (RConnection.RConnection OnConn = new RConnection.RConnection())
                    {
                        OnConn.Url = Login.webConURL;
                        // strSql = "exec Update_Asset_Location_by_HH '''" + TagID + "''' ," + ID_Location + ", " + Login.ID + " ,0,''," + ID_Reason;
                        // OnConn.runQuery(strSql);

                        // updated by vijay on 13 Nov 09 for batch processing 

                        ArrayList keys = new ArrayList();

                        keys.Add("TagID");
                        keys.Add("ID_Location");
                        keys.Add("Last_Modified_By");
                        keys.Add("ID_Reader");
                        keys.Add("ReferenceNo");
                        keys.Add("ID_Reason");

                        ArrayList values = new ArrayList();
                        values.Add(TagIDs);
                        values.Add(ID_Location);
                        values.Add(Login.ID);
                        values.Add(0);
                        values.Add("");
                        values.Add(ID_Reason);
                        result = OnConn.ExecQuery("Update_Asset_Location_by_HH", keys.ToArray(), values.ToArray());
                        madeOfflineUpdate = true;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.Message);
                }

            }

            if (!Login.OnLineMode || madeOfflineUpdate)
            {
                //throw new ApplicationException("Offline Function UpdateAssetLocation not working.");

                try
                {
                    using (CEConn localDB = new CEConn())
                    {
                        // updated by vijay on 13 Nov 09 for batch processing 

                        //strSql = "update Assets set ModifiedBy=" + Login.ID + ", ID_Location=" + ID_Location + " where TagID = '" + TagID + "'";

                        strSql = "update Assets set ModifiedBy=" + Login.ID + ", ID_Location=" + ID_Location + " where TagID IN (" + TagIDs + ")";
                        if (madeOfflineUpdate == true)
                            localDB.runQuery(strSql);
                        else
                            result = localDB.runQuery(strSql);

                        strSql = " Update Locations Set UsageCount = UsageCount + 1,OfflineUsageCount = OfflineUsageCount + 1,RowStatus=" + Convert.ToInt32(RowStatus.Modify) + ", Date_MOdified=getDate(), ModifiedBy=" + Login.ID + " Where ServerKey =" + ID_Location;
                        localDB.runQuery(strSql);

                    }
                }
                catch (Exception ee)
                {
                    Logger.LogError(ee.Message);
                }
            }

            return result;

        }

        public static int Update_AssetsLocation(string TagIDs, Int64 ID_Location, Int64 ID_Reason)
        {
            bool madeOfflineUpdate = false;
            string strSql;
            int result = 0;
            if (Login.OnLineMode)
            {
                try
                {
                    using (RConnection.RConnection OnConn = new RConnection.RConnection())
                    {
                        OnConn.Url = Login.webConURL;
                        // strSql = "exec Update_Asset_Location_by_HH " + TagIDs + " ," + ID_Location + ", " + Login.ID + " ,0,''," + ID_Reason;

                        ArrayList keys = new ArrayList();

                        keys.Add("TagID");
                        keys.Add("ID_Location");
                        keys.Add("Last_Modified_By");
                        keys.Add("ID_Reader");
                        keys.Add("ReferenceNo");
                        keys.Add("ID_Reason");

                        ArrayList values = new ArrayList();
                        values.Add(TagIDs);
                        values.Add(ID_Location);
                        values.Add(Login.ID);
                        values.Add(0);
                        values.Add("");
                        values.Add(ID_Reason);
                        result = OnConn.ExecQuery("Update_Asset_Location_by_HH", keys.ToArray(), values.ToArray());
                        madeOfflineUpdate = true;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.Message);
                }

            }

            try
            {
                if (!Login.OnLineMode)
                {
                    //throw new ApplicationException("Offline Function UpdateAssetLocation not working.");
                    using (CEConn localDB = new CEConn())
                    {
                        strSql = "update Assets set RowStatus=" + Convert.ToInt32(RowStatus.Modify) + ", Date_MOdified=getDate(), ModifiedBy=" + Login.ID + ", ID_Location=" + ID_Location + " where TagID IN (" + TagIDs + ")";
                        result = localDB.runQuery(strSql);

                        strSql = " Update Locations Set UsageCount = UsageCount + 1,OfflineUsageCount = OfflineUsageCount + 1,RowStatus=" + Convert.ToInt32(RowStatus.Modify) + ", Date_MOdified=getDate(), ModifiedBy=" + Login.ID + " Where ServerKey =" + ID_Location;
                        localDB.runQuery(strSql);
                    }
                }
                else if (madeOfflineUpdate == true && result > 0)
                {
                    using (CEConn localDB = new CEConn())
                    {
                        strSql = "update Assets set RowStatus=" + Convert.ToInt32(RowStatus.Modify) + ", Date_MOdified=getDate(), ModifiedBy=" + Login.ID + ", ID_Location=" + ID_Location + " where TagID IN (" + TagIDs + ")";
                        localDB.runQuery(strSql);

                        strSql = " Update Locations Set UsageCount = UsageCount + 1,OfflineUsageCount = OfflineUsageCount + 1,RowStatus=" + Convert.ToInt32(RowStatus.Modify) + ", Date_MOdified=getDate(), ModifiedBy=" + Login.ID + " Where ServerKey =" + ID_Location;
                        localDB.runQuery(strSql);
                    }
                }

               
            }
            catch (Exception ee)
            {
                Logger.LogError(ee.Message);
            }

            return result;
        }

        public static int UpdateAssetLocation(string TagID, Int64 ID_Location)
        {
            return UpdateAssetLocation(TagID, ID_Location, 0);
        }

        public static int UpdateAssetLocation(string TagIDs, Int64 ID_Location, string ReferenceNo, Boolean IsDispatch)
        {
            string strSql;
            int result = 0;

            if (Login.OnLineMode)
            {
                RConnection.RConnection OnConn = new RConnection.RConnection();
                OnConn.Url = Login.webConURL;
                // strSql = "exec Update_Asset_Location_by_HH '''" + TagID + "''' ," + ID_Location + ", " + Login.ID + " ,0,'" + ReferenceNo + "'";
                //OnConn.runQuery(strSql);

                // updated by vijay on 13 Nov 09 for batch processing 

                ArrayList keys = new ArrayList();

                keys.Add("TagID");
                keys.Add("ID_Location");
                keys.Add("Last_Modified_By");
                keys.Add("ID_Reader");
                keys.Add("ReferenceNo");
                keys.Add("ID_Reason");

                ArrayList values = new ArrayList();
                values.Add(TagIDs);
                values.Add(ID_Location);
                values.Add(Login.ID);
                values.Add(0);
                values.Add(ReferenceNo);
                values.Add(0);
                result = OnConn.ExecQuery("Update_Asset_Location_by_HH", keys.ToArray(), values.ToArray());

            }
            else
            {
                using (CEConn localDB = new CEConn())
                {
                    if (IsDispatch)
                    {
                        // strSql = "update Assets set RowStatus=" + Convert.ToInt32(RowStatus.Dispatched) + ", Date_MOdified=getDate(), ModifiedBy=" + Login.ID + ", ID_Location=" + ID_Location + " , ReferenceNo='" + ReferenceNo + "' where TagID='" + TagID + "'";
                        // updated by vijay on 13 Nov 09 for batch processing 

                        strSql = "update Assets set RowStatus=" + Convert.ToInt32(RowStatus.Dispatched) + ", Date_MOdified=getDate(), ModifiedBy=" + Login.ID + ", ID_Location=" + ID_Location + " , ReferenceNo='" + ReferenceNo + "' where TagID IN (" + TagIDs + ")";

                    }
                    else
                    {
                        //strSql = "update Assets set RowStatus=" + Convert.ToInt32(RowStatus.Modify) + ", Date_MOdified=getDate(), ModifiedBy=" + Login.ID + ", ID_Location=" + ID_Location + " , ReferenceNo='" + ReferenceNo + "' where TagID='" + TagID + "'";
                        strSql = "update Assets set RowStatus=" + Convert.ToInt32(RowStatus.Modify) + ", Date_MOdified=getDate(), ModifiedBy=" + Login.ID + ", ID_Location=" + ID_Location + " , ReferenceNo='" + ReferenceNo + "' where TagID IN (" + TagIDs + ")";

                    }

                    result = localDB.runQuery(strSql);

                    if (!IsDispatch)
                    {
                        strSql = " Update Locations Set UsageCount = UsageCount + 1,OfflineUsageCount = OfflineUsageCount + 1,RowStatus=" + Convert.ToInt32(RowStatus.Modify) + ", Date_MOdified=getDate(), ModifiedBy=" + Login.ID + " Where ServerKey =" + ID_Location;
                        localDB.runQuery(strSql);
                    }

                }
            }

            return result;
        }

        public static int UpdateAssetLocation(string TagID, Int64 ID_Location, string ReferenceNo)
        {
            return UpdateAssetLocation(TagID, ID_Location, ReferenceNo, false);
        }
    }
}
