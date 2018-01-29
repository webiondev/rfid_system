using System;

using System.Runtime.InteropServices;

/// using HRESULT_RFID    = System.Int32;
//System.Int32; //use System.HREsult helper class
using RFID_RADIO_HANDLE = System.UInt32;

namespace ReaderTypes
{
    #region RfidSp_enums
    public partial class RfidSp
    {
        public const int WM_USER = 0x0400;
        public const RFID_RADIO_HANDLE RFID_INVALID_RADIO_HANDLE = ((RFID_RADIO_HANDLE)0);

        public const int SELECTCRITERIA_COUNT = 4;
        public const int POSTMATCHCRITERIA_COUNT = 4;
        public const int RFID_18K6C_SELECT_MASK_BYTE_LEN = 32;
        public const int RFID_18K6C_SINGULATION_MASK_BYTE_LEN = 62;

        public const int RFID_18K6C_MAX_TAG_WRITE_WORDS = 8; // *must* match decl in RfidSp_common_defs.h
        public const int USHORTSEQNUMINVALID = 0xffff;

        public const UInt32 AntPwrLimitingBndNum = 4;
        public const float MAX_BAND4_ANTENNA_PWR = 29.5F;

        // bands 1,2,3,4,6
        public static readonly bool[] LinkProf1xBnds = null;
        public static readonly bool[] LinkProf4xBnds = null;

        static RfidSp()
        {
            LinkProf1xBnds = new bool[5] { true, true, true, false, false };
            LinkProf4xBnds = new bool[5] { true, true, true, false, false };
        }
    } //end class RfidSp 

    public partial class RfidSp
    {
        public const int RFID_PACKET_18K6C_TAG_ACCESS__DATA_MAXSIZ = 32; //512 bits (CS101 requirement)
        public const int RFID_PACKET_18K6C_INVENTORY__DATA_MAXSIZ = 4;

        public static bool SUCCEEDED(HRESULT_RFID hr) //uint
        {
            if ((HRESULT_RFID.S_OK == hr) ||
                (HRESULT_RFID.S_RFID_STATUS_OK == hr))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    } //end class RfidSp 

    // Xneed [Flags]
    public enum HRESULT_RFID : uint
    {
        S_OK = 0x00000000, // Success
        E_ABORT = 0x80004004, // Operation aborted
        E_ACCESSDENIED = 0x80070005, // General access denied error
        E_FAIL = 0x80004005, // Unspecified failure
        E_HANDLE = 0x80070006, // Handle that is not valid
        E_INVALIDARG = 0x80070057, // One or more arguments are not valid
        E_NOINTERFACE = 0x80004002, // No such interface supported
        E_NOTIMPL = 0x80004001, // Not implemented
        E_OUTOFMEMORY = 0x8007000E, // Failed to allocate necessary memory
        E_POINTER = 0x80004003, // Pointer that is not valid
        E_UNEXPECTED = 0x8000FFFF, //  Unexpected failure
        S_RFID_STATUS_OK = 0x00040000, // RFID Success
        E_RFID_ERROR_ALREADY_OPEN = 0x8004D8F1, // Attempted to open a radio that is already open
        E_RFID_ERROR_BUFFER_TOO_SMALL = 0x8004d8f2, //Buffer supplied is too small
        E_RFID_ERROR_FAILURE = 0x8004d8f3, //General failure
        E_RFID_ERROR_DRIVER_LOAD = 0x8004d8f4, //Failed to load radio bus driver
        E_RFID_ERROR_DRIVER_MISMATCH = 0x8004d8f5, //Library cannot use version of radio bus driver present on system
        E_RFID_ERROR_EMULATION_MODE = 0x8004d8f6, //Operation cannot be performed while library is in emulation mode
        E_RFID_ERROR_INVALID_ANTENNA = 0x8004d8f7, //Antenna number is invalid
        E_RFID_ERROR_INVALID_HANDLE = 0x8004d8f8, //Radio handle provided is invalid
        E_RFID_ERROR_INVALID_PARAMETER = 0x8004d8f9, //One of the parameters to the function is invalid
        E_RFID_ERROR_NO_SUCH_RADIO = 0x8004d8fa, //Attempted to open a non-existent radio
        E_RFID_ERROR_NOT_INITIALIZED = 0x8004d8fb, //Library has not been successfully initialized
        E_RFID_ERROR_NOT_SUPPORTED = 0x8004d8fc, //Function not supported
        E_RFID_ERROR_OPERATION_CANCELLED = 0x8004d8fd, //Operation was cancelled by call to cancel operation, close radio, or shut down the library
        E_RFID_ERROR_OUT_OF_MEMORY = 0x8004d8fe, //Library encountered an error allocating memory
        E_RFID_ERROR_RADIO_BUSY = 0x8004d8ff, //The operation cannot be performed because the radio is currently busy
        E_RFID_ERROR_RADIO_FAILURE = 0x8004d900, //The underlying radio module encountered an error
        E_RFID_ERROR_RADIO_NOT_PRESENT = 0x8004d901, //The radio has been detached from the system
        E_RFID_ERROR_CURRENTLY_NOT_ALLOWED = 0x8004d902, //The RFID library function is not allowed at this time.
        E_RFID_ERROR_RADIO_NOT_RESPONDING = 0x8004d903 //The radio module's MAC firmware is not responding to requests.
    }; // endof enum
    public enum RFID_PACKET_TYPE : uint
    {
        RFID_PACKET_TYPE_COMMAND_BEGIN = 0x0000,
        RFID_PACKET_TYPE_COMMAND_END,
        RFID_PACKET_TYPE_ANTENNA_CYCLE_BEGIN,
        RFID_PACKET_TYPE_ANTENNA_BEGIN,
        RFID_PACKET_TYPE_18K6C_INVENTORY_ROUND_BEGIN,
        RFID_PACKET_TYPE_18K6C_INVENTORY,
        RFID_PACKET_TYPE_18K6C_TAG_ACCESS,
        RFID_PACKET_TYPE_ANTENNA_CYCLE_END,
        RFID_PACKET_TYPE_ANTENNA_END,
        RFID_PACKET_TYPE_18K6C_INVENTORY_ROUND_END,
        RFID_PACKET_TYPE_INVENTORY_CYCLE_BEGIN,
        RFID_PACKET_TYPE_INVENTORY_CYCLE_END,
        RFID_PACKET_TYPE_CARRIER_INFO,
        RFID_PACKET_TYPE_NONCRITICAL_FAULT = 0x2000,
    }; // endof enum

