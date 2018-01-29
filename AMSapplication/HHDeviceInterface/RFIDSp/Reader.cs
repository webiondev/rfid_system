using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using ReaderTypes;


namespace HHDeviceInterface.RFIDSp
{
    public abstract class Reader
    {
        #region CS101

        //public Reader GetReader(string str)
        //{
        //    //Ree
        //}

        #region delegate Parameters
        public delegate void EPCEvent(string epc, string rssi);

        public EventHandler<DscvrTagEventArgs> GetTagEventArgs;
        public EventHandler<InvtryOpEventArgs> InventoryOpEventArgs;
        public EventHandler<LckOpEventArgs> LockOpEventArgs;
        public EventHandler<MemBnkRdEventArgs> MemberBankReadEvenArgs;
        public EventHandler<MemBnkWrEventArgs> MemberBankWriteEventArgs;
        public EventHandler<RdOpEventArgs> ReadOpEventArgs;
        public EventHandler<TagRateEventArgs> TagRateEventArgs;
        public EventHandler<TagRssiEventArgs> TagRessiEventArgs;
        public EventHandler<TempUpdateEventArgs> TempUpdateEventArgs;
        public EventHandler<WrOpEventArgs> WriteOpEventArgs;
        

        
        
        public ReqTagWrBnkData TagWriteBankData;
        public ReqTagRdBnkInfo TagReadBankData;
        public CustomFreqGetNotify CustomFreqGetNotification;
        public AntPortCfgGetOneNotify AntPotCfgGetOneNotification;
        public AntPortCfgSetOneNotify AntPotCfgSetOneNotification;
        public RadioStatusNotify RadioStatusNotification;
        public LnkProfInfoGetNotify LnkProfInfoGetNotification;
        public LnkProfNumSetNotify LnkProfNumoSetNotification;
        // public event CustomFreqSetNotify CustomFreqSetNotification;
        public HealthCheckStatusNotify HealthCheckStatusNotification;
        public ThrshTempGetNotify ThrshTempGetNotification;
        public RespDatModeSetNotify eventRespDataMode;
       

        #endregion

        #region Parameters
        public uint AccessPassword { get; set; }
        public uint NumberTags { get; set; }
        public byte[] LastWrittenEPC { get; set; }
        public UINT96_T TagtEPC;
        public UInt32 CurAccPasswd { get; set; }
        public UInt32 ProfNum;
        public UInt32 bandNum;
        public UInt32 PwrLevRegVal;
        public UInt32 RevPwrLevRegVal;
        public UInt32 RevPwrThrshRegVal;
        public UInt32 Size;
        public int power;
        public UInt32 PortNum;
        public ushort amb, xcvr, pamp;
        public int frequency;
        public SoundVol Svol;
        public RingTone RingingID;
        public UInt32 lnkProfNum { get; set; }
        public static ushort macerr;
        public RFID_VERSION DriverVer
        {
            get
            {
                return Rfid.st_RfidSpReq_Startup.LibraryVersion;
            }
        }    
        public RFID_VERSION MacVer
        {
            get
            {
                return Rfid.st_RfidSpReq_MacGetVersion.version;
            }
        }
        public string WriteTagEPC;
        #endregion

        #region Events
        abstract public void RegisterDscvrTagEvent(EventHandler<DscvrTagEventArgs> handler);
        abstract public void UnregisterDscvrTagEvent(EventHandler<DscvrTagEventArgs> handler);

        abstract public void RegisterInventoryOpStEvent(EventHandler<InvtryOpEventArgs> handler);
        abstract public void UnregisterInventoryOpStEvent(EventHandler<InvtryOpEventArgs> handler);

        abstract public void RegisterLckOpStEvent(EventHandler<LckOpEventArgs> handler);
        abstract public void UnregisterLckOpStEvent(EventHandler<LckOpEventArgs> handler);

        abstract public void RegisterMemBnkRdEvent(EventHandler<MemBnkRdEventArgs> handler);
        abstract public void UnregisterMemBnkRdEvent(EventHandler<MemBnkRdEventArgs> handler);

