using System;
using System.Collections.Generic;

using System.Text;


namespace ReaderTypes
{
    #region enum decls

    public enum AccErrorTypes
    {
        None,
        CRC,
        InvalidAddr,
        Unauthorized, // locked
        AckTimeout,
        Others, // ErrorCode: low-power, catch-all
        // Present in tag_data field
        VerifyError, // 0x01
        WriteCmdError, //0x02
        WriteResponseCrcError,//0x03
        WriteVerifyCrcError,//0x04
        WriteRetryCountExceeded, //0x05
        WriteError, // 0x06
        ReadError, // 0x07
        ReadRN16Error, // 0x07
        ReadCountInvalid, // 0x0A
        ReadRetryCountExceeded, //0x0B
        KillOrLockResponseError,//0x0C
        KillOrLockResponseCrcError,
        KillOrLockCmdError,
        KillOrLockInvalidHandle,//0x0D
        AccessPasswordError, // 0x0E (not in doc, but in code)
        BlockEraseCmdError,
        BlockEraseResponseCrcError,
        BlockEraseError,
        EraseRetryCountExceeded,
        BlockWriteCmdError,
        BlockWriteResponseCrcError,
        BlockWriteVerifyCrcError,
        BlockWriteRetryCountExceeded,
        BlockWriteError,
        totalNumDefined, // only use as count of AccErrTypes defined in this enum
    }

    public enum InvtryOpStatus
    {
        none,
        configured,
        started,
        cycBegin,
        cycEnd,
        errorStopped, // stopped with error 
        macErrorStopped, // stopped normally, but mac_error_register not 0
        stopped,
        error,
        intervalTimeRpt, // per-cycle?
    }
    public enum RdOpStatus
    {
        configured,
        started,
        stopped,
        completed, // completed a TagRd
        error,
        tagAccError,
        errorStopped,
    }
    public enum WrOpStatus
    {
        configured,
        started,
        stopped,
        completed, // completed a TagWrite (whether it's 'targeted' or not).
        error,
        tagAccError,
        errorStopped,
    }
    public enum LckOpStatus
    {
        configured,
        started,
        stopped,
        error,
        tagAcc, // Tag accessed successfully
        tagAccError, // Tag accessed with error
        errorStopped,
    }
    public enum HealthChkOpStatus
    {
        Idle,
        LnkProf,
        AntPwr,
        FreqChns,
        TagInvtrySetup,
        TagInvtryStarted,
        TagInvtryGotTag,
        TagInvtryStopped,
        errorStopped,
    }
    [Flags] // bit fields
    public enum MemoryBanks4Op : uint
    {
        None = 0x00,
        Zero = 0x01,
        One = 0x02,
        Two = 0x04,
        Three = 0x08
    }

    public enum RadioStatus : uint
    {
        Closed,         // Radio handle Close, Shutdown
        Closing,        // Close, Shutdown in progress
        ClosePend,   // Close requested once current command finished
        Opening,      // Startup, Enumerate, Open
        Opened,       // Open success but unable to start configuration somehow
        Configuring, // Cont mode
        Configured, // Done configuration, doesn't mean config success
        PowerSaved, // In power-save mode
        ErrorStopped, // Radio Open failed
    };

    public enum RadioOpRes : uint
    {
        // Only use in notification callback, not device state
        None,
        StateChanged,
        Error,
        Warning,
    };
    #endregion

    #region events
    // EventArgs for DiscoveredTag Notification
    
    public class DscvrTagEventArgs : System.EventArgs
    {
        public readonly UInt16 PC;
        public readonly UINT96_T EPC;
        public readonly UInt16 Cnt;
        public readonly float RSSI;
        public readonly FileTime UpdateTm;
        public readonly UInt16[] PCArr;
        public readonly UINT96_T[] EPCArr;
        public readonly FileTime[] LastInvtryTmArr;
        public readonly float[] RSSIArr;

        public DscvrTagEventArgs(UInt16[] PCArr, UINT96_T[] EPCArr, float[] RSSIArr, FileTime[] FTArr)
        {
            // for initialization only, meaningless in this case
            PC = 0;
            EPC = new UINT96_T();
            Cnt = 0;
            RSSI = 0.0F;
            UpdateTm = new FileTime();
            this.PCArr = PCArr != null ? ((UInt16[])PCArr.Clone()) : null;
            this.EPCArr = EPCArr != null ? ((UINT96_T[])EPCArr.Clone()) : null; // shallow copy
            this.RSSIArr = RSSIArr != null ? ((float[])RSSIArr.Clone()) : null;
            this.LastInvtryTmArr = FTArr != null ? ((FileTime[])FTArr.Clone()) : null; // shallow copy
        }

        public DscvrTagEventArgs(UInt16 PC, ref UINT96_T EPC, UInt16 cnt, float wbRSSI, FileTime fTime)
        {
            this.PC = PC;
            this.EPC = EPC;
            Cnt = cnt;
            RSSI = wbRSSI;
            UpdateTm = fTime;
        }

