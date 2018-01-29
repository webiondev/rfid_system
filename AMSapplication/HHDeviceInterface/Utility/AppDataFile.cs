using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
//using ClslibRfidSp;
using ClsLibBKLogs;
using ReaderTypes;

namespace OnRamp
{
    /// <summary>
    /// Each instance represents a file for this application under 
    /// System's app data Folder
    /// </summary>
    class AppDataFile
    {
        private Environment.SpecialFolder folderType = Environment.SpecialFolder.ApplicationData;
        private String folderPath = null;
        private String filePath = null;
       
        private Hashtable records = null;

        /// <summary>
        /// Initialize member variables
        /// </summary>
        /// <param name="filename"></param>
        public AppDataFile(String filename)
        {
            records = new Hashtable();

           

            try
            {
                folderPath = Environment.GetFolderPath(folderType) + @"\RFID\";

               // folderPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
               // folderPath = folderPath + "\\Data\\";
                                
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);
                //filePath = folderPath + @"\" + filename;

                filePath = folderPath + filename;

                if (!File.Exists(filePath))
                {
                    UserPref.IsFileFound = false;
                    Logger.LogError(filename + " file not found at " + folderPath); 
                }
                
                
            }
            //catch (System.Web.Services.Protocols.SoapException ex)
            //{
            //    //if (ex.Actor.ToString().ToLower().Trim() == "getlogin")
            //    //    Program.ShowError("Request from innvalid IP address.");
            //    //else
            //    //    Program.ShowError("Network Protocol Failure.");
            //    Logger.LogError(ex.Message); 
            //}
            catch (Exception e)
            {
                Logger.LogError(e.Message); 
                throw new ApplicationException("Could not access/create application data folder" + e.Message);
            }

            if (!LoadRecordsFromFile())
            {
                throw new ApplicationException("Unable to load data file. Possible file corruption.");
            }
        }


        private const Char RecEndMarker = ';';
        private const Char KeyValAssocMarker = '=';

        enum Tokenize
        {
            Idle,
            KeyStarted,
            KeyEnded, // mark by KeyValAssocMarker
            ValStarted,
            ValEnded, // mark by RecEndMarker
        }

        private bool LoadRecordsFromFile()
        {
            bool Succ = false;
            StreamReader rs = OpenStreamForReading();
            int RdVal;
            Char RdChar;
            StringBuilder KeySB, ValSB;
            KeySB = new StringBuilder();
            ValSB = new StringBuilder();
            Tokenize TokenSt = Tokenize.Idle;

            if (rs != null)
            {
                try
                {
                    // tokenize key, value and add to 'records'
                    while ((RdVal = rs.Read()) != -1) // EOF detection
                    {
                        RdChar = (Char)RdVal;
                        switch (RdChar)
                        {
                            case RecEndMarker:
                                bool NewRec = false;
                                switch (TokenSt)
                                {
                                    case Tokenize.ValStarted:
                                    case Tokenize.KeyEnded: // Empty Value Field
                                        TokenSt = Tokenize.ValEnded;
                                        NewRec = true;
                                        break;
                                }
                                if (NewRec)
                                {
                                    // Set Val to Key in Record
                                    records[KeySB.ToString()] = ValSB.ToString();
                                    // Reset String Builders
                                    KeySB = KeySB.Remove(0, KeySB.Length);
                                    ValSB = ValSB.Remove(0, ValSB.Length);
                                }
                                break;
                            case KeyValAssocMarker:
                                switch (TokenSt)
                                {
                                    case Tokenize.KeyStarted:
                                        TokenSt = Tokenize.KeyEnded;
                                        break;
                                }
                                break;
                            default:
                                switch (TokenSt)
                                {
                                    case Tokenize.Idle:
                                    case Tokenize.ValEnded:
                                        TokenSt = Tokenize.KeyStarted;
                                        KeySB = KeySB.Append(RdChar);
                                        break;
                                    case Tokenize.KeyStarted:
                                        KeySB = KeySB.Append(RdChar);
                                        break;
                                    case Tokenize.KeyEnded:
                                        TokenSt = Tokenize.ValStarted;
                                        ValSB = ValSB.Append(RdChar);
                                        break;
                                    case Tokenize.ValStarted:
                                        ValSB = ValSB.Append(RdChar);
                                        break;
                                }
                                break;
                        }
                    }
                    Succ = true;
                }
                catch (IOException)
                {
                    // Clear all Records
                    records.Clear();
                    Succ = false;
                   
                }
                finally
                {
                    rs.Close();
                }
            }
            else
                Succ = true; // No records from file                
            return Succ;
        }

