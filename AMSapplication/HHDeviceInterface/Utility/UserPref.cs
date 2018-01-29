using System;
//using ClslibRfidSp;
//using ClsReaderLib.Devices.Reader;
using ReaderTypes;

namespace OnRamp
{
    /// <summary>
    /// Session (S1,S2,S3)
    /// Q-Population
    /// Mask (PC(1), EPC(8))
    /// Duty Cycle
    /// And much much more...
    /// </summary>
    public class UserPref
    {

        //public static Client forClient = Client.SmartTrack;

        public static bool IsFileFound = true;

        public static Client forClient = Client.SmartTrack;

        public static string CurVersionNo = "V4.4.6";

        public static bool AppModuleImplementation = true;

        public  const string KeyFileName = "hhkeyv4.dat";

        public const string CSVPath_Local = "\\Flash Disk\\RFIDMobile";

       // public const string BackUpDirPath = "\\Backup Disk\\";

        public static string _BackUpDirPath = "\\Flash Disk\\";

        public string _AutoStartResetPath = "\\Flash Disk\\Start\\Reset\\";

        public string _AutoStartHardResetPath = "\\Flash Disk\\Start\\HardReset\\";

        public string _DBFilePath = "\\Flash Disk\\RFIDData\\";

        public string _AppInstallationPath = "\\Program Files\\RFID\\";

          int _ScannerType = 1;

          int _QValue = 1;
          int _LBTTime = 1;          
          int _ScanTime = 1;
          int _Session = 1;

        public static string pattern = "&FF@";

        public static string RegKeyfolderPath
        {
            get
            {
                return "\\Windows\\wincev5\\";
                //return Environment.GetFolderPath(Environment.SpecialFolder.Programs) + "//wincev5/";
            }
        } 
      
        // These enumeration corresponds to SettingForm
        public enum OperSession
        {
            S1,
            S2,
            S3
        };

        public enum OperQPopRange
        {
            LT10, // Q = 3
            LT100,   // Q = 5
            LT1000, // Q = 7
            LT10000,  // Q = 12
            GT10000,  // Q = 14
            SpecificNum
        };

        public enum OperQAlgo
        {
            Fixed,
            Dynamic
        };

        public struct EPCBankSelectMask
        {
            public Byte[] PC;
            public Byte[] EPC;
        };

        private struct DutyCyclePeriod // in millisec
        {
            public uint on;
            public uint off;
        }

        /// <summary>
        /// UserName is case insensitive
        /// Password is case sensitive
        /// </summary>
        private struct UserSignOn
        {
            public bool Required; // Display Sign-on screen on startup
            // TBD: User-Password List (more than one user)
            public String UserName;
            public String Password;
        }

        /// <summary>
        /// Only concern with optional area of the bank
        /// Size in words
        /// </summary>
        private struct TagOptMemCfg
        {
            public uint TIDBnkVndrAreaSz; // in Words (16-bit)
            public uint UsrBnkAreaSz; //
        }

        [Flags]
        public enum SndSituations
        {
            None = 0,
            Invtry = 0x00000001,
            Ranging = 0x00000002,
            RdWr = 0x00000004,
            Commission = 0x00000008,
            Authentication = 0x00000010,
            SetPerm = 0x00000020,
        }

        public enum AppMode
        {
            IntegratedMode,
            StandAloneMode,
        }

        private struct SndScheme
        {
            public SoundVol Vol;
            public RingTone OperStart;
            public RingTone OperEnd;
            public SndSituations Situations;
        }

        public struct HTTPProxy
        {
            public bool Required;
            public String SrvrAddr;
            public UInt16 SrvrPrt;
            public String Bypasses;
            public String Uname;
            public String Passwd;

            public HTTPProxy(bool required, String srvrAddr, UInt16 srvrPrt,
                String bypasses, String uname, String passwd)
            {
                Required = required;
                SrvrAddr = srvrAddr;
                SrvrPrt = srvrPrt;
                Bypasses = bypasses;
                Uname = uname;
                Passwd = passwd;
            }

            public bool IdenticalTo(ref HTTPProxy anotherPrxy)
            {
                return (
                    (Required == anotherPrxy.Required)
                && (String.Compare(SrvrAddr, anotherPrxy.SrvrAddr, true) == 0)
                && (SrvrPrt == anotherPrxy.SrvrPrt)
                && (String.Compare(Bypasses, anotherPrxy.Bypasses, true) == 0)
                && (String.Compare(Uname, anotherPrxy.Uname, true) == 0)
                && (String.Compare(Passwd, anotherPrxy.Passwd, true) == 0));
            }
        }

        private uint[] QSzTbl;

        private OperSession defSession;
        private OperQPopRange defPopRange;
        private int defPop; // Specific Tag Population
        private OperQAlgo defQAlgo = OperQAlgo.Dynamic;
        private EPCBankSelectMask defEPCSelMask = new EPCBankSelectMask();
        private DutyCyclePeriod defDutyCycle = new DutyCyclePeriod();
        private readonly int defXcvrTempLmt;
        private readonly int defPaTempLmt;
        private readonly int defAmbTempLmt;
        private readonly int defDeltaTempLmt;
#if DEBUG
        private bool defLogTraceOn = true;
#else
        private bool defLogTraceOn = false;
#endif
        private UserSignOn defUserSignOn;
        private TagOptMemCfg defTagOptMemCfg;
        private SndScheme defSndScheme = new SndScheme();
        private String defRdrName;
        private String defServiceURL;
        private String defOnExpired;
        private String defItemLimit;
        private string defHardwareKey;
        private String defKey;

        private int defPageSize;

        private CustomFreqGrp defFreqGrp;
        private int defSpecificFreqChn;
        private bool defFreqChnLBT;
        private uint defLinkProf;
        private float defAntPwr; // dBm
        private String defNTPSrvr;
        private HTTPProxy defHttpPrxy;
        private bool defEnableErrorLogging;

        private OperSession _session;
        private OperQPopRange _popRange;
        private int _pop; // Specific Tag Population
        private OperQAlgo _qAlgo;
        private EPCBankSelectMask _epcBnkMask;
        private DutyCyclePeriod _dutycycle;
        private int _xcvrTempLmt;
        private int _paTempLmt;
        private int _ambTempLmt;
        private int _deltaTempLmt;
        private bool _logTraceOn;
        private UserSignOn _userSignOn;
        private TagOptMemCfg _tagOptMemCfg;
        private SndScheme _sndScheme;
        private String _rdrName;
        private String _ServiceURL;

        private String _OnExpired;
        private String _ItemLimit;
        private string _HardwareKey;
        private String _Key;

        private Int32 _pageSize, _MsgWndTimeOut, _lowPowerValue, _medPowerValue, _highPowerValue;
        private bool _enableErrorLogging;
        private int searchOption;

        private CustomFreqGrp _freqGrp;
        private int _specificFreqChn;
        private bool _freqChnLBT;
        private uint _linkProf;
        private float _antPwr; // dBm
        private String _ntpSrvr;
        private HTTPProxy _httpPrxy;

        private AppMode _appMode;
        private string _tagPrefix;
        private string _csvPath;
        private int _backupDays;

        private bool _usingNewDBLib;

        private int _ddSelectionOption;

        private int _idInputMethod,_selectScreen_PageSize = 1000;

        private bool _ddSelectionOption_PreLoad; 

        private HardwareSelection selectedHardware = HardwareSelection.None;

        private RFID_ANTENNA_PWRLEVEL antennaPwrLevel = RFID_ANTENNA_PWRLEVEL.MEDIUM;