    ///RFID_REQorPACKET_TYPE_MSGID
    public enum RFID_MSGID : uint
    {
        RFID_REQUEST_TYPE_MSGID_Startup = RfidSp.WM_USER + 0x0040, //WM_USER+0x40 =RFID_REQUEST_TYPE_MSGID_START,
        RFID_REQUEST_TYPE_MSGID_Shutdown,
        RFID_REQUEST_TYPE_MSGID_RetrieveAttachedRadiosList,
        RFID_REQUEST_TYPE_MSGID_RadioOpen,
        RFID_REQUEST_TYPE_MSGID_RadioClose,
        RFID_REQUEST_TYPE_MSGID_RadioSetConfigurationParameter,
        RFID_REQUEST_TYPE_MSGID_RadioGetConfigurationParameter,
        RFID_REQUEST_TYPE_MSGID_RadioSetOperationMode,
        RFID_REQUEST_TYPE_MSGID_RadioGetOperationMode,
        RFID_REQUEST_TYPE_MSGID_RadioSetPowerState,
        RFID_REQUEST_TYPE_MSGID_RadioGetPowerState,
        RFID_REQUEST_TYPE_MSGID_RadioSetCurrentLinkProfile,
        RFID_REQUEST_TYPE_MSGID_RadioGetCurrentLinkProfile,
        RFID_REQUEST_TYPE_MSGID_RadioGetLinkProfile,
        RFID_REQUEST_TYPE_MSGID_AntennaPortGetStatus,
        RFID_REQUEST_TYPE_MSGID_AntennaPortSetState,
        RFID_REQUEST_TYPE_MSGID_AntennaPortSetConfiguration,
        RFID_REQUEST_TYPE_MSGID_AntennaPortGetConfiguration,
        RFID_REQUEST_TYPE_MSGID_18K6CSetSelectCriteria,
        RFID_REQUEST_TYPE_MSGID_18K6CGetSelectCriteria,
        RFID_REQUEST_TYPE_MSGID_18K6CSetPostMatchCriteria,
        RFID_REQUEST_TYPE_MSGID_18K6CGetPostMatchCriteria,
        RFID_REQUEST_TYPE_MSGID_18K6CSetQueryParameters,
        RFID_REQUEST_TYPE_MSGID_18K6CGetQueryParameters,
        RFID_REQUEST_TYPE_MSGID_18K6CTagInventory,
        RFID_REQUEST_TYPE_MSGID_18K6CTagRead,
        RFID_REQUEST_TYPE_MSGID_18K6CTagWrite,
        RFID_REQUEST_TYPE_MSGID_18K6CTagKill,
        RFID_REQUEST_TYPE_MSGID_18K6CTagLock,
        RFID_REQUEST_TYPE_MSGID_RadioCancelOperation,
        RFID_REQUEST_TYPE_MSGID_RadioAbortOperation,
        RFID_REQUEST_TYPE_MSGID_RadioSetResponseDataMode,
        RFID_REQUEST_TYPE_MSGID_RadioGetResponseDataMode,
        RFID_REQUEST_TYPE_MSGID_MacUpdateFirmware,
        RFID_REQUEST_TYPE_MSGID_MacGetVersion,
        RFID_REQUEST_TYPE_MSGID_MacReadOemData,
        RFID_REQUEST_TYPE_MSGID_MacWriteOemData,
        RFID_REQUEST_TYPE_MSGID_MacReset,
        RFID_REQUEST_TYPE_MSGID_MacClearError,
        RFID_REQUEST_TYPE_MSGID_MacBypassWriteRegister,
        RFID_REQUEST_TYPE_MSGID_MacBypassReadRegister,
        RFID_REQUEST_TYPE_MSGID_MacSetRegion,
        RFID_REQUEST_TYPE_MSGID_MacGetRegion,
        RFID_REQUEST_TYPE_MSGID_RadioSetGpioPinsConfiguration,
        RFID_REQUEST_TYPE_MSGID_RadioGetGpioPinsConfiguration,
        RFID_REQUEST_TYPE_MSGID_RadioReadGpioPins,
        RFID_REQUEST_TYPE_MSGID_RadioWriteGpioPins,
        RFID_REQUEST_TYPE_MSGID_RadioTurnCarrierWaveOn,
        RFID_REQUEST_TYPE_MSGID_RadioTurnCarrierWaveOff,
        RFID_REQUEST_TYPE_MSGID_CustomGetTemperature,
        RFID_REQUEST_TYPE_MSGID_CustomGetThrshTemperature,
        RFID_REQUEST_TYPE_MSGID_CustomTagInvtryRdRate, // Tag Inventory but only report Rd-Rate periodically
        RFID_REQUEST_TYPE_MSGID_CustomTagInvtryRssi,  // Tag Inventory but only report Rssi periodically
        RFID_REQUEST_TYPE_MSGID_CustomGetFreqBandNum,
        RFID_REQUEST_TYPE_MSGID_CustomGetRadioProfile,
        RFID_REQUEST_TYPE_MSGID_CustomSetRadioChn,
        RFID_REQUEST_TYPE_MSGID_CustomSetRadioProfile,
        RFID_REQUEST_TYPE_MSGID_END = RFID_REQUEST_TYPE_MSGID_CustomSetRadioProfile,
        //////// 43 Request ACK MsgId
        RFID_REQEND_TYPE_MSGID_START = RFID_REQUEST_TYPE_MSGID_END + 0x01,
        RFID_REQEND_TYPE_MSGID_Startup = RFID_REQEND_TYPE_MSGID_START,
        RFID_REQEND_TYPE_MSGID_Shutdown,
        RFID_REQEND_TYPE_MSGID_RetrieveAttachedRadiosList,
        RFID_REQEND_TYPE_MSGID_RadioOpen,
        RFID_REQEND_TYPE_MSGID_RadioClose,
        RFID_REQEND_TYPE_MSGID_RadioSetConfigurationParameter,
        RFID_REQEND_TYPE_MSGID_RadioGetConfigurationParameter,
        RFID_REQEND_TYPE_MSGID_RadioSetOperationMode,
        RFID_REQEND_TYPE_MSGID_RadioGetOperationMode,
        RFID_REQEND_TYPE_MSGID_RadioSetPowerState,
        RFID_REQEND_TYPE_MSGID_RadioGetPowerState,
        RFID_REQEND_TYPE_MSGID_RadioSetCurrentLinkProfile,
        RFID_REQEND_TYPE_MSGID_RadioGetCurrentLinkProfile,
        RFID_REQEND_TYPE_MSGID_RadioGetLinkProfile,
        RFID_REQEND_TYPE_MSGID_AntennaPortGetStatus,
        RFID_REQEND_TYPE_MSGID_AntennaPortSetState,
        RFID_REQEND_TYPE_MSGID_AntennaPortSetConfiguration,
        RFID_REQEND_TYPE_MSGID_AntennaPortGetConfiguration,
        RFID_REQEND_TYPE_MSGID_18K6CSetSelectCriteria,
        RFID_REQEND_TYPE_MSGID_18K6CGetSelectCriteria,
        RFID_REQEND_TYPE_MSGID_18K6CSetPostMatchCriteria,
        RFID_REQEND_TYPE_MSGID_18K6CGetPostMatchCriteria,
        RFID_REQEND_TYPE_MSGID_18K6CSetQueryParameters,
        RFID_REQEND_TYPE_MSGID_18K6CGetQueryParameters,
        RFID_REQEND_TYPE_MSGID_18K6CTagInventory,
        RFID_REQEND_TYPE_MSGID_18K6CTagRead,
        RFID_REQEND_TYPE_MSGID_18K6CTagWrite,
        RFID_REQEND_TYPE_MSGID_18K6CTagKill,
        RFID_REQEND_TYPE_MSGID_18K6CTagLock,
        RFID_REQEND_TYPE_MSGID_RadioCancelOperation,
        RFID_REQEND_TYPE_MSGID_RadioAbortOperation,
        RFID_REQEND_TYPE_MSGID_RadioSetResponseDataMode,
        RFID_REQEND_TYPE_MSGID_RadioGetResponseDataMode,
        RFID_REQEND_TYPE_MSGID_MacUpdateFirmware,
        RFID_REQEND_TYPE_MSGID_MacGetVersion,
        RFID_REQEND_TYPE_MSGID_MacReadOemData,
        RFID_REQEND_TYPE_MSGID_MacWriteOemData,
        RFID_REQEND_TYPE_MSGID_MacReset,
        RFID_REQEND_TYPE_MSGID_MacClearError,
        RFID_REQEND_TYPE_MSGID_MacBypassWriteRegister,
        RFID_REQEND_TYPE_MSGID_MacBypassReadRegister,
        RFID_REQEND_TYPE_MSGID_MacSetRegion,
        RFID_REQEND_TYPE_MSGID_MacGetRegion,
        RFID_REQEND_TYPE_MSGID_RadioSetGpioPinsConfiguration,
        RFID_REQEND_TYPE_MSGID_RadioGetGpioPinsConfiguration,
        RFID_REQEND_TYPE_MSGID_RadioReadGpioPins,
        RFID_REQEND_TYPE_MSGID_RadioWriteGpioPins,
        RFID_REQEND_TYPE_MSGID_RadioTurnCarrierWaveOn,
        RFID_REQEND_TYPE_MSGID_RadioTurnCarrierWaveOff,
        RFID_REQEND_TYPE_MSGID_CustomGetTemperature,
        RFID_REQEND_TYPE_MSGID_CustomGetThrshTemperature,
        RFID_REQEND_TYPE_MSGID_CustomTagInvtryRdRate,
        RFID_REQEND_TYPE_MSGID_CustomTagInvtryRssi,
        RFID_REQEND_TYPE_MSGID_CustomGetFreqBandNum,
        RFID_REQEND_TYPE_MSGID_CustomGetRadioProfile,
        RFID_REQEND_TYPE_MSGID_CustomSetRadioChn,
        RFID_REQEND_TYPE_MSGID_CustomSetRadioProfile,
        RFID_REQEND_TYPE_MSGID_END = RFID_REQEND_TYPE_MSGID_CustomSetRadioProfile,
        //////// Packets	
        RFID_PACKET_TYPE_MSGID_START = RFID_REQEND_TYPE_MSGID_END + 0x01, /// 12 Pkt MsgId. 0x0000+ MSGID_START 
        RFID_PACKET_TYPE_MSGID_COMMAND_BEGIN = RFID_PACKET_TYPE.RFID_PACKET_TYPE_COMMAND_BEGIN + RFID_PACKET_TYPE_MSGID_START,
        RFID_PACKET_TYPE_MSGID_COMMAND_END = RFID_PACKET_TYPE.RFID_PACKET_TYPE_COMMAND_END + RFID_PACKET_TYPE_MSGID_START,
        RFID_PACKET_TYPE_MSGID_ANTENNA_CYCLE_BEGIN = RFID_PACKET_TYPE.RFID_PACKET_TYPE_ANTENNA_CYCLE_BEGIN + RFID_PACKET_TYPE_MSGID_START,
        RFID_PACKET_TYPE_MSGID_ANTENNA_BEGIN = RFID_PACKET_TYPE.RFID_PACKET_TYPE_ANTENNA_BEGIN + RFID_PACKET_TYPE_MSGID_START,
        RFID_PACKET_TYPE_MSGID_18K6C_INVENTORY_ROUND_BEGIN = RFID_PACKET_TYPE.RFID_PACKET_TYPE_18K6C_INVENTORY_ROUND_BEGIN + RFID_PACKET_TYPE_MSGID_START,
        RFID_PACKET_TYPE_MSGID_18K6C_INVENTORY = RFID_PACKET_TYPE.RFID_PACKET_TYPE_18K6C_INVENTORY + RFID_PACKET_TYPE_MSGID_START,
        RFID_PACKET_TYPE_MSGID_18K6C_TAG_ACCESS = RFID_PACKET_TYPE.RFID_PACKET_TYPE_18K6C_TAG_ACCESS + RFID_PACKET_TYPE_MSGID_START,
        RFID_PACKET_TYPE_MSGID_ANTENNA_CYCLE_END = RFID_PACKET_TYPE.RFID_PACKET_TYPE_ANTENNA_CYCLE_END + RFID_PACKET_TYPE_MSGID_START,
        RFID_PACKET_TYPE_MSGID_ANTENNA_END = RFID_PACKET_TYPE.RFID_PACKET_TYPE_ANTENNA_END + RFID_PACKET_TYPE_MSGID_START,
        RFID_PACKET_TYPE_MSGID_18K6C_INVENTORY_ROUND_END = RFID_PACKET_TYPE.RFID_PACKET_TYPE_18K6C_INVENTORY_ROUND_END + RFID_PACKET_TYPE_MSGID_START,
        RFID_PACKET_TYPE_MSGID_INVENTORY_CYCLE_BEGIN = RFID_PACKET_TYPE.RFID_PACKET_TYPE_INVENTORY_CYCLE_BEGIN + RFID_PACKET_TYPE_MSGID_START,
        RFID_PACKET_TYPE_MSGID_INVENTORY_CYCLE_END = RFID_PACKET_TYPE.RFID_PACKET_TYPE_INVENTORY_CYCLE_END + RFID_PACKET_TYPE_MSGID_START,
        RFID_PACKET_TYPE_MSGID_CARRIER_INFO = RFID_PACKET_TYPE.RFID_PACKET_TYPE_CARRIER_INFO + RFID_PACKET_TYPE_MSGID_START,
        // non for the diagnostics pkt.
        // for the status pkt. 0x2000+ MSGID_START
        RFID_PACKET_TYPE_MSGID_NONCRITICAL_FAULT = RFID_PACKET_TYPE.RFID_PACKET_TYPE_NONCRITICAL_FAULT + RFID_PACKET_TYPE_MSGID_START,
        // Custom Packet (not following the format of common RFID_PACKET_TYPE
        RFID_PACKET_TYPE_MSGID_CUSTOM_ELAPSEDTM, // Cont. Inventory only
        RFID_PACKET_TYPE_MSGID_CUSTOM_TAGRATE,
        RFID_PACKET_TYPE_MSGID_CUSTOM_RSSI,
        RFID_PACKET_TYPE_MSGID_CUSTOM_TEMP,
        RFID_PACKET_TYPE_MSGID_END = RFID_PACKET_TYPE_MSGID_CUSTOM_TEMP,