        private bool SaveRecordsToFile()
        {
            StreamWriter ws = OpenStreamForWriting();
            bool Succ = false;
            if (ws != null)
            {
                //iterate the keys
                try
                {
                    foreach (String KeyStr in records.Keys)
                    {
                        ws.Write(KeyStr + KeyValAssocMarker + (String)records[KeyStr] + RecEndMarker);
                    }
                    Succ = true;
                }
                catch (IOException)
                {
                    Succ = false;
                }
                finally
                {
                    ws.Close();
                  
                }

                if (Directory.Exists(UserPref._BackUpDirPath))
                {
                    FileStream fs = null;
                    fs = new FileStream(UserPref._BackUpDirPath + UserPref.UserPrefFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
                    ws = new StreamWriter(fs, Encoding.UTF8);

                    try
                    {
                        foreach (String KeyStr in records.Keys)
                        {
                            ws.Write(KeyStr + KeyValAssocMarker + (String)records[KeyStr] + RecEndMarker);
                        }
                        Succ = true;
                    }
                    catch (IOException)
                    {
                        Succ = false;
                    }
                    finally
                    {
                        ws.Close();

                    }
                }
 
            }
            return Succ;
        }

        /// <summary>
        /// errMsg is set to null if return status is true(Succeed)
        /// </summary>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        private bool DeleteFile(out String errMsg)
        {
            bool Succ = false;
            errMsg = null;
            if (filePath != null)
            {
                try
                {
                    File.Delete(filePath);
                    Succ = true;
                }
                catch (IOException)
                {
                    errMsg = "File In Use";
                    Succ = false;
                }
                catch (UnauthorizedAccessException)
                {
                    errMsg = "Permission Denied";
                    Succ = false;
                }
            }

            return Succ;
        }

        private StreamReader OpenStreamForReading()
        {
            StreamReader rs = null;
            try
            {
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                rs = new StreamReader(fs, Encoding.UTF8);
            }
            catch (FileNotFoundException)
            {
                // allow file not found
                rs = null;
            }
            return rs;
        }

        /// <summary>
        /// Overwrite existing file (caller responsible for catching exceptions)
        /// </summary>
        /// <returns></returns>
        private StreamWriter OpenStreamForWriting()
        {
            StreamWriter ws = null;
            
            FileStream fs = null;
            fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            ws = new StreamWriter(fs, Encoding.UTF8);
        
            return ws;
        }

        /// <summary>
        /// Throws ApplicatiionException if failed
        /// </summary>
        /// <returns></returns>
        public void ClearAll()
        {
            // Delete File
            String ErrMsg;
            if (!DeleteFile(out ErrMsg))
            {
                throw new ApplicationException(ErrMsg);
            }
            else
            {
                // Clear HashTable
                records.Clear();
            }
        }

        public void ClearRecs(params String[] keys)
        {
            foreach (String key in keys)
            {
                records.Remove(key);
            }
            SaveRecordsToFile();
        }

        public void AddRec(String key, bool val)
        {
            records[key] = (val == true) ? "true" : "false";

            SaveRecordsToFile();
        }

        /// <summary>
        ///  Return boolean value of the key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns> false if key does not exist or value no correspond to boolean</returns>
        public bool GetBoolean(String key, out bool val)
        {
            bool Succ = false;
            val = false;
            if (records.ContainsKey(key))
            {
                String ValStr = (String)records[key];
                if (ValStr != null)
                {
                    if (String.Compare(ValStr, "true", true) == 0)
                    {
                        val = true;
                        Succ = true;
                    }
                    else if (String.Compare(ValStr, "false", true) == 0)
                    {
                        val = false;
                        Succ = true;
                    }
                }
            }
            return Succ;
        }

        public void AddRec(String key, Int32 val)
        {
            records[key] = val.ToString();
            SaveRecordsToFile();
        }

        public bool GetInt32(String key, out Int32 val)
        {
            bool Succ = false;
            val = 0;

            if (records.ContainsKey(key))
            {
                String ValStr = (String)records[key];
                if (ValStr != null)
                {
                    try
                    {
                        val = Int32.Parse(ValStr);
                        Succ = true;
                    }
                    catch
                    {
                        Succ = false;
                    }
                }
            }

            return Succ;
        }

        public void AddRec(String key, ref UINT96_T val)
        {
            records[key] = val.ToString();
            SaveRecordsToFile();
        }

        public bool GetUINT96(String key, out UINT96_T val)
        {
            bool Succ = false;
            val = new UINT96_T();
            if (records.ContainsKey(key))
            {
                String ValStr = (String)records[key];
                if (ValStr != null)
                {
                    Succ = val.ParseString(ValStr);
                }
            }

            return Succ;
        }

        public void AddRec(String key, Byte[] val)
        {
            if (val == null)
                val = new Byte[0];
            
            StringBuilder Sb = new StringBuilder(val.Length * 2);

            for (int i = 0; i < val.Length; i++)
                Sb = Sb.Append(val[i].ToString("X2"));

            records[key] = Sb.ToString();
            SaveRecordsToFile();
        }

        public bool GetByteArr(String key, out Byte[] val)
        {
            bool Succ = false;
            val = null;
            if (records.ContainsKey(key))
            {
                String ValStr = (String)records[key];

                if (ValStr != null && ValStr.Length > 0)
                {
                    try
                    {
                        val = new Byte[ValStr.Length / 2];
                        for (int bi = 0, si = 0; bi < val.Length; bi++, si += 2)
                            val[bi] = Byte.Parse(ValStr.Substring(si, 2), System.Globalization.NumberStyles.HexNumber);
                        Succ = true;
                    }
                    catch (System.FormatException)
                    {
                        Succ = false;
                    }
                }
                else
                {
                    val = new Byte[0];
                    Succ = true;
                }
            }
            else
                Succ = false;

            return Succ;
        }

        public void AddRec(String key, String val)
        {
            records[key] = val;
            SaveRecordsToFile();
        }

        public bool GetString(String key, out String val)
        {
            bool Succ = false;
            val = null;
            if (records.ContainsKey(key))
            {
                val = (String)records[key];
                Succ = true;
            }
            else
                Succ = false;

            return Succ;
        }

        public void AddRec(String key, float val)
        {
            records[key] = val.ToString();
            SaveRecordsToFile();
        }

        public bool GetFloat(String key, out float val)
        {
            bool Succ = false;
            val = 0;

            if (records.ContainsKey(key))
            {
                String ValStr = (String)records[key];
                if (ValStr != null)
                {
                    try
                    {
                        val = Single.Parse(ValStr);
                        Succ = true;
                    }
                    catch
                    {
                        Succ = false;
                    }
                }
            }

            return Succ;
        }
    }
}