        public enum Category
        {
            InventorySetup,
            OverheatProtect,
            Diagnostics,
            UserMgmt,
            TagMemory,
            Sound,
            ReaderIdentity,
            FreqLinkProfAnt, // Frequency, Link Prof, Antenna
            Network,
            ServiceURLInfo,
            BKeyInfo,
        }

        private void AssignDefaultValues()
        {
            AssignDefaultValue(Category.Diagnostics);
            AssignDefaultValue(Category.FreqLinkProfAnt);
            AssignDefaultValue(Category.InventorySetup);
            AssignDefaultValue(Category.Network);
            AssignDefaultValue(Category.OverheatProtect);
            AssignDefaultValue(Category.ReaderIdentity);
            AssignDefaultValue(Category.ServiceURLInfo);
            AssignDefaultValue(Category.Sound);
            AssignDefaultValue(Category.TagMemory);
            AssignDefaultValue(Category.UserMgmt);
            AssignDefaultValue(Category.BKeyInfo);
        }

        private void AssignDefaultValue(Category cat)
        {
            switch (cat)
            {
                case Category.InventorySetup:
                    _session = defSession;
                    _popRange = defPopRange;
                    _pop = defPop;
                    _epcBnkMask = defEPCSelMask;
                    _qAlgo = defQAlgo;
                    break;
                case Category.OverheatProtect:
                    _dutycycle = defDutyCycle;
                    _xcvrTempLmt = defXcvrTempLmt;
                    _ambTempLmt = defAmbTempLmt;
                    _paTempLmt = defPaTempLmt;
                    _deltaTempLmt = defDeltaTempLmt;
                    break;
                case Category.Diagnostics:
                    _logTraceOn = defLogTraceOn;
                    break;
                case Category.UserMgmt:
                    _userSignOn = defUserSignOn;
                    break;
                case Category.TagMemory:
                    _tagOptMemCfg = defTagOptMemCfg;
                    break;
                case Category.Sound:
                    _sndScheme = defSndScheme;
                    break;
                case Category.ReaderIdentity:
                    _rdrName = defRdrName;
                    break;
                case Category.ServiceURLInfo:
                    _ServiceURL = defServiceURL;
                    _pageSize = defPageSize;
                    _enableErrorLogging = defEnableErrorLogging;
                    break;
                case Category.BKeyInfo:
                    _OnExpired = defOnExpired;
                    _ItemLimit = defItemLimit;
                    _HardwareKey = defHardwareKey;
                    _Key = defKey;
                    selectedHardware = HardwareSelection.None;
                    antennaPwrLevel = RFID_ANTENNA_PWRLEVEL.MEDIUM;
                    break;
                case Category.FreqLinkProfAnt:
                    _freqGrp = defFreqGrp;
                    _specificFreqChn = defSpecificFreqChn;
                    _freqChnLBT = defFreqChnLBT;
                    _linkProf = defLinkProf;
                    _antPwr = defAntPwr;
                    break;
                case Category.Network:
                    _ntpSrvr = defNTPSrvr;
                    _httpPrxy = defHttpPrxy;
                    break;
            }
        }

        private void RemovePrefsByCategory(Category cat)
        {
            switch (cat)
            {
                case Category.InventorySetup:
                    UserPrefFile.ClearRecs(sessionKeyStr, popRangeKeyStr, popNumKeyStr, qAlgoKeyStr, epcMaskKeyStr);
                    break;
                case Category.OverheatProtect:
                    UserPrefFile.ClearRecs(dutyCycOnKeyStr, dutyCycOffKeyStr,
                        xcvrTempLmtKeyStr);
                    break;
                case Category.Diagnostics:
                    UserPrefFile.ClearRecs(logTraceOnKeyStr);
                    break;
                case Category.UserMgmt:
                    UserPrefFile.ClearRecs(userNameKeyStr, userPasswdKeyStr, userSignOnReqKeyStr);
                    break;
                case Category.TagMemory:
                    UserPrefFile.ClearRecs(tidOptMemKeyStr, usrOptMemKeyStr);
                    break;
                case Category.Sound:
                    UserPrefFile.ClearRecs(sndVolKeyStr, sndOperStartKeyStr, sndOperEndKeyStr,
                        sndSitKeyStr);
                    break;
                case Category.ReaderIdentity:
                    UserPrefFile.ClearRecs(rdrNameKeyStr);
                    break;
                case Category.ServiceURLInfo:
                    UserPrefFile.ClearRecs(ServiceURLKeyStr);
                    break;
                case Category.BKeyInfo:
                    UserPrefFile.ClearRecs(OnExpiredKeyStr);
                    UserPrefFile.ClearRecs(ItemLimitKeyStr);
                    UserPrefFile.ClearRecs(HardwareKeyKeyStr);
                    UserPrefFile.ClearRecs(KeyKeyStr);
                    break;
                case Category.FreqLinkProfAnt:
                    UserPrefFile.ClearRecs(rfidAntPwrKeyStr, rfidChnLBTKeyStr, rfidFreqChnKeyStr,
                        rfidFreqGrpKeyStr, rfidLinkProfKeyStr);
                    break;
                case Category.Network:
                    UserPrefFile.ClearRecs(ntpSrvrKeyStr, httpPrxyReqKeyStr,
                        httpPrxySrvrAddrKeyStr, httpPrxySrvrPrtKeyStr, httpPrxySrvrBypassesKeyStr,
                        httpPrxySrvrUnameKeyStr, httpPrxySrvrPasswdKeyStr);
                    break;

            }
        }

        private UserPref()
        {
            //SetupDefaultValues
            {
                defSession = OperSession.S1;
                defPopRange = OperQPopRange.SpecificNum;
                defPop = 32;
                defQAlgo = OperQAlgo.Dynamic;
                defDutyCycle.on = 0 /* 5 */ * 60 * 1000;
                defDutyCycle.off = 0 /* 10 */  * 60 * 1000;
                defXcvrTempLmt = 80;
                defAmbTempLmt = 80;
                defPaTempLmt = 85;
                defDeltaTempLmt = 35;
                defUserSignOn.Required = true /*false*/;  //BKEY
                // use Empty String instead of null for better String.Compare() with empty TextBox
                defUserSignOn.UserName = "" /*"root"*/;
                defUserSignOn.Password = "" /*"0000"*/;
                defSndScheme.Vol = SoundVol.High;
                defSndScheme.OperStart = RingTone.T2;
                defSndScheme.OperEnd = RingTone.T1;
                defSndScheme.Situations = SndSituations.Invtry | SndSituations.Ranging | SndSituations.RdWr
                                        | SndSituations.Authentication | SndSituations.Commission | SndSituations.SetPerm;
                defRdrName = "CS101-1";
                defServiceURL = "http://localhost/Services/";
                defOnExpired = "";
                defItemLimit = "";
                //This it the default ket to access the application
                defHardwareKey = "76246-98531-45678-52465";
                defKey = ""; //"0622340AE6DC24CDA87008BE4";

                defFreqGrp = CustomFreqGrp.FCC;
                defSpecificFreqChn = -1;
                defFreqChnLBT = false;
                defLinkProf = 2;
                defAntPwr = 30.0F;
                defNTPSrvr = "time.windows.com";
                defHttpPrxy = new HTTPProxy();
                defHttpPrxy.Required = false;
                defHttpPrxy.SrvrAddr = String.Empty;
                defHttpPrxy.SrvrPrt = 8080;
                defHttpPrxy.Bypasses = String.Empty;
                defHttpPrxy.Uname = String.Empty;
                defHttpPrxy.Passwd = String.Empty;
                defPageSize = 20;
                searchOption = 1;
                defEnableErrorLogging = true;
                _MsgWndTimeOut = 3;
                _lowPowerValue = 5;
                _medPowerValue = 20;
                _highPowerValue = 30;

                //Stand-alone application mode
                _appMode = AppMode.IntegratedMode;
                _tagPrefix = "000000000000000000000000";
                _csvPath = @"C:\RFIDMobile\";
                _backupDays = 7;
            }

            QSzTbl = new uint[6] { 3, 5, 7, 12, 14, 5 };

            // Struct Initialization
            _epcBnkMask = new EPCBankSelectMask();
            _userSignOn = new UserSignOn();
            defTagOptMemCfg = new TagOptMemCfg();
            _httpPrxy = new HTTPProxy();

            AssignDefaultValues();
        }

