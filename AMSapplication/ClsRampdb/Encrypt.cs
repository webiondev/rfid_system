/**************************************************************************************
 * Author : Deepanshu Jouhari
 * Modified Date : 15-Jun-2009
 * Last Modified by : 
 * Last Modified : 
 * Module Name : ClsRampDb
 * Decription : Encrypt class to implement Key Encoding logic.
 **************************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace ClsRampdb
{
    
    public class Encrypt
    {
        #region PrivateVarible

        private string _RefNumber;
        private string _Key;
        private bool _NeverExp;
        private DateTime _ExpDate;

        #endregion

        #region Public Properties

        public string RefNumber
        {
            get { return _RefNumber; }
            set { _RefNumber = value; }
        }
        public string Key
        {
            get { return _Key; }
            set { _Key = value; }
        }
        public bool NeverExp
        {
            get { return _NeverExp; }
            set { _NeverExp = value; }
        }
        public DateTime ExpDate
        {
            get { return _ExpDate; }
            set { _ExpDate = value; }
        }

        #endregion

        #region Static Methods

        public enum DeviceType
        {
            Handheld = 1,
            CSLFixedReader = 2
        }

        //This function returns 12 Digit code from getting 22 digit Device ID.
        private static string ConvertToMacFormat(String KeyStr)
        {
            StringBuilder retString = new StringBuilder(12);
            KeyStr = KeyStr.Replace("-", "");
            if (KeyStr.Length == 12)
            {
                return KeyStr;
            }
            //Processing for Handheld Device id
            KeyStr = KeyStr.PadLeft(22, '0');
            KeyStr = KeyStr.Substring(KeyStr.Length - 22);
            String S1 = "", S2 = "", S3 = "", S4 = "", S5 = "";
            S1 = KeyStr.Substring(0, 8);
            S1 = S1.Substring(3); //Retrive 5 Digit

            S2 = KeyStr.Substring(8, 4);
            S2 = S2.Substring(2); //Retrive 2 Digit

            S3 = KeyStr.Substring(12, 4);
            S3 = S3.Substring(2); //Retrive 2 Digit

            S4 = KeyStr.Substring(16, 4);
            S4 = S4.Substring(2);//Retrive 2 Digit

            S5 = KeyStr.Substring(20, 2);
            S5 = S5.Substring(1);//Retrive 1 Digit

            retString.Append(S1);
            retString.Append(S2);
            retString.Append(S3);
            retString.Append(S4);
            retString.Append(S5);

            return retString.ToString();
        }

        //This is the old version function Generate the Encrypted Key Related to It's parameter Ref Number and Encode Expiry Date value in it.
        public static string Generate_Key(string RefNumber, bool NeverExp, DateTime ExpDate)
        {

            try
            {
                if (RefNumber.Length != 12)
                {
                    throw new ApplicationException("Invalid MAC Address");
                }

                Random random = new Random();
                string sMACAddress = RefNumber;
                char[] arr = sMACAddress.ToCharArray();

                string s1 = "", s2 = "", s3 = "", s4 = "";

                if (Int64.Parse(arr[2].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) % 2 == 0)
                {
                    s1 = Convert.ToString(Math.Abs(Int64.Parse(arr[0].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) + 1));
                    s2 = Convert.ToString(Math.Abs(Int64.Parse(arr[1].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) + 1));
                    s3 = Convert.ToString(Math.Abs(Int64.Parse(arr[2].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) + 1));
                    arr[0] = Convert.ToChar(s1.Substring(s1.Length - 1, 1));
                    arr[1] = Convert.ToChar(s2.Substring(s2.Length - 1, 1));
                    arr[2] = Convert.ToChar(s3.Substring(s3.Length - 1, 1));

                }
                else
                {
                    s1 = Convert.ToString(Math.Abs(Int64.Parse(arr[0].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) - 1));
                    s2 = Convert.ToString(Math.Abs(Int64.Parse(arr[1].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) - 1));
                    s3 = Convert.ToString(Math.Abs(Int64.Parse(arr[2].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) - 1));
                    arr[0] = Convert.ToChar(s1.Substring(s1.Length - 1, 1));
                    arr[1] = Convert.ToChar(s2.Substring(s2.Length - 1, 1));
                    arr[2] = Convert.ToChar(s3.Substring(s3.Length - 1, 1));


                }

                if (Int64.Parse(arr[6].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) % 2 == 0)
                {
                    s1 = Convert.ToString(Math.Abs(Int64.Parse(arr[4].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) + 7));
                    s2 = Convert.ToString(Math.Abs(Int64.Parse(arr[5].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) + 7));
                    s3 = Convert.ToString(Math.Abs(Int64.Parse(arr[6].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) + 7));
                    s4 = Convert.ToString(Math.Abs(Int64.Parse(arr[7].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) + 7));
                    arr[4] = Convert.ToChar(s1.Substring(s1.Length - 1, 1));
                    arr[5] = Convert.ToChar(s2.Substring(s2.Length - 1, 1));
                    arr[6] = Convert.ToChar(s3.Substring(s3.Length - 1, 1));
                    arr[7] = Convert.ToChar(s4.Substring(s4.Length - 1, 1));

                }
                else
                {
                    s1 = Convert.ToString(Math.Abs(Int64.Parse(arr[4].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) - 7));
                    s2 = Convert.ToString(Math.Abs(Int64.Parse(arr[5].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) - 7));
                    s3 = Convert.ToString(Math.Abs(Int64.Parse(arr[6].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) - 7));
                    s4 = Convert.ToString(Math.Abs(Int64.Parse(arr[7].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) - 7));
                    arr[4] = Convert.ToChar(s1.Substring(s1.Length - 1, 1));
                    arr[5] = Convert.ToChar(s2.Substring(s2.Length - 1, 1));
                    arr[6] = Convert.ToChar(s3.Substring(s3.Length - 1, 1));
                    arr[7] = Convert.ToChar(s4.Substring(s4.Length - 1, 1));
                }

                if (Int64.Parse(arr[11].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) % 2 == 0)
                {
                    s1 = Convert.ToString(Math.Abs(Int64.Parse(arr[8].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) + 12));
                    s2 = Convert.ToString(Math.Abs(Int64.Parse(arr[9].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) + 12));
                    s3 = Convert.ToString(Math.Abs(Int64.Parse(arr[10].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) + 12));
                    s4 = Convert.ToString(Math.Abs(Int64.Parse(arr[11].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) + 12));
                    arr[8] = Convert.ToChar(s1.Substring(s1.Length - 1, 1));
                    arr[9] = Convert.ToChar(s2.Substring(s2.Length - 1, 1));
                    arr[10] = Convert.ToChar(s3.Substring(s3.Length - 1, 1));
                    arr[11] = Convert.ToChar(s4.Substring(s4.Length - 1, 1));

                }
                else
                {
                    s1 = Convert.ToString(Math.Abs(Int64.Parse(arr[8].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) - 12));
                    s2 = Convert.ToString(Math.Abs(Int64.Parse(arr[9].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) - 12));
                    s3 = Convert.ToString(Math.Abs(Int64.Parse(arr[10].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) - 12));
                    s4 = Convert.ToString(Math.Abs(Int64.Parse(arr[11].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) - 12));
                    arr[8] = Convert.ToChar(s1.Substring(s1.Length - 1, 1));
                    arr[9] = Convert.ToChar(s2.Substring(s2.Length - 1, 1));
                    arr[10] = Convert.ToChar(s3.Substring(s3.Length - 1, 1));
                    arr[11] = Convert.ToChar(s4.Substring(s4.Length - 1, 1));
                }


                StringBuilder sLevel1 = new StringBuilder();

                sLevel1.Append(arr, 0, arr.Length);

                string sDate = "";
                StringBuilder sb = new StringBuilder();
                if (NeverExp)
                    sb.Append("123199");
                else
                    sb.Append(ExpDate.ToString("MMddyy"));

                //Conver Date to HexaDecimal, Max No of digits are 5
                sDate = String.Format("{0:x}", Convert.ToInt64(sb.ToString())).PadLeft(5, '0');

                char[] arrDate = sDate.ToCharArray();

                ///ADD date And Random NO
                if (Int64.Parse(sMACAddress.Trim(), System.Globalization.NumberStyles.HexNumber) % 5 == 0)
                {
                    sLevel1.Insert(9, arrDate[4].ToString());
                    sLevel1.Insert(10, arrDate[3].ToString());
                    sLevel1.Insert(11, arrDate[2].ToString());
                    sLevel1.Insert(14, arrDate[1].ToString());
                    sLevel1.Insert(16, arrDate[0].ToString());

                    int i1 = random.Next(0, 9);
                    int i2 = random.Next(0, 9);
                    int i3 = random.Next(0, 9);

                    sLevel1.Insert(4, i1.ToString());
                    sLevel1.Insert(10, i2.ToString());
                    sLevel1.Insert(15, i3.ToString());
                }
                else if (Int64.Parse(sMACAddress.Trim(), System.Globalization.NumberStyles.HexNumber) % 3 == 0)
                {
                    sLevel1.Insert(1, arrDate[0].ToString());
                    sLevel1.Insert(5, arrDate[1].ToString());
                    sLevel1.Insert(9, arrDate[2].ToString());
                    sLevel1.Insert(15, arrDate[3].ToString());
                    sLevel1.Insert(16, arrDate[4].ToString());

                    int i1 = random.Next(0, 9);
                    int i2 = random.Next(0, 9);
                    int i3 = random.Next(0, 9);

                    sLevel1.Insert(3, i1.ToString());
                    sLevel1.Insert(15, i2.ToString());
                    sLevel1.Insert(18, i3.ToString());

                    //0107013666a61119175c
                }

                else if (Int64.Parse(sMACAddress.Trim(), System.Globalization.NumberStyles.HexNumber) % 2 == 0)
                {
                    sLevel1.Insert(3, arrDate[0].ToString());
                    sLevel1.Insert(8, arrDate[1].ToString());
                    sLevel1.Insert(9, arrDate[2].ToString());
                    sLevel1.Insert(13, arrDate[3].ToString());
                    sLevel1.Insert(15, arrDate[4].ToString());

                    int i1 = random.Next(0, 9);
                    int i2 = random.Next(0, 9);
                    int i3 = random.Next(0, 9);

                    sLevel1.Insert(3, i1.ToString());
                    sLevel1.Insert(5, i2.ToString());
                    sLevel1.Insert(17, i3.ToString());
                }
                else
                {
                    sLevel1.Insert(4, arrDate[4].ToString());
                    sLevel1.Insert(6, arrDate[3].ToString());
                    sLevel1.Insert(8, arrDate[2].ToString());
                    sLevel1.Insert(10, arrDate[1].ToString());
                    sLevel1.Insert(12, arrDate[0].ToString());

                    int i1 = random.Next(0, 9);
                    int i2 = random.Next(0, 9);
                    int i3 = random.Next(0, 9);

                    sLevel1.Insert(4, i1.ToString());
                    sLevel1.Insert(13, i2.ToString());
                    sLevel1.Insert(19, i3.ToString());
                }

                return sLevel1.ToString();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        //This is the function Generate the Encrypted Key Related to It's parameter Ref Number and Encode Expiry Date, Application Type and No of items value in it.
        public static string Generate_KeyV2(string RefNumber, bool NeverExp, DateTime ExpDate, String LengthOfKey, String dType, String noOfItems)
        {
            if (LengthOfKey != "D E E P @ R A M P")
            {
                return "";
            }
            try
            {

                RefNumber = ConvertToMacFormat(RefNumber);

                if (RefNumber.Length != 12)
                {
                    throw new ApplicationException("Invalid Hardware Key");
                }

                Random random = new Random();
                string sMACAddress = RefNumber;
                char[] arr = sMACAddress.ToCharArray();

                string s1 = "", s2 = "", s3 = "", s4 = "";

                if (Int64.Parse(arr[2].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) % 2 == 0)
                {
                    s1 = Convert.ToString(Math.Abs(Int64.Parse(arr[0].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) + 1));
                    s2 = Convert.ToString(Math.Abs(Int64.Parse(arr[1].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) + 1));
                    s3 = Convert.ToString(Math.Abs(Int64.Parse(arr[2].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) + 1));
                    arr[0] = Convert.ToChar(s1.Substring(s1.Length - 1, 1));
                    arr[1] = Convert.ToChar(s2.Substring(s2.Length - 1, 1));
                    arr[2] = Convert.ToChar(s3.Substring(s3.Length - 1, 1));
                }
                else
                {
                    s1 = Convert.ToString(Math.Abs(Int64.Parse(arr[0].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) - 1));
                    s2 = Convert.ToString(Math.Abs(Int64.Parse(arr[1].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) - 1));
                    s3 = Convert.ToString(Math.Abs(Int64.Parse(arr[2].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) - 1));
                    arr[0] = Convert.ToChar(s1.Substring(s1.Length - 1, 1));
                    arr[1] = Convert.ToChar(s2.Substring(s2.Length - 1, 1));
                    arr[2] = Convert.ToChar(s3.Substring(s3.Length - 1, 1));
                }

                if (Int64.Parse(arr[6].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) % 2 == 0)
                {
                    s1 = Convert.ToString(Math.Abs(Int64.Parse(arr[4].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) + 7));
                    s2 = Convert.ToString(Math.Abs(Int64.Parse(arr[5].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) + 7));
                    s3 = Convert.ToString(Math.Abs(Int64.Parse(arr[6].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) + 7));
                    s4 = Convert.ToString(Math.Abs(Int64.Parse(arr[7].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) + 7));
                    arr[4] = Convert.ToChar(s1.Substring(s1.Length - 1, 1));
                    arr[5] = Convert.ToChar(s2.Substring(s2.Length - 1, 1));
                    arr[6] = Convert.ToChar(s3.Substring(s3.Length - 1, 1));
                    arr[7] = Convert.ToChar(s4.Substring(s4.Length - 1, 1));

                }
                else
                {
                    s1 = Convert.ToString(Math.Abs(Int64.Parse(arr[4].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) - 7));
                    s2 = Convert.ToString(Math.Abs(Int64.Parse(arr[5].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) - 7));
                    s3 = Convert.ToString(Math.Abs(Int64.Parse(arr[6].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) - 7));
                    s4 = Convert.ToString(Math.Abs(Int64.Parse(arr[7].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) - 7));
                    arr[4] = Convert.ToChar(s1.Substring(s1.Length - 1, 1));
                    arr[5] = Convert.ToChar(s2.Substring(s2.Length - 1, 1));
                    arr[6] = Convert.ToChar(s3.Substring(s3.Length - 1, 1));
                    arr[7] = Convert.ToChar(s4.Substring(s4.Length - 1, 1));
                }

                if (Int64.Parse(arr[11].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) % 2 == 0)
                {
                    s1 = Convert.ToString(Math.Abs(Int64.Parse(arr[8].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) + 12));
                    s2 = Convert.ToString(Math.Abs(Int64.Parse(arr[9].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) + 12));
                    s3 = Convert.ToString(Math.Abs(Int64.Parse(arr[10].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) + 12));
                    s4 = Convert.ToString(Math.Abs(Int64.Parse(arr[11].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) + 12));
                    arr[8] = Convert.ToChar(s1.Substring(s1.Length - 1, 1));
                    arr[9] = Convert.ToChar(s2.Substring(s2.Length - 1, 1));
                    arr[10] = Convert.ToChar(s3.Substring(s3.Length - 1, 1));
                    arr[11] = Convert.ToChar(s4.Substring(s4.Length - 1, 1));

                }
                else
                {
                    s1 = Convert.ToString(Math.Abs(Int64.Parse(arr[8].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) - 12));
                    s2 = Convert.ToString(Math.Abs(Int64.Parse(arr[9].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) - 12));
                    s3 = Convert.ToString(Math.Abs(Int64.Parse(arr[10].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) - 12));
                    s4 = Convert.ToString(Math.Abs(Int64.Parse(arr[11].ToString().Trim(), System.Globalization.NumberStyles.HexNumber) - 12));
                    arr[8] = Convert.ToChar(s1.Substring(s1.Length - 1, 1));
                    arr[9] = Convert.ToChar(s2.Substring(s2.Length - 1, 1));
                    arr[10] = Convert.ToChar(s3.Substring(s3.Length - 1, 1));
                    arr[11] = Convert.ToChar(s4.Substring(s4.Length - 1, 1));
                }


                StringBuilder sLevel1 = new StringBuilder();

                sLevel1.Append(arr, 0, arr.Length);

                string sDate = "";
                StringBuilder sb = new StringBuilder();
                if (NeverExp)
                    sb.Append("123199");
                else
                    sb.Append(ExpDate.ToString("MMddyy"));

                sDate = String.Format("{0:x}", Convert.ToInt64(sb.ToString())).PadLeft(5, '0');

                char[] arrDate = sDate.ToCharArray();

                //Application Code
                String AppCodeStr;
                AppCodeStr = dType.PadLeft(2, '0');

                ///ADD date And Random NO
                if (Int64.Parse(sMACAddress.Trim(), System.Globalization.NumberStyles.HexNumber) % 5 == 0) //15 10 4 20 15 5 7 12 16 14 11 10 9, D C A
                {
                    #region "mod 5"
                    sLevel1.Insert(9, arrDate[4].ToString());
                    sLevel1.Insert(10, arrDate[3].ToString());
                    sLevel1.Insert(11, arrDate[2].ToString());
                    sLevel1.Insert(14, arrDate[1].ToString());
                    sLevel1.Insert(16, arrDate[0].ToString());



                    //AppCodeStr = Convert.ToString(Convert.ToInt32(dType));
                    //String.Format("{0:x}", Convert.ToInt64(AppCodeStr)).PadLeft(2, 'F');

                    char[] appCode = AppCodeStr.ToCharArray();

                    sLevel1.Insert(12, appCode[0].ToString());
                    sLevel1.Insert(7, appCode[1].ToString());
                    noOfItems = noOfItems.PadLeft(3, '0');

                    if (noOfItems != "000")
                    {
                        //Create No of Rows code
                        char[] NoOfItemsAr = noOfItems.ToCharArray();
                        sLevel1.Insert(5, NoOfItemsAr[0].ToString());
                        sLevel1.Insert(15, NoOfItemsAr[1].ToString());
                        sLevel1.Insert(20, NoOfItemsAr[2].ToString());
                    }
                    else
                    {
                        //Generate 3 more number
                        sLevel1.Insert(5, "A");
                        sLevel1.Insert(15, "C");
                        sLevel1.Insert(20, "D");
                    }




                    //int i1 = random.Next(0, 9);
                    int i2 = random.Next(0, 9);
                    int i3 = random.Next(0, 9);

                    sLevel1 = new StringBuilder(enHex(sLevel1.ToString(), RefNumber));

                    //sLevel1.Insert(4, i1.ToString());
                    sLevel1.Insert(10, i2.ToString());
                    sLevel1.Insert(15, i3.ToString());

                    #endregion
                }
                else if (Int64.Parse(sMACAddress.Trim(), System.Globalization.NumberStyles.HexNumber) % 3 == 0) // 18 15 3 21 16 6 9 12, 16 15 9 5 1, 5 D 1 
                {
                    #region "mod 3"

                    sLevel1.Insert(1, arrDate[0].ToString());
                    sLevel1.Insert(5, arrDate[1].ToString());
                    sLevel1.Insert(9, arrDate[2].ToString());
                    sLevel1.Insert(15, arrDate[3].ToString());
                    sLevel1.Insert(16, arrDate[4].ToString());

                    //Application Code
                    //String AppCodeStr;
                    //AppCodeStr = Convert.ToString(Convert.ToInt32(dType));
                    //AppCodeStr = String.Format("{0:x}", Convert.ToInt64(AppCodeStr)).PadLeft(2, 'F');
                    char[] appCode = AppCodeStr.ToCharArray();

                    sLevel1.Insert(12, appCode[0].ToString());
                    sLevel1.Insert(9, appCode[1].ToString());

                    //////////////////
                    if (noOfItems != "000")
                    {
                        //Create No of Rows code
                        char[] NoOfItemsAr = noOfItems.ToCharArray();
                        sLevel1.Insert(6, NoOfItemsAr[0].ToString());
                        sLevel1.Insert(16, NoOfItemsAr[1].ToString());
                        sLevel1.Insert(21, NoOfItemsAr[2].ToString());
                    }
                    else
                    {
                        //Generate 3 more Random number
                        sLevel1.Insert(6, "1");
                        sLevel1.Insert(16, "D");
                        sLevel1.Insert(21, "5");
                    }


                    //int i1 = random.Next(0, 9);
                    int i2 = random.Next(0, 9);
                    int i3 = random.Next(0, 9);

                    sLevel1 = new StringBuilder(enHex(sLevel1.ToString(), RefNumber));

                    //sLevel1.Insert(3, i1.ToString());
                    sLevel1.Insert(15, i2.ToString());
                    sLevel1.Insert(18, i3.ToString());


                    #endregion
                }
                else if (Int64.Parse(sMACAddress.Trim(), System.Globalization.NumberStyles.HexNumber) % 2 == 0) //17 5 3 19 14 4 16 15, 15 13 9 8 3, 5 D 8 
                {
                    #region "mod 2"
                    sLevel1.Insert(3, arrDate[0].ToString());
                    sLevel1.Insert(8, arrDate[1].ToString());
                    sLevel1.Insert(9, arrDate[2].ToString());
                    sLevel1.Insert(13, arrDate[3].ToString());
                    sLevel1.Insert(15, arrDate[4].ToString());

                    //Application Code
                    //String AppCodeStr;
                    //AppCodeStr = Convert.ToString(Convert.ToInt32(dType));
                    //AppCodeStr = String.Format("{0:x}", Convert.ToInt64(AppCodeStr)).PadLeft(2, 'F');
                    char[] appCode = AppCodeStr.ToCharArray();

                    sLevel1.Insert(15, appCode[0].ToString());
                    sLevel1.Insert(16, appCode[1].ToString());


                    ///////////////////////
                    if (noOfItems != "000")
                    {
                        //Create No of Rows code
                        char[] NoOfItemsAr = noOfItems.ToCharArray();
                        sLevel1.Insert(4, NoOfItemsAr[0].ToString());
                        sLevel1.Insert(14, NoOfItemsAr[1].ToString());
                        sLevel1.Insert(19, NoOfItemsAr[2].ToString());
                    }
                    else
                    {
                        //Generate 3 more number
                        sLevel1.Insert(4, "8");
                        sLevel1.Insert(14, "D");
                        sLevel1.Insert(19, "5");
                    }

                    /////////////////////
                    //int i1 = random.Next(0, 9);
                    int i2 = random.Next(0, 9);
                    int i3 = random.Next(0, 9);

                    sLevel1 = new StringBuilder(enHex(sLevel1.ToString(), RefNumber));

                    //sLevel1.Insert(3, i1.ToString());
                    sLevel1.Insert(5, i2.ToString());
                    sLevel1.Insert(17, i3.ToString());

                    #endregion
                }
                else  // 19 13 4 12 10 1 2 7, 12 10 8 6 4, A 6 9  
                {
                    #region "mod else"
                    sLevel1.Insert(4, arrDate[4].ToString());
                    sLevel1.Insert(6, arrDate[3].ToString());
                    sLevel1.Insert(8, arrDate[2].ToString());
                    sLevel1.Insert(10, arrDate[1].ToString());
                    sLevel1.Insert(12, arrDate[0].ToString());

                    ///////
                    //Application Code
                    //String AppCodeStr;
                    //AppCodeStr = Convert.ToString(Convert.ToInt32(dType));
                    //AppCodeStr = String.Format("{0:x}", Convert.ToInt64(AppCodeStr)).PadLeft(2, 'F');
                    char[] appCode = AppCodeStr.ToCharArray();

                    sLevel1.Insert(7, appCode[0].ToString());
                    sLevel1.Insert(2, appCode[1].ToString());


                    ///////////
                    if (noOfItems != "000")
                    {
                        //Create No of Rows code
                        char[] NoOfItemsAr = noOfItems.ToCharArray();
                        sLevel1.Insert(1, NoOfItemsAr[0].ToString());
                        sLevel1.Insert(10, NoOfItemsAr[1].ToString());
                        sLevel1.Insert(12, NoOfItemsAr[2].ToString());
                    }
                    else
                    {
                        //Generate 3 more Random number
                        sLevel1.Insert(1, "9");
                        sLevel1.Insert(10, "6");
                        sLevel1.Insert(12, "A");
                    }

                    ///
                    //int i1 = random.Next(0, 9);
                    int i2 = random.Next(0, 9);
                    int i3 = random.Next(0, 9);

                    sLevel1 = new StringBuilder(enHex(sLevel1.ToString(), RefNumber));

                    //sLevel1.Insert(4, i1.ToString());
                    sLevel1.Insert(13, i2.ToString());
                    sLevel1.Insert(19, i3.ToString());


                    #endregion
                }
                return sLevel1.ToString();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        //This function used to compar the User entered key and Generated Key.
        public static Boolean CompareKey(string key1, string key2, string RefNumber)
        {
            if (KeyValue(key1, RefNumber).Trim().ToString().ToLower() == KeyValue(key2, RefNumber).Trim().ToString().ToLower())
            {
                return true;
            }
            return false;
        }

        //Function is Used to ADD to HexaDecimal No. It is not return carry
        private static string addHexString(String S1, String S2, Int32 lth)
        {
            Int32 tempInt = 0;
            String tempStr = "";
            String ReminderString = "0";
            for (Int32 i = lth - 1; i >= 0; i--)
            {
                tempInt = Convert.ToInt32(S1.Substring(i, 1), 16) + Convert.ToInt32(S2.Substring(i, 1), 16) + Convert.ToInt32(ReminderString, 16);
                ReminderString = Convert.ToString(tempInt, 16).PadLeft(2, '0').Substring(0, 1);
                tempStr = Convert.ToString(tempInt, 16).PadLeft(2, '0').Substring(1, 1) + tempStr;
            }
            tempStr = ReminderString + tempStr;
            return tempStr.ToUpper();
        }

        //Function is Used to Sustract two HexaDecimal No. and returned the specified length limit.
        private static string subHexString(String FromS1, String ToS2, Int32 lth)
        {
            Int32 Returnlth = lth + 1;
            FromS1 = FromS1.PadLeft(Returnlth, '0');
            ToS2 = ToS2.PadLeft(Returnlth, '0');
            Int32 tempInt = 0;
            String tempStr = "";
            String ReminderString = "0";
            for (Int32 i = Returnlth - 1; i > 0; i--)
            {
                if (i == Returnlth - 1)
                {
                    tempInt = Convert.ToInt32(FromS1.Substring(i, 1).PadLeft(2, '1'), 16) - Convert.ToInt32(ToS2.Substring(i, 1), 16) + Convert.ToInt32(ReminderString, 16);
                }
                else
                {
                    tempInt = Convert.ToInt32(FromS1.Substring(i, 1).PadLeft(2, '1'), 16) - Convert.ToInt32(ToS2.Substring(i, 1), 16) + Convert.ToInt32(ReminderString, 16) - 1;
                }
                ReminderString = Convert.ToString(tempInt, 16).PadLeft(2, '0').Substring(0, 1);
                tempStr = Convert.ToString(tempInt, 16).PadLeft(2, '0').Substring(1, 1) + tempStr;
            }
            //tempStr = ReminderString + tempStr;
            return tempStr.ToUpper();
        }

        //This function is helpfull to maintain old version compability, It return Encrypted Key with salting
        private static string deHex(String sb, string RefNumber)
        {

            if (sb.Length > 21)
            {
                //Int64 l1, l2, l3;
                //l2 = Convert.ToInt64("311185736E1266D38045F2", 16);

                //l3 = Convert.ToInt64(sb, 16);
                //l1 = l3 - l2;
                //sb = Convert.ToString(l1, 16).PadLeft(23, '0');

                return subHexString(sb, "311185736E1266D38045F2", 22);
            }
            else
            {
                string sMACAddress = ConvertToMacFormat(RefNumber);
                if (Int64.Parse(sMACAddress.Trim(), System.Globalization.NumberStyles.HexNumber) % 5 == 0)
                {
                    sb = sb.Remove(4, 1);
                    return sb.ToString();
                }
                else if (Int64.Parse(sMACAddress.Trim(), System.Globalization.NumberStyles.HexNumber) % 3 == 0)
                {
                    sb = sb.Remove(3, 1);
                    return sb.ToString();
                }
                else if (Int64.Parse(sMACAddress.Trim(), System.Globalization.NumberStyles.HexNumber) % 2 == 0)
                {
                    sb = sb.Remove(3, 1);
                    return sb.ToString();
                }
                else
                {
                    sb = sb.Remove(4, 1);
                    return sb.ToString();
                }

            }
            
        }

        //This function is helpfull to maintain old version compability, It takes encoded key and return Encrypted Key without salting.
        private static string enHex(String sb, string RefNumber)
        {
            if (sb.Length > 21)
            {
                //Int64 l1, l2, l3;
                //l2 = Convert.ToDouble("311185736E1266D38045F2", 16);

                //l1 = Convert.ToDouble(sb, 16);
                //l3 = l1 + l2;
                //sb = Convert.ToDouble(l3, 16).PadLeft(23, '0');

                return addHexString(sb, "311185736E1266D38045F2", 22);
            }
            else
            {
                string sMACAddress = ConvertToMacFormat(RefNumber);
                Random random = new Random();
                int i1 = random.Next(0, 9);
                if (Int64.Parse(sMACAddress.Trim(), System.Globalization.NumberStyles.HexNumber) % 5 == 0)
                {

                    sb = sb.Insert(4, i1.ToString());
                    return sb.ToString();
                }
                else if (Int64.Parse(sMACAddress.Trim(), System.Globalization.NumberStyles.HexNumber) % 3 == 0)
                {
                    sb = sb.Insert(3, i1.ToString());
                    return sb.ToString();
                }
                else if (Int64.Parse(sMACAddress.Trim(), System.Globalization.NumberStyles.HexNumber) % 2 == 0)
                {
                    sb = sb.Insert(3, i1.ToString());
                    return sb.ToString();
                }
                else
                {
                    sb = sb.Insert(4, i1.ToString());
                    return sb.ToString();
                }

            }
            
        }

        //It returns Encryted after removing all encoded values.
        private static String KeyValue(string key, string RefNumber)
        {
            try
            {
                string sKey = key;
                string sMACAddress = ConvertToMacFormat(RefNumber);
                //string strRe = "";
                StringBuilder sredult = new StringBuilder();
                StringBuilder sb = new StringBuilder(sKey);


                if (Int64.Parse(sMACAddress.Trim(), System.Globalization.NumberStyles.HexNumber) % 5 == 0)
                {
                    sb.Remove(15, 1);
                    sb.Remove(10, 1);

                    sb = new StringBuilder(deHex(sb.ToString(), RefNumber));

                    //l3 = Convert.ToInt32(sb.ToString(), 16);
                    //l1 = l3 - l2;
                    //sb = new StringBuilder(Convert.ToString(l1, 16).PadLeft(23, '0'));  

                    //sb.Remove(4, 1);

                    return sb.ToString();
                }
                else if (Int64.Parse(sMACAddress.Trim(), System.Globalization.NumberStyles.HexNumber) % 3 == 0)
                {
                    sb.Remove(18, 1);
                    sb.Remove(15, 1);
                    sb = new StringBuilder(deHex(sb.ToString(), RefNumber));
                    //sb.Remove(3, 1);

                    return sb.ToString();
                }

                else if (Int64.Parse(sMACAddress.Trim(), System.Globalization.NumberStyles.HexNumber) % 2 == 0)
                {
                    sb.Remove(17, 1);
                    sb.Remove(5, 1);
                    sb = new StringBuilder(deHex(sb.ToString(), RefNumber));
                    //sb.Remove(3, 1);

                    return sb.ToString();
                }
                else
                {

                    sb.Remove(19, 1);
                    sb.Remove(13, 1);
                    sb = new StringBuilder(deHex(sb.ToString(), RefNumber));
                    //sb.Remove(4, 1);

                    return sb.ToString();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //It returns Expiry Date
        public static DateTime GetExpiryDate(string key, string RefNumber)
        {
            try
            {
                string sKey = key;
                string sMACAddress = ConvertToMacFormat(RefNumber);
                string strRe = "";
                StringBuilder sredult = new StringBuilder();
                StringBuilder sb = new StringBuilder(sKey);
                if (Int64.Parse(sMACAddress.Trim(), System.Globalization.NumberStyles.HexNumber) % 5 == 0) //15 10 4 20 15 5 7 12
                {
                    sb.Remove(15, 1);
                    sb.Remove(10, 1);
                    //sb.Remove(4, 1);
                    sb = new StringBuilder(deHex(sb.ToString(), RefNumber));

                    if (key.Length >= 25)
                    {
                        sb.Remove(20, 1);
                        sb.Remove(15, 1);
                        sb.Remove(5, 1);
                        sb.Remove(7, 1);
                        sb.Remove(12, 1);
                    }

                    sredult.Append(sb.ToString().Substring(16, 1));
                    sredult.Append(sb.ToString().Substring(14, 1));
                    sredult.Append(sb.ToString().Substring(11, 1));
                    sredult.Append(sb.ToString().Substring(10, 1));
                    sredult.Append(sb.ToString().Substring(9, 1));

                    strRe = sredult.ToString();

                }
                else if (Int64.Parse(sMACAddress.Trim(), System.Globalization.NumberStyles.HexNumber) % 3 == 0) //18 15 3 21 16 6 9 12
                {
                    sb.Remove(18, 1);
                    sb.Remove(15, 1);
                    //sb.Remove(3, 1);
                    sb = new StringBuilder(deHex(sb.ToString(), RefNumber));

                    if (key.Length >= 25)
                    {
                        sb.Remove(21, 1);
                        sb.Remove(16, 1);
                        sb.Remove(6, 1);
                        sb.Remove(9, 1);
                        sb.Remove(12, 1);
                    }

                    sredult.Append(sb.ToString().Substring(1, 1));
                    sredult.Append(sb.ToString().Substring(5, 1));
                    sredult.Append(sb.ToString().Substring(9, 1));
                    sredult.Append(sb.ToString().Substring(15, 1));
                    sredult.Append(sb.ToString().Substring(16, 1));

                    strRe = sredult.ToString();


                }

                else if (Int64.Parse(sMACAddress.Trim(), System.Globalization.NumberStyles.HexNumber) % 2 == 0) //17 5 3 19 14 4 16 15
                {
                    sb.Remove(17, 1);
                    sb.Remove(5, 1);
                    //sb.Remove(3, 1);
                    sb = new StringBuilder(deHex(sb.ToString(), RefNumber));

                    if (key.Length >= 25)
                    {
                        sb.Remove(19, 1);
                        sb.Remove(14, 1);
                        sb.Remove(4, 1);
                        sb.Remove(16, 1);
                        sb.Remove(15, 1);
                    }

                    sredult.Append(sb.ToString().Substring(3, 1));
                    //sb.Remove(17, 1);
                    sredult.Append(sb.ToString().Substring(8, 1));
                    sredult.Append(sb.ToString().Substring(9, 1));
                    sredult.Append(sb.ToString().Substring(13, 1));
                    sredult.Append(sb.ToString().Substring(15, 1));
                    strRe = sredult.ToString();


                }
                else //19 13 4 12 10 1 2 7
                {
                    sb.Remove(19, 1);
                    sb.Remove(13, 1);
                    //sb.Remove(4, 1);
                    sb = new StringBuilder(deHex(sb.ToString(), RefNumber));

                    if (key.Length >= 25)
                    {
                        sb.Remove(12, 1);
                        sb.Remove(10, 1);
                        sb.Remove(1, 1);
                        sb.Remove(2, 1);
                        sb.Remove(7, 1);
                    }

                    sredult.Append(sb.ToString().Substring(12, 1));
                    sredult.Append(sb.ToString().Substring(10, 1));
                    sredult.Append(sb.ToString().Substring(8, 1));
                    sredult.Append(sb.ToString().Substring(6, 1));
                    sredult.Append(sb.ToString().Substring(4, 1));


                    strRe = sredult.ToString();

                }
                strRe = Convert.ToInt32(strRe, 16).ToString();

                if (strRe.Length == 5)
                {
                    sb.Remove(0, sb.Length);
                    sb.Append("0");
                    sb.Append(strRe.Substring(0, 1));
                    sb.Append("/");
                    sb.Append(strRe.Substring(1, 2));
                    sb.Append("/20");
                    sb.Append(strRe.Substring(3, 2));
                }
                if (strRe.Length == 6)
                {
                    sb.Remove(0, sb.Length);
                    sb.Append(strRe.Substring(0, 2));
                    sb.Append("/");
                    sb.Append(strRe.Substring(2, 2));
                    sb.Append("/20");
                    sb.Append(strRe.Substring(4, 2));
                }

                try
                {
                    //return Convert.ToDateTime(sb.ToString());
                    //System.Globalization.CultureInfo p = System.Threading.Thread.CurrentThread.CurrentCulture;
                    //System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
                    //DateTime rtDate = Convert.ToDateTime(sb.ToString());
                    //System.Threading.Thread.CurrentThread.CurrentCulture = p;//CultureInfo.CreateSpecificCulture("de-DE");
                    //DateTime rtDate;
                    //rtDate = DateTime.ParseExact(sb.ToString(), "MM/dd/yyyy", null);

                    DateTime rtDate;
                    Int32 iDay, iMonth, iYear = 0;
                    iMonth = Convert.ToInt32(sb.ToString(0, 2));

                    iDay = Convert.ToInt32(sb.ToString(3, 2));

                    iYear = Convert.ToInt32(sb.ToString(6, 4));

                    rtDate = new DateTime(iYear, iMonth, iDay); 
                    return rtDate;

                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Invalid Hardware Key or Registration Key.", ex);
                }

            }
            catch (Exception ex)
            {
                throw new ApplicationException("Invalid Hardware Key or Registration key.", ex);
            }

        }

        //It returns Application code from Key
        public static String GetApplicationCode(string key, string RefNumber)
        {
            try
            {
                string sKey = key;
                string sMACAddress = ConvertToMacFormat(RefNumber);
                string strRe = "";
                StringBuilder sredult = new StringBuilder();
                StringBuilder sb = new StringBuilder(sKey);
                if (Int64.Parse(sMACAddress.Trim(), System.Globalization.NumberStyles.HexNumber) % 5 == 0)  //15 10 4 20 15 5 7 12
                {
                    sb.Remove(15, 1);
                    sb.Remove(10, 1);

                    //sb.Remove(4, 1);
                    sb = new StringBuilder(deHex(sb.ToString(), RefNumber));

                    if (key.Length >= 25)
                    {
                        //Remove No of Items
                        sb.Remove(20, 1);
                        sb.Remove(15, 1);
                        sb.Remove(5, 1);
                    }
                    sredult.Append(sb.ToString().Substring(7, 1));
                    sb.Remove(7, 1);
                    sredult.Append(sb.ToString().Substring(12, 1));
                    sb.Remove(12, 1);
                    strRe = sredult.ToString();

                }
                else if (Int64.Parse(sMACAddress.Trim(), System.Globalization.NumberStyles.HexNumber) % 3 == 0) //18 15 3 21 16 6 9 12
                {
                    sb.Remove(18, 1);
                    sb.Remove(15, 1);
                    //sb.Remove(3, 1);
                    sb = new StringBuilder(deHex(sb.ToString(), RefNumber));

                    if (key.Length >= 25)
                    {
                        //Remove No of Items
                        sb.Remove(21, 1);
                        sb.Remove(16, 1);
                        sb.Remove(6, 1);
                    }
                    sredult.Append(sb.ToString().Substring(9, 1));
                    sb.Remove(9, 1);
                    sredult.Append(sb.ToString().Substring(12, 1));
                    sb.Remove(12, 1);

                    strRe = sredult.ToString();


                }

                else if (Int64.Parse(sMACAddress.Trim(), System.Globalization.NumberStyles.HexNumber) % 2 == 0) //17 5 3 19 14 4 16 15
                {
                    sb.Remove(17, 1);
                    sb.Remove(5, 1);
                    //sb.Remove(3, 1);
                    sb = new StringBuilder(deHex(sb.ToString(), RefNumber));

                    if (key.Length >= 25)
                    {
                        //Remove No of Items
                        sb.Remove(19, 1);
                        sb.Remove(14, 1);
                        sb.Remove(4, 1);
                    }
                    sredult.Append(sb.ToString().Substring(16, 1));
                    sb.Remove(16, 1);
                    sredult.Append(sb.ToString().Substring(15, 1));
                    sb.Remove(15, 1);
                    strRe = sredult.ToString();


                }
                else //19 13 4 12 10 1 2 7
                {
                    sb.Remove(19, 1);
                    sb.Remove(13, 1);
                    //sb.Remove(4, 1);
                    sb = new StringBuilder(deHex(sb.ToString(), RefNumber));

                    if (key.Length >= 25)
                    {
                        //Remove No of Items
                        sb.Remove(12, 1);
                        sb.Remove(10, 1);
                        sb.Remove(1, 1);
                    }
                    sredult.Append(sb.ToString().Substring(2, 1));
                    sb.Remove(2, 1);
                    sredult.Append(sb.ToString().Substring(7, 1));
                    sb.Remove(7, 1);
                    strRe = sredult.ToString();
                }
                String returnString = "";
                returnString = strRe.Substring(1, 1) + strRe.Substring(0, 1);
                strRe = Convert.ToString(Convert.ToInt32(returnString, 16), 2);
                return strRe.PadLeft(4, '0');
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Invalid Hardware Key or Registration key.", ex);
            }
        }

        //It returns No of Items from Key
        public static int GetMaxNoofItems(string key, string RefNumber)
        {
            String intPart = "";
            String strRe = GetMaxNoofItemsCode(key, RefNumber);


            intPart = strRe.Substring(1, 1) + strRe.Substring(2, 1);

            Int32 iReturn = 0;
            if (strRe.Substring(0, 1).ToString() == "0")
            {
                iReturn = Convert.ToInt32(intPart);
            }
            else if (strRe.Substring(0, 1).ToString() == "1")
            {
                iReturn = (Convert.ToInt32(intPart) * 100);
            }
            else if (strRe.Substring(0, 1).ToString() == "2")
            {
                iReturn = (Convert.ToInt32(intPart) * 1000);
            }
            else
            {
                throw new ApplicationException("Invalid Key");
            }
            return iReturn;
        }

        //It returns No of Items from Item Code
        public static int GetMaxNoofItems(string ItemCode)
        {
            String intPart;
            String strRe = ItemCode; 
            intPart = strRe.Substring(1, 1) + strRe.Substring(2, 1);
            Int32 iReturn = 0;
            if (strRe.Substring(0, 1).ToString() == "0")
            {
                iReturn = Convert.ToInt32(intPart);
            }
            else if (strRe.Substring(0, 1).ToString() == "1")
            {
                iReturn = (Convert.ToInt32(intPart) * 100);
            }
            else if (strRe.Substring(0, 1).ToString() == "2")
            {
                iReturn = (Convert.ToInt32(intPart) * 1000);
            }
            else
            {
                throw new ApplicationException("Invalid Key");
            }
            return iReturn;
        }

        //It returns No of Items in encoded string
        public static String GetMaxNoofItemsCode(string key, string RefNumber)
        {
            try
            {
                string sKey = key;
                string sMACAddress = ConvertToMacFormat(RefNumber);
                string strRe = "";
                StringBuilder sredult = new StringBuilder();
                StringBuilder sb = new StringBuilder(sKey);
                if (Int64.Parse(sMACAddress.Trim(), System.Globalization.NumberStyles.HexNumber) % 5 == 0)  //15 10 4 20 15 5 
                {
                    sb.Remove(15, 1);
                    sb.Remove(10, 1);
                    //sb.Remove(4, 1);
                    sb = new StringBuilder(deHex(sb.ToString(), RefNumber));

                    sredult.Append(sb.ToString().Substring(20, 1));
                    sb.Remove(20, 1);
                    sredult.Append(sb.ToString().Substring(15, 1));
                    sb.Remove(15, 1);
                    sredult.Append(sb.ToString().Substring(5, 1));
                    sb.Remove(5, 1);
                    strRe = sredult.ToString();

                }
                else if (Int64.Parse(sMACAddress.Trim(), System.Globalization.NumberStyles.HexNumber) % 3 == 0) //18 15 3 21 16 6 
                {
                    sb.Remove(18, 1);
                    sb.Remove(15, 1);
                    //sb.Remove(3, 1);
                    sb = new StringBuilder(deHex(sb.ToString(), RefNumber));

                    sredult.Append(sb.ToString().Substring(21, 1));
                    sb.Remove(21, 1);
                    sredult.Append(sb.ToString().Substring(16, 1));
                    sb.Remove(16, 1);
                    sredult.Append(sb.ToString().Substring(6, 1));
                    sb.Remove(6, 1);
                    strRe = sredult.ToString();
                }

                else if (Int64.Parse(sMACAddress.Trim(), System.Globalization.NumberStyles.HexNumber) % 2 == 0) //17 5 3 19 14 4 
                {
                    sb.Remove(17, 1);
                    sb.Remove(5, 1);
                    //sb.Remove(3, 1);
                    sb = new StringBuilder(deHex(sb.ToString(), RefNumber));

                    sredult.Append(sb.ToString().Substring(19, 1));
                    sb.Remove(19, 1);
                    sredult.Append(sb.ToString().Substring(14, 1));
                    sb.Remove(14, 1);
                    sredult.Append(sb.ToString().Substring(4, 1));
                    sb.Remove(4, 1);
                    strRe = sredult.ToString();

                }
                else //19 13 4 12 10 1 
                {
                    sb.Remove(19, 1);
                    sb.Remove(13, 1);
                    //sb.Remove(4, 1);
                    sb = new StringBuilder(deHex(sb.ToString(), RefNumber));

                    sredult.Append(sb.ToString().Substring(12, 1));
                    sb.Remove(12, 1);
                    sredult.Append(sb.ToString().Substring(10, 1));
                    sb.Remove(10, 1);
                    sredult.Append(sb.ToString().Substring(1, 1));
                    sb.Remove(1, 1);
                    strRe = sredult.ToString();
                }

                //It should has 3 digit.
                strRe = strRe.Replace("A69", "000");
                strRe = strRe.Replace("5D8", "000");
                strRe = strRe.Replace("5D1", "000");
                strRe = strRe.Replace("DCA", "000");

                return strRe.Substring(2, 1) + strRe.Substring(1, 1) + strRe.Substring(0, 1);


            }
            catch (Exception ex)
            {
                throw new ApplicationException("Invalid Hardware Key or Registration key.", ex);
            }

        }

        //Function is used to validate Mac Address, entered by user.
        public static bool Validate_MACAddress(string RefNumber)
        {
            foreach (char c in RefNumber.ToUpper().ToCharArray())
            {
                if (!IsNumeric(c.ToString()))
                {
                    if (c == 'A' || c == 'B' || c == 'C' || c == 'D' || c == 'E' || c == 'F')
                    {
                        //
                    }
                    else
                        return false;
                }

            }
            return true;
        }

        //Function check the Hex Decimal No.
        public static bool IsNumeric(object num)
        {
            try
            {
                Convert.ToInt64(num);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //Function validate that user entered key passed in prameter are correct or not.
        public static bool IsValidKey(string key, string RefNumber, out String appCode,out DateTime ExpiryDate,out string NoOfItemCode)
        {
            String RefNo = RefNumber.ToUpper();
            String EnteredKey = key.ToUpper();

            int iClient = 0;

            string appCodeDecoded = GetAppModuleCodes(ref EnteredKey, ref iClient);
            appCodeDecoded = Convert.ToString(Convert.ToInt32(appCodeDecoded, 16), 2);
            appCodeDecoded = appCodeDecoded.PadLeft(32, '0');

            //Pref.allApplicationModules = appCodeDecoded;

            appCode = GetApplicationCode(EnteredKey, RefNo); //04
            //MessageBox.Show("AppCode " + appCode);
            ExpiryDate = GetExpiryDate(EnteredKey, RefNo);
            //MessageBox.Show("Expir " + ExpiryDate);
            NoOfItemCode = GetMaxNoofItemsCode(EnteredKey, RefNo);
            //MessageBox.Show("no items " + NoOfItems);
            String TempKey = Generate_KeyV2(RefNo, false, ExpiryDate, "D E E P @ R A M P", Convert.ToString(Convert.ToInt32(appCode, 2), 16), NoOfItemCode);
            //MessageBox.Show("temp key " + TempKey);
            if (CompareKey(TempKey, EnteredKey, RefNo))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public static String GetAppModuleCodes(ref string key, ref int Client)
        {
            StringBuilder sbKey = new StringBuilder(key);

            StringBuilder sbAppCode = new StringBuilder();

            Client = Convert.ToInt32(sbKey[27].ToString());
            sbAppCode.Append(sbKey[24]);
            sbAppCode.Append(sbKey[21]);
            sbAppCode.Append(sbKey[18]);
            sbAppCode.Append(sbKey[15]);
            sbAppCode.Append(sbKey[12]);
            sbAppCode.Append(sbKey[9]);
            sbAppCode.Append(sbKey[6]);
            sbAppCode.Append(sbKey[3]);

            sbKey.Remove(27, 1);

            sbKey.Remove(24, 1);

            sbKey.Remove(21, 1);

            sbKey.Remove(18, 1);

            sbKey.Remove(15, 1);

            sbKey.Remove(12, 1);

            sbKey.Remove(9, 1);

            sbKey.Remove(6, 1);

            sbKey.Remove(3, 1);

            key = sbKey.ToString();

            return sbAppCode.ToString();
        }


        //private static string enCode(string str)
        //{
        //    //char[] strAr=str.ToCharArray();
        //    //foreach(char ch in strAr)
        //    //{
        //    //    //ch =Convert.ToInt32(ch); 
        //    //}
        //}

        #endregion
    }
    
}
