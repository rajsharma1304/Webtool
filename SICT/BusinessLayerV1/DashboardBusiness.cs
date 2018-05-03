///<Copyright> Celstream Technologies Pvt. Ltd. </Copyright>
///<ProjectName>SICT </ProjectName>
///<FileName>DashboardBusiness.cs</FileName>
///<Author>David Boon</Author>
///<CreatedOn> 11 Feb 2015</CreatedOn>

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
using System.Reflection;
using System.ComponentModel;
using SICT.BusinessUtils;
using System.IO;
using System.Runtime.Serialization.Json;

namespace SICT.BusinessLayer.V1
{

    /// <summary>
    /// This class Implements functions related to Dashboards 
    /// </summary>
    public class DashboardBusiness
    {
        private static readonly string CLASS_NAME = "DashboardBusiness";

        #region DashboardCharts
        /// <summary>
        /// This Function is used to create TargetVsCompletes Charts for Each active Airport Login and Store it in the Aproapriate path
        /// </summary>
        /// <returns></returns>
        public ReturnValue CreateCacheFileforTargetVsCompletesCharts()
        {
            const string FUNCTION_NAME = "CreateCacheFileforTargetVsCompletesCharts";
            ReturnValue RetValue = new ReturnValue();
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start");
            try
            {
                DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();               
                DataTable DtLogin = new DataTable();
                SICTLogger.WriteVerbose(CLASS_NAME, FUNCTION_NAME, "Retrieving all Airport login's from DB");
                DtLogin = DBLayer.GetAllAirportLogins();
                if (DtLogin.Rows.Count > 1)
                {
                    for (int LoginCnt = 0; LoginCnt < DtLogin.Rows.Count; LoginCnt++)
                    {
                        int AirportLoginId = Convert.ToInt32(DtLogin.Rows[LoginCnt][BusinessConstants.AIRPORTLOGINID].ToString());
                        List<TargetsVsCompletes> TargetsandCompletes = new List<TargetsVsCompletes>();
                        List<MissingTargetsVsBusinessClass> MissingTargetsVsBusinessClass = new List<MissingTargetsVsBusinessClass>();
                        DataSet DSTargetCompletes = new DataSet();
                        SICTLogger.WriteVerbose(CLASS_NAME, FUNCTION_NAME, "Retrieving all Targets and Completes for the Airport login -" + AirportLoginId);
                        DSTargetCompletes = DBLayer.GetTargetandCompleteforAirportLogin(AirportLoginId);
                        if (DSTargetCompletes.Tables.Count > 2)
                        {
                            foreach (DataRow DrTarget in DSTargetCompletes.Tables[0].Rows)
                            {
                                //Target Vs Business Class
                                TargetsVsCompletes TempTargetsandCompletes = new TargetsVsCompletes();
                                int AirlineId = Convert.ToInt32(DrTarget[BusinessConstants.AIRLINEID].ToString());
                                int Target = Convert.ToInt32(DrTarget[BusinessConstants.TARGET].ToString());
                                string AirlineName = DrTarget[BusinessConstants.AIRLINENAME].ToString();

                                int Completes = 0;
                                DataRow[] DrCompletes = DSTargetCompletes.Tables[1].Select(BusinessConstants.AIRLINEID + "=" + AirlineId);
                                if (DrCompletes.Length > 0)
                                    Completes = Convert.ToInt32(DrCompletes[0][BusinessConstants.COMPLETES].ToString());

                                TempTargetsandCompletes.AirlineId = AirlineId;
                                TempTargetsandCompletes.AirlineName = AirlineName;
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
                                TempMissingTargetsVsBusinessClass.MissingTarget = Target - Completes;
                                TempMissingTargetsVsBusinessClass.MissingCompletes = (Target * (1/3)) - Convert.ToDouble(BusinessCompletes);

                                MissingTargetsVsBusinessClass.Add(TempMissingTargetsVsBusinessClass);

                            }
                        }
                        WriteTargetVsCompletesChartsCacheFile(AirportLoginId.ToString(), TargetsandCompletes);
                        WriteMissingTargetsVsBusinessClassChartCacheFile(AirportLoginId.ToString(), MissingTargetsVsBusinessClass);
                    }
                }

                //Admin Cache File Creation
                List<TargetsVsCompletes> AllTargetsandCompletes = new List<TargetsVsCompletes>();
                List<MissingTargetsVsBusinessClass> AllMissingTargetsVsBusinessClass = new List<MissingTargetsVsBusinessClass>();
                DataSet DSAll = DBLayer.GetAllTargetandComplete();
                if (DSAll.Tables.Count > 2)
                {
                    foreach (DataRow DrTarget in DSAll.Tables[0].Rows)
                    {
                        //Target Vs Business Class
                        TargetsVsCompletes TempTargetsandCompletes = new TargetsVsCompletes();
                        int AirlineId = Convert.ToInt32(DrTarget[BusinessConstants.AIRLINEID].ToString());
                        int Target = Convert.ToInt32(DrTarget[BusinessConstants.TARGET].ToString());
                        string AirlineName = DrTarget[BusinessConstants.AIRLINENAME].ToString(); ;
                        int Completes = 0;
                        DataRow[] DrCompletes = DSAll.Tables[1].Select(BusinessConstants.AIRLINEID + "=" + AirlineId);

                        if (DrCompletes.Length > 0)
                            Completes = Convert.ToInt32(DrCompletes[0][BusinessConstants.COMPLETES].ToString());

                        TempTargetsandCompletes.AirlineId = AirlineId;
                        TempTargetsandCompletes.AirlineName = DrTarget[BusinessConstants.AIRLINENAME].ToString();
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
                        TempMissingTargetsVsBusinessClass.MissingTarget = Target - Completes;
                        TempMissingTargetsVsBusinessClass.MissingCompletes = (Target * 0.30) - Convert.ToDouble(BusinessCompletes);

                        AllMissingTargetsVsBusinessClass.Add(TempMissingTargetsVsBusinessClass);
                    }
                }
                WriteTargetVsCompletesChartsCacheFile("Admin", AllTargetsandCompletes);
                WriteMissingTargetsVsBusinessClassChartCacheFile("Admin", AllMissingTargetsVsBusinessClass);
            }
            catch (Exception Ex)
            {
                RetValue.ReturnCode = 2;
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
        private void WriteTargetVsCompletesChartsCacheFile(string LoginId, List<TargetsVsCompletes> TargetsandCompletes)
        {
            const string FUNCTION_NAME = "WriteTargetVsCompletesChartsCacheFile";
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start for LoginId -" + LoginId);
            try
            {
                DepartureFormBusiness ObjDepartureFormBusiness = new DepartureFormBusiness();
                BusinessUtil ObjBusinessUtil = new BusinessUtil();
                string FilePath = string.Empty;
                string FolderPath = string.Empty;
                string TargetsandCompletesData = string.Empty;
                ObjBusinessUtil.GetTargetVsCompletesChartsFilePath(LoginId, ref FilePath, ref FolderPath);
                Boolean IsFolderExists = System.IO.Directory.Exists(FolderPath);
                if (!IsFolderExists)
                    System.IO.Directory.CreateDirectory(FolderPath);
                SICTLogger.WriteVerbose(CLASS_NAME, FUNCTION_NAME, "TargetVsCompletes Charts cache file for AirportLoginId- " + LoginId);
                using (var MemoryStream = new MemoryStream())
                {
                    var Serlizer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(List<TargetsVsCompletes>));
                    Serlizer.WriteObject(MemoryStream, TargetsandCompletes);
                    TargetsandCompletesData = System.Text.Encoding.UTF8.GetString(MemoryStream.GetBuffer(), 0, Convert.ToInt32(MemoryStream.Length));
                    ObjDepartureFormBusiness.WriteToFile(TargetsandCompletesData, FilePath);
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
        private void WriteMissingTargetsVsBusinessClassChartCacheFile(string LoginId, List<MissingTargetsVsBusinessClass> MissingTargetsVsBusinessClass)
        {
            const string FUNCTION_NAME = "WriteMissingTargetsVsBusinessClassChartCacheFile";
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start for LoginId -" + LoginId);
            try
            {
                DepartureFormBusiness ObjDepartureFormBusiness = new DepartureFormBusiness();
                BusinessUtil ObjBusinessUtil = new BusinessUtil();
                string FilePath = string.Empty;
                string FolderPath = string.Empty;
                string MissingTargetsandBusinessClassData = string.Empty;
                ObjBusinessUtil.GetMissingTargetsVsBusinessClassChartsFilePath(LoginId, ref FilePath, ref FolderPath);
                Boolean IsFolderExists = System.IO.Directory.Exists(FolderPath);
                if (!IsFolderExists)
                    System.IO.Directory.CreateDirectory(FolderPath);
                SICTLogger.WriteVerbose(CLASS_NAME, FUNCTION_NAME, "MissingTargetsVsBusinessClass Charts cache file for AirportLoginId- " + LoginId);
                using (var MemoryStream = new MemoryStream())
                {
                    var Serlizer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(List<MissingTargetsVsBusinessClass>));
                    Serlizer.WriteObject(MemoryStream, MissingTargetsVsBusinessClass);
                    MissingTargetsandBusinessClassData = System.Text.Encoding.UTF8.GetString(MemoryStream.GetBuffer(), 0, Convert.ToInt32(MemoryStream.Length));
                    ObjDepartureFormBusiness.WriteToFile(MissingTargetsandBusinessClassData, FilePath);
                }

            }
            catch (Exception Ex)
            {
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, Ex);
            }
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "End for LoginId -" + LoginId);
        }
        #endregion DashboardCharts

    }
}