        /// <summary>
        /// Remove Settings File and Restore Setting to Default
        /// Throws Exception if failed
        /// </summary>
        public void RestoreDefault()
        {
            UserPrefFile.ClearAll();
            AssignDefaultValues();
        }

        public void RestoreDefaultByCategories(params Category[] cats)
        {
            foreach (Category cat in cats)
            {
                RemovePrefsByCategory(cat);
                AssignDefaultValue(cat);
            }
        }

        static private UserPref thePref; // singleton
        private AppDataFile UserPrefFile;
        public const String UserPrefFileName = "userpref.dat";

        private const String sessionKeyStr = "session";
        private const String popRangeKeyStr = "poprange";
        private const String popNumKeyStr = "popnum";
        private const String qAlgoKeyStr = "qalgorithm";
        private const String pcMaskKeyStr = "pcmask";
        private const String epcMaskKeyStr = "epcmask";
        private const String dutyCycOnKeyStr = "dutycycon";
        private const String dutyCycOffKeyStr = "dutycycoff";
        private const String xcvrTempLmtKeyStr = "xcvrtemplmt";
        private const String logTraceOnKeyStr = "logtraceon";
        private const String userSignOnReqKeyStr = "usersignonreq";
        // TBD: User/Password stored in separate file in the future
        private const String userNameKeyStr = "usrname";
        private const String userPasswdKeyStr = "passwd";
        private const String tidOptMemKeyStr = "tidbnksz";
        private const String usrOptMemKeyStr = "usrbnksz";
        private const String sndVolKeyStr = "sndvol";
        private const String sndOperStartKeyStr = "sndoperstart";
        private const String sndOperEndKeyStr = "sndoperend";
        private const String sndSitKeyStr = "sndsituations";

        private const String rdrNameKeyStr = "rdrID";
        private const String ServiceURLKeyStr = "ServiceURL";
        private const String OnExpiredKeyStr = "onexpired";
        private const String ItemLimitKeyStr = "ItemLimit";
        private const String HardwareKeyKeyStr = "hardwarekey";
        private const String KeyKeyStr = "key";


        private const string PageSizeKey = "PageSize";

        private const string LowPowerValueKey = "LowPowerValue";
        private const string HighPowerValueKey = "HighPowerValue";
        private const string MediumPowerValueKey = "MediumPowerValue";

        private const string MsgWndtimeOutKey = "MsgWndTimeOut";
        private const string SearchOptionKey = "SearchOption";
        private const string EnableErrorLoggingKey = "EnableErrorLogging";

        private const String rfidFreqGrpKeyStr = "rfidfreqgrp";
        private const String rfidFreqChnKeyStr = "rfidfreqchn";
        private const String rfidChnLBTKeyStr = "rfidchnlbt";
        private const String rfidLinkProfKeyStr = "rfidlinkprof";
        private const String rfidAntPwrKeyStr = "rfidantpwr";
        private const String ntpSrvrKeyStr = "ntpsrvr";
        private const String httpPrxyReqKeyStr = "httpprxyrequired";
        private const String httpPrxySrvrAddrKeyStr = "httpprxysrvraddr";
        private const String httpPrxySrvrPrtKeyStr = "httpprxyprt";
        private const String httpPrxySrvrBypassesKeyStr = "httpprxybypasses";
        private const String httpPrxySrvrUnameKeyStr = "httpprxyuname";
        private const String httpPrxySrvrPasswdKeyStr = "httpprxypasswd";

        private const String HardWareSelectionKeyStr = "hardwareSelection";
        private const String AntennaPwrLevelKeyStr = "antennaPwrLevel";

        private const String BackUpDirPathKeyStr = "BackUpDirPathKey";
        private const String AutoResetKeyStr = "AutoResetKey";
        private const String AutoHardResetKeyStr = "AutoHardResetKey";
        private const String DBFilePathKey = "DBFilePathKey"; 
        private const String AppInstallationPathKeyStr = "AppInstallationPathKey";

        private const String ScannerTypeKeyStr = "ScannerTypeKey";

        private const String QValueKeyStr = "QValueKey";
        private const String ScanTimeKeyStr = "ScanTimeKey";
        private const String LBTTimeKeyStr = "LBTTimeKey";
        private const String SessionKeyStr = "SessionKey";
        private const String UsingNewDBLibStr = "UsingNewDBLibKey";

        private const String appmode = "appmode";
        //Stand-alone mode settings
        private const String tagprefix = "tagprefix"; //tag prefix used to prefix user entered tag id if user entered tagID is less than 24digits
        private const String csvpath = "csvpath"; //csv path to export file onto the server when synchronise HH
        private const String backupdays = "backupdays"; // number of day to keep the csv files for on HH. Delete files older than this nuber of days.

        public AppMode ApplicationMode {
            get { return _appMode; }
            set 
            {
                bool modified = _appMode != value;
                _appMode = value;
                if (modified)
                    UserPrefFile.AddRec(appmode, (int)_appMode);
            }
        }
        public string TagPreFix {
            get { return _tagPrefix; }
            set 
            {
                bool modified = _tagPrefix  != value;
                _tagPrefix = value;
                if (modified)
                    UserPrefFile.AddRec(tagprefix, _tagPrefix);
            }
        }
        public string CSVPath {
            get { return _csvPath; }
            set 
            {
                bool modified = _csvPath != value;
                _csvPath = value;
                if (modified)
                    UserPrefFile.AddRec(csvpath, _csvPath);
            }
        }
        public int BackupDays {

            get { return _backupDays; }
            set 
            {
                bool modified = _backupDays != value;
                _backupDays = value;
                if (modified)
                    UserPrefFile.AddRec(backupdays, _backupDays);
            }
        }

