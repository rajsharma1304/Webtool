using SICT.BusinessUtils;
using SICT.Constants;
using SICT.DataAccessLayer;
using SICT.DataContracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace SICT.BusinessLayer.V1
{
    public class CacheFileBusiness
    {
        private static readonly string CLASS_NAME = "CacheFileBusiness";

        public ReturnValue CreateAllCacheFiles(string Instance)
        {
            this.CreateCacheFileforAiprortAndAirline(Instance, true);
            ReturnValue RetValue = new ReturnValue();
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "CreateAllCacheFiles", "Start");
            try
            {
                this.CreateCacheFileforInterviewers();
                this.CreateCacheFileforAiprort();
                this.CreateCacheFileforAiprortAndAirline(Instance, true);
                if (Instance == BusinessConstants.Instance.US.ToString())
                {
                    this.CreateCacheFileforAiprortAndAirline(Instance, false);
                    this.CreateCacheFileforRouteAndDirection();
                }
                if (Instance == BusinessConstants.Instance.EUR.ToString())
                {
                    this.CreateCacheFileforFlightTypes();
                }
                if (Instance == BusinessConstants.Instance.AIR.ToString())
                {
                    this.CreateCacheFileforAiprortAndAirline(Instance, false);
                    this.CreateCacheFileforAircraftTypes();
                }
                this.CreateCacheFileforLanguage();
                this.CreateCacheFileforTargetVsCompletesCharts(Instance);
                this.CreateCacheFileforAirportReportAdminLogin();
                RetValue.ReturnCode = 1;
                RetValue.ReturnMessage = "Cache file Successfull ";
            }
            catch (System.Exception Ex)
            {
                RetValue.ReturnCode = -1;
                RetValue.ReturnMessage = "Cache file creation Failed - error in API ";
                SICTLogger.WriteException(CacheFileBusiness.CLASS_NAME, "CreateAllCacheFiles", Ex);
            }
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "CreateAllCacheFiles", "End");
            return RetValue;
        }

        public ReturnValue CreateCacheFileforInterviewers()
        {
            ReturnValue RetValue = new ReturnValue();
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforInterviewers", "Start");
            try
            {
                DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
                DataSet DS = new DataSet();
                SICTLogger.WriteVerbose(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforInterviewers", "Retrieving all Airport login's and corresponding interviewer list from DB");
                DS = DBLayer.GetAirportLoginandInterviewerList();
                if (DS.Tables.Count > 1)
                {
                    if (DS.Tables[0].Rows.Count > 0)
                    {
                        for (int LoginCnt = 0; LoginCnt < DS.Tables[0].Rows.Count; LoginCnt++)
                        {
                            int LoginId = System.Convert.ToInt32(DS.Tables[0].Rows[LoginCnt][BusinessConstants.AIRPORTLOGINID].ToString());
                            System.Collections.Generic.List<InterviewerDetail> InterviewerDetails = new System.Collections.Generic.List<InterviewerDetail>();
                            DataRow[] Interviwers = DS.Tables[1].Select(BusinessConstants.AIRPORTLOGINID + " = " + LoginId);
                            if (Interviwers.Length > 0)
                            {
                                DataRow[] array = Interviwers;
                                for (int i = 0; i < array.Length; i++)
                                {
                                    DataRow Dr = array[i];
                                    InterviewerDetails.Add(new InterviewerDetail
                                    {
                                        InterviewerId = System.Convert.ToInt32(Dr[BusinessConstants.INTERVIEWERID].ToString()),
                                        InterviewerName = Dr[BusinessConstants.INTERVIEWERNAME].ToString()
                                    });
                                }
                            }
                            this.WriteInterviewerDetailstoCacheFile(LoginId.ToString(), InterviewerDetails);
                        }
                        //System.Collections.Generic.List<InterviewerDetail> AllInterviewerDetails = new System.Collections.Generic.List<InterviewerDetail>();
                        //foreach (DataRow Dr in DS.Tables[1].Rows)
                        //{
                        //    DataRow Dr;
                        //    AllInterviewerDetails.Add(new InterviewerDetail
                        //    {
                        //        InterviewerId = System.Convert.ToInt32(Dr[BusinessConstants.INTERVIEWERID].ToString()),
                        //        InterviewerName = Dr[BusinessConstants.INTERVIEWERNAME].ToString()
                        //    });
                        //}
                        //this.WriteInterviewerDetailstoCacheFile(BusinessConstants.ADMIN_CACHEFILE_NAME, AllInterviewerDetails);
                        System.Collections.Generic.List<InterviewerDetail> list2 = new System.Collections.Generic.List<InterviewerDetail>();
                        foreach (DataRow dataRow in DS.Tables[1].Rows)
                        {
                            list2.Add(new InterviewerDetail
                            {
                                InterviewerId = System.Convert.ToInt32(dataRow[BusinessConstants.INTERVIEWERID].ToString()),
                                InterviewerName = dataRow[BusinessConstants.INTERVIEWERNAME].ToString()
                            });
                        }
                        this.WriteInterviewerDetailstoCacheFile(BusinessConstants.ADMIN_CACHEFILE_NAME, list2);
                    }
                }
            }
            catch (System.Exception Ex)
            {
                RetValue.ReturnCode = -1;
                RetValue.ReturnMessage = "Cache file creation Failed - error in API ";
                SICTLogger.WriteException(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforInterviewers", Ex);
            }
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforInterviewers", "End");
            return RetValue;
        }

        private void WriteInterviewerDetailstoCacheFile(string LoginId, System.Collections.Generic.List<InterviewerDetail> InterviewerDetails)
        {
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "WriteInterviewerDetailstoCacheFile", "Start for LoginId -" + LoginId);
            try
            {
                BusinessUtil ObjBusinessUtil = new BusinessUtil();
                string FilePath = string.Empty;
                string FolderPath = string.Empty;
                string InterviewerData = string.Empty;
                ObjBusinessUtil.GetInterviewerFilePath(LoginId, ref FilePath, ref FolderPath);
                if (!System.IO.Directory.Exists(FolderPath))
                {
                    System.IO.Directory.CreateDirectory(FolderPath);
                }
                SICTLogger.WriteVerbose(CacheFileBusiness.CLASS_NAME, "WriteInterviewerDetailstoCacheFile", "Writing to Interviewerlist cache file for AirportLoginId- " + LoginId);
                using (System.IO.MemoryStream MemoryStream = new System.IO.MemoryStream())
                {
                    DataContractJsonSerializer Serlizer = new DataContractJsonSerializer(typeof(System.Collections.Generic.List<InterviewerDetail>));
                    Serlizer.WriteObject(MemoryStream, InterviewerDetails);
                    InterviewerData = System.Text.Encoding.UTF8.GetString(MemoryStream.GetBuffer(), 0, System.Convert.ToInt32(MemoryStream.Length));
                    this.WriteToFile(InterviewerData, FilePath);
                }
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(CacheFileBusiness.CLASS_NAME, "WriteInterviewerDetailstoCacheFile", Ex);
            }
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "WriteInterviewerDetailstoCacheFile", "End for LoginId -" + LoginId);
        }

        public bool WriteToFile(string Value, string Path)
        {
            SICTLogger.WriteVerbose(CacheFileBusiness.CLASS_NAME, "WriteToFile", "Start");
            string Directorypath = string.Empty;
            bool IsSuccess = false;
            try
            {
                System.IO.File.WriteAllText(Path, Value);
                IsSuccess = true;
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(CacheFileBusiness.CLASS_NAME, "WriteToFile", Ex);
            }
            SICTLogger.WriteVerbose(CacheFileBusiness.CLASS_NAME, "WriteToFile", "END");
            return IsSuccess;
        }

        public ReturnValue CreateCacheFileforLanguage()
        {
            ReturnValue RetValue = new ReturnValue();
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforLanguage", "Start");
            try
            {
                DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
                DataTable DtLag = new DataTable();
                SICTLogger.WriteVerbose(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforLanguage", "Retrieving all Languages from DB");
                DtLag = DBLayer.GetAllLanguages();
                System.Collections.Generic.List<LanguageDetail> Languages = new System.Collections.Generic.List<LanguageDetail>();
                if (DtLag.Rows.Count > 0)
                {
                    foreach (DataRow Dr in DtLag.Rows)
                    {
                        Languages.Add(new LanguageDetail
                        {
                            LanguageId = System.Convert.ToInt32(Dr[BusinessConstants.LANGUAGEID].ToString()),
                            LanguageName = Dr[BusinessConstants.LANGUAGENAME].ToString()
                        });
                    }
                }
                this.WriteLanguagetoCacheFile(Languages);
            }
            catch (System.Exception Ex)
            {
                RetValue.ReturnCode = -1;
                RetValue.ReturnMessage = "Cache file creation Failed - error in API ";
                SICTLogger.WriteException(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforLanguage", Ex);
            }
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforLanguage", "End");
            return RetValue;
        }

        private void WriteLanguagetoCacheFile(System.Collections.Generic.List<LanguageDetail> Languages)
        {
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "WriteLanguagetoCacheFile", "Start");
            try
            {
                BusinessUtil ObjBusinessUtil = new BusinessUtil();
                string FilePath = string.Empty;
                string FolderPath = string.Empty;
                string LanguageData = string.Empty;
                ObjBusinessUtil.GetLanguageCachetFilePath(ref FilePath, ref FolderPath);
                if (!System.IO.Directory.Exists(FolderPath))
                {
                    System.IO.Directory.CreateDirectory(FolderPath);
                }
                SICTLogger.WriteVerbose(CacheFileBusiness.CLASS_NAME, "WriteLanguagetoCacheFile", "Writing to Language list cache file");
                using (System.IO.MemoryStream MemoryStream = new System.IO.MemoryStream())
                {
                    DataContractJsonSerializer Serlizer = new DataContractJsonSerializer(typeof(System.Collections.Generic.List<LanguageDetail>));
                    Serlizer.WriteObject(MemoryStream, Languages);
                    LanguageData = System.Text.Encoding.UTF8.GetString(MemoryStream.GetBuffer(), 0, System.Convert.ToInt32(MemoryStream.Length));
                    this.WriteToFile(LanguageData, FilePath);
                }
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(CacheFileBusiness.CLASS_NAME, "WriteLanguagetoCacheFile", Ex);
            }
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "WriteLanguagetoCacheFile", "End");
        }

        public ReturnValue CreateCacheFileforAiprort()
        {
            ReturnValue RetValue = new ReturnValue();
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforAiprort", "Start");
            try
            {
                DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
                DataSet DS = new DataSet();
                SICTLogger.WriteVerbose(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforAiprort", "Retrieving all Airport login's and corresponding Airport list from DB");
                DS = DBLayer.GetAirportLoginandAirportList();
                if (DS.Tables.Count > 1)
                {
                    if (DS.Tables[0].Rows.Count > 0)
                    {
                        System.Collections.Generic.List<AirportIdVstLoginId> AirportIdVstLoginId = new System.Collections.Generic.List<AirportIdVstLoginId>();
                        for (int LoginCnt = 0; LoginCnt < DS.Tables[0].Rows.Count; LoginCnt++)
                        {
                            int LoginId = System.Convert.ToInt32(DS.Tables[0].Rows[LoginCnt][BusinessConstants.AIRPORTLOGINID].ToString());
                            string UserName = DS.Tables[0].Rows[LoginCnt][BusinessConstants.USERNAME].ToString();
                            System.Collections.Generic.List<AirportDetail> AirportDetails = new System.Collections.Generic.List<AirportDetail>();
                            DataRow[] Airports = DS.Tables[1].Select("Code = '" + UserName + "'");
                            if (Airports.Length > 0)
                            {
                                AirportIdVstLoginId.Add(new AirportIdVstLoginId
                                {
                                    AId = System.Convert.ToInt32(Airports[0][BusinessConstants.AIRPORTID].ToString()),
                                    AName = Airports[0][BusinessConstants.AIRPORTNAME].ToString(),
                                    Code = Airports[0][BusinessConstants.CODE].ToString(),
                                    LId = LoginId
                                });
                                DataRow[] array = Airports;
                                for (int i = 0; i < array.Length; i++)
                                {
                                    DataRow Dr = array[i];
                                    AirportDetails.Add(new AirportDetail
                                    {
                                        AirportId = System.Convert.ToInt32(Dr[BusinessConstants.AIRPORTID].ToString()),
                                        AirportName = Dr[BusinessConstants.AIRPORTNAME].ToString(),
                                        Code = Dr[BusinessConstants.CODE].ToString()
                                    });
                                }
                            }
                            this.WriteAirportDetailstoCacheFile(LoginId.ToString(), AirportDetails);
                        }
                        this.WriteAirportIdVsLoginCacheFile(AirportIdVstLoginId);
                        System.Collections.Generic.List<AirportDetail> list3 = new System.Collections.Generic.List<AirportDetail>();
                        foreach (DataRow dataRow in DS.Tables[1].Rows)
                        {
                            list3.Add(new AirportDetail
                            {
                                AirportId = System.Convert.ToInt32(dataRow[BusinessConstants.AIRPORTID].ToString()),
                                AirportName = dataRow[BusinessConstants.AIRPORTNAME].ToString(),
                                Code = dataRow[BusinessConstants.CODE].ToString()
                            });
                        }
                        this.WriteAirportDetailstoCacheFile(BusinessConstants.ADMIN_CACHEFILE_NAME, list3);
                    }
                }
                DS = DBLayer.GetAdminAirportList();
                if (DS.Tables.Count > 1)
                {
                    if (DS.Tables[0].Rows.Count > 0)
                    {
                        for (int AdminLoginCnt = 0; AdminLoginCnt < DS.Tables[0].Rows.Count; AdminLoginCnt++)
                        {
                            int AdmninLoginId = System.Convert.ToInt32(DS.Tables[0].Rows[AdminLoginCnt][BusinessConstants.ADMINLOGINID].ToString());
                            System.Collections.Generic.List<AirportDetail> AirportDetails = new System.Collections.Generic.List<AirportDetail>();
                            DataRow[] Airports = DS.Tables[1].Select(BusinessConstants.ADMINLOGINID + " = " + AdmninLoginId);
                            if (Airports.Length > 0)
                            {
                                DataRow[] array = Airports;
                                for (int i = 0; i < array.Length; i++)
                                {
                                    DataRow Dr = array[i];
                                    AirportDetails.Add(new AirportDetail
                                    {
                                        AirportId = System.Convert.ToInt32(Dr[BusinessConstants.AIRPORTID].ToString()),
                                        AirportName = Dr[BusinessConstants.AIRPORTNAME].ToString(),
                                        Code = Dr[BusinessConstants.CODE].ToString()
                                    });
                                }
                            }
                            this.WriteAirportDetailstoCacheFile(AdmninLoginId.ToString(), AirportDetails);
                        }
                    }
                }
            }
            catch (System.Exception Ex)
            {
                RetValue.ReturnCode = -1;
                RetValue.ReturnMessage = "Cache file creation Failed - error in API ";
                SICTLogger.WriteException(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforAiprort", Ex);
            }
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforAiprort", "End");
            return RetValue;
        }

        private void WriteAirportIdVsLoginCacheFile(System.Collections.Generic.List<AirportIdVstLoginId> AirportIdVstLoginId)
        {
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "WriteAirportIdVsLoginCacheFile", "Start ");
            try
            {
                BusinessUtil ObjBusinessUtil = new BusinessUtil();
                string FilePath = string.Empty;
                string FolderPath = string.Empty;
                string Data = string.Empty;
                ObjBusinessUtil.AirportIdVstLoginId(ref FilePath, ref FolderPath);
                if (!System.IO.Directory.Exists(FolderPath))
                {
                    System.IO.Directory.CreateDirectory(FolderPath);
                }
                SICTLogger.WriteVerbose(CacheFileBusiness.CLASS_NAME, "WriteAirportIdVsLoginCacheFile", "Writing to AirportIdVstLoginId list cache file");
                using (System.IO.MemoryStream MemoryStream = new System.IO.MemoryStream())
                {
                    DataContractJsonSerializer Serlizer = new DataContractJsonSerializer(typeof(System.Collections.Generic.List<AirportIdVstLoginId>));
                    Serlizer.WriteObject(MemoryStream, AirportIdVstLoginId);
                    Data = System.Text.Encoding.UTF8.GetString(MemoryStream.GetBuffer(), 0, System.Convert.ToInt32(MemoryStream.Length));
                    this.WriteToFile(Data, FilePath);
                }
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(CacheFileBusiness.CLASS_NAME, "WriteAirportIdVsLoginCacheFile", Ex);
            }
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "WriteAirportIdVsLoginCacheFile", "End ");
        }

        private void WriteAirportDetailstoCacheFile(string LoginId, System.Collections.Generic.List<AirportDetail> AirportrDetails)
        {
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "WriteAirportDetailstoCacheFile", "Start for LoginId -" + LoginId);
            try
            {
                BusinessUtil ObjBusinessUtil = new BusinessUtil();
                string FilePath = string.Empty;
                string FolderPath = string.Empty;
                string AirportData = string.Empty;
                ObjBusinessUtil.GetAirportFilePath(LoginId, ref FilePath, ref FolderPath);
                if (!System.IO.Directory.Exists(FolderPath))
                {
                    System.IO.Directory.CreateDirectory(FolderPath);
                }
                SICTLogger.WriteVerbose(CacheFileBusiness.CLASS_NAME, "WriteAirportDetailstoCacheFile", "Writing to Airport list cache file for AirportLoginId- " + LoginId);
                using (System.IO.MemoryStream MemoryStream = new System.IO.MemoryStream())
                {
                    DataContractJsonSerializer Serlizer = new DataContractJsonSerializer(typeof(System.Collections.Generic.List<AirportDetail>));
                    Serlizer.WriteObject(MemoryStream, AirportrDetails);
                    AirportData = System.Text.Encoding.UTF8.GetString(MemoryStream.GetBuffer(), 0, System.Convert.ToInt32(MemoryStream.Length));
                    this.WriteToFile(AirportData, FilePath);
                }
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(CacheFileBusiness.CLASS_NAME, "WriteAirportDetailstoCacheFile", Ex);
            }
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "WriteAirportDetailstoCacheFile", "End for LoginId -" + LoginId);
        }

        public ReturnValue CreateCacheFileforAiprortAndAirline(string Instance, bool IsDepartureForm)
        {
            ReturnValue RetValue = new ReturnValue();
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforAiprortAndAirline", "Start");
            try
            {
                DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
                DataSet DS = new DataSet();
                string Type = string.Empty;
                string OriginORDestination = string.Empty;
                if (IsDepartureForm)
                {
                    Type = BusinessConstants.FORM_TYPE_DEPARTURE;
                    OriginORDestination = BusinessConstants.ORIGINID;
                }
                else
                {
                    Type = BusinessConstants.FORM_TYPE_ARRIVAL;
                    OriginORDestination = BusinessConstants.DESTINATIONID;
                }
                SICTLogger.WriteVerbose(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforAiprortAndAirline", "Retrieving all Airport's and corresponding Airline list from DB");
                DS = DBLayer.GetAirportandAirlinetList();
                if (DS.Tables.Count > 1)
                {
                    if (DS.Tables[0].Rows.Count > 0)
                    {
                        for (int AirportCnt = 0; AirportCnt < DS.Tables[0].Rows.Count; AirportCnt++)
                        {
                            int AirportId = System.Convert.ToInt32(DS.Tables[0].Rows[AirportCnt][BusinessConstants.AIRPORTID].ToString());
                            string Code = System.Convert.ToString(DS.Tables[0].Rows[AirportCnt]["Code"].ToString());
                            UserDetailsBusiness udb = new UserDetailsBusiness();
                            System.Collections.Generic.List<AirportAirlineDetail> AirportAirlineDetail = new System.Collections.Generic.List<AirportAirlineDetail>();
                            DataRow[] AirLines = DS.Tables[1].Select();
                            bool ISSpecialUser = udb.CheckIsSpecialUserOrNot(Code);
                            if (ISSpecialUser)
                            {
                                int AirportLoginId = System.Convert.ToInt32(DS.Tables[0].Rows[AirportCnt]["AirportLoginId"].ToString());
                                DataSet AirlineDS = DBLayer.GetAirlineDetailsForSpecialUser(AirportId, AirportLoginId);
                                AirLines = AirlineDS.Tables[0].Select("Type='" + Type + "'");
                            }
                            else if (Instance == BusinessConstants.Instance.US.ToString() || Instance == BusinessConstants.Instance.AIR.ToString())
                            {
                                if (ISSpecialUser)
                                {
                                    AirLines = DS.Tables[1].Select(string.Concat(new object[]
                                    {
                                        OriginORDestination,
                                        " = ",
                                        AirportId,
                                        " and Type='",
                                        Type,
                                        "'"
                                    }));
                                }
                                else
                                {
                                    AirLines = DS.Tables[1].Select(string.Concat(new object[]
                                    {
                                        OriginORDestination,
                                        " = ",
                                        AirportId,
                                        " and Type='",
                                        Type,
                                        "' and AirlineCode<>'VN'"
                                    }));
                                }
                            }
                            else
                            {
                                AirLines = DS.Tables[1].Select(OriginORDestination + " = " + AirportId);
                            }
                            if (AirLines.Length > 0)
                            {
                                DataRow[] array = AirLines;
                                for (int i = 0; i < array.Length; i++)
                                {
                                    DataRow Dr = array[i];
                                    AirportAirlineDetail TempAirportAirlineDetail = new AirportAirlineDetail();
                                    TempAirportAirlineDetail.FlightId = System.Convert.ToInt32(Dr[BusinessConstants.FLIGHTID].ToString());
                                    TempAirportAirlineDetail.AirlineId = System.Convert.ToInt32(Dr[BusinessConstants.AIRLINEID].ToString());
                                    TempAirportAirlineDetail.AirlineName = Dr[BusinessConstants.AIRLINENAME].ToString();
                                    TempAirportAirlineDetail.AirlineCode = Dr[BusinessConstants.AIRLINECODE].ToString();
                                    TempAirportAirlineDetail.OriginId = System.Convert.ToInt32(Dr[BusinessConstants.ORIGINID].ToString());
                                    TempAirportAirlineDetail.OriginName = Dr[BusinessConstants.ORIGINNAME].ToString();
                                    TempAirportAirlineDetail.DestinationId = System.Convert.ToInt32(Dr[BusinessConstants.DESTINATIONID].ToString());
                                    TempAirportAirlineDetail.DestinationName = Dr[BusinessConstants.DESTINATIONNAME].ToString();
                                    if (Instance == BusinessConstants.Instance.US.ToString())
                                    {
                                        TempAirportAirlineDetail.Route = Dr[BusinessConstants.ROUTE].ToString();
                                        TempAirportAirlineDetail.Direction = Dr[BusinessConstants.DIRECTION].ToString();
                                        TempAirportAirlineDetail.Type = Dr[BusinessConstants.TYPE].ToString();
                                    }
                                    else if (Instance == BusinessConstants.Instance.EUR.ToString())
                                    {
                                        TempAirportAirlineDetail.FlightType = Dr[BusinessConstants.FLIGHTTYPE].ToString();
                                    }
                                    else if (Instance == BusinessConstants.Instance.AIR.ToString())
                                    {
                                        TempAirportAirlineDetail.Type = Dr[BusinessConstants.TYPE].ToString();
                                    }
                                    AirportAirlineDetail.Add(TempAirportAirlineDetail);
                                }
                            }
                            this.WriteAirportandAirlinetoCacheFile(AirportId.ToString(), AirportAirlineDetail, IsDepartureForm);
                        }
                        System.Collections.Generic.List<AirportAirlineDetail> AllAirportAirlineDetail = new System.Collections.Generic.List<AirportAirlineDetail>();
                        foreach (DataRow Dr in DS.Tables[1].Rows)
                        {
                            AirportAirlineDetail TempAirportAirlineDetail = new AirportAirlineDetail();
                           // DataRow Dr;
                            TempAirportAirlineDetail.FlightId = System.Convert.ToInt32(Dr[BusinessConstants.FLIGHTID].ToString());
                            TempAirportAirlineDetail.AirlineId = System.Convert.ToInt32(Dr[BusinessConstants.AIRLINEID].ToString());
                            TempAirportAirlineDetail.AirlineName = Dr[BusinessConstants.AIRLINENAME].ToString();
                            TempAirportAirlineDetail.AirlineCode = Dr[BusinessConstants.AIRLINECODE].ToString();
                            TempAirportAirlineDetail.OriginId = System.Convert.ToInt32(Dr[BusinessConstants.ORIGINID].ToString());
                            TempAirportAirlineDetail.OriginName = Dr[BusinessConstants.ORIGINNAME].ToString();
                            TempAirportAirlineDetail.DestinationId = System.Convert.ToInt32(Dr[BusinessConstants.DESTINATIONID].ToString());
                            TempAirportAirlineDetail.DestinationName = Dr[BusinessConstants.DESTINATIONNAME].ToString();
                            if (Instance == BusinessConstants.Instance.US.ToString())
                            {
                                TempAirportAirlineDetail.Route = Dr[BusinessConstants.ROUTE].ToString();
                                TempAirportAirlineDetail.Direction = Dr[BusinessConstants.DIRECTION].ToString();
                                TempAirportAirlineDetail.Type = Dr[BusinessConstants.TYPE].ToString();
                            }
                            if (Instance == BusinessConstants.Instance.EUR.ToString())
                            {
                                TempAirportAirlineDetail.FlightType = Dr[BusinessConstants.FLIGHTTYPE].ToString();
                            }
                            else if (Instance == BusinessConstants.Instance.AIR.ToString())
                            {
                                TempAirportAirlineDetail.Type = Dr[BusinessConstants.TYPE].ToString();
                            }
                            AllAirportAirlineDetail.Add(TempAirportAirlineDetail);
                        }
                        this.WriteAirportandAirlinetoCacheFile(BusinessConstants.ADMIN_CACHEFILE_NAME, AllAirportAirlineDetail, IsDepartureForm);
                    }
                }
            }
            catch (System.Exception Ex)
            {
                RetValue.ReturnCode = -1;
                RetValue.ReturnMessage = "Cache file creation Failed - error in API ";
                SICTLogger.WriteException(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforAiprortAndAirline", Ex);
            }
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforAiprortAndAirline", "End");
            return RetValue;
        }

        private void WriteAirportandAirlinetoCacheFile(string AirportId, System.Collections.Generic.List<AirportAirlineDetail> AirportAirlineDetail, bool IsDepartureForm)
        {
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "WriteAirportandAirlinetoCacheFile", "Start for AirportId -" + AirportId);
            try
            {
                BusinessUtil ObjBusinessUtil = new BusinessUtil();
                string FilePath = string.Empty;
                string FolderPath = string.Empty;
                string AirportData = string.Empty;
                ObjBusinessUtil.GetAirportAirlineFilePath(AirportId, ref FilePath, ref FolderPath, IsDepartureForm);
                if (!System.IO.Directory.Exists(FolderPath))
                {
                    System.IO.Directory.CreateDirectory(FolderPath);
                }
                SICTLogger.WriteVerbose(CacheFileBusiness.CLASS_NAME, "WriteAirportandAirlinetoCacheFile", "Writing to Airport Airline list cache file for AirportId- " + AirportId);
                using (System.IO.MemoryStream MemoryStream = new System.IO.MemoryStream())
                {
                    DataContractJsonSerializer Serlizer = new DataContractJsonSerializer(typeof(System.Collections.Generic.List<AirportAirlineDetail>));
                    Serlizer.WriteObject(MemoryStream, AirportAirlineDetail);
                    AirportData = System.Text.Encoding.UTF8.GetString(MemoryStream.GetBuffer(), 0, System.Convert.ToInt32(MemoryStream.Length));
                    this.WriteToFile(AirportData, FilePath);
                }
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(CacheFileBusiness.CLASS_NAME, "WriteAirportandAirlinetoCacheFile", Ex);
            }
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "WriteAirportandAirlinetoCacheFile", "End for AirportId -" + AirportId);
        }

        public ReturnValue CreateCacheFileforRouteAndDirection()
        {
            ReturnValue RetValue = new ReturnValue();
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforRouteAndDirection", "Start");
            try
            {
                DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
                DataSet DS = new DataSet();
                SICTLogger.WriteVerbose(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforRouteAndDirection", "Retrieving all Languages from DB");
                DS = DBLayer.GetAllRoutesAndDirections();
                System.Collections.Generic.List<RouteDetail> RouteDetails = new System.Collections.Generic.List<RouteDetail>();
                System.Collections.Generic.List<DirectionDetail> DirectionDetails = new System.Collections.Generic.List<DirectionDetail>();
                if (DS.Tables.Count == 2)
                {
                    foreach (DataRow Dr in DS.Tables[0].Rows)
                    {
                        RouteDetails.Add(new RouteDetail
                        {
                            RouteId = System.Convert.ToInt32(Dr[BusinessConstants.ROUTEID].ToString()),
                            RouteName = Dr[BusinessConstants.ROUTENAME].ToString()
                        });
                    }
                    foreach (DataRow Dr in DS.Tables[1].Rows)
                    {
                        DirectionDetails.Add(new DirectionDetail
                        {
                            DirectionId = System.Convert.ToInt32(Dr[BusinessConstants.DIRECTIONID].ToString()),
                            DirectionName = Dr[BusinessConstants.DIRECTIONNAME].ToString()
                        });
                    }
                }
                this.WriteRouteAndDirectionDetailstoCacheFile(RouteDetails, DirectionDetails);
            }
            catch (System.Exception Ex)
            {
                RetValue.ReturnCode = -1;
                RetValue.ReturnMessage = "Cache file creation Failed - error in API ";
                SICTLogger.WriteException(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforRouteAndDirection", Ex);
            }
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforRouteAndDirection", "End");
            return RetValue;
        }

        private void WriteRouteAndDirectionDetailstoCacheFile(System.Collections.Generic.List<RouteDetail> Routes, System.Collections.Generic.List<DirectionDetail> Directions)
        {
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "WriteLanguagetoCacheFile", "Start");
            try
            {
                BusinessUtil ObjBusinessUtil = new BusinessUtil();
                string RouteFilePath = string.Empty;
                string DirectionFilePath = string.Empty;
                string FolderPath = string.Empty;
                string Data = string.Empty;
                ObjBusinessUtil.GetRouteAndDirectionCachetFilePath(ref RouteFilePath, ref DirectionFilePath, ref FolderPath);
                if (!System.IO.Directory.Exists(FolderPath))
                {
                    System.IO.Directory.CreateDirectory(FolderPath);
                }
                SICTLogger.WriteVerbose(CacheFileBusiness.CLASS_NAME, "WriteLanguagetoCacheFile", "Writing to Route Details list cache file");
                using (System.IO.MemoryStream MemoryStream = new System.IO.MemoryStream())
                {
                    DataContractJsonSerializer Serlizer = new DataContractJsonSerializer(typeof(System.Collections.Generic.List<RouteDetail>));
                    Serlizer.WriteObject(MemoryStream, Routes);
                    Data = System.Text.Encoding.UTF8.GetString(MemoryStream.GetBuffer(), 0, System.Convert.ToInt32(MemoryStream.Length));
                    this.WriteToFile(Data, RouteFilePath);
                }
                Data = string.Empty;
                SICTLogger.WriteVerbose(CacheFileBusiness.CLASS_NAME, "WriteLanguagetoCacheFile", "Writing to Direction Details list cache file");
                using (System.IO.MemoryStream MemoryStream = new System.IO.MemoryStream())
                {
                    DataContractJsonSerializer Serlizer = new DataContractJsonSerializer(typeof(System.Collections.Generic.List<DirectionDetail>));
                    Serlizer.WriteObject(MemoryStream, Directions);
                    Data = System.Text.Encoding.UTF8.GetString(MemoryStream.GetBuffer(), 0, System.Convert.ToInt32(MemoryStream.Length));
                    this.WriteToFile(Data, DirectionFilePath);
                }
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(CacheFileBusiness.CLASS_NAME, "WriteLanguagetoCacheFile", Ex);
            }
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "WriteLanguagetoCacheFile", "End");
        }

        public ReturnValue CreateCacheFileforAirportReportAdminLogin()
        {
            ReturnValue RetValue = new ReturnValue();
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforAirportReportAdminLogin", "Start");
            try
            {
                DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
                SICTLogger.WriteVerbose(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforAirportReportAdminLogin", "Retrieving all OriginsandDestinations from DB");
                DataTable AirportAdmins = DBLayer.GetAllAirportAdminLogins();
                DataTable DOrigins = DBLayer.GetAdminOriginandDestinations(0, false);
                System.Collections.Generic.List<Detail> AdminOrigins = new System.Collections.Generic.List<Detail>();
                foreach (DataRow Dr in DOrigins.Rows)
                {
                    AdminOrigins.Add(new Detail
                    {
                        Id = System.Convert.ToInt32(Dr["OriginOrDestId"].ToString()),
                        Name = Dr["Name"].ToString()
                    });
                }
                this.WriteDetailstoCacheFile(BusinessConstants.ADMIN_CACHEFILE_NAME, AdminOrigins, true);
                foreach (DataRow AirportAdmin in AirportAdmins.Rows)
                {
                    System.Collections.Generic.List<Detail> AirportAdminOrigins = new System.Collections.Generic.List<Detail>();
                    int UserId = System.Convert.ToInt32(AirportAdmin["AdminLoginId"].ToString());
                    DataTable DtAdmin = new DataTable();
                    DtAdmin = DBLayer.GetAdminOriginandDestinations(UserId, true);
                    foreach (DataRow Dr in DtAdmin.Rows)
                    {
                        AirportAdminOrigins.Add(new Detail
                        {
                            Id = System.Convert.ToInt32(Dr["OriginOrDestId"].ToString()),
                            Name = Dr["Name"].ToString()
                        });
                    }
                    this.WriteDetailstoCacheFile(UserId.ToString(), AirportAdminOrigins, true);
                }
                DataTable DtAirlines = DBLayer.GetAdminAirlines(0, false);
                System.Collections.Generic.List<Detail> AdminAirlines = new System.Collections.Generic.List<Detail>();
                foreach (DataRow Dr in DtAirlines.Rows)
                {
                    AdminAirlines.Add(new Detail
                    {
                        Id = System.Convert.ToInt32(Dr["AirlineId"].ToString()),
                        Name = Dr["AirlineName"].ToString()
                    });
                }
                this.WriteDetailstoCacheFile(BusinessConstants.ADMIN_CACHEFILE_NAME, AdminAirlines, false);
                foreach (DataRow AirportAdmin in AirportAdmins.Rows)
                {
                    System.Collections.Generic.List<Detail> AirportAdminAirlines = new System.Collections.Generic.List<Detail>();
                    int UserId = System.Convert.ToInt32(AirportAdmin["AdminLoginId"].ToString());
                    DataTable DtAdmin = new DataTable();
                    DtAdmin = DBLayer.GetAdminAirlines(UserId, true);
                    foreach (DataRow Dr in DtAdmin.Rows)
                    {
                        AirportAdminAirlines.Add(new Detail
                        {
                            Id = System.Convert.ToInt32(Dr["AirlineId"].ToString()),
                            Name = Dr["AirlineName"].ToString()
                        });
                    }
                    this.WriteDetailstoCacheFile(UserId.ToString(), AirportAdminAirlines, false);
                }
            }
            catch (System.Exception Ex)
            {
                RetValue.ReturnCode = -1;
                RetValue.ReturnMessage = "Cache file creation Failed - error in API ";
                SICTLogger.WriteException(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforAirportReportAdminLogin", Ex);
            }
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforAirportReportAdminLogin", "End");
            return RetValue;
        }

        private void WriteDetailstoCacheFile(string FileName, System.Collections.Generic.List<Detail> Details, bool IsOrigin)
        {
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "WriteDetailstoCacheFile", "Start");
            try
            {
                BusinessUtil ObjBusinessUtil = new BusinessUtil();
                string FilePath = string.Empty;
                string FolderPath = string.Empty;
                string Data = string.Empty;
                ObjBusinessUtil.GetAirportReportFilePath(ref FilePath, ref FolderPath, FileName, IsOrigin);
                if (!System.IO.Directory.Exists(FolderPath))
                {
                    System.IO.Directory.CreateDirectory(FolderPath);
                }
                SICTLogger.WriteVerbose(CacheFileBusiness.CLASS_NAME, "WriteDetailstoCacheFile", "Writing to Route Details list cache file");
                using (System.IO.MemoryStream MemoryStream = new System.IO.MemoryStream())
                {
                    DataContractJsonSerializer Serlizer = new DataContractJsonSerializer(typeof(System.Collections.Generic.List<Detail>));
                    Serlizer.WriteObject(MemoryStream, Details);
                    Data = System.Text.Encoding.UTF8.GetString(MemoryStream.GetBuffer(), 0, System.Convert.ToInt32(MemoryStream.Length));
                    this.WriteToFile(Data, FilePath);
                }
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(CacheFileBusiness.CLASS_NAME, "WriteDetailstoCacheFile", Ex);
            }
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "WriteDetailstoCacheFile", "End");
        }

        public ReturnValue CreateCacheFileforTargetVsCompletesCharts(string Instance)
        {
            ReturnValue RetValue = new ReturnValue();
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforTargetVsCompletesCharts", "Start");
            try
            {
                DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
                DataTable DtLogin = new DataTable();
                DtLogin = DBLayer.GetAllAirportLogins();
                string Date = string.Empty;
                string Date2 = string.Empty;
                this.GetSemesterDateRange(ref Date, ref Date2);
                int Year = 0;
                int Quarter = 0;
                this.GetQuarterAndYearDetails(ref Quarter, ref Year);
                if (DtLogin.Rows.Count > 1)
                {
                    for (int LoginCnt = 0; LoginCnt < DtLogin.Rows.Count; LoginCnt++)
                    {
                        int AirportLoginId = System.Convert.ToInt32(DtLogin.Rows[LoginCnt][BusinessConstants.AIRPORTLOGINID].ToString());
                        string AirportLoginName = DtLogin.Rows[LoginCnt]["UserName"].ToString();
                        System.Collections.Generic.List<TargetsVsCompletes> TargetsandCompletes = new System.Collections.Generic.List<TargetsVsCompletes>();
                        System.Collections.Generic.List<MissingTargetsVsBusinessClass> MissingTargetsVsBusinessClass = new System.Collections.Generic.List<MissingTargetsVsBusinessClass>();
                        DataSet DSTargetCompletes = new DataSet();
                        SICTLogger.WriteVerbose(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforTargetVsCompletesCharts", "Retrieving all Targets and Completes for the Airport login -" + AirportLoginId);
                        UserDetailsBusiness udb = new UserDetailsBusiness();
                        if (Instance == BusinessConstants.Instance.EUR.ToString())
                        {
                            DSTargetCompletes = DBLayer.GetTargetandCompleteforAirportLogin(AirportLoginId, Date, Date2);
                        }
                        else if (udb.CheckIsSpecialUserOrNot(AirportLoginName))
                        {
                            string AirlineIds = System.Convert.ToString(ConfigurationManager.AppSettings["AirlineIdForSpecialUserAirport"]);
                            DSTargetCompletes = DBLayer.GetTargetandCompleteforAirportLoginSpecialUser(AirportLoginId, Quarter, Year, AirlineIds);
                        }
                        else
                        {
                            DSTargetCompletes = DBLayer.GetTargetandCompleteforAirportLogin(AirportLoginId, Quarter, Year);
                        }
                        if (DSTargetCompletes.Tables.Count > 2)
                        {
                            foreach (DataRow DrTarget in DSTargetCompletes.Tables[0].Rows)
                            {
                                int Target = System.Convert.ToInt32(DrTarget[BusinessConstants.TARGET].ToString());
                                TargetsVsCompletes TempTargetsandCompletes = new TargetsVsCompletes();
                                int AirlineId = System.Convert.ToInt32(DrTarget[BusinessConstants.AIRLINEID].ToString());
                                string AirlineName = DrTarget[BusinessConstants.AIRLINENAME].ToString();
                                string Code = DrTarget[BusinessConstants.CODE].ToString();
                                int Completes = 0;
                                DataRow[] DrCompletes = DSTargetCompletes.Tables[1].Select(BusinessConstants.AIRLINEID + "=" + AirlineId);
                                if (DrCompletes.Length > 0)
                                {
                                    Completes = System.Convert.ToInt32(DrCompletes[0][BusinessConstants.COMPLETES].ToString());
                                }
                                TempTargetsandCompletes.AirlineId = AirlineId;
                                TempTargetsandCompletes.AirlineName = AirlineName;
                                TempTargetsandCompletes.Code = Code;
                                TempTargetsandCompletes.Target = Target;
                                TempTargetsandCompletes.Completes = Completes;
                                TargetsandCompletes.Add(TempTargetsandCompletes);
                                int BusinessCompletes = 0;
                                DataRow[] DrBusinessCompletes = DSTargetCompletes.Tables[2].Select(BusinessConstants.AIRLINEID + "=" + AirlineId);
                                if (DrBusinessCompletes.Length > 0)
                                {
                                    BusinessCompletes = System.Convert.ToInt32(DrBusinessCompletes[0][BusinessConstants.BUSINESSCOMPLETES].ToString());
                                }
                                double BusinessTarget;
                                if (Instance == BusinessConstants.Instance.EUR.ToString())
                                {
                                    BusinessTarget = (double)Target * 0.25;
                                }
                                else
                                {
                                    BusinessTarget = (double)Target * 0.33333333333333331;
                                }
                                int MissingTarget = Target - Completes;
                                int MissingCompletes = System.Convert.ToInt32(System.Math.Round(BusinessTarget - (double)BusinessCompletes, 0, System.MidpointRounding.AwayFromZero));
                                if (MissingTarget >= 0 && MissingCompletes >= 0)
                                {
                                    MissingTargetsVsBusinessClass.Add(new MissingTargetsVsBusinessClass
                                    {
                                        AirlineId = AirlineId,
                                        AirlineName = DrTarget[BusinessConstants.AIRLINENAME].ToString(),
                                        Code = DrTarget[BusinessConstants.CODE].ToString(),
                                        MissingTarget = MissingTarget,
                                        MissingCompletes = MissingCompletes
                                    });
                                }
                            }
                        }
                        this.WriteTargetVsCompletesChartsCacheFile(AirportLoginId.ToString(), TargetsandCompletes);
                        this.WriteMissingTargetsVsBusinessClassChartCacheFile(AirportLoginId.ToString(), MissingTargetsVsBusinessClass);
                    }
                }
                System.Collections.Generic.List<TargetsVsCompletes> AllTargetsandCompletes = new System.Collections.Generic.List<TargetsVsCompletes>();
                System.Collections.Generic.List<MissingTargetsVsBusinessClass> AllMissingTargetsVsBusinessClass = new System.Collections.Generic.List<MissingTargetsVsBusinessClass>();
                DataSet DSAll = new DataSet();
                if (Instance == BusinessConstants.Instance.EUR.ToString())
                {
                    DSAll = DBLayer.GetAllTargetandComplete(Date, Date2);
                }
                else
                {
                    DSAll = DBLayer.GetAllTargetandComplete(Quarter, Year);
                }
                if (DSAll.Tables.Count > 2)
                {
                    foreach (DataRow DrTarget in DSAll.Tables[0].Rows)
                    {
                        int Target = System.Convert.ToInt32(DrTarget[BusinessConstants.TARGET].ToString());
                        TargetsVsCompletes TempTargetsandCompletes = new TargetsVsCompletes();
                        int AirlineId = System.Convert.ToInt32(DrTarget[BusinessConstants.AIRLINEID].ToString());
                        string AirlineName = DrTarget[BusinessConstants.AIRLINENAME].ToString();
                        string Code = DrTarget[BusinessConstants.CODE].ToString();
                        int Completes = 0;
                        DataRow[] DrCompletes = DSAll.Tables[1].Select(BusinessConstants.AIRLINEID + "=" + AirlineId);
                        if (DrCompletes.Length > 0)
                        {
                            Completes = System.Convert.ToInt32(DrCompletes[0][BusinessConstants.COMPLETES].ToString());
                        }
                        TempTargetsandCompletes.AirlineId = AirlineId;
                        TempTargetsandCompletes.AirlineName = DrTarget[BusinessConstants.AIRLINENAME].ToString();
                        TempTargetsandCompletes.Code = Code;
                        TempTargetsandCompletes.Target = System.Convert.ToInt32(DrTarget[BusinessConstants.TARGET].ToString());
                        TempTargetsandCompletes.Completes = Completes;
                        AllTargetsandCompletes.Add(TempTargetsandCompletes);
                        int BusinessCompletes = 0;
                        DataRow[] DrBusinessCompletes = DSAll.Tables[2].Select(BusinessConstants.AIRLINEID + "=" + AirlineId);
                        if (DrBusinessCompletes.Length > 0)
                        {
                            BusinessCompletes = System.Convert.ToInt32(DrBusinessCompletes[0][BusinessConstants.BUSINESSCOMPLETES].ToString());
                        }
                        double BusinessTarget;
                        if (Instance == BusinessConstants.Instance.EUR.ToString())
                        {
                            BusinessTarget = (double)Target * 0.25;
                        }
                        else
                        {
                            BusinessTarget = (double)Target * 0.33333333333333331;
                        }
                        int MissingTarget = Target - Completes;
                        int MissingCompletes = System.Convert.ToInt32(System.Math.Round(BusinessTarget - (double)BusinessCompletes, 0, System.MidpointRounding.AwayFromZero));
                        if (MissingTarget >= 0 && MissingCompletes >= 0)
                        {
                            AllMissingTargetsVsBusinessClass.Add(new MissingTargetsVsBusinessClass
                            {
                                AirlineId = AirlineId,
                                AirlineName = DrTarget[BusinessConstants.AIRLINENAME].ToString(),
                                Code = DrTarget[BusinessConstants.CODE].ToString(),
                                MissingTarget = MissingTarget,
                                MissingCompletes = MissingCompletes
                            });
                        }
                    }
                }
                this.WriteTargetVsCompletesChartsCacheFile(BusinessConstants.ADMIN_CACHEFILE_NAME, AllTargetsandCompletes);
                this.WriteMissingTargetsVsBusinessClassChartCacheFile(BusinessConstants.ADMIN_CACHEFILE_NAME, AllMissingTargetsVsBusinessClass);
            }
            catch (System.Exception Ex)
            {
                RetValue.ReturnCode = -1;
                RetValue.ReturnMessage = "Cache file creation Failed - error in API ";
                SICTLogger.WriteException(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforTargetVsCompletesCharts", Ex);
            }
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforTargetVsCompletesCharts", "End");
            return RetValue;
        }

        public ReturnValue CreateCacheFileforTargetVsCompletesChartsForaAirport(string Instance, int AirportId)
        {
            ReturnValue RetValue = new ReturnValue();
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforTargetVsCompletesCharts", "Start");
            try
            {
                DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
                DataTable DtLogin = new DataTable();
                DtLogin = DBLayer.GetLoginIdFromAirportId(AirportId);
                if (DtLogin.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(DtLogin.Rows[0][BusinessConstants.AIRPORTLOGINID].ToString()))
                    {
                        int AirportLoginId = System.Convert.ToInt32(DtLogin.Rows[0][BusinessConstants.AIRPORTLOGINID].ToString());
                        string Date = string.Empty;
                        string Date2 = string.Empty;
                        this.GetSemesterDateRange(ref Date, ref Date2);
                        int Year = 0;
                        int Quarter = 0;
                        this.GetQuarterAndYearDetails(ref Quarter, ref Year);
                        System.Collections.Generic.List<TargetsVsCompletes> TargetsandCompletes = new System.Collections.Generic.List<TargetsVsCompletes>();
                        System.Collections.Generic.List<MissingTargetsVsBusinessClass> MissingTargetsVsBusinessClass = new System.Collections.Generic.List<MissingTargetsVsBusinessClass>();
                        DataSet DSTargetCompletes = new DataSet();
                        SICTLogger.WriteVerbose(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforTargetVsCompletesCharts", "Retrieving all Targets and Completes for the Airport login -" + AirportLoginId);
                        if (Instance == BusinessConstants.Instance.EUR.ToString())
                        {
                            DSTargetCompletes = DBLayer.GetTargetandCompleteforAirportLogin(AirportLoginId, Date, Date2);
                        }
                        else
                        {
                            DSTargetCompletes = DBLayer.GetTargetandCompleteforAirportLogin(AirportLoginId, Quarter, Year);
                        }
                        if (DSTargetCompletes.Tables.Count > 2)
                        {
                            foreach (DataRow DrTarget in DSTargetCompletes.Tables[0].Rows)
                            {
                                int Target = System.Convert.ToInt32(DrTarget[BusinessConstants.TARGET].ToString());
                                TargetsVsCompletes TempTargetsandCompletes = new TargetsVsCompletes();
                                int AirlineId = System.Convert.ToInt32(DrTarget[BusinessConstants.AIRLINEID].ToString());
                                string AirlineName = DrTarget[BusinessConstants.AIRLINENAME].ToString();
                                string Code = DrTarget[BusinessConstants.CODE].ToString();
                                int Completes = 0;
                                DataRow[] DrCompletes = DSTargetCompletes.Tables[1].Select(BusinessConstants.AIRLINEID + "=" + AirlineId);
                                if (DrCompletes.Length > 0)
                                {
                                    Completes = System.Convert.ToInt32(DrCompletes[0][BusinessConstants.COMPLETES].ToString());
                                }
                                TempTargetsandCompletes.AirlineId = AirlineId;
                                TempTargetsandCompletes.AirlineName = AirlineName;
                                TempTargetsandCompletes.Code = Code;
                                TempTargetsandCompletes.Target = Target;
                                TempTargetsandCompletes.Completes = Completes;
                                TargetsandCompletes.Add(TempTargetsandCompletes);
                                int BusinessCompletes = 0;
                                DataRow[] DrBusinessCompletes = DSTargetCompletes.Tables[2].Select(BusinessConstants.AIRLINEID + "=" + AirlineId);
                                if (DrBusinessCompletes.Length > 0)
                                {
                                    BusinessCompletes = System.Convert.ToInt32(DrBusinessCompletes[0][BusinessConstants.BUSINESSCOMPLETES].ToString());
                                }
                                double BusinessTarget;
                                if (Instance == BusinessConstants.Instance.EUR.ToString())
                                {
                                    BusinessTarget = (double)Target * 0.25;
                                }
                                else
                                {
                                    BusinessTarget = (double)Target * 0.33333333333333331;
                                }
                                int MissingTarget = Target - Completes;
                                int MissingCompletes = System.Convert.ToInt32(System.Math.Round(BusinessTarget - (double)BusinessCompletes, 0, System.MidpointRounding.AwayFromZero));
                                if (MissingTarget >= 0 && MissingCompletes >= 0)
                                {
                                    MissingTargetsVsBusinessClass.Add(new MissingTargetsVsBusinessClass
                                    {
                                        AirlineId = AirlineId,
                                        AirlineName = DrTarget[BusinessConstants.AIRLINENAME].ToString(),
                                        Code = DrTarget[BusinessConstants.CODE].ToString(),
                                        MissingTarget = MissingTarget,
                                        MissingCompletes = MissingCompletes
                                    });
                                }
                            }
                        }
                        this.WriteTargetVsCompletesChartsCacheFile(AirportLoginId.ToString(), TargetsandCompletes);
                        this.WriteMissingTargetsVsBusinessClassChartCacheFile(AirportLoginId.ToString(), MissingTargetsVsBusinessClass);
                        System.Collections.Generic.List<TargetsVsCompletes> AllTargetsandCompletes = new System.Collections.Generic.List<TargetsVsCompletes>();
                        System.Collections.Generic.List<MissingTargetsVsBusinessClass> AllMissingTargetsVsBusinessClass = new System.Collections.Generic.List<MissingTargetsVsBusinessClass>();
                        DataSet DSAll = new DataSet();
                        if (Instance == BusinessConstants.Instance.EUR.ToString())
                        {
                            DSAll = DBLayer.GetAllTargetandComplete(Date, Date2);
                        }
                        else
                        {
                            DSAll = DBLayer.GetAllTargetandComplete(Quarter, Year);
                        }
                        if (DSAll.Tables.Count > 2)
                        {
                            foreach (DataRow DrTarget in DSAll.Tables[0].Rows)
                            {
                                int Target = System.Convert.ToInt32(DrTarget[BusinessConstants.TARGET].ToString());
                                TargetsVsCompletes TempTargetsandCompletes = new TargetsVsCompletes();
                                int AirlineId = System.Convert.ToInt32(DrTarget[BusinessConstants.AIRLINEID].ToString());
                                string AirlineName = DrTarget[BusinessConstants.AIRLINENAME].ToString();
                                string Code = DrTarget[BusinessConstants.CODE].ToString();
                                int Completes = 0;
                                DataRow[] DrCompletes = DSAll.Tables[1].Select(BusinessConstants.AIRLINEID + "=" + AirlineId);
                                if (DrCompletes.Length > 0)
                                {
                                    Completes = System.Convert.ToInt32(DrCompletes[0][BusinessConstants.COMPLETES].ToString());
                                }
                                TempTargetsandCompletes.AirlineId = AirlineId;
                                TempTargetsandCompletes.AirlineName = DrTarget[BusinessConstants.AIRLINENAME].ToString();
                                TempTargetsandCompletes.Code = Code;
                                TempTargetsandCompletes.Target = System.Convert.ToInt32(DrTarget[BusinessConstants.TARGET].ToString());
                                TempTargetsandCompletes.Completes = Completes;
                                AllTargetsandCompletes.Add(TempTargetsandCompletes);
                                int BusinessCompletes = 0;
                                DataRow[] DrBusinessCompletes = DSAll.Tables[2].Select(BusinessConstants.AIRLINEID + "=" + AirlineId);
                                if (DrBusinessCompletes.Length > 0)
                                {
                                    BusinessCompletes = System.Convert.ToInt32(DrBusinessCompletes[0][BusinessConstants.BUSINESSCOMPLETES].ToString());
                                }
                                double BusinessTarget;
                                if (Instance == BusinessConstants.Instance.EUR.ToString())
                                {
                                    BusinessTarget = (double)Target * 0.25;
                                }
                                else
                                {
                                    BusinessTarget = (double)Target * 0.33333333333333331;
                                }
                                int MissingTarget = Target - Completes;
                                int MissingCompletes = System.Convert.ToInt32(System.Math.Round(BusinessTarget - (double)BusinessCompletes, 0, System.MidpointRounding.AwayFromZero));
                                if (MissingTarget >= 0 && MissingCompletes >= 0)
                                {
                                    AllMissingTargetsVsBusinessClass.Add(new MissingTargetsVsBusinessClass
                                    {
                                        AirlineId = AirlineId,
                                        AirlineName = DrTarget[BusinessConstants.AIRLINENAME].ToString(),
                                        Code = DrTarget[BusinessConstants.CODE].ToString(),
                                        MissingTarget = MissingTarget,
                                        MissingCompletes = MissingCompletes
                                    });
                                }
                            }
                        }
                        this.WriteTargetVsCompletesChartsCacheFile(BusinessConstants.ADMIN_CACHEFILE_NAME, AllTargetsandCompletes);
                        this.WriteMissingTargetsVsBusinessClassChartCacheFile(BusinessConstants.ADMIN_CACHEFILE_NAME, AllMissingTargetsVsBusinessClass);
                    }
                }
            }
            catch (System.Exception Ex)
            {
                RetValue.ReturnCode = -1;
                RetValue.ReturnMessage = "Cache file creation Failed - error in API ";
                SICTLogger.WriteException(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforTargetVsCompletesCharts", Ex);
            }
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforTargetVsCompletesCharts", "End");
            return RetValue;
        }

        private void WriteTargetVsCompletesChartsCacheFile(string LoginId, System.Collections.Generic.List<TargetsVsCompletes> TargetsandCompletes)
        {
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "WriteTargetVsCompletesChartsCacheFile", "Start for LoginId -" + LoginId);
            try
            {
                BusinessUtil ObjBusinessUtil = new BusinessUtil();
                string FilePath = string.Empty;
                string FolderPath = string.Empty;
                string TargetsandCompletesData = string.Empty;
                ObjBusinessUtil.GetTargetVsCompletesChartsFilePath(LoginId, ref FilePath, ref FolderPath);
                if (!System.IO.Directory.Exists(FolderPath))
                {
                    System.IO.Directory.CreateDirectory(FolderPath);
                }
                SICTLogger.WriteVerbose(CacheFileBusiness.CLASS_NAME, "WriteTargetVsCompletesChartsCacheFile", "TargetVsCompletes Charts cache file for AirportLoginId- " + LoginId);
                using (System.IO.MemoryStream MemoryStream = new System.IO.MemoryStream())
                {
                    DataContractJsonSerializer Serlizer = new DataContractJsonSerializer(typeof(System.Collections.Generic.List<TargetsVsCompletes>));
                    Serlizer.WriteObject(MemoryStream, TargetsandCompletes);
                    TargetsandCompletesData = System.Text.Encoding.UTF8.GetString(MemoryStream.GetBuffer(), 0, System.Convert.ToInt32(MemoryStream.Length));
                    this.WriteToFile(TargetsandCompletesData, FilePath);
                }
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(CacheFileBusiness.CLASS_NAME, "WriteTargetVsCompletesChartsCacheFile", Ex);
            }
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "WriteTargetVsCompletesChartsCacheFile", "End for LoginId -" + LoginId);
        }

        private void WriteMissingTargetsVsBusinessClassChartCacheFile(string LoginId, System.Collections.Generic.List<MissingTargetsVsBusinessClass> MissingTargetsVsBusinessClass)
        {
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "WriteMissingTargetsVsBusinessClassChartCacheFile", "Start for LoginId -" + LoginId);
            try
            {
                BusinessUtil ObjBusinessUtil = new BusinessUtil();
                string FilePath = string.Empty;
                string FolderPath = string.Empty;
                string MissingTargetsandBusinessClassData = string.Empty;
                ObjBusinessUtil.GetMissingTargetsVsBusinessClassChartsFilePath(LoginId, ref FilePath, ref FolderPath);
                if (!System.IO.Directory.Exists(FolderPath))
                {
                    System.IO.Directory.CreateDirectory(FolderPath);
                }
                SICTLogger.WriteVerbose(CacheFileBusiness.CLASS_NAME, "WriteMissingTargetsVsBusinessClassChartCacheFile", "MissingTargetsVsBusinessClass Charts cache file for AirportLoginId- " + LoginId);
                using (System.IO.MemoryStream MemoryStream = new System.IO.MemoryStream())
                {
                    DataContractJsonSerializer Serlizer = new DataContractJsonSerializer(typeof(System.Collections.Generic.List<MissingTargetsVsBusinessClass>));
                    Serlizer.WriteObject(MemoryStream, MissingTargetsVsBusinessClass);
                    MissingTargetsandBusinessClassData = System.Text.Encoding.UTF8.GetString(MemoryStream.GetBuffer(), 0, System.Convert.ToInt32(MemoryStream.Length));
                    this.WriteToFile(MissingTargetsandBusinessClassData, FilePath);
                }
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(CacheFileBusiness.CLASS_NAME, "WriteMissingTargetsVsBusinessClassChartCacheFile", Ex);
            }
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "WriteMissingTargetsVsBusinessClassChartCacheFile", "End for LoginId -" + LoginId);
        }

        public ReturnValue CreateCacheFileforFlightTypes()
        {
            ReturnValue RetValue = new ReturnValue();
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforFlightTypes", "Start");
            try
            {
                DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
                DataTable Dt = new DataTable();
                SICTLogger.WriteVerbose(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforFlightTypes", "Retrieving all Flight Types from DB");
                Dt = DBLayer.GetAllFlightTypes();
                System.Collections.Generic.List<Detail> Details = new System.Collections.Generic.List<Detail>();
                foreach (DataRow Dr in Dt.Rows)
                {
                    Details.Add(new Detail
                    {
                        Id = System.Convert.ToInt32(Dr[BusinessConstants.FLIGHTTYPEID].ToString()),
                        Name = Dr[BusinessConstants.FLIGHTTYPENAME].ToString()
                    });
                }
                this.WriteDetailstoCacheFile("FlightTypes", Details);
            }
            catch (System.Exception Ex)
            {
                RetValue.ReturnCode = -1;
                RetValue.ReturnMessage = "Cache file creation Failed - error in API ";
                SICTLogger.WriteException(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforFlightTypes", Ex);
            }
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforFlightTypes", "End");
            return RetValue;
        }

        private void WriteDetailstoCacheFile(string FileName, System.Collections.Generic.List<Detail> Details)
        {
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "WriteDetailstoCacheFile", "Start");
            try
            {
                BusinessUtil ObjBusinessUtil = new BusinessUtil();
                string FilePath = string.Empty;
                string FolderPath = string.Empty;
                string Data = string.Empty;
                ObjBusinessUtil.GetCachetFilePath(FileName, ref FilePath, ref FolderPath);
                if (!System.IO.Directory.Exists(FolderPath))
                {
                    System.IO.Directory.CreateDirectory(FolderPath);
                }
                SICTLogger.WriteVerbose(CacheFileBusiness.CLASS_NAME, "WriteDetailstoCacheFile", "Writing Flight Type cache file");
                using (System.IO.MemoryStream MemoryStream = new System.IO.MemoryStream())
                {
                    DataContractJsonSerializer Serlizer = new DataContractJsonSerializer(typeof(System.Collections.Generic.List<Detail>));
                    Serlizer.WriteObject(MemoryStream, Details);
                    Data = System.Text.Encoding.UTF8.GetString(MemoryStream.GetBuffer(), 0, System.Convert.ToInt32(MemoryStream.Length));
                    this.WriteToFile(Data, FilePath);
                }
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(CacheFileBusiness.CLASS_NAME, "WriteDetailstoCacheFile", Ex);
            }
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "WriteDetailstoCacheFile", "End");
        }

        public ReturnValue CreateCacheFileforAircraftTypes()
        {
            ReturnValue RetValue = new ReturnValue();
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforAircraftTypes", "Start");
            try
            {
                DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
                DataTable Dt = new DataTable();
                SICTLogger.WriteVerbose(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforAircraftTypes", "Retrieving all Aircraft Types from DB");
                Dt = DBLayer.GetAllAircraftTypes();
                System.Collections.Generic.List<Detail> Details = new System.Collections.Generic.List<Detail>();
                foreach (DataRow Dr in Dt.Rows)
                {
                    Details.Add(new Detail
                    {
                        Id = System.Convert.ToInt32(Dr[BusinessConstants.AIRCRAFTTYPE_ID].ToString()),
                        Name = Dr[BusinessConstants.AIRCRAFTTYPE_NAME].ToString()
                    });
                }
                this.WriteDetailstoCacheFile("AircraftTypes", Details);
            }
            catch (System.Exception Ex)
            {
                RetValue.ReturnCode = -1;
                RetValue.ReturnMessage = "Cache file creation Failed - error in API ";
                SICTLogger.WriteException(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforAircraftTypes", Ex);
            }
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "CreateCacheFileforAircraftTypes", "End");
            return RetValue;
        }

        private void GetQuarterAndYearDetails(ref int Quarter, ref int Year)
        {
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "GetQuarterAndYear", "Start");
            try
            {
                int Month = System.DateTime.Now.Month;
                Quarter = 0;
                Year = System.DateTime.Now.Year;
                int MonthsInOneQuarter = 3;
                if (Month % MonthsInOneQuarter > 0)
                {
                    Quarter = Month / MonthsInOneQuarter + 1;
                }
                else
                {
                    Quarter = Month / MonthsInOneQuarter;
                }
                if (Month == 4 || Month == 7 || Month == 10)
                {
                    int Day = System.DateTime.Now.Day;
                    if (Day <= 10)
                    {
                        Quarter--;
                    }
                }
                if (Month == 1)
                {
                    int Day = System.DateTime.Now.Day;
                    if (Day <= 10)
                    {
                        Quarter = 4;
                        Year--;
                    }
                }
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(CacheFileBusiness.CLASS_NAME, "GetQuarterAndYear", Ex);
            }
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "GetQuarterAndYear", "End ");
        }

        private void GetSemesterDateRange(ref string Date1, ref string Date2)
        {
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "GetQuarterAndYear", "Start");
            System.DateTime TempDate = default(System.DateTime);
            System.DateTime TempDate2 = default(System.DateTime);
            try
            {
                int CurMonth = System.DateTime.Now.Month;
                int CurYear = System.DateTime.Now.Year;
                int CurDay = System.DateTime.Now.Day;
                int Month;
                int Month2;
                int Day;
                int Day2;
                int Year3;
                int Year2;
                if (CurMonth >= 4 && CurMonth <= 9)
                {
                    Month = 4;
                    Month2 = 9;
                    Day = 1;
                    Day2 = 30;
                    Year2 = (Year3 = CurYear);
                    if (CurMonth == 4 && CurDay <= 10)
                    {
                        Month = 10;
                        Month2 = 3;
                        Year3 = CurYear - 1;
                        Year2 = CurYear;
                        Day = 1;
                        Day2 = 31;
                    }
                }
                else
                {
                    if (CurMonth >= 1 && CurMonth <= 3)
                    {
                        Month = 10;
                        Month2 = 3;
                        Year3 = CurYear - 1;
                        Year2 = CurYear;
                        Day = 1;
                        Day2 = 31;
                    }
                    else
                    {
                        Month = 10;
                        Month2 = 3;
                        Year3 = CurYear;
                        Year2 = CurYear + 1;
                        Day = 1;
                        Day2 = 31;
                    }
                    if (CurMonth == 10 && CurDay <= 10)
                    {
                        Month = 4;
                        Month2 = 9;
                        Year2 = (Year3 = CurYear);
                        Day = 1;
                        Day2 = 30;
                    }
                }
                TempDate = new System.DateTime(Year3, Month, Day);
                TempDate2 = new System.DateTime(Year2, Month2, Day2);
                Date1 = TempDate.Date.ToString("yyyy-MM-dd");
                Date2 = TempDate2.Date.ToString("yyyy-MM-dd");
                Date1 = Date1.Replace(".", "-");
                Date2 = Date2.Replace(".", "-");
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(CacheFileBusiness.CLASS_NAME, "GetQuarterAndYear", Ex);
            }
            SICTLogger.WriteInfo(CacheFileBusiness.CLASS_NAME, "GetQuarterAndYear", "End ");
        }
    }
}
