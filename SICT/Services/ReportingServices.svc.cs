using SICT.BusinessLayer.V1;
using SICT.CommonBusiness;
using SICT.Constants;
using SICT.DataContracts;
using SICT.Interface;
using System;
using System.ServiceModel.Web;

namespace SICT.Service
{
    public class ReportingServices : IReportingServices
    {
        private static readonly string CLASS_NAME = "ReportingServices";

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetInterviewerReport/{Instance}/{Version}/{SessionId}/{AirportId}/{OriginId}/{DestinationId}/{InterviewerId}/{AirlineId}/{FormType}/{Route}/{Direction}/{FlightType}/{AircraftType}/{StartDate}/{EndDate}/{ResponseDate}")]
        public InterviewerReportResponse GetInterviewerReport(string Instance, string Version, string SessionId, string AirportId, string OriginId, string DestinationId, string InterviewerId, string AirlineId, string FormType, string Route, string Direction, string FlightType, string AircraftType, string StartDate, string EndDate, string ResponseDate)
        {
            UserDetailsBusiness userDetailsBusiness = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
            InterviewerReportResponse interviewerReportResponse = new InterviewerReportResponse();
            SICTLogger.WriteInfo(ReportingServices.CLASS_NAME, "GetInterviewerReport", "Start ");
            try
            {
                if (userDetailsBusiness.IsSessionIdValid(SessionId, false))
                {
                    ReportingBusiness reportingBusiness = new FactoryBusiness().GetReportingBusiness(Version);
                    interviewerReportResponse = reportingBusiness.GetInterviewerReport(SessionId, AirportId, OriginId, DestinationId, InterviewerId, AirlineId, FormType, Route, Direction, FlightType, AircraftType, StartDate, EndDate, ResponseDate);
                }
                else
                {
                    interviewerReportResponse.ReturnCode = 0;
                    interviewerReportResponse.ReturnMessage = "Invalid session";
                    SICTLogger.WriteWarning(ReportingServices.CLASS_NAME, "GetInterviewerReport", "Invalid session ");
                }
            }
            catch (Exception ex)
            {
                interviewerReportResponse.ReturnCode = -1;
                interviewerReportResponse.ReturnMessage = "Error in API Execution";
                SICTLogger.WriteException(ReportingServices.CLASS_NAME, "GetInterviewerReport", ex);
            }
            SICTLogger.WriteInfo(ReportingServices.CLASS_NAME, "GetInterviewerReport", "End ");
            return interviewerReportResponse;
        }

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetDODReport/{Instance}/{Version}/{SessionId}/{AirportId}/{OriginId}/{DestinationId}/{AirlineId}/{FormType}/{InterviewerId}/{Route}/{Direction}/{FlightType}/{AircraftType}/{StartDate}/{EndDate}/{ResponseDate}")]
        public DODReportResponse GetDODReport(string Instance, string Version, string SessionId, string AirportId, string OriginId, string DestinationId, string AirlineId, string FormType, string InterviewerId, string Route, string Direction, string FlightType, string AircraftType, string StartDate, string EndDate, string ResponseDate)
        {
            UserDetailsBusiness userDetailsBusiness = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
            DODReportResponse dODReportResponse = new DODReportResponse();
            SICTLogger.WriteInfo(ReportingServices.CLASS_NAME, "GetDODReport", "Start ");
            try
            {
                if (userDetailsBusiness.IsSessionIdValid(SessionId, false))
                {
                    ReportingBusiness reportingBusiness = new FactoryBusiness().GetReportingBusiness(Version);
                    dODReportResponse = reportingBusiness.GetDODReport(SessionId, AirportId, OriginId, DestinationId, AirlineId, FormType, Route, Direction, FlightType, AircraftType, InterviewerId, StartDate, EndDate, ResponseDate);
                }
                else
                {
                    dODReportResponse.ReturnCode = 0;
                    dODReportResponse.ReturnMessage = "Invalid session";
                    SICTLogger.WriteWarning(ReportingServices.CLASS_NAME, "GetDODReport", "Invalid session ");
                }
            }
            catch (Exception ex)
            {
                dODReportResponse.ReturnCode = -1;
                dODReportResponse.ReturnMessage = "Error in API Execution";
                SICTLogger.WriteException(ReportingServices.CLASS_NAME, "GetDODReport", ex);
            }
            SICTLogger.WriteInfo(ReportingServices.CLASS_NAME, "GetDODReport", "End ");
            return dODReportResponse;
        }

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetFlightReport/{Instance}/{Version}/{SessionId}/{AirportId}/{OriginId}/{DestinationId}/{AirlineId}/{FormType}/{InterviewerId}/{Route}/{Direction}/{FlightType}/{AircraftType}/{StartDate}/{EndDate}/{ResponseDate}")]
        public FlightReportResponse GetFlightReport(string Instance, string Version, string SessionId, string AirportId, string OriginId, string DestinationId, string AirlineId, string FormType, string InterviewerId, string Route, string Direction, string FlightType, string AircraftType, string StartDate, string EndDate, string ResponseDate)
        {
            UserDetailsBusiness userDetailsBusiness = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
            FlightReportResponse flightReportResponse = new FlightReportResponse();
            SICTLogger.WriteInfo(ReportingServices.CLASS_NAME, "GetFlightReport", "Start ");
            try
            {
                if (userDetailsBusiness.IsSessionIdValid(SessionId, false))
                {
                    ReportingBusiness reportingBusiness = new FactoryBusiness().GetReportingBusiness(Version);
                    flightReportResponse = reportingBusiness.GetFlightReport(Instance, SessionId, AirportId, OriginId, DestinationId, AirlineId, FormType, Route, Direction, FlightType, AircraftType, InterviewerId, StartDate, EndDate, ResponseDate);
                }
                else
                {
                    flightReportResponse.ReturnCode = 0;
                    flightReportResponse.ReturnMessage = "Invalid session";
                    SICTLogger.WriteWarning(ReportingServices.CLASS_NAME, "GetFlightReport", "Invalid session ");
                }
            }
            catch (Exception ex)
            {
                flightReportResponse.ReturnCode = -1;
                flightReportResponse.ReturnMessage = "Error in API Execution";
                SICTLogger.WriteException(ReportingServices.CLASS_NAME, "GetFlightReport", ex);
            }
            SICTLogger.WriteInfo(ReportingServices.CLASS_NAME, "GetFlightReport", "End ");
            return flightReportResponse;
        }

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetAirlineReport/{Instance}/{Version}/{SessionId}/{AirportId}/{OriginId}/{DestinationId}/{FormType}/{InterviewerId}/{Route}/{Direction}/{FlightType}/{AircraftType}/{StartDate}/{EndDate}/{AirlineId}/{ResponseDate}")]
        public AirlineReportResponse GetAirlineReport(string Instance, string Version, string SessionId, string AirportId, string OriginId, string DestinationId, string FormType, string InterviewerId, string Route, string Direction, string FlightType, string AircraftType, string StartDate, string EndDate, string AirlineId, string ResponseDate)
        {
            UserDetailsBusiness userDetailsBusiness = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
            AirlineReportResponse airlineReportResponse = new AirlineReportResponse();
            SICTLogger.WriteInfo(ReportingServices.CLASS_NAME, "GetAirlineReport", "Start ");
            try
            {
                if (userDetailsBusiness.IsSessionIdValid(SessionId, false))
                {
                    ReportingBusiness reportingBusiness = new FactoryBusiness().GetReportingBusiness(Version);
                    airlineReportResponse = reportingBusiness.GetAirlineReport(Instance, SessionId, AirportId, OriginId, DestinationId, FormType, Route, Direction, FlightType, AircraftType, InterviewerId, StartDate, EndDate, AirlineId, ResponseDate);
                }
                else
                {
                    airlineReportResponse.ReturnCode = 0;
                    airlineReportResponse.ReturnMessage = "Invalid session";
                    SICTLogger.WriteWarning(ReportingServices.CLASS_NAME, "GetAirlineReport", "Invalid session ");
                }
            }
            catch (Exception ex)
            {
                airlineReportResponse.ReturnCode = -1;
                airlineReportResponse.ReturnMessage = "Error in API Execution";
                SICTLogger.WriteException(ReportingServices.CLASS_NAME, "GetAirlineReport", ex);
            }
            SICTLogger.WriteInfo(ReportingServices.CLASS_NAME, "GetAirlineReport", "End ");
            return airlineReportResponse;
        }

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetAirportReport/{Instance}/{Version}/{SessionId}/{AirportId}/{OriginId}/{DestinationId}/{AirlineId}/{FormType}/{InterviewerId}/{Route}/{Direction}/{FlightType}/{AircraftType}/{StartDate}/{EndDate}/{ResponseDate}")]
        public AirportReportResponse GetAirportReport(string Instance, string Version, string SessionId, string AirportId, string OriginId, string DestinationId, string AirlineId, string FormType, string InterviewerId, string Route, string Direction, string FlightType, string AircraftType, string StartDate, string EndDate, string ResponseDate)
        {
            UserDetailsBusiness userDetailsBusiness = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
            AirportReportResponse airportReportResponse = new AirportReportResponse();
            SICTLogger.WriteInfo(ReportingServices.CLASS_NAME, "GetAirportReport", "Start ");
            try
            {
                if (userDetailsBusiness.IsSessionIdValid(SessionId, false))
                {
                    ReportingBusiness reportingBusiness = new FactoryBusiness().GetReportingBusiness(Version);
                    airportReportResponse = reportingBusiness.GetAirportReport(Instance, SessionId, AirportId, OriginId, DestinationId, AirlineId, FormType, Route, Direction, FlightType, AircraftType, InterviewerId, StartDate, EndDate, ResponseDate);
                }
                else
                {
                    airportReportResponse.ReturnCode = 0;
                    airportReportResponse.ReturnMessage = "Invalid session";
                    SICTLogger.WriteWarning(ReportingServices.CLASS_NAME, "GetAirportReport", "Invalid session ");
                }
            }
            catch (Exception ex)
            {
                airportReportResponse.ReturnCode = -1;
                airportReportResponse.ReturnMessage = "Error in API Execution";
                SICTLogger.WriteException(ReportingServices.CLASS_NAME, "GetAirportReport", ex);
            }
            SICTLogger.WriteInfo(ReportingServices.CLASS_NAME, "GetAirportReport", "End ");
            return airportReportResponse;
        }

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetAircraftReport/{Instance}/{Version}/{SessionId}/{AirportId}/{OriginId}/{DestinationId}/{FormType}/{InterviewerId}/{StartDate}/{EndDate}/{ResponseDate}")]
        public AircraftReportResponse GetAircraftReport(string Instance, string Version, string SessionId, string AirportId, string OriginId, string DestinationId, string FormType, string InterviewerId, string StartDate, string EndDate, string ResponseDate)
        {
            UserDetailsBusiness userDetailsBusiness = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
            AircraftReportResponse aircraftReportResponse = new AircraftReportResponse();
            SICTLogger.WriteInfo(ReportingServices.CLASS_NAME, "GetAircraftReport", "Start ");
            try
            {
                if (userDetailsBusiness.IsSessionIdValid(SessionId, false))
                {
                    ReportingBusiness reportingBusiness = new FactoryBusiness().GetReportingBusiness(Version);
                    aircraftReportResponse = reportingBusiness.GetAircraftReport(Instance, SessionId, AirportId, OriginId, DestinationId, FormType, InterviewerId, StartDate, EndDate, ResponseDate);
                }
                else
                {
                    aircraftReportResponse.ReturnCode = 0;
                    aircraftReportResponse.ReturnMessage = "Invalid session";
                    SICTLogger.WriteWarning(ReportingServices.CLASS_NAME, "GetAircraftReport", "Invalid session ");
                }
            }
            catch (Exception ex)
            {
                aircraftReportResponse.ReturnCode = -1;
                aircraftReportResponse.ReturnMessage = "Error in API Execution";
                SICTLogger.WriteException(ReportingServices.CLASS_NAME, "GetAircraftReport", ex);
            }
            SICTLogger.WriteInfo(ReportingServices.CLASS_NAME, "GetAircraftReport", "End ");
            return aircraftReportResponse;
        }

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetAircraftQuotaReport/{Instance}/{Version}/{SessionId}/{AirportId}/{StartDate}/{EndDate}/{IsBusinessQuota}")]
        public AircraftQuotaReportResponse GetAircraftQuotaReport(string Instance, string Version, string SessionId, string AirportId, string StartDate, string EndDate, string IsBusinessQuota)
        {
            UserDetailsBusiness userDetailsBusiness = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
            AircraftQuotaReportResponse aircraftQuotaReportResponse = new AircraftQuotaReportResponse();
            SICTLogger.WriteInfo(ReportingServices.CLASS_NAME, "GetAircraftQuotaReport", "Start ");
            try
            {
                if (userDetailsBusiness.IsSessionIdValid(SessionId, false))
                {
                    ReportingBusiness reportingBusiness = new FactoryBusiness().GetReportingBusiness(Version);
                    aircraftQuotaReportResponse = reportingBusiness.GetAircraftQuotaReport(Instance, SessionId, AirportId, StartDate, EndDate, Convert.ToBoolean(IsBusinessQuota));
                }
                else
                {
                    aircraftQuotaReportResponse.ReturnCode = 0;
                    aircraftQuotaReportResponse.ReturnMessage = "Invalid session";
                    SICTLogger.WriteWarning(ReportingServices.CLASS_NAME, "GetAircraftQuotaReport", "Invalid session ");
                }
            }
            catch (Exception ex)
            {
                aircraftQuotaReportResponse.ReturnCode = -1;
                aircraftQuotaReportResponse.ReturnMessage = "Error in API Execution";
                SICTLogger.WriteException(ReportingServices.CLASS_NAME, "GetAircraftQuotaReport", ex);
            }
            SICTLogger.WriteInfo(ReportingServices.CLASS_NAME, "GetAircraftQuotaReport", "End ");
            return aircraftQuotaReportResponse;
        }

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetAllAirlineReport/{Instance}/{Version}/{SessionId}/{AirportId}/{OriginId}/{DestinationId}/{FormType}/{InterviewerId}/{Route}/{Direction}/{FlightType}/{AircraftType}/{StartDate}/{EndDate}/{AirlineId}/{ResponseDate}")]
        public AirlineReportResponse GetAllAirlineReport(string Instance, string Version, string SessionId, string AirportId, string OriginId, string DestinationId, string FormType, string InterviewerId, string Route, string Direction, string FlightType, string AircraftType, string StartDate, string EndDate, string AirlineId, string ResponseDate)
        {
            UserDetailsBusiness userDetailsBusiness = new FactoryBusiness().GetUserDetailsBusiness(BusinessConstants.VERSION_BASE);
            AirlineReportResponse airlineReportResponse = new AirlineReportResponse();
            SICTLogger.WriteInfo(ReportingServices.CLASS_NAME, "GetAirlineReport", "Start ");
            try
            {
                if (userDetailsBusiness.IsSessionIdValid(SessionId, false))
                {
                    ReportingBusiness reportingBusiness = new FactoryBusiness().GetReportingBusiness(Version);
                    airlineReportResponse = reportingBusiness.GetAllAirlineReport(Instance, SessionId, AirportId, OriginId, DestinationId, FormType, Route, Direction, FlightType, AircraftType, InterviewerId, StartDate, EndDate, AirlineId, ResponseDate);
                }
                else
                {
                    airlineReportResponse.ReturnCode = 0;
                    airlineReportResponse.ReturnMessage = "Invalid session";
                    SICTLogger.WriteWarning(ReportingServices.CLASS_NAME, "GetAirlineReport", "Invalid session ");
                }
            }
            catch (Exception ex)
            {
                airlineReportResponse.ReturnCode = -1;
                airlineReportResponse.ReturnMessage = "Error in API Execution";
                SICTLogger.WriteException(ReportingServices.CLASS_NAME, "GetAirlineReport", ex);
            }
            SICTLogger.WriteInfo(ReportingServices.CLASS_NAME, "GetAirlineReport", "End ");
            return airlineReportResponse;
        }
    }
}