        abstract public void RegisterMemBnkWrEvent(EventHandler<MemBnkWrEventArgs> handler);
        abstract public void UnregisterMemBnkWrEvent(EventHandler<MemBnkWrEventArgs> handler);

        abstract public void RegisterRdOpStEvent(EventHandler<RdOpEventArgs> handler);
        abstract public void UnregisterRdOpStEvent(EventHandler<RdOpEventArgs> handler);

        abstract public void RegisterTagPermSetEvent(EventHandler<DscvrTagEventArgs> handler);
        abstract public void UnregisterTagPermSetEvent(EventHandler<DscvrTagEventArgs> handler);

        abstract public void RegisterTagRateEvent(EventHandler<TagRateEventArgs> handler);
        abstract public void UnregisterTagRateEvent(EventHandler<TagRateEventArgs> handler);

        abstract public void RegisterTagRssiEvent(EventHandler<TagRssiEventArgs> handler);
        abstract public void UnregisterTagRssiEvent(EventHandler<TagRssiEventArgs> handler);

        abstract public void RegisterTempUpdateEvt(EventHandler<TempUpdateEventArgs> handler);
        abstract public void UnregisterTempUpdateEvt(EventHandler<TempUpdateEventArgs> handler);

        abstract public void RegisterWrOpStEvent(EventHandler<WrOpEventArgs> handler);
        abstract public void UnregisterWrOpStEvent(EventHandler<WrOpEventArgs> handler);

        abstract public void RegisterTagWriteBankDataEvent(ReqTagWrBnkData handler);
        abstract public void UnregisterTagWriteBankDataEvent(ReqTagWrBnkData handler);

        abstract public void RegisterTagReadBankDataEvent(ReqTagRdBnkInfo handler);
        abstract public void UnregisterTagReadBankDataEvent(ReqTagRdBnkInfo handler);

        abstract public void RegisterCustomFreqGetNotificationEvent(CustomFreqGetNotify handler);
        abstract public void UnregisterCustomFreqGetNotificationEvent(CustomFreqGetNotify handler);

        abstract public void RegisterAntPotCfgGetOneNotificationEvent(AntPortCfgGetOneNotify handler);
        abstract public void UnregisterAntPotCfgGetOneNotificationEvent(AntPortCfgGetOneNotify handler);

        abstract public void RegisterRadioStatusNotificationEvent(RadioStatusNotify handler);
        abstract public void UnregisterRadioStatusNotificationEvent(RadioStatusNotify handler);

        abstract public void RegisterLnkProfInfoGetNotificationEvent(LnkProfInfoGetNotify handler);
        abstract public void UnregisterLnkProfInfoGetNotificationEvent(LnkProfInfoGetNotify handler);

        abstract public void RegisterLnkProfNumoSetNotificationEvent(LnkProfNumSetNotify handler);
        abstract public void UnregisterLnkProfNumoSetNotificationEvent(LnkProfNumSetNotify handler);

        abstract public void RegisterHealthCheckStatusNotificationEvent(HealthCheckStatusNotify handler);
        abstract public void UnregisterHealthCheckStatusNotificationEvent(HealthCheckStatusNotify handler);

        abstract public void RegisterThrshTempGetNotificationEvent(ThrshTempGetNotify handler);
        abstract public void UnregisterThrshTempGetNotificationEvent(ThrshTempGetNotify handler);

        abstract public void RegistereventRespDataModeEvent(RespDatModeSetNotify handler);
        abstract public void UnregistereventRespDataModeEvent(RespDatModeSetNotify handler);

        abstract public void RegisterAntPotCfgSetOneNotificationEvent(AntPortCfgSetOneNotify handler);
        abstract public void UnregisterAntPotCfgSetOneNotificationEvent(AntPortCfgSetOneNotify handler);

        #endregion


        #region Properties
        abstract public HRESULT_RFID LastErrCode
        {
            get;
        }
        abstract public ushort LastMacErrCode
        {
            get;
        }
        #endregion


