using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using ReaderTypes;

namespace HHDeviceInterface.RFIDSp
{
    public interface IReader
    {
        #region CS101

        HRESULT_RFID LastErrCode
        {
            get;
        }
        ushort LastMacErrCode
        {
            get;
        }

        #region Events
          void RegisterDscvrTagEvent(EventHandler<DscvrTagEventArgs> handler);
          void UnregisterDscvrTagEvent(EventHandler<DscvrTagEventArgs> handler);
          void RegisterInventoryOpStEvent(EventHandler<InvtryOpEventArgs> handler);
          void UnregisterInventoryOpStEvent(EventHandler<InvtryOpEventArgs> handler);
          void RegisterLckOpStEvent(EventHandler<LckOpEventArgs> handler);
          void UnregisterLckOpStEvent(EventHandler<LckOpEventArgs> handler);
          void RegisterMemBnkRdEvent(EventHandler<MemBnkRdEventArgs> handler);
          void UnregisterMemBnkRdEvent(EventHandler<MemBnkRdEventArgs> handler);
          void RegisterMemBnkWrEvent(EventHandler<MemBnkWrEventArgs> handler);
          void UnregisterMemBnkWrEvent(EventHandler<MemBnkWrEventArgs> handler);
          void RegisterRdOpStEvent(EventHandler<RdOpEventArgs> handler);
          void UnregisterRdOpStEvent(EventHandler<RdOpEventArgs> handler);
          void RegisterTagPermSetEvent(EventHandler<DscvrTagEventArgs> handler);
          void UnregisterTagPermSetEvent(EventHandler<DscvrTagEventArgs> handler);
          void RegisterTagRateEvent(EventHandler<TagRateEventArgs> handler);
          void UnregisterTagRateEvent(EventHandler<TagRateEventArgs> handler);
          void RegisterTagRssiEvent(EventHandler<TagRssiEventArgs> handler);
          void UnregisterTagRssiEvent(EventHandler<TagRssiEventArgs> handler);
          void RegisterTempUpdateEvt(EventHandler<TempUpdateEventArgs> handler);
          void UnregisterTempUpdateEvt(EventHandler<TempUpdateEventArgs> handler);
          void RegisterWrOpStEvent(EventHandler<WrOpEventArgs> handler);
          void UnregisterWrOpStEvent(EventHandler<WrOpEventArgs> handler);

          void RegisterTagWriteBankDataEvent(ReqTagWrBnkData handler);
          void UnregisterTagWriteBankDataEvent(ReqTagWrBnkData handler);
          void RegisterTagReadBankDataEvent(ReqTagRdBnkInfo handler);
          void UnregisterTagReadBankDataEvent(ReqTagRdBnkInfo handler);
          void RegisterCustomerFreqGetNotificationEvent(CustomFreqGetNotify handler);
          void UnregisterCustomerFreqGetNotificationEvent(CustomFreqGetNotify handler);
          void RegisterAntPotCfgGetonNotificationEvent(AntPortCfgGetOneNotify handler);
          void UnregisterAntPotCfgGetonNotificationEvent(AntPortCfgGetOneNotify handler);
          void RegisterRadioStatusNotificationEvent(RadioStatusNotify handler);
          void UnregisterRadioStatusNotificationEvent(RadioStatusNotify handler);
          void RegisterLnkProfInfoGetNotificationEvent(LnkProfInfoGetNotify handler);
          void UnregisterLnkProfInfoGetNotificationEvent(LnkProfInfoGetNotify handler);
          void RegisterLnkProfNumoSetNotificationEvent(LnkProfNumSetNotify handler);
          void UnregisterLnkProfNumoSetNotificationEvent(LnkProfNumSetNotify handler);
          void RegisterHealthCheckStatusNotificationEvent(HealthCheckStatusNotify handler);
          void UnregisterHealthCheckStatusNotificationEvent(HealthCheckStatusNotify handler);
          void RegisterThrshTempGetNotificationEvent(ThrshTempGetNotify handler);
          void UnregisterThrshTempGetNotificationEvent(ThrshTempGetNotify handler);
          void RegistereventRespDataModeEvent(RespDatModeSetNotify handler);
          void UnregistereventRespDataModeEvent(RespDatModeSetNotify handler);
          void RegisterAntPotCfgSetonNotificationEvent(AntPortCfgSetOneNotify handler);
          void UnregisterAntPotCfgSetonNotificationEvent(AntPortCfgSetOneNotify handler);
        #endregion


        #region Properties

        

        #endregion

        #endregion