        RFIDMW_REQUEST_TYPE_MSGID_START = RFID_PACKET_TYPE_MSGID_END + 0x01,
        RFIDMW_REQUEST_TYPE_MSGID_TagInv_SetAllTaglist = RFIDMW_REQUEST_TYPE_MSGID_START,
        RFIDMW_REQUEST_TYPE_MSGID_TagInv_AddATag,
        RFIDMW_REQUEST_TYPE_MSGID_TagInv_FindATag,
        RFIDMW_REQUEST_TYPE_MSGID_TagInv_ClearAllTaglist,
        RFIDMW_REQUEST_TYPE_MSGID_TagInv_UpdateAllTaglistToFile,
        RFIDMW_REQUEST_TYPE_MSGID_TagInv_GetUpdateTaglist,
        RFIDMW_REQUEST_TYPE_MSGID_TagInv_GetAllTaglist,
        RFIDMW_REQUEST_TYPE_MSGID_TagInv_SetMsgMode,
        RFIDMW_REQUEST_TYPE_MSGID_END = RFIDMW_REQUEST_TYPE_MSGID_TagInv_SetMsgMode,
        RFIDMW_REQEND_TYPE_MSGID_START = RFIDMW_REQUEST_TYPE_MSGID_END + 0x01,
        RFIDMW_REQEND_TYPE_MSGID_TagInv_SetAllTaglist = RFIDMW_REQEND_TYPE_MSGID_START,
        RFIDMW_REQEND_TYPE_MSGID_TagInv_AddATag,
        RFIDMW_REQEND_TYPE_MSGID_TagInv_FindATag,
        RFIDMW_REQEND_TYPE_MSGID_TagInv_ClearAllTaglist,
        RFIDMW_REQEND_TYPE_MSGID_TagInv_UpdateAllTaglistToFile,
        RFIDMW_REQEND_TYPE_MSGID_TagInv_GetUpdateTaglist,
        RFIDMW_REQEND_TYPE_MSGID_TagInv_GetAllTaglist,
        RFIDMW_REQEND_TYPE_MSGID_TagInv_SetMsgMode,
        RFIDMW_REQEND_TYPE_MSGID_END = RFIDMW_REQEND_TYPE_MSGID_TagInv_SetMsgMode,

