/**************************************************************************************
 * Author : Deepanshu Jouhari
 * Modified Date : 15-Jun-2009
 * Last Modified by : 
 * Last Modified : 
 * Module Name : ClsRampDb
 * Decription : Documents class to interface SQL CE Documents Table
 **************************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using ClsLibBKLogs;

namespace ClsRampdb
{
    public class Documents
    {
        #region "Private variables and Properties"
        Int32 _ServerKey;
        Int64 _DocumentID;
        List<Assets> _lstAsset;
        List<double> _lstQty;
        String _DocumentNo;
        String _TagID;
        DateTime _DocumentDate;
        String _Notes;
        int _DocumentTypeID;
        DateTime _DispatchDate;
        Int32 _RowStatus;

        public Int32 ServerKey
        {
            get { return _ServerKey; }
        }
        public Int64 DocumentID
        {
            get { return _DocumentID; }
            set { _DocumentID = value; }
        }
        

        public List<Assets> LstAsset
        {
            get { return _lstAsset; }
            set { _lstAsset = value; }
        }


        
        public List<double> LstQty
        {
            get { return _lstQty; }
            set { _lstQty = value; }
        }

        
        
        public String DocumentNo
        {
            get { return _DocumentNo; }
            set { _DocumentNo = value; }
        }
        
        public String TagID
        {
            get { return _TagID; }
            set { _TagID = value; }
        }
        

        public DateTime DocumentDate
        {
            get { return _DocumentDate; }
            set { _DocumentDate = value; }
        }
        
        public String Notes
        {
            get { return _Notes; }
            set { _Notes = value; }
        }
        

        public int DocumentTypeID
        {
            get { return _DocumentTypeID; }
            set { _DocumentTypeID = value; }
        }
        

        public DateTime DispatchDate
        {
            get { return _DispatchDate; }
            set { _DispatchDate = value; }
        }

        public RowStatus Status
        {
            get { return (RowStatus)_RowStatus; }
        }
        #endregion

        public Documents(Int32 ServerKey)
        {
            List<string> lstAssetTagID = new List<string>(); 
            if (!Login.OnLineMode)
            {
                _ServerKey = ServerKey;
                _lstAsset = new List<Assets>();
                _lstQty = new List<double>();
                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = " select D.TagID,D.DocumentNo,D.DocumentID,D.DocumentDate,D.Notes,D.DocumentTypeID,D.DispatchDate,D.ServerKey,Ds.Qty,Ds.TagID as AssetTagID, D.RowStatus from DocumentDetails Ds,Documents D where D.DocumentNo=Ds.DocumentNo and D.ServerKey=" + ServerKey + "";
                    SqlCeDataReader dr;
                    dr = localDB.getReader(strSql);
                    bool runOnce = false;
                    while (dr.Read())
                    {
                        if (!runOnce)
                        {
                            _TagID = (String)dr["TagID"];
                            if (dr["DocumentNo"] != DBNull.Value)
                                _DocumentNo = (String)dr["DocumentNo"];
                            _DocumentID = Convert.ToInt64(dr["DocumentID"]);
                            _DocumentDate = Convert.ToDateTime(dr["DocumentDate"]);
                            if (dr["Notes"] != DBNull.Value)
                                _Notes = (String)dr["Notes"];
                            _DocumentTypeID = Convert.ToInt32(dr["DocumentTypeID"]);
                            _DispatchDate = Convert.ToDateTime(dr["DispatchDate"]);
                            _ServerKey = Convert.ToInt32(dr["ServerKey"]);
                            if (dr["RowStatus"] != DBNull.Value)
                                _RowStatus = Convert.ToInt32(dr["RowStatus"]);

                            runOnce = true; 
                        }
                        lstAssetTagID.Add((String)dr["AssetTagID"]);  

                        //_lstQty.Add(Convert.ToDouble("1"));
                    }
                    
                    dr.Close();
                }


                foreach (string tgNo in lstAssetTagID)
                {
                    Assets A = new Assets(tgNo);

                    if (A != null)
                    {
                        _lstAsset.Add(A);
                        _lstQty.Add(Convert.ToDouble("1"));
                    }
                }
            }
            else
            {
                throw new ApplicationException("Online functionality not implemented yet.");
            }
        }

        public Documents(String TagID)
        {
            if (!Login.OnLineMode)
            {
                _lstAsset = new List<Assets>();
                _lstQty = new List<double>();
                _TagID = TagID;
                using (CEConn localDB = new CEConn())
                {
                    

                    string strSql;
                    strSql = " select D.TagID,D.DocumentNo,D.DocumentID,D.DocumentDate,D.Notes,D.DocumentTypeID,D.DispatchDate,D.ServerKey,Ds.Qty,Ds.TagID as AssetTagID, D.RowStatus from DocumentDetails Ds,Documents D where D.DocumentNo=Ds.DocumentNo and D.TAGID='" + TagID + "'";
                    SqlCeDataReader dr;
                    dr = localDB.getReader(strSql);
                    bool runOnce = false;
                    while (dr.Read())
                    {
                        if (!runOnce)
                        {
                            _TagID = (String)dr["TagID"];
                            if (dr["DocumentNo"] != DBNull.Value)
                                _DocumentNo = (String)dr["DocumentNo"];
                            _DocumentID = Convert.ToInt64(dr["DocumentID"]);
                            _DocumentDate = Convert.ToDateTime(dr["DocumentDate"]);
                            if (dr["Notes"] != DBNull.Value)
                                _Notes = (String)dr["Notes"];
                            _DocumentTypeID = Convert.ToInt32(dr["DocumentTypeID"]);
                            _DispatchDate = Convert.ToDateTime(dr["DispatchDate"]);
                            _ServerKey = Convert.ToInt32(dr["ServerKey"]);
                            _RowStatus = Convert.ToInt32(dr["RowStatus"]);
                        }
                        Assets A = new Assets((String)dr["AssetTagID"]);
                        _lstAsset.Add(A);
                        _lstQty.Add(1);  
                    }
                    dr.Close();
                }
            }
            else
            {
                throw new ApplicationException("Online functionality not implemented yet.");
            }
        }

         

        public static void AddDocument(String TagID,
                        List<string> lstAssetTags,List<float> lstQty,String documentNo,
                        DateTime documentDate,String notes,int documentTypeID,
                        DateTime dispatchDate)
        {
            //if (!Login.OnLineMode)
            //{
                Int32 Skey;
                Skey = minServerKey();
                // added by vijay 08 Dec 2009
                if (Skey == -2)
                    Skey = Skey - 1;
                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    string strSqlDetail;

                    strSql = " select count(*) from Assets A,Employees E,Locations L, Documents D where D.TagID='" + TagID + "' or A.TagID='" + TagID + "' or E.TagID='" + TagID + "' or L.TagID='" + TagID + "'";
                    if (Convert.ToInt32(localDB.getScalerValue(strSql)) > 0)
                        throw new ApplicationException("Duplicate Tag ID.");

                    strSql = " select count(*) from Documents D where D.DocumentNo='" + documentNo + "'";
                    if (Convert.ToInt32(localDB.getScalerValue(strSql)) > 0)
                        throw new ApplicationException("Duplicate Document No.");

                    strSql = " insert into Documents(TagID,documentNo,documentDate,notes,documentTypeID,dispatchDate,Date_Modified,ModifiedBy,RowStatus,serverKey)";
                    strSql += " values('" + TagID + "','" + documentNo.Replace("'", "''") + "','"
                        + documentDate.ToString("yyyy-MM-dd hh:mm:ss") + "','" + notes.Replace("'", "''") + "'," + documentTypeID
                        + ",'" + dispatchDate.ToString("yyyy-MM-dd hh:mm:ss") + "',getDate()," + Login.ID + "," + Convert.ToInt32(RowStatus.New) + "," + Skey + ")";

                    Int32 lstCount = 0;
                    foreach (string strAssetTag in lstAssetTags) 
                    {
                        strSqlDetail = " insert into DocumentDetails(Qty,DocumentNo,TagID)";
                        strSqlDetail += " values(" + lstQty[lstCount] + ",'" + documentNo + "','" + strAssetTag + "')";
                        localDB.runQuery(strSqlDetail);   
                    }
                    localDB.runQuery(strSql);
                }
            //}
            //else
            //{
            //    throw new ApplicationException("Online functionality not implemented yet.");
            //}
        }

        private static Int32 minServerKey()
        {
            using (CEConn localDB = new CEConn())
            {
                string strSql;
                strSql = "select min(serverKey) from Documents";
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

        public static DataTable getRows()
        {
            DataTable dtDocs = new DataTable("dtDocs");
            string strSql;
            using (CEConn localDB = new CEConn())
            {
                //select D.TagID,D.DocumentNo,D.DocumentTypeID,D.ServerKey,Ds.Qty,Ds.TagID as AssetTagID from DocumentDetails Ds,Documents D where D.DocumentNo=Ds.DocumentNo and D.TAGID='" + TagID + "'
                strSql = "select D.TagID,D.DocumentNo,D.DocumentTypeID,D.ServerKey as DocumentID from Documents D"; // where RowStatus=" + Convert.ToInt32(Stat);
                localDB.FillDataTable(ref dtDocs, strSql);
            }
            return dtDocs;
        }
    }
}
