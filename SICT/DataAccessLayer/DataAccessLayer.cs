using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using SICT.Constants;
using SICT.DataContracts;
using SICT.DBUtils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace SICT.DataAccessLayer
{
    public class DataAccessLayer
    {
        private static readonly string CLASS_NAME = "DataAccessLayer";

        public System.Data.DataTable GetAllUserNameAndPassword()
        {
            System.Data.DataTable DtUsers = new System.Data.DataTable();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                using (DB.CreateConnection())
                {
                    System.Data.Common.DbCommand storedProcCommand = DB.GetStoredProcCommand("GetAllUserNameAndPassword");
                    System.Data.DataSet DS = DB.ExecuteDataSet(storedProcCommand);
                    if (DS.Tables.Count > 0)
                    {
                        DtUsers = DS.Tables[0];
                    }
                    else
                    {
                        SICTLogger.WriteWarning(DataAccessLayer.CLASS_NAME, "GetAllUserNameAndPassword", "User Dataset doesn't contain table");
                    }
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetAllUserNameAndPassword", ex);
            }
            return DtUsers;
        }

        public bool IsUserAlreadyLoggedIn(int UserId, ref double LoggedInDuration)
        {
            bool RetValue = false;
            try
            {
                System.Data.DataSet Ds = new System.Data.DataSet();
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetSessionIdByUserId");
                DB.AddInParameter(ObjCommand, BusinessConstants.USERID, System.Data.DbType.Int32, UserId);
                Ds = DB.ExecuteDataSet(ObjCommand);
                if (Ds.Tables.Count > 0)
                {
                    if (Ds.Tables[0].Rows.Count != 0)
                    {
                        DateTime LoggedInTime = Convert.ToDateTime(Ds.Tables[0].Rows[0][1]);
                        LoggedInDuration = DateTime.Now.Subtract(LoggedInTime).TotalMinutes;
                        RetValue = true;
                    }
                }
                else
                {
                    RetValue = false;
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "IsUserAlreadyLoggedIn", ex);
            }
            return RetValue;
        }

        public string GetExistingSessionIdAndUpdateTime(int UserId)
        {
            string SessionId = null;
            try
            {
                System.Data.DataSet Ds = new System.Data.DataSet();
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetSessionIdAndUpdateTimeByUserId");
                DB.AddInParameter(ObjCommand, BusinessConstants.USERID, System.Data.DbType.Int32, UserId);
                Ds = DB.ExecuteDataSet(ObjCommand);
                if (Ds.Tables.Count > 0)
                {
                    if (Ds.Tables[0].Rows.Count != 0)
                    {
                        SessionId = Ds.Tables[0].Rows[0][0].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetExistingSessionIdAndUpdateTime", ex);
            }
            return SessionId;
        }

        public string InsertValuesToSessionTable(string SessionId, int UserId)
        {
            int RetValue = 0;
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("UpdateSessionTable");
                DB.AddInParameter(ObjCommand, BusinessConstants.SESSIONID, System.Data.DbType.String, SessionId);
                DB.AddInParameter(ObjCommand, BusinessConstants.USERID, System.Data.DbType.Int32, UserId);
                RetValue = DB.ExecuteNonQuery(ObjCommand);
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "InsertValuesToSessionTable", ex);
            }
            string result;
            if (RetValue == -1)
            {
                result = SessionId;
            }
            else
            {
                result = null;
            }
            return result;
        }

        public bool CheckSessionIdExisistOrNot(string SessionId, bool IsValidCheck)
        {
            bool IsValid = false;
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                using (DB.CreateConnection())
                {
                    System.Data.Common.DbCommand DbCommand = DB.GetStoredProcCommand("CheckSessionIdExisistOrNot");
                    DB.AddInParameter(DbCommand, BusinessConstants.SEESIONID, System.Data.DbType.String, SessionId);
                    DB.AddInParameter(DbCommand, "IsValidCheck", System.Data.DbType.Boolean, IsValidCheck);
                    DB.AddOutParameter(DbCommand, "@IfSessionIdExists", System.Data.DbType.Boolean, 0);
                    DB.ExecuteNonQuery(DbCommand);
                    IsValid = (bool)DB.GetParameterValue(DbCommand, "@IfSessionIdExists");
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "CheckSessionIdExisistOrNot", ex);
            }
            return IsValid;
        }

        public void AddAuditLogInfo(string SessionId, string Source, string Type, string Description, string BrowserDetails, string DataRecieved)
        {
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                using (System.Data.Common.DbCommand DbCommand = DB.GetStoredProcCommand("InsertAuditLogs"))
                {
                    DB.AddInParameter(DbCommand, BusinessConstants.AUDITLOG_SOURCE, System.Data.DbType.String, Source);
                    DB.AddInParameter(DbCommand, BusinessConstants.AUDITLOG_DESCRIPTION, System.Data.DbType.String, Description);
                    DB.AddInParameter(DbCommand, BusinessConstants.TYPE, System.Data.DbType.String, Type);
                    DB.AddInParameter(DbCommand, BusinessConstants.SEESIONID, System.Data.DbType.String, SessionId);
                    DB.AddInParameter(DbCommand, BusinessConstants.BROWSERDETAILS, System.Data.DbType.String, BrowserDetails);
                    DB.AddInParameter(DbCommand, BusinessConstants.DATARECIEVED, System.Data.DbType.String, DataRecieved);
                    DB.ExecuteNonQuery(DbCommand);
                }
            }
            catch (Exception Ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "AddAuditLogInfo", Ex);
            }
        }

        public System.Data.DataSet GetDepartureFormDetailst(string SessionId, int StartIndex, int Offset, string OrderByCondition, string WhereCondition, int AirportId, string IsDepartureForm, bool IsSerialNoRetrieval, long StarSerialNo = 0L, long EndSerialNo = 0L)
        {
            System.Data.DataSet DSResult = new System.Data.DataSet();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetDepartureFormEntriesByStartIndexandOffset");
                DB.AddInParameter(ObjCommand, BusinessConstants.STARTINDEX, System.Data.DbType.Int32, StartIndex);
                DB.AddInParameter(ObjCommand, BusinessConstants.OFFSET, System.Data.DbType.Int32, Offset);
                if (!string.IsNullOrEmpty(OrderByCondition))
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.ORDERBYCONDITION, System.Data.DbType.String, OrderByCondition);
                }
                if (!string.IsNullOrEmpty(WhereCondition))
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.WHERECONDITION, System.Data.DbType.String, WhereCondition);
                }
                if (0L != StarSerialNo)
                {
                    DB.AddInParameter(ObjCommand, "StartSerailNo", System.Data.DbType.Int64, StarSerialNo);
                }
                if (0L != EndSerialNo)
                {
                    DB.AddInParameter(ObjCommand, "EndSerailNo", System.Data.DbType.Int64, EndSerialNo);
                }
                if (IsDepartureForm != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.ISDEPARTUREFORM, System.Data.DbType.Boolean, Convert.ToBoolean(IsDepartureForm));
                }
                DB.AddInParameter(ObjCommand, BusinessConstants.ISSERIALNORETRIEVAL, System.Data.DbType.Boolean, IsSerialNoRetrieval);
                if (AirportId != -1)
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.AIRPORTID, System.Data.DbType.Int32, AirportId);
                }
                DB.AddInParameter(ObjCommand, BusinessConstants.SESSIONID, System.Data.DbType.String, SessionId);
                DSResult = DB.ExecuteDataSet(ObjCommand);
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetCardDetailsByStartIndexandOffset", ex);
            }
            return DSResult;
        }

        public System.Data.DataSet GetDepartureFormDetailstForSpecialUser(string SessionId, int StartIndex, int Offset, string OrderByCondition, string WhereCondition, int AirportId, string IsDepartureForm, bool IsSerialNoRetrieval, long StarSerialNo = 0L, long EndSerialNo = 0L)
        {
            System.Data.DataSet DSResult = new System.Data.DataSet();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetDepartureFormEntriesByStartIndexandOffsetForSpecialUser");
                DB.AddInParameter(ObjCommand, BusinessConstants.STARTINDEX, System.Data.DbType.Int32, StartIndex);
                DB.AddInParameter(ObjCommand, BusinessConstants.OFFSET, System.Data.DbType.Int32, Offset);
                if (!string.IsNullOrEmpty(OrderByCondition))
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.ORDERBYCONDITION, System.Data.DbType.String, OrderByCondition);
                }
                if (!string.IsNullOrEmpty(WhereCondition))
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.WHERECONDITION, System.Data.DbType.String, WhereCondition);
                }
                if (0L != StarSerialNo)
                {
                    DB.AddInParameter(ObjCommand, "StartSerailNo", System.Data.DbType.Int64, StarSerialNo);
                }
                if (0L != EndSerialNo)
                {
                    DB.AddInParameter(ObjCommand, "EndSerailNo", System.Data.DbType.Int64, EndSerialNo);
                }
                if (IsDepartureForm != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.ISDEPARTUREFORM, System.Data.DbType.Boolean, Convert.ToBoolean(IsDepartureForm));
                }
                DB.AddInParameter(ObjCommand, BusinessConstants.ISSERIALNORETRIEVAL, System.Data.DbType.Boolean, IsSerialNoRetrieval);
                if (AirportId != -1)
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.AIRPORTID, System.Data.DbType.Int32, AirportId);
                }
                DB.AddInParameter(ObjCommand, BusinessConstants.SESSIONID, System.Data.DbType.String, SessionId);
                DSResult = DB.ExecuteDataSet(ObjCommand);
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetDepartureFormDetailstForSpecialUser", ex);
            }
            return DSResult;
        }

        public System.Data.DataSet GetGlobalDepartureFormDetailst(string SessionId, int StartIndex, int Offset, string OrderByCondition, string WhereCondition, int AirportId, string IsDepartureForm, bool IsSerialNoRetrieval, long StarSerialNo = 0L, long EndSerialNo = 0L)
        {
            System.Data.DataSet DSResult = new System.Data.DataSet();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetGlobalDepartureFormEntriesByStartIndexandOffset");
                DB.AddInParameter(ObjCommand, BusinessConstants.STARTINDEX, System.Data.DbType.Int32, StartIndex);
                DB.AddInParameter(ObjCommand, BusinessConstants.OFFSET, System.Data.DbType.Int32, Offset);
                if (!string.IsNullOrEmpty(OrderByCondition))
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.ORDERBYCONDITION, System.Data.DbType.String, OrderByCondition);
                }
                if (!string.IsNullOrEmpty(WhereCondition))
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.WHERECONDITION, System.Data.DbType.String, WhereCondition);
                }
                if (0L != StarSerialNo)
                {
                    DB.AddInParameter(ObjCommand, "StartSerailNo", System.Data.DbType.Int64, StarSerialNo);
                }
                if (0L != EndSerialNo)
                {
                    DB.AddInParameter(ObjCommand, "EndSerailNo", System.Data.DbType.Int64, EndSerialNo);
                }
                if (IsDepartureForm != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.ISDEPARTUREFORM, System.Data.DbType.Boolean, Convert.ToBoolean(IsDepartureForm));
                }
                DB.AddInParameter(ObjCommand, BusinessConstants.ISSERIALNORETRIEVAL, System.Data.DbType.Boolean, IsSerialNoRetrieval);
                if (AirportId != -1)
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.AIRPORTID, System.Data.DbType.Int32, AirportId);
                }
                DB.AddInParameter(ObjCommand, BusinessConstants.SESSIONID, System.Data.DbType.String, SessionId);
                DSResult = DB.ExecuteDataSet(ObjCommand);
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetCardDetailsByStartIndexandOffset", ex);
            }
            return DSResult;
        }

        public FormSubmitDetails SetFormDetails(string Instance, FormDetails FormDetails)
        {
            SICTLogger.WriteInfo(DataAccessLayer.CLASS_NAME, "SetFormDetails", "Start ");
            FormSubmitDetails FormSubmitDetails = new FormSubmitDetails();
            List<AirlineDetail> AirlineDetails = new List<AirlineDetail>();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                using (DB.CreateConnection())
                {
                    DateTime Dt = DateTime.Now;
                    try
                    {
                        List<int> DateDetails = new List<int>(Array.ConvertAll<string, int>(FormDetails.FieldWorkDate.Split(new char[]
                        {
                            '/'
                        }), new Converter<string, int>(int.Parse)));
                        if (DateDetails.Count == 3)
                        {
                            Dt = new DateTime(DateDetails[2], DateDetails[0], DateDetails[1]);
                        }
                    }
                    catch (Exception Ex)
                    {
                    }
                    for (int AirlineCnt = 0; AirlineCnt < FormDetails.Airlines.Length; AirlineCnt++)
                    {
                        AirlineDetail TempAirlineDetail = new AirlineDetail();
                        TempAirlineDetail.AirlineId = FormDetails.Airlines[AirlineCnt].AirlineId;
                        List<string> InValidLanguageList = new List<string>();
                        bool IsSerialNosValid = this.CheckisSerialNosValid(FormDetails.Airlines[AirlineCnt].Languages, ref InValidLanguageList);
                        if (IsSerialNosValid)
                        {
                            System.Data.Common.DbCommand DbCommand = DB.GetStoredProcCommand("SetFormDetails");
                            DB.AddInParameter(DbCommand, BusinessConstants.AIRPORTID, System.Data.DbType.String, FormDetails.AirportId);
                            DB.AddInParameter(DbCommand, BusinessConstants.AIRLINEID, System.Data.DbType.Int32, FormDetails.Airlines[AirlineCnt].AirlineId);
                            DB.AddInParameter(DbCommand, BusinessConstants.FORM_DISTRIBUTIONDATE, System.Data.DbType.Date, Dt);
                            DB.AddInParameter(DbCommand, BusinessConstants.FORM_INTERVIEWERID, System.Data.DbType.Int32, FormDetails.InterviewerId);
                            DB.AddInParameter(DbCommand, BusinessConstants.FORM_TYPE, System.Data.DbType.String, FormDetails.IsDepartureForm ? BusinessConstants.FORM_TYPE_DEPARTURE : BusinessConstants.FORM_TYPE_ARRIVAL);
                            DB.AddInParameter(DbCommand, BusinessConstants.FORM_DESTINATIONID, System.Data.DbType.Int32, FormDetails.Airlines[AirlineCnt].DestinationId);
                            DB.AddInParameter(DbCommand, BusinessConstants.FORM_ORIGINID, System.Data.DbType.Int32, FormDetails.Airlines[AirlineCnt].OriginId);
                            DB.AddInParameter(DbCommand, BusinessConstants.FORM_FLIGHTNUMBER, System.Data.DbType.String, FormDetails.Airlines[AirlineCnt].FlightNumber);
                            DB.AddInParameter(DbCommand, BusinessConstants.FORM_BUSINESSCARDS, System.Data.DbType.Int32, FormDetails.Airlines[AirlineCnt].BCardsDistributed);
                            DB.AddInParameter(DbCommand, BusinessConstants.FORM_CLIENTCOMMENTS, System.Data.DbType.String, null);
                            if (Instance == BusinessConstants.Instance.US.ToString())
                            {
                                DB.AddInParameter(DbCommand, BusinessConstants.FORM_ROUTE, System.Data.DbType.String, FormDetails.Airlines[AirlineCnt].Route);
                                DB.AddInParameter(DbCommand, BusinessConstants.FORM_DIRECTION, System.Data.DbType.String, FormDetails.Airlines[AirlineCnt].Direction);
                            }
                            else if (Instance == BusinessConstants.Instance.EUR.ToString())
                            {
                                DB.AddInParameter(DbCommand, BusinessConstants.FLIGHTTYPE, System.Data.DbType.String, FormDetails.Airlines[AirlineCnt].FlightType);
                            }
                            else if (Instance == BusinessConstants.Instance.AIR.ToString())
                            {
                                DB.AddInParameter(DbCommand, BusinessConstants.AIRCRAFTTYPE, System.Data.DbType.String, FormDetails.Airlines[AirlineCnt].AircraftType);
                            }
                            for (int LanguageCnt = 0; LanguageCnt < FormDetails.Airlines[AirlineCnt].Languages.Length; LanguageCnt++)
                            {
                                DB.AddInParameter(DbCommand, string.Format(BusinessConstants.FORM_LANGAUGE, LanguageCnt + 1), System.Data.DbType.Int32, FormDetails.Airlines[AirlineCnt].Languages[LanguageCnt].LanguageId);
                                DB.AddInParameter(DbCommand, string.Format(BusinessConstants.FORM_STARTCODE, LanguageCnt + 1), System.Data.DbType.Int64, FormDetails.Airlines[AirlineCnt].Languages[LanguageCnt].FirstSerialNo);
                                DB.AddInParameter(DbCommand, string.Format(BusinessConstants.FORM_ENDCODE, LanguageCnt + 1), System.Data.DbType.Int64, FormDetails.Airlines[AirlineCnt].Languages[LanguageCnt].LastSerialNo);
                            }
                            DB.AddOutParameter(DbCommand, "@" + BusinessConstants.FORM_RETVALUE, System.Data.DbType.Int32, 32);
                            DB.AddOutParameter(DbCommand, "@" + BusinessConstants.FORM_ISCARDNOVALID, System.Data.DbType.Boolean, 1);
                            DB.AddOutParameter(DbCommand, "@" + BusinessConstants.FORM_INVALIDLANGUAGES, System.Data.DbType.String, 100);
                            DB.ExecuteNonQuery(DbCommand);
                            int FormId = Convert.ToInt32(DB.GetParameterValue(DbCommand, "@" + BusinessConstants.FORM_RETVALUE));
                            bool IsCardNOValid = Convert.ToBoolean(DB.GetParameterValue(DbCommand, "@" + BusinessConstants.FORM_ISCARDNOVALID));
                            string InValidLanguages = DB.GetParameterValue(DbCommand, "@" + BusinessConstants.FORM_INVALIDLANGUAGES).ToString();
                            if (FormId <= 0)
                            {
                                SICTLogger.WriteWarning(DataAccessLayer.CLASS_NAME, "SetFormDetails", "Inserting Form Details into the DataBase Failed");
                                TempAirlineDetail.IsSuccess = false;
                            }
                            else
                            {
                                SICTLogger.WriteVerbose(DataAccessLayer.CLASS_NAME, "SetFormDetails", "Inserting Form Details into the DataBase Successful");
                                TempAirlineDetail.IsSuccess = true;
                                TempAirlineDetail.IsSerialNoValid = true;
                            }
                            if (!IsCardNOValid)
                            {
                                TempAirlineDetail.IsSerialNoValid = false;
                                if (!string.IsNullOrEmpty(InValidLanguages))
                                {
                                    char[] CommaSeparators = new char[]
                                    {
                                        ','
                                    };
                                    InValidLanguageList = InValidLanguages.Split(CommaSeparators, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
                                }
                                TempAirlineDetail.InvalidLanguages = InValidLanguageList;
                            }
                            TempAirlineDetail.FormId = FormId;
                            AirlineDetails.Add(TempAirlineDetail);
                        }
                        else
                        {
                            TempAirlineDetail.IsSerialNoValid = false;
                            TempAirlineDetail.InvalidLanguages = InValidLanguageList;
                            AirlineDetails.Add(TempAirlineDetail);
                        }
                    }
                    FormSubmitDetails.AirlineDetails = AirlineDetails;
                }
            }
            catch (Exception Ex)
            {
                FormSubmitDetails.ReturnCode = -1;
                FormSubmitDetails.ReturnMessage = "Error in API Execution";
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "SetFormDetails", Ex);
            }
            SICTLogger.WriteInfo(DataAccessLayer.CLASS_NAME, "SetFormDetails", "End ");
            return FormSubmitDetails;
        }

        public FormSubmitDetails UpdateFormDetails(string Instance, string SessionId, FormDetails FormDetails)
        {
            SICTLogger.WriteInfo(DataAccessLayer.CLASS_NAME, "UpdateFormDetails", "Start ");
            FormSubmitDetails FormSubmitDetails = new FormSubmitDetails();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                List<AirlineDetail> AirlineDetails = new List<AirlineDetail>();
                using (DB.CreateConnection())
                {
                    DateTime Dt = DateTime.Now;
                    try
                    {
                        List<int> DateDetails = new List<int>(Array.ConvertAll<string, int>(FormDetails.FieldWorkDate.Split(new char[]
                        {
                            '/'
                        }), new Converter<string, int>(int.Parse)));
                        if (DateDetails.Count == 3)
                        {
                            Dt = new DateTime(DateDetails[2], DateDetails[0], DateDetails[1]);
                        }
                    }
                    catch (Exception Ex)
                    {
                    }
                    for (int AirlineCnt = 0; AirlineCnt < FormDetails.Airlines.Length; AirlineCnt++)
                    {
                        AirlineDetail TempAirlineDetail = new AirlineDetail();
                        TempAirlineDetail.AirlineId = FormDetails.Airlines[AirlineCnt].AirlineId;
                        List<string> InValidLanguageList = new List<string>();
                        bool IsSerialNosValid = this.CheckisSerialNosValid(FormDetails.Airlines[AirlineCnt].Languages, ref InValidLanguageList);
                        if (IsSerialNosValid)
                        {
                            System.Data.Common.DbCommand DbCommand = DB.GetStoredProcCommand("UpdateFormDetails");
                            DB.AddInParameter(DbCommand, BusinessConstants.FORM_FORMID, System.Data.DbType.Int32, FormDetails.FormId);
                            DB.AddInParameter(DbCommand, BusinessConstants.SESSIONID, System.Data.DbType.String, SessionId);
                            DB.AddInParameter(DbCommand, BusinessConstants.AIRLINEID, System.Data.DbType.Int32, FormDetails.Airlines[AirlineCnt].AirlineId);
                            DB.AddInParameter(DbCommand, BusinessConstants.FORM_DISTRIBUTIONDATE, System.Data.DbType.Date, Dt);
                            DB.AddInParameter(DbCommand, BusinessConstants.FORM_INTERVIEWERID, System.Data.DbType.Int32, FormDetails.InterviewerId);
                            DB.AddInParameter(DbCommand, BusinessConstants.FORM_TYPE, System.Data.DbType.String, FormDetails.IsDepartureForm ? BusinessConstants.FORM_TYPE_DEPARTURE : BusinessConstants.FORM_TYPE_ARRIVAL);
                            DB.AddInParameter(DbCommand, BusinessConstants.FORM_DESTINATIONID, System.Data.DbType.Int32, FormDetails.Airlines[AirlineCnt].DestinationId);
                            DB.AddInParameter(DbCommand, BusinessConstants.FORM_ORIGINID, System.Data.DbType.Int32, FormDetails.Airlines[AirlineCnt].OriginId);
                            DB.AddInParameter(DbCommand, BusinessConstants.FORM_FLIGHTNUMBER, System.Data.DbType.String, FormDetails.Airlines[AirlineCnt].FlightNumber);
                            DB.AddInParameter(DbCommand, BusinessConstants.FORM_BUSINESSCARDS, System.Data.DbType.Int32, FormDetails.Airlines[AirlineCnt].BCardsDistributed);
                            DB.AddInParameter(DbCommand, BusinessConstants.FORM_CLIENTCOMMENTS, System.Data.DbType.String, null);
                            if (Instance == BusinessConstants.Instance.US.ToString())
                            {
                                DB.AddInParameter(DbCommand, BusinessConstants.FORM_ROUTE, System.Data.DbType.String, FormDetails.Airlines[AirlineCnt].Route);
                                DB.AddInParameter(DbCommand, BusinessConstants.FORM_DIRECTION, System.Data.DbType.String, FormDetails.Airlines[AirlineCnt].Direction);
                            }
                            else if (Instance == BusinessConstants.Instance.EUR.ToString())
                            {
                                DB.AddInParameter(DbCommand, BusinessConstants.FLIGHTTYPE, System.Data.DbType.String, FormDetails.Airlines[AirlineCnt].FlightType);
                            }
                            else if (Instance == BusinessConstants.Instance.AIR.ToString())
                            {
                                DB.AddInParameter(DbCommand, BusinessConstants.AIRCRAFTTYPE, System.Data.DbType.String, FormDetails.Airlines[AirlineCnt].AircraftType);
                            }
                            for (int LanguageCnt = 0; LanguageCnt < FormDetails.Airlines[AirlineCnt].Languages.Length; LanguageCnt++)
                            {
                                DB.AddInParameter(DbCommand, string.Format(BusinessConstants.FORM_LANGAUGE, LanguageCnt + 1), System.Data.DbType.Int32, FormDetails.Airlines[AirlineCnt].Languages[LanguageCnt].LanguageId);
                                DB.AddInParameter(DbCommand, string.Format(BusinessConstants.FORM_STARTCODE, LanguageCnt + 1), System.Data.DbType.Int64, FormDetails.Airlines[AirlineCnt].Languages[LanguageCnt].FirstSerialNo);
                                DB.AddInParameter(DbCommand, string.Format(BusinessConstants.FORM_ENDCODE, LanguageCnt + 1), System.Data.DbType.Int64, FormDetails.Airlines[AirlineCnt].Languages[LanguageCnt].LastSerialNo);
                            }
                            DB.AddOutParameter(DbCommand, "@" + BusinessConstants.FORM_RETVALUE, System.Data.DbType.Int32, 32);
                            DB.AddOutParameter(DbCommand, "@" + BusinessConstants.FORM_ISCARDNOVALID, System.Data.DbType.Boolean, 1);
                            DB.AddOutParameter(DbCommand, "@" + BusinessConstants.FORM_INVALIDLANGUAGES, System.Data.DbType.String, 100);
                            DB.ExecuteNonQuery(DbCommand);
                            int RetValue = Convert.ToInt32(DB.GetParameterValue(DbCommand, "@" + BusinessConstants.FORM_RETVALUE));
                            bool IsCardNOValid = Convert.ToBoolean(DB.GetParameterValue(DbCommand, "@" + BusinessConstants.FORM_ISCARDNOVALID));
                            string InValidLanguages = DB.GetParameterValue(DbCommand, "@" + BusinessConstants.FORM_INVALIDLANGUAGES).ToString();
                            if (RetValue <= 0)
                            {
                                SICTLogger.WriteWarning(DataAccessLayer.CLASS_NAME, "UpdateFormDetails", "Updating Form Details in the DataBase Failed");
                                TempAirlineDetail.IsSuccess = false;
                            }
                            else
                            {
                                SICTLogger.WriteVerbose(DataAccessLayer.CLASS_NAME, "UpdateFormDetails", "Updating Form Details in the DataBase Successful");
                                TempAirlineDetail.IsSuccess = true;
                                TempAirlineDetail.IsSerialNoValid = true;
                            }
                            if (!IsCardNOValid)
                            {
                                TempAirlineDetail.IsSerialNoValid = false;
                                if (!string.IsNullOrEmpty(InValidLanguages))
                                {
                                    InValidLanguages = InValidLanguages.Remove(InValidLanguages.LastIndexOf(','), 1);
                                    InValidLanguageList = InValidLanguages.Split(new char[]
                                    {
                                        ','
                                    }).ToList<string>();
                                }
                                TempAirlineDetail.InvalidLanguages = InValidLanguageList;
                            }
                            TempAirlineDetail.FormId = FormDetails.FormId;
                            AirlineDetails.Add(TempAirlineDetail);
                        }
                        else
                        {
                            TempAirlineDetail.IsSerialNoValid = false;
                            TempAirlineDetail.InvalidLanguages = InValidLanguageList;
                            AirlineDetails.Add(TempAirlineDetail);
                        }
                    }
                    FormSubmitDetails.AirlineDetails = AirlineDetails;
                }
            }
            catch (Exception Ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "UpdateFormDetails", Ex);
            }
            SICTLogger.WriteInfo(DataAccessLayer.CLASS_NAME, "UpdateFormDetails", "End ");
            return FormSubmitDetails;
        }

        public FormSubmitDetails UpdateFormDetailsForSpecialUser(string Instance, string SessionId, FormDetails FormDetails)
        {
            SICTLogger.WriteInfo(DataAccessLayer.CLASS_NAME, "UpdateFormDetailsForSpecialUser", "Start ");
            FormSubmitDetails FormSubmitDetails = new FormSubmitDetails();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                List<AirlineDetail> AirlineDetails = new List<AirlineDetail>();
                using (DB.CreateConnection())
                {
                    DateTime Dt = DateTime.Now;
                    try
                    {
                        List<int> DateDetails = new List<int>(Array.ConvertAll<string, int>(FormDetails.FieldWorkDate.Split(new char[]
                        {
                            '/'
                        }), new Converter<string, int>(int.Parse)));
                        if (DateDetails.Count == 3)
                        {
                            Dt = new DateTime(DateDetails[2], DateDetails[0], DateDetails[1]);
                        }
                    }
                    catch (Exception Ex)
                    {
                    }
                    for (int AirlineCnt = 0; AirlineCnt < FormDetails.Airlines.Length; AirlineCnt++)
                    {
                        AirlineDetail TempAirlineDetail = new AirlineDetail();
                        TempAirlineDetail.AirlineId = FormDetails.Airlines[AirlineCnt].AirlineId;
                        List<string> InValidLanguageList = new List<string>();
                        bool IsSerialNosValid = this.CheckisSerialNosValid(FormDetails.Airlines[AirlineCnt].Languages, ref InValidLanguageList);
                        if (IsSerialNosValid)
                        {
                            System.Data.Common.DbCommand DbCommand = DB.GetStoredProcCommand("UpdateFormDetailsForSpecialUser");
                            DB.AddInParameter(DbCommand, BusinessConstants.FORM_FORMID, System.Data.DbType.Int32, FormDetails.FormId);
                            DB.AddInParameter(DbCommand, BusinessConstants.AIRPORTID, System.Data.DbType.Int32, FormDetails.AirportId);
                            DB.AddInParameter(DbCommand, BusinessConstants.SESSIONID, System.Data.DbType.String, SessionId);
                            DB.AddInParameter(DbCommand, BusinessConstants.AIRLINEID, System.Data.DbType.Int32, FormDetails.Airlines[AirlineCnt].AirlineId);
                            DB.AddInParameter(DbCommand, BusinessConstants.FORM_DISTRIBUTIONDATE, System.Data.DbType.Date, Dt);
                            DB.AddInParameter(DbCommand, BusinessConstants.FORM_INTERVIEWERID, System.Data.DbType.Int32, FormDetails.InterviewerId);
                            DB.AddInParameter(DbCommand, BusinessConstants.FORM_TYPE, System.Data.DbType.String, FormDetails.IsDepartureForm ? BusinessConstants.FORM_TYPE_DEPARTURE : BusinessConstants.FORM_TYPE_ARRIVAL);
                            DB.AddInParameter(DbCommand, BusinessConstants.FORM_DESTINATIONID, System.Data.DbType.Int32, FormDetails.Airlines[AirlineCnt].DestinationId);
                            DB.AddInParameter(DbCommand, BusinessConstants.FORM_ORIGINID, System.Data.DbType.Int32, FormDetails.Airlines[AirlineCnt].OriginId);
                            DB.AddInParameter(DbCommand, BusinessConstants.FORM_FLIGHTNUMBER, System.Data.DbType.String, FormDetails.Airlines[AirlineCnt].FlightNumber);
                            DB.AddInParameter(DbCommand, BusinessConstants.FORM_BUSINESSCARDS, System.Data.DbType.Int32, FormDetails.Airlines[AirlineCnt].BCardsDistributed);
                            DB.AddInParameter(DbCommand, BusinessConstants.FORM_CLIENTCOMMENTS, System.Data.DbType.String, null);
                            if (Instance == BusinessConstants.Instance.US.ToString())
                            {
                                DB.AddInParameter(DbCommand, BusinessConstants.FORM_ROUTE, System.Data.DbType.String, FormDetails.Airlines[AirlineCnt].Route);
                                DB.AddInParameter(DbCommand, BusinessConstants.FORM_DIRECTION, System.Data.DbType.String, FormDetails.Airlines[AirlineCnt].Direction);
                            }
                            else if (Instance == BusinessConstants.Instance.EUR.ToString())
                            {
                                DB.AddInParameter(DbCommand, BusinessConstants.FLIGHTTYPE, System.Data.DbType.String, FormDetails.Airlines[AirlineCnt].FlightType);
                            }
                            else if (Instance == BusinessConstants.Instance.AIR.ToString())
                            {
                                DB.AddInParameter(DbCommand, BusinessConstants.AIRCRAFTTYPE, System.Data.DbType.String, FormDetails.Airlines[AirlineCnt].AircraftType);
                            }
                            for (int LanguageCnt = 0; LanguageCnt < FormDetails.Airlines[AirlineCnt].Languages.Length; LanguageCnt++)
                            {
                                DB.AddInParameter(DbCommand, string.Format(BusinessConstants.FORM_LANGAUGE, LanguageCnt + 1), System.Data.DbType.Int32, FormDetails.Airlines[AirlineCnt].Languages[LanguageCnt].LanguageId);
                                DB.AddInParameter(DbCommand, string.Format(BusinessConstants.FORM_STARTCODE, LanguageCnt + 1), System.Data.DbType.Int64, FormDetails.Airlines[AirlineCnt].Languages[LanguageCnt].FirstSerialNo);
                                DB.AddInParameter(DbCommand, string.Format(BusinessConstants.FORM_ENDCODE, LanguageCnt + 1), System.Data.DbType.Int64, FormDetails.Airlines[AirlineCnt].Languages[LanguageCnt].LastSerialNo);
                            }
                            DB.AddOutParameter(DbCommand, "@" + BusinessConstants.FORM_RETVALUE, System.Data.DbType.Int32, 32);
                            DB.AddOutParameter(DbCommand, "@" + BusinessConstants.FORM_ISCARDNOVALID, System.Data.DbType.Boolean, 1);
                            DB.AddOutParameter(DbCommand, "@" + BusinessConstants.FORM_INVALIDLANGUAGES, System.Data.DbType.String, 100);
                            DB.ExecuteNonQuery(DbCommand);
                            int RetValue = Convert.ToInt32(DB.GetParameterValue(DbCommand, "@" + BusinessConstants.FORM_RETVALUE));
                            bool IsCardNOValid = Convert.ToBoolean(DB.GetParameterValue(DbCommand, "@" + BusinessConstants.FORM_ISCARDNOVALID));
                            string InValidLanguages = DB.GetParameterValue(DbCommand, "@" + BusinessConstants.FORM_INVALIDLANGUAGES).ToString();
                            if (RetValue <= 0)
                            {
                                SICTLogger.WriteWarning(DataAccessLayer.CLASS_NAME, "UpdateFormDetailsForSpecialUser", "Updating Form Details in the DataBase Failed");
                                TempAirlineDetail.IsSuccess = false;
                            }
                            else
                            {
                                SICTLogger.WriteVerbose(DataAccessLayer.CLASS_NAME, "UpdateFormDetailsForSpecialUser", "Updating Form Details in the DataBase Successful");
                                TempAirlineDetail.IsSuccess = true;
                                TempAirlineDetail.IsSerialNoValid = true;
                            }
                            if (!IsCardNOValid)
                            {
                                TempAirlineDetail.IsSerialNoValid = false;
                                if (!string.IsNullOrEmpty(InValidLanguages))
                                {
                                    InValidLanguages = InValidLanguages.Remove(InValidLanguages.LastIndexOf(','), 1);
                                    InValidLanguageList = InValidLanguages.Split(new char[]
                                    {
                                        ','
                                    }).ToList<string>();
                                }
                                TempAirlineDetail.InvalidLanguages = InValidLanguageList;
                            }
                            TempAirlineDetail.FormId = FormDetails.FormId;
                            AirlineDetails.Add(TempAirlineDetail);
                        }
                        else
                        {
                            TempAirlineDetail.IsSerialNoValid = false;
                            TempAirlineDetail.InvalidLanguages = InValidLanguageList;
                            AirlineDetails.Add(TempAirlineDetail);
                        }
                    }
                    FormSubmitDetails.AirlineDetails = AirlineDetails;
                }
            }
            catch (Exception Ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "UpdateFormDetailsForSpecialUser", Ex);
            }
            SICTLogger.WriteInfo(DataAccessLayer.CLASS_NAME, "UpdateFormDetailsForSpecialUser", "End ");
            return FormSubmitDetails;
        }

        public bool DeleteFormDetails(int FormId)
        {
            SICTLogger.WriteInfo(DataAccessLayer.CLASS_NAME, "DeleteFormDetails", "Start ");
            bool IsSuccess = true;
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                using (DB.CreateConnection())
                {
                    System.Data.Common.DbCommand DbCommand = DB.GetStoredProcCommand("DeleteFormDetails");
                    DB.AddInParameter(DbCommand, BusinessConstants.FORM_FORMID, System.Data.DbType.Int32, FormId);
                    DB.AddOutParameter(DbCommand, "@" + BusinessConstants.FORM_RETVALUE, System.Data.DbType.Int32, 32);
                    DB.ExecuteNonQuery(DbCommand);
                    int RetValue = Convert.ToInt32(DB.GetParameterValue(DbCommand, "@" + BusinessConstants.FORM_RETVALUE));
                    if (RetValue <= 0)
                    {
                        SICTLogger.WriteWarning(DataAccessLayer.CLASS_NAME, "DeleteFormDetails", "Deleting Form Entry Failed in the DataBase ");
                        IsSuccess = false;
                    }
                    else
                    {
                        SICTLogger.WriteVerbose(DataAccessLayer.CLASS_NAME, "DeleteFormDetails", "Deleting Form Entry in the DataBase Successful");
                    }
                }
            }
            catch (Exception Ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "DeleteFormDetails", Ex);
            }
            SICTLogger.WriteInfo(DataAccessLayer.CLASS_NAME, "DeleteFormDetails", "End ");
            return IsSuccess;
        }

        private bool CheckisSerialNosValid(Language[] Languages, ref List<string> InValidLanguageList)
        {
            bool IsSerialNosValid = true;
            try
            {
                List<long> SerialNos = new List<long>();
                for (int i = 0; i < Languages.Length; i++)
                {
                    Language Language = Languages[i];
                    bool IsLanguageAdded = false;
                    for (long StartNo = Language.FirstSerialNo; StartNo <= Language.LastSerialNo; StartNo += 1L)
                    {
                        if (SerialNos.Contains(StartNo) && !IsLanguageAdded)
                        {
                            InValidLanguageList.Add(Language.LanguageId.ToString());
                            IsLanguageAdded = true;
                        }
                        SerialNos.Add(StartNo);
                    }
                }
                if (InValidLanguageList.Count > 0)
                {
                    IsSerialNosValid = false;
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "CheckisSerialNosValid", ex);
            }
            return IsSerialNosValid;
        }

        public System.Data.DataSet GetAirportLoginandInterviewerList()
        {
            System.Data.DataSet DSResult = new System.Data.DataSet();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetAirportLoginandInterviewerList");
                DSResult = DB.ExecuteDataSet(ObjCommand);
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetAirportLoginandInterviewerList", ex);
            }
            return DSResult;
        }

        public System.Data.DataSet GetAirportLoginandAirportList()
        {
            System.Data.DataSet DSResult = new System.Data.DataSet();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetAirportLoginandAirportList");
                DSResult = DB.ExecuteDataSet(ObjCommand);
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetAirportLoginandAirportList", ex);
            }
            return DSResult;
        }

        public System.Data.DataSet GetAdminAirportList()
        {
            System.Data.DataSet DSResult = new System.Data.DataSet();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetAdminLoginAndAirportList");
                DSResult = DB.ExecuteDataSet(ObjCommand);
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetAdminAirportList", ex);
            }
            return DSResult;
        }

        public System.Data.DataSet GetAirportandAirlinetList()
        {
            System.Data.DataSet DSResult = new System.Data.DataSet();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetAirportandAirlineDetails");
                DSResult = DB.ExecuteDataSet(ObjCommand);
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetAirportLoginandAirportList", ex);
            }
            return DSResult;
        }

        public System.Data.DataSet GetAirlineDetailsForSpecialUser(int AirportId, int AirportLoginId)
        {
            System.Data.DataSet DSResult = new System.Data.DataSet();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetAirlineDetailsForSpecialUser");
                DB.AddInParameter(ObjCommand, BusinessConstants.AIRPORTID, System.Data.DbType.Int32, AirportId);
                DB.AddInParameter(ObjCommand, "AirportLoginId", System.Data.DbType.Int32, AirportLoginId);
                DSResult = DB.ExecuteDataSet(ObjCommand);
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetAirlineDetailsForSpecialUser", ex);
            }
            return DSResult;
        }

        public System.Data.DataTable GetAllAirports()
        {
            System.Data.DataTable DtResult = new System.Data.DataTable();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetAllAirports");
                System.Data.DataSet DS = DB.ExecuteDataSet(ObjCommand);
                if (DS.Tables.Count > 0)
                {
                    DtResult = DS.Tables[0];
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetAllAirports", ex);
            }
            return DtResult;
        }

        public System.Data.DataTable GetAllAirportLogins()
        {
            System.Data.DataTable DtResult = new System.Data.DataTable();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetAllAirportLogins");
                System.Data.DataSet DS = DB.ExecuteDataSet(ObjCommand);
                if (DS.Tables.Count > 0)
                {
                    DtResult = DS.Tables[0];
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetAllAirportLogins", ex);
            }
            return DtResult;
        }

        public System.Data.DataTable GetLoginIdFromAirportId(int AirportId)
        {
            System.Data.DataTable DtResult = new System.Data.DataTable();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetLoginIdFromAirportId");
                DB.AddInParameter(ObjCommand, BusinessConstants.AIRPORTID, System.Data.DbType.Int32, AirportId);
                System.Data.DataSet DS = DB.ExecuteDataSet(ObjCommand);
                if (DS.Tables.Count > 0)
                {
                    DtResult = DS.Tables[0];
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetLoginIdFromAirportId", ex);
            }
            return DtResult;
        }

        public System.Data.DataTable GetAllLanguages()
        {
            System.Data.DataTable DTResult = new System.Data.DataTable();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetAllLanguages");
                System.Data.DataSet DS = DB.ExecuteDataSet(ObjCommand);
                if (DS.Tables.Count > 0)
                {
                    DTResult = DS.Tables[0];
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetAllLanguages", ex);
            }
            return DTResult;
        }

        public System.Data.DataSet GetAllRoutesAndDirections()
        {
            System.Data.DataSet DS = new System.Data.DataSet();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetAllRoutesAndDirections");
                DS = DB.ExecuteDataSet(ObjCommand);
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetAllRoutesAndDirections", ex);
            }
            return DS;
        }

        public System.Data.DataTable GetAllFlightTypes()
        {
            System.Data.DataTable Dt = new System.Data.DataTable();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetAllFlightTypes");
                System.Data.DataSet DS = DB.ExecuteDataSet(ObjCommand);
                if (DS.Tables.Count > 0)
                {
                    Dt = DS.Tables[0];
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetAllFlightTypes", ex);
            }
            return Dt;
        }

        public System.Data.DataTable GetAllAircraftTypes()
        {
            System.Data.DataTable Dt = new System.Data.DataTable();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetAllAircraftTypes");
                System.Data.DataSet DS = DB.ExecuteDataSet(ObjCommand);
                if (DS.Tables.Count > 0)
                {
                    Dt = DS.Tables[0];
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetAllAircraftTypes", ex);
            }
            return Dt;
        }

        public System.Data.DataTable GetAdminOriginandDestinations(int UserId, bool IsAirportAdmin)
        {
            System.Data.DataSet DS = new System.Data.DataSet();
            System.Data.DataTable DtRet = new System.Data.DataTable();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetAdminOriginandDestinations");
                if (IsAirportAdmin)
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.USERID, System.Data.DbType.Int32, UserId);
                }
                DS = DB.ExecuteDataSet(ObjCommand);
                if (DS.Tables.Count > 0)
                {
                    DtRet = DS.Tables[0];
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetAdminOriginandDestinations", ex);
            }
            return DtRet;
        }

        public System.Data.DataTable GetAdminAirlines(int UserId, bool IsAirportAdmin)
        {
            System.Data.DataSet DS = new System.Data.DataSet();
            System.Data.DataTable DtRet = new System.Data.DataTable();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetAdminAirlines");
                if (IsAirportAdmin)
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.USERID, System.Data.DbType.Int32, UserId);
                }
                DS = DB.ExecuteDataSet(ObjCommand);
                if (DS.Tables.Count > 0)
                {
                    DtRet = DS.Tables[0];
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetAdminAirlines", ex);
            }
            return DtRet;
        }

        public System.Data.DataTable GetAllAirportAdminLogins()
        {
            System.Data.DataSet DS = new System.Data.DataSet();
            System.Data.DataTable DtRet = new System.Data.DataTable();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetAllAirportAdminLogins");
                DS = DB.ExecuteDataSet(ObjCommand);
                if (DS.Tables.Count > 0)
                {
                    DtRet = DS.Tables[0];
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetAllAirportAdminLogins", ex);
            }
            return DtRet;
        }

        public System.Data.DataTable GetAllInterviewers()
        {
            System.Data.DataTable DTResult = new System.Data.DataTable();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetAllInterviewers");
                System.Data.DataSet DS = DB.ExecuteDataSet(ObjCommand);
                if (DS.Tables.Count > 0)
                {
                    DTResult = DS.Tables[0];
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetAllInterviewers", ex);
            }
            return DTResult;
        }

        public ReturnValue InsertInterviewer(int AirportId, string InterviewerName, bool IsActive)
        {
            ReturnValue ReturnValue = new ReturnValue();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                using (DB.CreateConnection())
                {
                    System.Data.Common.DbCommand DbCommand = DB.GetStoredProcCommand("InsertInterviewer");
                    DB.AddInParameter(DbCommand, BusinessConstants.AIRPORTID, System.Data.DbType.Int32, AirportId);
                    DB.AddInParameter(DbCommand, BusinessConstants.INTERVIEWERNAME, System.Data.DbType.String, InterviewerName);
                    DB.AddInParameter(DbCommand, BusinessConstants.ISACTIVE, System.Data.DbType.Boolean, IsActive);
                    DB.AddOutParameter(DbCommand, "@" + BusinessConstants.FORM_RETVALUE, System.Data.DbType.Int32, 32);
                    DB.AddOutParameter(DbCommand, "@" + BusinessConstants.DUPLICATE, System.Data.DbType.Boolean, 0);
                    DB.ExecuteNonQuery(DbCommand);
                    int RetValue = Convert.ToInt32(DB.GetParameterValue(DbCommand, "@" + BusinessConstants.FORM_RETVALUE));
                    if (!Convert.ToBoolean(DB.GetParameterValue(DbCommand, "@" + BusinessConstants.DUPLICATE)))
                    {
                        if (RetValue <= 0)
                        {
                            SICTLogger.WriteWarning(DataAccessLayer.CLASS_NAME, "InsertInterviewer", "Inserting Interviewer Failed in the DataBase ");
                            ReturnValue.ReturnCode = -1;
                            ReturnValue.ReturnMessage = "Inserting Interviewer Failed in the DataBase";
                        }
                        else
                        {
                            SICTLogger.WriteVerbose(DataAccessLayer.CLASS_NAME, "InsertInterviewer", "Inserting Interviewer in the DataBase Successful");
                            ReturnValue.ReturnCode = 1;
                            ReturnValue.ReturnMessage = "Inserting Interviewer in the DataBase Successful";
                        }
                    }
                    else
                    {
                        SICTLogger.WriteVerbose(DataAccessLayer.CLASS_NAME, "InsertInterviewer", "Duplicate Entry");
                        ReturnValue.ReturnCode = 4;
                        ReturnValue.ReturnMessage = "Duplicate Entry";
                    }
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "InsertInterviewer", ex);
                ReturnValue.ReturnCode = -1;
                ReturnValue.ReturnMessage = "Error in the DataBase";
            }
            return ReturnValue;
        }

        public ReturnValue UpdateInterviewer(int AirportId, int InterviewerId, string InterviewerName, bool IsActive)
        {
            ReturnValue ReturnValue = new ReturnValue();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                using (DB.CreateConnection())
                {
                    System.Data.Common.DbCommand DbCommand = DB.GetStoredProcCommand("UpdateInterviewer");
                    DB.AddInParameter(DbCommand, BusinessConstants.AIRPORTID, System.Data.DbType.Int32, AirportId);
                    DB.AddInParameter(DbCommand, BusinessConstants.INTERVIEWERID, System.Data.DbType.Int32, InterviewerId);
                    DB.AddInParameter(DbCommand, BusinessConstants.INTERVIEWERNAME, System.Data.DbType.String, InterviewerName);
                    DB.AddInParameter(DbCommand, BusinessConstants.ISACTIVE, System.Data.DbType.Boolean, IsActive);
                    DB.AddOutParameter(DbCommand, "@" + BusinessConstants.FORM_RETVALUE, System.Data.DbType.Int32, 32);
                    DB.AddOutParameter(DbCommand, "@" + BusinessConstants.DUPLICATE, System.Data.DbType.Boolean, 0);
                    DB.ExecuteNonQuery(DbCommand);
                    int RetValue = Convert.ToInt32(DB.GetParameterValue(DbCommand, "@" + BusinessConstants.FORM_RETVALUE));
                    if (!Convert.ToBoolean(DB.GetParameterValue(DbCommand, "@" + BusinessConstants.DUPLICATE)))
                    {
                        if (RetValue <= 0)
                        {
                            SICTLogger.WriteWarning(DataAccessLayer.CLASS_NAME, "UpdateInterviewer", "Updating Interviewer Details Failed in the DataBase ");
                            ReturnValue.ReturnCode = -1;
                            ReturnValue.ReturnMessage = "Updating Interviewer Details Failed in the DataBase";
                        }
                        else
                        {
                            SICTLogger.WriteVerbose(DataAccessLayer.CLASS_NAME, "UpdateInterviewer", "Updating Interviewer Details in the DataBase Successful");
                            ReturnValue.ReturnCode = 1;
                            ReturnValue.ReturnMessage = "Updating Interviewer Details in the DataBase Successful";
                        }
                    }
                    else
                    {
                        SICTLogger.WriteVerbose(DataAccessLayer.CLASS_NAME, "UpdateInterviewer", "Duplicate Entry");
                        ReturnValue.ReturnCode = 4;
                        ReturnValue.ReturnMessage = "Duplicate Entry";
                    }
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "UpdateInterviewer", ex);
                ReturnValue.ReturnCode = -1;
                ReturnValue.ReturnMessage = "Error in the DataBase";
            }
            return ReturnValue;
        }

        public ReturnValue InsertTarget(string Instance, int AirlineId, int OriginId, int Target, string Type, string Route, string Direction, string FlightType, string AircraftType)
        {
            ReturnValue ReturnValue = new ReturnValue();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                using (DB.CreateConnection())
                {
                    System.Data.Common.DbCommand DbCommand = DB.GetStoredProcCommand("InsertTarget");
                    DB.AddInParameter(DbCommand, BusinessConstants.AIRLINEID, System.Data.DbType.Int32, AirlineId);
                    DB.AddInParameter(DbCommand, BusinessConstants.ORIGINID, System.Data.DbType.Int32, OriginId);
                    DB.AddInParameter(DbCommand, BusinessConstants.TARGET, System.Data.DbType.Int32, Target);
                    if (Instance == BusinessConstants.Instance.US.ToString())
                    {
                        DB.AddInParameter(DbCommand, BusinessConstants.ROUTE, System.Data.DbType.String, Route);
                        DB.AddInParameter(DbCommand, BusinessConstants.DIRECTION, System.Data.DbType.String, Direction);
                        DB.AddInParameter(DbCommand, BusinessConstants.TYPE, System.Data.DbType.String, Type);
                    }
                    else if (Instance == BusinessConstants.Instance.EUR.ToString())
                    {
                        DB.AddInParameter(DbCommand, BusinessConstants.FLIGHTTYPE, System.Data.DbType.String, FlightType);
                    }
                    else if (Instance == BusinessConstants.Instance.AIR.ToString())
                    {
                        DB.AddInParameter(DbCommand, BusinessConstants.TYPE, System.Data.DbType.String, Type);
                        DB.AddInParameter(DbCommand, BusinessConstants.AIRCRAFTTYPE, System.Data.DbType.String, AircraftType);
                    }
                    DB.AddOutParameter(DbCommand, "@" + BusinessConstants.RETVALUE, System.Data.DbType.Int32, 32);
                    DB.AddOutParameter(DbCommand, "@" + BusinessConstants.DUPLICATE, System.Data.DbType.Boolean, 0);
                    DB.ExecuteNonQuery(DbCommand);
                    int RetValue = Convert.ToInt32(DB.GetParameterValue(DbCommand, "@" + BusinessConstants.FORM_RETVALUE));
                    if (!Convert.ToBoolean(DB.GetParameterValue(DbCommand, "@" + BusinessConstants.DUPLICATE)))
                    {
                        if (RetValue <= 0)
                        {
                            SICTLogger.WriteWarning(DataAccessLayer.CLASS_NAME, "InsertTarget", "Inserting Target Failed in the DataBase ");
                            ReturnValue.ReturnCode = -1;
                            ReturnValue.ReturnMessage = "Inserting Target Failed in the DataBase";
                        }
                        else
                        {
                            SICTLogger.WriteVerbose(DataAccessLayer.CLASS_NAME, "InsertTarget", "Inserting Target in the DataBase Successful");
                            ReturnValue.ReturnCode = 1;
                            ReturnValue.ReturnMessage = "Inserting Target in the DataBase Successful";
                        }
                    }
                    else
                    {
                        SICTLogger.WriteVerbose(DataAccessLayer.CLASS_NAME, "InsertTarget", "Duplicate Entry");
                        ReturnValue.ReturnCode = 4;
                        ReturnValue.ReturnMessage = "Duplicate Entry";
                    }
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "InsertTarget", ex);
                ReturnValue.ReturnCode = -1;
                ReturnValue.ReturnMessage = "Error in the DataBase";
            }
            return ReturnValue;
        }

        public ReturnValue UpdateTarget(List<TargetUpdateDetail> TargetUpdateDetails)
        {
            ReturnValue ReturnValue = new ReturnValue();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                using (DB.CreateConnection())
                {
                    foreach (TargetUpdateDetail TargetUpdateDetail in TargetUpdateDetails)
                    {
                        System.Data.Common.DbCommand DbCommand = DB.GetStoredProcCommand("UpdateTarget");
                        DB.AddInParameter(DbCommand, BusinessConstants.DISTRIBUTIONTARGETID, System.Data.DbType.Int32, TargetUpdateDetail.DistributionTargetId);
                        DB.AddInParameter(DbCommand, BusinessConstants.TARGET, System.Data.DbType.Int32, TargetUpdateDetail.Target);
                        DB.AddOutParameter(DbCommand, "@" + BusinessConstants.FORM_RETVALUE, System.Data.DbType.Int32, 32);
                        DB.ExecuteNonQuery(DbCommand);
                        int RetValue = Convert.ToInt32(DB.GetParameterValue(DbCommand, "@" + BusinessConstants.FORM_RETVALUE));
                        if (RetValue <= 0)
                        {
                            SICTLogger.WriteWarning(DataAccessLayer.CLASS_NAME, "UpdateTarget", "Updating Target Details Failed in the DataBase ");
                            ReturnValue.ReturnCode = -1;
                            ReturnValue.ReturnMessage = "Updating Target Details Failed in the DataBase";
                        }
                        else
                        {
                            SICTLogger.WriteVerbose(DataAccessLayer.CLASS_NAME, "UpdateTarget", "Updating Target Details in the DataBase Successful");
                            ReturnValue.ReturnCode = 1;
                            ReturnValue.ReturnMessage = "Updating Interviewer Target in the DataBase Successful";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "UpdateTarget", ex);
                ReturnValue.ReturnCode = -1;
                ReturnValue.ReturnMessage = "Error in the DataBase";
            }
            return ReturnValue;
        }

        public ReturnValue DeleteTarget(int DistributionTargetId)
        {
            ReturnValue ReturnValue = new ReturnValue();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                using (DB.CreateConnection())
                {
                    System.Data.Common.DbCommand DbCommand = DB.GetStoredProcCommand("DeleteTarget");
                    DB.AddInParameter(DbCommand, BusinessConstants.DISTRIBUTIONTARGETID, System.Data.DbType.Int32, DistributionTargetId);
                    DB.AddOutParameter(DbCommand, "@" + BusinessConstants.FORM_RETVALUE, System.Data.DbType.Int32, 32);
                    DB.ExecuteNonQuery(DbCommand);
                    int RetValue = Convert.ToInt32(DB.GetParameterValue(DbCommand, "@" + BusinessConstants.FORM_RETVALUE));
                    if (RetValue <= 0)
                    {
                        SICTLogger.WriteWarning(DataAccessLayer.CLASS_NAME, "DeleteTarget", "Deleting Target Details Failed in the DataBase ");
                        ReturnValue.ReturnCode = -1;
                        ReturnValue.ReturnMessage = "Deleting Target Details Failed in the DataBase";
                    }
                    else
                    {
                        SICTLogger.WriteVerbose(DataAccessLayer.CLASS_NAME, "DeleteTarget", "Deleting Target Details in the DataBase Successful");
                        ReturnValue.ReturnCode = 1;
                        ReturnValue.ReturnMessage = "Deleting Target in the DataBase Successful";
                    }
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "DeleteTarget", ex);
                ReturnValue.ReturnCode = -1;
                ReturnValue.ReturnMessage = "Error in the DataBase";
            }
            return ReturnValue;
        }

        public System.Data.DataSet GetTargetsByRange(int StartIndex, int Offset, string FormType, string OriginId, string AirlineId, string Route, string Direction, string FlightType, string AircraftType, string SearchText, string OrderByCondition)
        {
            System.Data.DataSet DS = new System.Data.DataSet();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetTargetsByRange");
                DB.AddInParameter(ObjCommand, BusinessConstants.STARTINDEX, System.Data.DbType.Int32, StartIndex);
                DB.AddInParameter(ObjCommand, BusinessConstants.OFFSET, System.Data.DbType.Int32, Offset);
                if (!string.IsNullOrEmpty(OrderByCondition))
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.ORDERBYCONDITION, System.Data.DbType.String, OrderByCondition);
                }
                if (OriginId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.ORIGINID, System.Data.DbType.Int32, Convert.ToInt32(OriginId));
                }
                if (AirlineId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.AIRLINEID, System.Data.DbType.Int32, Convert.ToInt32(AirlineId));
                }
                if (Route != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.ROUTE, System.Data.DbType.String, Route);
                }
                if (Direction != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.DIRECTION, System.Data.DbType.String, Direction);
                }
                if (FlightType != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.FLIGHTTYPE, System.Data.DbType.String, FlightType);
                }
                if (AircraftType != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.AIRCRAFTTYPE, System.Data.DbType.String, AircraftType);
                }
                if (FormType != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.FORM_TYPE, System.Data.DbType.String, FormType);
                }
                if (SearchText != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.SEARCHTEXT, System.Data.DbType.String, SearchText);
                }
                DS = DB.ExecuteDataSet(ObjCommand);
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetTargetsByRange", ex);
            }
            return DS;
        }

        public FlightResponse InserFlightCombination(string Instance, int AirlineId, int OriginId, int DestinationId, string Type, string Route, string Direction, string FlightType)
        {
            FlightResponse ReturnValue = new FlightResponse();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                using (DB.CreateConnection())
                {
                    System.Data.Common.DbCommand DbCommand = DB.GetStoredProcCommand("InserFlightCombination");
                    DB.AddInParameter(DbCommand, BusinessConstants.AIRLINEID, System.Data.DbType.Int32, AirlineId);
                    DB.AddInParameter(DbCommand, BusinessConstants.ORIGINID, System.Data.DbType.Int32, OriginId);
                    DB.AddInParameter(DbCommand, BusinessConstants.DESTINATIONID, System.Data.DbType.Int32, DestinationId);
                    if (Instance == BusinessConstants.Instance.US.ToString())
                    {
                        DB.AddInParameter(DbCommand, BusinessConstants.TYPE, System.Data.DbType.String, Type);
                        DB.AddInParameter(DbCommand, BusinessConstants.ROUTE, System.Data.DbType.String, Route);
                        DB.AddInParameter(DbCommand, BusinessConstants.DIRECTION, System.Data.DbType.String, Direction);
                    }
                    if (Instance == BusinessConstants.Instance.EUR.ToString())
                    {
                        DB.AddInParameter(DbCommand, BusinessConstants.FLIGHTTYPE, System.Data.DbType.String, FlightType);
                    }
                    if (Instance == BusinessConstants.Instance.AIR.ToString())
                    {
                        DB.AddInParameter(DbCommand, BusinessConstants.TYPE, System.Data.DbType.String, Type);
                    }
                    DB.AddOutParameter(DbCommand, "@" + BusinessConstants.FORM_RETVALUE, System.Data.DbType.Int32, 32);
                    DB.AddOutParameter(DbCommand, "@" + BusinessConstants.DUPLICATE, System.Data.DbType.Boolean, 0);
                    DB.AddOutParameter(DbCommand, "@" + BusinessConstants.ISTARGETPRESENT, System.Data.DbType.Boolean, 0);
                    DB.ExecuteNonQuery(DbCommand);
                    bool IsDuplicate = Convert.ToBoolean(DB.GetParameterValue(DbCommand, "@" + BusinessConstants.DUPLICATE));
                    bool IsTargetPresent = Convert.ToBoolean(DB.GetParameterValue(DbCommand, "@" + BusinessConstants.ISTARGETPRESENT));
                    if (!IsDuplicate)
                    {
                        int RetValue = Convert.ToInt32(DB.GetParameterValue(DbCommand, "@" + BusinessConstants.FORM_RETVALUE));
                        if (RetValue <= 0)
                        {
                            SICTLogger.WriteWarning(DataAccessLayer.CLASS_NAME, "InserFlightCombination", "Inserting Flight Combination Failed in the DataBase ");
                            ReturnValue.ReturnCode = -1;
                            ReturnValue.ReturnMessage = "Inserting Flight Combination Failed in the DataBase";
                        }
                        else
                        {
                            SICTLogger.WriteVerbose(DataAccessLayer.CLASS_NAME, "InserFlightCombination", "Inserting Flight Combination in the DataBase Successful");
                            ReturnValue.ReturnCode = 1;
                            ReturnValue.ReturnMessage = "Inserting Flight Combination in the DataBase Successful";
                            ReturnValue.IsTargetPresent = IsTargetPresent;
                        }
                    }
                    else
                    {
                        SICTLogger.WriteVerbose(DataAccessLayer.CLASS_NAME, "InserFlightCombination", "Duplicate Entry");
                        ReturnValue.ReturnCode = 4;
                        ReturnValue.ReturnMessage = "Duplicate Entry";
                    }
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "InserFlightCombination", ex);
                ReturnValue.ReturnCode = -1;
                ReturnValue.ReturnMessage = "Error in the DataBase";
            }
            return ReturnValue;
        }

        public FlightResponse UpdateFlightCombination(string Instance, FlightDetail FlightDetail)
        {
            FlightResponse ReturnValue = new FlightResponse();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                using (DB.CreateConnection())
                {
                    System.Data.Common.DbCommand DbCommand = DB.GetStoredProcCommand("UpdateFlightCombination");
                    DB.AddInParameter(DbCommand, BusinessConstants.FLIGHTCOMBINATIONID, System.Data.DbType.Int32, FlightDetail.FlightCombinationId);
                    DB.AddInParameter(DbCommand, BusinessConstants.AIRLINEID, System.Data.DbType.Int32, FlightDetail.AirlineId);
                    DB.AddInParameter(DbCommand, BusinessConstants.ORIGINID, System.Data.DbType.Int32, FlightDetail.OriginId);
                    DB.AddInParameter(DbCommand, BusinessConstants.DESTINATIONID, System.Data.DbType.Int32, FlightDetail.DestinationId);
                    if (Instance == BusinessConstants.Instance.US.ToString())
                    {
                        DB.AddInParameter(DbCommand, BusinessConstants.TYPE, System.Data.DbType.String, FlightDetail.Type);
                        DB.AddInParameter(DbCommand, BusinessConstants.ROUTE, System.Data.DbType.String, FlightDetail.Route);
                        DB.AddInParameter(DbCommand, BusinessConstants.DIRECTION, System.Data.DbType.String, FlightDetail.Direction);
                    }
                    if (Instance == BusinessConstants.Instance.EUR.ToString())
                    {
                        DB.AddInParameter(DbCommand, BusinessConstants.FLIGHTTYPE, System.Data.DbType.String, FlightDetail.FlightType);
                    }
                    if (Instance == BusinessConstants.Instance.AIR.ToString())
                    {
                        DB.AddInParameter(DbCommand, BusinessConstants.TYPE, System.Data.DbType.String, FlightDetail.Type);
                    }
                    DB.AddOutParameter(DbCommand, "@" + BusinessConstants.FORM_RETVALUE, System.Data.DbType.Int32, 32);
                    DB.AddOutParameter(DbCommand, "@" + BusinessConstants.DUPLICATE, System.Data.DbType.Boolean, 0);
                    DB.AddOutParameter(DbCommand, "@" + BusinessConstants.ISTARGETPRESENT, System.Data.DbType.Boolean, 0);
                    DB.ExecuteNonQuery(DbCommand);
                    int RetValue = Convert.ToInt32(DB.GetParameterValue(DbCommand, "@" + BusinessConstants.FORM_RETVALUE));
                    bool IsDuplicate = Convert.ToBoolean(DB.GetParameterValue(DbCommand, "@" + BusinessConstants.DUPLICATE));
                    bool IsTargetPresent = Convert.ToBoolean(DB.GetParameterValue(DbCommand, "@" + BusinessConstants.ISTARGETPRESENT));
                    if (!IsDuplicate)
                    {
                        if (RetValue <= 0)
                        {
                            SICTLogger.WriteWarning(DataAccessLayer.CLASS_NAME, "UpdateFlightCombination", "Updating FlightCombination Details Failed in the DataBase ");
                            ReturnValue.ReturnCode = -1;
                            ReturnValue.ReturnMessage = "Updating FlightCombination Details Failed in the DataBase";
                        }
                        else
                        {
                            SICTLogger.WriteVerbose(DataAccessLayer.CLASS_NAME, "UpdateFlightCombination", "Updating FlightCombination Details in the DataBase Successful");
                            ReturnValue.ReturnCode = 1;
                            ReturnValue.IsTargetPresent = IsTargetPresent;
                            ReturnValue.ReturnMessage = "Updating FlightCombination  in the DataBase Successful";
                        }
                    }
                    else
                    {
                        SICTLogger.WriteVerbose(DataAccessLayer.CLASS_NAME, "UpdateFlightCombination", "Duplicate Entry");
                        ReturnValue.ReturnCode = 4;
                        ReturnValue.ReturnMessage = "Duplicate Entry";
                    }
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "UpdateFlightCombination", ex);
                ReturnValue.ReturnCode = -1;
                ReturnValue.ReturnMessage = "Error in the DataBase";
            }
            return ReturnValue;
        }

        public FlightDeleteResponse DeleteFlightCombination(int FlightCombinationId)
        {
            FlightDeleteResponse ReturnValue = new FlightDeleteResponse();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                using (DB.CreateConnection())
                {
                    System.Data.Common.DbCommand DbCommand = DB.GetStoredProcCommand("DeleteFlightCombination");
                    DB.AddInParameter(DbCommand, BusinessConstants.FLIGHTCOMBINATIONID, System.Data.DbType.Int32, FlightCombinationId);
                    DB.AddOutParameter(DbCommand, "@" + BusinessConstants.FORM_RETVALUE, System.Data.DbType.Int32, 32);
                    DB.AddOutParameter(DbCommand, "@" + BusinessConstants.ISTARGETDELETEREQUIRED, System.Data.DbType.Boolean, 0);
                    DB.AddOutParameter(DbCommand, "@" + BusinessConstants.TARGETID, System.Data.DbType.Int32, 32);
                    DB.ExecuteNonQuery(DbCommand);
                    bool IsTargetDeleteRequired = Convert.ToBoolean(DB.GetParameterValue(DbCommand, "@" + BusinessConstants.ISTARGETDELETEREQUIRED));
                    int TargetId = Convert.ToInt32(DB.GetParameterValue(DbCommand, "@" + BusinessConstants.TARGETID));
                    int RetValue = Convert.ToInt32(DB.GetParameterValue(DbCommand, "@" + BusinessConstants.FORM_RETVALUE));
                    if (RetValue <= 0)
                    {
                        SICTLogger.WriteWarning(DataAccessLayer.CLASS_NAME, "FlightCombination", "Deleting FlightCombination Details Failed in the DataBase ");
                        ReturnValue.ReturnCode = -1;
                        ReturnValue.ReturnMessage = "Deleting FlightCombination Details Failed in the DataBase";
                    }
                    else
                    {
                        SICTLogger.WriteVerbose(DataAccessLayer.CLASS_NAME, "FlightCombination", "Deleting FlightCombination Details in the DataBase Successful");
                        ReturnValue.ReturnCode = 1;
                        ReturnValue.IsTargetDeleteRequired = IsTargetDeleteRequired;
                        ReturnValue.TargetId = TargetId;
                        ReturnValue.ReturnMessage = "Deleting FlightCombination in the DataBase Successful";
                    }
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "FlightCombination", ex);
                ReturnValue.ReturnCode = -1;
                ReturnValue.ReturnMessage = "Error in the DataBase";
            }
            return ReturnValue;
        }

        public System.Data.DataTable GetAllFlightCombination()
        {
            System.Data.DataTable DTResult = new System.Data.DataTable();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetAllFlightCombination");
                System.Data.DataSet DS = DB.ExecuteDataSet(ObjCommand);
                if (DS.Tables.Count > 0)
                {
                    DTResult = DS.Tables[0];
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetAllFlightCombination", ex);
            }
            return DTResult;
        }

        public System.Data.DataSet GetFlightCombinationByRange(int StartIndex, int Offset, string OrderByCondition, string FilterCondition)
        {
            System.Data.DataSet DS = new System.Data.DataSet();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetFlightCombinationByRange");
                DB.AddInParameter(ObjCommand, BusinessConstants.STARTINDEX, System.Data.DbType.Int32, StartIndex);
                DB.AddInParameter(ObjCommand, BusinessConstants.OFFSET, System.Data.DbType.Int32, Offset);
                if (!string.IsNullOrEmpty(OrderByCondition))
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.ORDERBYCONDITION, System.Data.DbType.String, OrderByCondition);
                }
                if (!string.IsNullOrEmpty(FilterCondition))
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.WHERECONDITION, System.Data.DbType.String, FilterCondition);
                }
                DS = DB.ExecuteDataSet(ObjCommand);
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetFlightCombinationByRange", ex);
            }
            return DS;
        }

        public System.Data.DataSet GetAllTargetandComplete(int Quarter, int Year)
        {
            System.Data.DataSet DSResult = new System.Data.DataSet();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetAllTargetandComplete");
                DB.AddInParameter(ObjCommand, BusinessConstants.QUARTER, System.Data.DbType.Int32, Quarter);
                DB.AddInParameter(ObjCommand, BusinessConstants.YEAR, System.Data.DbType.Int32, Year);
                DSResult = DB.ExecuteDataSet(ObjCommand);
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetAllTargetandComplete", ex);
            }
            return DSResult;
        }

        public System.Data.DataSet GetAllTargetandComplete(string Date1, string Date2)
        {
            System.Data.DataSet DSResult = new System.Data.DataSet();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetAllTargetandComplete");
                DB.AddInParameter(ObjCommand, BusinessConstants.DATE1, System.Data.DbType.String, Date1);
                DB.AddInParameter(ObjCommand, BusinessConstants.DATE2, System.Data.DbType.String, Date2);
                DSResult = DB.ExecuteDataSet(ObjCommand);
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetAllTargetandComplete", ex);
            }
            return DSResult;
        }

        public System.Data.DataSet GetTargetandCompleteforAirportLogin(int AirportLoginId, int Quarter, int Year)
        {
            System.Data.DataSet DSResult = new System.Data.DataSet();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetQuarterlyTargetandCompleteforAirportLogin");
                DB.AddInParameter(ObjCommand, BusinessConstants.AIRPORTLOGINID, System.Data.DbType.Int32, AirportLoginId);
                DB.AddInParameter(ObjCommand, BusinessConstants.QUARTER, System.Data.DbType.Int32, Quarter);
                DB.AddInParameter(ObjCommand, BusinessConstants.YEAR, System.Data.DbType.Int32, Year);
                DSResult = DB.ExecuteDataSet(ObjCommand);
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetTargetandCompleteforAirportLogin", ex);
            }
            return DSResult;
        }

        public System.Data.DataSet GetTargetandCompleteforAirportLoginSpecialUser(int AirportLoginId, int Quarter, int Year, string AirlineIds)
        {
            System.Data.DataSet DSResult = new System.Data.DataSet();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetQuarterlyTargetandCompleteforAirportLoginSpecialUser");
                DB.AddInParameter(ObjCommand, BusinessConstants.AIRPORTLOGINID, System.Data.DbType.Int32, AirportLoginId);
                DB.AddInParameter(ObjCommand, BusinessConstants.QUARTER, System.Data.DbType.Int32, Quarter);
                DB.AddInParameter(ObjCommand, BusinessConstants.YEAR, System.Data.DbType.Int32, Year);
                DB.AddInParameter(ObjCommand, "@AirlineIds", System.Data.DbType.String, AirlineIds);
                DSResult = DB.ExecuteDataSet(ObjCommand);
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetTargetandCompleteforAirportLoginSpecialUser", ex);
            }
            return DSResult;
        }

        public System.Data.DataSet GetTargetandCompleteforAirportLogin(int AirportLoginId, string Date1, string Date2)
        {
            System.Data.DataSet DSResult = new System.Data.DataSet();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetQuarterlyTargetandCompleteforAirportLogin");
                DB.AddInParameter(ObjCommand, BusinessConstants.AIRPORTLOGINID, System.Data.DbType.Int32, AirportLoginId);
                DB.AddInParameter(ObjCommand, BusinessConstants.DATE1, System.Data.DbType.String, Date1);
                DB.AddInParameter(ObjCommand, BusinessConstants.DATE2, System.Data.DbType.String, Date2);
                DSResult = DB.ExecuteDataSet(ObjCommand);
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetTargetandCompleteforAirportLogin", ex);
            }
            return DSResult;
        }

        public System.Data.DataTable GetInterviewerReport(string SessionId, string AirportId, string FilterCondition, string FilterConditionResponse)
        {
            System.Data.DataSet DS = new System.Data.DataSet();
            System.Data.DataTable DtRet = new System.Data.DataTable();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetInterviewerReport");
                if (AirportId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.AIRPORTID, System.Data.DbType.Int32, Convert.ToInt32(AirportId));
                }
                DB.AddInParameter(ObjCommand, BusinessConstants.WHERECONDITION, System.Data.DbType.String, FilterCondition);
                DB.AddInParameter(ObjCommand, BusinessConstants.SESSIONID, System.Data.DbType.String, SessionId);
                DB.AddInParameter(ObjCommand, BusinessConstants.RESPONSEDATEWHERECONDITION, System.Data.DbType.String, FilterConditionResponse);
                DS = DB.ExecuteDataSet(ObjCommand);
                if (DS.Tables.Count > 0)
                {
                    DtRet = DS.Tables[0];
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetInterviewerReport", ex);
            }
            return DtRet;
        }

        public System.Data.DataTable GetInterviewerReportForSpecialUser(string SessionId, string AirportId, string FilterCondition, string FilterConditionResponse)
        {
            System.Data.DataSet DS = new System.Data.DataSet();
            System.Data.DataTable DtRet = new System.Data.DataTable();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetInterviewerReportForSpecialUsers");
                if (AirportId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.AIRPORTID, System.Data.DbType.Int32, Convert.ToInt32(AirportId));
                }
                DB.AddInParameter(ObjCommand, BusinessConstants.WHERECONDITION, System.Data.DbType.String, FilterCondition);
                DB.AddInParameter(ObjCommand, BusinessConstants.SESSIONID, System.Data.DbType.String, SessionId);
                DB.AddInParameter(ObjCommand, BusinessConstants.RESPONSEDATEWHERECONDITION, System.Data.DbType.String, FilterConditionResponse);
                DS = DB.ExecuteDataSet(ObjCommand);
                if (DS.Tables.Count > 0)
                {
                    DtRet = DS.Tables[0];
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetInterviewerReportForSpecialUser", ex);
            }
            return DtRet;
        }

        public System.Data.DataTable GetDODReport(string SessionId, string AirportId, string FilterCondition, string FilterConditionResponse)
        {
            System.Data.DataSet DS = new System.Data.DataSet();
            System.Data.DataTable DtRet = new System.Data.DataTable();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetDODReport");
                if (AirportId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.AIRPORTID, System.Data.DbType.Int32, Convert.ToInt32(AirportId));
                }
                DB.AddInParameter(ObjCommand, BusinessConstants.WHERECONDITION, System.Data.DbType.String, FilterCondition);
                DB.AddInParameter(ObjCommand, BusinessConstants.SESSIONID, System.Data.DbType.String, SessionId);
                DB.AddInParameter(ObjCommand, BusinessConstants.RESPONSEDATEWHERECONDITION, System.Data.DbType.String, FilterConditionResponse);
                DS = DB.ExecuteDataSet(ObjCommand);
                if (DS.Tables.Count > 0)
                {
                    DtRet = DS.Tables[0];
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetDODReport", ex);
            }
            return DtRet;
        }

        public System.Data.DataTable GetDODReportForSpecialUser(string SessionId, string AirportId, string FilterCondition, string FilterConditionResponse)
        {
            System.Data.DataSet DS = new System.Data.DataSet();
            System.Data.DataTable DtRet = new System.Data.DataTable();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetDODReportForSpecialUser");
                if (AirportId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.AIRPORTID, System.Data.DbType.Int32, Convert.ToInt32(AirportId));
                }
                DB.AddInParameter(ObjCommand, BusinessConstants.WHERECONDITION, System.Data.DbType.String, FilterCondition);
                DB.AddInParameter(ObjCommand, BusinessConstants.SESSIONID, System.Data.DbType.String, SessionId);
                DB.AddInParameter(ObjCommand, BusinessConstants.RESPONSEDATEWHERECONDITION, System.Data.DbType.String, FilterConditionResponse);
                DS = DB.ExecuteDataSet(ObjCommand);
                if (DS.Tables.Count > 0)
                {
                    DtRet = DS.Tables[0];
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetDODReport", ex);
            }
            return DtRet;
        }

        public System.Data.DataTable GetFlightReport(string SessionId, string AirportId, string FilterCondition, string FilterConditionResponse)
        {
            System.Data.DataSet DS = new System.Data.DataSet();
            System.Data.DataTable DtRet = new System.Data.DataTable();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetFlightReport");
                if (AirportId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.AIRPORTID, System.Data.DbType.Int32, Convert.ToInt32(AirportId));
                }
                DB.AddInParameter(ObjCommand, BusinessConstants.WHERECONDITION, System.Data.DbType.String, FilterCondition);
                DB.AddInParameter(ObjCommand, BusinessConstants.SESSIONID, System.Data.DbType.String, SessionId);
                DB.AddInParameter(ObjCommand, BusinessConstants.RESPONSEDATEWHERECONDITION, System.Data.DbType.String, FilterConditionResponse);
                DS = DB.ExecuteDataSet(ObjCommand);
                if (DS.Tables.Count > 0)
                {
                    DtRet = DS.Tables[0];
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetFlightReport", ex);
            }
            return DtRet;
        }

        public System.Data.DataTable GetFlightReportForSpecialUser(string SessionId, string AirportId, string FilterCondition, string FilterConditionResponse)
        {
            System.Data.DataSet DS = new System.Data.DataSet();
            System.Data.DataTable DtRet = new System.Data.DataTable();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetFlightReportForSpecialUser");
                if (AirportId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.AIRPORTID, System.Data.DbType.Int32, Convert.ToInt32(AirportId));
                }
                DB.AddInParameter(ObjCommand, BusinessConstants.WHERECONDITION, System.Data.DbType.String, FilterCondition);
                DB.AddInParameter(ObjCommand, BusinessConstants.SESSIONID, System.Data.DbType.String, SessionId);
                DB.AddInParameter(ObjCommand, BusinessConstants.RESPONSEDATEWHERECONDITION, System.Data.DbType.String, FilterConditionResponse);
                DS = DB.ExecuteDataSet(ObjCommand);
                if (DS.Tables.Count > 0)
                {
                    DtRet = DS.Tables[0];
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetFlightReportForSpecialUser", ex);
            }
            return DtRet;
        }

        public System.Data.DataTable GetAirlineReport(string SessionId, string AirportId, string FilterCondition, string TargetFilterCondition, string FilterConditionResponse)
        {
            System.Data.DataSet DS = new System.Data.DataSet();
            System.Data.DataTable DtRet = new System.Data.DataTable();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetAirlineReport");
                ObjCommand.CommandTimeout = 800;

                if (AirportId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.AIRPORTID, System.Data.DbType.Int32, Convert.ToInt32(AirportId));
                }
                DB.AddInParameter(ObjCommand, BusinessConstants.WHERECONDITION, System.Data.DbType.String, FilterCondition);
                if (!string.IsNullOrEmpty(TargetFilterCondition))
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.TARGETCONDITION, System.Data.DbType.String, TargetFilterCondition);
                }
                DB.AddInParameter(ObjCommand, BusinessConstants.SESSIONID, System.Data.DbType.String, SessionId);
                DB.AddInParameter(ObjCommand, BusinessConstants.RESPONSEDATEWHERECONDITION, System.Data.DbType.String, FilterConditionResponse);
                DS = DB.ExecuteDataSet(ObjCommand);
                if (DS.Tables.Count > 0)
                {
                    DtRet = DS.Tables[0];
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetAirlineReport", ex);
            }
            return DtRet;
        }

        public System.Data.DataTable GetAirlineReportForSpecialUser(string SessionId, string AirportId, string FilterCondition, string TargetFilterCondition, string FilterConditionResponse)
        {
            System.Data.DataSet DS = new System.Data.DataSet();
            System.Data.DataTable DtRet = new System.Data.DataTable();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetAirlineReportForSpecialUser");
                if (AirportId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.AIRPORTID, System.Data.DbType.Int32, Convert.ToInt32(AirportId));
                }
                DB.AddInParameter(ObjCommand, BusinessConstants.WHERECONDITION, System.Data.DbType.String, FilterCondition);
                if (!string.IsNullOrEmpty(TargetFilterCondition))
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.TARGETCONDITION, System.Data.DbType.String, TargetFilterCondition);
                }
                DB.AddInParameter(ObjCommand, BusinessConstants.SESSIONID, System.Data.DbType.String, SessionId);
                DB.AddInParameter(ObjCommand, BusinessConstants.RESPONSEDATEWHERECONDITION, System.Data.DbType.String, FilterConditionResponse);
                DS = DB.ExecuteDataSet(ObjCommand);
                if (DS.Tables.Count > 0)
                {
                    DtRet = DS.Tables[0];
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetAirlineReportForSpecialUser", ex);
            }
            return DtRet;
        }

        public System.Data.DataTable GetAllAirlineReport(string SessionId, string AirportId, string FilterCondition, string TargetFilterCondition, string FilterConditionResponse)
        {
            System.Data.DataSet DS = new System.Data.DataSet();
            System.Data.DataTable DtRet = new System.Data.DataTable();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetAllAirlineReport");
                if (AirportId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.AIRPORTID, System.Data.DbType.Int32, Convert.ToInt32(AirportId));
                }
                DB.AddInParameter(ObjCommand, BusinessConstants.WHERECONDITION, System.Data.DbType.String, FilterCondition);
                if (!string.IsNullOrEmpty(TargetFilterCondition))
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.TARGETCONDITION, System.Data.DbType.String, TargetFilterCondition);
                }
                DB.AddInParameter(ObjCommand, BusinessConstants.SESSIONID, System.Data.DbType.String, SessionId);
                DB.AddInParameter(ObjCommand, BusinessConstants.RESPONSEDATEWHERECONDITION, System.Data.DbType.String, FilterConditionResponse);
                DS = DB.ExecuteDataSet(ObjCommand);
                if (DS.Tables.Count > 0)
                {
                    DtRet = DS.Tables[0];
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetAirlineReport", ex);
            }
            return DtRet;
        }

        public System.Data.DataTable GetAircraftReport(string SessionId, string AirportId, string FilterCondition, string TargetFilterCondition, string FilterConditionResponse)
        {
            System.Data.DataSet DS = new System.Data.DataSet();
            System.Data.DataTable DtRet = new System.Data.DataTable();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetAircraftReport_temp");
                if (AirportId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.AIRPORTID, System.Data.DbType.Int32, Convert.ToInt32(AirportId));
                }
                DB.AddInParameter(ObjCommand, BusinessConstants.WHERECONDITION, System.Data.DbType.String, FilterCondition);
                if (!string.IsNullOrEmpty(TargetFilterCondition))
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.TARGETCONDITION, System.Data.DbType.String, TargetFilterCondition);
                }
                DB.AddInParameter(ObjCommand, BusinessConstants.SESSIONID, System.Data.DbType.String, SessionId);
                DB.AddInParameter(ObjCommand, BusinessConstants.RESPONSEDATEWHERECONDITION, System.Data.DbType.String, FilterConditionResponse);
                DS = DB.ExecuteDataSet(ObjCommand);
                if (DS.Tables.Count > 0)
                {
                    DtRet = DS.Tables[0];
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetAircraftReport", ex);
            }
            return DtRet;
        }

        public System.Data.DataSet GetAircraftQuotaReport(string SessionId, string AirportId, string FilterCondition, bool IsBusinessQuota)
        {
            System.Data.DataSet DS = new System.Data.DataSet();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetAircraftQuotaReport");
                if (AirportId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.AIRPORTID, System.Data.DbType.Int32, Convert.ToInt32(AirportId));
                }
                DB.AddInParameter(ObjCommand, BusinessConstants.WHERECONDITION, System.Data.DbType.String, FilterCondition);
                DB.AddInParameter(ObjCommand, "IsBusinessClass", System.Data.DbType.Boolean, IsBusinessQuota);
                DB.AddInParameter(ObjCommand, BusinessConstants.SESSIONID, System.Data.DbType.String, SessionId);
                DS = DB.ExecuteDataSet(ObjCommand);
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetAircraftQuotaReport", ex);
            }
            return DS;
        }

        public System.Data.DataTable GetAirportReport(string SessionId, string AirportId, string FilterCondition, bool IsDeparture, string TargetFilterCondition, string ResponseDateWhereCondition)
        {
            System.Data.DataSet DS = new System.Data.DataSet();
            System.Data.DataTable DtRet = new System.Data.DataTable();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetAirportReport");
                if (AirportId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.AIRPORTID, System.Data.DbType.Int32, Convert.ToInt32(AirportId));
                }
                DB.AddInParameter(ObjCommand, BusinessConstants.WHERECONDITION, System.Data.DbType.String, FilterCondition);
                DB.AddInParameter(ObjCommand, BusinessConstants.ISDEPARTURE, System.Data.DbType.Boolean, IsDeparture);
                DB.AddInParameter(ObjCommand, BusinessConstants.RESPONSEDATEWHERECONDITION, System.Data.DbType.String, ResponseDateWhereCondition);
                if (!string.IsNullOrEmpty(TargetFilterCondition))
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.TARGETCONDITION, System.Data.DbType.String, TargetFilterCondition);
                }
                DB.AddInParameter(ObjCommand, BusinessConstants.SESSIONID, System.Data.DbType.String, SessionId);
                DS = DB.ExecuteDataSet(ObjCommand);
                if (DS.Tables.Count > 0)
                {
                    DtRet = DS.Tables[0];
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetAirportReport", ex);
            }
            return DtRet;
        }

        public System.Data.DataTable GetFormFilterNames(string AirportId, string AirlineId, string InterviewerId, string OriginorDestId)
        {
            System.Data.DataSet DS = new System.Data.DataSet();
            System.Data.DataTable DtRet = new System.Data.DataTable();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetFormFilterNames");
                if (AirportId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.USERID, System.Data.DbType.Int32, Convert.ToInt32(AirportId));
                }
                if (AirlineId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.USERID, System.Data.DbType.Int32, Convert.ToInt32(AirlineId));
                }
                if (InterviewerId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.USERID, System.Data.DbType.Int32, Convert.ToInt32(InterviewerId));
                }
                if (OriginorDestId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    DB.AddInParameter(ObjCommand, "OriginorDestId", System.Data.DbType.Int32, Convert.ToInt32(OriginorDestId));
                }
                DS = DB.ExecuteDataSet(ObjCommand);
                if (DS.Tables.Count > 0)
                {
                    DtRet = DS.Tables[0];
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetFormFilterNames", ex);
            }
            return DtRet;
        }

        public bool CheckIsUploadInProgress()
        {
            bool IsUploadInProgress = false;
            System.Data.DataSet DS = new System.Data.DataSet();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("CheckIsUploadInProgress");
                DS = DB.ExecuteDataSet(ObjCommand);
                if (DS.Tables.Count > 0)
                {
                    IsUploadInProgress = Convert.ToBoolean(DS.Tables[0].Rows[0][0].ToString());
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "CheckIsUploadInProgress", ex);
            }
            return IsUploadInProgress;
        }

        public ReturnValue BulkCopy(System.Data.DataTable Dt, bool IsLastbatch)
        {
            ReturnValue ReturnValue = new ReturnValue();
            try
            {
                using (System.Data.SqlClient.SqlBulkCopy Copy = new System.Data.SqlClient.SqlBulkCopy(ConfigurationManager.ConnectionStrings[BusinessConstants.LOCALDBCONNECTION].ToString()))
                {
                    SICTLogger.WriteWarning(DataAccessLayer.CLASS_NAME, "BulkCopy", "Bulk Copy to DB Start");
                    Copy.BatchSize = Dt.Rows.Count;
                    Copy.NotifyAfter = Dt.Rows.Count;
                    Copy.ColumnMappings.Add(0, 1);
                    Copy.ColumnMappings.Add(1, 2);
                    Copy.ColumnMappings.Add(2, 3);
                    Copy.ColumnMappings.Add(3, 4);
                    Copy.DestinationTableName = "confirmit_data";
                    Copy.WriteToServer(Dt);
                    SICTLogger.WriteWarning(DataAccessLayer.CLASS_NAME, "BulkCopy", "Bulk Copy to DB End");
                }
                ReturnValue.ReturnCode = 1;
                ReturnValue.ReturnMessage = "Upload in DB Successfull";
            }
            catch (Exception ex)
            {
                ReturnValue.ReturnCode = -1;
                ReturnValue.ReturnMessage = "Upload in DB Failed";
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "BulkCopy", ex);
            }
            return ReturnValue;
        }

        public void OnSqlRowsCopied(object sender, System.Data.SqlClient.SqlRowsCopiedEventArgs e)
        {
            try
            {
                SICTLogger.WriteWarning(DataAccessLayer.CLASS_NAME, "OnSqlRowsCopied", "UpdateConfirmitStatus Start");
                int WaitTime = Convert.ToInt32(ConfigurationManager.AppSettings[BusinessConstants.CONFIG_DB_COMMANDTIMEOUT].ToString());
                SqlDatabase DB = new DBUtil().GetDataBase();
                using (DB.CreateConnection())
                {
                    System.Data.Common.DbCommand DbCommand = DB.GetStoredProcCommand("UpdateConfirmitStatus");
                    DbCommand.CommandTimeout = WaitTime;
                    DB.ExecuteNonQuery(DbCommand);
                }
                SICTLogger.WriteWarning(DataAccessLayer.CLASS_NAME, "OnSqlRowsCopied", "UpdateConfirmitStatus End");
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "OnSqlRowsCopied", ex);
            }
        }

        public System.Data.DataSet GetConfirmitData(int StartIndex, int Offset, string OrderByCondition, string FilterCondition)
        {
            System.Data.DataSet DS = new System.Data.DataSet();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetCardsByRange");
                DB.AddInParameter(ObjCommand, BusinessConstants.STARTINDEX, System.Data.DbType.Int32, StartIndex);
                DB.AddInParameter(ObjCommand, BusinessConstants.OFFSET, System.Data.DbType.Int32, Offset);
                if (!string.IsNullOrEmpty(OrderByCondition))
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.ORDERBYCONDITION, System.Data.DbType.String, OrderByCondition);
                }
                if (!string.IsNullOrEmpty(FilterCondition))
                {
                    DB.AddInParameter(ObjCommand, BusinessConstants.WHERECONDITION, System.Data.DbType.String, FilterCondition);
                }
                DS = DB.ExecuteDataSet(ObjCommand);
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetConfirmitCardDetails", ex);
            }
            return DS;
        }

        public System.Data.DataTable GetConfirmitCounts()
        {
            System.Data.DataTable Dt = new System.Data.DataTable();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetConfirmitCounts");
                System.Data.DataSet DS = DB.ExecuteDataSet(ObjCommand);
                if (DS.Tables.Count > 0)
                {
                    Dt = DS.Tables[0];
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetConfirmitCounts", ex);
            }
            return Dt;
        }

        public System.Data.DataTable GetLastUploadDate()
        {
            System.Data.DataTable Dt = new System.Data.DataTable();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetLastUploadDate");
                System.Data.DataSet DS = DB.ExecuteDataSet(ObjCommand);
                if (DS.Tables.Count > 0)
                {
                    Dt = DS.Tables[0];
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetLastUploadDate", ex);
            }
            return Dt;
        }

        public string GetAirportCodeFromAirportId(int AirportId)
        {
            string AirportCode = string.Empty;
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetAirportCodeFromAirportId");
                DB.AddInParameter(ObjCommand, BusinessConstants.AIRPORTID, System.Data.DbType.Int32, AirportId);
                AirportCode = Convert.ToString(DB.ExecuteScalar(ObjCommand));
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetAirportCodeFromAirportId", ex);
            }
            return AirportCode;
        }

        public ReturnValue AddUser(string AirportName, string UserName, string Password, int RoleId, bool IsActive, bool ArrivalFormAccess, bool DepartureFormAccess, bool IsLoginRequired)
        {
            ReturnValue ReturnValue = new ReturnValue();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                using (DB.CreateConnection())
                {
                    System.Data.Common.DbCommand DbCommand = DB.GetStoredProcCommand("AddUser");
                    DB.AddInParameter(DbCommand, BusinessConstants.USER_AIRPORTNAME, System.Data.DbType.String, AirportName);
                    DB.AddInParameter(DbCommand, BusinessConstants.USER_USERNAME, System.Data.DbType.String, UserName);
                    DB.AddInParameter(DbCommand, BusinessConstants.USER_PASSWORD, System.Data.DbType.String, Password);
                    DB.AddInParameter(DbCommand, BusinessConstants.USER_ROLE, System.Data.DbType.Int32, RoleId);
                    DB.AddInParameter(DbCommand, BusinessConstants.USER_ISACTIVE, System.Data.DbType.Boolean, IsActive);
                    DB.AddInParameter(DbCommand, BusinessConstants.USER_ARRIVALFORMACCESS, System.Data.DbType.Boolean, ArrivalFormAccess);
                    DB.AddInParameter(DbCommand, BusinessConstants.USER_DEPARTUREFORMACCESS, System.Data.DbType.Boolean, DepartureFormAccess);
                    DB.AddInParameter(DbCommand, BusinessConstants.USER_ISLOGIN, System.Data.DbType.Boolean, IsLoginRequired);
                    DB.AddOutParameter(DbCommand, "@" + BusinessConstants.RETVALUE, System.Data.DbType.Int32, 32);
                    DB.AddOutParameter(DbCommand, "@" + BusinessConstants.DUPLICATE, System.Data.DbType.Boolean, 0);
                    DB.ExecuteNonQuery(DbCommand);
                    int RetValue = Convert.ToInt32(DB.GetParameterValue(DbCommand, "@" + BusinessConstants.RETVALUE));
                    if (!Convert.ToBoolean(DB.GetParameterValue(DbCommand, "@" + BusinessConstants.DUPLICATE)))
                    {
                        if (RetValue <= 0)
                        {
                            SICTLogger.WriteWarning(DataAccessLayer.CLASS_NAME, "AddUser", "Inserting User Failed in the DataBase ");
                            ReturnValue.ReturnCode = -1;
                            ReturnValue.ReturnMessage = "Inserting User Failed in the DataBase";
                        }
                        else
                        {
                            SICTLogger.WriteVerbose(DataAccessLayer.CLASS_NAME, "AddUser", "Inserting User in the DataBase Successful");
                            ReturnValue.ReturnCode = 1;
                            ReturnValue.ReturnMessage = "Inserting User in the DataBase Successful";
                        }
                    }
                    else
                    {
                        SICTLogger.WriteVerbose(DataAccessLayer.CLASS_NAME, "AddUser", "Duplicate Entry");
                        ReturnValue.ReturnCode = 4;
                        ReturnValue.ReturnMessage = "Duplicate Entry";
                    }
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "AddUser", ex);
                ReturnValue.ReturnCode = -1;
                ReturnValue.ReturnMessage = "Error in the DataBase";
            }
            return ReturnValue;
        }

        public ReturnValue UpdateUser(UserDetail UserDetail)
        {
            ReturnValue ReturnValue = new ReturnValue();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                using (DB.CreateConnection())
                {
                    System.Data.Common.DbCommand DbCommand = DB.GetStoredProcCommand("UpdateUser");
                    if (-1 != UserDetail.UserId)
                    {
                        DB.AddInParameter(DbCommand, BusinessConstants.USER_USERID, System.Data.DbType.Int32, UserDetail.UserId);
                    }
                    DB.AddInParameter(DbCommand, BusinessConstants.USER_AIRPORTNAME, System.Data.DbType.String, UserDetail.AirportName);
                    DB.AddInParameter(DbCommand, BusinessConstants.USER_USERNAME, System.Data.DbType.String, UserDetail.UserName);
                    if (!string.IsNullOrEmpty(UserDetail.Password))
                    {
                        DB.AddInParameter(DbCommand, BusinessConstants.USER_PASSWORD, System.Data.DbType.String, UserDetail.Password);
                    }
                    DB.AddInParameter(DbCommand, BusinessConstants.USER_ROLE, System.Data.DbType.Int32, UserDetail.RoleId);
                    DB.AddInParameter(DbCommand, BusinessConstants.AIRPORTID, System.Data.DbType.Int32, UserDetail.AirportId);
                    DB.AddInParameter(DbCommand, BusinessConstants.USER_ISACTIVE, System.Data.DbType.Boolean, UserDetail.IsActive);
                    DB.AddInParameter(DbCommand, BusinessConstants.USER_ARRIVALFORMACCESS, System.Data.DbType.Boolean, UserDetail.ArrivalFormAccess);
                    DB.AddInParameter(DbCommand, BusinessConstants.USER_DEPARTUREFORMACCESS, System.Data.DbType.Boolean, UserDetail.DepartureFormAccess);
                    DB.AddInParameter(DbCommand, BusinessConstants.USER_ISLOGIN, System.Data.DbType.Boolean, UserDetail.IsLogin);
                    DB.AddOutParameter(DbCommand, "@" + BusinessConstants.RETVALUE, System.Data.DbType.Int32, 32);
                    DB.AddOutParameter(DbCommand, "@" + BusinessConstants.DUPLICATE, System.Data.DbType.Boolean, 0);
                    DB.ExecuteNonQuery(DbCommand);
                    int RetValue = Convert.ToInt32(DB.GetParameterValue(DbCommand, "@" + BusinessConstants.FORM_RETVALUE));
                    if (!Convert.ToBoolean(DB.GetParameterValue(DbCommand, "@" + BusinessConstants.DUPLICATE)))
                    {
                        if (RetValue <= 0)
                        {
                            SICTLogger.WriteWarning(DataAccessLayer.CLASS_NAME, "UpdateUser", "Updating User details Failed in the DataBase ");
                            ReturnValue.ReturnCode = -1;
                            ReturnValue.ReturnMessage = "Updating User details  Failed in the DataBase";
                        }
                        else
                        {
                            SICTLogger.WriteVerbose(DataAccessLayer.CLASS_NAME, "UpdateUser", "Updating User details  in the DataBase Successful");
                            ReturnValue.ReturnCode = 1;
                            ReturnValue.ReturnMessage = "Updating User details  in the DataBase Successful";
                        }
                    }
                    else
                    {
                        SICTLogger.WriteVerbose(DataAccessLayer.CLASS_NAME, "UpdateUser", "Duplicate Entry");
                        ReturnValue.ReturnCode = 4;
                        ReturnValue.ReturnMessage = "Duplicate Entry";
                    }
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "UpdateUser", ex);
                ReturnValue.ReturnCode = -1;
                ReturnValue.ReturnMessage = "Error in the DataBase";
            }
            return ReturnValue;
        }

        public System.Data.DataTable GetAllUseDetails()
        {
            System.Data.DataTable Dt = new System.Data.DataTable();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetAllUser");
                System.Data.DataSet DS = DB.ExecuteDataSet(ObjCommand);
                if (DS.Tables.Count > 0)
                {
                    Dt = DS.Tables[0];
                }
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetAllUseDetails", ex);
            }
            return Dt;
        }

        public string GetUserNameBySessionId(string SessionId)
        {
            string UserName = string.Empty;
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetUserNameBySessionId");
                DB.AddInParameter(ObjCommand, BusinessConstants.SESSIONID, System.Data.DbType.String, SessionId);
                UserName = Convert.ToString(DB.ExecuteScalar(ObjCommand));
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetUserNameBySessionId", ex);
            }
            return UserName;
        }

        public string GetAirportIdByUserName(string UserName)
        {
            string AirportId = string.Empty;
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetAirportIdByUserName");
                DB.AddInParameter(ObjCommand, BusinessConstants.USERNAME, System.Data.DbType.String, UserName);
                AirportId = Convert.ToString(DB.ExecuteScalar(ObjCommand));
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetAirportIdByUserName", ex);
            }
            return AirportId;
        }

        public System.Data.DataSet GetAllInterviewersIdBySessionId(string SessionId)
        {
            System.Data.DataSet Ds = new System.Data.DataSet();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("GetAllInterviewersIdBySessionId");
                DB.AddInParameter(ObjCommand, BusinessConstants.SESSIONID, System.Data.DbType.String, SessionId);
                Ds = DB.ExecuteDataSet(ObjCommand);
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "GetAllInterviewersIdBySessionId", ex);
            }
            return Ds;
        }

        public System.Data.DataSet getAirlineNamesBySessionId(string SessionId)
        {
            System.Data.DataSet Ds = new System.Data.DataSet();
            try
            {
                SqlDatabase DB = new DBUtil().GetDataBase();
                System.Data.Common.DbCommand ObjCommand = DB.GetStoredProcCommand("getAirlineNamesBySessionId");
                DB.AddInParameter(ObjCommand, BusinessConstants.SESSIONID, System.Data.DbType.String, SessionId);
                Ds = DB.ExecuteDataSet(ObjCommand);
            }
            catch (Exception ex)
            {
                SICTLogger.WriteException(DataAccessLayer.CLASS_NAME, "getAirlineNamesBySessionId", ex);
            }
            return Ds;
        }
    }
}