        RFIDMW

    };
    // endof enum
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public enum RFID_RADIO_OPERATION_MODE : uint
    {
        RFID_RADIO_OPERATION_MODE_CONTINUOUS,
        RFID_RADIO_OPERATION_MODE_NONCONTINUOUS
    };
    public enum RFID_RADIO_POWER_STATE : uint
    {
        RFID_RADIO_POWER_STATE_FULL,
        RFID_RADIO_POWER_STATE_STANDBY
    };
    public enum RFID_ANTENNA_PORT_STATE : uint
    {
        RFID_ANTENNA_PORT_STATE_DISABLED,
        RFID_ANTENNA_PORT_STATE_ENABLED
    };
    public enum RFID_18K6C_MODULATION_TYPE : uint
    {
        RFID_18K6C_MODULATION_TYPE_DSB_ASK,
        RFID_18K6C_MODULATION_TYPE_SSB_ASK,
        RFID_18K6C_MODULATION_TYPE_PR_ASK
    };
    public enum RFID_18K6C_DATA_0_1_DIFFERENCE : uint
    {
        RFID_18K6C_DATA_0_1_DIFFERENCE_HALF_TARI,
        RFID_18K6C_DATA_0_1_DIFFERENCE_ONE_TARI
    };
    public enum RFID_18K6C_DIVIDE_RATIO : uint
    {
        RFID_18K6C_DIVIDE_RATIO_8,
        RFID_18K6C_DIVIDE_RATIO_64DIV3
    };
    public enum RFID_18K6C_MILLER_NUMBER : uint
    {
        RFID_18K6C_MILLER_NUMBER_FM0,
        RFID_18K6C_MILLER_NUMBER_2,
        RFID_18K6C_MILLER_NUMBER_4,
        RFID_18K6C_MILLER_NUMBER_8
    };
    public enum RFID_RADIO_PROTOCOL : uint
    {
        RFID_RADIO_PROTOCOL_ISO18K6C
    };
    public enum RFID_18K6C_MEMORY_BANK : uint
    {
        RFID_18K6C_MEMORY_BANK_RESERVED,
        RFID_18K6C_MEMORY_BANK_EPC,
        RFID_18K6C_MEMORY_BANK_TID,
        RFID_18K6C_MEMORY_BANK_USER
    };
    public enum RFID_18K6C_TARGET : uint
    {
        RFID_18K6C_TARGET_INVENTORY_S0,
        RFID_18K6C_TARGET_INVENTORY_S1,
        RFID_18K6C_TARGET_INVENTORY_S2,
        RFID_18K6C_TARGET_INVENTORY_S3,
        RFID_18K6C_TARGET_SELECTED
    };
    public enum RFID_18K6C_ACTION : uint
    {
        RFID_18K6C_ACTION_ASLINVA_DSLINVB,
        RFID_18K6C_ACTION_ASLINVA_NOTHING,
        RFID_18K6C_ACTION_NOTHING_DSLINVB,
        RFID_18K6C_ACTION_NSLINVS_NOTHING,
        RFID_18K6C_ACTION_DSLINVB_ASLINVA,
        RFID_18K6C_ACTION_DSLINVB_NOTHING,
        RFID_18K6C_ACTION_NOTHING_ASLINVA,
        RFID_18K6C_ACTION_NOTHING_NSLINVS
    };
    public enum RFID_18K6C_SELECTED : uint
    {
        RFID_18K6C_SELECTED_ALL = 0,
        RFID_18K6C_SELECTED_OFF = 2,
        RFID_18K6C_SELECTED_ON = 3
    };
    public enum RFID_18K6C_INVENTORY_SESSION : uint
    {
        RFID_18K6C_INVENTORY_SESSION_S0,
        RFID_18K6C_INVENTORY_SESSION_S1,
        RFID_18K6C_INVENTORY_SESSION_S2,
        RFID_18K6C_INVENTORY_SESSION_S3
    };
    public enum RFID_18K6C_INVENTORY_SESSION_TARGET : uint
    {
        RFID_18K6C_INVENTORY_SESSION_TARGET_A,
        RFID_18K6C_INVENTORY_SESSION_TARGET_B
    };
    public enum RFID_18K6C_SINGULATION_ALGORITHM : uint
    {
        RFID_18K6C_SINGULATION_ALGORITHM_FIXEDQ = 0,
        RFID_18K6C_SINGULATION_ALGORITHM_DYNAMICQ = 1
    };
    public enum RFID_18K6C_WRITE_TYPE : uint
    {
        RFID_18K6C_WRITE_TYPE_SEQUENTIAL,
        RFID_18K6C_WRITE_TYPE_RANDOM
    };
    public enum RFID_18K6C_TAG_PWD_PERM : uint
    {
        RFID_18K6C_TAG_PWD_PERM_ACCESSIBLE,
        RFID_18K6C_TAG_PWD_ALWAYS_ACCESSIBLE,
        RFID_18K6C_TAG_PWD_SECURED_ACCESSIBLE,
        RFID_18K6C_TAG_PWD_ALWAYS_NOT_ACCESSIBLE,
        RFID_18K6C_TAG_PWD_PERM_NO_CHANGE
    };
    public enum RFID_18K6C_TAG_MEM_PERM : uint
    {
        RFID_18K6C_TAG_MEM_PERM_WRITEABLE,
        RFID_18K6C_TAG_MEM_ALWAYS_WRITEABLE,
        RFID_18K6C_TAG_MEM_SECURED_WRITEABLE,
        RFID_18K6C_TAG_MEM_ALWAYS_NOT_WRITEABLE,
        RFID_18K6C_TAG_MEM_NO_CHANGE
    };
    public enum RFID_RESPONSE_TYPE : uint
    {
        RFID_RESPONSE_TYPE_DATA = 0xFFFFFFFF
    };
    public enum RFID_RESPONSE_MODE : uint
    {
        RFID_RESPONSE_MODE_COMPACT = 0x00000001,
        RFID_RESPONSE_MODE_NORMAL = 0x00000003,
        RFID_RESPONSE_MODE_EXTENDED = 0x00000007
    };
    public enum RFID_MAC_RESET_TYPE : uint
    {
        RFID_MAC_RESET_TYPE_SOFT
    };
    public enum RFID_MAC_REGION : uint
    {
        RFID_MAC_REGION_FCC_GENERIC,
        RFID_MAC_REGION_ETSI_GENERIC
    };
    public enum RFID_RADIO_GPIO_PIN : uint
    {
        RFID_RADIO_GPIO_PIN_0 = 0x00000001 << 0, // SET_BIT(0),
        RFID_RADIO_GPIO_PIN_1 = 0x00000001 << 1, // SET_BIT(1),
        RFID_RADIO_GPIO_PIN_2 = 0x00000001 << 2, // SET_BIT(2),
        RFID_RADIO_GPIO_PIN_3 = 0x00000001 << 3, // SET_BIT(3),
        RFID_RADIO_GPIO_PIN_4 = 0x00000001 << 4, // SET_BIT(4),
        RFID_RADIO_GPIO_PIN_5 = 0x00000001 << 5, // SET_BIT(5),
        RFID_RADIO_GPIO_PIN_6 = 0x00000001 << 6, // SET_BIT(6),
        RFID_RADIO_GPIO_PIN_7 = 0x00000001 << 7, // SET_BIT(7),
        RFID_RADIO_GPIO_PIN_8 = 0x00000001 << 8, // SET_BIT(8),
        RFID_RADIO_GPIO_PIN_9 = 0x00000001 << 9, // SET_BIT(9),
        RFID_RADIO_GPIO_PIN_10 = 0x00000001 << 10, // SET_BIT(10),
        RFID_RADIO_GPIO_PIN_11 = 0x00000001 << 11, // SET_BIT(11),
        RFID_RADIO_GPIO_PIN_12 = 0x00000001 << 12, // SET_BIT(12),
        RFID_RADIO_GPIO_PIN_13 = 0x00000001 << 13, // SET_BIT(13),
        RFID_RADIO_GPIO_PIN_14 = 0x00000001 << 14, // SET_BIT(14),
        RFID_RADIO_GPIO_PIN_15 = 0x00000001 << 15, // SET_BIT(15),
        RFID_RADIO_GPIO_PIN_16 = 0x00000001 << 16, // SET_BIT(16),
        RFID_RADIO_GPIO_PIN_17 = 0x00000001 << 17, // SET_BIT(17),
        RFID_RADIO_GPIO_PIN_18 = 0x00000001 << 18, // SET_BIT(18),
        RFID_RADIO_GPIO_PIN_19 = 0x00000001 << 19, // SET_BIT(19),
        RFID_RADIO_GPIO_PIN_20 = 0x00000001 << 20, // SET_BIT(20),
        RFID_RADIO_GPIO_PIN_21 = 0x00000001 << 21, // SET_BIT(21),
        RFID_RADIO_GPIO_PIN_22 = 0x00000001 << 22, // SET_BIT(22),
        RFID_RADIO_GPIO_PIN_23 = 0x00000001 << 23, // SET_BIT(23),
        RFID_RADIO_GPIO_PIN_24 = 0x00000001 << 24, // SET_BIT(24),
        RFID_RADIO_GPIO_PIN_25 = 0x00000001 << 25, // SET_BIT(25),
        RFID_RADIO_GPIO_PIN_26 = 0x00000001 << 26, // SET_BIT(26),
        RFID_RADIO_GPIO_PIN_27 = 0x00000001 << 27, // SET_BIT(27),
        RFID_RADIO_GPIO_PIN_28 = 0x00000001 << 28, // SET_BIT(28),
        RFID_RADIO_GPIO_PIN_29 = 0x00000001 << 29, // SET_BIT(29),
        RFID_RADIO_GPIO_PIN_30 = 0x00000001 << 30, // SET_BIT(30),
        RFID_RADIO_GPIO_PIN_31 = 0x80000000 //1<<31  // SET_BIT(31)
    };
    //// Annoynomous flags

