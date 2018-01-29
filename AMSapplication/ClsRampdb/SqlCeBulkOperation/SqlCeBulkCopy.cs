using System;
using System.Collections.Generic;
using System.Text;

using System.Data.SqlServerCe;
using System.Data;
using ClsLibBKLogs;
using System.Data.SqlClient;
 

namespace ErikEJ.SqlCe
{
    public delegate void SqlCeRowsCopiedEventHandler(object sender, SqlCeRowsCopiedEventArgs e);

    public class SqlCeBulkCopy : IDisposable
    {
        private int notifyAfter;
        private DataRowState rowState;
        private SqlCeConnection conn;
        private bool ownsConnection;
        private bool keepNulls;
        private bool keepIdentity;
        private string destination;
        private SqlCeBulkCopyOptions options;
        private SqlCeBulkCopyColumnMappingCollection mappings = new SqlCeBulkCopyColumnMappingCollection();

         

        public SqlCeBulkCopy(SqlCeConnection connection)
        {
            this.conn = connection;
        }

        public SqlCeBulkCopy(SqlCeConnection connection, SqlCeBulkCopyOptions copyOptions)
        {
            this.conn = connection;
            this.options = copyOptions;
            this.keepNulls = this.IsCopyOption(SqlCeBulkCopyOptions.KeepNulls);
            this.keepIdentity = this.IsCopyOption(SqlCeBulkCopyOptions.KeepIdentity);
        }

        public SqlCeBulkCopy(string connectionString)
        {
            this.conn = new SqlCeConnection(connectionString);
            this.ownsConnection = true;
        }
        public SqlCeBulkCopy(string connectionString, SqlCeBulkCopyOptions copyOptions)
        {
            this.conn = new SqlCeConnection(connectionString);
            this.ownsConnection = true;
            this.options = copyOptions;
            this.keepNulls = this.IsCopyOption(SqlCeBulkCopyOptions.KeepNulls);
            this.keepIdentity = this.IsCopyOption(SqlCeBulkCopyOptions.KeepIdentity);
        }

        //TODO Implement
        public SqlCeBulkCopyColumnMappingCollection ColumnMappings
        {
            get
            {
                return this.mappings;
            }
        }

        public string DestinationTableName
        {
            get
            {
                return this.destination;
            }
            set
            {
                this.destination = value;
            }
        }

        public int NotifyAfter
        {
            get
            {
                return this.notifyAfter;
            }
            set
            {
                if (value < 0)
                {
                    throw new IndexOutOfRangeException();
                }
                notifyAfter = value;
            }
        }

        public event SqlCeRowsCopiedEventHandler SqlCeRowsCopied;

        public void Close()
        {
            if (this.ownsConnection && this.conn != null)
            {
                this.conn.Dispose();
            }
        }

        public void WriteToServer(DataRow[] rows)
        {
            throw new NotImplementedException();
        }

        public void WriteToServer(DataTable table)
        {
            WriteToServer(table, 0);
        }

        public void WriteToServer(DataTable table, DataRowState rowState)
        {
            this.rowState = rowState;
            CheckDestination();

            if (this.mappings.Count < 1)
            {
                if (this.conn.State != ConnectionState.Open)
                {
                    this.conn.Open();
                }
                using (SqlCeCommand cmd = new SqlCeCommand(this.destination, this.conn))
                {
                    cmd.CommandType = CommandType.TableDirect;
                    using (SqlCeResultSet rs = cmd.ExecuteResultSet(ResultSetOptions.Updatable))
                    {
                        int idOrdinal = this.IdentityOrdinal();
                        int offset = 0;
                        SqlCeUpdatableRecord rec = rs.CreateRecord();
                        //this.mappings.ValidateCollection(rec, table.Columns);

                        int fieldCount = rec.FieldCount;
                        if (idOrdinal > -1)
                        {
                            fieldCount = fieldCount - 1;
                            offset = 1;
                        }
                        if (table.Columns.Count != fieldCount)
                        {
                            throw new Exception("Field counts do not match");
                        }
                        int rowCounter = 0;
                        int totalRows = 0;
                        IdInsertOn();
                        foreach (DataRow row in table.Rows)
                        {
                            // Never process deleted rows
                            if (row.RowState == DataRowState.Deleted)
                                continue;

                            // if a specific rowstate is requested
                            if (this.rowState != 0)
                            {
                                if (row.RowState != this.rowState)
                                    continue;
                            }


                            for (int i = 0; i < rec.FieldCount; i++)
                            {
                                // Let the destination assign identity values
                                if (!keepIdentity && i == idOrdinal)
                                    continue;

                                int y = i - offset;

                                if (row[y] != null && row[y].GetType() != typeof(System.DBNull))
                                {
                                    rec.SetValue(i, row[y]);
                                }
                                else
                                {
                                    if (keepNulls)
                                    {
                                        rec.SetValue(i, DBNull.Value);
                                    }
                                    else
                                    {
                                        rec.SetDefault(i);
                                    }
                                }
                                // Fire event if needed
                                if (this.notifyAfter > 0 && rowCounter == this.notifyAfter)
                                {
                                    FireRowsCopiedEvent(totalRows);
                                    rowCounter = 0;
                                }
                            }
                            rowCounter++;
                            totalRows++;
                            rs.Insert(rec);
                        }
                        IdInsertOff();
                    }
                }


            }
        }

