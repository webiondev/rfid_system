using System;
using System.Collections.Generic;
using System.Text;

namespace ClsLibBKLogs
{
    public class LocationItem
    {
        public string Name { get; set; }
        public string ID_Location { get; set; }

        public LocationItem(string _Name, string _Id_Location)
        {
            Name = _Name;
            ID_Location = _Id_Location;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public enum RowStatus
    {

        New = 10,
        TagWrite = 20,
        Inventory = 30,
        InProcess = 40,
        Synchronized = 50,
        Modify = 60,
        Dispatched = 61,
        Inventory_Mismatch = 98,
        Error = 99,
        None = 100,
        SyncNew = 201,
        SyncModify = 202,
        SyncDelete = 203

        //New=10,
        //TagWrite=20,
        //Inventory=30,
        //InProcess=40,
        //Synchronized=50,
        //Error=99
    }

    public enum ScanStatus
    {
        All = 0,
        Scanned = 1,
        Missing = 2
    }

    public enum FSStatusType
    {
        //Donot change the code it is also using in SPs
        New = 1010,
        InProcess = 1020,
        Completed = 1030,
        Expired = 1040,
        Cancel = 1050,
        Ignored = 1060
    }

    //public enum AppModules
    //{
    //    //Donot change the code it is also using in SPs
    //    ALL = 1,
    //    RemoveDispatch = 2,
    //    RemoveFieldService = 3,
    //    RemoveDispatchandFS=4
    //}

    public struct AppModules
    {
        //Donot change the code it is also using in SPs
        //public const bool RemoveDispatch = true; //Enable Disable Dispatch Functionality.      
        //public const bool RemoveFieldService = true; //Enable Disable FieldService Functionality
        //public const bool ImmunoSearch = false; //Changes the form in Search Process
        //public const bool VernonFlag = true; // It forces the selection of Reason in Inventory

        public static bool RemoveDispatch = true; //Enable Disable Dispatch Functionality.      
        public static bool RemoveFieldService = true; //Enable Disable FieldService Functionality
        public static bool ImmunoSearch = false; //Changes the form in Search Process
        public static bool VernonFlag = true; // It forces the selection of Reason in Inventory

    } 

    public static class ProgressStatus
    {
        public delegate void progressStateHandler(Int64 status);
        public static event progressStateHandler progressState;

        private static Int32 _Status = 0;
        public static Int32 maxVal = 100;
        public static Int32 minVal = 10;
        private static bool autoStartMode = false;
        private static bool _pauseIncrement = false;

        public static Int32 Status
        {
            set
            {
                _Status = value;
            }
        }

        private static void changeprogressState(Int64 status)
        {
            if (progressState != null)
                progressState(status);

        }

        public static void start()
        {
            _Status = minVal;
            changeprogressState(minVal);
        }

        public static void autoStart()
        {
            autoStartMode = true;
            _Status = minVal;
            changeprogressState(minVal);
            while (autoStartMode)
            {
                if (!_pauseIncrement)
                    increment();

                System.Threading.Thread.Sleep(100);
            }
        }

        public static void increment()
        {
            _Status = _Status + minVal;
            if (_Status > maxVal)
                _Status = minVal;
            changeprogressState(_Status);
        }

        public static void pauseAutoIncrement()
        {
            _pauseIncrement = true;
        }

        public static void resumeAutoIncrement()
        {
            _pauseIncrement = false;
        }

        public static void finish()
        {
            _pauseIncrement = false;
            autoStartMode = false;
            _Status = maxVal;
            changeprogressState(maxVal);
        }

    }
}
