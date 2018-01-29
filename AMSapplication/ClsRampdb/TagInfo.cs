using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using ClsLibBKLogs;

namespace ClsRampdb
{
    public class TagInfo
    {
        String _TagID;
        Boolean _isLocationTag;
        Boolean _isEmployeeTag;
        Boolean _isDocumentTag;
        Boolean _isAssetTag;
        Int32 _DataKey;
        //Int32 _AssetID;

        public Int32 DataKey
        {
            get
            {
                return _DataKey;
            }
        }

        public TagInfo(String TagID)
        {
            _TagID = TagID;
        }

        public bool isLocationTag()
        {
            
            if (_isEmployeeTag)
                return false;

            if (_isLocationTag == true)
                return _isLocationTag;

            if (_isAssetTag == true)
                return false; 

            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = " select serverKey as ID_Location from Locations where TagID='" + _TagID + "'";
                    _DataKey = Convert.ToInt32(localDB.getScalerValue(strSql));

                } 
            }
            else
            {
                RConnection.RConnection OnConn = new RConnection.RConnection();
                OnConn.Url = Login.webConURL;
                string strSql;
                try
                {
                    strSql = " select ID_Location from Locations where TagID='" + _TagID + "'";
                    string oStr = OnConn.getScalerValue(strSql);
                    if (oStr.ToString().Length != 0)
                        _DataKey = Convert.ToInt32(oStr);
                    else
                        _DataKey = 0;
                    
                }
                catch (Exception ex)
                {
                    strSql = ex.ToString();
                    _DataKey = 0;
                    Logger.LogError(ex.Message);
                }
                //    throw new ApplicationException("Online functionality not implemented yet."); 
            }

            if (_DataKey != 0)
            {
                _isLocationTag = true;
                return true;
            }
            else
            {
                //We can assign DataKey here from Asset table.
                _isLocationTag = false;
                return false;
            }

        }

        public bool isEmployeeTag()
        {
            if (_isLocationTag)
                return false;

            if (_isEmployeeTag == true)
                return _isEmployeeTag;

            if (_isAssetTag == true)
                return false; 

            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = " select serverKey as ID_Employee from Employees where TagID='" + _TagID + "'";
                    _DataKey = Convert.ToInt32(localDB.getScalerValue(strSql));
                }
            }
            else
            {
                RConnection.RConnection OnConn = new RConnection.RConnection();
                OnConn.Url = Login.webConURL;
                string strSql;
                strSql = " select ID_Employee from Employees where TagID='" + _TagID + "' and Is_Active=1 and Is_Deleted=0";
                String strVal = OnConn.getScalerValue(strSql);
                if (strVal.Trim().Length != 0)
                    _DataKey = Convert.ToInt32(strVal);
                else
                    _DataKey = 0;
                //throw new ApplicationException("Online functionality not implemented yet.");
            }

            if (_DataKey != 0)
            {
                _isEmployeeTag = true;
                return true;
            }
            else
            {
                //We can assign DataKey here from Asset table.
                _isEmployeeTag = false;
                return false;
            }
        }

        public bool isDocumentTag()
        {
            if (_isDocumentTag)
                return true; 

            if (_isEmployeeTag)
                return false;

            if (_isAssetTag == true)
                return false; 

            if (_isLocationTag == true)
                return false; 

            
            using (CEConn localDB = new CEConn())
            {
                string strSql;
                strSql = " select serverKey as DocumentID from Documents where TagID='" + _TagID + "'";
                _DataKey = Convert.ToInt32(localDB.getScalerValue(strSql));
            }
            
            if (_DataKey != 0)
            {
                _isDocumentTag = true;
                return true;
            }
            else
            {
                //We can assign DataKey here from Asset table.
                _isDocumentTag = false;
                return false;
            }

        }

        public bool isAssetTag()
        {
            if (_isLocationTag)
                return false;

            if (_isEmployeeTag == true)
                return false;

            if (_isAssetTag == true)
                return true; 

            if (!Login.OnLineMode)
            {
                using (CEConn localDB = new CEConn())
                {
                    string strSql;
                    strSql = " select serverKey as ID_Asset from Assets where TagID='" + _TagID + "'";
                    _DataKey = Convert.ToInt32(localDB.getScalerValue(strSql));
                }
            }
            else
            {
                RConnection.RConnection OnConn = new RConnection.RConnection();
                OnConn.Url = Login.webConURL;
                string strSql;
                strSql = " select ID_Asset from Assets where TagID='" + _TagID + "' and Is_Active=1 and Is_Deleted=0";
                String strVal = OnConn.getScalerValue(strSql);
                if (strVal.Trim().Length != 0)
                    _DataKey = Convert.ToInt32(strVal);
                else
                    _DataKey = 0;
                //throw new ApplicationException("Online functionality not implemented yet.");
            }

            if (_DataKey != 0)
            {
                _isAssetTag = true;
                return true;
            }
            else
            {
                //We can assign DataKey here from Asset table.
                _isAssetTag = false;
                return false;
            }
        }

    }
}
