///<Copyright> Cross-Tab  </Copyright>
///<ProjectName>SICT </ProjectName>
///<FileName>ManagementServices.svc </FileName>
///<CreatedOn> 18 Feb 2015</CreatedOn>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using SICT.DataContracts;
using SICT.BusinessLayer.V1;
using SICT.Interface;
using SICT.CommonBusiness;
using SICT.Constants;

namespace SICT.Service
{

    public class ManagementServices : IManagementServices
    {
        private static readonly string CLASS_NAME = "ManagementServices";
        #region MasterPageImplementation

        #region Interviewer
        [WebInvoke(
           Method = "PUT",
           BodyStyle = WebMessageBodyStyle.WrappedRequest,
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json,
           UriTemplate = "Interviewer/Add")]
        public ReturnValue InsertInterviewer(string Instance, string SessionId, string Version, InterviewerDetail InterviewerDetail)
        {
            const string FUNCTION_NAME = "InserInterviewer";
            ReturnValue ReturnValue = new ReturnValue();
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start");
            try
            {
                UserDetailsBusiness ObjSessionValidation = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
                ManagementBusiness ObjManagementBusiness = new FactoryBusiness().GetManagementBusiness(Version);
                if (ObjSessionValidation.IsSessionIdValid(SessionId))
                {
                    ReturnValue = ObjManagementBusiness.InsertInterviewer(InterviewerDetail);
                }
                else
                {
                    ReturnValue.ReturnCode = 0;
                    ReturnValue.ReturnMessage = "Invalid session";
                    SICTLogger.WriteWarning(CLASS_NAME, FUNCTION_NAME, "Invalid session ");
                }
            }
            catch (Exception ex)
            {
                ReturnValue.ReturnCode = -1;
                ReturnValue.ReturnMessage = "Error in API Execution";
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, ex);
            }
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "End");
            return ReturnValue;
        }


        [WebInvoke(
          Method = "POST",
          BodyStyle = WebMessageBodyStyle.WrappedRequest,
          RequestFormat = WebMessageFormat.Json,
          ResponseFormat = WebMessageFormat.Json,
          UriTemplate = "Interviewer/Update")]
        public ReturnValue UpdateInterviewer(string Instance, string SessionId, string Version, InterviewerDetail InterviewerDetail)
        {
            const string FUNCTION_NAME = "UpdateInterviewer";
            ReturnValue ReturnValue = new ReturnValue();
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start");
            try
            {
                UserDetailsBusiness ObjSessionValidation = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
                ManagementBusiness ObjManagementBusiness = new FactoryBusiness().GetManagementBusiness(Version);
                if (ObjSessionValidation.IsSessionIdValid(SessionId))
                {
                    ReturnValue = ObjManagementBusiness.UpdateInterviewer(InterviewerDetail);
                }
                else
                {
                    ReturnValue.ReturnCode = 0;
                    ReturnValue.ReturnMessage = "Invalid session";
                    SICTLogger.WriteWarning(CLASS_NAME, FUNCTION_NAME, "Invalid session ");
                }
            }
            catch (Exception ex)
            {
                ReturnValue.ReturnCode = -1;
                ReturnValue.ReturnMessage = "Error in API Execution";
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, ex);
            }
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "End");
            return ReturnValue;
        }


        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetAllInterviewers/{Instance}/{Version}/{SessionId}")]
        public InterviewerDetailResponse GetAllInterviewers(string Instance, string Version, string SessionId)
        {
            const string FUNCTION_NAME = "GetAllInterviewers";
            UserDetailsBusiness ObjSessionValidation = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
            InterviewerDetailResponse InterviewerDetailResponse = new InterviewerDetailResponse();
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start for SessionId - " + SessionId);
            try
            {

                if (ObjSessionValidation.IsSessionIdValid(SessionId))
                {
                    ManagementBusiness ObjManagementBusiness = new FactoryBusiness().GetManagementBusiness(Version);
                    InterviewerDetailResponse = ObjManagementBusiness.GetAllInterviewers();
                }
                else
                {
                    InterviewerDetailResponse.ReturnCode = 0;
                    InterviewerDetailResponse.ReturnMessage = "Invalid session";
                    SICTLogger.WriteWarning(CLASS_NAME, FUNCTION_NAME, "Invalid session ");
                }
            }
            catch (Exception Ex)
            {
                InterviewerDetailResponse.ReturnCode = -1;
                InterviewerDetailResponse.ReturnMessage = "Error in API Execution";
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, Ex);
            }
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "End for SessionId- " + SessionId);
            return InterviewerDetailResponse;
        }


        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetInterviewersByRange/{Instance}/{Version}/{SessionId}/{StartIndex}/{Offset}/{AirportId}/{InterviewerName}")]
        public InterviewerDetailResponse GetInterviewersByRange(string Instance, string Version, string SessionId, string StartIndex, string Offset, string AirportId, string InterviewerName)
        {
            const string FUNCTION_NAME = "GetInterviewersByRange";
            UserDetailsBusiness ObjSessionValidation = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
            InterviewerDetailResponse InterviewerDetailResponse = new InterviewerDetailResponse();
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start for SessionId - " + SessionId);
            try
            {
                if (ObjSessionValidation.IsSessionIdValid(SessionId))
                {
                    ManagementBusiness ObjManagementBusiness = new FactoryBusiness().GetManagementBusiness(Version);
                    InterviewerDetailResponse = ObjManagementBusiness.GetInterviewersbyRange(Convert.ToInt32(StartIndex), Convert.ToInt32(Offset), AirportId, InterviewerName);
                }
                else
                {
                    InterviewerDetailResponse.ReturnCode = 0;
                    InterviewerDetailResponse.ReturnMessage = "Invalid session";
                    SICTLogger.WriteWarning(CLASS_NAME, FUNCTION_NAME, "Invalid session ");
                }
            }
            catch (Exception Ex)
            {
                InterviewerDetailResponse.ReturnCode = -1;
                InterviewerDetailResponse.ReturnMessage = "Error in API Execution";
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, Ex);
            }
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "End for SessionId- " + SessionId);
            return InterviewerDetailResponse;
        }
        #endregion Interviewer

        #region Target
        [WebInvoke(
          Method = "PUT",
          BodyStyle = WebMessageBodyStyle.WrappedRequest,
          RequestFormat = WebMessageFormat.Json,
          ResponseFormat = WebMessageFormat.Json,
          UriTemplate = "Target/Add")]
        public ReturnValue InsertTarget(string Instance, string SessionId, string Version, TargetDetail TargetDetail)
        {
            const string FUNCTION_NAME = "InserInterviewer";
            ReturnValue ReturnValue = new ReturnValue();
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start");
            try
            {
                UserDetailsBusiness ObjSessionValidation = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
                ManagementBusiness ObjManagementBusiness = new FactoryBusiness().GetManagementBusiness(Version);
                if (ObjSessionValidation.IsSessionIdValid(SessionId))
                {
                    ReturnValue = ObjManagementBusiness.InsertTarget(Instance, TargetDetail);
                }
                else
                {
                    ReturnValue.ReturnCode = 0;
                    ReturnValue.ReturnMessage = "Invalid session";
                    SICTLogger.WriteWarning(CLASS_NAME, FUNCTION_NAME, "Invalid session ");
                }
            }
            catch (Exception ex)
            {
                ReturnValue.ReturnCode = -1;
                ReturnValue.ReturnMessage = "Error in API Execution";
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, ex);
            }
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "End");
            return ReturnValue;
        }


        [WebInvoke(
          Method = "POST",
          BodyStyle = WebMessageBodyStyle.WrappedRequest,
          RequestFormat = WebMessageFormat.Json,
          ResponseFormat = WebMessageFormat.Json,
          UriTemplate = "Target/Update")]
        public ReturnValue UpdateTarget(string Instance, string SessionId, string Version, List<TargetUpdateDetail> TargetUpdateDetails)
        {
            const string FUNCTION_NAME = "UpdateTarget";
            ReturnValue ReturnValue = new ReturnValue();
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start");
            try
            {
                UserDetailsBusiness ObjSessionValidation = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
                ManagementBusiness ObjManagementBusiness = new FactoryBusiness().GetManagementBusiness(Version);
                if (ObjSessionValidation.IsSessionIdValid(SessionId))
                {
                    ReturnValue = ObjManagementBusiness.UpdateTarget(TargetUpdateDetails, Instance);
                }
                else
                {
                    ReturnValue.ReturnCode = 0;
                    ReturnValue.ReturnMessage = "Invalid session";
                    SICTLogger.WriteWarning(CLASS_NAME, FUNCTION_NAME, "Invalid session ");
                }
            }
            catch (Exception ex)
            {
                ReturnValue.ReturnCode = -1;
                ReturnValue.ReturnMessage = "Error in API Execution";
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, ex);
            }
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "End");
            return ReturnValue;
        }


        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetTargetsByRange/{Instance}/{Version}/{SessionId}/{StartIndex}/{Offset}/{FormType}/{OriginId}/{AirlineId}/{Route}/{Direction}/{FlightType}/{AircraftType}/{Search}/{Sort}/{IsAsc}")]
        public TargetDetailResponse GetTargetsByRange(string Instance, string Version, string SessionId, string StartIndex, string Offset, string FormType, string OriginId, string AirlineId, string Route, string Direction, string FlightType, string AircraftType, string Search, string Sort, string IsAsc)
        {
            const string FUNCTION_NAME = "GetInterviewersByRange";
            UserDetailsBusiness ObjSessionValidation = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
            TargetDetailResponse TargetDetailResponse = new TargetDetailResponse();
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start for SessionId - " + SessionId);
            try
            {
                //int a = (BusinessConstants.Instance)Instance.;
                if (ObjSessionValidation.IsSessionIdValid(SessionId))
                {
                    ManagementBusiness ObjManagementBusiness = new FactoryBusiness().GetManagementBusiness(Version);
                    TargetDetailResponse = ObjManagementBusiness.GetTargetsbyRange(Instance, Convert.ToInt32(StartIndex), Convert.ToInt32(Offset), FormType, OriginId, AirlineId, Route, Direction, FlightType, AircraftType, Search, Sort, IsAsc);
                }
                else
                {
                    TargetDetailResponse.ReturnCode = 0;
                    TargetDetailResponse.ReturnMessage = "Invalid session";
                    SICTLogger.WriteWarning(CLASS_NAME, FUNCTION_NAME, "Invalid session ");
                }
            }
            catch (Exception Ex)
            {
                TargetDetailResponse.ReturnCode = -1;
                TargetDetailResponse.ReturnMessage = "Error in API Execution";
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, Ex);
            }
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "End for SessionId- " + SessionId);
            return TargetDetailResponse;

        }

        [WebInvoke(
           Method = "DELETE",
           BodyStyle = WebMessageBodyStyle.WrappedRequest,
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json,
           UriTemplate = "Target/Delete")]
        public ReturnValue DeleteTarget(string Instance, string SessionId, string Version, string DistributionTargetId)
        {
            const string FUNCTION_NAME = "DeleteTarget";
            ReturnValue ReturnValue = new ReturnValue();
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start");
            try
            {
                UserDetailsBusiness ObjSessionValidation = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
                ManagementBusiness ObjManagementBusiness = new FactoryBusiness().GetManagementBusiness(Version);
                if (ObjSessionValidation.IsSessionIdValid(SessionId))
                {
                    ReturnValue = ObjManagementBusiness.DeleteTarget(Convert.ToInt32(DistributionTargetId), Instance);
                }
                else
                {
                    ReturnValue.ReturnCode = 0;
                    ReturnValue.ReturnMessage = "Invalid session";
                    SICTLogger.WriteWarning(CLASS_NAME, FUNCTION_NAME, "Invalid session ");
                }
            }
            catch (Exception ex)
            {
                ReturnValue.ReturnCode = -1;
                ReturnValue.ReturnMessage = "Error in API Execution";
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, ex);
            }
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "End");
            return ReturnValue;
        }

        #endregion Target
        #region FlightCombination

        [WebInvoke(
          Method = "PUT",
          BodyStyle = WebMessageBodyStyle.WrappedRequest,
          RequestFormat = WebMessageFormat.Json,
          ResponseFormat = WebMessageFormat.Json,
          UriTemplate = "FlightCombination/Add")]
        public FlightResponse InsertFlightCombination(string Instance, string SessionId, string Version, FlightDetail FlightDetail)
        {
            const string FUNCTION_NAME = "InsertFlightCombination";
            FlightResponse ReturnValue = new FlightResponse();
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start");
            try
            {
                UserDetailsBusiness ObjSessionValidation = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
                ManagementBusiness ObjManagementBusiness = new FactoryBusiness().GetManagementBusiness(Version);
                if (ObjSessionValidation.IsSessionIdValid(SessionId))
                {
                    ReturnValue = ObjManagementBusiness.InserFlightCombination(Instance, FlightDetail);
                }
                else
                {
                    ReturnValue.ReturnCode = 0;
                    ReturnValue.ReturnMessage = "Invalid session";
                    SICTLogger.WriteWarning(CLASS_NAME, FUNCTION_NAME, "Invalid session ");
                }
            }
            catch (Exception ex)
            {
                ReturnValue.ReturnCode = -1;
                ReturnValue.ReturnMessage = "Error in API Execution";
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, ex);
            }
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "End");
            return ReturnValue;


        }


        [WebInvoke(
          Method = "POST",
          BodyStyle = WebMessageBodyStyle.WrappedRequest,
          RequestFormat = WebMessageFormat.Json,
          ResponseFormat = WebMessageFormat.Json,
          UriTemplate = "FlightCombination/Update")]
        public FlightResponse UpdateFlightCombination(string Instance, string SessionId, string Version, FlightDetail FlightDetail)
        {
            const string FUNCTION_NAME = "UpdateFlightCombination";
            FlightResponse ReturnValue = new FlightResponse();
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start");
            try
            {
                UserDetailsBusiness ObjSessionValidation = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
                ManagementBusiness ObjManagementBusiness = new FactoryBusiness().GetManagementBusiness(Version);
                if (ObjSessionValidation.IsSessionIdValid(SessionId))
                {
                    ReturnValue = ObjManagementBusiness.UpdateFlightComination(Instance, FlightDetail);
                }
                else
                {
                    ReturnValue.ReturnCode = 0;
                    ReturnValue.ReturnMessage = "Invalid session";
                    SICTLogger.WriteWarning(CLASS_NAME, FUNCTION_NAME, "Invalid session ");
                }
            }
            catch (Exception ex)
            {
                ReturnValue.ReturnCode = -1;
                ReturnValue.ReturnMessage = "Error in API Execution";
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, ex);
            }
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "End");
            return ReturnValue;
        }

        [WebInvoke(
          Method = "DELETE",
          BodyStyle = WebMessageBodyStyle.WrappedRequest,
          RequestFormat = WebMessageFormat.Json,
          ResponseFormat = WebMessageFormat.Json,
          UriTemplate = "FlightCombination/Delete")]
        public FlightDeleteResponse DeleteFlightCombination(string Instance, string SessionId, string Version, string FlightCombinationId)
        {
            const string FUNCTION_NAME = "DeleteFlightCombination";
            FlightDeleteResponse ReturnValue = new FlightDeleteResponse();
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start");
            try
            {
                UserDetailsBusiness ObjSessionValidation = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
                ManagementBusiness ObjManagementBusiness = new FactoryBusiness().GetManagementBusiness(Version);
                if (ObjSessionValidation.IsSessionIdValid(SessionId))
                {
                    ReturnValue = ObjManagementBusiness.DeleteFlightCombination(Instance, Convert.ToInt32(FlightCombinationId));
                }
                else
                {
                    ReturnValue.ReturnCode = 0;
                    ReturnValue.ReturnMessage = "Invalid session";
                    SICTLogger.WriteWarning(CLASS_NAME, FUNCTION_NAME, "Invalid session ");
                }
            }
            catch (Exception ex)
            {
                ReturnValue.ReturnCode = -1;
                ReturnValue.ReturnMessage = "Error in API Execution";
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, ex);
            }
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "End");
            return ReturnValue;
        }

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetFlightCombinationsByRange/{Instance}/{Version}/{SessionId}/{StartIndex}/{Offset}/{FormType}/{OriginId}/{DestinationId}/{AirlineId}/{Route}/{Direction}/{FlightType}/{SearchVal=-1}/{SortVal=-1}/{SortOrder=null}")]
        public FlightCombinationResponse GetFlightCombinationsByRange(string Instance, string Version, string SessionId, string StartIndex, string Offset, string FormType, string OriginId, string DestinationId, string AirlineId, string Route, string Direction, string FlightType,string SearchVal,string SortVal,string SortOrder)
        {
            const string FUNCTION_NAME = "GetFlightCombinationsByRange";
            UserDetailsBusiness ObjSessionValidation = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
            FlightCombinationResponse FlightCombinationResponse = new FlightCombinationResponse();
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start for SessionId - " + SessionId);
            try
            {
                if (ObjSessionValidation.IsSessionIdValid(SessionId))
                {
                    ManagementBusiness ObjManagementBusiness = new FactoryBusiness().GetManagementBusiness(Version);
                    FlightCombinationResponse = ObjManagementBusiness.GetFlightCombinationsByRange(Instance, Convert.ToInt32(StartIndex), Convert.ToInt32(Offset), FormType, OriginId, DestinationId, AirlineId, Route, Direction, FlightType,SearchVal,SortVal, string.IsNullOrEmpty(SortOrder)?false:Convert.ToBoolean(SortOrder));
                }
                else
                {
                    FlightCombinationResponse.ReturnCode = 0;
                    FlightCombinationResponse.ReturnMessage = "Invalid session";
                    SICTLogger.WriteWarning(CLASS_NAME, FUNCTION_NAME, "Invalid session ");
                }
            }
            catch (Exception Ex)
            {
                FlightCombinationResponse.ReturnCode = 0;
                FlightCombinationResponse.ReturnMessage = "Error in API Execution";
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, Ex);
            }
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "End for SessionId- " + SessionId);
            return FlightCombinationResponse;

        }
        #endregion FlightCombination

        #region Confirmit

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetConfirmitCardDetails/{Instance}/{Version}/{SessionId}/{StartIndex}/{Offset}/{SearchVal}/{SortVal}/{SortOrder}")]
        public ConfirmitDetailsResponse GetConfirmitCardDetails(string Instance, string Version, string SessionId, string StartIndex, string Offset, string SearchVal, string SortVal, string SortOrder)
        {
            const string FUNCTION_NAME = "GetConfirmitCardDetails";
            UserDetailsBusiness ObjSessionValidation = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
            ConfirmitDetailsResponse ConfirmitDetailsResponse = new ConfirmitDetailsResponse();
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start for SessionId - " + SessionId);
            try
            {
                if (ObjSessionValidation.IsSessionIdValid(SessionId))
                {
                    ManagementBusiness ObjManagementBusiness = new FactoryBusiness().GetManagementBusiness(Version);
                    ConfirmitDetailsResponse = ObjManagementBusiness.GetConfirmitCardData(Convert.ToInt32(StartIndex), Convert.ToInt32(Offset), SearchVal, SortVal, Convert.ToBoolean(SortOrder));
                }
                else
                {
                    ConfirmitDetailsResponse.ReturnCode = 0;
                    ConfirmitDetailsResponse.ReturnMessage = "Invalid session";
                    SICTLogger.WriteWarning(CLASS_NAME, FUNCTION_NAME, "Invalid session ");
                }
            }
            catch (Exception Ex)
            {
                ConfirmitDetailsResponse.ReturnCode = -1;
                ConfirmitDetailsResponse.ReturnMessage = "Error in API Execution";
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, Ex);
            }
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "End for SessionId- " + SessionId);
            return ConfirmitDetailsResponse;
        }


        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetConfirmitCounts/{Instance}/{Version}/{SessionId}")]
        public ConfirmitCountsResponse GetConfirmitCounts(string Instance, string Version, string SessionId)
        {
            const string FUNCTION_NAME = "GetConfirmitCardDetails";
            UserDetailsBusiness ObjSessionValidation = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
            ConfirmitCountsResponse ConfirmitCountsResponse = new ConfirmitCountsResponse();
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "Start for SessionId - " + SessionId);
            try
            {
                if (ObjSessionValidation.IsSessionIdValid(SessionId))
                {
                    ManagementBusiness ObjManagementBusiness = new FactoryBusiness().GetManagementBusiness(Version);
                    ConfirmitCountsResponse = ObjManagementBusiness.GetConfirmitCounts();
                }
                else
                {
                    ConfirmitCountsResponse.ReturnCode = 0;
                    ConfirmitCountsResponse.ReturnMessage = "Invalid session";
                    SICTLogger.WriteWarning(CLASS_NAME, FUNCTION_NAME, "Invalid session ");
                }
            }
            catch (Exception Ex)
            {
                ConfirmitCountsResponse.ReturnCode = -1;
                ConfirmitCountsResponse.ReturnMessage = "Error in API Execution";
                SICTLogger.WriteException(CLASS_NAME, FUNCTION_NAME, Ex);
            }
            SICTLogger.WriteInfo(CLASS_NAME, FUNCTION_NAME, "End for SessionId- " + SessionId);
            return ConfirmitCountsResponse;
        }

        #endregion Confirmit

        #endregion MasterPageImplementation
    }

}

