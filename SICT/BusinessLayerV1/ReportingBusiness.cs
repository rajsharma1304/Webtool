using SICT.Constants;
using SICT.DataAccessLayer;
using SICT.DataContracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;

namespace SICT.BusinessLayer.V1
{
    public class ReportingBusiness
    {
        private static readonly string CLASS_NAME = "ReportingBusiness";

        private string BuildMultiFilterConditionForReport(string AirportId, string OriginId, string DestinationId, string FormType, string Route, string Direction, string StartDate, string EndDate, string InterviewerId = "-1", string AirlineId = "-1", string FlightType = "-1", string AircraftType = "-1")
        {
            SICTLogger.WriteInfo(ReportingBusiness.CLASS_NAME, "BuildMultiFilterConditionForReport", "Start For AiprortId -" + AirportId);
            string FilterCondition = string.Empty;
            try
            {
                System.Collections.Generic.List<string> Filters = new System.Collections.Generic.List<string>();
                if (AirlineId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    string Filter = BusinessConstants.TABLE_INDCARDENTRY_AIRLINEID + " in(" + AirlineId + ")";
                    Filters.Add(Filter);
                }
                if (FormType != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    string[] FormTypesplit = FormType.Split(new char[]
                    {
                        ','
                    });
                    string FormTypeList = "'" + string.Join("','", FormTypesplit) + "'";
                    string Filter = BusinessConstants.TABLE_INDCARDENTRY_TYPE + " in(" + FormTypeList + ")";
                    Filters.Add(Filter);
                    if (OriginId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                    {
                        Filter = BusinessConstants.TABLE_INDCARDENTRY_ORIGINID + " in(" + OriginId + ")";
                        Filters.Add(Filter);
                    }
                    if (DestinationId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                    {
                        string[] DestinationIdsplit = DestinationId.Split(new char[]
                        {
                            ','
                        });
                        string DestinationIdList = "'" + string.Join("','", DestinationIdsplit) + "'";
                        Filter = BusinessConstants.TABLE_INDCARDENTRY_DESTINATIONID + " in(" + DestinationIdList + ")";
                        Filters.Add(Filter);
                    }
                }
                else
                {
                    string TempFiltersFormat = string.Empty;
                    string TempFilter = string.Empty;
                    if (OriginId != BusinessConstants.DEFAULT_SELECTION_VALUE && DestinationId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                    {
                        TempFiltersFormat = "(({0}={1} and {2}={3}) or ({0}={3} and {2}={1}))";
                        TempFilter = string.Format(TempFiltersFormat, new object[]
                        {
                            BusinessConstants.TABLE_INDCARDENTRY_ORIGINID,
                            OriginId,
                            BusinessConstants.TABLE_INDCARDENTRY_DESTINATIONID,
                            DestinationId
                        });
                        Filters.Add(TempFilter);
                    }
                    else if (OriginId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                    {
                        TempFiltersFormat = "({0}={2} or {1}={2})";
                        TempFilter = string.Format(TempFiltersFormat, BusinessConstants.TABLE_INDCARDENTRY_ORIGINID, BusinessConstants.TABLE_INDCARDENTRY_DESTINATIONID, OriginId);
                        Filters.Add(TempFilter);
                    }
                    else if (DestinationId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                    {
                        TempFiltersFormat = "({0}={2} or {1}={2})";
                        TempFilter = string.Format(TempFiltersFormat, BusinessConstants.TABLE_INDCARDENTRY_ORIGINID, BusinessConstants.TABLE_INDCARDENTRY_DESTINATIONID, DestinationId);
                        Filters.Add(TempFilter);
                    }
                }
                if (Route != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    string[] Routesplit = Route.Split(new char[]
                    {
                        ','
                    });
                    string RouteList = "'" + string.Join("','", Routesplit) + "'";
                    string Filter = BusinessConstants.TABLE_INDCARDENTRY_ROUTE + " in(" + RouteList + ")";
                    Filters.Add(Filter);
                }
                if (Direction != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    string[] Directionsplit = Direction.Split(new char[]
                    {
                        ','
                    });
                    string DirectionList = "'" + string.Join("','", Directionsplit) + "'";
                    string Filter = BusinessConstants.TABLE_INDCARDENTRY_DIRECTION + " in(" + DirectionList + ")";
                    Filters.Add(Filter);
                }
                if (InterviewerId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    string[] InterviewerIdsplit = InterviewerId.Split(new char[]
                    {
                        ','
                    });
                    string InterviewerIdList = "'" + string.Join("','", InterviewerIdsplit) + "'";
                    string Filter = BusinessConstants.TABLE_INDCARDENTRY_INTERVIEWERID + " in(" + InterviewerIdList + ")";
                    Filters.Add(Filter);
                }
                if (FlightType != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    string[] FlightTypesplit = FlightType.Split(new char[]
                    {
                        ','
                    });
                    string FlightTypeList = "'" + string.Join("','", FlightTypesplit) + "'";
                    string Filter = BusinessConstants.TABLE_INDCARDENTRY_FLIGHTTYPE + " in(" + FlightTypeList + ")";
                    Filters.Add(Filter);
                }
                if (AircraftType != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    string[] AircraftTypesplit = AircraftType.Split(new char[]
                    {
                        ','
                    });
                    string AircraftTypeList = "'" + string.Join("','", AircraftTypesplit) + "'";
                    string Filter = BusinessConstants.TABLE_INDCARDENTRY_AIRCRAFT_TYPE + " in(" + AircraftTypeList + ")";
                    Filters.Add(Filter);
                }
                if (StartDate != BusinessConstants.DEFAULT_SELECTION_VALUE && EndDate != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    string Filter = string.Format(" {0} between '{1}' and '{2}' ", BusinessConstants.TABLE_INDCARDENTRY_DISTRIBUTION_DATE, StartDate, EndDate);
                    Filters.Add(Filter);
                }
                else if (StartDate != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    string Filter = string.Format(" {0} >= '{1}' ", BusinessConstants.TABLE_INDCARDENTRY_DISTRIBUTION_DATE, StartDate);
                    Filters.Add(Filter);
                }
                else if (EndDate != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    string Filter = string.Format(" {0} <= '{1}' ", BusinessConstants.TABLE_INDCARDENTRY_DISTRIBUTION_DATE, EndDate);
                    Filters.Add(Filter);
                }
                if (Filters.Count > 0)
                {
                    FilterCondition = string.Join(" and ", Filters);
                }
                if (string.IsNullOrEmpty(FilterCondition))
                {
                    FilterCondition = string.Format(" {0} > {1} ", BusinessConstants.TABLE_INDCARDENTRY_ID, 0);
                }
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(ReportingBusiness.CLASS_NAME, "BuildMultiFilterConditionForReport", Ex);
            }
            SICTLogger.WriteInfo(ReportingBusiness.CLASS_NAME, "BuildMultiFilterConditionForReport", "End For AiprortId -" + AirportId);
            return FilterCondition;
        }

        private string BuildMultiFilterConditionForResponseDate(string AirportId, string OriginId, string DestinationId, string FormType, string Route, string Direction, string StartDate, string ResponseDate, string InterviewerId = "-1", string AirlineId = "-1", string FlightType = "-1", string AircraftType = "-1")
        {
            SICTLogger.WriteInfo(ReportingBusiness.CLASS_NAME, "BuildMultiFilterConditionForReport", "Start For AiprortId -" + AirportId);
            string FilterCondition = string.Empty;
            try
            {
                System.Collections.Generic.List<string> Filters = new System.Collections.Generic.List<string>();
                if (AirlineId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    string Filter = BusinessConstants.TABLE_INDCARDENTRY_AIRLINEID + " in(" + AirlineId + ")";
                    Filters.Add(Filter);
                }
                if (FormType != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    string[] FormTypesplit = FormType.Split(new char[]
                    {
                        ','
                    });
                    string FormTypeList = "'" + string.Join("','", FormTypesplit) + "'";
                    string Filter = BusinessConstants.TABLE_INDCARDENTRY_TYPE + " in(" + FormTypeList + ")";
                    Filters.Add(Filter);
                    if (OriginId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                    {
                        Filter = BusinessConstants.TABLE_INDCARDENTRY_ORIGINID + " in(" + OriginId + ")";
                        Filters.Add(Filter);
                    }
                    if (DestinationId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                    {
                        string[] DestinationIdsplit = DestinationId.Split(new char[]
                        {
                            ','
                        });
                        string DestinationIdList = "'" + string.Join("','", DestinationIdsplit) + "'";
                        Filter = BusinessConstants.TABLE_INDCARDENTRY_DESTINATIONID + " in(" + DestinationIdList + ")";
                        Filters.Add(Filter);
                    }
                }
                else
                {
                    string TempFiltersFormat = string.Empty;
                    string TempFilter = string.Empty;
                    if (OriginId != BusinessConstants.DEFAULT_SELECTION_VALUE && DestinationId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                    {
                        TempFiltersFormat = "(({0}={1} and {2}={3}) or ({0}={3} and {2}={1}))";
                        TempFilter = string.Format(TempFiltersFormat, new object[]
                        {
                            BusinessConstants.TABLE_INDCARDENTRY_ORIGINID,
                            OriginId,
                            BusinessConstants.TABLE_INDCARDENTRY_DESTINATIONID,
                            DestinationId
                        });
                        Filters.Add(TempFilter);
                    }
                    else if (OriginId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                    {
                        TempFiltersFormat = "({0}={2} or {1}={2})";
                        TempFilter = string.Format(TempFiltersFormat, BusinessConstants.TABLE_INDCARDENTRY_ORIGINID, BusinessConstants.TABLE_INDCARDENTRY_DESTINATIONID, OriginId);
                        Filters.Add(TempFilter);
                    }
                    else if (DestinationId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                    {
                        TempFiltersFormat = "({0}={2} or {1}={2})";
                        TempFilter = string.Format(TempFiltersFormat, BusinessConstants.TABLE_INDCARDENTRY_ORIGINID, BusinessConstants.TABLE_INDCARDENTRY_DESTINATIONID, DestinationId);
                        Filters.Add(TempFilter);
                    }
                }
                if (Route != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    string[] Routesplit = Route.Split(new char[]
                    {
                        ','
                    });
                    string RouteList = "'" + string.Join("','", Routesplit) + "'";
                    string Filter = BusinessConstants.TABLE_INDCARDENTRY_ROUTE + " in(" + RouteList + ")";
                    Filters.Add(Filter);
                }
                if (Direction != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    string[] Directionsplit = Direction.Split(new char[]
                    {
                        ','
                    });
                    string DirectionList = "'" + string.Join("','", Directionsplit) + "'";
                    string Filter = BusinessConstants.TABLE_INDCARDENTRY_DIRECTION + " in(" + DirectionList + ")";
                    Filters.Add(Filter);
                }
                if (InterviewerId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    string[] InterviewerIdsplit = InterviewerId.Split(new char[]
                    {
                        ','
                    });
                    string InterviewerIdList = "'" + string.Join("','", InterviewerIdsplit) + "'";
                    string Filter = BusinessConstants.TABLE_INDCARDENTRY_INTERVIEWERID + " in(" + InterviewerIdList + ")";
                    Filters.Add(Filter);
                }
                if (FlightType != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    string[] FlightTypesplit = FlightType.Split(new char[]
                    {
                        ','
                    });
                    string FlightTypeList = "'" + string.Join("','", FlightTypesplit) + "'";
                    string Filter = BusinessConstants.TABLE_INDCARDENTRY_FLIGHTTYPE + " in(" + FlightTypeList + ")";
                    Filters.Add(Filter);
                }
                if (AircraftType != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    string[] AircraftTypesplit = AircraftType.Split(new char[]
                    {
                        ','
                    });
                    string AircraftTypeList = "'" + string.Join("','", AircraftTypesplit) + "'";
                    string Filter = BusinessConstants.TABLE_INDCARDENTRY_AIRCRAFT_TYPE + " in(" + AircraftTypeList + ")";
                    Filters.Add(Filter);
                }
                if (StartDate != BusinessConstants.DEFAULT_SELECTION_VALUE && ResponseDate != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    string Filter = string.Format(" {0} between '{1}' and '{2}' ", BusinessConstants.TABLE_INDCARDENTRY_DISTRIBUTION_DATE, StartDate, ResponseDate);
                    Filters.Add(Filter);
                }
                else if (StartDate != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    string Filter = string.Format(" {0} >= '{1}' ", BusinessConstants.TABLE_INDCARDENTRY_DISTRIBUTION_DATE, StartDate);
                    Filters.Add(Filter);
                }
                else if (ResponseDate != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    string Filter = string.Format(" {0} <= '{1}' ", BusinessConstants.TABLE_INDCARDENTRY_DISTRIBUTION_DATE, ResponseDate);
                    Filters.Add(Filter);
                }
                if (Filters.Count > 0)
                {
                    FilterCondition = string.Join(" and ", Filters);
                }
                if (string.IsNullOrEmpty(FilterCondition))
                {
                    FilterCondition = string.Format(" {0} > {1} ", BusinessConstants.TABLE_INDCARDENTRY_ID, 0);
                }
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(ReportingBusiness.CLASS_NAME, "BuildMultiFilterConditionForReport", Ex);
            }
            SICTLogger.WriteInfo(ReportingBusiness.CLASS_NAME, "BuildMultiFilterConditionForReport", "End For AiprortId -" + AirportId);
            return FilterCondition;
        }

        public InterviewerReportResponse GetInterviewerReport(string SessionId, string AirportId, string OriginId, string DestinationId, string InterviewerId, string AirlineId, string FormType, string Route, string Direction, string FlightType, string AircraftType, string StartDate, string EndDate, string ResponseDate)
        {
            SICTLogger.WriteInfo(ReportingBusiness.CLASS_NAME, "GetInterviewerReport", "Start for AirportId -" + AirportId);
            InterviewerReportResponse InterviewerReportResponse = new InterviewerReportResponse();
            System.Collections.Generic.List<InterviewerReportDetail> InterviewerReportDetails = new System.Collections.Generic.List<InterviewerReportDetail>();
            UserDetailsBusiness Udb = new UserDetailsBusiness();
            DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
            try
            {
                if (FormType == BusinessConstants.FORM_TYPE_ARRIVAL)
                {
                    string TempId = OriginId;
                    OriginId = DestinationId;
                    DestinationId = TempId;
                }
                string FilterCondition = this.BuildMultiFilterConditionForReport(AirportId, OriginId, DestinationId, FormType, Route, Direction, StartDate, EndDate, InterviewerId, AirlineId, FlightType, AircraftType);
                string FilterConditionResponse = this.BuildMultiFilterConditionForResponseDate(AirportId, OriginId, DestinationId, FormType, Route, Direction, StartDate, ResponseDate, InterviewerId, AirlineId, FlightType, AircraftType);
                DataTable DtReport = new DataTable();
                SICTLogger.WriteVerbose(ReportingBusiness.CLASS_NAME, "GetInterviewerReport", "Start Retieving Interviewer Report Data From DB");
                string UserName = DBLayer.GetUserNameBySessionId(SessionId);
                if (Udb.CheckIsSpecialUserOrNot(UserName))
                {
                    DtReport = DBLayer.GetInterviewerReportForSpecialUser(SessionId, AirportId, FilterCondition, FilterConditionResponse);
                }
                else
                {
                    DtReport = DBLayer.GetInterviewerReport(SessionId, AirportId, FilterCondition, FilterConditionResponse);
                }
                if (DtReport.Rows.Count > 0)
                {
                    foreach (DataRow Dr in DtReport.Rows)
                    {
                        InterviewerReportDetail TempInterviewerReportDetail = new InterviewerReportDetail();
                        int iInterviewerId = System.Convert.ToInt32(Dr[BusinessConstants.REPORT_INTERVIEWERID].ToString());
                        string InterviewerName = Dr[BusinessConstants.REPORT_INTERVIEWERNAME].ToString();
                        int CardsDistributed = 0;
                        int Completes = 0;
                        int BusinessCompletes = 0;
                        int RCardsDistributed = 0;
                        int RCompletes = 0;
                        if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_CARDSDISTRIBUTED].ToString()))
                        {
                            CardsDistributed = System.Convert.ToInt32(Dr[BusinessConstants.REPORT_CARDSDISTRIBUTED].ToString());
                        }
                        if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_COMPLETES].ToString()))
                        {
                            Completes = System.Convert.ToInt32(Dr[BusinessConstants.REPORT_COMPLETES].ToString());
                        }
                        if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_BUSINESSCOMPLETES].ToString()))
                        {
                            BusinessCompletes = System.Convert.ToInt32(Dr[BusinessConstants.REPORT_BUSINESSCOMPLETES].ToString());
                        }
                        double ResponseRate = 0.0;
                        double BResponseRate = 0.0;
                        if (CardsDistributed > 0)
                        {
                            ResponseRate = System.Convert.ToDouble(Completes) / System.Convert.ToDouble(CardsDistributed) * 100.0;
                        }
                        if (ResponseDate != "-1")
                        {
                            if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_CARDSDISTRIBUTEDRESPONSE].ToString()))
                            {
                                RCardsDistributed = System.Convert.ToInt32(Dr[BusinessConstants.REPORT_CARDSDISTRIBUTEDRESPONSE].ToString());
                            }
                            if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_COMPLETESRESPONSE].ToString()))
                            {
                                RCompletes = System.Convert.ToInt32(Dr[BusinessConstants.REPORT_COMPLETESRESPONSE].ToString());
                            }
                            if (RCardsDistributed > 0)
                            {
                                ResponseRate = System.Convert.ToDouble(RCompletes) / System.Convert.ToDouble(RCardsDistributed) * 100.0;
                            }
                        }
                        if (Completes > 0)
                        {
                            BResponseRate = System.Convert.ToDouble(BusinessCompletes) / System.Convert.ToDouble(Completes) * 100.0;
                        }
                        TempInterviewerReportDetail.InterviewerId = iInterviewerId;
                        TempInterviewerReportDetail.InterviewerName = InterviewerName;
                        TempInterviewerReportDetail.Distributed = CardsDistributed;
                        TempInterviewerReportDetail.TotalCompletes = Completes;
                        TempInterviewerReportDetail.BCompletes = BusinessCompletes;
                        TempInterviewerReportDetail.Incompletes = CardsDistributed - Completes;
                        TempInterviewerReportDetail.ResponseRate = ResponseRate.Round();
                        TempInterviewerReportDetail.BResponseRate = BResponseRate.Round();
                        TempInterviewerReportDetail.RCardsDistributed = RCardsDistributed;
                        TempInterviewerReportDetail.RTotalCompletes = RCompletes;
                        InterviewerReportDetails.Add(TempInterviewerReportDetail);
                    }
                }
                InterviewerReportResponse.InterviewerReportDetails = InterviewerReportDetails;
                InterviewerReportResponse.ReturnCode = 1;
                InterviewerReportResponse.ReturnMessage = "Successfully Retrieved";
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(ReportingBusiness.CLASS_NAME, "GetInterviewerReport", Ex);
                InterviewerReportResponse.ReturnCode = -1;
                InterviewerReportResponse.ReturnMessage = "Error in API";
            }
            SICTLogger.WriteInfo(ReportingBusiness.CLASS_NAME, "GetInterviewerReport", "End for AirportId -" + AirportId);
            return InterviewerReportResponse;
        }

        public DODReportResponse GetDODReport(string SessionId, string AirportId, string OriginId, string DestinationId, string AirlineId, string FormType, string Route, string Direction, string FlightType, string AircraftType, string InterviewerId, string StartDate, string EndDate, string ResponseDate)
        {
            SICTLogger.WriteInfo(ReportingBusiness.CLASS_NAME, "GetDODReport", "Start for AirportId -" + AirportId);
            DODReportResponse DODReportResponse = new DODReportResponse();
            System.Collections.Generic.List<DODReportDetail> DODReportDetails = new System.Collections.Generic.List<DODReportDetail>();
            DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
            try
            {
                if (FormType == BusinessConstants.FORM_TYPE_ARRIVAL)
                {
                    string TempId = OriginId;
                    OriginId = DestinationId;
                    DestinationId = TempId;
                }
                string FilterCondition = this.BuildMultiFilterConditionForReport(AirportId, OriginId, DestinationId, FormType, Route, Direction, StartDate, EndDate, InterviewerId, AirlineId, FlightType, AircraftType);
                string FilterConditionResponse = this.BuildMultiFilterConditionForReport(AirportId, OriginId, DestinationId, FormType, Route, Direction, StartDate, ResponseDate, InterviewerId, AirlineId, FlightType, AircraftType);
                DataTable DtReport = new DataTable();
                SICTLogger.WriteVerbose(ReportingBusiness.CLASS_NAME, "GetDODReport", "Start Retieving DOD Report Data From DB");
                UserDetailsBusiness Udb = new UserDetailsBusiness();
                if (!Udb.CheckIsSpecialUserOrNot(DBLayer.GetUserNameBySessionId(SessionId)))
                {
                    DtReport = DBLayer.GetDODReport(SessionId, AirportId, FilterCondition, FilterConditionResponse);
                }
                else
                {
                    DtReport = DBLayer.GetDODReportForSpecialUser(SessionId, AirportId, FilterCondition, FilterConditionResponse);
                }
                if (DtReport.Rows.Count > 0)
                {
                    foreach (DataRow Dr in DtReport.Rows)
                    {
                        DODReportDetail TempDODReportDetail = new DODReportDetail();
                        System.DateTime Date = System.Convert.ToDateTime(Dr[BusinessConstants.REPORT_DOD].ToString()).Date;
                        int CardsDistributed = 0;
                        int Completes = 0;
                        int BusinessCompletes = 0;
                        int RCardsDistributed = 0;
                        int RCompletes = 0;
                        if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_CARDSDISTRIBUTED].ToString()))
                        {
                            CardsDistributed = System.Convert.ToInt32(Dr[BusinessConstants.REPORT_CARDSDISTRIBUTED].ToString());
                        }
                        if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_COMPLETES].ToString()))
                        {
                            Completes = System.Convert.ToInt32(Dr[BusinessConstants.REPORT_COMPLETES].ToString());
                        }
                        if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_BUSINESSCOMPLETES].ToString()))
                        {
                            BusinessCompletes = System.Convert.ToInt32(Dr[BusinessConstants.REPORT_BUSINESSCOMPLETES].ToString());
                        }
                        double ResponseRate = 0.0;
                        double BResponseRate = 0.0;
                        if (CardsDistributed > 0)
                        {
                            ResponseRate = System.Convert.ToDouble(Completes) / System.Convert.ToDouble(CardsDistributed) * 100.0;
                        }
                        if (ResponseDate != "-1")
                        {
                            if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_CARDSDISTRIBUTEDRESPONSE].ToString()))
                            {
                                RCardsDistributed = System.Convert.ToInt32(Dr[BusinessConstants.REPORT_CARDSDISTRIBUTEDRESPONSE].ToString());
                            }
                            if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_COMPLETESRESPONSE].ToString()))
                            {
                                RCompletes = System.Convert.ToInt32(Dr[BusinessConstants.REPORT_COMPLETESRESPONSE].ToString());
                            }
                            if (RCardsDistributed > 0)
                            {
                                ResponseRate = System.Convert.ToDouble(RCompletes) / System.Convert.ToDouble(RCardsDistributed) * 100.0;
                            }
                        }
                        if (Completes > 0)
                        {
                            BResponseRate = System.Convert.ToDouble(BusinessCompletes) / System.Convert.ToDouble(Completes) * 100.0;
                        }
                        TempDODReportDetail.DOD = Date.DateFormat();
                        TempDODReportDetail.DOD = TempDODReportDetail.DOD.Replace(".", "/");
                        TempDODReportDetail.Distributed = CardsDistributed;
                        TempDODReportDetail.TotalCompletes = Completes;
                        TempDODReportDetail.BCompletes = BusinessCompletes;
                        TempDODReportDetail.Incompletes = CardsDistributed - Completes;
                        TempDODReportDetail.ResponseRate = ResponseRate.Round();
                        TempDODReportDetail.BResponseRate = BResponseRate.Round();
                        TempDODReportDetail.RCardsDistributed = RCardsDistributed;
                        TempDODReportDetail.RTotalCompletes = RCompletes;
                        DODReportDetails.Add(TempDODReportDetail);
                    }
                }
                DODReportResponse.DODReportDetails = DODReportDetails;
                DODReportResponse.ReturnCode = 1;
                DODReportResponse.ReturnMessage = "Successfully Retrieved";
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(ReportingBusiness.CLASS_NAME, "GetDODReport", Ex);
                DODReportResponse.ReturnCode = -1;
                DODReportResponse.ReturnMessage = "Error in API";
            }
            SICTLogger.WriteInfo(ReportingBusiness.CLASS_NAME, "GetDODReport", "End for AirportId -" + AirportId);
            return DODReportResponse;
        }

        public FlightReportResponse GetFlightReport(string Instance, string SessionId, string AirportId, string OriginId, string DestinationId, string AirlineId, string FormType, string Route, string Direction, string FlightType, string AircraftType, string InterviewerId, string StartDate, string EndDate, string ResponseDate)
        {
            SICTLogger.WriteInfo(ReportingBusiness.CLASS_NAME, "GetFlightReport", "Start for AirportId -" + AirportId);
            FlightReportResponse FlightReportResponse = new FlightReportResponse();
            System.Collections.Generic.List<FlightReportDetail> FlightReportDetails = new System.Collections.Generic.List<FlightReportDetail>();
            DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
            try
            {
                UserDetailsBusiness Udb = new UserDetailsBusiness();
                bool IsSpecialUser = Udb.CheckIsSpecialUserOrNot(DBLayer.GetUserNameBySessionId(SessionId));
                if (FormType == BusinessConstants.FORM_TYPE_ARRIVAL)
                {
                    string TempId = OriginId;
                    OriginId = DestinationId;
                    DestinationId = TempId;
                }
                string FilterCondition = this.BuildMultiFilterConditionForReport(AirportId, OriginId, DestinationId, FormType, Route, Direction, StartDate, EndDate, InterviewerId, AirlineId, FlightType, AircraftType);
                string FilterConditionResponse = this.BuildMultiFilterConditionForResponseDate(AirportId, OriginId, DestinationId, FormType, Route, Direction, StartDate, ResponseDate, InterviewerId, AirlineId, FlightType, AircraftType);
                DataTable DtReport = new DataTable();
                SICTLogger.WriteVerbose(ReportingBusiness.CLASS_NAME, "GetFlightReport", "Start Retieving Flight Report  Data From DB");
                if (IsSpecialUser)
                {
                    DtReport = DBLayer.GetFlightReportForSpecialUser(SessionId, AirportId, FilterCondition, FilterConditionResponse);
                }
                else
                {
                    DtReport = DBLayer.GetFlightReport(SessionId, AirportId, FilterCondition, FilterConditionResponse);
                }
                if (IsSpecialUser)
                {
                    DataSet DSAirlines = new DataSet();
                    DSAirlines = DBLayer.getAirlineNamesBySessionId(SessionId);
                    DtReport = this.GetFilteredAirlineReport(DtReport, DSAirlines.Tables[0]);
                }
                if (DtReport.Rows.Count > 0)
                {
                    foreach (DataRow Dr in DtReport.Rows)
                    {
                        FlightReportDetail TempFlightReportDetail = new FlightReportDetail();
                        int CardsDistributed = 0;
                        int Completes = 0;
                        int BusinessCompletes = 0;
                        int RCardsDistributed = 0;
                        int RCompletes = 0;
                        if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_CARDSDISTRIBUTED].ToString()))
                        {
                            CardsDistributed = System.Convert.ToInt32(Dr[BusinessConstants.REPORT_CARDSDISTRIBUTED].ToString());
                        }
                        if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_COMPLETES].ToString()))
                        {
                            Completes = System.Convert.ToInt32(Dr[BusinessConstants.REPORT_COMPLETES].ToString());
                        }
                        if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_BUSINESSCOMPLETES].ToString()))
                        {
                            BusinessCompletes = System.Convert.ToInt32(Dr[BusinessConstants.REPORT_BUSINESSCOMPLETES].ToString());
                        }
                        double ResponseRate = 0.0;
                        double BResponseRate = 0.0;
                        if (CardsDistributed > 0)
                        {
                            ResponseRate = System.Convert.ToDouble(Completes) / System.Convert.ToDouble(CardsDistributed) * 100.0;
                        }
                        if (ResponseDate != "-1")
                        {
                            if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_CARDSDISTRIBUTEDRESPONSE].ToString()))
                            {
                                RCardsDistributed = System.Convert.ToInt32(Dr[BusinessConstants.REPORT_CARDSDISTRIBUTEDRESPONSE].ToString());
                            }
                            if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_COMPLETESRESPONSE].ToString()))
                            {
                                RCompletes = System.Convert.ToInt32(Dr[BusinessConstants.REPORT_COMPLETESRESPONSE].ToString());
                            }
                            if (RCardsDistributed > 0)
                            {
                                ResponseRate = System.Convert.ToDouble(RCompletes) / System.Convert.ToDouble(RCardsDistributed) * 100.0;
                            }
                        }
                        if (Completes > 0)
                        {
                            BResponseRate = System.Convert.ToDouble(BusinessCompletes) / System.Convert.ToDouble(Completes) * 100.0;
                        }
                        if (Instance == BusinessConstants.Instance.EUR.ToString())
                        {
                            TempFlightReportDetail.FlightType = Dr[BusinessConstants.REPORT_FLIGHTTYPE].ToString();
                        }
                        TempFlightReportDetail.OriginName = Dr[BusinessConstants.REPORT_ORIGINNAME].ToString();
                        TempFlightReportDetail.FlightNumber = Dr[BusinessConstants.REPORT_FLIGHTNUMBER].ToString();
                        TempFlightReportDetail.AirlineName = Dr[BusinessConstants.REPORT_AIRLINENAME].ToString();
                        TempFlightReportDetail.DestinationName = Dr[BusinessConstants.REPORT_DESTINATIONNAME].ToString();
                        TempFlightReportDetail.Distributed = CardsDistributed;
                        TempFlightReportDetail.TotalCompletes = Completes;
                        TempFlightReportDetail.BCompletes = BusinessCompletes;
                        TempFlightReportDetail.Incompletes = CardsDistributed - Completes;
                        TempFlightReportDetail.ResponseRate = ResponseRate.Round();
                        TempFlightReportDetail.BResponseRate = BResponseRate.Round();
                        TempFlightReportDetail.RCardsDistributed = RCardsDistributed;
                        TempFlightReportDetail.RTotalCompletes = RCompletes;
                        FlightReportDetails.Add(TempFlightReportDetail);
                    }
                }
                FlightReportResponse.FlightReportDetails = FlightReportDetails;
                FlightReportResponse.ReturnCode = 1;
                FlightReportResponse.ReturnMessage = "Successfully Retrieved";
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(ReportingBusiness.CLASS_NAME, "GetFlightReport", Ex);
                FlightReportResponse.ReturnCode = -1;
                FlightReportResponse.ReturnMessage = "Error in API";
            }
            SICTLogger.WriteInfo(ReportingBusiness.CLASS_NAME, "GetFlightReport", "End for AirportId -" + AirportId);
            return FlightReportResponse;
        }

        public DataTable GetFilteredAirlineReport(DataTable DtArrReport, DataTable DtAirlines)
        {
            DataTable FinalDT = new DataTable();
            try
            {
                FinalDT = DtArrReport.Clone();
                System.Collections.Generic.IEnumerable<DataRow> Adj = from s in DtArrReport.AsEnumerable()
                                                                      join sp in DtAirlines.AsEnumerable() on s.Field<string>("AirlineName") equals sp.Field<string>("airline_name")
                                                                      select s;
                if (Adj != null && Adj.Count<DataRow>() != 0)
                {
                    FinalDT = Adj.CopyToDataTable<DataRow>();
                }
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(ReportingBusiness.CLASS_NAME, "GetFilteredAirlineReport", Ex);
            }
            return FinalDT; 
        }

        public AirlineReportResponse GetAirlineReport(string Instance, string SessionId, string AirportId, string OriginId, string DestinationId, string FormType, string Route, string Direction, string FlightType, string AircraftType, string InterviewerId, string StartDate, string EndDate, string AirlineId, string ResponseDate)
        {
            SICTLogger.WriteInfo(ReportingBusiness.CLASS_NAME, "GetAirlineReport", "Start for AirportId -" + AirportId);
            AirlineReportResponse AirlineReportResponse = new AirlineReportResponse();
            System.Collections.Generic.List<AirlineReportDetail> AirlineReportDetails = new System.Collections.Generic.List<AirlineReportDetail>();
            DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
            try
            {
                string FieldWorkId = OriginId;
                if (FormType == BusinessConstants.FORM_TYPE_ARRIVAL)
                {
                    string TempId = OriginId;
                    OriginId = DestinationId;
                    DestinationId = TempId;
                }
                bool IsTargetZeroRequired = System.Convert.ToBoolean(ConfigurationManager.AppSettings[BusinessConstants.ISTARGETZEROREQUIRED].ToString());
                UserDetailsBusiness Udb = new UserDetailsBusiness();
                bool IsSpecialUser = Udb.CheckIsSpecialUserOrNot(DBLayer.GetUserNameBySessionId(SessionId));
                DataTable DtReport = new DataTable();
                if (FormType == BusinessConstants.DEFAULT_SELECTION_VALUE && (Instance == BusinessConstants.Instance.US.ToString() || Instance == BusinessConstants.Instance.AIR.ToString()))
                {
                    string TempFormType = BusinessConstants.FORM_TYPE_ARRIVAL;
                    string TempOriginId = DestinationId;
                    string TempDestinationId = OriginId;
                    string FilterCondition = this.BuildMultiFilterConditionForReport(AirportId, TempOriginId, TempDestinationId, TempFormType, Route, Direction, StartDate, EndDate, InterviewerId, AirlineId, FlightType, AircraftType);
                    string TargetFilterCondition = this.BuildMultipleFilterConditionForTarget(Instance, AirportId, FieldWorkId, TempFormType, Route, Direction, FlightType, AircraftType, IsTargetZeroRequired, AirlineId);
                    string ResponseDateFilterCondition = this.BuildMultiFilterConditionForResponseDate(AirportId, TempOriginId, TempDestinationId, TempFormType, Route, Direction, StartDate, ResponseDate, InterviewerId, AirlineId, FlightType, AircraftType);
                    DataSet DSAirlines = new DataSet();
                    if (IsSpecialUser)
                    {
                        DSAirlines = DBLayer.getAirlineNamesBySessionId(SessionId);
                    }
                    DataTable DtArrReport = new DataTable();
                    if (IsSpecialUser)
                    {
                        DtArrReport = DBLayer.GetAirlineReportForSpecialUser(SessionId, AirportId, FilterCondition, TargetFilterCondition, ResponseDateFilterCondition);
                    }
                    else
                    {
                        DtArrReport = DBLayer.GetAirlineReport(SessionId, AirportId, FilterCondition, TargetFilterCondition, ResponseDateFilterCondition);
                    }
                    if (IsSpecialUser)
                    {
                        DtArrReport = this.GetFilteredAirlineReport(DtArrReport, DSAirlines.Tables[0]);
                    }
                    TempFormType = BusinessConstants.FORM_TYPE_DEPARTURE;
                    FilterCondition = this.BuildMultiFilterConditionForReport(AirportId, OriginId, DestinationId, TempFormType, Route, Direction, StartDate, EndDate, InterviewerId, AirlineId, FlightType, AircraftType);
                    TargetFilterCondition = this.BuildMultipleFilterConditionForTarget(Instance, AirportId, FieldWorkId, TempFormType, Route, Direction, FlightType, AircraftType, IsTargetZeroRequired, AirlineId);
                    ResponseDateFilterCondition = this.BuildMultiFilterConditionForResponseDate(AirportId, OriginId, DestinationId, TempFormType, Route, Direction, StartDate, ResponseDate, InterviewerId, AirlineId, FlightType, AircraftType);
                    DataTable DtDprReport = new DataTable();
                    if (IsSpecialUser)
                    {
                        DtDprReport = DBLayer.GetAirlineReportForSpecialUser(SessionId, AirportId, FilterCondition, TargetFilterCondition, ResponseDateFilterCondition);
                    }
                    else
                    {
                        DtDprReport = DBLayer.GetAirlineReport(SessionId, AirportId, FilterCondition, TargetFilterCondition, ResponseDateFilterCondition);
                    }
                    if (IsSpecialUser)
                    {
                        DtDprReport = this.GetFilteredAirlineReport(DtDprReport, DSAirlines.Tables[0]);
                    }
                    DtReport = DtArrReport.Copy();
                    DtReport.Merge(DtDprReport);
                    if (Instance == BusinessConstants.Instance.US.ToString())
                    {
                        var query = from row in DtReport.AsEnumerable()
                                    group row by row.Field<string>(BusinessConstants.REPORT_AIRLINENAME) into grp
                                    orderby grp.Key
                                    select new
                                    {
                                        AirlineName = grp.Key,
                                        Target = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_TARGET)),
                                        CardsDistributed = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_CARDSDISTRIBUTED)),
                                        CardsDistributedResponse = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_CARDSDISTRIBUTEDRESPONSE)),
                                        BCardsDistributed = grp.Sum((DataRow r) => r.Field<Int32>("BCardsDistributed")),
                                        Completes = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_COMPLETES)),
                                        CompletesResponse = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_COMPLETESRESPONSE)),
                                        BusinessCompletes = grp.Sum((DataRow r) => r.Field<Int32>("BusinessCompletes")),
                                        ECompletes = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_ECOMPLETES)),
                                        PECompletes = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_PECOMPLETES)),
                                        FCCompletes = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_FCCOMPLETES))
                                    };
                        DtReport = this.ConvertToDataTable(query);
                    }
                    else if (Instance == BusinessConstants.Instance.AIR.ToString())
                    {
                        var query2 = from row in DtReport.AsEnumerable()
                                     group row by new
                                     {
                                         AirlineName = row.Field<string>(BusinessConstants.REPORT_AIRLINENAME),
                                         AircraftName = row.Field<string>(BusinessConstants.AIRCRAFTTYPE_NAME)
                                     } into grp
                                     orderby grp.Key.AirlineName
                                     select new
                                     {
                                         AirlineName = grp.Key.AirlineName,
                                         AircraftName = grp.Key.AircraftName,
                                         Target = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_TARGET)),
                                         CardsDistributed = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_CARDSDISTRIBUTED)),
                                         CardsDistributedResponse = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_CARDSDISTRIBUTEDRESPONSE)),
                                         BCardsDistributed = grp.Sum((DataRow r) => r.Field<Int32>("BCardsDistributed")),
                                         Completes = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_COMPLETES)),
                                         CompletesResponse = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_COMPLETESRESPONSE)),
                                         BusinessCompletes = grp.Sum((DataRow r) => r.Field<Int32>("BusinessCompletes")),
                                         ECompletes = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_ECOMPLETES)),
                                         PECompletes = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_PECOMPLETES)),
                                         FCCompletes = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_FCCOMPLETES))
                                     };
                        DtReport = this.ConvertToDataTable(query2);
                    }
                }
                else
                {
                    string FilterCondition = this.BuildMultiFilterConditionForReport(AirportId, OriginId, DestinationId, FormType, Route, Direction, StartDate, EndDate, InterviewerId, AirlineId, FlightType, AircraftType);
                    string TargetFilterCondition = this.BuildMultipleFilterConditionForTarget(Instance, AirportId, FieldWorkId, FormType, Route, Direction, FlightType, AircraftType, IsTargetZeroRequired, AirlineId);
                    string ResponseDateFilterCondition = this.BuildMultiFilterConditionForReport(AirportId, OriginId, DestinationId, FormType, Route, Direction, StartDate, ResponseDate, InterviewerId, AirlineId, FlightType, AircraftType);
                    SICTLogger.WriteVerbose(ReportingBusiness.CLASS_NAME, "GetAirlineReport", "Start Retieving Airline Report  Data From DB");
                    DtReport = DBLayer.GetAirlineReport(SessionId, AirportId, FilterCondition, TargetFilterCondition, ResponseDateFilterCondition);
                    if (IsSpecialUser)
                    {
                        DtReport = DBLayer.GetAirlineReportForSpecialUser(SessionId, AirportId, FilterCondition, TargetFilterCondition, ResponseDateFilterCondition);
                    }
                    else
                    {
                        DtReport = DBLayer.GetAirlineReport(SessionId, AirportId, FilterCondition, TargetFilterCondition, ResponseDateFilterCondition);
                    }
                    if (IsSpecialUser)
                    {
                        DataSet DSAirlines = new DataSet();
                        DSAirlines = DBLayer.getAirlineNamesBySessionId(SessionId);
                        DtReport = this.GetFilteredAirlineReport(DtReport, DSAirlines.Tables[0]);
                    }
                }
                if (DtReport.Rows.Count > 0)
                {
                    foreach (DataRow Dr in DtReport.Rows)
                    {
                        double Target = 0.0;
                        if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_TARGET].ToString()))
                        {
                            Target = System.Convert.ToDouble(Dr[BusinessConstants.REPORT_TARGET].ToString());
                        }
                        AirlineReportDetail TempAirlineReportDetail = new AirlineReportDetail();
                        string AirlineName = Dr[BusinessConstants.REPORT_AIRLINENAME].ToString();
                        if (Instance == BusinessConstants.Instance.EUR.ToString())
                        {
                            TempAirlineReportDetail.Type = Dr[BusinessConstants.FLIGHTTYPE].ToString();
                        }
                        else if (Instance == BusinessConstants.Instance.AIR.ToString())
                        {
                            TempAirlineReportDetail.Type = Dr[BusinessConstants.AIRCRAFTTYPE_NAME].ToString();
                        }
                        double CardsDistributed = 0.0;
                        double Completes = 0.0;
                        double BusinessCompletes = 0.0;
                        double ECompletes = 0.0;
                        double PECompletes = 0.0;
                        double FCCompletes = 0.0;
                        int RCardsDistributed = 0;
                        int RCompletes = 0;
                        if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_TARGET].ToString()))
                        {
                            Target = System.Convert.ToDouble(Dr[BusinessConstants.REPORT_TARGET].ToString());
                        }
                        double BTarget;
                        if (Instance == BusinessConstants.Instance.EUR.ToString())
                        {
                            BTarget = Target * 0.25;
                        }
                        else
                        {
                            BTarget = Target * 0.33333333333333331;
                        }
                        if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_CARDSDISTRIBUTED].ToString()))
                        {
                            CardsDistributed = System.Convert.ToDouble(Dr[BusinessConstants.REPORT_CARDSDISTRIBUTED].ToString());
                        }
                        if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_COMPLETES].ToString()))
                        {
                            Completes = System.Convert.ToDouble(Dr[BusinessConstants.REPORT_COMPLETES].ToString());
                        }
                        if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_BUSINESSCOMPLETES].ToString()))
                        {
                            BusinessCompletes = System.Convert.ToDouble(Dr[BusinessConstants.REPORT_BUSINESSCOMPLETES].ToString());
                        }
                        if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_ECOMPLETES].ToString()))
                        {
                            ECompletes = System.Convert.ToDouble(Dr[BusinessConstants.REPORT_ECOMPLETES].ToString());
                        }
                        if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_PECOMPLETES].ToString()))
                        {
                            PECompletes = System.Convert.ToDouble(Dr[BusinessConstants.REPORT_PECOMPLETES].ToString());
                        }
                        if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_FCCOMPLETES].ToString()))
                        {
                            FCCompletes = System.Convert.ToDouble(Dr[BusinessConstants.REPORT_FCCOMPLETES].ToString());
                        }
                        double ResponseRate = 0.0;
                        double BResponseRate = 0.0;
                        if (CardsDistributed > 0.0)
                        {
                            ResponseRate = Completes / CardsDistributed * 100.0;
                        }
                        if (ResponseDate != "-1")
                        {
                            if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_CARDSDISTRIBUTEDRESPONSE].ToString()))
                            {
                                RCardsDistributed = System.Convert.ToInt32(Dr[BusinessConstants.REPORT_CARDSDISTRIBUTEDRESPONSE].ToString());
                            }
                            if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_COMPLETESRESPONSE].ToString()))
                            {
                                RCompletes = System.Convert.ToInt32(Dr[BusinessConstants.REPORT_COMPLETESRESPONSE].ToString());
                            }
                            if (RCardsDistributed > 0)
                            {
                                ResponseRate = System.Convert.ToDouble(RCompletes) / System.Convert.ToDouble(RCardsDistributed) * 100.0;
                            }
                        }
                        if (Completes > 0.0)
                        {
                            BResponseRate = BusinessCompletes / Completes * 100.0;
                        }
                        double TargetAchieved = 0.0;
                        double BTargetAchieved = 0.0;
                        if (Target > 0.0)
                        {
                            TargetAchieved = (Completes / Target * 100.0).Round();
                        }
                        if (BTarget > 0.0)
                        {
                            BTargetAchieved = (BusinessCompletes / BTarget * 100.0).Round();
                        }
                        double MissingTarget = Target - Completes;
                        double MissingBTarget = BTarget - BusinessCompletes;
                        TempAirlineReportDetail.AirlineName = AirlineName;
                        TempAirlineReportDetail.Target = Target;
                        TempAirlineReportDetail.Distributed = CardsDistributed.Round();
                        TempAirlineReportDetail.TotalCompletes = Completes.Round();
                        TempAirlineReportDetail.BCompletes = BusinessCompletes.Round();
                        TempAirlineReportDetail.ECompletes = ECompletes.Round();
                        TempAirlineReportDetail.PECompletes = PECompletes.Round();
                        TempAirlineReportDetail.FCCompletes = FCCompletes.Round();
                        TempAirlineReportDetail.Incompletes = (CardsDistributed - Completes).Round();
                        TempAirlineReportDetail.ResponseRate = ResponseRate.Round();
                        TempAirlineReportDetail.BResponseRate = BResponseRate.Round();
                        TempAirlineReportDetail.TargetAchieved = TargetAchieved.Round();
                        TempAirlineReportDetail.BTargetAchieved = BTargetAchieved.Round();
                        TempAirlineReportDetail.MissingTarget = MissingTarget.Round();
                        TempAirlineReportDetail.MissingBTarget = MissingBTarget.Round();
                        TempAirlineReportDetail.RTotalCompletes = RCompletes;
                        TempAirlineReportDetail.RCardsDistributed = RCardsDistributed;
                        AirlineReportDetails.Add(TempAirlineReportDetail);
                    }
                }
                AirlineReportResponse.AirlineReportDetails = AirlineReportDetails;
                AirlineReportResponse.ReturnCode = 1;
                AirlineReportResponse.ReturnMessage = "Successfully Retrieved";
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(ReportingBusiness.CLASS_NAME, "GetAirlineReport", Ex);
                AirlineReportResponse.ReturnCode = -1;
                AirlineReportResponse.ReturnMessage = "Error in API";
            }
            SICTLogger.WriteInfo(ReportingBusiness.CLASS_NAME, "GetAirlineReport", "End for AirportId -" + AirportId);
            return AirlineReportResponse;
        }

        public string GetAllinterviewerIdsBySessionId(string SessionId)
        {
            string InterviewerIds = string.Empty;
            try
            {
                DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
                DataSet InterviewerDS = DBLayer.GetAllInterviewersIdBySessionId(SessionId);
                foreach (DataRow Drow in InterviewerDS.Tables[0].Rows)
                {
                    InterviewerIds = InterviewerIds + Drow["InterviewerId"] + ",";
                }
                if (InterviewerIds.Length > 0)
                {
                    InterviewerIds = InterviewerIds.Remove(InterviewerIds.Length - 1);
                }
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(ReportingBusiness.CLASS_NAME, "GetAllinterviewerIdsBySessionId", Ex);
            }
            return InterviewerIds;
        }

        public AirlineReportResponse GetAllAirlineReport(string Instance, string SessionId, string AirportId, string OriginId, string DestinationId, string FormType, string Route, string Direction, string FlightType, string AircraftType, string InterviewerId, string StartDate, string EndDate, string AirlineId, string ResponseDate)
        {
            SICTLogger.WriteInfo(ReportingBusiness.CLASS_NAME, "GetAllAirlineReport", "Start for AirportId -" + AirportId);
            AirlineReportResponse AirlineReportResponse = new AirlineReportResponse();
            System.Collections.Generic.List<AirlineReportDetail> AirlineReportDetails = new System.Collections.Generic.List<AirlineReportDetail>();
            DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
            try
            {
                string FieldWorkId = OriginId;
                if (FormType == BusinessConstants.FORM_TYPE_ARRIVAL)
                {
                    string TempId = OriginId;
                    OriginId = DestinationId;
                    DestinationId = TempId;
                }
                bool IsTargetZeroRequired = System.Convert.ToBoolean(ConfigurationManager.AppSettings[BusinessConstants.ISTARGETZEROREQUIRED].ToString());
                DataTable DtReport = new DataTable();
                if (FormType == BusinessConstants.DEFAULT_SELECTION_VALUE && (Instance == BusinessConstants.Instance.US.ToString() || Instance == BusinessConstants.Instance.AIR.ToString()))
                {
                    string TempFormType = BusinessConstants.FORM_TYPE_ARRIVAL;
                    string TempOriginId = DestinationId;
                    string TempDestinationId = OriginId;
                    string FilterCondition = this.BuildMultiFilterConditionForReport(AirportId, TempOriginId, TempDestinationId, TempFormType, Route, Direction, StartDate, EndDate, InterviewerId, AirlineId, FlightType, AircraftType);
                    string TargetFilterCondition = this.BuildMultipleFilterConditionForTarget(Instance, AirportId, FieldWorkId, TempFormType, Route, Direction, FlightType, AircraftType, IsTargetZeroRequired, AirlineId);
                    string ResponseDateFilterCondition = this.BuildMultiFilterConditionForResponseDate(AirportId, TempOriginId, TempDestinationId, TempFormType, Route, Direction, StartDate, ResponseDate, InterviewerId, AirlineId, FlightType, AircraftType);
                    DataTable DtArrReport = new DataTable();
                    DtArrReport = DBLayer.GetAllAirlineReport(SessionId, AirportId, FilterCondition, TargetFilterCondition, ResponseDateFilterCondition);
                    TempFormType = BusinessConstants.FORM_TYPE_DEPARTURE;
                    FilterCondition = this.BuildMultiFilterConditionForReport(AirportId, OriginId, DestinationId, TempFormType, Route, Direction, StartDate, EndDate, InterviewerId, AirlineId, FlightType, AircraftType);
                    TargetFilterCondition = this.BuildMultipleFilterConditionForTarget(Instance, AirportId, FieldWorkId, TempFormType, Route, Direction, FlightType, AircraftType, IsTargetZeroRequired, AirlineId);
                    ResponseDateFilterCondition = this.BuildMultiFilterConditionForResponseDate(AirportId, OriginId, DestinationId, TempFormType, Route, Direction, StartDate, ResponseDate, InterviewerId, AirlineId, FlightType, AircraftType);
                    DataTable DtDprReport = new DataTable();
                    DtDprReport = DBLayer.GetAllAirlineReport(SessionId, AirportId, FilterCondition, TargetFilterCondition, ResponseDateFilterCondition);
                    DtReport = DtArrReport.Copy();
                    DtReport.Merge(DtDprReport);
                    if (Instance == BusinessConstants.Instance.US.ToString())
                    {
                        var query = from row in DtReport.AsEnumerable()
                                    group row by row.Field<string>(BusinessConstants.REPORT_AIRLINENAME) into grp
                                    orderby grp.Key
                                    select new
                                    {
                                        AirlineName = grp.Key,
                                        Target = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_TARGET)),
                                        CardsDistributed = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_CARDSDISTRIBUTED)),
                                        CardsDistributedResponse = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_CARDSDISTRIBUTEDRESPONSE)),
                                        BCardsDistributed = grp.Sum((DataRow r) => r.Field<Int32>("BCardsDistributed")),
                                        Completes = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_COMPLETES)),
                                        CompletesResponse = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_COMPLETESRESPONSE)),
                                        BusinessCompletes = grp.Sum((DataRow r) => r.Field<Int32>("BusinessCompletes")),
                                        ECompletes = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_ECOMPLETES)),
                                        PECompletes = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_PECOMPLETES)),
                                        FCCompletes = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_FCCOMPLETES))
                                    };
                        DtReport = this.ConvertToDataTable(query);
                    }
                    else if (Instance == BusinessConstants.Instance.AIR.ToString())
                    {
                        var query2 = from row in DtReport.AsEnumerable()
                                     group row by new
                                     {
                                         AirlineName = row.Field<string>(BusinessConstants.REPORT_AIRLINENAME),
                                         AircraftName = row.Field<string>(BusinessConstants.AIRCRAFTTYPE_NAME)
                                     } into grp
                                     orderby grp.Key.AirlineName
                                     select new
                                     {
                                         AirlineName = grp.Key.AirlineName,
                                         AircraftName = grp.Key.AircraftName,
                                         Target = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_TARGET)),
                                         CardsDistributed = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_CARDSDISTRIBUTED)),
                                         CardsDistributedResponse = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_CARDSDISTRIBUTEDRESPONSE)),
                                         BCardsDistributed = grp.Sum((DataRow r) => r.Field<Int32>("BCardsDistributed")),
                                         Completes = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_COMPLETES)),
                                         CompletesResponse = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_COMPLETESRESPONSE)),
                                         BusinessCompletes = grp.Sum((DataRow r) => r.Field<Int32>("BusinessCompletes")),
                                         ECompletes = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_ECOMPLETES)),
                                         PECompletes = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_PECOMPLETES)),
                                         FCCompletes = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_FCCOMPLETES))
                                     };
                        DtReport = this.ConvertToDataTable(query2);
                    }
                }
                else
                {
                    string FilterCondition = this.BuildMultiFilterConditionForReport(AirportId, OriginId, DestinationId, FormType, Route, Direction, StartDate, EndDate, InterviewerId, AirlineId, FlightType, AircraftType);
                    string TargetFilterCondition = this.BuildMultipleFilterConditionForTarget(Instance, AirportId, FieldWorkId, FormType, Route, Direction, FlightType, AircraftType, IsTargetZeroRequired, AirlineId);
                    string ResponseDateFilterCondition = this.BuildMultiFilterConditionForReport(AirportId, OriginId, DestinationId, FormType, Route, Direction, StartDate, ResponseDate, InterviewerId, AirlineId, FlightType, AircraftType);
                    SICTLogger.WriteVerbose(ReportingBusiness.CLASS_NAME, "GetAllAirlineReport", "Start Retieving AllAirline Report  Data From DB");
                    DtReport = DBLayer.GetAllAirlineReport(SessionId, AirportId, FilterCondition, TargetFilterCondition, ResponseDateFilterCondition);
                }
                if (DtReport.Rows.Count > 0)
                {
                    foreach (DataRow Dr in DtReport.Rows)
                    {
                        double Target = 0.0;
                        if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_TARGET].ToString()))
                        {
                            Target = System.Convert.ToDouble(Dr[BusinessConstants.REPORT_TARGET].ToString());
                        }
                        AirlineReportDetail TempAirlineReportDetail = new AirlineReportDetail();
                        string AirlineName = Dr[BusinessConstants.REPORT_AIRLINENAME].ToString();
                        if (Instance == BusinessConstants.Instance.EUR.ToString())
                        {
                            TempAirlineReportDetail.Type = Dr[BusinessConstants.FLIGHTTYPE].ToString();
                        }
                        else if (Instance == BusinessConstants.Instance.AIR.ToString())
                        {
                            TempAirlineReportDetail.Type = Dr[BusinessConstants.AIRCRAFTTYPE_NAME].ToString();
                        }
                        double CardsDistributed = 0.0;
                        double Completes = 0.0;
                        double BusinessCompletes = 0.0;
                        double ECompletes = 0.0;
                        double PECompletes = 0.0;
                        double FCCompletes = 0.0;
                        int RCardsDistributed = 0;
                        int RCompletes = 0;
                        if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_TARGET].ToString()))
                        {
                            Target = System.Convert.ToDouble(Dr[BusinessConstants.REPORT_TARGET].ToString());
                        }
                        double BTarget;
                        if (Instance == BusinessConstants.Instance.EUR.ToString())
                        {
                            BTarget = Target * 0.25;
                        }
                        else
                        {
                            BTarget = Target * 0.33333333333333331;
                        }
                        if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_CARDSDISTRIBUTED].ToString()))
                        {
                            CardsDistributed = System.Convert.ToDouble(Dr[BusinessConstants.REPORT_CARDSDISTRIBUTED].ToString());
                        }
                        if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_COMPLETES].ToString()))
                        {
                            Completes = System.Convert.ToDouble(Dr[BusinessConstants.REPORT_COMPLETES].ToString());
                        }
                        if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_BUSINESSCOMPLETES].ToString()))
                        {
                            BusinessCompletes = System.Convert.ToDouble(Dr[BusinessConstants.REPORT_BUSINESSCOMPLETES].ToString());
                        }
                        if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_ECOMPLETES].ToString()))
                        {
                            ECompletes = System.Convert.ToDouble(Dr[BusinessConstants.REPORT_ECOMPLETES].ToString());
                        }
                        if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_PECOMPLETES].ToString()))
                        {
                            PECompletes = System.Convert.ToDouble(Dr[BusinessConstants.REPORT_PECOMPLETES].ToString());
                        }
                        if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_FCCOMPLETES].ToString()))
                        {
                            FCCompletes = System.Convert.ToDouble(Dr[BusinessConstants.REPORT_FCCOMPLETES].ToString());
                        }
                        double ResponseRate = 0.0;
                        double BResponseRate = 0.0;
                        if (CardsDistributed > 0.0)
                        {
                            ResponseRate = Completes / CardsDistributed * 100.0;
                        }
                        if (ResponseDate != "-1")
                        {
                            if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_CARDSDISTRIBUTEDRESPONSE].ToString()))
                            {
                                RCardsDistributed = System.Convert.ToInt32(Dr[BusinessConstants.REPORT_CARDSDISTRIBUTEDRESPONSE].ToString());
                            }
                            if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_COMPLETESRESPONSE].ToString()))
                            {
                                RCompletes = System.Convert.ToInt32(Dr[BusinessConstants.REPORT_COMPLETESRESPONSE].ToString());
                            }
                            if (RCardsDistributed > 0)
                            {
                                ResponseRate = System.Convert.ToDouble(RCompletes) / System.Convert.ToDouble(RCardsDistributed) * 100.0;
                            }
                        }
                        if (Completes > 0.0)
                        {
                            BResponseRate = BusinessCompletes / Completes * 100.0;
                        }
                        double TargetAchieved = 0.0;
                        double BTargetAchieved = 0.0;
                        if (Target > 0.0)
                        {
                            TargetAchieved = (Completes / Target * 100.0).Round();
                        }
                        if (BTarget > 0.0)
                        {
                            BTargetAchieved = (BusinessCompletes / BTarget * 100.0).Round();
                        }
                        double MissingTarget = Target - Completes;
                        double MissingBTarget = BTarget - BusinessCompletes;
                        TempAirlineReportDetail.AirlineName = AirlineName;
                        TempAirlineReportDetail.Target = Target;
                        TempAirlineReportDetail.Distributed = CardsDistributed.Round();
                        TempAirlineReportDetail.TotalCompletes = Completes.Round();
                        TempAirlineReportDetail.BCompletes = BusinessCompletes.Round();
                        TempAirlineReportDetail.ECompletes = ECompletes.Round();
                        TempAirlineReportDetail.PECompletes = PECompletes.Round();
                        TempAirlineReportDetail.FCCompletes = FCCompletes.Round();
                        TempAirlineReportDetail.Incompletes = (CardsDistributed - Completes).Round();
                        TempAirlineReportDetail.ResponseRate = ResponseRate.Round();
                        TempAirlineReportDetail.BResponseRate = BResponseRate.Round();
                        TempAirlineReportDetail.TargetAchieved = TargetAchieved.Round();
                        TempAirlineReportDetail.BTargetAchieved = BTargetAchieved.Round();
                        TempAirlineReportDetail.MissingTarget = MissingTarget.Round();
                        TempAirlineReportDetail.MissingBTarget = MissingBTarget.Round();
                        TempAirlineReportDetail.RTotalCompletes = RCompletes;
                        TempAirlineReportDetail.RCardsDistributed = RCardsDistributed;
                        AirlineReportDetails.Add(TempAirlineReportDetail);
                    }
                }
                AirlineReportResponse.AirlineReportDetails = AirlineReportDetails;
                AirlineReportResponse.ReturnCode = 1;
                AirlineReportResponse.ReturnMessage = "Successfully Retrieved";
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(ReportingBusiness.CLASS_NAME, "GetAllAirlineReport", Ex);
                AirlineReportResponse.ReturnCode = -1;
                AirlineReportResponse.ReturnMessage = "Error in API";
            }
            SICTLogger.WriteInfo(ReportingBusiness.CLASS_NAME, "GetAllAirlineReport", "End for AirportId -" + AirportId);
            return AirlineReportResponse;
        }

        public DataTable ConvertToDataTable<T>(System.Collections.Generic.IEnumerable<T> varlist)
        {
            DataTable dtReturn = new DataTable();
            System.Reflection.PropertyInfo[] oProps = null;
            DataTable result;
            if (varlist == null)
            {
                result = dtReturn;
            }
            else
            {
                foreach (T rec in varlist)
                {
                    System.Reflection.PropertyInfo[] array;
                    if (oProps == null)
                    {
                        oProps = rec.GetType().GetProperties();
                        array = oProps;
                        for (int i = 0; i < array.Length; i++)
                        {
                            System.Reflection.PropertyInfo pi = array[i];
                            System.Type colType = pi.PropertyType;
                            if (colType.IsGenericType && colType.GetGenericTypeDefinition() == typeof(System.Nullable<>))
                            {
                                colType = colType.GetGenericArguments()[0];
                            }
                            dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                        }
                    }
                    DataRow dr = dtReturn.NewRow();
                    array = oProps;
                    for (int i = 0; i < array.Length; i++)
                    {
                        System.Reflection.PropertyInfo pi = array[i];
                        dr[pi.Name] = ((pi.GetValue(rec, null) == null) ? System.DBNull.Value : pi.GetValue(rec, null));
                    }
                    dtReturn.Rows.Add(dr);
                }
                result = dtReturn;
            }
            return result;
        }

        public DataTable ConvertJSONToDataTable<T>(System.Collections.Generic.IEnumerable<T> varlist)
        {
            DataTable dtReturn = new DataTable();
            System.Reflection.PropertyInfo[] oProps = null;
            DataTable result;
            if (varlist == null)
            {
                result = dtReturn;
            }
            else
            {
                foreach (T rec in varlist)
                {
                    System.Reflection.PropertyInfo[] array;
                    if (oProps == null)
                    {
                        oProps = rec.GetType().GetProperties();
                        array = oProps;
                        for (int i = 0; i < array.Length; i++)
                        {
                            System.Reflection.PropertyInfo pi = array[i];
                            System.Type colType = pi.PropertyType;
                            if (colType.IsGenericType && colType.GetGenericTypeDefinition() == typeof(System.Nullable<>))
                            {
                                colType = colType.GetGenericArguments()[0];
                            }
                            dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                        }
                    }
                    DataRow dr = dtReturn.NewRow();
                    array = oProps;
                    for (int i = 0; i < array.Length; i++)
                    {
                        System.Reflection.PropertyInfo pi = array[i];
                        dr[pi.Name] = ((pi.GetValue(rec, null) == null) ? System.DBNull.Value : pi.GetValue(rec, null));
                    }
                    dtReturn.Rows.Add(dr);
                }
                result = dtReturn;
            }
            return result;
        }

        private string BuildMultipleFilterConditionForTarget(string Instance, string AirportId, string OriginId, string FormType, string Route, string Direction, string FlightType, string AircraftType, bool IsTargetZeroRequired, string AirlineId)
        {
            SICTLogger.WriteInfo(ReportingBusiness.CLASS_NAME, "BuildFilterConditionForTarget", "Start For AiprortId -" + AirportId);
            string TargetFilterCondition = string.Empty;
            try
            {
                string FilterFormat = "{0} = {1}";
                System.Collections.Generic.List<string> Filters = new System.Collections.Generic.List<string>();
                if (Instance == BusinessConstants.Instance.US.ToString() || Instance == BusinessConstants.Instance.AIR.ToString())
                {
                    if (FormType != BusinessConstants.DEFAULT_SELECTION_VALUE)
                    {
                        string Filter = string.Format(FilterFormat, BusinessConstants.TABLE_INDCARDENTRY_TYPE, "'" + FormType + "'");
                        Filters.Add(Filter);
                    }
                }
                if (AirlineId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    string Filter = BusinessConstants.TABLE_INDCARDENTRY_AIRLINEID + " in(" + AirlineId + ")";
                    Filters.Add(Filter);
                }
                if (OriginId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    string Filter = BusinessConstants.TABLE_INDCARDENTRY_ORIGINID + " in(" + OriginId + ")";
                    Filters.Add(Filter);
                }
                if (Route != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    string[] Routesplit = Route.Split(new char[]
                    {
                        ','
                    });
                    string RouteList = "'" + string.Join("','", Routesplit) + "'";
                    string Filter = BusinessConstants.TABLE_INDCARDENTRY_ROUTE + " in(" + RouteList + ")";
                    Filters.Add(Filter);
                }
                if (Direction != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    string[] Directionsplit = Direction.Split(new char[]
                    {
                        ','
                    });
                    string DirectionList = "'" + string.Join("','", Directionsplit) + "'";
                    string Filter = BusinessConstants.TABLE_INDCARDENTRY_DIRECTION + " in(" + DirectionList + ")";
                    Filters.Add(Filter);
                }
                if (FlightType != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    string[] FlightTypesplit = FlightType.Split(new char[]
                    {
                        ','
                    });
                    string FlightTypeList = "'" + string.Join("','", FlightTypesplit) + "'";
                    string Filter = BusinessConstants.TABLE_INDCARDENTRY_TYPE + " in(" + FlightTypeList + ")";
                    Filters.Add(Filter);
                }
                if (AircraftType != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    string[] AircraftTypesplit = AircraftType.Split(new char[]
                    {
                        ','
                    });
                    string AircraftTypeList = "'" + string.Join("','", AircraftTypesplit) + "'";
                    string Filter = BusinessConstants.TABLE_INDCARDENTRY_AIRCRAFT_TYPE + " in(" + AircraftTypeList + ")";
                    Filters.Add(Filter);
                }
                if (IsTargetZeroRequired)
                {
                    string Filter = string.Format("{0}>={1}", BusinessConstants.TABLE_TARGET, 0);
                    Filters.Add(Filter);
                }
                if (Filters.Count > 0)
                {
                    TargetFilterCondition = string.Join(" and ", Filters);
                }
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(ReportingBusiness.CLASS_NAME, "BuildFilterConditionForTarget", Ex);
            }
            SICTLogger.WriteInfo(ReportingBusiness.CLASS_NAME, "BuildFilterConditionForTarget", "End For AiprortId -" + AirportId);
            return TargetFilterCondition;
        }

        public AirportReportResponse GetAirportReport(string Instance, string SessionId, string AirportId, string OriginId, string DestinationId, string AirlineId, string FormType, string Route, string Direction, string FlightType, string AircraftType, string InterviewerId, string StartDate, string EndDate, string ResponseDate)
        {
            SICTLogger.WriteInfo(ReportingBusiness.CLASS_NAME, "GetAirlineReport", "Start ");
            AirportReportResponse AirportReportResponse = new AirportReportResponse();
            System.Collections.Generic.List<AirportReportDetail> AirportReportDetails = new System.Collections.Generic.List<AirportReportDetail>();
            DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
            try
            {
                bool IsTargetZeroRequired = System.Convert.ToBoolean(ConfigurationManager.AppSettings[BusinessConstants.ISTARGETZEROREQUIRED].ToString());
                string FieldWorkId = OriginId;
                if (FormType == BusinessConstants.FORM_TYPE_ARRIVAL)
                {
                    string TempId = OriginId;
                    OriginId = DestinationId;
                    DestinationId = TempId;
                }
                DataTable DtReport = new DataTable();
                if (FormType == BusinessConstants.DEFAULT_SELECTION_VALUE && (Instance == BusinessConstants.Instance.US.ToString() || Instance == BusinessConstants.Instance.AIR.ToString()))
                {
                    string TempFormType = BusinessConstants.FORM_TYPE_ARRIVAL;
                    string TempOriginId = DestinationId;
                    string TempDestinationId = OriginId;
                    bool IsDeparture = false;
                    string FilterCondition = this.BuildMultiFilterConditionForReport(AirportId, TempOriginId, TempDestinationId, TempFormType, Route, Direction, StartDate, EndDate, InterviewerId, AirlineId, FlightType, AircraftType);
                    string TargetFilterCondition = this.BuildMultipleFilterConditionForTarget(Instance, AirportId, FieldWorkId, TempFormType, Route, Direction, FlightType, AircraftType, IsTargetZeroRequired, AirlineId);
                    string ResponseDateFilterCondition = this.BuildMultiFilterConditionForResponseDate(AirportId, TempOriginId, TempDestinationId, TempFormType, Route, Direction, StartDate, ResponseDate, InterviewerId, AirlineId, FlightType, AircraftType);
                    DataTable DtArrReport = new DataTable();
                    DtArrReport = DBLayer.GetAirportReport(SessionId, AirportId, FilterCondition, IsDeparture, TargetFilterCondition, ResponseDateFilterCondition);
                    TempFormType = BusinessConstants.FORM_TYPE_DEPARTURE;
                    IsDeparture = true;
                    FilterCondition = this.BuildMultiFilterConditionForReport(AirportId, OriginId, DestinationId, TempFormType, Route, Direction, StartDate, EndDate, InterviewerId, AirlineId, FlightType, AircraftType);
                    TargetFilterCondition = this.BuildMultipleFilterConditionForTarget(Instance, AirportId, FieldWorkId, TempFormType, Route, Direction, FlightType, AircraftType, IsTargetZeroRequired, AirlineId);
                    ResponseDateFilterCondition = this.BuildMultiFilterConditionForResponseDate(AirportId, OriginId, DestinationId, TempFormType, Route, Direction, StartDate, ResponseDate, InterviewerId, AirlineId, FlightType, AircraftType);
                    DataTable DtDprReport = new DataTable();
                    DtDprReport = DBLayer.GetAirportReport(SessionId, AirportId, FilterCondition, IsDeparture, TargetFilterCondition, ResponseDateFilterCondition);
                    DtReport = DtArrReport.Copy();
                    DtReport.Merge(DtDprReport);
                    if (Instance == BusinessConstants.Instance.US.ToString())
                    {
                        var query = from row in DtReport.AsEnumerable()
                                    group row by row.Field<string>(BusinessConstants.REPORT_ORIGINNAME) into grp
                                    orderby grp.Key
                                    select new
                                    {
                                        OriginName = grp.Key,
                                        Target = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_TARGET)),
                                        CardsDistributed = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_CARDSDISTRIBUTED)),
                                        CardsDistributedResponse = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_CARDSDISTRIBUTEDRESPONSE)),
                                        Completes = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_COMPLETES)),
                                        CompletesResponse = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_COMPLETESRESPONSE)),
                                        BusinessCompletes = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_BUSINESSCOMPLETES))
                                    };
                        DtReport = this.ConvertToDataTable(query);
                    }
                    else if (Instance == BusinessConstants.Instance.AIR.ToString())
                    {
                        var query2 = from row in DtReport.AsEnumerable()
                                     group row by new
                                     {
                                         OriginName = row.Field<string>(BusinessConstants.REPORT_ORIGINNAME),
                                         AircraftName = row.Field<string>(BusinessConstants.AIRCRAFTTYPE_NAME)
                                     } into grp
                                     orderby grp.Key.OriginName
                                     select new
                                     {
                                         OriginName = grp.Key.OriginName,
                                         AircraftName = grp.Key.AircraftName,
                                         Target = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_TARGET)),
                                         CardsDistributed = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_CARDSDISTRIBUTED)),
                                         CardsDistributedResponse = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_CARDSDISTRIBUTEDRESPONSE)),
                                         Completes = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_COMPLETES)),
                                         CompletesResponse = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_COMPLETESRESPONSE)),
                                         BusinessCompletes = grp.Sum((DataRow r) => r.Field<Int32>(BusinessConstants.REPORT_BUSINESSCOMPLETES))
                                     };
                        DtReport = this.ConvertToDataTable(query2);
                    }
                }
                else
                {
                    bool IsDeparture = FormType == BusinessConstants.FORM_TYPE_DEPARTURE;
                    string FilterCondition = this.BuildMultiFilterConditionForReport(AirportId, OriginId, DestinationId, FormType, Route, Direction, StartDate, EndDate, InterviewerId, AirlineId, FlightType, AircraftType);
                    string TargetFilterCondition = this.BuildMultipleFilterConditionForTarget(Instance, AirportId, FieldWorkId, FormType, Route, Direction, FlightType, AircraftType, IsTargetZeroRequired, AirlineId);
                    string ResponseDateFilterCondition = this.BuildMultiFilterConditionForReport(AirportId, OriginId, DestinationId, FormType, Route, Direction, StartDate, ResponseDate, InterviewerId, AirlineId, FlightType, AircraftType);
                    SICTLogger.WriteVerbose(ReportingBusiness.CLASS_NAME, "GetAirlineReport", "Start Retieving Airline Report  Data From DB");
                    DtReport = DBLayer.GetAirportReport(SessionId, AirportId, FilterCondition, IsDeparture, TargetFilterCondition, ResponseDateFilterCondition);
                }
                if (DtReport.Rows.Count > 0)
                {
                    foreach (DataRow Dr in DtReport.Rows)
                    {
                        AirportReportDetail TempAirportReportDetail = new AirportReportDetail();
                        double Target = 0.0;
                        if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_TARGET].ToString()))
                        {
                            Target = System.Convert.ToDouble(Dr[BusinessConstants.REPORT_TARGET].ToString());
                        }
                        int CardsDistributed = 0;
                        int Completes = 0;
                        int BusinessCompletes = 0;
                        int RCardsDistributed = 0;
                        int RCompletes = 0;
                        double BTarget;
                        if (Instance == BusinessConstants.Instance.EUR.ToString())
                        {
                            BTarget = Target * 0.25;
                        }
                        else
                        {
                            BTarget = Target * 0.33333333333333331;
                        }
                        if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_CARDSDISTRIBUTED].ToString()))
                        {
                            CardsDistributed = System.Convert.ToInt32(Dr[BusinessConstants.REPORT_CARDSDISTRIBUTED].ToString());
                        }
                        if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_COMPLETES].ToString()))
                        {
                            Completes = System.Convert.ToInt32(Dr[BusinessConstants.REPORT_COMPLETES].ToString());
                        }
                        if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_BUSINESSCOMPLETES].ToString()))
                        {
                            BusinessCompletes = System.Convert.ToInt32(Dr[BusinessConstants.REPORT_BUSINESSCOMPLETES].ToString());
                        }
                        double ResponseRate = 0.0;
                        double BResponseRate = 0.0;
                        if (CardsDistributed > 0)
                        {
                            ResponseRate = System.Convert.ToDouble(Completes) / System.Convert.ToDouble(CardsDistributed) * 100.0;
                        }
                        if (ResponseDate != "-1")
                        {
                            if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_CARDSDISTRIBUTEDRESPONSE].ToString()))
                            {
                                RCardsDistributed = System.Convert.ToInt32(Dr[BusinessConstants.REPORT_CARDSDISTRIBUTEDRESPONSE].ToString());
                            }
                            if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_COMPLETESRESPONSE].ToString()))
                            {
                                RCompletes = System.Convert.ToInt32(Dr[BusinessConstants.REPORT_COMPLETESRESPONSE].ToString());
                            }
                            if (RCardsDistributed > 0)
                            {
                                ResponseRate = System.Convert.ToDouble(RCompletes) / System.Convert.ToDouble(RCardsDistributed) * 100.0;
                            }
                        }
                        if (Completes > 0)
                        {
                            BResponseRate = System.Convert.ToDouble(BusinessCompletes) / System.Convert.ToDouble(Completes) * 100.0;
                        }
                        double TargetAchieved = 0.0;
                        double BTargetAchieved = 0.0;
                        if (Target > 0.0)
                        {
                            TargetAchieved = ((double)Completes / Target * 100.0).Round();
                        }
                        if (BTarget > 0.0)
                        {
                            BTargetAchieved = ((double)BusinessCompletes / BTarget * 100.0).Round();
                        }
                        TempAirportReportDetail.OriginName = Dr[BusinessConstants.REPORT_ORIGINNAME].ToString();
                        if (Instance == BusinessConstants.Instance.EUR.ToString())
                        {
                            TempAirportReportDetail.Type = Dr[BusinessConstants.FLIGHTTYPE].ToString();
                        }
                        else if (Instance == BusinessConstants.Instance.AIR.ToString())
                        {
                            TempAirportReportDetail.Type = Dr[BusinessConstants.AIRCRAFTTYPE_NAME].ToString();
                        }
                        TempAirportReportDetail.Target = Target;
                        TempAirportReportDetail.Distributed = CardsDistributed;
                        TempAirportReportDetail.TotalCompletes = Completes;
                        TempAirportReportDetail.BCompletes = BusinessCompletes;
                        TempAirportReportDetail.ResponseRate = ResponseRate.Round();
                        TempAirportReportDetail.BResponseRate = BResponseRate.Round();
                        TempAirportReportDetail.TargetAchieved = TargetAchieved.Round();
                        TempAirportReportDetail.BTargetAchieved = BTargetAchieved.Round();
                        TempAirportReportDetail.RCardsDistributed = RCardsDistributed;
                        TempAirportReportDetail.RTotalCompletes = RCompletes;
                        AirportReportDetails.Add(TempAirportReportDetail);
                    }
                }
                AirportReportResponse.AirportReportDetails = AirportReportDetails;
                AirportReportResponse.ReturnCode = 1;
                AirportReportResponse.ReturnMessage = "Successfully Retrieved";
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(ReportingBusiness.CLASS_NAME, "GetAirlineReport", Ex);
                AirportReportResponse.ReturnCode = -1;
                AirportReportResponse.ReturnMessage = "Error in API";
            }
            SICTLogger.WriteInfo(ReportingBusiness.CLASS_NAME, "GetAirlineReport", "End ");
            return AirportReportResponse;
        }

        public AircraftReportResponse GetAircraftReport(string Instance, string SessionId, string AirportId, string OriginId, string DestinationId, string FormType, string InterviewerId, string StartDate, string EndDate, string ResponseDate)
        {
            SICTLogger.WriteInfo(ReportingBusiness.CLASS_NAME, "GetAircraftReport", "Start for AirportId -" + AirportId);
            AircraftReportResponse AircraftReportResponse = new AircraftReportResponse();
            System.Collections.Generic.List<AircraftReportDetail> AircraftReportDetails = new System.Collections.Generic.List<AircraftReportDetail>();
            DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
            try
            {
                string FieldWorkId = OriginId;
                if (FormType == BusinessConstants.FORM_TYPE_ARRIVAL)
                {
                    string TempId = OriginId;
                    OriginId = DestinationId;
                    DestinationId = TempId;
                }
                bool IsTargetZeroRequired = System.Convert.ToBoolean(ConfigurationManager.AppSettings[BusinessConstants.ISTARGETZEROREQUIRED].ToString());
                string FilterCondition = this.BuildMultiFilterConditionForReport(AirportId, OriginId, DestinationId, FormType, BusinessConstants.DEFAULT_SELECTION_VALUE, BusinessConstants.DEFAULT_SELECTION_VALUE, StartDate, EndDate, InterviewerId, BusinessConstants.DEFAULT_SELECTION_VALUE, BusinessConstants.DEFAULT_SELECTION_VALUE, BusinessConstants.DEFAULT_SELECTION_VALUE);
                string TargetFilterCondition = this.BuildMultipleFilterConditionForTarget(Instance, AirportId, FieldWorkId, FormType, BusinessConstants.DEFAULT_SELECTION_VALUE, BusinessConstants.DEFAULT_SELECTION_VALUE, BusinessConstants.DEFAULT_SELECTION_VALUE, BusinessConstants.DEFAULT_SELECTION_VALUE, IsTargetZeroRequired, BusinessConstants.DEFAULT_SELECTION_VALUE);
                string FilterConditionResponse = this.BuildMultiFilterConditionForResponseDate(AirportId, OriginId, DestinationId, FormType, BusinessConstants.DEFAULT_SELECTION_VALUE, BusinessConstants.DEFAULT_SELECTION_VALUE, StartDate, ResponseDate, InterviewerId, BusinessConstants.DEFAULT_SELECTION_VALUE, BusinessConstants.DEFAULT_SELECTION_VALUE, BusinessConstants.DEFAULT_SELECTION_VALUE);
                DataTable DtReport = new DataTable();
                SICTLogger.WriteVerbose(ReportingBusiness.CLASS_NAME, "GetAircraftReport", "Start Retieving Aircraft Report  Data From DB");
                DtReport = DBLayer.GetAircraftReport(SessionId, AirportId, FilterCondition, TargetFilterCondition, FilterConditionResponse);
                if (DtReport.Rows.Count > 0)
                {
                    foreach (DataRow Dr in DtReport.Rows)
                    {
                        double Target = 0.0;
                        if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_TARGET].ToString()))
                        {
                            Target = System.Convert.ToDouble(Dr[BusinessConstants.REPORT_TARGET].ToString());
                        }
                        if (Target != 0.0 || !IsTargetZeroRequired)
                        {
                            AircraftReportDetail TempAircraftReportDetails = new AircraftReportDetail();
                            string AircraftName = string.Empty;
                            double CardsDistributed = 0.0;
                            double Completes = 0.0;
                            double BusinessCompletes = 0.0;
                            int RCardsDistributed = 0;
                            int RCompletes = 0;
                            if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_TARGET].ToString()))
                            {
                                AircraftName = Dr[BusinessConstants.AIRCRAFTTYPE].ToString();
                            }
                            double BTarget = 0.33333333333333331 * Target;
                            if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_CARDSDISTRIBUTED].ToString()))
                            {
                                CardsDistributed = System.Convert.ToDouble(Dr[BusinessConstants.REPORT_CARDSDISTRIBUTED].ToString());
                            }
                            if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_COMPLETES].ToString()))
                            {
                                Completes = System.Convert.ToDouble(Dr[BusinessConstants.REPORT_COMPLETES].ToString());
                            }
                            if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_BUSINESSCOMPLETES].ToString()))
                            {
                                BusinessCompletes = System.Convert.ToDouble(Dr[BusinessConstants.REPORT_BUSINESSCOMPLETES].ToString());
                            }
                            double ResponseRate = 0.0;
                            double BResponseRate = 0.0;
                            if (CardsDistributed > 0.0)
                            {
                                ResponseRate = Completes / CardsDistributed * 100.0;
                            }
                            if (ResponseDate != "-1")
                            {
                                if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_CARDSDISTRIBUTEDRESPONSE].ToString()))
                                {
                                    RCardsDistributed = System.Convert.ToInt32(Dr[BusinessConstants.REPORT_CARDSDISTRIBUTEDRESPONSE].ToString());
                                }
                                if (!string.IsNullOrEmpty(Dr[BusinessConstants.REPORT_COMPLETESRESPONSE].ToString()))
                                {
                                    RCompletes = System.Convert.ToInt32(Dr[BusinessConstants.REPORT_COMPLETESRESPONSE].ToString());
                                }
                                if (RCardsDistributed > 0)
                                {
                                    ResponseRate = System.Convert.ToDouble(RCompletes) / System.Convert.ToDouble(RCardsDistributed) * 100.0;
                                }
                            }
                            if (Completes > 0.0)
                            {
                                BResponseRate = BusinessCompletes / Completes * 100.0;
                            }
                            double TargetAchieved = 0.0;
                            double BTargetAchieved = 0.0;
                            if (Target > 0.0)
                            {
                                TargetAchieved = (Completes / Target * 100.0).Round();
                            }
                            if (BTarget > 0.0)
                            {
                                BTargetAchieved = (BusinessCompletes / BTarget * 100.0).Round();
                            }
                            double MissingTarget = Target - Completes;
                            double MissingBTarget = BTarget - BusinessCompletes;
                            TempAircraftReportDetails.AircraftType = AircraftName;
                            TempAircraftReportDetails.Target = Target;
                            TempAircraftReportDetails.Distributed = CardsDistributed.Round();
                            TempAircraftReportDetails.TotalCompletes = Completes.Round();
                            TempAircraftReportDetails.BCompletes = BusinessCompletes.Round();
                            TempAircraftReportDetails.Incompletes = (CardsDistributed - Completes).Round();
                            TempAircraftReportDetails.ResponseRate = ResponseRate.Round();
                            TempAircraftReportDetails.BResponseRate = BResponseRate.Round();
                            TempAircraftReportDetails.TargetAchieved = TargetAchieved.Round();
                            TempAircraftReportDetails.BTargetAchieved = BTargetAchieved.Round();
                            TempAircraftReportDetails.MissingTarget = MissingTarget.Round();
                            TempAircraftReportDetails.MissingBTarget = MissingBTarget.Round();
                            TempAircraftReportDetails.ToDistribute = (Target - CardsDistributed).Round();
                            TempAircraftReportDetails.ToBDistribute = (BTarget - CardsDistributed).Round();
                            TempAircraftReportDetails.RCardsDistributed = RCardsDistributed;
                            TempAircraftReportDetails.RTotalCompletes = RCompletes;
                            AircraftReportDetails.Add(TempAircraftReportDetails);
                        }
                    }
                }
                AircraftReportResponse.AircraftReportDetail = AircraftReportDetails;
                AircraftReportResponse.ReturnCode = 1;
                AircraftReportResponse.ReturnMessage = "Successfully Retrieved";
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(ReportingBusiness.CLASS_NAME, "GetAircraftReport", Ex);
                AircraftReportResponse.ReturnCode = -1;
                AircraftReportResponse.ReturnMessage = "Error in API";
            }
            SICTLogger.WriteInfo(ReportingBusiness.CLASS_NAME, "GetAircraftReport", "End for AirportId -" + AirportId);
            return AircraftReportResponse;
        }

        public AircraftQuotaReportResponse GetAircraftQuotaReport(string Instance, string SessionId, string AirportId, string StartDate, string EndDate, bool IsBusinessQuota)
        {
            SICTLogger.WriteInfo(ReportingBusiness.CLASS_NAME, "GetAircraftReport", "Start for AirportId -" + AirportId);
            AircraftQuotaReportResponse AircraftQuotaReportResponse = new AircraftQuotaReportResponse();
            DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
            try
            {
                string FilterCondition = this.BuildMultiFilterConditionForReport(AirportId, BusinessConstants.DEFAULT_SELECTION_VALUE, BusinessConstants.DEFAULT_SELECTION_VALUE, BusinessConstants.DEFAULT_SELECTION_VALUE, BusinessConstants.DEFAULT_SELECTION_VALUE, BusinessConstants.DEFAULT_SELECTION_VALUE, StartDate, EndDate, BusinessConstants.DEFAULT_SELECTION_VALUE, BusinessConstants.DEFAULT_SELECTION_VALUE, BusinessConstants.DEFAULT_SELECTION_VALUE, BusinessConstants.DEFAULT_SELECTION_VALUE);
                DataSet DSQuota = new DataSet();
                SICTLogger.WriteVerbose(ReportingBusiness.CLASS_NAME, "GetAircraftReport", "Start Retieving Aircraft Quota Report  Data From DB");
                DSQuota = DBLayer.GetAircraftQuotaReport(SessionId, AirportId, FilterCondition, IsBusinessQuota);
                System.Collections.Generic.List<AirlineQuota> Airlines = new System.Collections.Generic.List<AirlineQuota>();
                if (DSQuota.Tables.Count == 3)
                {
                    foreach (DataRow DrAirline in DSQuota.Tables[0].Rows)
                    {
                        AirlineQuota TempAirlineQuota = new AirlineQuota();
                        int AirlineId = System.Convert.ToInt32(DrAirline[BusinessConstants.AIRLINEID].ToString());
                        string AirlineName = DrAirline[BusinessConstants.AIRLINENAME].ToString();
                        TempAirlineQuota.AirlineName = AirlineName;
                        System.Collections.Generic.Dictionary<string, int> Aircrafts = new System.Collections.Generic.Dictionary<string, int>();
                        foreach (DataRow DrAircraft in DSQuota.Tables[1].Rows)
                        {
                            string AircraftName = DrAircraft[BusinessConstants.AIRCRAFTTYPE_NAME].ToString();
                            int QuotaVal = 0;
                            string SelectQuery = string.Format("AirlineId={0} and AircraftName='{1}'", AirlineId, AircraftName);
                            DataRow[] DrVal = DSQuota.Tables[2].Select(SelectQuery);
                            if (DrVal.Length > 0)
                            {
                                QuotaVal = System.Convert.ToInt32(DrVal[0]["Completes"].ToString());
                            }
                            Aircrafts.Add(AircraftName, QuotaVal);
                        }
                        TempAirlineQuota.Aircrafts = Aircrafts;
                        Airlines.Add(TempAirlineQuota);
                    }
                }
                AircraftQuotaReportResponse.Airlines = Airlines;
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(ReportingBusiness.CLASS_NAME, "GetAircraftReport", Ex);
                AircraftQuotaReportResponse.ReturnCode = -1;
                AircraftQuotaReportResponse.ReturnMessage = "Error in API";
            }
            SICTLogger.WriteInfo(ReportingBusiness.CLASS_NAME, "GetAircraftReport", "End for AirportId -" + AirportId);
            return AircraftQuotaReportResponse;
        }
    }
}