        public int WriteToServer(DataTable table, DataRowState rowState, string str)
        {
            this.rowState = rowState;
            //CheckDestination();

            int totalRows = 0;
            int errorCount = 0;

            if (this.mappings.Count < 1)
            {
                if (this.conn.State != ConnectionState.Open)
                {
                    this.conn.Open();
                }
                using (SqlCeCommand cmd = new SqlCeCommand(this.destination, this.conn))
                {
                    cmd.CommandType = CommandType.TableDirect;
                    using (SqlCeResultSet rs = cmd.ExecuteResultSet(ResultSetOptions.Updatable))
                    {
                        int idOrdinal = this.IdentityOrdinal();
                       // int offset = 0;
                        SqlCeUpdatableRecord rec = rs.CreateRecord();                         
                        
                       // DataTable dt = rs.GetSchemaTable();                        

                        //this.mappings.ValidateCollection(rec, table.Columns);

                        int fieldCount = rec.FieldCount;

                        //if (idOrdinal > -1)
                        //{
                        //    fieldCount = fieldCount - 1;
                        //    offset = 1;
                        //}
                        //if (table.Columns.Count - 3 != rec.FieldCount)
                        //{
                        //    throw new Exception("Field counts do not match");
                        //}
                        int rowCounter = 0;                       
                        //IdInsertOn();
                        
                        string[] colNames = new string[rec.FieldCount];

                        for (int y = 0; y < rec.FieldCount; y++)
                        {
                            colNames[y] = rec.GetName(y);
                        }

                        foreach (DataRow row in table.Rows)
                        {
                            try
                            {
                                if ((row["Is_Deleted"] == DBNull.Value) || ((Convert.ToInt32(row["Is_Deleted"]) == 0) && (Convert.ToInt32(row["Is_Active"]) == 1)))
                                {
                                    // Never process deleted rows
                                    //if (row.RowState == DataRowState.Deleted)
                                    //    continue;

                                    //// if a specific rowstate is requested
                                    //if (this.rowState != 0)
                                    //{
                                    //    if (row.RowState != this.rowState)
                                    //        continue;
                                    //}

                                    for (int y = 1; y < rec.FieldCount; y++)
                                    {
                                        // Let the destination assign identity values
                                        try
                                        {

                                            object value = row[colNames[y]];

                                            if (value != null && value != DBNull.Value)
                                            {
                                                rec.SetValue(y, value);
                                            }
                                            else
                                            {
                                                if (keepNulls)
                                                {
                                                    // rec.SetValue(i, DBNull.Value);
                                                    rec.SetValue(y, DBNull.Value);
                                                }
                                                else
                                                {
                                                    rec.SetDefault(y);
                                                }
                                            }
                                            // Fire event if needed
                                            if (this.notifyAfter > 0 && rowCounter == this.notifyAfter)
                                            {
                                                FireRowsCopiedEvent(totalRows);
                                                rowCounter = 0;
                                            }
                                        }
                                        catch (Exception iEx)
                                        {
                                        }
                                    }
                                    rowCounter++;                                   
                                    rs.Insert(rec);                                     
                                    totalRows++;
                                }
                            }
                            catch (SqlCeException sqlex)
                            {                                
                                errorCount++;
                                if (errorCount > 50)
                                {
                                    throw sqlex;
                                   // break;
                                }
                            }
                            catch (Exception ex)
                            {
                                errorCount++;
                                if (errorCount > 50)
                                {
                                    throw ex;
                                    //break;
                                }
                            }
                        }
                      //  IdInsertOff();
                    }
                }


            }
            return totalRows;
        }


