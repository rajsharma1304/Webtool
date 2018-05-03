using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SICT.DataContracts;
using System.Data;
using System.Net;
using System.Configuration;
using System.Web.Script.Serialization;
using SICT.DataAccessLayer;
using SICT.Constants;
using System.ComponentModel;
using System.Reflection;
using SICT.BusinessUtils;
using System.IO;
using SICT.BusinessLayer.V1;
using System.Timers;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;



namespace SICT.JanitorServices
{
    class JanitorService
    {
        private static readonly string CLASS_NAME = "JanitorService";
        private static Timer TargetVsCompletesCreationTimer; 
        private const string ZIPFILE = "ZipFile";
        private const string INTERVAL = "Interval";
        public static readonly string USIDBCONNECTION = "USIDBConnection";
        public static readonly string AIRDBCONNECTION = "AIRDBConnection";
        public static readonly string USDDBCONNECTION = "USDDBConnection";
        public static readonly string EURDBCONNECTION = "EURDBConnection";
        public static readonly string CONFIGURATION_TARGETVSCOMPLETESCHART_FILEPATH_USI = "TargetsVsCompletesChartFilePath_USI";
        public static readonly string CONFIGURATION_TARGETVSCOMPLETESCHART_FILEPATH_EUR = "TargetsVsCompletesChartFilePath_EUR";
        public static readonly string CONFIGURATION_TARGETVSCOMPLETESCHART_FILEPATH_USD = "TargetsVsCompletesChartFilePath_USD";
        public static readonly string CONFIGURATION_TARGETVSCOMPLETESCHART_FILEPATH_AIR = "TargetsVsCompletesChartFilePath_AIR";