        public DscvrTagEventArgs(UInt16 PC, ref UINT96_T EPC)
        {
            this.PC = PC;
            this.EPC = EPC;
            Cnt = 0;
            RSSI = 0.0F;
            UpdateTm = new FileTime();
        }
    }

    // EventArgs for Tag-Rate Notification (RFID_PACKET_TYPE_MSGID_CUSTOM_TAGRATE)
    public class TagRateEventArgs : System.EventArgs
    {
        public readonly UInt32 Period_ms;
        public readonly UInt32 TagCnt;

        public TagRateEventArgs(UInt32 period, UInt32 tagCnt)
        {
            Period_ms = period;
            TagCnt = tagCnt;
        }
    }

    // EventArgs for Tag-Rate Notification (RFID_PACKET_TYPE_MSGID_CUSTOM_RSSI)
    public class TagRssiEventArgs : System.EventArgs
    {
        public readonly float RSSI;

        public TagRssiEventArgs(float RSSI)
        {
            this.RSSI = RSSI;
        }
    }

    // EventArgs for InvtryOpStatus Notification
    public class InvtryOpEventArgs : System.EventArgs
    {
        public readonly InvtryOpStatus status;
        public readonly String msg; // mostly for error message
        public readonly Int16 AmbTemp;
        public readonly Int16 XcvrTemp;
        public readonly Int16 PATemp;
        public readonly UInt32 ElapsedTime; // ms passed since COMMAND begin

        public InvtryOpEventArgs(InvtryOpStatus update)
        {
            this.msg = null;
            status = update;
        }
        public InvtryOpEventArgs(InvtryOpStatus update, String msg)
        {
            this.msg = msg;
            status = update;
        }

        public InvtryOpEventArgs(InvtryOpStatus tempStatus, Int16 amb, Int16 xcvr, Int16 pwrAmp)
        {
            status = tempStatus;
            AmbTemp = amb;
            XcvrTemp = xcvr;
            PATemp = pwrAmp;
        }

        public InvtryOpEventArgs(InvtryOpStatus timeElapsed, UInt32 msElapsed)
        {
            status = timeElapsed;
            ElapsedTime = msElapsed;
        }
    }

    // EventArgs for RdOpStatus Notification
    public class RdOpEventArgs : System.EventArgs
    {
        public readonly RdOpStatus Status;
        public readonly String ErrMsg;
        public readonly AccErrorTypes TagAccErr;

        public RdOpEventArgs(RdOpStatus update, String errMsg,
           AccErrorTypes taErr)
        {
            Status = update;
            ErrMsg = errMsg;
            TagAccErr = taErr;
        }
    }

    // EventArgs for MemBankRead Notification
    public class MemBnkRdEventArgs : System.EventArgs
    {
        public readonly UInt16 CRC;
        public readonly UInt16 PC;
        public readonly UINT96_T EPC;
        public readonly MemoryBanks4Op BankNum;
        public readonly String Data;
        public readonly UInt16 RSSI;

        //public MemBnkRdEventArgs(ref UInt16 crc, ref UInt16 pc, ref UINT96_T epc,
        //   RFID_18K6C_MEMORY_BANK bankNum, String data, UInt16 RSSI)
        //{

        //}

        public MemBnkRdEventArgs(ref UInt16 crc, ref UInt16 pc, ref UINT96_T epc,
            RFID_18K6C_MEMORY_BANK bankNum, String data, UInt16 RSSI)
        {
            this.CRC = crc;
            this.PC = pc;
            this.EPC = epc; // shallow copy is enough

            switch (bankNum)
            {
                case RFID_18K6C_MEMORY_BANK.RFID_18K6C_MEMORY_BANK_RESERVED:
                    this.BankNum = MemoryBanks4Op.Zero;
                    break;
                case RFID_18K6C_MEMORY_BANK.RFID_18K6C_MEMORY_BANK_EPC:
                    this.BankNum = MemoryBanks4Op.One;
                    break;
                case RFID_18K6C_MEMORY_BANK.RFID_18K6C_MEMORY_BANK_TID:
                    this.BankNum = MemoryBanks4Op.Two;
                    break;
                case RFID_18K6C_MEMORY_BANK.RFID_18K6C_MEMORY_BANK_USER:
                    this.BankNum = MemoryBanks4Op.Three;
                    break;
                default:
                    this.BankNum = MemoryBanks4Op.Zero;
                    break;
            }

            this.Data = data;
            this.RSSI = RSSI;
        }
    }

    // EventArgs for WrOpStatus Notification (TAG_ACCESS (err) + MwTagWrite)
    public class WrOpEventArgs : System.EventArgs
    {
        public readonly WrOpStatus Status;
        public readonly String ErrMsg;
        public readonly AccErrorTypes TagAccErr;
        public readonly UInt16 AccTagPC;
        public readonly UINT96_T AccTagEPC;

