/**************************************************************************************
 * Author : Deepanshu Jouhari
 * Modified Date : 15-Jul-2009
 * Last Modified by : 
 * Last Modified : 
 * Module Name : ClsRampDb
 * Decription : Sync class to implement several Sync function
 **************************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ClsRampdb;
using ErikEJ.SqlCe;
using System.Collections;
using ClsLibBKLogs;

namespace ClsRampdb
{
    public class Sync
    {
        public String strError = "";

        #region Variable Declaration
        String[] tblArr = { "reasons", "employees", "locations", "fs_templates", "fieldservice", "tasks", "assets", "menu_options", "asset_groups", "master_securitygroups", "authority", "asset_status" }; //, "asset_status", "asset_types"
        Synchronize.Synchronise syn;
        public Sync()
        {
            syn = new Synchronize.Synchronise();
            syn.Url = Login.webURL;
        }

        #endregion

        #region Methods

        /// <summary>
        ///Sync Inventory 
        /// </summary>
        /// <returns></returns>
        /// 

        public DataTable SyncInventory()
        {
            DataTable dtResult = new DataTable("result");
            try
            {
                ProgressStatus.minVal = 5;
                ProgressStatus.maxVal = 100;
                //System.Threading.Thread pr = new System.Threading.Thread(ProgressStatus.autoStart);
                //pr.Start();
                //ProgressStatus.autostart();  
                string strSql = "";
                DataTable dtInventory = new DataTable("dtInventory");

                dtInventory = Assets.getInventoryRows(RowStatus.Inventory);
                ProgressStatus.increment();
                if (dtInventory.Rows.Count > 0)
                {
                    dtResult = syn.InventoryCheck(dtInventory);
                    ProgressStatus.increment();
                    if (dtResult.Rows.Count > 0)
                    {
                        using (CEConn localDB = new CEConn())
                        {
                            //foreach (DataRow dr in dtResult.Rows)
                            //{

                            //if (Convert.ToInt32(dr["RowStatus"]) == Convert.ToInt32(RowStatus.Error))
                            //{
                            strSql = "delete from Inventory where ID_Location >=0 and ModifiedBy >=0";
                            localDB.runQuery(strSql);
                            //}
                            //else
                            //{
                            //    strSql = "update Inventory set RowStatus=" + Convert.ToInt32(RowStatus.Synchronized) + " where ServerKey=" + Convert.ToInt32(dr["ServerKey"]);
                            //    localDB.runQuery(strSql);
                            //}
                            //}
                        }
                    }
                }

            }
            catch (System.Web.Services.Protocols.SoapException ex)
            {
                if (ex.Actor.ToString().ToLower().Trim() == "getlogin")
                    throw new ApplicationException("Invalid Access");
                else
                    throw ex;
            }
            catch (Exception ep)
            {
                throw ep;
            }
            finally
            {
                ProgressStatus.finish();
            }
            return dtResult;
        }

        /// <summary>
        /// Synchronise All Tables
        /// </summary>
        /// <returns></returns>
        public string Sync_Tables()
        {
            ProgressStatus.minVal = 10;
            ProgressStatus.maxVal = 100;
            //System.Threading.Thread pr = new System.Threading.Thread(ProgressStatus.autoStart);
            //pr.Start();
            string strSql;
            //D 'ProgressStatus.pauseAutoIncrement(); 

            //Call of the tables one by one in the order Master to Detail
            //Update other table if Sync table id has some reference at it's detail table
            //Call detail table sync
            //Send Only new rows and return their ID 
            //...........perform New Operation................
            //Employee Table
            //Modify Employee table with Synchronized Status

            DataTable dtEmployee = new DataTable("newemployee");
            DataTable dtLocation = new DataTable("newlocation");
            DataTable dtAssets = new DataTable("newassets");
            DataTable dtFS = new DataTable("newfs");
            DataTable dtResult = new DataTable("result");

            #region "New Employee Add"
            ProgressStatus.increment();
            dtEmployee = Employee.getNewRows();
            ProgressStatus.increment();
            if (dtEmployee.Rows.Count > 0)
            {
                ProgressStatus.increment();
                dtResult = syn.AddEmployee(dtEmployee);
                //D 'ProgressStatus.pauseAutoIncrement(); 

                using (CEConn localDB = new CEConn())
                {
                    //DataRow[] errRow;
                    //errRow = dtResult.Select("RowStatus=" + Convert.ToInt32(RowStatus.Error));    
                    foreach (DataRow dr in dtResult.Rows)
                    {
                        ProgressStatus.increment();
                        if (Convert.ToInt32(dr["RowStatus"]) == Convert.ToInt32(RowStatus.Error))
                        {
                            DataRow[] drArr;
                            drArr = dtEmployee.Select("ServerKey='" + dr["ServerKey"] + "'");
                            if (drArr.Length > 0)
                                strError += "Employee " + drArr[0]["Name"].ToString() + "not added.\r";
                            strSql = "update Employees set RowStatus=" + Convert.ToInt32(RowStatus.Error) + " where ServerKey=" + Convert.ToInt32(dr["ServerKey"]);
                            localDB.runQuery(strSql);
                        }
                        else
                        {
                            strSql = "update Employees set RowStatus=" + Convert.ToInt32(RowStatus.Synchronized) + ", ServerKey=" + Convert.ToInt32(dr["PKey"]) + " where ServerKey=" + Convert.ToInt32(dr["ServerKey"]);
                            localDB.runQuery(strSql);

                            //for location
                            strSql = "update Locations set ModifiedBy=" + Convert.ToInt32(dr["PKey"]) + " where ModifiedBy=" + Convert.ToInt32(dr["ServerKey"]);
                            localDB.runQuery(strSql);

                            //For Assets
                            strSql = "update Assets set ModifiedBy=" + Convert.ToInt32(dr["PKey"]) + " where ModifiedBy=" + Convert.ToInt32(dr["ServerKey"]);
                            localDB.runQuery(strSql);

                            //For Inventory
                            strSql = "update inventory set ModifiedBy=" + Convert.ToInt32(dr["PKey"]) + " where ModifiedBy=" + Convert.ToInt32(dr["ServerKey"]);
                            localDB.runQuery(strSql);

                        }
                    }
                    strSql = "delete from Employees where RowStatus= " + Convert.ToInt32(RowStatus.Error);
                    localDB.runQuery(strSql);

                }
            }
            #endregion

            #region "New Location Add"


            dtLocation = Locations.getNewRows();
            if (dtLocation.Rows.Count > 0)
            {
                ProgressStatus.increment();
                dtResult = syn.AddLocation(dtLocation);
                //D 'ProgressStatus.pauseAutoIncrement(); 

                using (CEConn localDB = new CEConn())
                {
                    //DataRow[] errRow;
                    //errRow = dtResult.Select("RowStatus=" + Convert.ToInt32(RowStatus.Error));    
                    foreach (DataRow dr in dtResult.Rows)
                    {
                        ProgressStatus.increment();
                        if (Convert.ToInt32(dr["RowStatus"]) == Convert.ToInt32(RowStatus.Error))
                        {
                            DataRow[] drArr;
                            drArr = dtLocation.Select("ServerKey='" + dr["ServerKey"] + "'");
                            if (drArr.Length > 0)
                                strError += "Location " + drArr[0]["Name"].ToString() + "not added.\r";

                            strSql = "update Locations set RowStatus=" + Convert.ToInt32(RowStatus.Error) + " where ServerKey=" + Convert.ToInt32(dr["ServerKey"]);
                            localDB.runQuery(strSql);
                        }
                        else
                        {
                            strSql = "update Locations set RowStatus=" + Convert.ToInt32(RowStatus.Synchronized) + ", ServerKey=" + Convert.ToInt32(dr["PKey"]) + " where ServerKey=" + Convert.ToInt32(dr["ServerKey"]);
                            localDB.runQuery(strSql);

                            //For Assets
                            strSql = "update Assets set ID_Location=" + Convert.ToInt32(dr["PKey"]) + " where ID_Location=" + Convert.ToInt32(dr["ServerKey"]);
                            localDB.runQuery(strSql);

                            //For Inventory
                            strSql = "update inventory set ID_Location=" + Convert.ToInt32(dr["PKey"]) + " where ID_Location=" + Convert.ToInt32(dr["ServerKey"]);
                            localDB.runQuery(strSql);
                        }
                    }
                    strSql = "delete from Locations where RowStatus= " + Convert.ToInt32(RowStatus.Error);
                    localDB.runQuery(strSql);

                }
            }
            #endregion

            #region "New Asset Add"

            dtAssets = Assets.getNewRows();
            if (dtAssets.Rows.Count > 0)
            {
                ProgressStatus.increment();
                dtResult = syn.AddAssets(dtAssets);
                //D 'ProgressStatus.pauseAutoIncrement(); 

                using (CEConn localDB = new CEConn())
                {
                    foreach (DataRow dr in dtResult.Rows)
                    {
                        ProgressStatus.increment();
                        if (Convert.ToInt32(dr["RowStatus"]) == Convert.ToInt32(RowStatus.Error))
                        {
                            DataRow[] drArr;
                            drArr = dtAssets.Select("ServerKey='" + dr["ServerKey"] + "'");
                            if (drArr.Length > 0)
                                strError += "Asset " + drArr[0]["Name"].ToString() + "not added.\r";

                            strSql = "update Assets set RowStatus=" + Convert.ToInt32(RowStatus.Error) + " where ServerKey=" + Convert.ToInt32(dr["ServerKey"]);
                            localDB.runQuery(strSql);
                        }
                        else
                        {
                            strSql = "update Assets set RowStatus=" + Convert.ToInt32(RowStatus.Synchronized) + ", ServerKey=" + Convert.ToInt32(dr["PKey"]) + " where ServerKey=" + Convert.ToInt32(dr["ServerKey"]);
                            localDB.runQuery(strSql);
                        }
                    }

                }
            }
            #endregion

            #region "Asset Modify"
            dtAssets = Assets.getRows(RowStatus.Modify);
            if (dtAssets.Rows.Count > 0)
            {
                ProgressStatus.increment();
                dtResult = syn.ModifyAssets(dtAssets);
                //ProgressStatus.pauseAutoIncrement(); 

                using (CEConn localDB = new CEConn())
                {
                    foreach (DataRow dr in dtResult.Rows)
                    {
                        ProgressStatus.increment();
                        if (Convert.ToInt32(dr["RowStatus"]) == Convert.ToInt32(RowStatus.Error))
                        {
                            DataRow[] drArr;
                            drArr = dtAssets.Select("ServerKey='" + dr["ServerKey"] + "'");
                            if (drArr.Length > 0)
                                strError += "Tag no " + drArr[0]["TagID"].ToString() + "not update.\r";

                            strSql = "update Assets set RowStatus=" + Convert.ToInt32(RowStatus.Error) + " where ServerKey=" + Convert.ToInt32(dr["ServerKey"]);
                            localDB.runQuery(strSql);
                        }
                        else
                        {
                            strSql = "update Assets set RowStatus=" + Convert.ToInt32(RowStatus.Synchronized) + ", ServerKey=" + Convert.ToInt32(dr["PKey"]) + " where ServerKey=" + Convert.ToInt32(dr["ServerKey"]);
                            localDB.runQuery(strSql);
                        }
                    }
                    strSql = "delete from Assets where RowStatus= " + Convert.ToInt32(RowStatus.Error);
                    localDB.runQuery(strSql);
                }
            }
            #endregion

            #region"Tag Write functionality"
            dtAssets = Assets.getRows(RowStatus.TagWrite);
            if (dtAssets.Rows.Count > 0)
            {
                ProgressStatus.increment();
                dtResult = syn.TagWrite(dtAssets);
                //D 'ProgressStatus.pauseAutoIncrement(); 

                using (CEConn localDB = new CEConn())
                {
                    foreach (DataRow dr in dtResult.Rows)
                    {
                        ProgressStatus.increment();
                        if (Convert.ToInt32(dr["RowStatus"]) == Convert.ToInt32(RowStatus.Error))
                        {
                            DataRow[] drArr;
                            drArr = dtAssets.Select("ServerKey='" + dr["ServerKey"] + "'");
                            if (drArr.Length > 0)
                                strError += "Tag no " + drArr[0]["TagID"].ToString() + "not write.\r";

                            strSql = "update Assets set RowStatus=" + Convert.ToInt32(RowStatus.Error) + " where ServerKey=" + Convert.ToInt32(dr["ServerKey"]);
                            localDB.runQuery(strSql);
                        }
                        else
                        {
                            strSql = "update Assets set RowStatus=" + Convert.ToInt32(RowStatus.Synchronized) + ", ServerKey=" + Convert.ToInt32(dr["PKey"]) + " where ServerKey=" + Convert.ToInt32(dr["ServerKey"]);
                            localDB.runQuery(strSql);
                        }
                    }
                    strSql = "delete from Assets where RowStatus= " + Convert.ToInt32(RowStatus.Error);
                    localDB.runQuery(strSql);
                }
            }
            #endregion

            #region "FieldServicePosting"
            dtFS = FS_AssetStatus.getRows(RowStatus.Modify);
            if (dtFS.Rows.Count > 0)
            {
                ProgressStatus.increment();
                dtResult = syn.UpdateFS_AssetStatus(dtFS);
                using (CEConn localDB = new CEConn())
                {
                    foreach (DataRow dr in dtResult.Rows)
                    {
                        ProgressStatus.increment();
                        if (Convert.ToInt32(dr["RowStatus"]) == Convert.ToInt32(RowStatus.Error))
                        {
                            DataRow[] drArr;
                            drArr = dtFS.Select("ServerKey='" + dr["ServerKey"] + "'");
                            if (drArr.Length > 0)
                            {
                                //Assets ast = new Assets(drArr[0]["TagID"].ToString());
                                strError += "FS Task for Item " + drArr[0]["Name"].ToString() + " not updated.\r";
                            }

                            strSql = "update FS_AssetStatus set RowStatus=" + Convert.ToInt32(RowStatus.Error) + " where ServerKey=" + Convert.ToInt32(dr["ServerKey"]);
                            localDB.runQuery(strSql);
                        }
                        else
                        {
                            //Instead of Update just delete field service data.
                            //strSql = "update FS_AssetStatus set RowStatus=" + Convert.ToInt32(RowStatus.Synchronized) + ", ServerKey=" + Convert.ToInt32(dr["PKey"]) + " where ServerKey=" + Convert.ToInt32(dr["ServerKey"]);
                            strSql = "delete from FS_AssetStatus where ServerKey=" + Convert.ToInt32(dr["ServerKey"]);
                            localDB.runQuery(strSql);
                        }
                    }
                    strSql = "delete from FS_AssetStatus where RowStatus= " + Convert.ToInt32(RowStatus.Error);
                    localDB.runQuery(strSql);
                }
            }
            #endregion

            //Run the query and fetch all the data and OverWrite
            DateTime lstSyncedDate;

            StringBuilder sqlQueries;
            StringBuilder keys;

            SqlCeBulkCopyOptions options = new SqlCeBulkCopyOptions();
            options = SqlCeBulkCopyOptions.KeepIdentity;
            options = options |= SqlCeBulkCopyOptions.KeepNulls;

            ArrayList delItems;
            int rowsCount = 0;

            foreach (String tbname in tblArr)
            {
                ProgressStatus.increment();
                lstSyncedDate = Util.getLastSyncedDate(tbname);
                DateTime outDate = lstSyncedDate;
                String errMsg;
                ProgressStatus.increment();
                Int64 ItemLimit = 0;
                ItemLimit = Login.ItemLimit - Assets.getNoOfItems();
                if (ItemLimit == 0)
                    ItemLimit = -1;
                dtResult = (DataTable)syn.SyncTableswithItemLimit(lstSyncedDate.ToString(), tbname, ItemLimit, out outDate);
                //D 'ProgressStatus.pauseAutoIncrement(); 
                try
                {
                    if (dtResult != null && dtResult.Rows.Count > 0)
                    {
                        // Util.updateTables(tbname, dr, outDate, ref sqlQueries);

                        //DataColumn dc;
                        //dc = dtResult.Columns["Date_Modified"];
                        //if (dc == null)
                        //{
                        //    dc = new DataColumn("Date_Modified");
                        //    dtResult.Columns.Add(dc);
                        //}
                        //dc.DefaultValue = outDate;

                        //dc = dtResult.Columns["RowStatus"];
                        //if (dc == null)
                        //{
                        //    dc = new DataColumn("RowStatus");
                        //    dtResult.Columns.Add(dc);
                        //}
                        //dc.DefaultValue = Convert.ToInt32(RowStatus.Synchronized);

                        //keys = new StringBuilder("-1");

                        //delItems = new ArrayList();
                        //rowsCount = 0;

                        //sqlQueries = new StringBuilder();
                        foreach (DataRow dr in dtResult.Rows)
                        {
                            //Assume that lstSyncedDate come with updated date need to pass as reference para.
                            // Delete Old Row and Insert New Row by function
                            strError += Util.updateTables(tbname, dr, outDate);

                            //rowsCount++;

                            //if (rowsCount >= 800)
                            //{
                            //    delItems.Add(keys.ToString());
                            //    keys = new StringBuilder("-1");
                            //    rowsCount = 0;
                            //}

                            //if (dr["ServerKey"] != DBNull.Value)
                            //    keys.Append("," + dr["ServerKey"].ToString());

                            // Util.updateTables(tbname, dr, outDate, ref sqlQueries);
                        }

                        //if(rowsCount > 0)
                        //    delItems.Add(keys.ToString());

                        //int totalRows = 0;
                        //try
                        //{
                        //    using (CEConn localDB = new CEConn())
                        //    {
                        //        foreach (string delList in delItems)
                        //        {
                        //            strSql = "delete from " + tbname + " where serverKey IN (" + delList + ");";
                        //            localDB.runQuery(strSql);
                        //        }

                        //        // localDB.runQuery(sqlQueries.ToString()); 

                        //        using (SqlCeBulkCopy bc = new SqlCeBulkCopy(localDB.CEConnection.ConnectionString, options))
                        //        {
                        //            bc.DestinationTableName = tbname;
                        //            totalRows = bc.WriteToServer(dtResult, 0, "");
                        //        }
                        //    }                            

                        //}
                        //catch (Exception ex)
                        //{
                        //   // strError += "\n";
                        //}

                    }
                }
                catch (System.Web.Services.Protocols.SoapException ex)
                {

                    if (ex.Actor.ToString().ToLower().Trim() == "getlogin")
                        strError += "Request from innvalid IP address.\n";
                    else
                        strError += "Soap Exception\n";

                }
                catch (Exception ep)
                {
                    strError += ep.ToString() + "\n";
                }

                //Update the Row Status of Table
                //Call Web Service
                //If got dataset delete all rows of table 
                //Insert new Row in table
            }

            //Sync Inventory is differen process need no i
            ProgressStatus.finish();
            return "";

        }

        public string SyncTables()
        {
            ProgressStatus.minVal = 10;
            ProgressStatus.maxVal = 100;
            //System.Threading.Thread pr = new System.Threading.Thread(ProgressStatus.autoStart);
            //pr.Start();
            string strSql;
            //D 'ProgressStatus.pauseAutoIncrement(); 

            //Call of the tables one by one in the order Master to Detail
            //Update other table if Sync table id has some reference at it's detail table
            //Call detail table sync
            //Send Only new rows and return their ID 
            //...........perform New Operation................
            //Employee Table
            //Modify Employee table with Synchronized Status

            DataTable dtEmployee = new DataTable("newemployee");
            DataTable dtLocation = new DataTable("newlocation");
            DataTable dtAssets = new DataTable("newassets");
            DataTable dtFS = new DataTable("newfs");
            DataTable dtResult = new DataTable("result");

            #region "New Employee Add"
            ProgressStatus.increment();
            dtEmployee = Employee.getNewRows();
            ProgressStatus.increment();
            if (dtEmployee.Rows.Count > 0)
            {
                ProgressStatus.increment();
                dtResult = syn.AddEmployee(dtEmployee);
                //D 'ProgressStatus.pauseAutoIncrement(); 

                using (CEConn localDB = new CEConn())
                {
                    //DataRow[] errRow;
                    //errRow = dtResult.Select("RowStatus=" + Convert.ToInt32(RowStatus.Error));    
                    foreach (DataRow dr in dtResult.Rows)
                    {
                        ProgressStatus.increment();
                        if (Convert.ToInt32(dr["RowStatus"]) == Convert.ToInt32(RowStatus.Error))
                        {
                            DataRow[] drArr;
                            drArr = dtEmployee.Select("ServerKey='" + dr["ServerKey"] + "'");
                            if (drArr.Length > 0)
                                strError += "Employee " + drArr[0]["Name"].ToString() + "not added.\r";
                            strSql = "update Employees set RowStatus=" + Convert.ToInt32(RowStatus.Error) + " where ServerKey=" + Convert.ToInt32(dr["ServerKey"]);
                            localDB.runQuery(strSql);
                        }
                        else
                        {
                            strSql = "update Employees set RowStatus=" + Convert.ToInt32(RowStatus.Synchronized) + ", ServerKey=" + Convert.ToInt32(dr["PKey"]) + " where ServerKey=" + Convert.ToInt32(dr["ServerKey"]);
                            localDB.runQuery(strSql);

                            //for location
                            strSql = "update Locations set ModifiedBy=" + Convert.ToInt32(dr["PKey"]) + " where ModifiedBy=" + Convert.ToInt32(dr["ServerKey"]);
                            localDB.runQuery(strSql);

                            //For Assets
                            strSql = "update Assets set ModifiedBy=" + Convert.ToInt32(dr["PKey"]) + " where ModifiedBy=" + Convert.ToInt32(dr["ServerKey"]);
                            localDB.runQuery(strSql);

                            //For Inventory
                            strSql = "update inventory set ModifiedBy=" + Convert.ToInt32(dr["PKey"]) + " where ModifiedBy=" + Convert.ToInt32(dr["ServerKey"]);
                            localDB.runQuery(strSql);

                        }
                    }
                    strSql = "delete from Employees where RowStatus= " + Convert.ToInt32(RowStatus.Error);
                    localDB.runQuery(strSql);

                }
            }
            #endregion 

            #region "New Location Add"


            dtLocation = Locations.getNewRows();

            #region "New Dispatch Location Add"
            if (dtLocation.Rows.Count > 0)
            {
                DataRow[] dispatchRow = dtLocation.Select("tagID = '" + "0000000000000000000000D" + "'");
                if (dispatchRow != null && dispatchRow.Length > 0)
                {
                    string dispatchLocationID = Locations.getDispatchLocation();

                    if (dispatchLocationID != "")
                    {
                        int serverKey = Convert.ToInt32(dispatchRow[0]["ServerKey"]);
                        using (CEConn localDB = new CEConn())
                        {
                            strSql = "update Locations set RowStatus=" + Convert.ToInt32(RowStatus.Synchronized) + ", ServerKey=" + Convert.ToInt32(dispatchLocationID) + " where ServerKey=" + serverKey;
                            localDB.runQuery(strSql);

                            //For Assets
                            strSql = "update Assets set ID_Location=" + Convert.ToInt32(dispatchLocationID) + " where ID_Location=" + serverKey;
                            localDB.runQuery(strSql);

                            //For Inventory
                            strSql = "update inventory set ID_Location=" + Convert.ToInt32(dispatchLocationID) + " where ID_Location=" + serverKey;
                            localDB.runQuery(strSql);

                        }

                        dtLocation.Rows.Remove(dispatchRow[0]);
                    }

                }
            }
            #endregion


            if (dtLocation.Rows.Count > 0)
            {               

                ProgressStatus.increment();
                dtResult = syn.AddLocation(dtLocation);
                //D 'ProgressStatus.pauseAutoIncrement(); 

                using (CEConn localDB = new CEConn())
                {
                    //DataRow[] errRow;
                    //errRow = dtResult.Select("RowStatus=" + Convert.ToInt32(RowStatus.Error));    
                    foreach (DataRow dr in dtResult.Rows)
                    {
                        ProgressStatus.increment();
                        if (Convert.ToInt32(dr["RowStatus"]) == Convert.ToInt32(RowStatus.Error))
                        {
                            DataRow[] drArr;
                            drArr = dtLocation.Select("ServerKey='" + dr["ServerKey"] + "'");
                            if (drArr.Length > 0)
                                strError += "Location " + drArr[0]["Name"].ToString() + "not added.\r";

                            strSql = "update Locations set RowStatus=" + Convert.ToInt32(RowStatus.Error) + " where ServerKey=" + Convert.ToInt32(dr["ServerKey"]);
                            localDB.runQuery(strSql);
                        }
                        else
                        {
                            strSql = "update Locations set RowStatus=" + Convert.ToInt32(RowStatus.Synchronized) + ", ServerKey=" + Convert.ToInt32(dr["PKey"]) + " where ServerKey=" + Convert.ToInt32(dr["ServerKey"]);
                            localDB.runQuery(strSql);

                            //For Assets
                            strSql = "update Assets set ID_Location=" + Convert.ToInt32(dr["PKey"]) + " where ID_Location=" + Convert.ToInt32(dr["ServerKey"]);
                            localDB.runQuery(strSql);

                            //For Inventory
                            strSql = "update inventory set ID_Location=" + Convert.ToInt32(dr["PKey"]) + " where ID_Location=" + Convert.ToInt32(dr["ServerKey"]);
                            localDB.runQuery(strSql);
                        }
                    }
                    strSql = "delete from Locations where RowStatus= " + Convert.ToInt32(RowStatus.Error);
                    localDB.runQuery(strSql);

                }
            }
            #endregion

            #region "Modified Locations"

            dtLocation = null;

            dtLocation = Locations.getModifiedRows();
            if (dtLocation != null && dtLocation.Rows.Count > 0)
            {
                ProgressStatus.increment();
                // dtResult = syn.AddLocation(dtLocation);
                //D 'ProgressStatus.pauseAutoIncrement(); 

                using (CEConn localDB = new CEConn())
                {
                    //DataRow[] errRow;
                    //errRow = dtResult.Select("RowStatus=" + Convert.ToInt32(RowStatus.Error));    
                    foreach (DataRow dr in dtLocation.Rows)
                    {
                        ProgressStatus.increment();

                        try
                        {
                            Locations.EditLocation(Convert.ToString(dr["TagID"]), Convert.ToString(dr["LocationNo"]), Convert.ToString(dr["Name"]), Convert.ToInt32(dr["ServerKey"]), Convert.ToInt32(dr["OfflineUsageCount"]));

                            strSql = "update Locations set RowStatus=" + Convert.ToInt32(RowStatus.Synchronized) + " where ServerKey=" + Convert.ToInt32(dr["ServerKey"]);
                            localDB.runQuery(strSql);

                        }
                        catch (Exception ex)
                        {
                            Logger.LogError("Sync Operation - Location Modify " + ex.Message + "\n" + ex.StackTrace);
                            strError += "Location " + dr["Name"].ToString() + " not Modified.\r";
                            strSql = "update Locations set RowStatus=" + Convert.ToInt32(RowStatus.Error) + " where ServerKey=" + Convert.ToInt32(dr["ServerKey"]);
                            localDB.runQuery(strSql);
                        }

                    }

                    strSql = "delete from Locations where RowStatus= " + Convert.ToInt32(RowStatus.Error);
                    localDB.runQuery(strSql);

                }
            }
            using (CEConn localDB = new CEConn())
            {

                strSql = "Update Locations SET OfflineUsageCount = 0";
                localDB.runQuery(strSql);
            }

            #endregion

            #region "New Asset Add"

            dtAssets = Assets.getNewRows();
            if (dtAssets.Rows.Count > 0)
            {
                ProgressStatus.increment();
                dtResult = syn.AddAssets(dtAssets);
                //D 'ProgressStatus.pauseAutoIncrement(); 

                using (CEConn localDB = new CEConn())
                {
                    foreach (DataRow dr in dtResult.Rows)
                    {
                        ProgressStatus.increment();
                        if (Convert.ToInt32(dr["RowStatus"]) == Convert.ToInt32(RowStatus.Error))
                        {
                            DataRow[] drArr;
                            drArr = dtAssets.Select("ServerKey='" + dr["ServerKey"] + "'");
                            if (drArr.Length > 0)
                                strError += "Asset " + drArr[0]["Name"].ToString() + "not added.\r";

                            strSql = "delete from Assets where ServerKey=" + Convert.ToInt32(dr["ServerKey"]);
                           // strSql = "update Assets set RowStatus=" + Convert.ToInt32(RowStatus.Error) + " where ServerKey=" + Convert.ToInt32(dr["ServerKey"]);
                            localDB.runQuery(strSql);
                        }
                        else
                        {
                            strSql = "update Assets set RowStatus=" + Convert.ToInt32(RowStatus.Synchronized) + ", ServerKey=" + Convert.ToInt32(dr["PKey"]) + " where ServerKey=" + Convert.ToInt32(dr["ServerKey"]);
                            localDB.runQuery(strSql);
                        }
                    } 

                }
            }
            #endregion

            #region "Asset Modify"
            dtAssets = Assets.getRows(RowStatus.Modify);
            if (dtAssets.Rows.Count > 0)
            {
                ProgressStatus.increment();
                dtResult = syn.ModifyAssets(dtAssets);
                //ProgressStatus.pauseAutoIncrement(); 

                using (CEConn localDB = new CEConn())
                {
                    foreach (DataRow dr in dtResult.Rows)
                    {
                        ProgressStatus.increment();
                        if (Convert.ToInt32(dr["RowStatus"]) == Convert.ToInt32(RowStatus.Error))
                        {
                            DataRow[] drArr;
                            drArr = dtAssets.Select("ServerKey='" + dr["ServerKey"] + "'");
                            if (drArr.Length > 0)
                                strError += "Tag no " + drArr[0]["TagID"].ToString() + "not update.\r";

                            strSql = "update Assets set RowStatus=" + Convert.ToInt32(RowStatus.Error) + " where ServerKey=" + Convert.ToInt32(dr["ServerKey"]);
                            localDB.runQuery(strSql);
                        }
                        else
                        {
                            strSql = "update Assets set RowStatus=" + Convert.ToInt32(RowStatus.Synchronized) + ", ServerKey=" + Convert.ToInt32(dr["PKey"]) + " where ServerKey=" + Convert.ToInt32(dr["ServerKey"]);
                            localDB.runQuery(strSql);
                        }
                    }
                    strSql = "delete from Assets where RowStatus= " + Convert.ToInt32(RowStatus.Error);
                    localDB.runQuery(strSql);
                }
            }
            #endregion

            #region"Tag Write functionality"
            dtAssets = Assets.getRows(RowStatus.TagWrite);
            if (dtAssets.Rows.Count > 0)
            {
                ProgressStatus.increment();
                dtResult = syn.TagWrite(dtAssets);
                //D 'ProgressStatus.pauseAutoIncrement(); 

                using (CEConn localDB = new CEConn())
                {
                    foreach (DataRow dr in dtResult.Rows)
                    {
                        ProgressStatus.increment();
                        if (Convert.ToInt32(dr["RowStatus"]) == Convert.ToInt32(RowStatus.Error))
                        {
                            DataRow[] drArr;
                            drArr = dtAssets.Select("ServerKey='" + dr["ServerKey"] + "'");
                            if (drArr.Length > 0)
                                strError += "Tag no " + drArr[0]["TagID"].ToString() + "not write.\r";

                            strSql = "update Assets set RowStatus=" + Convert.ToInt32(RowStatus.Error) + " where ServerKey=" + Convert.ToInt32(dr["ServerKey"]);
                            localDB.runQuery(strSql);
                        }
                        else
                        {
                            strSql = "update Assets set RowStatus=" + Convert.ToInt32(RowStatus.Synchronized) + ", ServerKey=" + Convert.ToInt32(dr["PKey"]) + " where ServerKey=" + Convert.ToInt32(dr["ServerKey"]);
                            localDB.runQuery(strSql);
                        }
                    }
                    strSql = "delete from Assets where RowStatus= " + Convert.ToInt32(RowStatus.Error);
                    localDB.runQuery(strSql);
                }
            }
            #endregion

            #region "FieldServicePosting"
            dtFS = FS_AssetStatus.getRows(RowStatus.Modify);
            if (dtFS.Rows.Count > 0)
            {
                ProgressStatus.increment();
                dtResult = syn.UpdateFS_AssetStatus(dtFS);
                using (CEConn localDB = new CEConn())
                {
                    foreach (DataRow dr in dtResult.Rows)
                    {
                        ProgressStatus.increment();
                        if (Convert.ToInt32(dr["RowStatus"]) == Convert.ToInt32(RowStatus.Error))
                        {
                            DataRow[] drArr;
                            drArr = dtFS.Select("ServerKey='" + dr["ServerKey"] + "'");
                            if (drArr.Length > 0)
                            {
                                //Assets ast = new Assets(drArr[0]["TagID"].ToString());
                                strError += "FS Task for Item " + drArr[0]["Name"].ToString() + " not updated.\r";
                            }

                            strSql = "update FS_AssetStatus set RowStatus=" + Convert.ToInt32(RowStatus.Error) + " where ServerKey=" + Convert.ToInt32(dr["ServerKey"]);
                            localDB.runQuery(strSql);
                        }
                        else
                        {
                            //Instead of Update just delete field service data.
                            //strSql = "update FS_AssetStatus set RowStatus=" + Convert.ToInt32(RowStatus.Synchronized) + ", ServerKey=" + Convert.ToInt32(dr["PKey"]) + " where ServerKey=" + Convert.ToInt32(dr["ServerKey"]);
                            strSql = "delete from FS_AssetStatus where ServerKey=" + Convert.ToInt32(dr["ServerKey"]);
                            localDB.runQuery(strSql);
                        }
                    }
                    strSql = "delete from FS_AssetStatus where RowStatus= " + Convert.ToInt32(RowStatus.Error);
                    localDB.runQuery(strSql);
                }
            }
            #endregion

            //Run the query and fetch all the data and OverWrite
            DateTime lstSyncedDate;

            StringBuilder sqlQueries;
            StringBuilder keys;

            SqlCeBulkCopyOptions options = new SqlCeBulkCopyOptions();
            options = SqlCeBulkCopyOptions.KeepIdentity;
            options = options |= SqlCeBulkCopyOptions.KeepNulls;

            ArrayList delItems;
            int rowsCount = 0;

            foreach (String tbname in tblArr)
            {
                ProgressStatus.increment();
                lstSyncedDate = Util.getLastSyncedDate(tbname);
                lstSyncedDate = lstSyncedDate.AddSeconds(1); 
                DateTime outDate = lstSyncedDate;
                String errMsg;
                ProgressStatus.increment();
                Int64 ItemLimit = 0;
                ItemLimit = Login.ItemLimit - Assets.getNoOfItems();
                if (ItemLimit == 0)
                    ItemLimit = -1;
                dtResult = (DataTable)syn.SyncTableswithItemLimit(lstSyncedDate.ToString(), tbname, ItemLimit, out outDate);
                //D 'ProgressStatus.pauseAutoIncrement(); 
                try
                {
                    if (dtResult != null && dtResult.Rows.Count > 0)
                    {
                        // Util.updateTables(tbname, dr, outDate, ref sqlQueries);

                        //DataColumn dc, dcc;
                        //dc = dtResult.Columns["Date_Modified"];
                        //if (dc == null)
                        //{
                        //    dc = new DataColumn("Date_Modified");
                        //    dc.DefaultValue = outDate;
                        //    dtResult.Columns.Add(dc);
                        //}
                        //else
                        //    dc.DefaultValue = outDate;

                        //dcc = dtResult.Columns["RowStatus"];
                        //if (dcc == null)
                        //{
                        //    dcc = new DataColumn("RowStatus", Type.GetType("System.Int16"));
                        //    dcc.DefaultValue = Convert.ToInt16(RowStatus.Synchronized);
                        //    dtResult.Columns.Add(dcc);
                        //}
                        //else
                        //    dcc.DefaultValue = Convert.ToInt32(RowStatus.Synchronized);

                        //dtResult.AcceptChanges();

                        keys = new StringBuilder("-1");

                        delItems = new ArrayList();
                        rowsCount = 0;

                        //sqlQueries = new StringBuilder();

                        // check if the table has any rows 

                        int count = Util.getRecordCount(tbname);

                        if (count > 0)
                        {
                            foreach (DataRow dr in dtResult.Rows)
                            {   

                                rowsCount++;

                                if (rowsCount >= 800)
                                {
                                    delItems.Add(keys.ToString());
                                    keys = new StringBuilder("-1");
                                    rowsCount = 0;
                                }

                                if (dr["ServerKey"] != DBNull.Value)
                                    keys.Append("," + dr["ServerKey"].ToString());

                            }

                            if (rowsCount > 0)
                                delItems.Add(keys.ToString());

                            try
                            {
                                using (CEConn localDB = new CEConn())
                                {
                                    try
                                    {
                                        foreach (string delList in delItems)
                                        {
                                            strSql = "delete from " + tbname + " where serverKey IN (" + delList + ");";
                                            localDB.runQuery(strSql);
                                            localDB.commitTransaction();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        localDB.rollbackTransaction();
                                        strError += "Error while deleting records in table " + tbname + ".\n";
                                        Logger.LogError(ex.Message);
                                    }

                                }

                            }
                            catch (Exception ex)
                            {
                                // strError += "\n";
                                Logger.LogError(ex.Message);
                            }

                        } 

                        try
                        {
                            int totalRows = 0;
                            using (CEConn localDB = new CEConn())
                            {
                                SqlCeBulkCopy bc = new SqlCeBulkCopy(localDB.CEConnection, options);
                                bc.DestinationTableName = tbname;
                               // totalRows = bc.WriteToServer(dtResult, 0, "");
                                totalRows = bc.WriteToServerNew(dtResult, 0, "");

                            }
                        }
                        catch (Exception ex)
                        {
                            strError += "Error while inserting records in table " + tbname + ".\n";
                            Logger.LogError("Error while inserting records in table " + tbname + ".\n" + ex.Message);
                        }

                    }

                    //Update Date_Modified of the table to Today.Now. 
                    using (CEConn localDB = new CEConn())
                    {
                        try
                        {
                            strSql = "update " + tbname + " set Date_Modified = getdate();";
                            localDB.runQuery(strSql);
                            localDB.commitTransaction();
                        }
                        catch { }
                    }

                }
                catch (System.Web.Services.Protocols.SoapException ex)
                {

                    if (ex.Actor.ToString().ToLower().Trim() == "getlogin")
                        strError += "Request from innvalid IP address.\n";
                    else
                        strError += "Soap Exception\n";

                    Logger.LogError(ex.Message);

                }
                catch (Exception ep)
                {
                    strError += ep.ToString() + "\n";
                    Logger.LogError(ep.Message);
                }

                //Update the Row Status of Table
                //Call Web Service
                //If got dataset delete all rows of table 
                //Insert new Row in table
            }

            //Sync Inventory is differen process need no i
            ProgressStatus.finish();
            return strError;

        }


        /// <summary>
        ///Sync Inventory 
        /// </summary>
        /// <returns></returns>
        /// 

        public DataTable SyncFieldService()
        {
            DataTable dtResult = new DataTable("result");
            try
            {
                ProgressStatus.minVal = 5;
                ProgressStatus.maxVal = 100;
                //System.Threading.Thread pr = new System.Threading.Thread(ProgressStatus.autoStart);
                //pr.Start();
                //ProgressStatus.autostart();  
                string strSql = "";
                DataTable dtFS = new DataTable("dtFS");

                dtFS = Task.getTaskPerformedRows(RowStatus.New);
                ProgressStatus.increment();
                if (dtFS.Rows.Count > 0)
                {
                    dtResult = syn.FieldServiceCheck(dtFS);
                    ProgressStatus.increment();
                    if (dtResult.Rows.Count > 0)
                    {
                        using (CEConn localDB = new CEConn())
                        {
                            strSql = "delete from PerformedTasks";
                            localDB.runQuery(strSql);
                        }
                    }
                }

            }
            catch (System.Web.Services.Protocols.SoapException ex)
            {
                if (ex.Actor.ToString().ToLower().Trim() == "getlogin")
                    throw new ApplicationException("Invalid Access");
                else
                    throw ex;
            }
            catch (Exception ep)
            {
                throw ep;
            }
            finally
            {
                ProgressStatus.finish();
            }
            return dtResult;
        }

        //Currently Running
        public string SyncFieldServiceData(Int64 locationID)
        {
            strError = "";
            ProgressStatus.minVal = 5;
            ProgressStatus.maxVal = 100;
            DataTable dtResult = new DataTable("result");
            ProgressStatus.increment();
            String errMsg;
            string strSql;
            string modiBy;
            string displayName = "";
            ProgressStatus.increment();
            dtResult = (DataTable)syn.GetFSbyLocation(locationID);
            //D 'ProgressStatus.pauseAutoIncrement(); 
            try
            {
                CEConn localDB;
                using (localDB = new CEConn())
                {
                    //strSql = "select count(*) from FS_AssetStatus where RowStatus = " + Convert.ToInt32(RowStatus.Modify)";
                    if (locationID <= 0)
                    {
                        strSql = "delete from FS_AssetStatus where (RowStatus <> " + Convert.ToInt32(RowStatus.New) + " and RowStatus <> " + Convert.ToInt32(RowStatus.Modify) + ") or ID_Asset not in(select id_Asset from Assets) ";
                    }
                    else
                    {
                        strSql = "delete from FS_AssetStatus where RowStatus <> " + Convert.ToInt32(RowStatus.Modify) + " or ID_Asset not in(select id_Asset from Assets) ";
                    }
                    localDB.runQuery(strSql);
                }

                if (dtResult != null)
                {

                    foreach (DataRow dRow in dtResult.Rows)
                    {
                        using (localDB = new CEConn())
                        {
                            ProgressStatus.increment();
                            strSql = "";
                            modiBy = "";
                            try
                            {

                                strSql = "delete from FS_AssetStatus where (ID_Tasks = " + dRow["ID_Tasks"] + " and ID_Template = " + dRow["ID_Template"] + " and ID_Asset = " + dRow["ID_Asset"] + " and FSStatus=" + Convert.ToInt32(FSStatusType.New) + ") or serverKey=" + dRow["ServerKey"];
                                localDB.runQuery(strSql);

                                if ((dRow["Is_Deleted"] == DBNull.Value) || (Convert.ToInt32(dRow["Is_Deleted"]) == 0) || (Convert.ToInt32(dRow["Is_Active"]) == 1))
                                {
                                    if (dRow["ModifiedBy"] != DBNull.Value)
                                        modiBy = Convert.ToString(dRow["ModifiedBy"]);
                                    else
                                        modiBy = "0";

                                    strSql = "insert into FS_AssetStatus(ID_Tasks,ID_Asset,ID_Template,DueDate,TaskStatus,FSStatus,ID_Employee,ServerKey,ModifiedBy,RowStatus) ";
                                    strSql += " values(" + dRow["ID_Tasks"] + "," + dRow["ID_Asset"] + "," + dRow["ID_Template"] + ",'" + Convert.ToDateTime(dRow["DueDate"]).ToString("yyyy-MM-dd hh:mm:ss tt") + "'," + dRow["TaskStatus"] + "," + dRow["FSStatus"] + "," + dRow["ID_Employee"] + "," + dRow["ServerKey"] + "," + modiBy + "," + Convert.ToInt32(RowStatus.Synchronized) + ")";

                                    displayName = dRow["ServerKey"].ToString();

                                    localDB.runQuery(strSql);
                                }

                            }

                            catch (System.Data.SqlServerCe.SqlCeException exps)
                            {
                                //errStr = tblName + " " + displayName + " not synchronized\r";
                                strError += exps.ToString() + "Error occured in ServerKey " + displayName + ".\r";
                                Logger.LogError(exps.ToString() + "Error occured in ServerKey " + displayName);
                            }
                            catch (Exception ep)
                            {
                                throw ep;
                            }
                        }
                    }
                }



                if (strError.Trim().Length != 0)
                {
                    throw new ApplicationException("Error Occured in Data " + strError);
                }

            }
            catch (System.Web.Services.Protocols.SoapException ex)
            {
                if (ex.Actor.ToString().ToLower().Trim() == "getlogin")
                {
                    throw new ApplicationException("Request from innvalid IP address.");
                    //errMsg = "Request from innvalid IP address.";
                }
                else
                {
                    throw ex;
                    //errMsg = "Soap Exception";
                }
            }
            catch (Exception ep)
            {
                throw ep;
                //errMsg = ep.ToString();
            }
            ProgressStatus.finish();
            return "";

        }

        #endregion
    }
}
