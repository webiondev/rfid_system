using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;  
using RFID_RADIO_HANDLE = System.UInt32;
using HRESULT_RFID_STATUS = ReaderTypes.HRESULT_RFID;
using System.IO;

namespace ReaderTypes
{
    public partial class Datalog
    {
        private static StreamWriter logWri;
        private static StreamWriter errWri;
        public static String ErrMsg = null;
        public Datalog() { }
        private const String ClslibLogFileName = "CsLog.txt";
        private const String ClslibErrLogFileName = "CsErrorLog.txt";
        private const String DefaultLogDir = @"\RfidLog";

        public static bool TraceIsOn
        {
            get
            {
                return logWri != null;
            }
        }

        public static bool LogInit(string logDirPath)
        {
            try
            {
                if (!Directory.Exists(logDirPath))
                    Directory.CreateDirectory(logDirPath);
                /// Open ClsLib Log File
                // use Create FileMode to 'truncate' existing file
                FileStream logFileStm = File.Open(logDirPath + ClslibLogFileName,
                    FileMode.Create, FileAccess.Write, FileShare.Read);
                logWri = new StreamWriter(logFileStm);
                logWri.AutoFlush = true;
                logWri.NewLine = "\r\n";
                logWri.WriteLine("Open log");
                /// Open W_RfidSp Log File
                HRESULT_RFID hRes;
                //hRes = CF.f_CFlow_StartLogTrace(logDirPath);
                //if (!SP.SUCCEEDED(hRes))
                //    throw new ApplicationException("W_RfidSp Log Trace error - " + hRes.ToString());
            }
            catch (Exception e)
            {
                ErrMsg = ("Unable to start Log Trace:\r\n" + e.Message).ToString();
                if (logWri != null)
                    logWri.Close();
                logWri = null;
            }
            return (logWri != null);
        }

        private static bool ErrLogInit(string logDirPath)  /// "\\Rfid\\CsLog.txt"
        {
            try
            {
                if (!Directory.Exists(logDirPath))
                    Directory.CreateDirectory(logDirPath);
                /// Open ClsLib ErrLog File
                // use Create FileMode to 'truncate' existing file
                FileStream logFileStm = File.Open(logDirPath + Path.DirectorySeparatorChar + ClslibErrLogFileName,
                    FileMode.Create, FileAccess.Write, FileShare.Read);
                errWri = new StreamWriter(logFileStm);
                errWri.AutoFlush = true;
                errWri.NewLine = "\r\n";
                errWri.WriteLine("Open error log");
            }
            catch (Exception e)
            {
                ErrMsg = ("Unable to start Error Log Trace:\r\n" + e.Message).ToString();
                if (errWri != null)
                    errWri.Close();
                errWri = null;
            }
            return (errWri != null);
        }

        public static int LogUninit()
        {
            if (logWri != null)
            {
                try
                {
                    logWri.WriteLine("Close log");
                    logWri.Close();
                   // CF.f_CFlow_StopLogTrace();
                }
                catch (Exception e)
                {
                    ErrMsg = ("Unable to stop Log Trace:\r\n" + e.Message).ToString();
                }
                finally
                {
                    logWri = null;
                }
            }
            return 0;
        }

        public static int ErrLogUninit()
        {
            if (errWri != null)
            {
                try
                {
                    errWri.WriteLine("Close Error log");
                    errWri.Close();
                }
                catch (Exception e)
                {
                    ErrMsg = ("Unable to stop Log Trace:\r\n" + e.Message).ToString();
                }
                finally
                {
                    errWri = null;
                }
            }
            return 0;
        }

        public static void LogStr(string str)
        {
            if (logWri != null)
            {
                logWri.WriteLine("(" + DateTime.Now.ToShortDateString()
                    + " " + DateTime.Now.ToShortTimeString() + ") " + str);
            }
        }

        public static void LogStr1(string str)
        {
            if (logWri != null)
            {
                LogStr(str);
            }
        }

