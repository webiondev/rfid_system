using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace HHDeviceInterface.Utility
{
    public class RegistrationData
    {
        public string FileName { get; set; }
        public string RegKey { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public bool IsKeyFound { get; set; }

        public RegistrationData()
        {
            IsKeyFound = false;
        }

        public void GetKeyData(string filePath,string pattern)
        {

            string keyFile;
            //if (filePath.LastIndexOf(@"\") == filePath.Length - 1)
            //{
            //    keyFile = filePath + FileName;
            //}
            //else
            //{
            //    keyFile = filePath + @"\" + FileName;
            //}

            keyFile = filePath + FileName;

            try
            {
                if (File.Exists(keyFile))
                {
                    StreamReader sr = new StreamReader(keyFile);
                    string key_LastDate = sr.ReadLine();

                    sr.Close();

                    string[] keydata = Regex.Split(key_LastDate, pattern);

                    string regKey = keydata[0];
                    string lastDate = keydata[1];

                   RegKey = regKey;

                    long dtTicks = long.Parse(lastDate, System.Globalization.NumberStyles.AllowHexSpecifier);

                    LastUpdateDate = new DateTime(dtTicks);

                    if (LastUpdateDate.Date < DateTime.Now.Date)
                    {
                        LastUpdateDate = DateTime.Now;
                    }

                    IsKeyFound = true;

                }
              
            }
            catch (Exception ex)
            {
                throw ex;
                //this.Close();
            }
        }

        public void SaveKeyData(string filePath, string pattern)
        {
            try
            {
                string keyFile;
                //if (filePath.LastIndexOf(@"\") == filePath.Length - 1)
                //{
                //    keyFile = filePath + FileName;
                //}
                //else
                //{
                //    keyFile = filePath + @"\" + FileName;
                //}

                keyFile = filePath + FileName;

                StreamWriter sw = new StreamWriter(keyFile, false);

                string dtValue = Convert.ToString(LastUpdateDate.Ticks, 16);

                string key_date = RegKey.Trim().ToUpper() + pattern + dtValue.ToUpper();

                sw.WriteLine(key_date);
                sw.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

     
}