        public int WriteToServerNew(DataTable table, DataRowState rowState, string str)
        {
            this.rowState = rowState;
            //CheckDestination();

            int totalRows = 0;
            int errorCount = 0;

            if (this.mappings.Count < 1)
            {
                if (this.conn.State != ConnectionState.Open)
                {
                    this.conn.Open();
                }
                using (SqlCeCommand cmd = new SqlCeCommand(this.destination, this.conn))
                {
                    cmd.CommandType = CommandType.TableDirect;
                    using (SqlCeResultSet rs = cmd.ExecuteResultSet(ResultSetOptions.Updatable))
                    {
                        int idOrdinal = this.IdentityOrdinal();
                        // int offset = 0;
                        SqlCeUpdatableRecord rec = rs.CreateRecord();

                        // DataTable dt = rs.GetSchemaTable();                        

                        //this.mappings.ValidateCollection(rec, table.Columns);

                        int fieldCount = rec.FieldCount;

                        //if (idOrdinal > -1)
                        //{
                        //    fieldCount = fieldCount - 1;
                        //    offset = 1;
                        //}
                        //if (table.Columns.Count - 3 != rec.FieldCount)
                        //{
                        //    throw new Exception("Field counts do not match");
                        //}
                        int rowCounter = 0;
                        //IdInsertOn();

                        //string[] colNames = new string[rec.FieldCount];

                        //for (int y = 0; y < rec.FieldCount; y++)
                        //{
                        //    colNames[y] = rec.GetName(y);
                        //}

                        int[] colIndex = new int[rec.FieldCount];

                        string colName;
                        for (int y = 1; y < rec.FieldCount; y++)
                        {
                            colName = rec.GetName(y);
                            if (colName.ToLower() == "rowstatus")
                            {
                                colIndex[y] = -1;
                            }
                            else
                            {
                                colIndex[y] = table.Columns[rec.GetName(y)].Ordinal;
                            }
                           
                        }

                        object value;

                        foreach (DataRow row in table.Rows)
                        {
                            try
                            {
                                if ((row["Is_Deleted"] == DBNull.Value) || ((Convert.ToInt32(row["Is_Deleted"]) == 0) && (Convert.ToInt32(row["Is_Active"]) == 1)))
                                {
                                    // Never process deleted rows
                                    //if (row.RowState == DataRowState.Deleted)
                                    //    continue;

                                    //// if a specific rowstate is requested
                                    //if (this.rowState != 0)
                                    //{
                                    //    if (row.RowState != this.rowState)
                                    //        continue;
                                    //}

                                    for (int y = 1; y < rec.FieldCount; y++)
                                    {
                                        // Let the destination assign identity values
                                        try
                                        {
                                            if (colIndex[y] == -1)
                                            {
                                                value =  Convert.ToInt16(RowStatus.Synchronized);
                                            }
                                            else
                                            {
                                                value = row[colIndex[y]];
                                            }

                                            if (value != null && value != DBNull.Value)
                                            {
                                                rec.SetValue(y, value);
                                            }
                                            else
                                            {
                                                if (keepNulls)
                                                {
                                                    // rec.SetValue(i, DBNull.Value);
                                                    rec.SetValue(y, DBNull.Value);
                                                }
                                                else
                                                {
                                                    rec.SetDefault(y);
                                                }
                                            }
                                            // Fire event if needed
                                            if (this.notifyAfter > 0 && rowCounter == this.notifyAfter)
                                            {
                                                FireRowsCopiedEvent(totalRows);
                                                rowCounter = 0;
                                            }
                                        }
                                        catch (Exception iEx)
                                        {
                                        }
                                    }
                                    rowCounter++;
                                    rs.Insert(rec);
                                    totalRows++;
                                }
                            }
                            catch (SqlCeException sqlex)
                            {
                                errorCount++;
                                if (errorCount > 50)
                                {
                                    throw sqlex;
                                    // break;
                                }
                            }
                            catch (Exception ex)
                            {
                                errorCount++;
                                if (errorCount > 50)
                                {
                                    throw ex;
                                    //break;
                                }
                            }
                        }
                        //  IdInsertOff();
                    }
                }


            }
            return totalRows;
        }