        private void LoadPrefFromFile()
        {
            UserPrefFile = new AppDataFile(UserPrefFileName);

            Int32 I32Val;
            Byte[] ByteArrVal;
            bool BoolVal;
            String StrVal;
            float FloatVal;

            if (UserPrefFile.GetInt32(sessionKeyStr, out I32Val))
                _session = (OperSession)I32Val;
            if (UserPrefFile.GetInt32(popRangeKeyStr, out I32Val))
                _popRange = (OperQPopRange)I32Val;
            if (UserPrefFile.GetInt32(popNumKeyStr, out I32Val))
                PopSize = I32Val; // borrow PopSize to calculate QSzTbl
            if (UserPrefFile.GetInt32(qAlgoKeyStr, out I32Val))
                _qAlgo = (OperQAlgo)I32Val;
            if (UserPrefFile.GetByteArr(pcMaskKeyStr, out ByteArrVal))
                _epcBnkMask.PC = ByteArrVal;
            if (UserPrefFile.GetByteArr(epcMaskKeyStr, out ByteArrVal))
                _epcBnkMask.EPC = ByteArrVal;
            if (UserPrefFile.GetInt32(dutyCycOnKeyStr, out I32Val))
                _dutycycle.on = (uint)I32Val;
            if (UserPrefFile.GetInt32(dutyCycOffKeyStr, out I32Val))
                _dutycycle.off = (uint)I32Val;
            if (UserPrefFile.GetInt32(xcvrTempLmtKeyStr, out I32Val))
                _xcvrTempLmt = I32Val;
            if (UserPrefFile.GetBoolean(logTraceOnKeyStr, out BoolVal))
                _logTraceOn = BoolVal;
            if (UserPrefFile.GetBoolean(userSignOnReqKeyStr, out BoolVal))
                _userSignOn.Required = BoolVal;
            if (UserPrefFile.GetString(userNameKeyStr, out StrVal))
                _userSignOn.UserName = StrVal;
            if (UserPrefFile.GetString(userPasswdKeyStr, out StrVal))
                _userSignOn.Password = StrVal;
            if (UserPrefFile.GetInt32(tidOptMemKeyStr, out I32Val))
                _tagOptMemCfg.TIDBnkVndrAreaSz = (uint)I32Val;
            if (UserPrefFile.GetInt32(usrOptMemKeyStr, out I32Val))
                _tagOptMemCfg.UsrBnkAreaSz = (uint)I32Val;
            if (UserPrefFile.GetInt32(sndVolKeyStr, out I32Val))
                _sndScheme.Vol = (SoundVol)I32Val;
            if (UserPrefFile.GetInt32(sndOperStartKeyStr, out I32Val))
                _sndScheme.OperStart = (RingTone)I32Val;
            if (UserPrefFile.GetInt32(sndOperEndKeyStr, out I32Val))
                _sndScheme.OperEnd = (RingTone)I32Val;
            if (UserPrefFile.GetInt32(sndSitKeyStr, out I32Val))
                _sndScheme.Situations = (SndSituations)I32Val;

            if (UserPrefFile.GetString(rdrNameKeyStr, out StrVal))
                _rdrName = StrVal;
            if (UserPrefFile.GetString(ServiceURLKeyStr, out StrVal))
                _ServiceURL = StrVal;

            if (UserPrefFile.GetInt32(PageSizeKey, out I32Val))
                _pageSize = I32Val;

            if (UserPrefFile.GetInt32(MsgWndtimeOutKey, out I32Val))
                _MsgWndTimeOut = I32Val;


            if (UserPrefFile.GetInt32(LowPowerValueKey, out I32Val))
                _lowPowerValue = I32Val;


            if (UserPrefFile.GetInt32(MediumPowerValueKey, out I32Val))
                _medPowerValue = I32Val;


            if (UserPrefFile.GetInt32(HighPowerValueKey, out I32Val))
                _highPowerValue = I32Val;

            if (UserPrefFile.GetBoolean(EnableErrorLoggingKey, out BoolVal))
                _enableErrorLogging = BoolVal;

            if (UserPrefFile.GetInt32(SearchOptionKey, out I32Val))
                searchOption = I32Val;

            if (UserPrefFile.GetString(OnExpiredKeyStr, out StrVal))
                _OnExpired = StrVal;
            if (UserPrefFile.GetString(ItemLimitKeyStr, out StrVal))
                _ItemLimit = StrVal;
            if (UserPrefFile.GetString(HardwareKeyKeyStr, out StrVal))
                _HardwareKey = StrVal;
            if (UserPrefFile.GetString(KeyKeyStr, out StrVal))
                _Key = StrVal;

            if (UserPrefFile.GetInt32(rfidFreqGrpKeyStr, out I32Val))
                _freqGrp = (CustomFreqGrp)I32Val;
            if (UserPrefFile.GetInt32(rfidFreqChnKeyStr, out I32Val))
                _specificFreqChn = I32Val;
            if (UserPrefFile.GetBoolean(rfidChnLBTKeyStr, out BoolVal))
                _freqChnLBT = BoolVal;
            if (UserPrefFile.GetInt32(rfidLinkProfKeyStr, out I32Val))
                _linkProf = I32Val < 0 ? defLinkProf : (uint)I32Val;
            if (UserPrefFile.GetFloat(rfidAntPwrKeyStr, out FloatVal))
                _antPwr = FloatVal < 0.0F ? defAntPwr : FloatVal;
            if (UserPrefFile.GetString(ntpSrvrKeyStr, out StrVal))
                _ntpSrvr = String.IsNullOrEmpty(StrVal) ? String.Empty : StrVal;
            if (UserPrefFile.GetBoolean(httpPrxyReqKeyStr, out BoolVal))
                _httpPrxy.Required = BoolVal;
            if (UserPrefFile.GetString(httpPrxySrvrAddrKeyStr, out StrVal))
                _httpPrxy.SrvrAddr = StrVal;
            if (UserPrefFile.GetInt32(httpPrxySrvrPrtKeyStr, out I32Val))
                _httpPrxy.SrvrPrt = (UInt16)I32Val;
            if (UserPrefFile.GetString(httpPrxySrvrBypassesKeyStr, out StrVal))
                _httpPrxy.Bypasses = StrVal;
            if (UserPrefFile.GetString(httpPrxySrvrUnameKeyStr, out StrVal))
                _httpPrxy.Uname = StrVal;
            if (UserPrefFile.GetString(httpPrxySrvrPasswdKeyStr, out StrVal))
                _httpPrxy.Passwd = StrVal;

            if (UserPrefFile.GetInt32(HardWareSelectionKeyStr, out I32Val))
                selectedHardware = (HardwareSelection)I32Val;

            if (UserPrefFile.GetInt32(AntennaPwrLevelKeyStr, out I32Val))
                antennaPwrLevel = (RFID_ANTENNA_PWRLEVEL)I32Val;

            if (UserPrefFile.GetString(BackUpDirPathKeyStr, out StrVal))
                _BackUpDirPath = StrVal;

            if (UserPrefFile.GetString(AutoResetKeyStr, out StrVal))
                _AutoStartResetPath = StrVal;

            if (UserPrefFile.GetString(AutoHardResetKeyStr, out StrVal))
                _AutoStartHardResetPath = StrVal;

            if (UserPrefFile.GetString(DBFilePathKey, out StrVal))
                _DBFilePath = StrVal;

            if (UserPrefFile.GetInt32(ScannerTypeKeyStr, out I32Val))
                _ScannerType =  I32Val;

            if (UserPrefFile.GetInt32(QValueKeyStr, out I32Val))
                _QValue = I32Val;

            if (UserPrefFile.GetInt32(ScanTimeKeyStr, out I32Val))
                _ScanTime = I32Val;

            if (UserPrefFile.GetInt32(LBTTimeKeyStr, out I32Val))
                _LBTTime = I32Val;

            if (UserPrefFile.GetInt32(SessionKeyStr, out I32Val))
                _Session = I32Val;

            if (UserPrefFile.GetBoolean(UsingNewDBLibStr, out BoolVal))
                _usingNewDBLib = BoolVal;

             if (UserPrefFile.GetInt32("DDSelectionOption", out I32Val))
                 _ddSelectionOption = I32Val;

             if (UserPrefFile.GetBoolean("DDSelectionOption_PreLoad", out BoolVal))
                 _ddSelectionOption_PreLoad = BoolVal;

             if (UserPrefFile.GetInt32("IDInputMethod", out I32Val))
                   _idInputMethod = I32Val;

             if (UserPrefFile.GetInt32("SelectScreen_PageSize", out I32Val))
                  _selectScreen_PageSize = I32Val;

            //reading application mode : integrated or stand-alone
             if (UserPrefFile.GetInt32(appmode, out I32Val))
                 ApplicationMode = (AppMode)I32Val;

            //reading settings for stand-alone mode
             if (UserPrefFile.GetString(tagprefix, out StrVal))
                 TagPreFix = StrVal;
             if (UserPrefFile.GetString(csvpath, out StrVal))
                 CSVPath = StrVal;
             if (UserPrefFile.GetInt32(backupdays, out I32Val))
                 BackupDays = I32Val;
             
        }