        // create Error Log File on demand (regardless of UserPref.TraceLog option)
        public static void LogErr(string str)
        {
            if (errWri == null)
            {
                if (ErrLogInit(DefaultLogDir) == false)
                    return;
            }
            errWri.WriteLine("(" + DateTime.Now.ToShortDateString()
                + " " + DateTime.Now.ToShortTimeString() + ") " + str);
        }
    }
    public partial class Rfid
    {
        public static int LOGICAL_ANTENNA_COUNT = 16;

        /////////////////////////////////////////////////////////////
        public static RFID_Startup_T st_RfidSpReq_Startup;
        public static RFID_Shutdown_T st_RfidSpReq_Shutdown;
        public static RFID_RetrieveAttachedRadiosList_T st_RfidSpReq_RetrieveAttachedRadiosList;
        public static RFID_RadioOpen_T st_RfidSpReq_RadioOpen;
        public static RFID_RadioClose_T st_RfidSpReq_RadioClose;
        public static RFID_RadioGetSetConfigurationParameter_T st_RfidSpReq_RadioSetConfigurationParameter;
        public static RFID_RadioGetSetConfigurationParameter_T st_RfidSpReq_RadioGetConfigurationParameter;
        public static RFID_RadioGetSetOperationMode_T st_RfidSpReq_RadioSetOperationMode;
        public static RFID_RadioGetSetOperationMode_T st_RfidSpReq_RadioGetOperationMode;
        public static RFID_RadioGetSetPowerState_T st_RfidSpReq_RadioSetPowerState;
        public static RFID_RadioGetSetPowerState_T st_RfidSpReq_RadioGetPowerState;
        public static RFID_RadioGetSetCurrentLinkProfile_T st_RfidSpReq_RadioSetCurrentLinkProfile;
        public static RFID_RadioGetSetCurrentLinkProfile_T st_RfidSpReq_RadioGetCurrentLinkProfile;
        public static RFID_RadioGetLinkProfile_T st_RfidSpReq_RadioGetLinkProfile;
        public static RFID_AntennaPortGetStatus_T[] st_RfidSpReq_AntennaPortGetStatus;
        public static RFID_AntennaPortSetState_T st_RfidSpReq_AntennaPortSetState;
        public static RFID_AntennaPortGetSetConfiguration_T st_RfidSpReq_AntennaPortSetConfiguration;
        public static RFID_AntennaPortGetSetConfiguration_T[] st_RfidSpReq_AntennaPortGetConfiguration;
        public static RFID_18K6CSetSelectCriteria_T st_RfidSpReq_18K6CSetSelectCriteria;
        public static RFID_18K6CGetSelectCriteria_T st_RfidSpReq_18K6CGetSelectCriteria;
        public static RFID_18K6CSetPostMatchCriteria_T st_RfidSpReq_18K6CSetPostMatchCriteria;
        public static RFID_18K6CGetPostMatchCriteria__T st_RfidSpReq_18K6CGetPostMatchCriteria;
        //X fixed RFID_18K6C_SELECT_CRITERION          arySelectCrit[SELECTCRITERIA_COUNT];       //4; for 18K6CGetSelectCriteria msg
        //X fixed RFID_18K6C_SINGULATION_CRITERION     aryPostMatchCrit[POSTMATCHCRITERIA_COUNT]; //4; for 18K6CGetPostMatchCriteria msg
        public static RFID_18K6CSetQueryParameters_T st_RfidSpReq_18K6CSetQueryParameters;
        public static RFID_18K6CGetQueryParameters_T st_RfidSpReq_18K6CGetQueryParameters;
        public static RFID_18K6CTagInventory_T st_RfidSpReq_18K6CTagInventory;
        public static RFID_18K6CTagRead_T st_RfidSpReq_18K6CTagRead;
        public static RFID_18K6CTagWrite_T st_RfidSpReq_18K6CTagWrite;
        public static RFID_18K6CTagKill_T st_RfidSpReq_18K6CTagKill;
        public static RFID_18K6CTagLock_T st_RfidSpReq_18K6CTagLock;
        public static RFID_RadioGetSetResponseDataMode_T st_RfidSpReq_RadioSetResponseDataMode;
        public static RFID_RadioGetSetResponseDataMode_T st_RfidSpReq_RadioGetResponseDataMode;
        public static RFID_MacUpdateFirmware_T st_RfidSpReq_MacUpdateFirmware;
        public static RFID_MacGetVersion_T st_RfidSpReq_MacGetVersion;
        public static RFID_MacReadWriteOemData_T st_RfidSpReq_MacReadOemData;
        public static RFID_MacReadWriteOemData_T st_RfidSpReq_MacWriteOemData;
        public static RFID_MacReset_T st_RfidSpReq_MacReset;
        public static RFID_MacClearError_T st_RfidSpReq_MacClearError;
        public static RFID_MacBypassReadWriteRegister_T st_RfidSpReq_MacBypassWriteRegister;
        public static RFID_MacBypassReadWriteRegister_T st_RfidSpReq_MacBypassReadRegister;
        public static RFID_MacGetSetRegion_T st_RfidSpReq_MacSetRegion;
        public static RFID_MacGetSetRegion_T st_RfidSpReq_MacGetRegion;
        public static RFID_RadioSetGpioPinsConfiguration_T st_RfidSpReq_RadioSetGpioPinsConfiguration;
        public static RFID_RadioGetGpioPinsConfiguration_T st_RfidSpReq_RadioGetGpioPinsConfiguration;
        public static RFID_RadioReadWriteGpioPins_T st_RfidSpReq_RadioReadGpioPins;
        public static RFID_RadioReadWriteGpioPins_T st_RfidSpReq_RadioWriteGpioPins;
        public static RFID_RadioCancelOperation_T st_RfidSpReq_RadioCancelOperation;
        public static RFID_RadioAbortOperation_T st_RfidSpReq_RadioAbortOperation;
        /// In App, define the same structures to memcpy the message from Thread
        public static RFID_PACKETMSG_COMMAND_BEGIN_T st_RfidSpPkt_CmdBegin;
        public static RFID_PACKETMSG_COMMAND_END_T st_RfidSpPkt_CmdEnd;
        public static RFID_PACKETMSG_CARRIER_INFO_T st_RfidSpPkt_CarrierInfo;
        public static RFID_PACKETMSG_18K6C_TAG_ACCESS_T st_RfidSpPkt_TagAccessData;
        public static RFID_PACKETMSG_ANTENNA_CYCLE_BEGIN_T st_RfidSpPkt_AntCycBegin;
        public static RFID_PACKETMSG_ANTENNA_CYCLE_END_T st_RfidSpPkt_AntCycEnd;
        public static RFID_PACKETMSG_ANTENNA_BEGIN_T st_RfidSpPkt_AntBegin;
        public static RFID_PACKETMSG_ANTENNA_END_T st_RfidSpPkt_AntEnd;
        public static RFID_PACKETMSG_INVENTORY_CYCLE_BEGIN_T st_RfidSpPkt_InvenCycBegin;
        public static RFID_PACKETMSG_INVENTORY_CYCLE_END_T st_RfidSpPkt_InvenCycEnd;
        public static RFID_PACKETMSG_18K6C_INVENTORY_ROUND_BEGIN_T st_RfidSpPkt_InvenRndBegin;
        public static RFID_PACKETMSG_18K6C_INVENTORY_ROUND_END_T st_RfidSpPkt_InvenRndEnd;
        public static RFID_PACKETMSG_18K6C_INVENTORY_AND_DATA_T st_RfidSpPkt_InvenData;
        public static RFID_PACKETMSG_NONCRITICAL_FAULT_T st_RfidSpPkt_NonCritFault;
        public static RFID_CUSTOMMSG_TEMP st_RfidSpPkt_CustomTemp;
        //////////////////////////////////////////////////////////////////////////////////
        public static PECRECORD_T st_RfidMw_AddATag_PecRec;
        public static PECRECORD_T st_RfidMw_FindATag_PecRec;
        //////////////////////////////////////////////////////////////////////////////////
        public static IntPtr hWnd; //Caller HWND    
        public static UInt32 rfid_cookie;
        public static RFID_RADIO_HANDLE rfid_handle;
        public static UInt32 curr_link_profile;
        public static RFID_RADIO_LINK_PROFILE link_profile;
        //////////////////////////////////////////////////////////////////////////////////   
        public static PECRECORD_T st_RfidMw_PecRec;
        //////////////////////////////////////////////////////////////////////////////////   
        Rfid()
        {
            hWnd = IntPtr.Zero;
            rfid_cookie = 0;
            rfid_handle = 0;
            curr_link_profile = 0;
            link_profile = new RFID_RADIO_LINK_PROFILE();
        }
        static Rfid()
        {
            // Array instantiation
            st_RfidSpReq_AntennaPortGetStatus = new RFID_AntennaPortGetStatus_T[LOGICAL_ANTENNA_COUNT];
            st_RfidSpReq_AntennaPortGetConfiguration = new RFID_AntennaPortGetSetConfiguration_T[LOGICAL_ANTENNA_COUNT];
        }
    }
  

