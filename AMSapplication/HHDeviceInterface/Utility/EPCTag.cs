using System;
using System.Collections.Generic;
using System.Text;
//using ClslibRfidSp;
//using ClsReaderLib.Devices.Reader;
using ReaderTypes;

namespace OnRamp
{

    public class EPCTag
    {      

        #region Tag Password Routines
        static public UInt32 PasswdStrToUInt32(String pwdStr)
        {
            UInt32 pwdUInt32 = 0;

            try
            {
                for (int i = 0, s=28; i < pwdStr.Length; i++, s-=4)
                {
                    byte nibble = Byte.Parse(pwdStr.Substring(i, 1), System.Globalization.NumberStyles.HexNumber);
                    pwdUInt32 |= (uint)((nibble & 0x0f)  << s);
                }
            }
            catch (Exception e)
            {
                Datalog.LogStr("Byte Parse " + pwdStr + " failed with reason: " + e.Message);
                pwdUInt32 = 0; // reset to 0 to be safe
            }
             
            return pwdUInt32;
        }
        static public String PasswdUInt32ToStr(UInt32 pwdUInt32)
        {
            return pwdUInt32.ToString("X8");
        }

        #endregion
        #region TagAccessPacket utilities
        static private String DataFieldToStr(UInt32[] dFieldArr, int nBytes)
        {
            int i;
            StringBuilder DataStrBldr;
#if false // compiler error
            if (sizeof(UInt32) != 4)
                throw new ApplicationException ("Bad UInt32 size assumption");
#endif
            DataStrBldr = new StringBuilder(nBytes);
            for (i = 0; i < (nBytes / 4); i++)
            {
                DataStrBldr.AppendFormat(null, "{0:X2}{1:X2}{2:X2}{3:X2}",
                    (dFieldArr[i] >> 24) & 0x000000FF, (dFieldArr[i] >> 16) & 0x000000FF,
                    (dFieldArr[i] >> 8) & 0x000000FF, (dFieldArr[i]) & 0x000000FF);
            }
            // remaining bytes
            int remBytes = nBytes % 4;
            int j, shiftCnt = 0;
            // MSB first
            for (j = 0; j < remBytes; j++)
            {
                shiftCnt = (3 - j) * 8;
                DataStrBldr.AppendFormat(null, "{0:X2}", (dFieldArr[i] >> shiftCnt) & (0x000000FF));
            }

            return DataStrBldr.ToString();
        }

        static public int TagAccPktGetDataFieldSize(UInt16 cmnPktLen, Byte cmnFlags)
        {
            // Note: IntelSampleLive (original RfidMw) uses "pkt_len - 1"(follow Intel code sample) 
            // instead of pkt_len -3 (as in Intel doc).
            // Based on observation of Tracer output, Intel docs makes more sense,
            // this code works with modified RfidMw
            return ((cmnPktLen - 3) * 4) - ((cmnFlags >> 6) & 0x03);
        }

        static public AccErrorTypes TagAccPktGetErrType(ref RFID_PACKETMSG_18K6C_TAG_ACCESS_T TAPkt)
        {
             AccErrorTypes TAErr =  AccErrorTypes.None;

            if ((TAPkt.cmn.flags & 0x01) != 0)
            {
                // CRC Error
                if ((TAPkt.cmn.flags & 0x08) != 0)
                {
                    TAErr = AccErrorTypes.CRC;
                }
                // BackScatter Error
                else if ((TAPkt.cmn.flags & 0x02) != 0)
                {
                    switch (TAPkt.error_code)
                    {
                        case 0x03: /* Memory Location error */
                            TAErr = AccErrorTypes.InvalidAddr; break;
                        case 0x04: /* Memory location not writable/locked */
                            TAErr = AccErrorTypes.Unauthorized; break;
                        case 0x00: /* catch-all */
                        case 0x0B: /* low power */
                        case 0x0F: /* unrecognized error code */
                        default:
                            TAErr = AccErrorTypes.Others; break;
                    }
                }
                // ACK Timeout
                else if ((TAPkt.cmn.flags & 0x04) != 0)
                {
                    TAErr = AccErrorTypes.AckTimeout;
                }
                // Error code in tag_data field
                else
                {
                    int DataSize = TagAccPktGetDataFieldSize(TAPkt.cmn.pkt_len, TAPkt.cmn.flags);
                    String DataStr;
                    DataStr = EPCTag.DataFieldToStr(TAPkt.tag_data, DataSize);
                    Byte ErrCode = Byte.Parse(DataStr.Substring(0, 2),
                        System.Globalization.NumberStyles.HexNumber);
                    if (ErrCode >= 0x01 && ErrCode <= 0x18)
                        TAErr = (AccErrorTypes)((int)AccErrorTypes.VerifyError + (ErrCode - 0x01));
                    else
                        throw new ApplicationException("Unexpected TagAccess error code:" + ErrCode);
                }
            }
            return TAErr;
        }