        public string BackUpDirPath
        {
            set
            {
                if (value == null)
                    value = "";
                bool Modified = (String.Compare(_BackUpDirPath, value, true) != 0);
               // bool Modified = _BackUpDirPath != value;
                _BackUpDirPath = value;
                if (Modified)
                    UserPrefFile.AddRec(BackUpDirPathKeyStr, _BackUpDirPath);

            }
            get
            {
                return _BackUpDirPath;
            }
        }

        public string AutoStartResetPath
        {
            set
            {
                if (value == null)
                    value = "";
                bool Modified = (String.Compare(_AutoStartResetPath, value, true) != 0);
                _AutoStartResetPath = value;

                if (Modified)
                    UserPrefFile.AddRec(AutoResetKeyStr, _AutoStartResetPath);

            }
            get
            {
                return _AutoStartResetPath;
            }
        }

        public string AutoStartHardResetPath
        {
            set
            {
                if (value == null)
                    value = "";
                bool Modified = (String.Compare(_AutoStartHardResetPath, value, true) != 0);
                _AutoStartHardResetPath = value;

                if (Modified)
                    UserPrefFile.AddRec(AutoHardResetKeyStr, _AutoStartHardResetPath);

            }
            get
            {
                return _AutoStartHardResetPath;
            }
        }

        public string AppInstallationPath
        {
            set
            {
                if (value == null)
                    value = "";
                bool Modified = (String.Compare(_AppInstallationPath, value, true) != 0);
                _AppInstallationPath = value;

                if (Modified)
                    UserPrefFile.AddRec(AppInstallationPathKeyStr, _AppInstallationPath);

            }
            get
            {
                return _AppInstallationPath;
            }
        }

        public string DBFilePath
        {
            set
            {
                if (value == null)
                    value = "";
                bool Modified = (String.Compare(_DBFilePath, value, true) != 0);
                _DBFilePath = value;

                if (Modified)
                    UserPrefFile.AddRec(DBFilePathKey, _DBFilePath);

            }
            get
            {
                return _DBFilePath;
            }
        }
     
        public HardwareSelection SelectedHardware
        {
            set
            {

                bool Modified = selectedHardware != value;
                selectedHardware = value;              
                if (Modified)
                    UserPrefFile.AddRec(HardWareSelectionKeyStr, (int)selectedHardware);

            }
            get
            {
                return selectedHardware;
            }
        }

        public RFID_ANTENNA_PWRLEVEL AntennaPowerLevel
        {
            set
            {

                bool Modified = antennaPwrLevel != value;
                antennaPwrLevel = value;
                if (Modified)
                    UserPrefFile.AddRec(AntennaPwrLevelKeyStr, (int)antennaPwrLevel);

            }
            get
            {
                return antennaPwrLevel;
            }
        }

        public Int32 ScannerType
        {
            set
            {

                bool Modified = _ScannerType != value;
                _ScannerType = value;
                if (Modified)
                    UserPrefFile.AddRec(ScannerTypeKeyStr, (int)_ScannerType);

            }
            get
            {
                return _ScannerType;
            }
        }

        public Int32 QValue
        {
            set
            {

                bool Modified = _QValue != value;
                _QValue = value;
                if (Modified)
                    UserPrefFile.AddRec(QValueKeyStr, (int)_QValue);

            }
            get
            {
                return _QValue;
            }
        }

        public Int32 LBTTime
        {
            set
            {

                bool Modified = _LBTTime != value;
                _LBTTime = value;
                if (Modified)
                    UserPrefFile.AddRec(LBTTimeKeyStr, (int)_LBTTime);

            }
            get
            {
                return _LBTTime;
            }
        }

        public Int32 ScanTime
        {
            set
            {

                bool Modified = _ScanTime != value;
                _ScanTime = value;
                if (Modified)
                    UserPrefFile.AddRec(ScanTimeKeyStr, (int)_ScanTime);

            }
            get
            {
                return _ScanTime;
            }
        }

        public Int32 ATIDSession
        {
            set
            {

                bool Modified = _Session != value;
                _Session = value;
                if (Modified)
                    UserPrefFile.AddRec(SessionKeyStr, (int)_Session);

            }
            get
            {
                return _Session;
            }
        }

        public bool UsingNewDBLib
        {
            set
            {

                bool Modified = _usingNewDBLib != value;
                _usingNewDBLib = value;
                if (Modified)
                    UserPrefFile.AddRec(UsingNewDBLibStr,_usingNewDBLib);

            }
            get
            {
                return _usingNewDBLib;
            }
        }



        public Int32 IDInputMethod
        {
            set
            {

                bool Modified = _idInputMethod != value;
                _idInputMethod = value;
                if (Modified)
                    UserPrefFile.AddRec("IDInputMethod", (int)_idInputMethod);

            }
            get
            {
                return _idInputMethod;
            }
        }

        public Int32 SelectScreen_PageSize
        {
            set
            {

                bool Modified = _selectScreen_PageSize != value;
                _selectScreen_PageSize = value;
                if (Modified)
                    UserPrefFile.AddRec("SelectScreen_PageSize", (int)_selectScreen_PageSize);

            }
            get
            {
                return _selectScreen_PageSize;
            }
        }

        public Int32 DDSelectionOption
        {
            set
            {

                bool Modified = _ddSelectionOption != value;
                _ddSelectionOption = value;
                if (Modified)
                    UserPrefFile.AddRec("DDSelectionOption", (int)_ddSelectionOption);

            }
            get
            {
                return _ddSelectionOption;
            }
        }

        public bool DDSelectionOption_PreLoad
        {
            set
            {

                bool Modified = _ddSelectionOption_PreLoad != value;
                _ddSelectionOption_PreLoad = value;
                if (Modified)
                    UserPrefFile.AddRec("DDSelectionOption_PreLoad", _ddSelectionOption_PreLoad);

            }
            get
            {
                return _ddSelectionOption_PreLoad;
            }
        }

        static public UserPref GetInstance()
        {
            if (thePref == null)
            {
                thePref = new UserPref();
                try
                {
                    thePref.LoadPrefFromFile();
                }
                catch
                {
                    // with any errors, just use default
                }
                // Initialize EPC Tag with Optional Bank Size Preference
                if (thePref.TIDOptMemSz > 0)
                    EPCTag.AvailBanksAdd(MemoryBanks4Op.Two, (ushort)thePref.TIDOptMemSz);
                else
                    EPCTag.AvailBanksSub(MemoryBanks4Op.Two);
                if (thePref.UsrOptMemSz > 0)
                    EPCTag.AvailBanksAdd(MemoryBanks4Op.Three, (ushort)thePref.UsrOptMemSz);
                else
                    EPCTag.AvailBanksSub(MemoryBanks4Op.Three);
            }
            return thePref;
        }

        static public UserPref GetInstance(bool newInstance)
        {
            if(newInstance) thePref = null;

            return GetInstance();
        }

        public OperSession Session
        {
            set
            {
                bool modified = (_session != value);
                _session = value;
                if (modified)
                    UserPrefFile.AddRec(sessionKeyStr, (Int32)_session);
            }
            get
            {
                return _session;
            }
        }

