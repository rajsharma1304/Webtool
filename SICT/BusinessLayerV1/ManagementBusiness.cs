using SICT.Constants;
using SICT.DataAccessLayer;
using SICT.DataContracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;

namespace SICT.BusinessLayer.V1
{
    public class ManagementBusiness
    {
        public enum TargetColForDB
        {
            [Description("A.airline_name")]
            AirlineName,
            [Description("AR.airport_name")]
            Origin,
            [Description("D.target")]
            Target,
            [Description("D.type")]
            Type,
            [Description("D.route")]
            Route,
            [Description("D.direction")]
            Direction,
            [Description("D.type")]
            FlightType,
            [Description("D.aircraft_type")]
            AircraftType
        }

        private static readonly string CLASS_NAME = "ManagementBusiness";

        public static string GetEnumDescription(System.Enum value)
        {
            System.Reflection.FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            string result;
            if (attributes != null && attributes.Length > 0)
            {
                result = attributes[0].Description;
            }
            else
            {
                result = value.ToString();
            }
            return result;
        }

        public ReturnValue InsertInterviewer(InterviewerDetail InterviewerDetail)
        {
            ReturnValue ReturnValue = new ReturnValue();
            SICTLogger.WriteInfo(ManagementBusiness.CLASS_NAME, "InsertInterviewer", "Start ");
            try
            {
                DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
                SICTLogger.WriteVerbose(ManagementBusiness.CLASS_NAME, "InsertInterviewer", "Inserting Interviewer to DB Start");
                UserDetailsBusiness udb = new UserDetailsBusiness();
                string AirportCode = DBLayer.GetAirportCodeFromAirportId(InterviewerDetail.AirportId);
                if (udb.CheckIsSpecialUserOrNot(AirportCode))
                {
                    string[] spliresult = InterviewerDetail.InterviewerName.Split(new char[]
                    {
                        '-'
                    });
                    InterviewerDetail.InterviewerName = "VN - " + spliresult[1];
                }
                ReturnValue = DBLayer.InsertInterviewer(InterviewerDetail.AirportId, InterviewerDetail.InterviewerName, InterviewerDetail.IsActive);
                Task.Factory.StartNew<ReturnValue>(() => new CacheFileBusiness().CreateCacheFileforInterviewers());
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(ManagementBusiness.CLASS_NAME, "InsertInterviewer", Ex);
                ReturnValue.ReturnCode = -1;
                ReturnValue.ReturnMessage = "Error in API";
            }
            SICTLogger.WriteInfo(ManagementBusiness.CLASS_NAME, "InsertInterviewer", "End");
            return ReturnValue;
        }

        public ReturnValue UpdateInterviewer(InterviewerDetail InterviewerDetail)
        {
            ReturnValue ReturnValue = new ReturnValue();
            SICTLogger.WriteInfo(ManagementBusiness.CLASS_NAME, "UpdateInterviewer", "Start ");
            try
            {
                DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
                SICTLogger.WriteVerbose(ManagementBusiness.CLASS_NAME, "UpdateInterviewer", "Update Interviewer Details in the DB Start");
                UserDetailsBusiness udb = new UserDetailsBusiness();
                string AirportCode = DBLayer.GetAirportCodeFromAirportId(InterviewerDetail.AirportId);
                if (udb.CheckIsSpecialUserOrNot(AirportCode))
                {
                    string[] spliresult = InterviewerDetail.InterviewerName.Split(new char[]
                    {
                        '-'
                    });
                    InterviewerDetail.InterviewerName = "VN - " + spliresult[1];
                }
                ReturnValue = DBLayer.UpdateInterviewer(InterviewerDetail.AirportId, InterviewerDetail.InterviewerId, InterviewerDetail.InterviewerName, InterviewerDetail.IsActive);
                Task.Factory.StartNew<ReturnValue>(() => new CacheFileBusiness().CreateCacheFileforInterviewers());
            }
            catch (System.Exception Ex)
            {
                ReturnValue.ReturnCode = -1;
                ReturnValue.ReturnMessage = "Error in API";
                SICTLogger.WriteException(ManagementBusiness.CLASS_NAME, "UpdateInterviewer", Ex);
            }
            SICTLogger.WriteInfo(ManagementBusiness.CLASS_NAME, "UpdateInterviewer", "End");
            return ReturnValue;
        }

        public InterviewerDetailResponse GetAllInterviewers()
        {
            System.Collections.Generic.List<InterviewerDetail> InterviewerDetail = new System.Collections.Generic.List<InterviewerDetail>();
            InterviewerDetailResponse InterviewerDetailResponse = new InterviewerDetailResponse();
            SICTLogger.WriteInfo(ManagementBusiness.CLASS_NAME, "GetAllInterviewers", "Start ");
            try
            {
                DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
                SICTLogger.WriteVerbose(ManagementBusiness.CLASS_NAME, "GetAllInterviewers", "Retrieving Interviewer Details from the DB Start");
                DataTable DtInterviewer = new DataTable();
                DtInterviewer = DBLayer.GetAllInterviewers();
                if (DtInterviewer.Rows.Count > 0)
                {
                    foreach (DataRow Dr in DtInterviewer.Rows)
                    {
                        InterviewerDetail.Add(new InterviewerDetail
                        {
                            InterviewerId = System.Convert.ToInt32(Dr[BusinessConstants.INTERVIEWERID].ToString()),
                            InterviewerName = Dr[BusinessConstants.INTERVIEWERNAME].ToString(),
                            AirportId = System.Convert.ToInt32(Dr[BusinessConstants.AIRPORTID].ToString()),
                            AirportName = Dr[BusinessConstants.AIRPORTNAME].ToString(),
                            IsActive = System.Convert.ToBoolean(Dr[BusinessConstants.ISACTIVE].ToString())
                        });
                    }
                }
                InterviewerDetailResponse.InterviewerDetail = InterviewerDetail;
                InterviewerDetailResponse.ReturnCode = 1;
                InterviewerDetailResponse.ReturnMessage = "Successfull Retrieved";
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(ManagementBusiness.CLASS_NAME, "GetAllInterviewers", Ex);
                InterviewerDetailResponse.ReturnCode = -1;
                InterviewerDetailResponse.ReturnMessage = "Error in API";
            }
            SICTLogger.WriteInfo(ManagementBusiness.CLASS_NAME, "GetAllInterviewers", "End");
            return InterviewerDetailResponse;
        }