    #region RfidSp_structs
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_Startup_T
    {
        public RFID_VERSION LibraryVersion; //[out] RFID_VERSION*
        public UInt32 flags;
        public HRESULT_RFID_STATUS status;  //[ret]
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_Shutdown_T
    {
        public HRESULT_RFID_STATUS status;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_RetrieveAttachedRadiosList_T
    {
        public RFID_RADIO_ENUM_T radio_enum; //assumed single radio object for RFID_RADIO_ENUM* pBuffer[N],
        public UInt32 flags;
        public HRESULT_RFID_STATUS status;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_RadioOpen_T
    {
        public UInt32 cookie;
        public RFID_RADIO_HANDLE handle; //[out]
        public UInt32 flags;
        public HRESULT_RFID_STATUS status;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_RadioClose_T
    {
        public RFID_RADIO_HANDLE handle;
        public HRESULT_RFID_STATUS status;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_RadioGetSetConfigurationParameter_T
    {
        public RFID_RADIO_HANDLE handle;
        public UInt16 parameter;
        public UInt32 value;  //[out/in]
        public HRESULT_RFID_STATUS status;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_RadioGetSetOperationMode_T
    {
        public RFID_RADIO_HANDLE handle;
        public RFID_RADIO_OPERATION_MODE mode;
        public HRESULT_RFID_STATUS status;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_RadioGetSetPowerState_T
    {
        public RFID_RADIO_HANDLE handle;
        public RFID_RADIO_POWER_STATE state;
        public HRESULT_RFID_STATUS status;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_RadioGetSetCurrentLinkProfile_T
    {
        public RFID_RADIO_HANDLE handle;
        public UInt32 profile;
        public HRESULT_RFID_STATUS status;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_RadioGetLinkProfile_T
    {
        public RFID_RADIO_HANDLE handle;
        public UInt32 profile;
        public RFID_RADIO_LINK_PROFILE linkProfileInfo; //[out]
        public HRESULT_RFID_STATUS status;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_AntennaPortGetStatus_T
    {
        public RFID_RADIO_HANDLE handle;
        public UInt32 antennaPort;
        public RFID_ANTENNA_PORT_STATUS portStatus;   //[out]
        public HRESULT_RFID_STATUS status;

        public void Clone(out RFID_AntennaPortGetStatus_T anotherStatus)
        {
            anotherStatus = new RFID_AntennaPortGetStatus_T();
            anotherStatus.handle = handle;
            anotherStatus.antennaPort = antennaPort;
            anotherStatus.portStatus = portStatus;
            anotherStatus.status = status;
        }
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_AntennaPortSetState_T
    {
        public RFID_RADIO_HANDLE handle;
        public UInt32 antennaPort;
        public RFID_ANTENNA_PORT_STATE state;
        public HRESULT_RFID_STATUS status;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_AntennaPortGetSetConfiguration_T
    {
        public RFID_RADIO_HANDLE handle;
        public UInt32 antennaPort;
        public RFID_ANTENNA_PORT_CONFIG config;    // [const struct* / stuict*]
        public HRESULT_RFID_STATUS status;
        public void Clone(out RFID_AntennaPortGetSetConfiguration_T anotherConfig)
        {
            anotherConfig = new RFID_AntennaPortGetSetConfiguration_T();
            anotherConfig.handle = handle;
            anotherConfig.antennaPort = antennaPort;
            anotherConfig.config = this.config;
            anotherConfig.status = status;
        }
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_18K6CSetSelectCriteria__T
    {
        public RFID_RADIO_HANDLE handle;
        public UInt32 countCriteria;    /// public RFID_18K6C_SELECT_CRITERIA    criteria; //[in] const*
        public UInt32 flags;
        public HRESULT_RFID_STATUS status;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_18K6CSetSelectCriteria_T
    {
        public RFID_RADIO_HANDLE handle;
        public RFID_18K6C_SELECT_CRITERION_T criteria;
        public UInt32 flags;
        public HRESULT_RFID_STATUS status;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_18K6CGetSelectCriteria__T
    {
        public RFID_RADIO_HANDLE handle;
        public UInt32 countCriteria;     /// public RFID_18K6C_SELECT_CRITERIA criteria;
        public HRESULT_RFID_STATUS status;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_18K6CGetSelectCriteria_T
    {
        public RFID_RADIO_HANDLE handle;
        public RFID_18K6C_SELECT_CRITERION_T criteria;
        public HRESULT_RFID_STATUS status;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_18K6CSetPostMatchCriteria__T
    {
        public RFID_RADIO_HANDLE handle;
        public UInt32 countCriteria;    /// public RFID_18K6C_SINGULATION_CRITERIA  criteria; //[in] const*
        public UInt32 flags;
        public HRESULT_RFID_STATUS status;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_18K6CGetPostMatchCriteria__T
    {
        public RFID_RADIO_HANDLE handle;
        public UInt32 countCriteria;    /// public RFID_18K6C_SINGULATION_CRITERIA criteria;
        public HRESULT_RFID_STATUS status;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_18K6CSetPostMatchCriteria_T
    {
        public RFID_RADIO_HANDLE handle;
        public RFID_18K6C_SINGULATION_CRITERIA criteria;
        public UInt32 flags;
        public HRESULT_RFID_STATUS status;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_18K6CSetQueryParameters_T
    {
        public RFID_RADIO_HANDLE handle;
        public RFID_18K6C_QUERY_PARMS parms; //[in] const*
        public UInt32 flags;
        public HRESULT_RFID_STATUS status;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_18K6CGetQueryParameters_T
    {
        public RFID_RADIO_HANDLE handle;
        public RFID_18K6C_QUERY_PARMS parms;
        public HRESULT_RFID_STATUS status;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_18K6CTagInventory_T
    {
        public RFID_RADIO_HANDLE handle;
        public RFID_18K6C_INVENTORY_PARMS invenParms; //[in] const*
        public UInt32 flags;
        public HRESULT_RFID_STATUS status;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_18K6CTagRead_T
    {
        public RFID_RADIO_HANDLE handle;
        public RFID_18K6C_READ_PARMS readParms; //[in] const*
        public UInt32 flags;
        public HRESULT_RFID_STATUS status;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_18K6CTagWrite_T
    {
        public RFID_RADIO_HANDLE handle;
        public RFID_18K6C_WRITE_PARMS_T writeParms; //[in] const*
        public UInt32 flags;
        public HRESULT_RFID_STATUS status;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_18K6CTagKill_T
    {
        public RFID_RADIO_HANDLE handle;
        public RFID_18K6C_KILL_PARMS killParms; //[in] const
        public UInt32 flags;
        public HRESULT_RFID_STATUS status;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_18K6CTagLock_T
    {
        public RFID_RADIO_HANDLE handle;
        public RFID_18K6C_LOCK_PARMS lockParms; //[in] const*
        public UInt32 flags;
        public HRESULT_RFID_STATUS status;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_RadioGetSetResponseDataMode_T
    {
        public RFID_RADIO_HANDLE handle;
        public UInt32 responseType; //RFID_RESPONSE_TYPE
        public UInt32 responseMode; //[in] |[out] RFID_RESPONSE_MODE 
        public HRESULT_RFID_STATUS status;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_MacUpdateFirmware_T
    {
        public RFID_RADIO_HANDLE handle;
        public UInt32 length;
        public UIntPtr pImage; //const INT8U*
        public UInt32 flags;
        public HRESULT_RFID_STATUS status;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_MacGetVersion_T
    {
        public RFID_RADIO_HANDLE handle;
        public RFID_VERSION version;
        public HRESULT_RFID_STATUS status;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_MacReadWriteOemData_T
    {
        public RFID_RADIO_HANDLE handle;
        public UInt32 address;
        public UInt32 count;
        public UInt32[] pData;
        public HRESULT_RFID_STATUS status;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_MacReset_T
    {
        public RFID_RADIO_HANDLE handle;
        public RFID_MAC_RESET_TYPE resetType; //
        public HRESULT_RFID_STATUS status;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_MacClearError_T
    {
        public RFID_RADIO_HANDLE handle;
        public HRESULT_RFID_STATUS status;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_MacBypassReadWriteRegister_T
    {
        public RFID_RADIO_HANDLE handle;
        public UInt16 address;
        public UInt16 value;
        public HRESULT_RFID_STATUS status;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_MacGetSetRegion_T
    {
        public RFID_RADIO_HANDLE handle;
        public UInt32 region; //RFID_MAC_REGION 
        public IntPtr pRegionConfig; //void*
        public HRESULT_RFID_STATUS status;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_RadioSetGpioPinsConfiguration_T
    {
        public RFID_RADIO_HANDLE handle;
        public UInt32 mask;
        public UInt32 configuration;
        public HRESULT_RFID_STATUS status;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_RadioGetGpioPinsConfiguration_T
    {
        public RFID_RADIO_HANDLE handle;
        public UInt32 configuration;
        public HRESULT_RFID_STATUS status;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_RadioReadWriteGpioPins_T
    {
        public RFID_RADIO_HANDLE handle;
        public UInt32 mask;
        public UInt32 value;
        public HRESULT_RFID_STATUS status;
    };
    ////////////////////////////////////////////
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_RadioCancelOperation_T
    {
        public RFID_RADIO_HANDLE handle;
        public UInt32 flags;
        public HRESULT_RFID_STATUS status;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_RadioAbortOperation_T
    {
        public RFID_RADIO_HANDLE handle;
        public UInt32 flags;
        public HRESULT_RFID_STATUS status;
    };
    ////////////////////////////////////////////
    //// PAcket Messages ///////////////////////
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_PACKETMSG_COMMON_T
    {
        public Byte pkt_ver; 	//INT8U  Packet specific version number
        public Byte flags;    	//       Packet specific flags 
        public UInt16 pkt_type;   //public UInt16 Packet type identifier
        public UInt16 pkt_len; 	// Packet length preamble: number of 32-bit words that follow the common
        public UInt16 res0; 	    // Reserved for future use
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_PACKETMSG_COMMAND_BEGIN_T
    {
        public RFID_PACKETMSG_COMMON_T cmn;	 // The command for which the packet sequence is in response to
        public UInt32 command; // Current millisecond counter
        public UInt32 ms_ctr;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_PACKETMSG_COMMAND_END_T
    {
        public RFID_PACKETMSG_COMMON_T cmn;
        public UInt32 ms_ctr; // Current millisecond counter
        public UInt32 status; // Command status indicator 
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_PACKETMSG_ANTENNA_CYCLE_BEGIN_T
    {
        public RFID_PACKETMSG_COMMON_T cmn;
        // No other packet specific fields
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_PACKETMSG_ANTENNA_CYCLE_END_T
    {
        public RFID_PACKETMSG_COMMON_T cmn;
        // No other packet specific fields 
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_PACKETMSG_ANTENNA_BEGIN_T
    {
        public RFID_PACKETMSG_COMMON_T cmn;	// The logical antenna ID     
        public UInt32 antenna;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_PACKETMSG_ANTENNA_END_T
    {
        public RFID_PACKETMSG_COMMON_T cmn;	// No other packet specific fields
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_PACKETMSG_INVENTORY_CYCLE_BEGIN_T
    {
        public RFID_PACKETMSG_COMMON_T cmn;
        public UInt32 ms_ctr;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_PACKETMSG_INVENTORY_CYCLE_END_T
    {
        public RFID_PACKETMSG_COMMON_T cmn;
        public UInt32 ms_ctr;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_PACKETMSG_18K6C_INVENTORY_ROUND_BEGIN_T
    {
        public RFID_PACKETMSG_COMMON_T cmn;
        // No packet specific fields
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_PACKETMSG_18K6C_INVENTORY_ROUND_END_T
    {
        public RFID_PACKETMSG_COMMON_T cmn;
        // No packet specific fields
    } ;
    [StructLayout(LayoutKind.Sequential)] // Pointers and fixed size buffers may only be used in an unsafe context
    public struct RFID_PACKETMSG_18K6C_TAG_ACCESS_T
    {
        public RFID_PACKETMSG_COMMON_T cmn;
        public UInt32 ms_ctr;	//UInt32
        public Byte command;	//INT8U
        public Byte error_code; //INT8U Error code from tag access: 0=NoError, RFID_18K6C_TAG_ACCESS_CRC_INVALID;ACCESS_TIMEOUT;BACKSCATTER_ERROR;ACCESS_ERROR
        public UInt16 res0;		//public UInt16
        public UInt32 res1;		//UInt32
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = RfidSp.RFID_PACKET_18K6C_TAG_ACCESS__DATA_MAXSIZ)]
        public UInt32[] tag_data;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_PACKETMSG_18K6C_INVENTORY_AND_DATA_T
    {
        public RFID_PACKETMSG_COMMON_T cmn;
        public UInt32 ms_ctr;
        public UInt16 rssi;		//public UInt16
        public UInt16 ana_ctrl1;
        public UInt32 res0;		//UInt32
        //// 2007 Jan definition // INT8U  nb_rssi; INT8U res0; public UInt16 res1; UInt32 res2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = RfidSp.RFID_PACKET_18K6C_INVENTORY__DATA_MAXSIZ)]
        public UInt32[] inv_data;  //RfidSp.RFID_PACKET_18K6C_INVENTORY__DATA_MAXSIZ
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_PACKETMSG_NONCRITICAL_FAULT_T
    {
        public RFID_PACKETMSG_COMMON_T cmn;
        public UInt32 ms_ctr;		// Current millisecond counter
        public UInt16 fault_type;  	// Fault type
        public UInt16 fault_subtype;	// Fault subtype 
        public UInt32 context;  	// Context specific data for fault
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_PACKETMSG_CARRIER_INFO_T
    {
        public RFID_PACKETMSG_COMMON_T cmn;
        public UInt32 ms_ctr;		// Current millisecond counter
        public UInt32 plldivmult;  // current plldivmult setting 
        public UInt16 chan;		// channel	
        public UInt16 cw_flags;  // carrier flags
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_CUSTOMMSG_TEMP
    {
        public UInt16 amb;	// Ambient (value/threshold)
        public UInt16 xceiver;	// Transceiver (value/threshold)
        public UInt16 pow_amp; // Power Amplifier (value/threshold)
        public UInt16 pow_amp_delta; // only for threshold
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_CUSTOMMSG_FREQ
    {
        public CustomFreqGrp freqGrp;
        public int chnNum;
        public bool enLBT;
    }
    #endregion
    ///////////////////////////////////////////////////////////////////////////////////////
    #region RfidMw_structs
    [StructLayout(LayoutKind.Sequential)]
    public struct RfidMw_CmdCommon_T
    {   // [MarshalAs(UnmanagedType.U4)]  LPStruct
        public uint msgid;
        public int wData;
        public int lData;
        public ushort msglen;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct RfidMw_Cmd_T
    {  // [MarshalAs(UnmanagedType.LPStruct)]LPStr
        public RfidMw_CmdCommon_T cmm;
        public ushort sizbuf;
        ///     public byte[] arybuf;
    }
    // const int USHORTSEQNUMINVALID = 0xffff; 
    // USHORTSEQNUMMAX = USHORTSEQNUMINVALID - 1 
    [StructLayout(LayoutKind.Sequential)]
    public struct UINT96_T
    {
        public UInt32 m_MSB;
        public UInt32 m_CSB;
        public UInt32 m_LSB;

        public UINT96_T(UInt32 msb, UInt32 csb, UInt32 lsb)
        {
            m_MSB = msb;
            m_CSB = csb;
            m_LSB = lsb;
        }

        public override String ToString()
        {
            StringBuilder Sb = new StringBuilder();

            Sb.AppendFormat(null, "{0:X8}{1:X8}{2:X8}",
                m_MSB, m_CSB, m_LSB);
            return Sb.ToString();
        }

        public bool ParseString(String s)
        {
            bool Succ = false;

            if (s != null && s.Length >= 24)
            {
                try
                {
                    m_MSB = UInt32.Parse(s.Substring(0, 8), System.Globalization.NumberStyles.HexNumber);
                    m_CSB = UInt32.Parse(s.Substring(8, 8), System.Globalization.NumberStyles.HexNumber);
                    m_LSB = UInt32.Parse(s.Substring(16, 8), System.Globalization.NumberStyles.HexNumber);

                    Succ = true;
                }
                catch
                {
                    Succ = false;
                }
            }
            return Succ;
        }

        public Byte[] ToByteArray()
        {
            Byte[] Arr = new Byte[12];
            for (int i = 0, s = 24; i < 4; i++, s -= 8)
                Arr[i] = (Byte)((m_MSB >> s) & 0x000000ff);
            for (int i = 4, s = 24; i < 8; i++, s -= 8)
                Arr[i] = (Byte)((m_CSB >> s) & 0x000000ff);
            for (int i = 8, s = 24; i < 12; i++, s -= 8)
                Arr[i] = (Byte)((m_LSB >> s) & 0x000000ff);

            return Arr;
        }

        public void PlusPlus()
        {
            if (this.m_LSB == UInt32.MaxValue)
            {
                this.m_LSB = 0;
                if (this.m_CSB == UInt32.MaxValue)
                {
                    this.m_CSB = 0;
                    if (this.m_MSB == UInt32.MaxValue)
                        throw new ApplicationException("Max value reached!");
                    else
                        this.m_MSB++;
                }
                else
                    this.m_CSB++;
            }
            else
            {
                this.m_LSB++;
            }
        }
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct PECRECORD_T
    {
        public ushort m_seqnum;   // 0- 1 local time-sequence
        public ushort m_Pc;       // 2- 3
        public UINT96_T m_Epc;      // 4-15
        public ushort m_Crc;      //16-17 optional ini 0x0000
        public ushort m_Cnt;      //20-21 ini from Db	
        public ushort m_Flg;      //valid states 0(00):exist 2(10):exist,Cnt_chg 3(11):new,Cnt_chg
        public ushort m_Rssi;     //22-23 ini 0x0000
        public ushort m_AntCtrl;  //24-25 ini 0x0000
        public FileTime m_LastUpdated;
    }; ///26B
    #endregion
    ///////////////////////////////////////////////////////////////////////////////////////
    #region SettingMgt_structs

    #endregion

    #region Win32 compatible P/I structs
    [StructLayout(LayoutKind.Sequential)]
    public class SystemTime // defined as class for simpler P/I in-out parameter
    {
        public ushort wYear;
        public ushort wMonth;
        public ushort wDayOfWeek;
        public ushort wDay;
        public ushort wHour;
        public ushort wMinute;
        public ushort wSecond;
        public ushort wMilliseconds;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FileTime
    {
        public UInt32 dwLowDateTime;
        public UInt32 dwHighDateTime;
    }
    #endregion

    #region ATID Specific Data Structure

        public struct CH_STATE
        {
            public bool CH1;
            public bool CH10;
            public bool CH2;
            public bool CH3;
            public bool CH4;
            public bool CH5;
            public bool CH6;
            public bool CH7;
            public bool CH8;
            public bool CH9;
        }

        public struct CONTROL_VALUE
        {
            public bool Buzzer;
            public int CHNumber;
            public CH_STATE ChState;
            public bool ContinueMode;
            public HOPPING_CODE Hopping;
            public bool HoppingOn;
            public int LBT_Time;
            public int Power;
            public int Qvalue;
            public int ScanTime;
            public int SessionValue;
            public int Timeout;
            public string Version;
            public string FirmWareVersion;
        }

        public enum HOPPING_CODE
        {
            KOREA_FHSS = 48,
            KOREA_LBT = 49,
            JAPAN_LBT = 50,
            EURO_LBT = 51,
            USA_FHSS = 52,
            CHINA_FHSS = 54,
            JAPAN_NO_LBT = 55,
            KOREA_KCCh = 56,
            ANONYMOUS = 255,
        }

    #endregion ATID Specific Data Structure

}