        public RFID_18K6C_INVENTORY_SESSION InvtrySession
        {
            get
            {
                RFID_18K6C_INVENTORY_SESSION InvSess = RFID_18K6C_INVENTORY_SESSION.RFID_18K6C_INVENTORY_SESSION_S2;
                switch (_session)
                {
                    case OperSession.S1:
                        InvSess = RFID_18K6C_INVENTORY_SESSION.RFID_18K6C_INVENTORY_SESSION_S1;
                        break;
                    case OperSession.S2:
                        InvSess = RFID_18K6C_INVENTORY_SESSION.RFID_18K6C_INVENTORY_SESSION_S2;
                        break;
                    case OperSession.S3:
                        InvSess = RFID_18K6C_INVENTORY_SESSION.RFID_18K6C_INVENTORY_SESSION_S3;
                        break;
                }
                return InvSess;
            }
        }
        public OperQPopRange PopRange
        {
            set
            {
                bool modified = (_popRange != value);
                _popRange = value;
                if (modified)
                    UserPrefFile.AddRec(popRangeKeyStr, (Int32)_popRange);
            }
            get
            {
                return _popRange;
            }
        }

        static int[] QPopulations = 
            {
                1,
                2,
                4,
                8,
                16,
                
                32,
                64,
                128,
                256,
                512,

                1024,
                2048,
                4096,
                8192,
                16384,

                32768,
            };

        public static uint Population2QSize(int popSz)
        {
            uint QSz = 7;
            int i = 0;
            for (i = 0; i < QPopulations.Length; i++)
            {
                if (popSz <= QPopulations[i])
                {
                    QSz = (uint)i;
                    break;
                }
            }
            if (i >= QPopulations.Length)
                QSz = (uint)(QPopulations.Length - 1); // use the largest one
            return QSz;
        }

        public int PopSize
        {
            set
            {
                if (value >= 0)
                {
                    bool Modified = _pop != value;
                    _pop = value;
                    QSzTbl[(int)OperQPopRange.SpecificNum] = Population2QSize(_pop);
                    if (Modified)
                        UserPrefFile.AddRec(popNumKeyStr, _pop);
                }
            }
            get
            {
                return _pop;
            }
        }



        public uint QSize
        {
            get
            {
                return QSzTbl[(int)_popRange];
            }
        }

        public RFID_18K6C_SINGULATION_ALGORITHM QAlgo
        {
            set
            {
                OperQAlgo NewAlgo = OperQAlgo.Dynamic;
                switch (value)
                {
                    case RFID_18K6C_SINGULATION_ALGORITHM.RFID_18K6C_SINGULATION_ALGORITHM_DYNAMICQ:
                        NewAlgo = OperQAlgo.Dynamic;
                        break;
                    case RFID_18K6C_SINGULATION_ALGORITHM.RFID_18K6C_SINGULATION_ALGORITHM_FIXEDQ:
                        NewAlgo = OperQAlgo.Fixed;
                        break;
                }

                bool Modified = NewAlgo != _qAlgo;
                _qAlgo = NewAlgo;
                if (Modified)
                    UserPrefFile.AddRec(qAlgoKeyStr, (int)_qAlgo);
            }
            get
            {
                RFID_18K6C_SINGULATION_ALGORITHM RfidQAlgo = RFID_18K6C_SINGULATION_ALGORITHM.RFID_18K6C_SINGULATION_ALGORITHM_DYNAMICQ;
                switch (_qAlgo)
                {
                    case OperQAlgo.Fixed:
                        RfidQAlgo = RFID_18K6C_SINGULATION_ALGORITHM.RFID_18K6C_SINGULATION_ALGORITHM_FIXEDQ;
                        break;
                    default:
                        RfidQAlgo = RFID_18K6C_SINGULATION_ALGORITHM.RFID_18K6C_SINGULATION_ALGORITHM_DYNAMICQ;
                        break;
                }
                return RfidQAlgo;
            }
        }

        public Byte[] PCMask
        {
            set
            {
                bool modified = (_epcBnkMask.PC != value);
                _epcBnkMask.PC = value;
                if (modified)
                    UserPrefFile.AddRec(pcMaskKeyStr, _epcBnkMask.PC);
            }
            get
            {
                return _epcBnkMask.PC;
            }
        }

        public Byte[] EPCMask
        {
            set
            {
                bool modified = (_epcBnkMask.EPC != value);
                _epcBnkMask.EPC = value;
                if (modified)
                    UserPrefFile.AddRec(epcMaskKeyStr, _epcBnkMask.EPC);
            }
            get
            {
                return _epcBnkMask.EPC;
            }
        }

        public void GetEPCBnkSelMask(out Byte[] mask, out uint WdOffset)
        {
            int MaskLen = 0;
            WdOffset = EPCTag.CRC16FldSz; //(CRC

            if (_epcBnkMask.PC == null || _epcBnkMask.PC.Length == 0)
            {
                WdOffset += EPCTag.PCFldSz; // PC not part of the Select Mask
            }
            else
            {
                MaskLen = EPCTag.PCFldSz * 2;
            }

            if (_epcBnkMask.EPC != null && _epcBnkMask.EPC.Length > 0)
            {
                MaskLen += _epcBnkMask.EPC.Length;
            }
            mask = new Byte[MaskLen];
            if (WdOffset == EPCTag.CRC16FldSz)
            {
                int PCMaskSz = EPCTag.PCFldSz * 2;
                // PC is part of the mask
                Array.Copy(_epcBnkMask.PC, mask, PCMaskSz);
                if (MaskLen > PCMaskSz)
                    Array.Copy(_epcBnkMask.EPC, 0, mask, EPCTag.PCFldSz * 2, MaskLen - PCMaskSz);
            }
            if ((WdOffset == (EPCTag.CRC16FldSz + EPCTag.PCFldSz)) && MaskLen > 0)
            {
                Array.Copy(_epcBnkMask.EPC, mask, MaskLen);
            }
        }

        /// <summary>
        /// Duty Cycle "On" Duration in minutes
        /// </summary>
        public uint DCOnDuration
        {
            get
            {
#if BURN_IN
                return 0;
#else
                return _dutycycle.on / (60 * 1000);
#endif
            }
            set
            {
                uint newVal = value * 1000 * 60;
                bool modified = (_dutycycle.on != newVal);
                _dutycycle.on = newVal;
                if (modified)
                    UserPrefFile.AddRec(dutyCycOnKeyStr, (Int32)_dutycycle.on);
            }
        }

        public uint DCOffDuration
        {
            get
            {
#if BURN_IN
                return 0;
#else
                return _dutycycle.off / (60 * 1000);
#endif
            }
            set
            {
                uint newVal = value * 1000 * 60;
                bool modified = (_dutycycle.off != newVal);
                _dutycycle.off = newVal;
                if (modified)
                    UserPrefFile.AddRec(dutyCycOffKeyStr, (Int32)_dutycycle.off);
            }
        }

        public int XcvrTempLmt
        {
            get
            {
                return _xcvrTempLmt;
            }
            set
            {
                int newVal = value;
                bool modified = (_xcvrTempLmt != newVal);
                _xcvrTempLmt = newVal;
                if (modified)
                    UserPrefFile.AddRec(xcvrTempLmtKeyStr, (Int32)_xcvrTempLmt);
            }
        }

        public int AmbTempLmt
        {
            get
            {
                return _ambTempLmt;
            }
        }

        public int PATempLmt
        {
            get
            {
                return _paTempLmt;
            }
        }

        public int DeltaTempLmt
        {
            get
            {
                return _deltaTempLmt;
            }
        }