          bool TagRangeStart();
        bool TagInventoryStart(int Option);
        bool TagInventoryFirstStart();
        bool TagInventoryOnceStart(bool perfSelCmd);
        bool TagInvtryRssiStop();
        bool TagInventoryStop();
        bool TagInventoryRdRateStop();
        bool TagInventoryAbort();
        bool TagInvtryClr();
        bool SetRepeatedTagObsrvMode(bool enable);
        bool LinkProfNumGet();
        bool LinkProfNumSet();
        bool LinkProfInfoGet();
        bool GetMacError();
        void BuzzerBeep(int Option);
        void MelodyRing();
        bool TagReadStop();
        bool TagWriteStart(int Option);
        bool TagWriteStop();
        bool TagSetPermStart();
        bool TagSetPermStart(ref RFID_18K6C_TAG_PERM perm, ushort PC, ref UINT96_T EPC, uint accPasswd);
        bool TagSetPermStop();
        bool TagReadBanksStart();
        bool CustomFreqBandNumGet();
        bool CustomFreqBandNumSet();
        bool CustomFreqGet();
        bool AntPortCfgGetOne();
        bool AntPortCfgSetPwr();
        bool SetResponseDataMode(int Option);
        bool GetTempThreshold();
        bool SetTempThreshold(ushort xcvrLmt);
        bool SetTempThreshold(ushort xcvrLmt, ushort ambLmt, ushort paLmt, ushort deltaLmt);
        bool GetCurrTemp();
        bool CancelRunningOp();
        bool PerformHealthCheck();
        bool StopHealthCheck();
        bool RadioReady();
        bool RFGenerateRandomData();
        bool RFGenerateRandomDataSetup();
        bool CarrierWaveOn();
        bool CarrierWaveOff();
        bool RadioOpen();
        bool RadioReOpen();
        bool RadioClose();
        bool GetPwrLvlRegVals();
        bool ClearMacError();
        void Dispose();
        bool AntPortCfgSetOne(uint portNum, ref RFID_ANTENNA_PORT_CONFIG cfg, AntPortCfgSetOneNotify notify);
        bool TagInvtryRdRateStart(RFID_18K6C_INVENTORY_SESSION session, bool search1Only, byte[] epcBnkMask, uint maskOffset);
        bool TagInventoryRssiStart(RFID_18K6C_INVENTORY_SESSION session, bool search1Only, byte[] epcBnkMask, uint maskOffset, bool filterRes);
        bool CustomFreqSet(CustomFreqGrp freqSet, int chn, bool enLBT, CustomFreqSetNotify notify);
        bool CustomFreqSet(CustomFreqGrp freqSet, bool enLBT, CustomFreqSetNotify notify);
        bool CustomFreqSet(CustomFreqGrp freqSet, CustomFreqSetNotify notify);

        #region Commented Interface code......
        //bool TagRangeStart(RFID_18K6C_INVENTORY_SESSION session, uint qSize, RFID_18K6C_SINGULATION_ALGORITHM qAlgo);
        //bool TagInventoryStart(RFID_18K6C_INVENTORY_SESSION session, uint qSize, byte[] epcBankMask, uint maskOffset);
        //bool TagInventoryStart(RFID_18K6C_INVENTORY_SESSION session, uint qSize, RFID_18K6C_SINGULATION_ALGORITHM qAlgo, byte[] epcBankMask, uint maskOffset);
        //bool TagInvtryFirstStart(RFID_18K6C_INVENTORY_SESSION session, uint qSize, byte[] epcBankMask, uint maskOffset);
        //bool LinkProfNumGet(out uint profNum);
        //bool LinkProfNumSet(uint lnkProfNum, LnkProfNumSetNotify notify);
        //bool LinkProfInfoGet(uint lnkProfNum, LnkProfInfoGetNotify notify);
        //bool GetMacError(out ushort macerr);
        //void BuzzerBeep(SoundVol vol);
        //void BuzzerBeep(int freq, SoundVol vol);
        //void MelodyRing(RingTone toneID, SoundVol vol);
        //bool TagWriteStart( ReqTagWrBnkData tagWrDel);
        //bool TagWriteStart(ReqTagWrBnkData tagWrDel, uint accPwd);
        //bool TagWriteStart(ReqTagWrBnkData tagWrDel, ushort qSize);
        //bool TagWriteStart(ReqTagWrBnkData tagWrDel, ushort qSize, uint accPwd);
        //bool TagWriteStart(uint qSize, ref UINT96_T epc, ReqTagWrBnkData tagWrDel);
        //bool TagWriteStart(byte[] epcInvMatchMask, ushort maskOffset, ReqTagWrBnkData tagWrDel, ushort qSize);
        //bool TagWriteStart(uint qSize, ref UINT96_T epc, ReqTagWrBnkData tagWrDel, uint accPwd);
        //bool TagWriteStart(byte[] epcInvMatchMask, ushort maskOffset, ReqTagWrBnkData tagWrDel, ushort qSize, uint accPwd);
        // bool TagSetPermStart(ref RFID_18K6C_TAG_PERM perm, ushort PC, ref UINT96_T EPC, uint accPasswd);
        //bool TagReadBanksStart(uint qSize, ref UINT96_T epc, ReqTagRdBnkInfo cb, uint accPwd);
        //bool CustomFreqBandNumGet(out uint bandNum);
        // bool CustomFreqBandNumSet(uint bandNum);
        //bool CustomFreqGet(CustomFreqGetNotify notify);
        //bool AntPortCfgGetOne(uint portNum, AntPortCfgGetOneNotify notify);
        //bool AntPortCfgSetPwr(uint portNum, int pwr, AntPortCfgSetOneNotify notify);
        //bool SetResponseDataMode(RFID_RESPONSE_MODE mode);
        //bool SetResponseDataMode(RFID_RESPONSE_MODE mode, RespDatModeSetNotify notify);
        //bool GetTempThreshold(ThrshTempGetNotify notify);
        //bool GetCurrTemp(out ushort amb, out ushort xcvr, out ushort pamp);
        //bool PerformHealthCheck(HealthCheckStatusNotify notify);
        //bool RFGenerateRandomData(uint size);
        //bool RadioOpen(RadioStatusNotify radOpnNotify);
        //bool RadioReOpen(RadioStatusNotify radReOpnNotify);
        //bool RadioClose(RadioStatusNotify radClsNotify);
        // bool GetPwrLvlRegVals(out uint pwrLvl, out uint revPwr, out uint pwrThrsh);
        #endregion

    }
}
