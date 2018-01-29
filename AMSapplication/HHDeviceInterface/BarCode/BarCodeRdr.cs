using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using ReaderTypes;
using System.Windows.Forms;


namespace HHDeviceInterface.BarCode
{
    public abstract class BarCodeReader
    {
       
        public abstract bool ChangeCurrentSetting(string tag, string subTag, string dataFld, out string retStr);
        public abstract bool ChangeSavedSetting(string tag, string subTag, string dataFld, out string retStr);
        public abstract void Dispose();
        public abstract bool GetCurrentSetting(string tag, string subTag, out string retStr);
        public abstract bool ScanStart();

        public abstract bool InitializeReader();
        public abstract bool StartReader();
        public abstract bool StopReader();
        public abstract bool CloseReader();

        public abstract bool ScanTryStop();
        public abstract void RegisterCodeRcvdNotificationEvent(CodeRcvdNotify handler);
        public abstract void UnregisterCodeRcvdNotificationEvent(CodeRcvdNotify handler);

        public abstract void RegisterStopNotificationEvent(StopNotify handler);
        public abstract void UnregisterStopNotificationEvent(StopNotify handler);

        public CodeRcvdNotify scanNotify;
        public StopNotify stopNotify;

        public Form  notifyee, stopNotifee;
    }
}