        public bool LogTraceOn
        {
            set
            {
                bool modified = (_logTraceOn != value);
                _logTraceOn = value;
                if (modified)
                    UserPrefFile.AddRec(logTraceOnKeyStr, _logTraceOn);
            }
            get
            {
#if BURN_IN
                return true; // always turn on log
#else
                return _logTraceOn;
#endif
            }
        }

        public bool UserSignOnRequired
        {
            set
            {
                bool modified = (_userSignOn.Required != value);
                _userSignOn.Required = value;
                if (modified)
                    UserPrefFile.AddRec(userSignOnReqKeyStr, value);
            }
            get
            {
                return _userSignOn.Required;
            }
        }


        public String UserName
        {
            set
            {
                // use Empty String instead of null for better String.Compare() with empty TextBox
                if (value == null)
                    value = "";
                bool modified = (String.Compare(_userSignOn.UserName, value, true) != 0);
                _userSignOn.UserName = value;
                if (modified)
                    UserPrefFile.AddRec(userNameKeyStr, value);
            }
            get
            {
                return _userSignOn.UserName;
            }
        }

        public String Passwd
        {
            set
            {
                // use Empty String instead of null for better String.Compare() with empty TextBox
                if (value == null)
                    value = "";
                bool modified = (String.Compare(_userSignOn.Password, value, false) != 0);
                _userSignOn.Password = value;
                if (modified) // TBD: encrypt password before saving to file
                    UserPrefFile.AddRec(userPasswdKeyStr, value);
            }
            get
            {
                return _userSignOn.Password;
            }
        }

        public uint TIDOptMemSz
        {
            set
            {
                bool modified = (_tagOptMemCfg.TIDBnkVndrAreaSz != value);
                _tagOptMemCfg.TIDBnkVndrAreaSz = value;
                if (modified)
                    UserPrefFile.AddRec(tidOptMemKeyStr, (Int32)value);
            }
            get
            {
                return _tagOptMemCfg.TIDBnkVndrAreaSz;
            }
        }

        public uint UsrOptMemSz
        {
            set
            {
                bool modified = (_tagOptMemCfg.UsrBnkAreaSz != value);
                _tagOptMemCfg.UsrBnkAreaSz = value;
                if (modified)
                    UserPrefFile.AddRec(usrOptMemKeyStr, (Int32)value);
            }
            get
            {
                return _tagOptMemCfg.UsrBnkAreaSz;
            }
        }

        public SoundVol SndVol
        {
            set
            {
                bool modified = (_sndScheme.Vol != value);
                _sndScheme.Vol = value;
                if (modified)
                    UserPrefFile.AddRec(sndVolKeyStr, (Int32)value);
            }
            get
            {
                return _sndScheme.Vol;
            }
        }

        public RingTone OperStartMldy
        {
            set
            {
                bool modified = (_sndScheme.OperStart != value);
                _sndScheme.OperStart = value;
                if (modified)
                    UserPrefFile.AddRec(sndOperStartKeyStr, (Int32)value);
            }
            get
            {
                return _sndScheme.OperStart;
            }
        }

        public RingTone OperEndMldy
        {
            set
            {
                bool modified = (_sndScheme.OperEnd != value);
                _sndScheme.OperEnd = value;
                if (modified)
                    UserPrefFile.AddRec(sndOperEndKeyStr, (Int32)value);
            }
            get
            {
                return _sndScheme.OperEnd;
            }
        }

        public SndSituations SndNotifications
        {
            set
            {
                bool modified = (_sndScheme.Situations != value);
                _sndScheme.Situations = value;
                if (modified)
                    UserPrefFile.AddRec(sndSitKeyStr, (Int32)value);
            }
            get
            {
                return _sndScheme.Situations;
            }
        }

        public String RdrName
        {
            set
            {
                // case-insensitive compare
                bool modified = String.Compare(value, _rdrName, true) != 0;
                _rdrName = value;
                if (modified)
                    UserPrefFile.AddRec(rdrNameKeyStr, _rdrName);
            }
            get
            {
                return (_rdrName == null) ? String.Empty : _rdrName;
            }
        }

        public String ServiceURL
        {
            set
            {
                // case-insensitive compare
                bool modified = String.Compare(value, _ServiceURL, true) != 0;
                _ServiceURL = value;
                if (modified)
                    UserPrefFile.AddRec(ServiceURLKeyStr, _ServiceURL);
            }
            get
            {
                return (_ServiceURL == null) ? String.Empty : _ServiceURL;
            }
        }

        public Int32 PageSize
        {
            set
            {
                // case-insensitive compare
                bool modified = (value != _pageSize);
                _pageSize = value;
                if (modified)
                {
                    if (UserPrefFile != null)
                        UserPrefFile.AddRec(PageSizeKey, _pageSize);
                }

            }
            get
            {
                return _pageSize;
            }
        }

        public Int32 LowPowerValue
        {
            set
            {
                // case-insensitive compare
                bool modified = (value != _lowPowerValue);
                _lowPowerValue = value;
                if (modified)
                {
                    if (UserPrefFile != null)
                        UserPrefFile.AddRec(LowPowerValueKey, _lowPowerValue);
                }

            }
            get
            {
                return _lowPowerValue;
            }
        }

        public Int32 MediumPowerValue
        {
            set
            {
                // case-insensitive compare
                bool modified = (value != _medPowerValue);
                _medPowerValue = value;
                if (modified)
                {
                    if (UserPrefFile != null)
                        UserPrefFile.AddRec(MediumPowerValueKey, _medPowerValue);
                }

            }
            get
            {
                return _medPowerValue;
            }
        }

        public Int32 HighPowerValue
        {
            set
            {
                // case-insensitive compare
                bool modified = (value != _highPowerValue);
                _highPowerValue = value;
                if (modified)
                {
                    if (UserPrefFile != null)
                        UserPrefFile.AddRec(HighPowerValueKey, _highPowerValue);
                }

            }
            get
            {
                return _highPowerValue;
            }
        }

        public Int32 MsgWndTimeOut
        {
            set
            {
                // case-insensitive compare
                bool modified = (value != _MsgWndTimeOut);
                _MsgWndTimeOut = value;
                if (modified)
                {
                    if (UserPrefFile != null)
                        UserPrefFile.AddRec(MsgWndtimeOutKey, _MsgWndTimeOut);
                }

            }
            get
            {
                return _MsgWndTimeOut;
            }
        }

        public bool EnableErrorLogging
        {
            set
            {
                // case-insensitive compare
                bool modified = (value != _enableErrorLogging);
                _enableErrorLogging = value;
                if (modified)
                {
                    if (UserPrefFile != null)
                        UserPrefFile.AddRec(EnableErrorLoggingKey, _enableErrorLogging);
                }

            }
            get
            {
                return _enableErrorLogging;
            }
        }

        public Int32 SearchOption
        {
            set
            {
                // case-insensitive compare
                bool modified = (value != searchOption);
                searchOption = value;
                if (modified)
                {
                    if (UserPrefFile != null)
                        UserPrefFile.AddRec(SearchOptionKey, searchOption);
                }
            }
            get
            {
                return searchOption;
            }
        }

        public String OnExpired
        {
            set
            {
                // case-insensitive compare
                bool modified = String.Compare(value, _OnExpired, true) != 0;
                _OnExpired = value;
                if (modified)
                    UserPrefFile.AddRec(OnExpiredKeyStr, _OnExpired);
            }
            get
            {
                return (_OnExpired == null) ? String.Empty : _OnExpired;
            }
        }