        public void Start()
        {
            const string FUNCTION_NAME = "Start";
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start ");
            try
            {
                //TargetVsCompletesCreationTimer = new Timer();
                //double Interval = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings[INTERVAL]);
                //int Intervalhours = 24;
                //string Hour = DateTime.Now.ToString("HH");
                //string Minute = DateTime.Now.ToString("mm");
                //int TotalMinutes = Intervalhours * 60 + Convert.ToInt32(Minute);

                //int WaitMinutes;

                //if (TotalMinutes % Convert.ToInt32(Interval) == 0)
                //{
                //    TargetVsCompletesCreationTimer.Interval = 1 * 60 * 1000;
                //}
                //else
                //{
                //    if (TotalMinutes < Convert.ToInt32(Interval))
                //        WaitMinutes = Convert.ToInt32(Interval) - TotalMinutes;
                //    else
                //        WaitMinutes = TotalMinutes - Convert.ToInt32(Interval);
                //    TargetVsCompletesCreationTimer.Interval = WaitMinutes * 60 * 1000;
                //}
                TargetVsCompletesCreationTimer = new Timer();
                //// Tell the timer what top do when it elapses
                TargetVsCompletesCreationTimer.Elapsed += new ElapsedEventHandler(CompressLogFileandUpdateCacheFile);
                //// Set it to go off every five minutes
                double Interval = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings[INTERVAL]);
                TargetVsCompletesCreationTimer.Interval = Interval * 60 * 1000;
                //// And start it
                TargetVsCompletesCreationTimer.Start();

                CompressLogFileandUpdateCacheFile(null, null);

                //TargetVsCompletesCreationTimer.Elapsed += new ElapsedEventHandler(CompressLogFileandUpdateCacheFile);
                //TargetVsCompletesCreationTimer.Enabled = true;

                //CompressLogFileandUpdateCacheFile();

                Console.ReadLine();

            }
            catch (Exception Ex)
            {
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, Ex);
            }
        }


        /// <summary>
        ///Function to retrieve all log files, compress them using 7zip and cleaning up the log files.
        ///<param name="source"></param>
        ///<param name="e"></param>
        /// <returns></returns>
        private void CompressLogFileandUpdateCacheFile(object source, ElapsedEventArgs e)
        {
            const string FUNCTION_NAME = "CompressFile";
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start ");
            TargetVsCompletesCreationTimer.Stop();
            try
            {
                NameValueCollection Settings = System.Configuration.ConfigurationManager.GetSection("customAppSettingsGroup/Logs") as NameValueCollection;

                String[] AllFiles = Settings.AllKeys;
                foreach (String FileKey in AllFiles)
                {
                    string StrValue = Settings.Get(FileKey);
                    string[] StrArray = StrValue.Split(',');

                    String SourceFileName = StrArray[0];//settings.Get(FILENAME);
                    String SourceDir = StrArray[1];//settings.Get(SOURCEDIRECTORY);
                    String DestDir = StrArray[2];// settings.Get(DESTINATIONDIRECTORY);
                    string TgtFileName = StrArray[3];
                    string TargetName = string.Concat(Path.GetFileNameWithoutExtension(TgtFileName),
                            DateTime.Now.ToString("dd-MM-yyyy HH mm"), Path.GetExtension(TgtFileName));
                    TargetName = string.Concat(DestDir, TargetName);
                    // Use 7-zip, specify a=archive and -tgzip=gzip
                    foreach (string f in Directory.GetFiles(SourceDir, SourceFileName, SearchOption.AllDirectories))
                    {
                        string SourceName = f;
                        ProcessStartInfo Prc7ZipInfo = new ProcessStartInfo();
                        Prc7ZipInfo.FileName = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings[ZIPFILE]);
                        Prc7ZipInfo.Arguments = " a -tzip \"" + TargetName + "\" \"" + SourceName + "\" ";
                        Prc7ZipInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        Process Prc7Zip = Process.Start(Prc7ZipInfo);
                        Prc7Zip.WaitForExit();
                    }

                    foreach (string f in Directory.GetFiles(SourceDir, SourceFileName, SearchOption.AllDirectories))
                    {
                        if (!IsFileLocked(f))
                            File.Delete(f);
                    }
                }
                //for updating cache files               
                CreateCacheFileforTargetVsCompletesCharts(BusinessConstants.Instance.US.ToString());
                CreateCacheFileforTargetVsCompletesCharts(BusinessConstants.Instance.AIR.ToString());
                CreateCacheFileforTargetVsCompletesCharts(BusinessConstants.Instance.EUR.ToString());
                CreateCacheFileforTargetVsCompletesCharts(BusinessConstants.Instance.USD.ToString());
            }
            catch (Exception Ex)
            {
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, Ex);
            }
            double Interval = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings[INTERVAL]);
            TargetVsCompletesCreationTimer.Interval = Interval * 60 * 1000;
            TargetVsCompletesCreationTimer.Start();
          

            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "End ");
        }


        ///// <summary>
        ///// This Function is used to compress the cache files and update Dashboard Charts Cache Files
        ///// </summary>
        //private void CompressLogFileandUpdateCacheFile()
        //{
        //    const string FUNCTION_NAME = "CompressFile";
        //    SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start ");
        //    try
        //    {
        //        NameValueCollection Settings = System.Configuration.ConfigurationManager.GetSection("customAppSettingsGroup/Logs") as NameValueCollection;

        //        String[] AllFiles = Settings.AllKeys;
        //        foreach (String FileKey in AllFiles)
        //        {
        //            string StrValue = Settings.Get(FileKey);
        //            string[] StrArray = StrValue.Split(',');

        //            String SourceFileName = StrArray[0];//settings.Get(FILENAME);
        //            String SourceDir = StrArray[1];//settings.Get(SOURCEDIRECTORY);
        //            String DestDir = StrArray[2];// settings.Get(DESTINATIONDIRECTORY);
        //            string TgtFileName = StrArray[3];
        //            string TargetName = string.Concat(Path.GetFileNameWithoutExtension(TgtFileName),
        //                    DateTime.Now.ToString("dd-MM-yyyy HH mm"), Path.GetExtension(TgtFileName));
        //            TargetName = string.Concat(DestDir, TargetName);
        //            // Use 7-zip, specify a=archive and -tgzip=gzip
        //            foreach (string f in Directory.GetFiles(SourceDir, SourceFileName, SearchOption.AllDirectories))
        //            {
        //                string SourceName = f;
        //                ProcessStartInfo Prc7ZipInfo = new ProcessStartInfo();
        //                Prc7ZipInfo.FileName = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings[ZIPFILE]);
        //                Prc7ZipInfo.Arguments = " a -tzip \"" + TargetName + "\" \"" + SourceName + "\" ";
        //                Prc7ZipInfo.WindowStyle = ProcessWindowStyle.Hidden;
        //                Process Prc7Zip = Process.Start(Prc7ZipInfo);
        //                Prc7Zip.WaitForExit();
        //            }

        //            foreach (string f in Directory.GetFiles(SourceDir, SourceFileName, SearchOption.AllDirectories))
        //            {
        //                if (!IsFileLocked(f))
        //                    File.Delete(f);
        //            }
        //        }
        //        //for updating cache files               
        //        CreateCacheFileforTargetVsCompletesCharts(BusinessConstants.Instance.US.ToString());
        //        CreateCacheFileforTargetVsCompletesCharts(BusinessConstants.Instance.AIR.ToString());
        //        CreateCacheFileforTargetVsCompletesCharts(BusinessConstants.Instance.EUR.ToString());
        //        CreateCacheFileforTargetVsCompletesCharts(BusinessConstants.Instance.USD.ToString());
        //    }
        //    catch (Exception Ex)
        //    {

        //        SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, Ex);
        //    }

        //    SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "End ");
        //}

        /// <summary>
        ///Function to check if file is locked or is in use
        /// </summary>
        /// <param name="File">File name to be cheked</param>
        /// <returns>returns if file is locked or not.</returns>
        private bool IsFileLocked(String File)
        {
            const string FUNCTION_NAME = "IsFileLocked";
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start ");
            FileStream Stream = null;
            FileInfo Fileinfo = new FileInfo(File);
            try
            {
                Stream = Fileinfo.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                SICTLogger.WriteWarning(CLASS_NAME, FUNCTION_NAME, "File is in use");
                return true;
            }
            finally
            {
                if (Stream != null)
                    Stream.Close();
            }

            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "End ");
            return false;
        }

        #region DashboardCharts

        /// <summary>
        /// This Function is used to create TargetVsCompletes Charts for Each active Airport Login and Store it in the Aproapriate path
        /// </summary>
        /// <returns></returns>
        public ReturnValue CreateCacheFileforTargetVsCompletesCharts(string Instance)
        {
            const string FUNCTION_NAME = "CreateCacheFileforTargetVsCompletesCharts";
            ReturnValue RetValue = new ReturnValue();
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start");
            try
            {
                int Year = 0;
                int Quarter = 0;
                GetQuarterAndYearDetails(ref Quarter, ref  Year);
                DataTable DtLogin = new DataTable();
                SICTLogger.WriteVerbose(CLASS_NAME, FUNCTION_NAME, "Retrieving all Airport login's from DB");
                DtLogin = GetAllAirportLogins(Instance);
                if (DtLogin.Rows.Count > 1)
                {
                    for (int LoginCnt = 0; LoginCnt < DtLogin.Rows.Count; LoginCnt++)
                    {
                        int AirportLoginId = Convert.ToInt32(DtLogin.Rows[LoginCnt][BusinessConstants.AIRPORTLOGINID].ToString());
                        List<TargetsVsCompletes> TargetsandCompletes = new List<TargetsVsCompletes>();
                        List<MissingTargetsVsBusinessClass> MissingTargetsVsBusinessClass = new List<MissingTargetsVsBusinessClass>();
                        DataSet DSTargetCompletes = new DataSet();
                        SICTLogger.WriteVerbose(CLASS_NAME, FUNCTION_NAME, "Retrieving all Targets and Completes for the Airport login -" + AirportLoginId);
                        DSTargetCompletes = GetTargetandCompleteforAirportLogin(Instance, AirportLoginId, Quarter, Year);
                        if (DSTargetCompletes.Tables.Count > 2)
                        {
                            foreach (DataRow DrTarget in DSTargetCompletes.Tables[0].Rows)
                            {
                                //Target Vs Business Class
                                TargetsVsCompletes TempTargetsandCompletes = new TargetsVsCompletes();
                                int AirlineId = Convert.ToInt32(DrTarget[BusinessConstants.AIRLINEID].ToString());
                                int Target = Convert.ToInt32(DrTarget[BusinessConstants.TARGET].ToString());
                                string AirlineName = DrTarget[BusinessConstants.AIRLINENAME].ToString();
                                string Code = DrTarget[BusinessConstants.CODE].ToString();

                                int Completes = 0;
                                DataRow[] DrCompletes = DSTargetCompletes.Tables[1].Select(BusinessConstants.AIRLINEID + "=" + AirlineId);
                                if (DrCompletes.Length > 0)
                                    Completes = Convert.ToInt32(DrCompletes[0][BusinessConstants.COMPLETES].ToString());

                                TempTargetsandCompletes.AirlineId = AirlineId;
                                TempTargetsandCompletes.AirlineName = AirlineName;
                                TempTargetsandCompletes.Code = Code;
                                TempTargetsandCompletes.Target = Target;
                                TempTargetsandCompletes.Completes = Completes;

                                TargetsandCompletes.Add(TempTargetsandCompletes);

                                //Missing Targets Vs Missing Business Class
                                MissingTargetsVsBusinessClass TempMissingTargetsVsBusinessClass = new MissingTargetsVsBusinessClass();
                                int BusinessCompletes = 0;
                                DataRow[] DrBusinessCompletes = DSTargetCompletes.Tables[2].Select(BusinessConstants.AIRLINEID + "=" + AirlineId);
                                if (DrBusinessCompletes.Length > 0)
                                    BusinessCompletes = Convert.ToInt32(DrBusinessCompletes[0][BusinessConstants.BUSINESSCOMPLETES].ToString());

                                TempMissingTargetsVsBusinessClass.AirlineId = AirlineId;
                                TempMissingTargetsVsBusinessClass.AirlineName = DrTarget[BusinessConstants.AIRLINENAME].ToString();
                                TempMissingTargetsVsBusinessClass.Code = DrTarget[BusinessConstants.CODE].ToString();
                                TempMissingTargetsVsBusinessClass.MissingTarget = Target - Completes;
                                double BusinessTarget = (Target * (1 / 3.0));
                                TempMissingTargetsVsBusinessClass.MissingCompletes = Convert.ToInt32(Math.Round(BusinessTarget - BusinessCompletes, 0, MidpointRounding.AwayFromZero));

                                MissingTargetsVsBusinessClass.Add(TempMissingTargetsVsBusinessClass);

                            }
                        }
                        WriteTargetVsCompletesChartsCacheFile(Instance, AirportLoginId.ToString(), TargetsandCompletes);
                        WriteMissingTargetsVsBusinessClassChartCacheFile(Instance, AirportLoginId.ToString(), MissingTargetsVsBusinessClass);
                    }
                }

                //Admin Cache File Creation
                List<TargetsVsCompletes> AllTargetsandCompletes = new List<TargetsVsCompletes>();
                List<MissingTargetsVsBusinessClass> AllMissingTargetsVsBusinessClass = new List<MissingTargetsVsBusinessClass>();
                DataSet DSAll = GetAllTargetandComplete(Instance, Quarter, Year);
                if (DSAll.Tables.Count > 2)
                {
                    foreach (DataRow DrTarget in DSAll.Tables[0].Rows)
                    {
                        //Target Vs Business Class
                        TargetsVsCompletes TempTargetsandCompletes = new TargetsVsCompletes();
                        int AirlineId = Convert.ToInt32(DrTarget[BusinessConstants.AIRLINEID].ToString());
                        int Target = Convert.ToInt32(DrTarget[BusinessConstants.TARGET].ToString());
                        string AirlineName = DrTarget[BusinessConstants.AIRLINENAME].ToString();
                        string Code = DrTarget[BusinessConstants.CODE].ToString();
                        int Completes = 0;
                        DataRow[] DrCompletes = DSAll.Tables[1].Select(BusinessConstants.AIRLINEID + "=" + AirlineId);

                        if (DrCompletes.Length > 0)
                            Completes = Convert.ToInt32(DrCompletes[0][BusinessConstants.COMPLETES].ToString());

                        TempTargetsandCompletes.AirlineId = AirlineId;
                        TempTargetsandCompletes.AirlineName = DrTarget[BusinessConstants.AIRLINENAME].ToString();
                        TempTargetsandCompletes.Code = Code;
                        TempTargetsandCompletes.Target = Convert.ToInt32(DrTarget[BusinessConstants.TARGET].ToString());
                        TempTargetsandCompletes.Completes = Completes;

                        AllTargetsandCompletes.Add(TempTargetsandCompletes);

                        //Missing Targets Vs Missing Business Class
                        MissingTargetsVsBusinessClass TempMissingTargetsVsBusinessClass = new MissingTargetsVsBusinessClass();
                        int BusinessCompletes = 0;
                        DataRow[] DrBusinessCompletes = DSAll.Tables[2].Select(BusinessConstants.AIRLINEID + "=" + AirlineId);
                        if (DrBusinessCompletes.Length > 0)
                            BusinessCompletes = Convert.ToInt32(DrBusinessCompletes[0][BusinessConstants.BUSINESSCOMPLETES].ToString());

                        TempMissingTargetsVsBusinessClass.AirlineId = AirlineId;
                        TempMissingTargetsVsBusinessClass.AirlineName = DrTarget[BusinessConstants.AIRLINENAME].ToString();
                        TempMissingTargetsVsBusinessClass.Code = DrTarget[BusinessConstants.CODE].ToString();
                        TempMissingTargetsVsBusinessClass.MissingTarget = Target - Completes;
                        double BusinessTarget = (Target * (1 / 3.0));
                        TempMissingTargetsVsBusinessClass.MissingCompletes = Convert.ToInt32(Math.Round(BusinessTarget - BusinessCompletes, 0, MidpointRounding.AwayFromZero));

                        AllMissingTargetsVsBusinessClass.Add(TempMissingTargetsVsBusinessClass);
                    }
                }
                WriteTargetVsCompletesChartsCacheFile(Instance, BusinessConstants.ADMIN_CACHEFILE_NAME, AllTargetsandCompletes);
                WriteMissingTargetsVsBusinessClassChartCacheFile(Instance, BusinessConstants.ADMIN_CACHEFILE_NAME, AllMissingTargetsVsBusinessClass);
            }
            catch (Exception Ex)
            {
                RetValue.ReturnCode = -1;
                RetValue.ReturnMessage = "Cache file creation Failed - error in API ";
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, Ex);
            }
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "End");
            return RetValue;
        }

        /// <summary>
        /// This Function is used to write the TargetsVsCompletes Data Passed to corresponding FilePath
        /// </summary>
        /// <param name="LoginId"></param>
        /// <param name="TargetsandCompletes"></param>
        private void WriteTargetVsCompletesChartsCacheFile(string Instance, string LoginId, List<TargetsVsCompletes> TargetsandCompletes)
        {
            const string FUNCTION_NAME = "WriteTargetVsCompletesChartsCacheFile";
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start for LoginId -" + LoginId);
            try
            {
                string FilePath = string.Empty;
                string FolderPath = string.Empty;
                string TargetsandCompletesData = string.Empty;
                GetTargetVsCompletesChartsFilePath(Instance,LoginId, ref FilePath, ref FolderPath);
                Boolean IsFolderExists = System.IO.Directory.Exists(FolderPath);
                if (!IsFolderExists)
                    System.IO.Directory.CreateDirectory(FolderPath);
                SICTLogger.WriteVerbose(CLASS_NAME, FUNCTION_NAME, "TargetVsCompletes Charts cache file for AirportLoginId- " + LoginId);
                using (var MemoryStream = new MemoryStream())
                {
                    var Serlizer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(List<TargetsVsCompletes>));
                    Serlizer.WriteObject(MemoryStream, TargetsandCompletes);
                    TargetsandCompletesData = System.Text.Encoding.UTF8.GetString(MemoryStream.GetBuffer(), 0, Convert.ToInt32(MemoryStream.Length));
                    WriteToFile(TargetsandCompletesData, FilePath);
                }
            }
            catch (Exception Ex)
            {
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, Ex);
            }
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "End for LoginId -" + LoginId);
        }

        /// <summary>
        /// This Function is used to write the Missing Targets Vs Missing Business Class Data Passed to corresponding FilePath
        /// </summary>
        /// <param name="LoginId"></param>
        /// <param name="MissingTargetsVsBusinessClass"></param>
        private void WriteMissingTargetsVsBusinessClassChartCacheFile(string Instance, string LoginId, List<MissingTargetsVsBusinessClass> MissingTargetsVsBusinessClass)
        {
            const string FUNCTION_NAME = "WriteMissingTargetsVsBusinessClassChartCacheFile";
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start for LoginId -" + LoginId);
            try
            {
                string FilePath = string.Empty;
                string FolderPath = string.Empty;
                string MissingTargetsandBusinessClassData = string.Empty;
                GetMissingTargetsVsBusinessClassChartsFilePath(Instance, LoginId, ref FilePath, ref FolderPath);
                Boolean IsFolderExists = System.IO.Directory.Exists(FolderPath);
                if (!IsFolderExists)
                    System.IO.Directory.CreateDirectory(FolderPath);
                SICTLogger.WriteVerbose(CLASS_NAME, FUNCTION_NAME, "MissingTargetsVsBusinessClass Charts cache file for AirportLoginId- " + LoginId);
                using (var MemoryStream = new MemoryStream())
                {
                    var Serlizer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(List<MissingTargetsVsBusinessClass>));
                    Serlizer.WriteObject(MemoryStream, MissingTargetsVsBusinessClass);
                    MissingTargetsandBusinessClassData = System.Text.Encoding.UTF8.GetString(MemoryStream.GetBuffer(), 0, Convert.ToInt32(MemoryStream.Length));
                    WriteToFile(MissingTargetsandBusinessClassData, FilePath);
                }

            }
            catch (Exception Ex)
            {
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, Ex);
            }
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "End for LoginId -" + LoginId);
        }

        /// <summary>
        /// Function to Write a Result to File.
        /// </summary>
        /// <param name="Value">Data to be write</param>
        /// <param name="Path">Path of the Filer</param>
        /// <returns>If successful, Returns true. Otherwise returns false </returns>
        public Boolean WriteToFile(string Value, string Path)
        {
            const string FUNCTION_NAME = "WriteToFile";
            SICTLogger.WriteVerbose(CLASS_NAME, FUNCTION_NAME, "Start");
            string Directorypath = string.Empty;
            Boolean IsSuccess = false;
            try
            {
                File.WriteAllText(Path, Value);
                IsSuccess = true;
            }

            catch (Exception Ex)
            {
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, Ex);
            }
            SICTLogger.WriteVerbose(CLASS_NAME, FUNCTION_NAME, "END");
            return IsSuccess;
        }

        /// <summary>
        /// This Function is Used to Get Quarter and Year For Dashboard Chart Data retrieval
        /// </summary>
        /// <param name="Quarter"></param>
        /// <param name="Year"></param>
        private void GetQuarterAndYearDetails(ref int Quarter, ref int Year)
        {
            const string FUNCTION_NAME = "GetQuarterAndYear";
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start");
            try
            {
                int Month = DateTime.Now.Month;
                Quarter = 0;
                Year = DateTime.Now.Year;
                int MonthsInOneQuarter = 3;

                if (Month % MonthsInOneQuarter > 0)
                {
                    Quarter = Month / MonthsInOneQuarter + 1;
                }
                else
                {
                    Quarter = Month / MonthsInOneQuarter;
                }

                //Condition to Conside new quarter only after 10 days is passed 
                if (Month == 4 || Month == 7 || Month == 10)
                {
                    int Day = DateTime.Now.Day;
                    if (Day <= 10)
                        Quarter -= 1;
                }
                if (Month == 1)
                {
                    int Day = DateTime.Now.Day;
                    if (Day <= 10)
                    {
                        Quarter = 4;
                        Year -= 1;
                    }
                }

            }
            catch (Exception Ex)
            {
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, Ex);
            }
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "End ");
        }

        #endregion DashboardCharts

        #region DB
        /// <summary>
        /// This Function is used to retrieve all the Active AirportLogins from the DB
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllAirportLogins(string Instance)
        {
            DataTable DtResult = new DataTable();
            const string FUNCTION_NAME = "GetAllAirportLogins";
            try
            {
                SqlDatabase DB = GetDataBase(Instance);
                DbCommand ObjCommand = DB.GetStoredProcCommand("GetAllAirportLogins");
                DataSet DS = DB.ExecuteDataSet(ObjCommand);
                if (DS.Tables.Count > 0)
                    DtResult = DS.Tables[0];

            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, ex);
            }
            return DtResult;
        }

        /// <summary>
        /// This Function is used to retrieve all the Airlines Targets and Completes for a Particular Airport Login
        /// </summary>
        /// <param name="AirportId">Id of the Airport</param>
        /// <returns></returns>
        public DataSet GetTargetandCompleteforAirportLogin(string Instance, int AirportLoginId, int Quarter, int Year)
        {
            DataSet DSResult = new DataSet();
            const string FUNCTION_NAME = "GetTargetandCompleteforAirportLogin";
            try
            {
                SqlDatabase DB = GetDataBase(Instance);
                DbCommand ObjCommand = DB.GetStoredProcCommand("GetQuarterlyTargetandCompleteforAirportLogin");
                DB.AddInParameter(ObjCommand, BusinessConstants.AIRPORTLOGINID, DbType.Int32, AirportLoginId);
                DB.AddInParameter(ObjCommand, BusinessConstants.QUARTER, DbType.Int32, Quarter);
                DB.AddInParameter(ObjCommand, BusinessConstants.YEAR, DbType.Int32, Year);
                DSResult = DB.ExecuteDataSet(ObjCommand);
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, ex);
            }
            return DSResult;
        }

        /// <summary>
        /// This Function is used to retieve the target for all the airlines from the DB 
        /// </summary>
        /// <returns></returns>
        public DataSet GetAllTargetandComplete(string Instance, int Quarter, int Year)
        {
            DataSet DSResult = new DataSet();
            const string FUNCTION_NAME = "GetAllTargetandComplete";
            try
            {
                SqlDatabase DB = GetDataBase(Instance);
                DbCommand ObjCommand = DB.GetStoredProcCommand("GetAllTargetandComplete");
                DB.AddInParameter(ObjCommand, BusinessConstants.QUARTER, DbType.Int32, Quarter);
                DB.AddInParameter(ObjCommand, BusinessConstants.YEAR, DbType.Int32, Year);
                DSResult = DB.ExecuteDataSet(ObjCommand);
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, ex);
            }
            return DSResult;
        }


        public SqlDatabase GetDataBase(string Instance)
        {
            SqlDatabase LocalDB = null;
            LocalDB = new SqlDatabase(DBConnection(Instance));
            return LocalDB;

        }

        public string DBConnection(string Instance)
        {
            const string FUNCTION_NAME = "DBConnection";
            string DBCon = string.Empty;
            try
            {
                if (Instance == BusinessConstants.Instance.US.ToString())
                    DBCon = ConfigurationManager.ConnectionStrings[USIDBCONNECTION].ToString();
                else if (Instance == BusinessConstants.Instance.AIR.ToString())
                    DBCon = ConfigurationManager.ConnectionStrings[AIRDBCONNECTION].ToString();
                else if (Instance == BusinessConstants.Instance.EUR.ToString())
                    DBCon = ConfigurationManager.ConnectionStrings[EURDBCONNECTION].ToString();
                else if (Instance == BusinessConstants.Instance.USD.ToString())
                    DBCon = ConfigurationManager.ConnectionStrings[USDDBCONNECTION].ToString();
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, ex);
            }

            return DBCon;
        }

        #endregion DB

        #region Utils

        public void GetTargetVsCompletesChartsFilePath(string Instance,string LoginId, ref string FilePath, ref string FolderPath)
        {
            if (Instance==BusinessConstants.Instance.US.ToString())
            {
                FilePath = string.Format(@"{0}/TargetsVsCompletes_{1}.json",
                        ConfigurationManager.AppSettings[CONFIGURATION_TARGETVSCOMPLETESCHART_FILEPATH_USI].ToString(), LoginId);

                FolderPath = string.Format(@"{0}",
                    ConfigurationManager.AppSettings[CONFIGURATION_TARGETVSCOMPLETESCHART_FILEPATH_USI].ToString());
            }
            else if (Instance == BusinessConstants.Instance.EUR.ToString())
            {
                FilePath = string.Format(@"{0}/TargetsVsCompletes_{1}.json",
                        ConfigurationManager.AppSettings[CONFIGURATION_TARGETVSCOMPLETESCHART_FILEPATH_EUR].ToString(), LoginId);

                FolderPath = string.Format(@"{0}",
                    ConfigurationManager.AppSettings[CONFIGURATION_TARGETVSCOMPLETESCHART_FILEPATH_EUR].ToString());
            }
            else if (Instance == BusinessConstants.Instance.AIR.ToString())
            {
                FilePath = string.Format(@"{0}/TargetsVsCompletes_{1}.json",
                        ConfigurationManager.AppSettings[CONFIGURATION_TARGETVSCOMPLETESCHART_FILEPATH_AIR].ToString(), LoginId);

                FolderPath = string.Format(@"{0}",
                    ConfigurationManager.AppSettings[CONFIGURATION_TARGETVSCOMPLETESCHART_FILEPATH_AIR].ToString());
            }
            else if (Instance == BusinessConstants.Instance.USD.ToString())
            {
                FilePath = string.Format(@"{0}/TargetsVsCompletes_{1}.json",
                        ConfigurationManager.AppSettings[CONFIGURATION_TARGETVSCOMPLETESCHART_FILEPATH_USD].ToString(), LoginId);

                FolderPath = string.Format(@"{0}",
                    ConfigurationManager.AppSettings[CONFIGURATION_TARGETVSCOMPLETESCHART_FILEPATH_USD].ToString());
            }
        }

        public void GetMissingTargetsVsBusinessClassChartsFilePath(string Instance, string LoginId, ref string FilePath, ref string FolderPath)
        {
            if (Instance == BusinessConstants.Instance.US.ToString())
            {
                FilePath = string.Format(@"{0}/MissingTargetsVsBusinessClass_{1}.json",
                        ConfigurationManager.AppSettings[CONFIGURATION_TARGETVSCOMPLETESCHART_FILEPATH_USI].ToString(), LoginId);

                FolderPath = string.Format(@"{0}",
                    ConfigurationManager.AppSettings[CONFIGURATION_TARGETVSCOMPLETESCHART_FILEPATH_USI].ToString());
            }
            else if (Instance == BusinessConstants.Instance.EUR.ToString())
            {
                FilePath = string.Format(@"{0}/MissingTargetsVsBusinessClass_{1}.json",
                      ConfigurationManager.AppSettings[CONFIGURATION_TARGETVSCOMPLETESCHART_FILEPATH_EUR].ToString(), LoginId);

                FolderPath = string.Format(@"{0}",
                    ConfigurationManager.AppSettings[CONFIGURATION_TARGETVSCOMPLETESCHART_FILEPATH_EUR].ToString());
            }
            else if (Instance == BusinessConstants.Instance.AIR.ToString())
            {
                FilePath = string.Format(@"{0}/MissingTargetsVsBusinessClass_{1}.json",
                      ConfigurationManager.AppSettings[CONFIGURATION_TARGETVSCOMPLETESCHART_FILEPATH_AIR].ToString(), LoginId);

                FolderPath = string.Format(@"{0}",
                    ConfigurationManager.AppSettings[CONFIGURATION_TARGETVSCOMPLETESCHART_FILEPATH_AIR].ToString());
            }
            else if (Instance == BusinessConstants.Instance.USD.ToString())
            {
                FilePath = string.Format(@"{0}/MissingTargetsVsBusinessClass_{1}.json",
                      ConfigurationManager.AppSettings[CONFIGURATION_TARGETVSCOMPLETESCHART_FILEPATH_USD].ToString(), LoginId);

                FolderPath = string.Format(@"{0}",
                    ConfigurationManager.AppSettings[CONFIGURATION_TARGETVSCOMPLETESCHART_FILEPATH_USD].ToString());
            }

        }

        #endregion Utils


    }
}