        static public String TagAccPktGetDataStr(ref RFID_PACKETMSG_18K6C_TAG_ACCESS_T TAPkt)
        {
            int DataSize = TagAccPktGetDataFieldSize(TAPkt.cmn.pkt_len,
                                          Rfid.st_RfidSpPkt_TagAccessData.cmn.flags);
            String DataStr;
            DataStr = DataFieldToStr(TAPkt.tag_data, DataSize);

            return DataStr;
        }
        #endregion

        #region Tag Characteristics
        static private UInt16[] BankSz; // in words

        static public UInt16 MaxBnkMemSizeInWds(RFID_18K6C_MEMORY_BANK bnk)
        {
            return BankSz[T18K6CBankToIdx(bnk)];
        }

        static public UInt16 MaxBnkMemSizeInBits(RFID_18K6C_MEMORY_BANK bnk)
        {
            return (ushort)(BankSz[T18K6CBankToIdx(bnk)] * 16);
        }

        static public ushort CRC16FldSz
        {
            get
            {
                return 1;
            }
        }
        static public ushort PCFldSz
        {
            get
            {
                return 1;
            }
        }
        static public ushort EPCFldSz
        {
            get
            {
                return 6;
            }
        }

        static public ushort KillPwdFldSz
        {
            get
            {
                return 2;
            }
        }

        static public ushort AccPwdFldSz
        {
            get
            {
                return 2;
            }
        }

        static EPCTag() // Static Constructors
        {
            _AvailBanks = MemoryBanks4Op.Zero
                                | MemoryBanks4Op.One
                                | MemoryBanks4Op.Two;
            BankSz = new ushort[4] {
                    2 + 2, // access + kill passwords
                    (ushort)(CRC16FldSz + PCFldSz + EPCFldSz), // CRC + PC + EPC
                    1 + 1, // Allocation-Class-ID + Class-Functionality
                     0
            };
           
        }
        static private MemoryBanks4Op _AvailBanks;
        static public  MemoryBanks4Op AvailBanks
        {
            get 
            {
                return _AvailBanks;
            }
        }
        static public void AvailBanksAdd (MemoryBanks4Op bnk, ushort numWds)
        {
            // Only affect TID[Vendor Area] and User Memory
            switch (bnk)
            {
                case MemoryBanks4Op.Two:
                    BankSz[2] = (ushort)(1 + 1 + numWds);
                    break;
                case MemoryBanks4Op.Three:
                    _AvailBanks |= MemoryBanks4Op.Three;
                    BankSz[3] = numWds;
                    break;
            }
        }

        static public void AvailBanksSub(MemoryBanks4Op bnk)
        {
            // Only affect TID[Vendor Area] and User Memory
            switch (bnk)
            {
                case MemoryBanks4Op.Two:
                    BankSz[2] = 1 + 1;
                    break;
                case MemoryBanks4Op.Three:
                    _AvailBanks &= ~(MemoryBanks4Op.Three);
                    BankSz[3] = 0;
                    break;
            }
        }

        // Application-specific Bank Access Order
        static public MemoryBanks4Op NextBankToAcc(ref MemoryBanks4Op selBanks)
        {
            MemoryBanks4Op nxtBnk = CurBankToAcc(selBanks);

            selBanks &= ~nxtBnk;
            
            return nxtBnk;
        }

        static public MemoryBanks4Op CurBankToAcc(MemoryBanks4Op selBanks)
        {
            MemoryBanks4Op nxtBnk = MemoryBanks4Op.None;

            if ((selBanks & MemoryBanks4Op.Zero) != 0)
                nxtBnk = MemoryBanks4Op.Zero;
            else if ((selBanks & MemoryBanks4Op.One) != 0)
                nxtBnk = MemoryBanks4Op.One;
            else if ((selBanks & MemoryBanks4Op.Two) != 0)
                nxtBnk = MemoryBanks4Op.Two;
            else if ((selBanks & MemoryBanks4Op.Three) != 0)
                nxtBnk = MemoryBanks4Op.Three;

            return nxtBnk;
        }