        public String ItemLimit
        {
            set
            {
                // case-insensitive compare
                bool modified = String.Compare(value, _ItemLimit, true) != 0;
                _ItemLimit = value;
                if (modified)
                    UserPrefFile.AddRec(ItemLimitKeyStr, _ItemLimit);
            }
            get
            {
                return (_ItemLimit == null) ? String.Empty : _ItemLimit;
            }
        }

        public String HardwareKey
        {
            set
            {
                // case-insensitive compare
                bool modified = String.Compare(value, _HardwareKey, true) != 0;
                _HardwareKey = value;
                if (modified)
                    UserPrefFile.AddRec(HardwareKeyKeyStr, _HardwareKey);
            }
            get
            {
                return (_HardwareKey == null) ? String.Empty : _HardwareKey;
            }
        }

        public String Key
        {
            set
            {
                // case-insensitive compare
                bool modified = String.Compare(value, _Key, true) != 0;
                _Key = value;
                if (modified)
                    UserPrefFile.AddRec(KeyKeyStr, _Key);
            }
            get
            {
                return (_Key == null) ? String.Empty : _Key;
            }
        }

        public CustomFreqGrp FreqProf
        {
            set
            {
                bool Modified = (_freqGrp != value);
                _freqGrp = value;
                if (Modified)
                    UserPrefFile.AddRec(rfidFreqGrpKeyStr, (Int32)value);
            }
            get
            {
                return _freqGrp;
            }
        }

        public int SingleFreqChn
        {
            set
            {
                bool Modified = (_specificFreqChn < 0 ? (value >= 0) : (_specificFreqChn != value));
                _specificFreqChn = value;
                if (Modified)
                    UserPrefFile.AddRec(rfidFreqChnKeyStr, value);
            }
            get
            {
                return _specificFreqChn;
            }
        }

        public bool EnFreqChnLBT
        {
            set
            {
                bool Modified = _freqChnLBT != value;
                _freqChnLBT = value;
                if (Modified)
                    UserPrefFile.AddRec(rfidChnLBTKeyStr, value);
            }
            get
            {
                return _freqChnLBT;
            }
        }

        public CustomFreqGrp PwrOnDefaultFreqProf
        {
            get
            {
                return defFreqGrp; //Note: this must match the firmware settings
            }
        }

        public int PwrOnDefaultSingleFreqChn
        {
            get
            {
                return defSpecificFreqChn; //Note: this must match the firmware settings
            }
        }

        public bool PwrOnDefaultChnLBT
        {
            get
            {
                return defFreqChnLBT; // Note: this must match the firmware settings
            }
        }

        public float AntennaPwr
        {
            set
            {
                if (value >= 0)
                {
                    bool Modified = (_antPwr != value);
                    _antPwr = value;
                    if (Modified)
                        UserPrefFile.AddRec(rfidAntPwrKeyStr, (float)value);
                }
            }
            get
            {
                return _antPwr;
            }
        }

        public uint LinkProf
        {
            set
            {
                bool Modified = (_linkProf != value);
                _linkProf = value;
                if (Modified)
                    UserPrefFile.AddRec(rfidLinkProfKeyStr, (Int32)value);
            }
            get
            {
                return _linkProf;
            }
        }

        public String NTPServer
        {
            set
            {
                bool Modified = (String.Compare(_ntpSrvr, value, true) != 0);
                _ntpSrvr = String.IsNullOrEmpty(value) ? String.Empty : value;
                if (Modified)
                    UserPrefFile.AddRec(ntpSrvrKeyStr, value);
            }
            get
            {
                return String.IsNullOrEmpty(_ntpSrvr) ? String.Empty : _ntpSrvr;
            }
        }

        public HTTPProxy HttpProxy
        {
            set
            {
                bool Modified = _httpPrxy.IdenticalTo(ref value);
                _httpPrxy = value; // shallow copy
                if (Modified)
                {
                    UserPrefFile.AddRec(httpPrxyReqKeyStr, value.Required);
                    UserPrefFile.AddRec(httpPrxySrvrAddrKeyStr, value.SrvrAddr);
                    UserPrefFile.AddRec(httpPrxySrvrPrtKeyStr, (Int32)value.SrvrPrt);
                    UserPrefFile.AddRec(httpPrxySrvrBypassesKeyStr, value.Bypasses);
                    UserPrefFile.AddRec(httpPrxySrvrUnameKeyStr, value.Uname);
                    UserPrefFile.AddRec(httpPrxySrvrPasswdKeyStr, value.Passwd);
                }
            }
            get
            {
                return _httpPrxy;
            }
        }

        public enum Client
        {
            OnRamp,
            SmartTrack,
            Access
        };

        #region Get/Set AppModules

        public string allApplicationModules = "";
        //  public  bool RemoveDispatch = true; //Enable Disable Dispatch Functionality.      
        //  public  bool RemoveFieldService = true; //Enable Disable FieldService Functionality
        // public  bool ImmunoSearch = false; //Changes the form in Search Process
        //public  bool VernonFlag = true; // It forces the selection of Reason in Inventory

        public ExtraAppModule VernonFlag = new ExtraAppModule("Vernon", 15);
        public ExtraAppModule ImmunoSearch = new ExtraAppModule("ImmunoSearch", 16);

        public void SetAppModules()
        {
            try
            {
                UserPrefFile.AddRec("AM_AllAppModules", allApplicationModules);
                int iClient = 0;
                if (forClient == Client.OnRamp)
                {
                    iClient = 1;
                }
                else if (forClient == Client.Access)
                {
                    iClient = 2;
                }
                else if (forClient == Client.SmartTrack)
                {
                    iClient = 0;
                }

                UserPrefFile.AddRec("Client", iClient);

                // UserPrefFile.AddRec("AM_RemoveDispatch", RemoveDispatch);
                // UserPrefFile.AddRec("AM_RemoveFieldService", RemoveFieldService);
                // UserPrefFile.AddRec("AM_ImmunoSearch", ImmunoSearch);
                //UserPrefFile.AddRec("AM_VernonFlag", VernonFlag);

            }
            catch (Exception ex)
            {
            }
        }

        public void GetAppModules()
        {
            int intVal;
            string strVal;
            try
            {

                if (UserPrefFile.GetString("AM_AllAppModules", out strVal))
                    allApplicationModules = strVal;

                if (UserPrefFile.GetInt32("Client", out intVal))
                {
                    if (intVal == 1)
                        forClient = Client.OnRamp;
                    else if (intVal == 0)
                        forClient = Client.SmartTrack;
                    else if (intVal == 2)
                        forClient = Client.Access;
                }

                //if (UserPrefFile.GetBoolean("AM_ImmunoSearch", out BoolVal))
                //    ImmunoSearch = BoolVal;

                //if (UserPrefFile.GetBoolean("AM_VernonFlag", out BoolVal))
                //    VernonFlag = BoolVal;

                //if (UserPrefFile.GetBoolean("AM_RemoveDispatch", out BoolVal))
                //    RemoveDispatch = BoolVal;

                //if (UserPrefFile.GetBoolean("AM_RemoveFieldService", out BoolVal))
                //    RemoveFieldService = BoolVal;


            }
            catch (Exception ex)
            {
            }
        }

        #endregion Get/Set AppModules

    }

    public struct ExtraAppModule
    {
        public string ModuleName;
        public int ModuleNo;
        public bool Enable;

        public ExtraAppModule(string mName, int mNo, bool enable)
        {
            ModuleName = mName;
            ModuleNo = mNo;
            Enable = enable;
        }
        public ExtraAppModule(string mName, int mNo)
        {
            ModuleName = mName;
            ModuleNo = mNo;
            Enable = false;
        }

    }

   

    }