        public WrOpEventArgs(WrOpStatus update, String errMsg,
            AccErrorTypes tagErr, UInt16 tagPC, UINT96_T tagEPC)
        {
            Status = update;
            ErrMsg = errMsg;
            TagAccErr = tagErr;
            AccTagPC = tagPC;
            AccTagEPC = tagEPC;
        }

        public WrOpEventArgs(WrOpStatus update, String errMsg)
        {
            Status = update;
            ErrMsg = errMsg;
            TagAccErr = AccErrorTypes.None;
            AccTagPC = 0;
            AccTagEPC = new UINT96_T();
        }
    }

    // EventArgs for MemBankWrite Notification (AddATag + TAG_ACCESS)
    public class MemBnkWrEventArgs : System.EventArgs
    {
        public readonly UINT96_T EPC;
        public readonly MemoryBanks4Op BankNum;
        public readonly ushort wdOffset;
        public readonly ushort wdCnt;
        public MemBnkWrEventArgs(ref UINT96_T epc, MemoryBanks4Op bnk)
        {
            this.EPC = epc;
            this.BankNum = bnk;
        }

        public MemBnkWrEventArgs(ref UINT96_T epc, MemoryBanks4Op bnk, ushort offset, ushort cnt)
        {
            this.EPC = epc;
            this.BankNum = bnk;
            this.wdOffset = offset;
            this.wdCnt = cnt;
        }
    }

    public class LckOpEventArgs : System.EventArgs
    {
        public readonly LckOpStatus Status;
        public readonly String ErrMsg;
        public readonly AccErrorTypes TagAccErr;

        public LckOpEventArgs(LckOpStatus update, String errMsg)
        {
            Status = update;
            ErrMsg = errMsg;
            TagAccErr = AccErrorTypes.None;
        }

        public LckOpEventArgs(LckOpStatus update, String errMsg,
            AccErrorTypes taErr)
        {
            Status = update;
            ErrMsg = errMsg;
            TagAccErr = taErr;
        }
    }
    #endregion

    #region
    public delegate bool ReqTagRdBnkInfo(out MemoryBanks4Op bnk, out ushort wdOffset, out ushort wdCnt);

    public delegate bool ReqTagWrBnkData(out MemoryBanks4Op bnk, out ushort wdOffset, out String dataStr); // ask the callee for EPC, 

    public delegate void AntPortStatusGetAllNotify(bool succ, RFID_ANTENNA_PORT_STATUS[] ports, string errMsg);
    public delegate void AntPortCfgGetOneNotify(bool succ, uint portNum, ref RFID_ANTENNA_PORT_CONFIG cfg, string errMsg);
    public delegate void AntPortCfgSetOneNotify(bool succ, uint portNum, string errMsg);
    public delegate void ThrshTempGetNotify(ushort amb, ushort xcvr, ushort pwrAmp, ushort paDetla);
    public delegate void CustomFreqGetNotify(bool succ, CustomFreqGrp freqGrp, int chn, bool enLBT);
    public delegate void HealthCheckStatusNotify(HealthChkOpStatus status, string msg);
    public delegate void CustomFreqSetNotify(bool succ, String errMsg);
    public delegate void LnkProfInfoGetNotify(bool succ, ref RFID_RADIO_LINK_PROFILE lnkPrf, String errMsg);
    public delegate void LnkProfNumSetNotify(bool succ, String errMsg);

    public delegate void RespDatModeSetNotify(bool succ, string errMsg);

    public delegate void RadioStatusNotify(RadioOpRes res, RadioStatus status, String msg);

    public delegate bool CodeRcvdNotify(bool Succ, String codeStr, String errStr);
    public delegate void StopNotify();
    
    #endregion

    public struct MemBnkSz
    {
        private uint b2;
        private uint b3;

        public MemBnkSz(uint b2Sz, uint b3Sz)
        {
            b2 = b2Sz;
            b3 = b3Sz;
        }

        public uint Bnk2
        {
            get
            {
                return b2;
            }
        }

        public uint Bnk3
        {
            get
            {
                return b3;
            }
        }
    }

    public enum TagVendor
    {
        Any = 0, // default setting
        NXP = 1
    }

    public class TempUpdateEventArgs : EventArgs
    {
        public readonly UInt16 Amb;
        public readonly UInt16 Xcvr;
        public readonly UInt16 PA;

        public TempUpdateEventArgs(UInt16 amb, UInt16 xcvr, UInt16 pa)
        {
            Amb = amb;
            Xcvr = xcvr;
            PA = pa;
        }
    }

    public enum HardwareSelection
    {
        None,
        CS101Reader,
        AT870Reader

    }

    // public delegate bool ReqTagRdBnkInfo(out MemoryBanks4Op bnk, out ushort wdOffset, out ushort wdCnt);
}
