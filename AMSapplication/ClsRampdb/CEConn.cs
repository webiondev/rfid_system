/**************************************************************************************
 * Author : Deepanshu Jouhari
 * Modified Date : 15-Sep-2008
 * Last Modified by : 
 * Last Modified : 
 * Module Name : ClsRampDb
 * Decription : CEConn class to implement db connection interface to other classes.
 **************************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using ClsLibBKLogs;

namespace ClsRampdb
{
    public class CEConn:IDisposable 
    {
        #region "Varibles"
        public static String dbFilePath; 

        //private  SqlCeConnection  _CEConnection;
         private SqlCeTransaction _CETransaction;

        private static SqlCeConnection _CEConnection;
       // private static SqlCeTransaction _CETransaction;

        //private String _CEConnStr;
       
        #region "Properties"
        public SqlCeConnection CEConnection
        {
	        get
	        {
                return _CEConnection; 
	        }
        }
        #endregion
        #endregion

        public CEConn()
        {
            try
            {
                if (dbFilePath == "")
                {
                    throw new ApplicationException("Please set Database file Path first.");
                }
               // _CEConnection = null;
               //dbFilePath = @"./ReaderDB.sdf";

                if(_CEConnection == null)
                    _CEConnection = new SqlCeConnection("DataSource='" + dbFilePath + "'");

                if (_CEConnection.State == ConnectionState.Closed || _CEConnection.State == ConnectionState.Broken)
                {
                    try
                    {
                        _CEConnection.Open();
                       
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("Connection.Open() -- " + ex.Message);
                        throw ex;
                    }
                }
                _CETransaction = _CEConnection.BeginTransaction(); 
               
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
            //_CETransaction =  
        }

        public int runQuery(string sqlText)
        {
            int result = 0;
            try
            {
                //_CEConnection.Open();
                SqlCeCommand selCmd = new SqlCeCommand(sqlText, _CEConnection, _CETransaction);
                result = selCmd.ExecuteNonQuery();
                selCmd.Dispose();
                return result;
                 
            }
            catch (SqlCeException sqlceex)
            {
               // Logger.LogError(sqlceex.Message);
                throw sqlceex;
               
            }
            catch (Exception ex)
            {
                //Logger.LogError(ex.Message);
                throw  ex;
            }
            
        }

        public int runQueryWithoutTransaction(string sqlText)
        {
            try
            {
                //_CEConnection.Open();
                SqlCeCommand selCmd = new SqlCeCommand(sqlText, _CEConnection);
                return selCmd.ExecuteNonQuery();

            }
            catch (SqlCeException sqlceex)
            {
                throw sqlceex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        
        public void runQuery(string spName, SqlCeParameter[] paramAr)
        {
            //_CEConnection.Open();
            try
            {
                SqlCeCommand selCmd = new SqlCeCommand(spName, _CEConnection, _CETransaction);
                if (paramAr.Length != 0)
                {
                    foreach (SqlCeParameter param in paramAr)
                    {
                        if (param != null)
                        {
                            selCmd.Parameters.Add(paramAr[0]);
                        }
                    }
                }
                selCmd.ExecuteNonQuery();
                selCmd.Dispose();
            }
            catch (Exception ex)
            {
                Logger.LogError("CEConn - runQuery " + ex.Message);
            }
        }

        public void FillDataTable(ref DataTable dataTbl, string spName, SqlCeParameter[] paramAr)
        {
            SqlCeCommand selCmd = new SqlCeCommand(spName, _CEConnection, _CETransaction);
            
            if (paramAr.Length != 0)
            {
                foreach (SqlCeParameter param in paramAr)
                {
                    if (param != null)
                    {
                        selCmd.Parameters.Add(paramAr[0]);
                    }
                }
            }

            SqlCeDataAdapter DbAdptr = new SqlCeDataAdapter(selCmd);

            if (dataTbl == null)
                dataTbl = new DataTable();

            DbAdptr.Fill(dataTbl);
        }

        public void FillDataTable(ref DataTable dataTbl, string sqlText)
        {
            SqlCeCommand selCmd = new SqlCeCommand(sqlText, _CEConnection, _CETransaction);
            
            SqlCeDataAdapter DbAdptr = new SqlCeDataAdapter(selCmd);

            if (dataTbl == null)
                dataTbl = new DataTable();

            DbAdptr.Fill(dataTbl);

        }

        public SqlCeDataReader getReader(string sqlText)
        {
            SqlCeCommand selCmd = new SqlCeCommand(sqlText, _CEConnection, _CETransaction);
            return selCmd.ExecuteReader();    
        }

        public object getScalerValue(string sqlText)
        {
            SqlCeCommand selCmd = new SqlCeCommand(sqlText, _CEConnection, _CETransaction);
            return selCmd.ExecuteScalar();
        }

        public SqlCeDataReader getReader(string spName, SqlCeParameter[] paramAr)
        {
            SqlCeCommand selCmd = new SqlCeCommand(spName, _CEConnection, _CETransaction);
            if (paramAr.Length != 0)
            {
                foreach (SqlCeParameter param in paramAr)
                {
                    if (param != null)
                    {
                        selCmd.Parameters.Add(paramAr[0]);
                    }
                }
            }
            return selCmd.ExecuteReader();
        }

        public object getScalerValue(string spName, SqlCeParameter[] paramAr)
        {
            SqlCeCommand selCmd = new SqlCeCommand(spName, _CEConnection, _CETransaction);
            if (paramAr.Length != 0)
            {
                foreach (SqlCeParameter param in paramAr)
                {
                    if (param != null)
                    {
                        selCmd.Parameters.Add(paramAr[0]);
                    }
                }
            }

            object objVal = selCmd.ExecuteScalar();

            selCmd.Dispose();           

            return objVal;
        }

        public void commitTransaction()
        {
            _CETransaction.Commit();
            _CETransaction = _CEConnection.BeginTransaction();   
        }

        public void rollbackTransaction()
        {
            _CETransaction.Rollback();
            _CETransaction = _CEConnection.BeginTransaction();
        }

        public void Dispose()
        {
            try
            {
                _CETransaction.Commit(CommitMode.Immediate);                   
                
            }
            catch (Exception ex)
            {
                Logger.LogError("CEConn - Dispose 1" + ex.Message);
            }
            //try
            //{

            //    if (_CEConnection.State == ConnectionState.Open)
            //    {
            //        _CEConnection.Close();
            //    }
            //    _CEConnection.Dispose();
            //    _CEConnection = null;
            //}
            //catch (Exception exx)
            //{
            //    Logger.LogError("CEConn - Dispose 2" + exx.Message);
            //}
        }

        public DataTable GetGroups(string sqlText)
        {
            DataTable dt = new DataTable();

            DataColumn dc = new DataColumn("ID_AssetGroup",Type.GetType("System.Int64"));
            dt.Columns.Add(dc);

            dc = new DataColumn("Name", Type.GetType("System.String"));
            dt.Columns.Add(dc);

            DataRow dr;

            using (SqlCeCommand cmd = new SqlCeCommand(sqlText,_CEConnection))
            {
                cmd.CommandType = CommandType.Text;
                using (SqlCeResultSet rs = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                {
                    while (rs.Read())
                    {
                        dr = dt.NewRow();
                        dr["ID_AssetGroup"] = rs.GetInt64(rs.GetOrdinal("ID_AssetGroup"));
                        dr["Name"] = rs.GetString(rs.GetOrdinal("Name"));
                        dt.Rows.Add(dr);
                    }
                }
            } 

            dt.AcceptChanges();
            return dt;
        }

      

    }
}