        public void WriteToServer(IDataReader reader)
        {
            try
            {
                CheckDestination();

                if (this.mappings.Count < 1)
                {
                    if (this.conn.State != ConnectionState.Open)
                    {
                        this.conn.Open();
                    }
                    using (SqlCeCommand cmd = new SqlCeCommand(this.destination, this.conn))
                    {
                        cmd.CommandType = CommandType.TableDirect;
                        using (SqlCeResultSet rs = cmd.ExecuteResultSet(ResultSetOptions.Updatable))
                        {
                            int idOrdinal = this.IdentityOrdinal();
                            int offset = 0;
                            SqlCeUpdatableRecord rec = rs.CreateRecord();
                            //this.mappings.ValidateCollection(rec, table.Columns);

                            int fieldCount = rec.FieldCount;
                            if (idOrdinal > -1)
                            {
                                fieldCount = fieldCount - 1;
                                offset = 1;
                            }
                            if (reader.FieldCount != rec.FieldCount)
                            {
                                throw new Exception("Field counts do not match");
                            }
                            int rowCounter = 0;
                            int totalRows = 0;
                            //   IdInsertOn();
                            while (reader.Read())
                            {
                                for (int i = 0; i < fieldCount; i++)
                                {

                                    // Let the destination assign identity values
                                    if (!keepIdentity && i == idOrdinal)
                                        continue;

                                    int y = i - offset;

                                    if (reader[y] != null && reader[y].GetType() != typeof(System.DBNull))
                                    {
                                        rec.SetValue(i, reader[y]);
                                    }
                                    else
                                    {
                                        if (keepNulls)
                                        {
                                            rec.SetValue(i, DBNull.Value);
                                        }
                                        else
                                        {
                                            rec.SetDefault(i);
                                        }
                                    }
                                    // Fire event if needed
                                    if (this.notifyAfter > 0 && rowCounter == this.notifyAfter)
                                    {
                                        FireRowsCopiedEvent(totalRows);
                                        rowCounter = 0;
                                    }
                                }
                                rowCounter++;
                                totalRows++;
                                rs.Insert(rec);
                            }
                            //    IdInsertOff();
                        }
                    }
                }
            }
            finally
            {
                reader.Close();
            }
        }

        private void CheckDestination()
        {
            if (string.IsNullOrEmpty(this.destination))
            {
                throw new Exception("DestinationTable not specified");
            }
        }

        private void IdInsertOn()
        {
            if (keepIdentity)
            {
                using (SqlCeCommand idCmd = new SqlCeCommand(string.Format("SET IDENTITY_INSERT [{0}] ON", this.DestinationTableName), this.conn))
                {
                    idCmd.ExecuteNonQuery();
                }
            }
        }

        private void IdInsertOff()
        {
            if (keepIdentity)
            {
                using (SqlCeCommand idCmd = new SqlCeCommand(string.Format("SET IDENTITY_INSERT [{0}] OFF", this.DestinationTableName), this.conn))
                {
                    idCmd.ExecuteNonQuery();
                }
            }
        }

        private int IdentityOrdinal()
        {
            int ordinal = -1;
            if (!IsCopyOption(SqlCeBulkCopyOptions.KeepIdentity))
            {
                using (SqlCeCommand ordCmd = new SqlCeCommand(string.Format("SELECT ORDINAL_POSITION FROM information_schema.columns WHERE TABLE_NAME = N'{0}' AND AUTOINC_SEED IS NOT NULL", this.DestinationTableName), this.conn))
                {
                    object val = ordCmd.ExecuteScalar();
                    if (val != null)
                        ordinal = (int)val - 1;
                }
            }
            return ordinal;
        }


        private void OnRowsCopied(SqlCeRowsCopiedEventArgs value)
        {
            SqlCeRowsCopiedEventHandler handler = this.SqlCeRowsCopied;
            if (handler != null)
            {
                handler(this, value);
            }
        }

        private void FireRowsCopiedEvent(long rowsCopied)
        {
            SqlCeRowsCopiedEventArgs args = new SqlCeRowsCopiedEventArgs(rowsCopied);
            this.OnRowsCopied(args);
        }

        private bool IsCopyOption(SqlCeBulkCopyOptions copyOption)
        {
            return ((this.options & copyOption) == copyOption);
        }


        #region IDisposable Members

        public void Dispose()
        {
            if (this.ownsConnection && this.conn != null)
            {
                this.conn.Dispose();
            }
        }

        #endregion
    }
}
