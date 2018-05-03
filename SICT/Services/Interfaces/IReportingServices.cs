using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using SICT.DataContracts;

namespace SICT.Interface
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IReportingServices" in both code and config file together.
    [ServiceContract]
    public interface IReportingServices
    {
        [OperationContract]
        InterviewerReportResponse GetInterviewerReport(string Instance, string Version, string SessionId, string AirportId, string OriginId, string DestinationId, string InterviewerId, string AirlineId, string FormType, string Route, string Direction, string FlightType, string AircraftType, string StartDate, string EndDate, string ResponseDate);

        [OperationContract]
        DODReportResponse GetDODReport(string Instance, string Version, string SessionId, string AirportId, string OriginId, string DestinationId, string AirlineId, string FormType, string InterviewerId, string Route, string Direction, string FlightType, string AircraftType, string StartDate, string EndDate, string ResponseDate);

        [OperationContract]
        FlightReportResponse GetFlightReport(string Instance, string Version, string SessionId, string AirportId, string OriginId, string DestinationId, string AirlineId, string FormType, string InterviewerId, string Route, string Direction, string FlightType, string AircraftType, string StartDate, string EndDate, string ResponseDate);

        [OperationContract]
        AirlineReportResponse GetAirlineReport(string Instance, string Version, string SessionId, string AirportId, string OriginId, string DestinationId, string FormType, string InterviewerId, string Route, string Direction, string FlightType, string AircraftType, string StartDate, string EndDate, string AirlineId, string ResponseDate);

        [OperationContract]
        AirportReportResponse GetAirportReport(string Instance, string Version, string SessionId, string AirportId, string OriginId, string DestinationId, string AirlineId, string FormType, string InterviewerId, string Route, string Direction, string FlightType, string AircraftType, string StartDate, string EndDate, string ResponseDate);

        [OperationContract]
        AircraftReportResponse GetAircraftReport(string Instance, string Version, string SessionId, string AirportId, string OriginId, string DestinationId, string FormType, string InterviewerId, string StartDate, string EndDate, string ResponseDate);

        [OperationContract]
        AircraftQuotaReportResponse GetAircraftQuotaReport(string Instance, string Version, string SessionId, string AirportId, string StartDate, string EndDate, string IsBusinessQuota);

        [OperationContract]
        AirlineReportResponse GetAllAirlineReport(string Instance, string Version, string SessionId, string AirportId, string OriginId, string DestinationId, string FormType, string InterviewerId, string Route, string Direction, string FlightType, string AircraftType, string StartDate, string EndDate, string AirlineId, string ResponseDate);

    }
}