        #region Abstract Methods
        abstract public bool TagRangeStart();
        abstract public bool TagInventoryStart(EPCEvent evt);
        abstract public bool TagInventoryStart(int option);
        abstract public bool TagInventoryFirstStart();
        abstract public bool TagInventoryOnceStart(bool perfSelCmd);
        abstract public bool TagInvtryRssiStop();
        abstract public bool TagInventoryStop();
        abstract public bool TagInventoryRdRateStop();
        abstract public bool TagInventoryAbort();
        abstract public bool TagInvtryClr();
        abstract public bool SetRepeatedTagObsrvMode(bool enable);
        abstract public bool LinkProfNumGet();
        abstract public bool LinkProfNumSet();
        abstract public bool LinkProfInfoGet();
        abstract public bool GetMacError();
        abstract public void BuzzerBeep(int Option);
        abstract public void MelodyRing();
        abstract public bool TagReadStop();
        abstract public bool TagWriteStart(int Option);
        abstract public bool TagWriteStop();
        abstract public bool TagSetPermStart();
        abstract public bool TagSetPermStart(ref RFID_18K6C_TAG_PERM perm, ushort PC, ref UINT96_T EPC, uint accPasswd);
        abstract public bool TagSetPermStop();
        abstract public bool TagReadBanksStart();
        abstract public bool CustomFreqBandNumGet();
        abstract public bool CustomFreqBandNumSet();
        abstract public bool CustomFreqGet();
        abstract public bool AntPortCfgGetOne();
        abstract public bool AntPortCfgSetPwr();
        abstract public bool SetResponseDataMode(int Option);
        abstract public bool GetTempThreshold();
        abstract public bool SetTempThreshold(ushort xcvrLmt);
        abstract public bool SetTempThreshold(ushort xcvrLmt, ushort ambLmt, ushort paLmt, ushort deltaLmt);
        abstract public bool GetCurrTemp();
        abstract public bool CancelRunningOp();
        abstract public bool PerformHealthCheck();
        abstract public bool StopHealthCheck();
        abstract public bool RadioReady();
        abstract public bool RFGenerateRandomData();
        abstract public bool RFGenerateRandomDataSetup();
        abstract public bool CarrierWaveOn();
        abstract public bool CarrierWaveOff();
        abstract public bool RadioOpen();
        abstract public bool RadioReOpen();
        abstract public bool RadioClose();
        abstract public bool GetPwrLvlRegVals();
        abstract public bool ClearMacError();
        abstract public void Dispose();
        abstract public bool AntPortCfgSetOne(uint portNum, ref RFID_ANTENNA_PORT_CONFIG cfg, AntPortCfgSetOneNotify notify);
        abstract public bool TagInvtryRdRateStart(RFID_18K6C_INVENTORY_SESSION session, bool search1Only, byte[] epcBnkMask, uint maskOffset);
        abstract public bool TagInventoryRssiStart(RFID_18K6C_INVENTORY_SESSION session, bool search1Only, byte[] epcBnkMask, uint maskOffset, bool filterRes);
        abstract public bool CustomFreqSet(CustomFreqGrp freqSet, int chn, bool enLBT, CustomFreqSetNotify notify);
        abstract public bool CustomFreqSet(CustomFreqGrp freqSet, bool enLBT, CustomFreqSetNotify notify);
        abstract public bool CustomFreqSet(CustomFreqGrp freqSet, CustomFreqSetNotify notify);


        abstract public bool MacErrorIsOverheat();
        abstract public bool MacErrorIsFatal();
        abstract public bool MacErrorIsNegligible();
        abstract public HRESULT_RFID f_CFlow_RfidDev_RadioGetConfigurationParameter();

        virtual public CONTROL_VALUE GetConfiguration()
        {
            CONTROL_VALUE cs = new CONTROL_VALUE();
            return cs;
        }

        virtual public void SetConfiguration(CONTROL_VALUE cs)
        {

        }

        #endregion

        #endregion
    }
}