        public InterviewerDetailResponse GetInterviewersbyRange(int StartIndex, int Offset, string AirportId, string InterviewerName)
        {
            InterviewerDetailResponse InterviewerDetailResponse = new InterviewerDetailResponse();
            System.Collections.Generic.List<InterviewerDetail> InterviewerDetail = new System.Collections.Generic.List<InterviewerDetail>();
            SICTLogger.WriteInfo(ManagementBusiness.CLASS_NAME, "GetInterviewersbyRange", "Start ");
            try
            {
                int Cnt = 0;
                DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
                SICTLogger.WriteVerbose(ManagementBusiness.CLASS_NAME, "GetInterviewersbyRange", "Retrieving Interviewer Details from the DB Start");
                DataTable DtInterviewer = new DataTable();
                DtInterviewer = DBLayer.GetAllInterviewers();
                if (DtInterviewer.Rows.Count > 0)
                {
                    if (AirportId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                    {
                        DataRow[] AirLines = DtInterviewer.Select(BusinessConstants.AIRPORTID + " = " + System.Convert.ToInt32(AirportId));
                        if (AirLines.Length > 0)
                        {
                            DtInterviewer = AirLines.CopyToDataTable<DataRow>();
                        }
                        else
                        {
                            DtInterviewer.Clear();
                        }
                    }
                    if (InterviewerName != BusinessConstants.DEFAULT_SELECTION_VALUE)
                    {
                        DataRow[] Interviewers = DtInterviewer.Select(BusinessConstants.INTERVIEWERNAME + " like '%" + InterviewerName + "%'");
                        if (Interviewers.Length > 0)
                        {
                            DtInterviewer = Interviewers.CopyToDataTable<DataRow>();
                        }
                        else
                        {
                            DtInterviewer.Clear();
                        }
                    }
                    Cnt = DtInterviewer.Rows.Count;
                    int EndIndex = StartIndex + Offset - 1;
                    if (StartIndex > Cnt)
                    {
                        StartIndex = Cnt;
                    }
                    if (EndIndex > Cnt)
                    {
                        EndIndex = Cnt;
                    }
                    for (int RowCnt = StartIndex; RowCnt < EndIndex; RowCnt++)
                    {
                        InterviewerDetail.Add(new InterviewerDetail
                        {
                            InterviewerId = System.Convert.ToInt32(DtInterviewer.Rows[RowCnt][BusinessConstants.INTERVIEWERID].ToString()),
                            InterviewerName = DtInterviewer.Rows[RowCnt][BusinessConstants.INTERVIEWERNAME].ToString(),
                            AirportId = System.Convert.ToInt32(DtInterviewer.Rows[RowCnt][BusinessConstants.AIRPORTID].ToString()),
                            AirportName = DtInterviewer.Rows[RowCnt][BusinessConstants.AIRPORTNAME].ToString(),
                            Code = DtInterviewer.Rows[RowCnt][BusinessConstants.CODE].ToString(),
                            IsActive = System.Convert.ToBoolean(DtInterviewer.Rows[RowCnt][BusinessConstants.ISACTIVE].ToString())
                        });
                    }
                }
                InterviewerDetailResponse.InterviewerDetail = InterviewerDetail;
                InterviewerDetailResponse.RecordsCnt = Cnt;
                InterviewerDetailResponse.ReturnCode = 1;
                InterviewerDetailResponse.ReturnMessage = "Successfully Retrieved";
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(ManagementBusiness.CLASS_NAME, "GetInterviewersbyRange", Ex);
                InterviewerDetailResponse.ReturnCode = -1;
                InterviewerDetailResponse.ReturnMessage = "Error in API";
            }
            SICTLogger.WriteInfo(ManagementBusiness.CLASS_NAME, "GetInterviewersbyRange", "End");
            return InterviewerDetailResponse;
        }

        public ReturnValue InsertTarget(string Instance, TargetDetail TargetDetail)
        {
            ReturnValue ReturnValue = new ReturnValue();
            SICTLogger.WriteInfo(ManagementBusiness.CLASS_NAME, "InsertTarget", "Start ");
            try
            {
                DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
                SICTLogger.WriteVerbose(ManagementBusiness.CLASS_NAME, "InsertTarget", "Inserting Target Details to DB Start");
                ReturnValue = DBLayer.InsertTarget(Instance, TargetDetail.AirlineId, TargetDetail.OriginId, TargetDetail.Target, TargetDetail.Type, TargetDetail.Route, TargetDetail.Direction, TargetDetail.FlightType, TargetDetail.AircraftType);
                Task.Factory.StartNew<ReturnValue>(() => new CacheFileBusiness().CreateCacheFileforTargetVsCompletesCharts(Instance));
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(ManagementBusiness.CLASS_NAME, "InsertTarget", Ex);
                ReturnValue.ReturnCode = -1;
                ReturnValue.ReturnMessage = "Error in API";
            }
            SICTLogger.WriteInfo(ManagementBusiness.CLASS_NAME, "InsertTarget", "End");
            return ReturnValue;
        }

        public ReturnValue UpdateTarget(System.Collections.Generic.List<TargetUpdateDetail> TargetUpdateDetails, string Instance)
        {
            ReturnValue ReturnValue = new ReturnValue();
            SICTLogger.WriteInfo(ManagementBusiness.CLASS_NAME, "UpdateTarget", "Start ");
            try
            {
                DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
                SICTLogger.WriteVerbose(ManagementBusiness.CLASS_NAME, "UpdateTarget", "Update Target Details in the DB Start");
                ReturnValue = DBLayer.UpdateTarget(TargetUpdateDetails);
                Task.Factory.StartNew<ReturnValue>(() => new CacheFileBusiness().CreateCacheFileforTargetVsCompletesCharts(Instance));
            }
            catch (System.Exception Ex)
            {
                ReturnValue.ReturnCode = -1;
                ReturnValue.ReturnMessage = "Error in API";
                SICTLogger.WriteException(ManagementBusiness.CLASS_NAME, "UpdateTarget", Ex);
            }
            SICTLogger.WriteInfo(ManagementBusiness.CLASS_NAME, "UpdateTarget", "End");
            return ReturnValue;
        }

        public ReturnValue DeleteTarget(int DistributionTargetId, string Instance)
        {
            ReturnValue ReturnValue = new ReturnValue();
            SICTLogger.WriteInfo(ManagementBusiness.CLASS_NAME, "DeleteTarget", "Start ");
            try
            {
                DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
                SICTLogger.WriteVerbose(ManagementBusiness.CLASS_NAME, "DeleteTarget", "Delete Target Details in the DB Start");
                ReturnValue = DBLayer.DeleteTarget(DistributionTargetId);
                new CacheFileBusiness().CreateCacheFileforTargetVsCompletesCharts(Instance);
            }
            catch (System.Exception Ex)
            {
                ReturnValue.ReturnCode = -1;
                ReturnValue.ReturnMessage = "Error in API";
                SICTLogger.WriteException(ManagementBusiness.CLASS_NAME, "DeleteTarget", Ex);
            }
            SICTLogger.WriteInfo(ManagementBusiness.CLASS_NAME, "DeleteTarget", "End");
            return ReturnValue;
        }

        public TargetDetailResponse GetTargetsbyRange(string Instance, int StartIndex, int Offset, string FormType, string OriginId, string AirlineId, string Route, string Direction, string FlightType, string AircraftType, string SearchText, string Sort, string IsAsc)
        {
            TargetDetailResponse TargetDetailResponse = new TargetDetailResponse();
            System.Collections.Generic.List<TargetDetail> TargetDetails = new System.Collections.Generic.List<TargetDetail>();
            SICTLogger.WriteInfo(ManagementBusiness.CLASS_NAME, "GetTargetsbyRange", "Start ");
            try
            {
                int Cnt = 0;
                DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
                string OrderByCondition = string.Empty;
                if (Sort != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    bool SortOrder = System.Convert.ToBoolean(IsAsc);
                    switch (Sort)
                    {
                        case "AirlineName":
                            if (SortOrder)
                            {
                                OrderByCondition = ManagementBusiness.GetEnumDescription(ManagementBusiness.TargetColForDB.AirlineName) + " Asc ";
                            }
                            else
                            {
                                OrderByCondition = ManagementBusiness.GetEnumDescription(ManagementBusiness.TargetColForDB.AirlineName) + " Desc ";
                            }
                            break;
                        case "Origin":
                            if (SortOrder)
                            {
                                OrderByCondition = ManagementBusiness.GetEnumDescription(ManagementBusiness.TargetColForDB.Origin) + " Asc ";
                            }
                            else
                            {
                                OrderByCondition = ManagementBusiness.GetEnumDescription(ManagementBusiness.TargetColForDB.Origin) + " Desc ";
                            }
                            break;
                        case "Route":
                            if (SortOrder)
                            {
                                OrderByCondition = ManagementBusiness.GetEnumDescription(ManagementBusiness.TargetColForDB.Route) + " Asc ";
                            }
                            else
                            {
                                OrderByCondition = ManagementBusiness.GetEnumDescription(ManagementBusiness.TargetColForDB.Route) + " Desc ";
                            }
                            break;
                        case "Direction":
                            if (SortOrder)
                            {
                                OrderByCondition = ManagementBusiness.GetEnumDescription(ManagementBusiness.TargetColForDB.Direction) + " Asc ";
                            }
                            else
                            {
                                OrderByCondition = ManagementBusiness.GetEnumDescription(ManagementBusiness.TargetColForDB.Direction) + " Desc ";
                            }
                            break;
                        case "Target":
                            if (SortOrder)
                            {
                                OrderByCondition = ManagementBusiness.GetEnumDescription(ManagementBusiness.TargetColForDB.Target) + " Asc ";
                            }
                            else
                            {
                                OrderByCondition = ManagementBusiness.GetEnumDescription(ManagementBusiness.TargetColForDB.Target) + " Desc ";
                            }
                            break;
                        case "Type":
                            if (SortOrder)
                            {
                                OrderByCondition = ManagementBusiness.GetEnumDescription(ManagementBusiness.TargetColForDB.Type) + " Asc ";
                            }
                            else
                            {
                                OrderByCondition = ManagementBusiness.GetEnumDescription(ManagementBusiness.TargetColForDB.Type) + " Desc ";
                            }
                            break;
                        case "FlightType":
                            if (SortOrder)
                            {
                                OrderByCondition = ManagementBusiness.GetEnumDescription(ManagementBusiness.TargetColForDB.FlightType) + " Asc ";
                            }
                            else
                            {
                                OrderByCondition = ManagementBusiness.GetEnumDescription(ManagementBusiness.TargetColForDB.FlightType) + " Desc ";
                            }
                            break;
                        case "AircraftType":
                            if (SortOrder)
                            {
                                OrderByCondition = ManagementBusiness.GetEnumDescription(ManagementBusiness.TargetColForDB.AircraftType) + " Asc ";
                            }
                            else
                            {
                                OrderByCondition = ManagementBusiness.GetEnumDescription(ManagementBusiness.TargetColForDB.AircraftType) + " Desc ";
                            }
                            break;
                    }
                }
                SICTLogger.WriteVerbose(ManagementBusiness.CLASS_NAME, "GetTargetsbyRange", "Retrieving Target Details from the DB Start");
                DataSet Targets = new DataSet();
                Targets = DBLayer.GetTargetsByRange(StartIndex, Offset, FormType, OriginId, AirlineId, Route, Direction, FlightType, AircraftType, SearchText, OrderByCondition);
                if (Targets.Tables.Count == 2)
                {
                    Cnt = System.Convert.ToInt32(Targets.Tables[0].Rows[0][0].ToString());
                    for (int RowCnt = 0; RowCnt < Targets.Tables[1].Rows.Count; RowCnt++)
                    {
                        TargetDetail TempTargetDetail = new TargetDetail();
                        TempTargetDetail.DistributionTargetId = System.Convert.ToInt32(Targets.Tables[1].Rows[RowCnt][BusinessConstants.DISTRIBUTIONTARGETID].ToString());
                        TempTargetDetail.AirlineId = System.Convert.ToInt32(Targets.Tables[1].Rows[RowCnt][BusinessConstants.AIRLINEID].ToString());
                        TempTargetDetail.AirlineName = Targets.Tables[1].Rows[RowCnt][BusinessConstants.AIRLINENAME].ToString();
                        TempTargetDetail.OriginId = System.Convert.ToInt32(Targets.Tables[1].Rows[RowCnt][BusinessConstants.ORIGINID].ToString());
                        TempTargetDetail.Origin = Targets.Tables[1].Rows[RowCnt][BusinessConstants.ORIGIN].ToString();
                        TempTargetDetail.Code = Targets.Tables[1].Rows[RowCnt][BusinessConstants.CODE].ToString();
                        TempTargetDetail.Target = System.Convert.ToInt32(Targets.Tables[1].Rows[RowCnt][BusinessConstants.TARGET].ToString());
                        if (Instance == BusinessConstants.Instance.US.ToString())
                        {
                            TempTargetDetail.Type = Targets.Tables[1].Rows[RowCnt][BusinessConstants.TYPE].ToString();
                            TempTargetDetail.Route = Targets.Tables[1].Rows[RowCnt][BusinessConstants.ROUTE].ToString();
                            TempTargetDetail.Direction = Targets.Tables[1].Rows[RowCnt][BusinessConstants.DIRECTION].ToString();
                        }
                        else if (Instance == BusinessConstants.Instance.EUR.ToString())
                        {
                            TempTargetDetail.FlightType = Targets.Tables[1].Rows[RowCnt][BusinessConstants.FLIGHTTYPE].ToString();
                        }
                        else if (Instance == BusinessConstants.Instance.AIR.ToString())
                        {
                            TempTargetDetail.Type = Targets.Tables[1].Rows[RowCnt][BusinessConstants.TYPE].ToString();
                            TempTargetDetail.AircraftType = Targets.Tables[1].Rows[RowCnt][BusinessConstants.AIRCRAFTTYPE].ToString();
                        }
                        TargetDetails.Add(TempTargetDetail);
                    }
                }
                TargetDetailResponse.TargetDetails = TargetDetails;
                TargetDetailResponse.RecordsCnt = Cnt;
                TargetDetailResponse.ReturnCode = 1;
                TargetDetailResponse.ReturnMessage = "Successfully Retrieved";
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(ManagementBusiness.CLASS_NAME, "GetTargetsbyRange", Ex);
                TargetDetailResponse.ReturnCode = -1;
                TargetDetailResponse.ReturnMessage = "Error in API";
            }
            SICTLogger.WriteInfo(ManagementBusiness.CLASS_NAME, "GetTargetsbyRange", "End");
            return TargetDetailResponse;
        }

        public FlightResponse InserFlightCombination(string Instance, FlightDetail FlightDetail)
        {
            FlightResponse ReturnValue = new FlightResponse();
            SICTLogger.WriteInfo(ManagementBusiness.CLASS_NAME, "InserFlightCombination", "Start ");
            try
            {
                DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
                SICTLogger.WriteVerbose(ManagementBusiness.CLASS_NAME, "InserFlightCombination", "Inserting FlightDetail Details to DB Start");
                if (FlightDetail.Type == BusinessConstants.FORM_TYPE_ARRIVAL)
                {
                    int TempId = FlightDetail.OriginId;
                    FlightDetail.OriginId = FlightDetail.DestinationId;
                    FlightDetail.DestinationId = TempId;
                }
                ReturnValue = DBLayer.InserFlightCombination(Instance, FlightDetail.AirlineId, FlightDetail.OriginId, FlightDetail.DestinationId, FlightDetail.Type, FlightDetail.Route, FlightDetail.Direction, FlightDetail.FlightType);
                Task.Factory.StartNew<ReturnValue>(() => new CacheFileBusiness().CreateCacheFileforAiprortAndAirline(Instance, true));
                if (Instance == BusinessConstants.Instance.US.ToString() || Instance == BusinessConstants.Instance.AIR.ToString())
                {
                    Task.Factory.StartNew<ReturnValue>(() => new CacheFileBusiness().CreateCacheFileforAiprortAndAirline(Instance, false));
                }
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(ManagementBusiness.CLASS_NAME, "InserFlightCombination", Ex);
                ReturnValue.ReturnCode = -1;
                ReturnValue.ReturnMessage = "Error in API";
            }
            SICTLogger.WriteInfo(ManagementBusiness.CLASS_NAME, "InserFlightCombination", "End");
            return ReturnValue;
        }

        public FlightResponse UpdateFlightComination(string Instance, FlightDetail FlightDetail)
        {
            FlightResponse ReturnValue = new FlightResponse();
            SICTLogger.WriteInfo(ManagementBusiness.CLASS_NAME, "UpdateFlightComination", "Start ");
            try
            {
                DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
                SICTLogger.WriteVerbose(ManagementBusiness.CLASS_NAME, "UpdateFlightComination", "Update FlightComination Details in the DB Start");
                if (FlightDetail.Type == BusinessConstants.FORM_TYPE_ARRIVAL)
                {
                    int TempId = FlightDetail.OriginId;
                    FlightDetail.OriginId = FlightDetail.DestinationId;
                    FlightDetail.DestinationId = TempId;
                }
                ReturnValue = DBLayer.UpdateFlightCombination(Instance, FlightDetail);
                Task.Factory.StartNew<ReturnValue>(() => new CacheFileBusiness().CreateCacheFileforAiprortAndAirline(Instance, true));
                if (Instance == BusinessConstants.Instance.US.ToString() || Instance == BusinessConstants.Instance.AIR.ToString())
                {
                    Task.Factory.StartNew<ReturnValue>(() => new CacheFileBusiness().CreateCacheFileforAiprortAndAirline(Instance, false));
                }
            }
            catch (System.Exception Ex)
            {
                ReturnValue.ReturnCode = -1;
                ReturnValue.ReturnMessage = "Error in API";
                SICTLogger.WriteException(ManagementBusiness.CLASS_NAME, "UpdateFlightComination", Ex);
            }
            SICTLogger.WriteInfo(ManagementBusiness.CLASS_NAME, "UpdateFlightComination", "End");
            return ReturnValue;
        }

        public FlightDeleteResponse DeleteFlightCombination(string Instance, int FlightCombinationId)
        {
            FlightDeleteResponse ReturnValue = new FlightDeleteResponse();
            SICTLogger.WriteInfo(ManagementBusiness.CLASS_NAME, "DeleteFlightCombination", "Start ");
            try
            {
                DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
                SICTLogger.WriteVerbose(ManagementBusiness.CLASS_NAME, "DeleteFlightCombination", "Delete Target Details in the DB Start");
                ReturnValue = DBLayer.DeleteFlightCombination(FlightCombinationId);
                new CacheFileBusiness().CreateCacheFileforAiprortAndAirline(Instance, true);
                if (Instance == BusinessConstants.Instance.US.ToString() || Instance == BusinessConstants.Instance.AIR.ToString())
                {
                    new CacheFileBusiness().CreateCacheFileforAiprortAndAirline(Instance, false);
                }
            }
            catch (System.Exception Ex)
            {
                ReturnValue.ReturnCode = -1;
                ReturnValue.ReturnMessage = "Error in API";
                SICTLogger.WriteException(ManagementBusiness.CLASS_NAME, "DeleteFlightCombination", Ex);
            }
            SICTLogger.WriteInfo(ManagementBusiness.CLASS_NAME, "DeleteFlightCombination", "End");
            return ReturnValue;
        }

        public FlightCombinationResponse GetFlightCombinationsByRange(string Instance, int StartIndex, int Offset, string FormType, string OriginId, string DestinationId, string AirlineId, string Route, string Direction, string FlightType, string SearchVal, string SortVal, bool SortOrder)
        {
            FlightCombinationResponse FlightCombinationResponse = new FlightCombinationResponse();
            System.Collections.Generic.List<FlightDetail> FlightDetails = new System.Collections.Generic.List<FlightDetail>();
            SICTLogger.WriteInfo(ManagementBusiness.CLASS_NAME, "GetFlightCombinationsByRange", "Start ");
            try
            {
                if (FormType == BusinessConstants.FORM_TYPE_ARRIVAL)
                {
                    string TempId = OriginId;
                    OriginId = DestinationId;
                    DestinationId = TempId;
                }
                int Cnt = 0;
                DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
                SICTLogger.WriteVerbose(ManagementBusiness.CLASS_NAME, "GetFlightCombinationsByRange", "Retrieving Flight Combination Details from the DB Start");
                DataSet DSFlights = new DataSet();
                string OrderByCondition = string.Empty;
                string WhereCondition = string.Empty;
                this.BuildOrderByandWhereConditionsForFlights(Instance, SortVal, SortOrder, SearchVal, FormType, OriginId, DestinationId, AirlineId, Route, Direction, FlightType, ref OrderByCondition, ref WhereCondition);
                DSFlights = DBLayer.GetFlightCombinationByRange(StartIndex, Offset, OrderByCondition, WhereCondition);
                if (DSFlights.Tables.Count == 2)
                {
                    if (!string.IsNullOrEmpty(DSFlights.Tables[0].Rows[0][0].ToString()))
                    {
                        Cnt = System.Convert.ToInt32(DSFlights.Tables[0].Rows[0][0].ToString());
                    }
                    for (int RowCnt = 0; RowCnt < DSFlights.Tables[1].Rows.Count; RowCnt++)
                    {
                        FlightDetail TempFlightDetails = new FlightDetail();
                        TempFlightDetails.FlightCombinationId = System.Convert.ToInt32(DSFlights.Tables[1].Rows[RowCnt][BusinessConstants.FLIGHTCOMBINATIONID].ToString());
                        TempFlightDetails.AirlineId = System.Convert.ToInt32(DSFlights.Tables[1].Rows[RowCnt][BusinessConstants.AIRLINEID].ToString());
                        TempFlightDetails.AirlineName = DSFlights.Tables[1].Rows[RowCnt][BusinessConstants.AIRLINENAME].ToString();
                        TempFlightDetails.OriginId = System.Convert.ToInt32(DSFlights.Tables[1].Rows[RowCnt][BusinessConstants.ORIGINID].ToString());
                        TempFlightDetails.Origin = DSFlights.Tables[1].Rows[RowCnt][BusinessConstants.ORIGIN].ToString();
                        TempFlightDetails.DestinationId = System.Convert.ToInt32(DSFlights.Tables[1].Rows[RowCnt][BusinessConstants.DESTINATIONID].ToString());
                        TempFlightDetails.Destination = DSFlights.Tables[1].Rows[RowCnt][BusinessConstants.DESTINATION].ToString();
                        if (Instance == BusinessConstants.Instance.US.ToString())
                        {
                            string Type = DSFlights.Tables[1].Rows[RowCnt][BusinessConstants.TYPE].ToString();
                            TempFlightDetails.Route = DSFlights.Tables[1].Rows[RowCnt][BusinessConstants.ROUTE].ToString();
                            TempFlightDetails.Direction = DSFlights.Tables[1].Rows[RowCnt][BusinessConstants.DIRECTION].ToString();
                            TempFlightDetails.Type = Type;
                            if (Type == BusinessConstants.FORM_TYPE_ARRIVAL)
                            {
                                TempFlightDetails.OriginId = System.Convert.ToInt32(DSFlights.Tables[1].Rows[RowCnt][BusinessConstants.DESTINATIONID].ToString());
                                TempFlightDetails.Origin = DSFlights.Tables[1].Rows[RowCnt][BusinessConstants.DESTINATION].ToString();
                                TempFlightDetails.DestinationId = System.Convert.ToInt32(DSFlights.Tables[1].Rows[RowCnt][BusinessConstants.ORIGINID].ToString());
                                TempFlightDetails.Destination = DSFlights.Tables[1].Rows[RowCnt][BusinessConstants.ORIGIN].ToString();
                            }
                        }
                        else if (Instance == BusinessConstants.Instance.AIR.ToString())
                        {
                            string Type = DSFlights.Tables[1].Rows[RowCnt][BusinessConstants.TYPE].ToString();
                            TempFlightDetails.Type = DSFlights.Tables[1].Rows[RowCnt][BusinessConstants.TYPE].ToString();
                            if (Type == BusinessConstants.FORM_TYPE_ARRIVAL)
                            {
                                TempFlightDetails.OriginId = System.Convert.ToInt32(DSFlights.Tables[1].Rows[RowCnt][BusinessConstants.DESTINATIONID].ToString());
                                TempFlightDetails.Origin = DSFlights.Tables[1].Rows[RowCnt][BusinessConstants.DESTINATION].ToString();
                                TempFlightDetails.DestinationId = System.Convert.ToInt32(DSFlights.Tables[1].Rows[RowCnt][BusinessConstants.ORIGINID].ToString());
                                TempFlightDetails.Destination = DSFlights.Tables[1].Rows[RowCnt][BusinessConstants.ORIGIN].ToString();
                            }
                        }
                        else
                        {
                            TempFlightDetails.Type = BusinessConstants.FORM_TYPE_DEPARTURE;
                        }
                        if (Instance == BusinessConstants.Instance.EUR.ToString())
                        {
                            TempFlightDetails.FlightType = DSFlights.Tables[1].Rows[RowCnt][BusinessConstants.FLIGHTTYPE].ToString();
                        }
                        FlightDetails.Add(TempFlightDetails);
                    }
                }
                FlightCombinationResponse.FlightDetails = FlightDetails;
                FlightCombinationResponse.RecordsCnt = Cnt;
                FlightCombinationResponse.ReturnCode = 1;
                FlightCombinationResponse.ReturnMessage = "Successfully Retrieved";
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(ManagementBusiness.CLASS_NAME, "GetFlightCombinationsByRange", Ex);
                FlightCombinationResponse.ReturnCode = -1;
                FlightCombinationResponse.ReturnMessage = "Error in API";
            }
            SICTLogger.WriteInfo(ManagementBusiness.CLASS_NAME, "GetFlightCombinationsByRange", "End");
            return FlightCombinationResponse;
        }

        public ConfirmitDetailsResponse GetConfirmitCardData(int StartIndex, int Offset, string SearchVal, string SortVal, bool SortOrder)
        {
            ConfirmitDetailsResponse ConfirmitDetailsResponse = new ConfirmitDetailsResponse();
            SICTLogger.WriteInfo(ManagementBusiness.CLASS_NAME, "GetConfirmitCardDetails", "Start ");
            try
            {
                System.Collections.Generic.List<ConfirmitDetail> ConfirmitDetailsData = new System.Collections.Generic.List<ConfirmitDetail>();
                DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
                SICTLogger.WriteVerbose(ManagementBusiness.CLASS_NAME, "GetConfirmitCardDetails", "Retrieving Card Details from the DB Start");
                DataSet DsCrads = new DataSet();
                string FilterCondition = string.Empty;
                string OrderByCondition = string.Empty;
                this.BuildOrderByandWhereConditionsForConfirmitData(SearchVal, SortVal, SortOrder, ref OrderByCondition, ref FilterCondition);
                DsCrads = DBLayer.GetConfirmitData(StartIndex, Offset, OrderByCondition, FilterCondition);
                if (DsCrads.Tables.Count == 2)
                {
                    long RecordsCnt = 0L;
                    if (DsCrads.Tables[0].Rows.Count > 0)
                    {
                        RecordsCnt = System.Convert.ToInt64(DsCrads.Tables[0].Rows[0][BusinessConstants.FORM_TOTALRECORDS].ToString());
                    }
                    foreach (DataRow Dr in DsCrads.Tables[1].Rows)
                    {
                        ConfirmitDetail TempConfirmitDetails = new ConfirmitDetail();
                        TempConfirmitDetails.RowCnt = System.Convert.ToInt64(Dr[BusinessConstants.CONFIRMIT_ROWNUMBER].ToString());
                        TempConfirmitDetails.CardNumber = System.Convert.ToInt64(Dr[BusinessConstants.CONFIRMIT_CARDNUMBER].ToString());
                        if (!string.IsNullOrEmpty(Dr[BusinessConstants.CONFIRMIT_CLSS].ToString()))
                        {
                            TempConfirmitDetails.Class = Dr[BusinessConstants.CONFIRMIT_CLSS].ToString();
                        }
                        if (!string.IsNullOrEmpty(Dr[BusinessConstants.CONFIRMIT_UPLOADEDATE].ToString()))
                        {
                            TempConfirmitDetails.UploadeDate = System.Convert.ToDateTime(Dr[BusinessConstants.CONFIRMIT_UPLOADEDATE].ToString()).Date.ToString("MM/DD/YY");
                        }
                        if (!string.IsNullOrEmpty(Dr[BusinessConstants.AIRPORTNAME].ToString()))
                        {
                            TempConfirmitDetails.AirportName = Dr[BusinessConstants.AIRPORTNAME].ToString();
                        }
                        if (!string.IsNullOrEmpty(Dr[BusinessConstants.STATUS].ToString()))
                        {
                            TempConfirmitDetails.Status = Dr[BusinessConstants.STATUS].ToString();
                        }
                        ConfirmitDetailsData.Add(TempConfirmitDetails);
                    }
                    ConfirmitDetailsResponse.RecordsCnt = RecordsCnt;
                    ConfirmitDetailsResponse.ConfirmitDetails = ConfirmitDetailsData;
                    ConfirmitDetailsResponse.ReturnCode = 1;
                    ConfirmitDetailsResponse.ReturnMessage = "Successfully Retrieved";
                }
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(ManagementBusiness.CLASS_NAME, "GetConfirmitCardDetails", Ex);
                ConfirmitDetailsResponse.ReturnCode = -1;
                ConfirmitDetailsResponse.ReturnMessage = "Error in API";
            }
            SICTLogger.WriteInfo(ManagementBusiness.CLASS_NAME, "GetConfirmitCardDetails", "End");
            return ConfirmitDetailsResponse;
        }

        public ConfirmitCountsResponse GetConfirmitCounts()
        {
            ConfirmitCountsResponse ConfirmitCountsResponse = new ConfirmitCountsResponse();
            SICTLogger.WriteInfo(ManagementBusiness.CLASS_NAME, "GetConfirmitCounts", "Start ");
            try
            {
                long Completes = 0L;
                long BCompletes = 0L;
                DataAccessLayer.DataAccessLayer DBLayer = new DataAccessLayer.DataAccessLayer();
                SICTLogger.WriteVerbose(ManagementBusiness.CLASS_NAME, "GetConfirmitCounts", "Retrieving Counts from the DB Start");
                DataTable DtCompletes = new DataTable();
                DtCompletes = DBLayer.GetConfirmitCounts();
                if (DtCompletes.Rows.Count > 0)
                {
                    Completes = System.Convert.ToInt64(DtCompletes.Rows[0][BusinessConstants.CONFIRMIT_COMPLETESCOUNT].ToString());
                    BCompletes = System.Convert.ToInt64(DtCompletes.Rows[0][BusinessConstants.CONFIRMIT_BUSINESSCOUNT].ToString());
                }
                ConfirmitCountsResponse.Completes = Completes;
                ConfirmitCountsResponse.BusinessCompletes = BCompletes;
                ConfirmitCountsResponse.ReturnCode = 1;
                ConfirmitCountsResponse.ReturnMessage = "Count Successfully Retrieved";
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(ManagementBusiness.CLASS_NAME, "GetConfirmitCounts", Ex);
                ConfirmitCountsResponse.ReturnCode = -1;
                ConfirmitCountsResponse.ReturnMessage = "Error in API";
            }
            SICTLogger.WriteInfo(ManagementBusiness.CLASS_NAME, "GetConfirmitCounts", "End");
            return ConfirmitCountsResponse;
        }

        public void BuildOrderByandWhereConditionsForConfirmitData(string SearchVal, string SortVal, bool SortOrder, ref string OrderByCondition, ref string WhereCondition)
        {
            SICTLogger.WriteInfo(ManagementBusiness.CLASS_NAME, "BuildOrderByandWhereConditionsForConfirmitData", "BuildOrderByandWhereConditionsForConfirmitDataStart");
            try
            {
                SICTLogger.WriteVerbose(ManagementBusiness.CLASS_NAME, "BuildOrderByandWhereConditionsForConfirmitData", "Building Sort By Condition");
                string FilterCondition = string.Empty;
                string SearchCondition = string.Empty;
                if (BusinessConstants.DEFAULT_SELECTION_VALUE != SortVal)
                {
                    if (SortVal != null)
                    {
                        if (!(SortVal == "CardNumber"))
                        {
                            if (!(SortVal == "UploadDate"))
                            {
                                if (!(SortVal == "Status"))
                                {
                                    if (!(SortVal == "Class"))
                                    {
                                        if (SortVal == "AirportName")
                                        {
                                            if (SortOrder)
                                            {
                                                OrderByCondition = BusinessConstants.TABLE_CONFIRMIT_AIRPORTNAME + " Asc ";
                                            }
                                            else
                                            {
                                                OrderByCondition = BusinessConstants.TABLE_CONFIRMIT_AIRPORTNAME + " Desc ";
                                            }
                                        }
                                    }
                                    else if (SortOrder)
                                    {
                                        OrderByCondition = BusinessConstants.TABLE_CONFIRMIT_CLASS + " Asc ";
                                    }
                                    else
                                    {
                                        OrderByCondition = BusinessConstants.TABLE_CONFIRMIT_CLASS + " Desc ";
                                    }
                                }
                                else if (SortOrder)
                                {
                                    OrderByCondition = BusinessConstants.TABLE_CONFIRMIT_STATUS + " Asc ";
                                }
                                else
                                {
                                    OrderByCondition = BusinessConstants.TABLE_CONFIRMIT_STATUS + " Desc ";
                                }
                            }
                            else if (SortOrder)
                            {
                                OrderByCondition = BusinessConstants.TABLE_CONFIRMIT_DATE + " Asc ";
                            }
                            else
                            {
                                OrderByCondition = BusinessConstants.TABLE_CONFIRMIT_DATE + " Desc ";
                            }
                        }
                        else if (SortOrder)
                        {
                            OrderByCondition = BusinessConstants.TABLE_CONFIRMIT_CARDNUMBER + " Asc ";
                        }
                        else
                        {
                            OrderByCondition = BusinessConstants.TABLE_CONFIRMIT_CARDNUMBER + " Desc ";
                        }
                    }
                }
                SICTLogger.WriteVerbose(ManagementBusiness.CLASS_NAME, "BuildOrderByandWhereConditionsForConfirmitData", "Building Search Condition");
                System.Collections.Generic.List<string> SearchFilterConditions = new System.Collections.Generic.List<string>();
                if (BusinessConstants.DEFAULT_SELECTION_VALUE != SearchVal)
                {
                    SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, BusinessConstants.TABLE_CONFIRMIT_CARDNUMBER, SearchVal));
                    SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, BusinessConstants.TABLE_CONFIRMIT_DATE, SearchVal));
                    SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, BusinessConstants.TABLE_CONFIRMIT_STATUS, SearchVal));
                    SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, BusinessConstants.TABLE_CONFIRMIT_CLASS, SearchVal));
                    SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, BusinessConstants.TABLE_CONFIRMIT_AIRPORTNAME, SearchVal));
                    WhereCondition = string.Join(BusinessConstants.SPLIT_OR, SearchFilterConditions);
                }
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(ManagementBusiness.CLASS_NAME, "BuildOrderByandWhereConditionsForConfirmitData", Ex);
            }
            SICTLogger.WriteInfo(ManagementBusiness.CLASS_NAME, "BuildOrderByandWhereConditionsForConfirmitData", "BuildOrderByandWhereConditionsForConfirmitDataEnd");
        }

        public void BuildOrderByandWhereConditionsForFlights(string Instance, string SortVal, bool SortOrder, string SearchVal, string FormType, string OriginId, string DestinationId, string AirlineId, string Route, string Direction, string FlightType, ref string OrderByCondition, ref string WhereCondition)
        {
            SICTLogger.WriteInfo(ManagementBusiness.CLASS_NAME, "BuildOrderByandWhereConditionsForFlights", "BuildOrderByandWhereConditionsForFlightsStart");
            try
            {
                string FilterCondition = string.Empty;
                SICTLogger.WriteVerbose(ManagementBusiness.CLASS_NAME, "BuildOrderByandWhereConditionsForFlights", "Building Where Condition");
                System.Collections.Generic.List<string> FilterConditions = new System.Collections.Generic.List<string>();
                string FilterFormat = "{0} = {1}";
                string StringFilterFormat = "{0} = '{1}'";
                string SearchCondition = string.Empty;
                if (BusinessConstants.DEFAULT_SELECTION_VALUE != SortVal)
                {
                    switch (SortVal)
                    {
                        case "FlightCombinationId":
                            if (SortOrder)
                            {
                                OrderByCondition = BusinessConstants.TABLE_FLIGHT_FLIGHTCOMBINATIONID + " Asc ";
                            }
                            else
                            {
                                OrderByCondition = BusinessConstants.TABLE_FLIGHT_FLIGHTCOMBINATIONID + " Desc ";
                            }
                            break;
                        case "AirlineId":
                            if (SortOrder)
                            {
                                OrderByCondition = BusinessConstants.TABLE_FLIGHT_AIRLINEID + " Asc ";
                            }
                            else
                            {
                                OrderByCondition = BusinessConstants.TABLE_FLIGHT_AIRLINEID + " Desc ";
                            }
                            break;
                        case "AirlineName":
                            if (SortOrder)
                            {
                                OrderByCondition = BusinessConstants.TABLE_FLIGHT_AIRLINENAME + " Asc ";
                            }
                            else
                            {
                                OrderByCondition = BusinessConstants.TABLE_FLIGHT_AIRLINENAME + " Desc ";
                            }
                            break;
                        case "Origin":
                            if (SortOrder)
                            {
                                OrderByCondition = BusinessConstants.TABLE_FLIGHT_ORIGIN + " Asc ";
                            }
                            else
                            {
                                OrderByCondition = BusinessConstants.TABLE_FLIGHT_ORIGIN + " Desc ";
                            }
                            break;
                        case "Destination":
                            if (SortOrder)
                            {
                                OrderByCondition = BusinessConstants.TABLE_FLIGHT_DESTINATION + " Asc ";
                            }
                            else
                            {
                                OrderByCondition = BusinessConstants.TABLE_FLIGHT_DESTINATION + " Desc ";
                            }
                            break;
                        case "Type":
                            if (SortOrder)
                            {
                                OrderByCondition = BusinessConstants.TABLE_FLIGHT_TYPE + " Asc ";
                            }
                            else
                            {
                                OrderByCondition = BusinessConstants.TABLE_FLIGHT_TYPE + " Desc ";
                            }
                            break;
                        case "Route":
                            if (SortOrder)
                            {
                                OrderByCondition = BusinessConstants.TABLE_FLIGHT_ROUTE + " Asc ";
                            }
                            else
                            {
                                OrderByCondition = BusinessConstants.TABLE_FLIGHT_ROUTE + " Desc ";
                            }
                            break;
                        case "Direction":
                            if (SortOrder)
                            {
                                OrderByCondition = BusinessConstants.TABLE_FLIGHT_DIRECTION + " Asc ";
                            }
                            else
                            {
                                OrderByCondition = BusinessConstants.TABLE_FLIGHT_DIRECTION + " Desc ";
                            }
                            break;
                        case "FlightType":
                            if (SortOrder)
                            {
                                OrderByCondition = BusinessConstants.TABLE_FLIGHT_FLIGHTTYPE + " Asc ";
                            }
                            else
                            {
                                OrderByCondition = BusinessConstants.TABLE_FLIGHT_FLIGHTTYPE + " Desc ";
                            }
                            break;
                    }
                }
                if (FormType != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    FilterConditions.Add(string.Format(StringFilterFormat, BusinessConstants.TABLE_FLIGHT_TYPE, FormType));
                    if (OriginId != BusinessConstants.DEFAULT_SELECTION_VALUE && DestinationId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                    {
                        System.Collections.Generic.List<string> OandDConditions = new System.Collections.Generic.List<string>();
                        OandDConditions.Add(string.Format(FilterFormat, BusinessConstants.TABLE_FLIGHT_ORIGINID, System.Convert.ToInt32(OriginId)));
                        OandDConditions.Add(string.Format(FilterFormat, BusinessConstants.TABLE_FLIGHT_DESTINATIONID, System.Convert.ToInt32(DestinationId)));
                        FilterConditions.Add(string.Format("({0})", string.Join(BusinessConstants.SPLIT_AND, OandDConditions)));
                    }
                    else if (OriginId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                    {
                        FilterConditions.Add(string.Format(FilterFormat, BusinessConstants.TABLE_FLIGHT_ORIGINID, System.Convert.ToInt32(OriginId)));
                    }
                    else if (DestinationId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                    {
                        FilterConditions.Add(string.Format(FilterFormat, BusinessConstants.TABLE_FLIGHT_DESTINATIONID, System.Convert.ToInt32(DestinationId)));
                    }
                }
                else if (Instance == BusinessConstants.Instance.US.ToString())
                {
                    if (OriginId != BusinessConstants.DEFAULT_SELECTION_VALUE && DestinationId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                    {
                        System.Collections.Generic.List<string> OandDConditions = new System.Collections.Generic.List<string>();
                        OandDConditions.Add(string.Format(FilterFormat, BusinessConstants.TABLE_FLIGHT_ORIGINID, System.Convert.ToInt32(OriginId)));
                        OandDConditions.Add(string.Format(FilterFormat, BusinessConstants.TABLE_FLIGHT_DESTINATIONID, System.Convert.ToInt32(DestinationId)));
                        OandDConditions.Add(string.Format(FilterFormat, BusinessConstants.TABLE_FLIGHT_ORIGINID, System.Convert.ToInt32(DestinationId)));
                        OandDConditions.Add(string.Format(FilterFormat, BusinessConstants.TABLE_FLIGHT_DESTINATIONID, System.Convert.ToInt32(OriginId)));
                        FilterConditions.Add(string.Format("({0})", string.Join(BusinessConstants.SPLIT_OR, OandDConditions)));
                    }
                    else if (OriginId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                    {
                        System.Collections.Generic.List<string> OandDConditions = new System.Collections.Generic.List<string>();
                        OandDConditions.Add(string.Format(FilterFormat, BusinessConstants.TABLE_FLIGHT_ORIGINID, System.Convert.ToInt32(OriginId)));
                        OandDConditions.Add(string.Format(FilterFormat, BusinessConstants.TABLE_FLIGHT_DESTINATIONID, System.Convert.ToInt32(OriginId)));
                        FilterConditions.Add(string.Format("({0})", string.Join(BusinessConstants.SPLIT_OR, OandDConditions)));
                    }
                    else if (DestinationId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                    {
                        System.Collections.Generic.List<string> OandDConditions = new System.Collections.Generic.List<string>();
                        OandDConditions.Add(string.Format(FilterFormat, BusinessConstants.TABLE_FLIGHT_ORIGINID, System.Convert.ToInt32(DestinationId)));
                        OandDConditions.Add(string.Format(FilterFormat, BusinessConstants.TABLE_FLIGHT_DESTINATIONID, System.Convert.ToInt32(DestinationId)));
                        FilterConditions.Add(string.Format("({0})", string.Join(BusinessConstants.SPLIT_OR, OandDConditions)));
                    }
                }
                else if (OriginId != BusinessConstants.DEFAULT_SELECTION_VALUE && DestinationId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    System.Collections.Generic.List<string> OandDConditions = new System.Collections.Generic.List<string>();
                    OandDConditions.Add(string.Format(FilterFormat, BusinessConstants.TABLE_FLIGHT_ORIGINID, System.Convert.ToInt32(OriginId)));
                    OandDConditions.Add(string.Format(FilterFormat, BusinessConstants.TABLE_FLIGHT_DESTINATIONID, System.Convert.ToInt32(DestinationId)));
                    FilterConditions.Add(string.Format("({0})", string.Join(BusinessConstants.SPLIT_AND, OandDConditions)));
                }
                else if (OriginId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    FilterConditions.Add(string.Format(FilterFormat, BusinessConstants.TABLE_FLIGHT_ORIGINID, System.Convert.ToInt32(OriginId)));
                }
                else if (DestinationId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    FilterConditions.Add(string.Format(FilterFormat, BusinessConstants.TABLE_FLIGHT_DESTINATIONID, System.Convert.ToInt32(DestinationId)));
                }
                if (AirlineId != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    FilterConditions.Add(string.Format(FilterFormat, BusinessConstants.TABLE_FLIGHT_AIRLINEID, System.Convert.ToInt32(AirlineId)));
                }
                if (Route != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    FilterConditions.Add(string.Format(StringFilterFormat, BusinessConstants.TABLE_FLIGHT_ROUTE, Route));
                }
                if (Direction != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    FilterConditions.Add(string.Format(StringFilterFormat, BusinessConstants.TABLE_FLIGHT_DIRECTION, Direction));
                }
                if (FlightType != BusinessConstants.DEFAULT_SELECTION_VALUE)
                {
                    FilterConditions.Add(string.Format(StringFilterFormat, BusinessConstants.TABLE_FLIGHT_FLIGHTTYPE, FlightType));
                }
                FilterCondition = string.Join(BusinessConstants.SPLIT_AND, FilterConditions);
                SICTLogger.WriteVerbose(ManagementBusiness.CLASS_NAME, "BuildOrderByandWhereConditionsForFlights", "Building Search Condition");
                System.Collections.Generic.List<string> SearchFilterConditions = new System.Collections.Generic.List<string>();
                if (BusinessConstants.DEFAULT_SELECTION_VALUE != SearchVal)
                {
                    SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, BusinessConstants.TABLE_FLIGHT_FLIGHTCOMBINATIONID, SearchVal));
                    SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, BusinessConstants.TABLE_FLIGHT_AIRLINEID, SearchVal));
                    SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, BusinessConstants.TABLE_FLIGHT_AIRLINENAME, SearchVal));
                    SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, BusinessConstants.TABLE_FLIGHT_ORIGIN, SearchVal));
                    SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, BusinessConstants.TABLE_FLIGHT_DESTINATION, SearchVal));
                    if (Instance == BusinessConstants.Instance.US.ToString())
                    {
                        SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, BusinessConstants.TABLE_FLIGHT_TYPE, SearchVal));
                        SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, BusinessConstants.TABLE_FLIGHT_ROUTE, SearchVal));
                        SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, BusinessConstants.TABLE_FLIGHT_DIRECTION, SearchVal));
                    }
                    else if (Instance == BusinessConstants.Instance.EUR.ToString())
                    {
                        SearchFilterConditions.Add(string.Format(BusinessConstants.WHERECONDITIONFORMAT, BusinessConstants.TABLE_FLIGHT_FLIGHTTYPE, SearchVal));
                    }
                    SearchCondition = string.Join(BusinessConstants.SPLIT_OR, SearchFilterConditions);
                }
                if (!string.IsNullOrEmpty(FilterCondition) && !string.IsNullOrEmpty(SearchCondition))
                {
                    WhereCondition = string.Format("(({0}) and ({1}))", FilterCondition, SearchCondition);
                }
                else if (!string.IsNullOrEmpty(FilterCondition))
                {
                    WhereCondition = string.Format("({0})", FilterCondition);
                }
                else if (!string.IsNullOrEmpty(SearchCondition))
                {
                    WhereCondition = string.Format("({0})", SearchCondition);
                }
            }
            catch (System.Exception Ex)
            {
                SICTLogger.WriteException(ManagementBusiness.CLASS_NAME, "BuildOrderByandWhereConditionsForFlights", Ex);
            }
            SICTLogger.WriteInfo(ManagementBusiness.CLASS_NAME, "BuildOrderByandWhereConditionsForFlights", "BuildOrderByandWhereConditionsForFlightsEnd");
        }
    }
}