    public enum RFID_Startup_EMULATION_FLAG
    {
        RFID_FLAG_LIBRARY_EMULATION = 0x00000001
    }; /// for RFID_Startup
    public enum RFID_RadioOpen_EMULATION_FLAG
    {
        RFID_FLAG_MAC_EMULATION = 0x00000001
    }; /// for RFID_RadioOpen
    public enum RFID_18K6CTag_FLAG
    {
        RFID_FLAG_PERFORM_SELECT = 0x00000001,
        RFID_FLAG_PERFORM_POST_MATCH = 0x00000002
    };

    // Element Order is important, change with care
    public enum CustomFreqGrp
    {
        FCC, //default
        CN,
        TW,
        ETSI,
        KR,
        HK,
        JPN,
        PlaceHolder, // irrelevant
        NumCountries = PlaceHolder,
    }; /// for CustomSetRadioProfile

    public enum SoundVol
    {
        Mute = 0,
        Low = 1,
        Med = 2,
        High = 3,
    };

    public enum RingTone
    {
        T1 = 1,
        T2,
        T3,
        T4,
        T5
    };
    //////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_VERSION
    {
        public UInt32 major;
        public UInt32 minor;
        public UInt32 patch;

        public override string ToString()
        {
            return major.ToString() + "." + minor.ToString() + "." + patch.ToString();
        }
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_RADIO_INFO
    {
        public UInt32 length;
        public RFID_VERSION driverVersion;
        public UInt32 cookie;
        public UInt32 idLength;
        public IntPtr pUniqueId;     //INT 8U* Dont_Care ansi_string
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_RADIO_ENUM_T
    {         //Fixed to Single Radio Object
        public UInt32 length;
        public UInt32 totalLength;
        public UInt32 countRadios;
        public RFID_RADIO_INFO _RadioInfo; //IntPtr ppRadioInfo
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_RADIO_LINK_PROFILE_ISO18K6C_CONFIG
    {
        public UInt32 length;
        public RFID_18K6C_MODULATION_TYPE modulationType;
        public UInt32 tari;
        public RFID_18K6C_DATA_0_1_DIFFERENCE data01Difference;
        public UInt32 pulseWidth;
        public UInt32 rtCalibration;
        public UInt32 trCalibration;
        public RFID_18K6C_DIVIDE_RATIO divideRatio;
        public RFID_18K6C_MILLER_NUMBER millerNumber;
        public UInt32 trLinkFrequency;
        public UInt32 varT2Delay;
        public UInt32 rxDelay;
        public UInt32 minT2Delay;
        public UInt32 txPropagationDelay;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_RADIO_LINK_PROFILE
    {
        public UInt32 length;
        public UInt32 enabled;         // BOOL32
        public UInt64 profileId;
        public UInt32 profileVersion;
        public RFID_RADIO_PROTOCOL profileProtocol;
        public UInt32 denseReaderMode; // BOOL32
        public UInt32 widebandRssiSamples;
        public UInt32 narrowbandRssiSamples;
        public UInt32 realtimeRssiEnabled;
        public UInt32 realtimeWidebandRssiSamples;
        public UInt32 realtimeNarrowbandRssiSamples;
        public RFID_RADIO_LINK_PROFILE_ISO18K6C_CONFIG iso18K6C; //union { RFID_RADIO_LINK_PROFILE_ISO18K6C_CONFIG iso18K6C;} profileConfig;
    } ;
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_ANTENNA_PORT_STATUS
    {
        public UInt32 length;
        public RFID_ANTENNA_PORT_STATE state;
        public UInt32 antennaSenseValue;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_ANTENNA_PORT_CONFIG
    {
        public UInt32 length;
        public UInt32 powerLevel;
        public UInt32 dwellTime;
        public UInt32 numberInventoryCycles;
        public UInt32 physicalRxPort;
        public UInt32 physicalTxPort;
        public UInt32 antennaSenseThreshold;

        // Note: Exclude 'length' field for comparison
        public bool SameAs(ref RFID_ANTENNA_PORT_CONFIG anotherPrt)
        {
            return (
                    this.powerLevel == anotherPrt.powerLevel
                    && this.dwellTime == anotherPrt.dwellTime
                    && this.numberInventoryCycles == anotherPrt.numberInventoryCycles
                    && this.physicalRxPort == anotherPrt.physicalRxPort
                    && this.physicalTxPort == anotherPrt.physicalTxPort
                    && this.antennaSenseThreshold == anotherPrt.antennaSenseThreshold);
        }
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_18K6C_SELECT_MASK
    {
        public RFID_18K6C_MEMORY_BANK bank;
        public UInt32 offset;
        public UInt32 count;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = RfidSp.RFID_18K6C_SELECT_MASK_BYTE_LEN)]
        public Byte[] mask;

        public void CopyTo(ref RFID_18K6C_SELECT_MASK dstMask)
        {
            dstMask.bank = this.bank;
            dstMask.offset = this.offset;
            dstMask.count = this.count;
            dstMask.mask = (this.mask != null) ? (Byte[])this.mask.Clone() : new Byte[0];
        }
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_18K6C_SELECT_ACTION
    {
        public RFID_18K6C_TARGET target;
        public RFID_18K6C_ACTION action;
        public UInt32 enableTruncate; //BOOL32
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_18K6C_SELECT_CRITERION
    {
        public RFID_18K6C_SELECT_MASK mask;
        public RFID_18K6C_SELECT_ACTION action;

        public void CopyTo(ref RFID_18K6C_SELECT_CRITERION dstCrit)
        {
            this.mask.CopyTo(ref dstCrit.mask);
            dstCrit.action = this.action; // shallow copy is enough
        }
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_18K6C_SELECT_CRITERION_T
    {
        public UInt32 countCriteria;
        public IntPtr criteria; //C: RFID_18K6C_SELECT_CRITERION *criteria;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_18K6C_SINGULATION_MASK
    {
        public UInt32 offset;
        public UInt32 count;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = RfidSp.RFID_18K6C_SINGULATION_MASK_BYTE_LEN)]
        public Byte[] mask;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_18K6C_SINGULATION_CRITERION
    {
        public UInt32 match;  //BOOL32
        public RFID_18K6C_SINGULATION_MASK mask;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_18K6C_SINGULATION_CRITERIA
    {
        public UInt32 countCriteria;
        public IntPtr criteria;//C: RFID_18K6C_SINGULATION_CRITERION*   pCriteria;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_18K6C_TAG_GROUP
    {
        public RFID_18K6C_SELECTED selected;
        public RFID_18K6C_INVENTORY_SESSION session;
        public RFID_18K6C_INVENTORY_SESSION_TARGET target;

        public bool SameAs(ref RFID_18K6C_TAG_GROUP anotherGrp)
        {
            return (this.selected == anotherGrp.selected
                    && this.session == anotherGrp.session
                    && this.target == anotherGrp.target);
        }
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_18K6C_COMMON_PARMS
    {
        public UInt32 tagStopCount;
        public IntPtr pCallback;      //RFID_PACKET_CALLBACK_FUNCTION
        public IntPtr context;        //void*    //Nullable
        public IntPtr pCallbackCode;  //INT 32S* //##
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_18K6C_SINGULATION_FIXEDQ_PARMS
    {
        public UInt32 length;
        public UInt32 qValue;
        public UInt32 retryCount;
        public UInt32 toggleTarget; //BOOL32
        public UInt32 repeatUntilNoTags; //BOOL32

        public RFID_18K6C_SINGULATION_FIXEDQ_PARMS(UInt32 qVal,
            UInt32 retryCnt, UInt32 toggleTgt, UInt32 rpttUntilNoTags)
        {
            length = 0; //sizeof(RFID_18K6C_SINGULATION_FIXEDQ_PARMS);
            qValue = qVal;
            retryCount = retryCnt;
            toggleTarget = toggleTgt;
            repeatUntilNoTags = rpttUntilNoTags;
        }

        // Does not compare 'length' field
        public bool SameAs(ref RFID_18K6C_SINGULATION_FIXEDQ_PARMS anotherParm)
        {
            return ((qValue == anotherParm.qValue) && (retryCount == anotherParm.retryCount)
                && (toggleTarget == anotherParm.toggleTarget)
                && (repeatUntilNoTags == anotherParm.repeatUntilNoTags));
        }
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_18K6C_SINGULATION_DYNAMICQ_PARMS
    {
        public UInt32 length;
        public UInt32 startQValue;
        public UInt32 minQValue;
        public UInt32 maxQValue;
        public UInt32 retryCount;
        public UInt32 maxQueryRepCount;
        public UInt32 toggleTarget; //BOOL32

        public RFID_18K6C_SINGULATION_DYNAMICQ_PARMS(UInt32 startQVal,
            UInt32 minQVal, UInt32 maxQVal, UInt32 retryCnt, UInt32 maxQueryRepCnt,
            UInt32 toggleTgt)
        {
            length = 0; // sizeof(RFID_18K6C_SINGULATION_DYNAMICQ_PARMS);
            startQValue = startQVal;
            minQValue = minQVal;
            maxQValue = maxQVal;
            retryCount = retryCnt;
            maxQueryRepCount = maxQueryRepCnt;
            toggleTarget = toggleTgt;
        }

        // Does not compare 'length' field
        public bool SameAs(ref RFID_18K6C_SINGULATION_DYNAMICQ_PARMS anotherParm)
        {
            return ((startQValue == anotherParm.startQValue) && (minQValue == anotherParm.minQValue)
                && (maxQValue == anotherParm.maxQValue) && (retryCount == anotherParm.retryCount)
                && (maxQueryRepCount == anotherParm.maxQueryRepCount)
                && (toggleTarget == anotherParm.toggleTarget));
        }
    };
    [StructLayout(LayoutKind.Explicit)] // FIXEDQ_PARMS union DYNAMICQ_PARMS 
    public struct RFID_18K6C_SINGULATION_ALGORITHM_PARMS_T
    {
        [FieldOffset(0)]
        public RFID_18K6C_SINGULATION_ALGORITHM singulationAlgorithm;
        [FieldOffset(4)]
        public RFID_18K6C_SINGULATION_FIXEDQ_PARMS fixedQ;
        [FieldOffset(4)]
        public RFID_18K6C_SINGULATION_DYNAMICQ_PARMS dynamicQ;

        // return true if same, false differs
        public bool SameAs(ref RFID_18K6C_SINGULATION_ALGORITHM_PARMS_T anotherParm)
        {
            bool Result = false;

            if (singulationAlgorithm == anotherParm.singulationAlgorithm)
            {
                switch (singulationAlgorithm)
                {
                    case RFID_18K6C_SINGULATION_ALGORITHM.RFID_18K6C_SINGULATION_ALGORITHM_FIXEDQ:
                        Result = fixedQ.SameAs(ref anotherParm.fixedQ);
                        break;
                    case RFID_18K6C_SINGULATION_ALGORITHM.RFID_18K6C_SINGULATION_ALGORITHM_DYNAMICQ:
                        Result = dynamicQ.SameAs(ref anotherParm.dynamicQ);
                        break;
                }
            }
            else
                Result = false;

            return Result;
        }
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_18K6C_QUERY_PARMS
    {
        public RFID_18K6C_TAG_GROUP tagGroup;
        public RFID_18K6C_SINGULATION_ALGORITHM_PARMS_T singulationParms;

        public void Clone(out RFID_18K6C_QUERY_PARMS dst)
        {
            dst = new RFID_18K6C_QUERY_PARMS();
            dst.tagGroup = this.tagGroup;
            dst.singulationParms.singulationAlgorithm = this.singulationParms.singulationAlgorithm;
            switch (this.singulationParms.singulationAlgorithm)
            {
                case RFID_18K6C_SINGULATION_ALGORITHM.RFID_18K6C_SINGULATION_ALGORITHM_FIXEDQ:
                    dst.singulationParms.fixedQ = this.singulationParms.fixedQ;
                    break;
                case RFID_18K6C_SINGULATION_ALGORITHM.RFID_18K6C_SINGULATION_ALGORITHM_DYNAMICQ:
                    dst.singulationParms.dynamicQ = this.singulationParms.dynamicQ;
                    break;
            }
        }

        public bool SameAs(ref RFID_18K6C_QUERY_PARMS anotherParm)
        {
            return (this.tagGroup.SameAs(ref anotherParm.tagGroup)
                && this.singulationParms.SameAs(ref anotherParm.singulationParms));
        }
    } ;
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_18K6C_INVENTORY_PARMS
    {
        public UInt32 length; //= sizeof(RFID_18K6C_INVENTORY_PARMS);
        public RFID_18K6C_COMMON_PARMS common;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_18K6C_READ_PARMS
    {
        public UInt32 length;
        public RFID_18K6C_COMMON_PARMS common;
        public RFID_18K6C_MEMORY_BANK bank;
        public UInt16 offset;
        public UInt16 count;
        public UInt32 accessPassword;
    };
    // make it safe by using ValAsArray marshaling (fixed size anyway)
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_18K6C_WRITE_SEQUENTIAL_PARMS_T
    {
        public UInt32 length; //= sizeof(RFID_18K6C_WRITE_SEQUENTIAL_PARMS); 
        public RFID_18K6C_MEMORY_BANK bank;
        public UInt16 count;  //1-8
        public UInt16 offset;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = RfidSp.RFID_18K6C_MAX_TAG_WRITE_WORDS)]
        //        [MarshalAs(UnmanagedType.LPArray)]
        public ushort[] pData;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_18K6C_WRITE_RANDOM_PARMS_T
    {
        public UInt32 length; //= sizeof(RFID_18K6C_WRITE_RANDOM_PARMS) (typo); 
        public RFID_18K6C_MEMORY_BANK bank;
        public UInt16 count;   //1-8
        public UInt16 reserved;//=0  
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = RfidSp.RFID_18K6C_MAX_TAG_WRITE_WORDS)]
        //        [MarshalAs(UnmanagedType.LPArray)]
        public ushort[] pOffset;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = RfidSp.RFID_18K6C_MAX_TAG_WRITE_WORDS)]
        //[MarshalAs(UnmanagedType.LPArray)]
        public ushort[] pData;
    };
    // preserve union type data marshaling
    [StructLayout(LayoutKind.Explicit)]
    public struct RFID_18K6C_WRITE_PARMS_T
    {
        [FieldOffset(0)]
        public UInt32 length; //= sizeof(RFID_18K6C_WRITE_PARMS);
        [FieldOffset(4)]
        public RFID_18K6C_COMMON_PARMS common; // size 16
        [FieldOffset(20)]
        public RFID_18K6C_WRITE_TYPE writeType; // size 4
        // union begin
        [FieldOffset(24)]
        // size =  (4+4+2+2+2*RfidSp.RFID_18K6C_MAX_TAG_WRITE_WORDS)
        // or size = (4+4+2+2+4)
        public RFID_18K6C_WRITE_SEQUENTIAL_PARMS_T sequential;
        [FieldOffset(24)]
        // size =  (4+4+2+2+2*RfidSp.RFID_18K6C_MAX_TAG_WRITE_WORDS 
        //                + 2*RfidSp.RFID_18K6C_MAX_TAG_WRITE_WORDS)
        // or size = (4+4+2+2+4+4)
        public RFID_18K6C_WRITE_RANDOM_PARMS_T random;
        // union ends
        [FieldOffset(/*44*/68/*164*/)]
        public UInt32 verify; //BOOL32 0 write-only; >0 w+r verify data
        [FieldOffset(/*48*/72/*168*/)]
        public UInt32 verifyRetryCount; //0-7
        [FieldOffset(/*52*/76/*172*/)]
        public UInt32 accessPassword;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_18K6C_KILL_PARMS
    {
        public UInt32 length; //= sizeof(RFID_18K6C_KILL_PARMS); 
        public RFID_18K6C_COMMON_PARMS common;
        public UInt32 accessPassword;
        public UInt32 killPassword;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_18K6C_LOCK_PARMS
    {
        public UInt32 length; //= sizeof(RFID_18K6C_LOCK_PARMS)
        public RFID_18K6C_COMMON_PARMS common;
        public RFID_18K6C_TAG_PERM permissions;
        public UInt32 accessPassword;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct RFID_18K6C_TAG_PERM
    {
        public RFID_18K6C_TAG_PWD_PERM killPasswordPermissions;
        public RFID_18K6C_TAG_PWD_PERM accessPasswordPermissions;
        public RFID_18K6C_TAG_MEM_PERM epcMemoryBankPermissions;
        public RFID_18K6C_TAG_MEM_PERM tidMemoryBankPermissions;
        public RFID_18K6C_TAG_MEM_PERM userMemoryBankPermissions;
    };
    #endregion

    public enum RFID_ANTENNA_PWRLEVEL
    {
    
        LOW = 1,
        MEDIUM = 2,
        HIGH = 3
    }


    ///////////////////////////////////////////////////////////////////////////////////////
    #region RfidMw_enums

    #endregion
    ///////////////////////////////////////////////////////////////////////////////////////
    #region SettingMgt_enums

    #endregion

    //public enum BUZZER_SOUND : uint
    //{
    //    BUZZER_LOW_SOUND,
    //    BUZZER_MIDDLE_SOUND,
    //    BUZZER_HIGH_SOUND
    //};
    //public enum WIFISTATE : uint
    //{
    //    WIFI_CONNECTION,
    //    WIFI_SCANNING,
    //    WIFI_DISCONNCET
    //};
}