        // MemoryBanks4Op to RFID_18K6C_MEMORY_BANK
        static public RFID_18K6C_MEMORY_BANK MemoryBanks4OpTo18K6CBank(MemoryBanks4Op B4Op)
        {
            RFID_18K6C_MEMORY_BANK Bnk = RFID_18K6C_MEMORY_BANK.RFID_18K6C_MEMORY_BANK_RESERVED;

            if ((B4Op & MemoryBanks4Op.Zero) != 0)
                Bnk = RFID_18K6C_MEMORY_BANK.RFID_18K6C_MEMORY_BANK_RESERVED;
            else if ((B4Op & MemoryBanks4Op.One) != 0)
                Bnk = RFID_18K6C_MEMORY_BANK.RFID_18K6C_MEMORY_BANK_EPC;
            else if ((B4Op & MemoryBanks4Op.Two) != 0)
                Bnk = RFID_18K6C_MEMORY_BANK.RFID_18K6C_MEMORY_BANK_TID;
            else if ((B4Op & MemoryBanks4Op.Three) != 0)
                Bnk = RFID_18K6C_MEMORY_BANK.RFID_18K6C_MEMORY_BANK_USER;

            return Bnk;
        }
        static public MemoryBanks4Op T18K6CBankToMemoryBanks4Op(RFID_18K6C_MEMORY_BANK T18K6CBnk)
        {
            MemoryBanks4Op B4Op = MemoryBanks4Op.None;

            switch (T18K6CBnk)
            {
                case RFID_18K6C_MEMORY_BANK.RFID_18K6C_MEMORY_BANK_RESERVED:
                    B4Op = MemoryBanks4Op.Zero;
                    break;
                case RFID_18K6C_MEMORY_BANK.RFID_18K6C_MEMORY_BANK_EPC:
                    B4Op = MemoryBanks4Op.One;
                    break;
                case RFID_18K6C_MEMORY_BANK.RFID_18K6C_MEMORY_BANK_TID:
                    B4Op = MemoryBanks4Op.Two;
                    break;
                case RFID_18K6C_MEMORY_BANK.RFID_18K6C_MEMORY_BANK_USER:
                    B4Op = MemoryBanks4Op.Three;
                    break;
            }
            return B4Op;
        }

        static public uint MemoryBanks4OpToIdx(MemoryBanks4Op B4Op)
        {
            uint Idx = 0;

            switch (B4Op)
            {
                case MemoryBanks4Op.Zero:
                    Idx = 0;
                    break;
                case MemoryBanks4Op.One:
                    Idx = 1;
                    break;
                case MemoryBanks4Op.Two:
                    Idx = 2;
                    break;
                case MemoryBanks4Op.Three:
                    Idx = 3;
                    break;
                default:
                    throw new ApplicationException("Unexpected bank number");
            }
            return Idx;
        }

        // Return Bank Index from Enum : 0 - 3
        static public int T18K6CBankToIdx(RFID_18K6C_MEMORY_BANK T18K6CBnk)
        {
            int Idx;

            switch (T18K6CBnk)
            {
                case RFID_18K6C_MEMORY_BANK.RFID_18K6C_MEMORY_BANK_RESERVED:
                    Idx = 0;
                    break;
                case RFID_18K6C_MEMORY_BANK.RFID_18K6C_MEMORY_BANK_EPC:
                    Idx = 1;
                    break;
                case RFID_18K6C_MEMORY_BANK.RFID_18K6C_MEMORY_BANK_TID:
                    Idx = 2;
                    break;
                case RFID_18K6C_MEMORY_BANK.RFID_18K6C_MEMORY_BANK_USER:
                    Idx = 3;
                    break;
                default:
                    throw new ApplicationException("Unexpected RFID_18K6C_MEMORY_BANK");
            }
            return Idx;
        }
        #endregion

        #region Preset Vendor Memory Config
     

        public static bool TagVendorMemBnkSz(TagVendor vndr, out MemBnkSz sz)
        {
            bool Succ = false;

            sz = new MemBnkSz(2, 0);
            switch (vndr)
            {
                case TagVendor.Any:
                    sz = new MemBnkSz(2, 0);
                    Succ = true;
                    break;
                case TagVendor.NXP:
                    sz = new MemBnkSz(4, 14);
                    Succ = true;
                    break;
                default:
                    Succ = false;
                    break;
            }
            return Succ;
        }

        #endregion
    }
}
